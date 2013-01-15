﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Telonics
{
    /// <summary>
    /// Provides access to data on the Argos Web Server
    /// </summary>
    static public class ArgosWebSite
    {

        /// <summary>
        /// Provides the results (in a modified CSV format) from the Argos web server
        /// This object is immutable, and should only be created by an ArgosWebsite object
        /// Thhis object encapsulates the representation of the results.
        /// </summary>
        public class ArgosWebResult
        {
            private readonly string _text;

            //We can't make the constructor private (ArgosWebSite can't create it)
            //The best we can do is make it internal, and make sure no other methods in the
            //library call the constructor.
            internal ArgosWebResult(string text)
            {
                if (String.IsNullOrEmpty(text))
                    throw new ArgumentNullException("text");
                _text = text;
            }

            /// <summary>
            /// Returns the Argos web results as a single string
            /// </summary>
            /// <returns>string</returns>
            public override string ToString()
            {
                return _text;
            }

            /// <summary>
            /// Returns the Argos web results as an array of UTF8 bytes.
            /// </summary>
            /// <returns>Byte Array</returns>
            public Byte[] ToBytes()
            {
                var e = new UTF8Encoding();
                return e.GetBytes(_text);
            }
        }

        private const string _argosUrl = @"http://ws-argos.clsamerica.com/argosDws/services/DixService";

        //Argos soap request for messages as CSV
        private const string _csvRequest = @"
<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:argos=""http://service.dataxmldistribution.argos.cls.fr/types"">
<soap:Header/>
<soap:Body>
<argos:csvRequest>
<argos:username>{0}</argos:username>
<argos:password>{1}</argos:password>
{2}
<argos:nbDaysFromNow>{3}</argos:nbDaysFromNow>
<argos:displayLocation>true</argos:displayLocation>
<argos:displayDiagnostic>true</argos:displayDiagnostic>
<argos:displayRawData>true</argos:displayRawData>
<argos:displayImageLocation>true</argos:displayImageLocation>
<argos:displayHexId>true</argos:displayHexId>
<argos:showHeader>true</argos:showHeader>
</argos:csvRequest>
</soap:Body>
</soap:Envelope>
";
        //replace {2} with either platform or program
        //Use the programNumber to get all the collars in a program
        //Use platform to get a specific collar (platformId = ArgosId)
        private const string _platform = "<argos:platformId>{0}</argos:platformId>";
        private const string _program = "<argos:programNumber>{0}</argos:programNumber>";


        /// <summary>
        /// Queries the Argos Web Services, and returns the results for a collar.
        /// </summary>
        /// <param name="username">A user name assigned by the Argos Web Service</param>
        /// <param name="password">The user's password</param>
        /// <param name="collar">A collar (platform) identifier in the user's account</param>
        /// <param name="days">Number of days in the past to retrieve (1 to 10)</param>
        /// <param name="error">Contains any errors encountered; null with no errors</param>
        /// <returns>Returns the results from the web server.  If null check the error output parameter</returns>
        static public ArgosWebResult GetCollar(string username, string password, string collar, int days, out string error)
        {
            error = CheckParameters(username, password, collar, days);
            if (error != null)
                return null;
            var selector = String.Format(_platform, collar);
            var request = String.Format(_csvRequest, username, password, selector, days);
            return GetCsv(request, out error);
        }

        /// <summary>
        /// Queries the Argos Web Services, and returns the results for a program (collection of collars).
        /// </summary>
        /// <param name="username">A user name assigned by the Argos Web Service</param>
        /// <param name="password">The user's password</param>
        /// <param name="program">A program identifier in the user's account</param>
        /// <param name="days">Number of days in the past to retrieve (1 to 10)</param>
        /// <param name="error">Contains any errors encountered; null with no errors</param>
        /// <returns>Returns the results from the web server.  If null check the error output parameter</returns>
        static public ArgosWebResult GetProgram(string username, string password, string program, int days, out string error)
        {
            error = CheckParameters(username, password, program, days);
            if (error != null)
                return null;
            var selector = String.Format(_program, program);
            var request = String.Format(_csvRequest, username, password, selector, days);
            return GetCsv(request, out error);
        }

        static private string CheckParameters(string username, string password, string selector, int days)
        {
            string error = null;
            if (String.IsNullOrEmpty(username))
                error = "No username provided";
            if (String.IsNullOrEmpty(password))
            {
                const string msg = "No password provided";
                if (error == null)
                    error = msg;
                else
                    error += "; " + msg;
            }
            if (String.IsNullOrEmpty(selector))
            {
                const string msg = "No selector (collar or program) was provided";
                if (error == null)
                    error = msg;
                else
                    error += "; " + msg;
            }
            if (days < 1 || days > 10)
            {
                const string msg = "Days out of range (1..10)";
                if (error == null)
                    error = msg;
                else
                    error += "; " + msg;
            }
            return error;
        }

        static private ArgosWebResult GetCsv(string request, out string error)
        {
            error = null;
            try
            {
                HttpWebRequest req = GetRequest(request);
                string response = GetResponse(req);
                if (String.IsNullOrEmpty(response))
                    error = "No response";
                else if (response == "<errors><error code=\"4\">no data</error></errors>")
                    error = "No data:  unknown program/collar, or no fixes in requested time period";
                else if (response == "<errors><error code=\"3\">authentification error</error></errors>")
                    error = "Authentication Error: bad username or password";
                else if (response.Contains("<errors>"))
                    error = "Unknown error: " + response;
                if (error != null)
                    return null;
                return new ArgosWebResult(response);
            }
            catch (Exception ex)
            {
                error = "EXCEPTION\n\n" + ex.Message;
                return null;
            }
        }

        static private HttpWebRequest GetRequest(string request)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(request);
            var req = WebRequest.Create(_argosUrl);
            //req.ContentType = "application/soap+xml; charset=utf-8";
            req.Method = "POST";
            req.ContentLength = buffer.Length;
            var sw = req.GetRequestStream();
            sw.Write(buffer, 0, buffer.Length);
            sw.Close();
            return (HttpWebRequest)req;
        }

        static private string GetResponse(HttpWebRequest request)
        {
            var resp = (HttpWebResponse)request.GetResponse();
            var respStream = resp.GetResponseStream();
            if (respStream == null)
                throw new ApplicationException("Null response from Web Service");
            string response;
            using (var stream = new StreamReader(respStream))
            {
                response = stream.ReadToEnd();
            }
            int start = response.IndexOf("<return>", StringComparison.Ordinal) + 8;
            int end = response.IndexOf("</return>", StringComparison.Ordinal);
            response = response.Substring(start, end - start);
            return HttpUtility.HtmlDecode(response);
        }
    }
}