﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Animal_Movement")]
	public partial class AnimalMovementViews : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    #endregion
		
		public AnimalMovementViews() : 
				base(global::DataModel.Properties.Settings.Default.Animal_MovementConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public AnimalMovementViews(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AnimalMovementViews(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AnimalMovementViews(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public AnimalMovementViews(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<AnimalFixesByFile> AnimalFixesByFiles
		{
			get
			{
				return this.GetTable<AnimalFixesByFile>();
			}
		}
		
		public System.Data.Linq.Table<PartiallyProcessedArgosFiles> PartiallyProcessedArgosFiles
		{
			get
			{
				return this.GetTable<PartiallyProcessedArgosFiles>();
			}
		}
		
		public System.Data.Linq.Table<NeverProcessedArgosFiles> NeverProcessedArgosFiles
		{
			get
			{
				return this.GetTable<NeverProcessedArgosFiles>();
			}
		}
		
		public System.Data.Linq.Table<NeverProcessedDataLogFiles> NeverProcessedDataLogFiles
		{
			get
			{
				return this.GetTable<NeverProcessedDataLogFiles>();
			}
		}
		
		public System.Data.Linq.Table<AllTpfFileData> AllTpfFileDatas
		{
			get
			{
				return this.GetTable<AllTpfFileData>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.AnimalLocationSummary", IsComposable=true)]
		public IQueryable<AnimalLocationSummaryResult> AnimalLocationSummary([global::System.Data.Linq.Mapping.ParameterAttribute(Name="ProjectId", DbType="NVarChar(255)")] string projectId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="AnimalId", DbType="NVarChar(255)")] string animalId)
		{
			return this.CreateMethodCallQuery<AnimalLocationSummaryResult>(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), projectId, animalId);
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.CollarFixesByFile", IsComposable=true)]
		public IQueryable<CollarFixesByFileResult> CollarFixesByFile([global::System.Data.Linq.Mapping.ParameterAttribute(Name="CollarManufacturer", DbType="NVarChar(255)")] string collarManufacturer, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="CollarId", DbType="NVarChar(255)")] string collarId)
		{
			return this.CreateMethodCallQuery<CollarFixesByFileResult>(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), collarManufacturer, collarId);
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.CollarFixSummary", IsComposable=true)]
		public IQueryable<CollarFixSummaryResult> CollarFixSummary([global::System.Data.Linq.Mapping.ParameterAttribute(Name="CollarManufacturer", DbType="NVarChar(255)")] string collarManufacturer, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="CollarId", DbType="NVarChar(255)")] string collarId)
		{
			return this.CreateMethodCallQuery<CollarFixSummaryResult>(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), collarManufacturer, collarId);
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.ConflictingFixes", IsComposable=true)]
		public IQueryable<ConflictingFixesResult> ConflictingFixes([global::System.Data.Linq.Mapping.ParameterAttribute(Name="CollarManufacturer", DbType="NVarChar(255)")] string collarManufacturer, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="CollarId", DbType="NVarChar(255)")] string collarId, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="LastXdays", DbType="Int")] System.Nullable<int> lastXdays)
		{
			return this.CreateMethodCallQuery<ConflictingFixesResult>(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), collarManufacturer, collarId, lastXdays);
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.DaysSinceLastDownload", IsComposable=true)]
		public System.Nullable<int> DaysSinceLastDownload()
		{
			return ((System.Nullable<int>)(this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod()))).ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.FileHasGen4Data", IsComposable=true)]
		public System.Nullable<bool> FileHasGen4Data([global::System.Data.Linq.Mapping.ParameterAttribute(Name="FileId", DbType="Int")] System.Nullable<int> fileId)
		{
			return ((System.Nullable<bool>)(this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), fileId).ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.GetTelonicsParametersForArgosDates", IsComposable=true)]
		public IQueryable<GetTelonicsParametersForArgosDatesResult> GetTelonicsParametersForArgosDates([global::System.Data.Linq.Mapping.ParameterAttribute(Name="PlatformID", DbType="VarChar(8)")] string platformID, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="StartDate", DbType="DateTime2")] System.Nullable<System.DateTime> startDate, [global::System.Data.Linq.Mapping.ParameterAttribute(Name="EndDate", DbType="DateTime2")] System.Nullable<System.DateTime> endDate)
		{
			return this.CreateMethodCallQuery<GetTelonicsParametersForArgosDatesResult>(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), platformID, startDate, endDate);
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.AnimalFixesByFile")]
	public partial class AnimalFixesByFile
	{
		
		private int _FileId;
		
		private string _Manufacturer;
		
		private string _CollarId;
		
		private string _Project;
		
		private string _AnimalId;
		
		private System.Nullable<System.DateTime> _First_Fix;
		
		private System.Nullable<System.DateTime> _Last_Fix;
		
		private System.Nullable<int> _Number_of_Fixes;
		
		public AnimalFixesByFile()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileId", DbType="Int NOT NULL")]
		public int FileId
		{
			get
			{
				return this._FileId;
			}
			set
			{
				if ((this._FileId != value))
				{
					this._FileId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Manufacturer", DbType="NVarChar(200)")]
		public string Manufacturer
		{
			get
			{
				return this._Manufacturer;
			}
			set
			{
				if ((this._Manufacturer != value))
				{
					this._Manufacturer = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CollarId", DbType="VarChar(16) NOT NULL", CanBeNull=false)]
		public string CollarId
		{
			get
			{
				return this._CollarId;
			}
			set
			{
				if ((this._CollarId != value))
				{
					this._CollarId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Project", DbType="NVarChar(150)")]
		public string Project
		{
			get
			{
				return this._Project;
			}
			set
			{
				if ((this._Project != value))
				{
					this._Project = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_AnimalId", DbType="VarChar(16) NOT NULL", CanBeNull=false)]
		public string AnimalId
		{
			get
			{
				return this._AnimalId;
			}
			set
			{
				if ((this._AnimalId != value))
				{
					this._AnimalId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[First Fix]", Storage="_First_Fix", DbType="DateTime2")]
		public System.Nullable<System.DateTime> First_Fix
		{
			get
			{
				return this._First_Fix;
			}
			set
			{
				if ((this._First_Fix != value))
				{
					this._First_Fix = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Last Fix]", Storage="_Last_Fix", DbType="DateTime2")]
		public System.Nullable<System.DateTime> Last_Fix
		{
			get
			{
				return this._Last_Fix;
			}
			set
			{
				if ((this._Last_Fix != value))
				{
					this._Last_Fix = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Number of Fixes]", Storage="_Number_of_Fixes", DbType="Int")]
		public System.Nullable<int> Number_of_Fixes
		{
			get
			{
				return this._Number_of_Fixes;
			}
			set
			{
				if ((this._Number_of_Fixes != value))
				{
					this._Number_of_Fixes = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ArgosFile_NeedsPartialProcessing")]
	public partial class PartiallyProcessedArgosFiles
	{
		
		private int _FileId;
		
		private string _PlatformId;
		
		public PartiallyProcessedArgosFiles()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileId", DbType="Int NOT NULL")]
		public int FileId
		{
			get
			{
				return this._FileId;
			}
			set
			{
				if ((this._FileId != value))
				{
					this._FileId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PlatformId", DbType="VarChar(8) NOT NULL", CanBeNull=false)]
		public string PlatformId
		{
			get
			{
				return this._PlatformId;
			}
			set
			{
				if ((this._PlatformId != value))
				{
					this._PlatformId = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.ArgosFile_NeverProcessed")]
	public partial class NeverProcessedArgosFiles
	{
		
		private int _FileId;
		
		public NeverProcessedArgosFiles()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileId", DbType="Int NOT NULL")]
		public int FileId
		{
			get
			{
				return this._FileId;
			}
			set
			{
				if ((this._FileId != value))
				{
					this._FileId = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.DataLog_NeverProcessed")]
	public partial class NeverProcessedDataLogFiles
	{
		
		private int _FileId;
		
		public NeverProcessedDataLogFiles()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileId", DbType="Int NOT NULL")]
		public int FileId
		{
			get
			{
				return this._FileId;
			}
			set
			{
				if ((this._FileId != value))
				{
					this._FileId = value;
				}
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.AllTpfFileData")]
	public partial class AllTpfFileData
	{
		
		private System.Nullable<int> _FileId;
		
		private string _CTN;
		
		private string _Platform;
		
		private System.Nullable<double> _Frequency;
		
		private System.Nullable<System.DateTime> _TimeStamp;
		
		private string _FileName;
		
		private char _Status;
		
		public AllTpfFileData()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileId", DbType="Int")]
		public System.Nullable<int> FileId
		{
			get
			{
				return this._FileId;
			}
			set
			{
				if ((this._FileId != value))
				{
					this._FileId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CTN", DbType="NVarChar(16)")]
		public string CTN
		{
			get
			{
				return this._CTN;
			}
			set
			{
				if ((this._CTN != value))
				{
					this._CTN = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Platform", DbType="NVarChar(8)")]
		public string Platform
		{
			get
			{
				return this._Platform;
			}
			set
			{
				if ((this._Platform != value))
				{
					this._Platform = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Frequency", DbType="Float")]
		public System.Nullable<double> Frequency
		{
			get
			{
				return this._Frequency;
			}
			set
			{
				if ((this._Frequency != value))
				{
					this._Frequency = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TimeStamp", DbType="DateTime2")]
		public System.Nullable<System.DateTime> TimeStamp
		{
			get
			{
				return this._TimeStamp;
			}
			set
			{
				if ((this._TimeStamp != value))
				{
					this._TimeStamp = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileName", DbType="NVarChar(255) NOT NULL", CanBeNull=false)]
		public string FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				if ((this._FileName != value))
				{
					this._FileName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Status", DbType="Char(1) NOT NULL")]
		public char Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				if ((this._Status != value))
				{
					this._Status = value;
				}
			}
		}
	}
	
	public partial class AnimalLocationSummaryResult
	{
		
		private System.Nullable<int> _Count;
		
		private System.Nullable<double> _Left;
		
		private System.Nullable<double> _Right;
		
		private System.Nullable<double> _Bottom;
		
		private System.Nullable<double> _Top;
		
		private System.Nullable<System.DateTime> _First;
		
		private System.Nullable<System.DateTime> _Last;
		
		public AnimalLocationSummaryResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Count", DbType="Int")]
		public System.Nullable<int> Count
		{
			get
			{
				return this._Count;
			}
			set
			{
				if ((this._Count != value))
				{
					this._Count = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Left]", Storage="_Left", DbType="Float")]
		public System.Nullable<double> Left
		{
			get
			{
				return this._Left;
			}
			set
			{
				if ((this._Left != value))
				{
					this._Left = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Right]", Storage="_Right", DbType="Float")]
		public System.Nullable<double> Right
		{
			get
			{
				return this._Right;
			}
			set
			{
				if ((this._Right != value))
				{
					this._Right = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Bottom", DbType="Float")]
		public System.Nullable<double> Bottom
		{
			get
			{
				return this._Bottom;
			}
			set
			{
				if ((this._Bottom != value))
				{
					this._Bottom = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Top]", Storage="_Top", DbType="Float")]
		public System.Nullable<double> Top
		{
			get
			{
				return this._Top;
			}
			set
			{
				if ((this._Top != value))
				{
					this._Top = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_First", DbType="DateTime2")]
		public System.Nullable<System.DateTime> First
		{
			get
			{
				return this._First;
			}
			set
			{
				if ((this._First != value))
				{
					this._First = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Last", DbType="DateTime2")]
		public System.Nullable<System.DateTime> Last
		{
			get
			{
				return this._Last;
			}
			set
			{
				if ((this._Last != value))
				{
					this._Last = value;
				}
			}
		}
	}
	
	public partial class CollarFixesByFileResult
	{
		
		private int _FileId;
		
		private string _File;
		
		private string _Status;
		
		private System.Nullable<int> _FixCount;
		
		private System.Nullable<System.DateTime> _First;
		
		private System.Nullable<System.DateTime> _Last;
		
		public CollarFixesByFileResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileId", DbType="Int NOT NULL")]
		public int FileId
		{
			get
			{
				return this._FileId;
			}
			set
			{
				if ((this._FileId != value))
				{
					this._FileId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[File]", Storage="_File", DbType="NVarChar(259) NOT NULL", CanBeNull=false)]
		public string File
		{
			get
			{
				return this._File;
			}
			set
			{
				if ((this._File != value))
				{
					this._File = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Status", DbType="NVarChar(32) NOT NULL", CanBeNull=false)]
		public string Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				if ((this._Status != value))
				{
					this._Status = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FixCount", DbType="Int")]
		public System.Nullable<int> FixCount
		{
			get
			{
				return this._FixCount;
			}
			set
			{
				if ((this._FixCount != value))
				{
					this._FixCount = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_First", DbType="DateTime2")]
		public System.Nullable<System.DateTime> First
		{
			get
			{
				return this._First;
			}
			set
			{
				if ((this._First != value))
				{
					this._First = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Last", DbType="DateTime2")]
		public System.Nullable<System.DateTime> Last
		{
			get
			{
				return this._Last;
			}
			set
			{
				if ((this._Last != value))
				{
					this._Last = value;
				}
			}
		}
	}
	
	public partial class CollarFixSummaryResult
	{
		
		private System.Nullable<int> _Count;
		
		private System.Nullable<int> _Unique;
		
		private System.Nullable<System.DateTime> _First;
		
		private System.Nullable<System.DateTime> _Last;
		
		public CollarFixSummaryResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Count", DbType="Int")]
		public System.Nullable<int> Count
		{
			get
			{
				return this._Count;
			}
			set
			{
				if ((this._Count != value))
				{
					this._Count = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="[Unique]", Storage="_Unique", DbType="Int")]
		public System.Nullable<int> Unique
		{
			get
			{
				return this._Unique;
			}
			set
			{
				if ((this._Unique != value))
				{
					this._Unique = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_First", DbType="DateTime2")]
		public System.Nullable<System.DateTime> First
		{
			get
			{
				return this._First;
			}
			set
			{
				if ((this._First != value))
				{
					this._First = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Last", DbType="DateTime2")]
		public System.Nullable<System.DateTime> Last
		{
			get
			{
				return this._Last;
			}
			set
			{
				if ((this._Last != value))
				{
					this._Last = value;
				}
			}
		}
	}
	
	public partial class ConflictingFixesResult
	{
		
		private int _FixId;
		
		private System.Nullable<int> _HiddenBy;
		
		private int _FileId;
		
		private int _LineNumber;
		
		private System.Nullable<System.DateTime> _LocalFixTime;
		
		private double _Lat;
		
		private double _Lon;
		
		public ConflictingFixesResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FixId", DbType="Int NOT NULL")]
		public int FixId
		{
			get
			{
				return this._FixId;
			}
			set
			{
				if ((this._FixId != value))
				{
					this._FixId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_HiddenBy", DbType="Int")]
		public System.Nullable<int> HiddenBy
		{
			get
			{
				return this._HiddenBy;
			}
			set
			{
				if ((this._HiddenBy != value))
				{
					this._HiddenBy = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileId", DbType="Int NOT NULL")]
		public int FileId
		{
			get
			{
				return this._FileId;
			}
			set
			{
				if ((this._FileId != value))
				{
					this._FileId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LineNumber", DbType="Int NOT NULL")]
		public int LineNumber
		{
			get
			{
				return this._LineNumber;
			}
			set
			{
				if ((this._LineNumber != value))
				{
					this._LineNumber = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LocalFixTime", DbType="DateTime")]
		public System.Nullable<System.DateTime> LocalFixTime
		{
			get
			{
				return this._LocalFixTime;
			}
			set
			{
				if ((this._LocalFixTime != value))
				{
					this._LocalFixTime = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lat", DbType="Float NOT NULL")]
		public double Lat
		{
			get
			{
				return this._Lat;
			}
			set
			{
				if ((this._Lat != value))
				{
					this._Lat = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Lon", DbType="Float NOT NULL")]
		public double Lon
		{
			get
			{
				return this._Lon;
			}
			set
			{
				if ((this._Lon != value))
				{
					this._Lon = value;
				}
			}
		}
	}
	
	public partial class GetTelonicsParametersForArgosDatesResult
	{
		
		private int _DeploymentId;
		
		private System.Nullable<int> _ParameterId;
		
		private string _PlatformId;
		
		private string _CollarManufacturer;
		
		private string _CollarId;
		
		private string _CollarModel;
		
		private System.Nullable<int> _Gen3Period;
		
		private System.Nullable<char> _Format;
		
		private string _FileName;
		
		private System.Data.Linq.Binary _Contents;
		
		private System.Nullable<System.DateTime> _StartDate;
		
		private System.Nullable<System.DateTime> _EndDate;
		
		public GetTelonicsParametersForArgosDatesResult()
		{
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DeploymentId", DbType="Int NOT NULL")]
		public int DeploymentId
		{
			get
			{
				return this._DeploymentId;
			}
			set
			{
				if ((this._DeploymentId != value))
				{
					this._DeploymentId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ParameterId", DbType="Int")]
		public System.Nullable<int> ParameterId
		{
			get
			{
				return this._ParameterId;
			}
			set
			{
				if ((this._ParameterId != value))
				{
					this._ParameterId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PlatformId", DbType="VarChar(8) NOT NULL", CanBeNull=false)]
		public string PlatformId
		{
			get
			{
				return this._PlatformId;
			}
			set
			{
				if ((this._PlatformId != value))
				{
					this._PlatformId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CollarManufacturer", DbType="VarChar(16) NOT NULL", CanBeNull=false)]
		public string CollarManufacturer
		{
			get
			{
				return this._CollarManufacturer;
			}
			set
			{
				if ((this._CollarManufacturer != value))
				{
					this._CollarManufacturer = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CollarId", DbType="VarChar(16) NOT NULL", CanBeNull=false)]
		public string CollarId
		{
			get
			{
				return this._CollarId;
			}
			set
			{
				if ((this._CollarId != value))
				{
					this._CollarId = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CollarModel", DbType="VarChar(24) NOT NULL", CanBeNull=false)]
		public string CollarModel
		{
			get
			{
				return this._CollarModel;
			}
			set
			{
				if ((this._CollarModel != value))
				{
					this._CollarModel = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Gen3Period", DbType="Int")]
		public System.Nullable<int> Gen3Period
		{
			get
			{
				return this._Gen3Period;
			}
			set
			{
				if ((this._Gen3Period != value))
				{
					this._Gen3Period = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Format", DbType="Char(1)")]
		public System.Nullable<char> Format
		{
			get
			{
				return this._Format;
			}
			set
			{
				if ((this._Format != value))
				{
					this._Format = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FileName", DbType="NVarChar(255)")]
		public string FileName
		{
			get
			{
				return this._FileName;
			}
			set
			{
				if ((this._FileName != value))
				{
					this._FileName = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Contents", DbType="VarBinary(MAX)", UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary Contents
		{
			get
			{
				return this._Contents;
			}
			set
			{
				if ((this._Contents != value))
				{
					this._Contents = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StartDate", DbType="DateTime2")]
		public System.Nullable<System.DateTime> StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				if ((this._StartDate != value))
				{
					this._StartDate = value;
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_EndDate", DbType="DateTime2")]
		public System.Nullable<System.DateTime> EndDate
		{
			get
			{
				return this._EndDate;
			}
			set
			{
				if ((this._EndDate != value))
				{
					this._EndDate = value;
				}
			}
		}
	}
}
#pragma warning restore 1591
