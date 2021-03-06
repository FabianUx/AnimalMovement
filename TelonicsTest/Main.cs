using System;
using System.Collections.Generic;
using System.IO;
using Telonics;
using System.Linq;

namespace TelonicsTest
{
    static class MainClass
    {

        public static void Main()
        {
            //TestCrc();
            //TestArgosEmailFile();
            //TestArgosAwsGen3File();
            //TestArgosAwsGen4File();
            //TestBits();
            //TestArgosFolder();
            TestGetPlatformList();
        }

        public static void TestGetPlatformList()
        {
            string error;
            var result = ArgosWebSite.GetPlatformList("xxx", "xxx", out error);
            if (error != null)
                Console.WriteLine(error);
            else
                foreach (var tuple in result)
                    Console.WriteLine("Program: {0}, Platform: {1}", tuple.Item1, tuple.Item2);
        }

        public static void TestArgosEmailFile()
        {
            const string path = @"..\..\SampleFiles\Gen34moose08-09-2-12.TXT";
            const string tpf = @"..\..\SampleFiles\080711003_87744.tpf";
            Console.WriteLine("File {0}", path);

            var processorDict = new Dictionary<string, IProcessor>
                {
                    {"77267", new Gen3Processor(TimeSpan.FromMinutes(24*60))},
                    {"87744", new Gen4Processor(File.ReadAllBytes(tpf))},
                };
            ArgosFile a = new ArgosEmailFile(path);
            SummarizeFile(a);
            foreach (var id in new[] {"77267", "87744"})
            {
                Console.WriteLine("Messages for {0} in File", id);
                var platform = id; // to protect against AccessToForEachVariableInClosure
                var transmissions = a.GetTransmissions().Where(t => t.PlatformId == platform);
                foreach (var s in processorDict[id].ProcessTransmissions(transmissions, a))
                    Console.WriteLine(s);
            }
        }

        public static void TestArgosAwsGen3File()
        {
            const string path = @"..\..\SampleFiles\53478_20130129_Gen3.aws";
            Console.WriteLine("File {0}", path);
            ArgosFile a = new ArgosAwsFile(path);
            var processor = new Gen3Processor(TimeSpan.FromMinutes(24*60));
            SummarizeFile(a);
            Console.WriteLine("Messages in File");
            foreach (var s in processor.ProcessTransmissions(a.GetTransmissions(), a))
                Console.WriteLine(s);
        }


        public static void TestArgosAwsGen4File()
        {
            const string path = @"..\..\SampleFiles\37780 20121219_231239_Gen4.aws";
            const string tpf = @"..\..\SampleFiles\100628007B_37780.tpf";
            Console.WriteLine("File {0}", path);
            ArgosFile a = new ArgosAwsFile(path);
            var processor = new Gen4Processor(File.ReadAllBytes(tpf));
            SummarizeFile(a);
            Console.WriteLine("Messages in File");
            foreach (var s in processor.ProcessTransmissions(a.GetTransmissions(), a))
                Console.WriteLine(s);
        }

        private static void SummarizeFile(ArgosFile a)
        {
            Console.WriteLine("Transmissions in File");
            foreach (var t in a.GetTransmissions())
                Console.WriteLine(t.ToFormatedString());
            Console.WriteLine("Programs in File");
            foreach (var p in a.GetPrograms())
                Console.WriteLine("  {0}", p);
            Console.WriteLine("Collars in File");
            foreach (var p in a.GetPlatforms())
                Console.WriteLine("  {0} Start {1} End {2}", p, a.FirstTransmission(p), a.LastTransmission(p));
        }

        public static void TestArgosFolder()
        {
            const string id = "60793";
            const int hours = 25;

            const string inPath = @"C:\tmp\Argos_Emails";
            const string outPath = @"C:\tmp\reports\" + id + "_2012a.txt";

            if (!Directory.Exists(inPath))
            {
                Console.Write("Invalid Directory {0}", inPath);
            }
            Console.Write("Processing Directory {0}", inPath);
            using (var f = new StreamWriter(outPath))
            {
                foreach (var file in Directory.EnumerateFiles(inPath))
                {
                    var path = Path.Combine(inPath, file);
                    Console.WriteLine("  File {0}", file);
                    ArgosFile a = new ArgosEmailFile(path);
                    var processor = new Gen3Processor(TimeSpan.FromMinutes(hours*60));
                    //CollarFinder = (i, d) => i
                    var lines = new string[0]; 
                    try
                    {
                        var transmissions = a.GetTransmissions().Where(t => t.PlatformId == id);
                        lines = processor.ProcessTransmissions(transmissions, a).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: InTelonicsToData(): {0}", ex.Message);
                    }
                    foreach (var l in lines)
                        f.WriteLine(l);
                }
            }
        }

        /* Simple Test Program for 6-bit CRC generation algorithm for T03 Format. */
        /****************************** Definitions ********************************/
        struct Field
        {
            public Field(int value, int length)
                : this()
            {
                Value = value;
                Length = length;
            }
            public int Value { get; private set; }
            public int Length { get; private set; }
        }

        public static void TestCrc()
        {
            Field[] testCase =
                {
                    new Field(4, 3),
                    new Field(10, 4),
                    new Field(30, 5),
                    new Field(55, 6),
                    new Field(111, 7),
                    new Field(222, 8),
                    new Field(950, 10),
                    new Field(3999, 12)
                };

            // Display test values
            Console.WriteLine("\nTest case input values in decimal and binary:");
            for (int i = 0; i < 8; i++)
            {
                string binaryString = Convert.ToString(testCase[i].Value, 2);
                Console.WriteLine("{0} {1,4} {2,2} {3,12}",
                                  i + 1, testCase[i].Value, testCase[i].Length, binaryString);
            }

            // Calculate CRC value
            var crc = new Crc();
            for (int i = 0; i < 8; i++)
                crc.Update(testCase[i].Value, testCase[i].Length);

            // Display CRC value
            int a = crc.Value; 	// Get final CRC value
            string binaryStr = Convert.ToString(a, 2);
            Console.WriteLine("\n6-bit CRC of all fields = {0:X} (hex) = {1} (binary)", a, binaryStr);
            Console.WriteLine("Done.");
        }

        public static void TestBits()
        {
            Byte[] bytes = { 127, 255, 255, 255, 123, 23, 210, 3, 0 };
            foreach (var b in bytes)
            {
                var s = Convert.ToString(b, 2).PadLeft(8, '0');
                Console.WriteLine(s);
            }
            Console.WriteLine("Bit at {0} is {1}", 0, bytes.BooleanAt(0));
            Console.WriteLine("Bit at {0} is {1}", 1, bytes.BooleanAt(1));
            Console.WriteLine("Bit at {0} is {1}", 31, bytes.BooleanAt(31));
            Console.WriteLine("Bit at {0} is {1}", 32, bytes.BooleanAt(32));

            Console.WriteLine("Byte at {0},{1} is {2}", 0, 3, bytes.ByteAt(0, 3));
            Console.WriteLine("Byte at {0},{1} is {2}", 1, 3, bytes.ByteAt(1, 3));
            Console.WriteLine("Byte at {0},{1} is {2}", 2, 3, bytes.ByteAt(2, 3));
            Console.WriteLine("Byte at {0},{1} is {2}", 37, 8, bytes.ByteAt(37, 8));
            Console.WriteLine("UInt16 at {0},{1} is {2}", 37, 16, bytes.UInt16At(37, 16));
            Console.WriteLine("Uint32 at {0},{1} is {2}", 37, 32, bytes.UInt32At(37, 32));

            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 0, 22, 4, bytes.UInt32At(0, 22).ToSignedBinary(22, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 1, 22, 4, bytes.UInt32At(1, 22).ToSignedBinary(22, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 0, 21, 4, bytes.UInt32At(0, 21).ToSignedBinary(21, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 1, 21, 4, bytes.UInt32At(1, 21).ToSignedBinary(21, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 0, 17, 4, bytes.UInt32At(0, 17).ToSignedBinary(17, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 1, 17, 4, bytes.UInt32At(1, 17).ToSignedBinary(17, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 0, 12, 4, bytes.UInt32At(0, 12).ToSignedBinary(12, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 1, 12, 4, bytes.UInt32At(1, 12).ToSignedBinary(12, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 0, 9, 4, bytes.UInt32At(0, 9).ToSignedBinary(9, 4));
            Console.WriteLine("SignedBinary at {0},{1}/{2} is {3}", 1, 9, 4, bytes.UInt32At(1, 9).ToSignedBinary(9, 4));
            Console.WriteLine("TwosComplement at {0},{1}/{2} is {3}", 0, 9, 4, bytes.UInt32At(0, 9).TwosComplement(9, 4));
            Console.WriteLine("TwosComplement at {0},{1}/{2} is {3}", 1, 9, 4, bytes.UInt32At(1, 9).TwosComplement(9, 4));
            Console.WriteLine("TwosComplement at {0},{1}/{2} is {3}", 63, 9, 4, bytes.UInt32At(63, 9).TwosComplement(9, 4));
        }
    }
}
