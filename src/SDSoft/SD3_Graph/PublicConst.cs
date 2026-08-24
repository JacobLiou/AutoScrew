using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct PublicConst
	{
		public const float Width = 1600f;

		public const float Height = 900f;

		public const int LangCN = 0;

		public const int LangEN = 1;

		public const int LangZN = 2;

		public const int LangJP = 5;

		public const int Axis_X = 0;

		public const int Axis_Y = 1;

		public const int AlarmMsgStop = 0;

		public const int AlarmMsgStart = 1;

		public const int AlarmMsgNone = 99;

		public const int AlarmMsgWidth = 500;

		public const int TGDef = 0;

		public const int LODef = 1;

		public const int ST1Def = 2;

		public const int ST2Def = 3;

		public const int ST3Def = 4;

		public const int ST4Def = 5;

		public const int ST5Def = 6;

		public const int ST6Def = 7;

		public const int ST7Def = 8;

		public const int ST8Def = 9;

		public const int ST9Def = 10;

		public const int STADef = 11;

		public const int HeaderDef = 12;

		public const int AngDef = 0;

		public const int TorqDef = 1;

		public const int TorqRateDef = 2;

		public const int BG1Def = 3;

		public const int BG2Def = 4;

		public const int Spd1Def = 4;

		public const int Spd2Def = 5;

		public const int MaxTimeDef = 6;

		public const int MinTimeDef = 7;

		public const int MaxTorqDef = 8;

		public const int MinTorqDef = 9;

		public const int MaxAngDef = 10;

		public const int MinAngDef = 11;

		public const int MaxClampTorqDef = 12;

		public const int MinClampTorqDef = 13;

		public const int MaxClampAngDef = 14;

		public const int MinClampAngDef = 15;

		public const int MaxSwitchTorqDef = 16;

		public const int A_Base = 0;

		public const int A_TG = 1;

		public const int A_LO = 2;

		public const int A_S1Def = 3;

		public const int A_S2Def = 4;

		public const int A_S3Def = 5;

		public const int A_S4Def = 6;

		public const int A_S5Def = 7;

		public const int A_S6Def = 8;

		public const int A_S7Def = 9;

		public const int A_S8Def = 10;

		public const int A_S9Def = 11;

		public const int A_SADef = 12;

		public const int Standard_Em = 0;

		public const int Standard_RunDM = 1;

		public const int Standard_PreTG = 2;

		public const int Standard_TG = 3;

		public const int Enhanced_TG = 0;

		public const int PrePosition_Em = 0;

		public const int PrePosition_RunD = 1;

		public const int ShowNon = 0;

		public const int ShowJump = 1;

		public const int Inf = 999999;

		public const int CtrlType_101inch = 0;

		public const int CtrlType_043inch = 1;

		public const int CtrlType_Noninch = 2;

		public const int DefToolType_Mini = 0;

		public const int DefToolType_Stand = 1;

		public const int DefToolType_Mid = 2;

		public const int CtrlMode_101Stand = 0;

		public const int CtrlMode_043Mini = 1;

		public const int CtrlMode_101Mode = 2;

		public const int CtrlMode_043Stand = 3;

		public const int CtrlMode_101Mid = 4;

		public const int ToolSpec_Unknow = 0;

		public const int ToolSpec_250KG = 25000;

		public const int ToolSpec_170KG = 17000;

		public const int ToolSpec_120KG = 12000;

		public const int ToolSpec_075KG = 7500;

		public const int ToolSpec_050KG = 5000;

		public const int ToolSpec_030KG = 3000;

		public const int ToolSpec_012KG = 1200;

		public const int ToolSpec_003KG = 350;

		public const int ToolSpec_002KG = 200;

		public const int ToolSpec_001_3KG = 130;

		public const int ToolSpec_001KG = 100;

		public const int ToolSpec_Online = 0;

		public const int ToolSpec_NonSpec = 9999;

		public const int CtrlMode_SingleTool_00 = 0;

		public const int CtrlMode_DualTool_01 = 1;

		public const int CtrlNonUser = 99;

		public const int HMIVer_YieldShutOff = 169;

		public const int FinalStageSpeedLimit = 100;
	}
}
