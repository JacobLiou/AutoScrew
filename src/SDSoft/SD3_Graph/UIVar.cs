using System.Collections.Generic;

namespace SD3_Graph
{
	public class UIVar
	{
		public List<int> List_Time = new List<int>();

		public List<int> List_Angle = new List<int>();

		public List<int> List_Torq = new List<int>();

		public List<int> List_TorqRate = new List<int>();

		public List<int> List_Stage = new List<int>();

		public ReportInfoStuc List_Info = default(ReportInfoStuc);

		public ReportScaleStuc List_Scale = default(ReportScaleStuc);

		public OtherInfo List_OtherInfo = default(OtherInfo);

		public List<ushort> List_Param_Unit = new List<ushort>();

		public string IPstr = "192.168.1.11";

		public long DontCareSpaceSize = long.MaxValue;

		public long NeedSpaceMBSize = 300L;

		public long NeedSpace4GBSize = 4096L;

		public ushort StartYY;

		public ushort StartMM;

		public ushort StartDD;

		public ushort EndYY;

		public ushort EndMM;

		public ushort EndDD;

		public ushort EnDisFTDate;

		public ushort EnDisFTTool;

		public ushort EnDisFTStatus;

		public readonly bool PCFTPMaster = true;

		public readonly string FTPSavePath = ".\\ScrewInfo\\BinInfo\\";

		public readonly int passivePort = 603;

		public int CurveSelectX = 0;

		public int CurveSelectY = 0;

		public int AutoFit = 0;

		public bool IsGuidePicFromCtrl;

		public bool IsReadSupportFTPServer;

		public bool IsReadSupportFTPClient;

		public bool IsNonFireWall;

		public ushort SpecCtrl;

		public ParamStucVer1 RunningParamX = default(ParamStucVer1);

		public ParamStucVer1 RunningParamY = default(ParamStucVer1);

		public SeqBaseStuc RunningSeqX = default(SeqBaseStuc);

		public SeqBaseStuc RunningSeqY = default(SeqBaseStuc);

		public string RangeStr;

		public string UploadToCtrl;

		public string DownloadFromCtrl;

		public string ImportFromCSV;

		public string ExportToCSV;

		public string ExportResultInfoToCSV;

		public string ExportSingleResultAndCurveToCSV;

		public string ShowFilterConditions;

		public string StopFollowingTheLatestEntry;

		public string SelectMultipleReportItemsForAnalysis;

		public SrcStuc RunningSrcX = default(SrcStuc);

		public SrcStuc RunningSrcY = default(SrcStuc);

		public SrcMode RunningSrcMode = default(SrcMode);

		public ReportInfoStuc RunningInfoX = default(ReportInfoStuc);

		public ReportScaleStuc RunningScaleX = default(ReportScaleStuc);

		public List<short> RunningCurveTimeX = new List<short>();

		public List<short> RunningCurveAngleX = new List<short>();

		public List<short> RunningCurveTorqueX = new List<short>();

		public List<short> RunningCurveTorqueRateX = new List<short>();

		public ReportInfoStuc RunningInfoY = default(ReportInfoStuc);

		public ReportScaleStuc RunningScaleY = default(ReportScaleStuc);

		public List<short> RunningCurveTimeY = new List<short>();

		public List<short> RunningCurveAngleY = new List<short>();

		public List<short> RunningCurveTorqueY = new List<short>();

		public List<short> RunningCurveTorqueRateY = new List<short>();

		public DetectPageAxis PageAxisInfo = default(DetectPageAxis);

		public ushort ToolMaxSpeed_X;

		public ushort ToolMinSpeed_X;

		public ushort ToolMaxULTorqueFW_X;

		public ushort ToolMaxTorqueFW_X;

		public ushort ToolSetTorqueFW_X;

		public ushort ToolMinTorqueFW_X;

		public ushort ToolTorqueSpec_X;

		public ushort ToolMaxSpeed_Y;

		public ushort ToolMinSpeed_Y;

		public ushort ToolMaxULTorqueFW_Y;

		public ushort ToolMaxTorqueFW_Y;

		public ushort ToolSetTorqueFW_Y;

		public ushort ToolMinTorqueFW_Y;

		public ushort ToolTorqueSpec_Y;

		public ushort ParmShowTorqueUnit;

		public ushort RunningToolMaxSpeed;

		public ushort RunningToolMinSpeed;

		public ushort RunningToolMaxULTorqueFW;

		public ushort RunningToolMaxTorqueFW;

		public ushort RunningToolSetTorqueFW;

		public ushort RunningToolMinTorqueFW;

		public ushort LastCurveCnt;

		public ushort LastCurveCnt2;

		public int ParamPageAxis;

		public int UIPageNonSave;

		public bool PCSoftSupport;

		public bool GuideFuncEnable;

		public ushort PM101;

		public ushort CtrlDualTool;

		public ushort ToolX_ModelType;

		public ushort ToolY_ModelType;

		public ushort NonPushStartTypeX;

		public ushort NonPushStartTypeY;

		public ushort NonLightBrightX;

		public ushort NonLightBrightY;
	}
}
