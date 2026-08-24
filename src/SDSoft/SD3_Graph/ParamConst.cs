using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ParamConst
	{
		public const int ALL_STAGE_NUM = 7;

		public const int STAGE_NUM = 6;

		public const int TGStageEnd = 5;

		public const int LoosStageStart = 10;

		public const int LoosStageEnd = 11;

		public const int EncpParamGP = 500;

		public const int EncpParamStr = 20;

		public const int EncpParam1 = 70;

		public const int EncpParam2 = 500;

		public const int EncpParam3 = 600;

		public const int EncpEachItem = 50;

		public const ushort CCW = 1;

		public const ushort CW = 0;

		public const ushort DefDiffAngOfSnugPoint = 300;

		public const ushort DefAngOfBackSP = 300;

		public const ushort DefTorqOfTGAngle = 300;

		public const ushort DefTorqOfTGAngle_MINI = 30;

		public const ushort DefTorqOfSnugAngle = 100;

		public const ushort DefTorqOfSnugAngle_MINI = 10;

		public const ushort DefTorqRateCmd = 100;

		public const ushort DefTGACC = 1000;

		public const ushort DefLoosAng1 = 100;

		public const ushort DefLoosAng2 = 7200;

		public const ushort DefLoosVel = 300;

		public const ushort DEF_ANGLE_TYPE = 0;

		public const ushort DEF_TORQ_TYPE = 1;

		public const ushort DEF_TORQRATE_TYPE = 2;

		public const ushort DEF_CLAMPTORQ_TYPE = 3;

		public const ushort DEF_CLAMPANGLE_TYPE = 4;

		public const ushort DEF_YIELD_TYPE = 5;

		public const ushort DEF_ANGORTORQ_TYPE = 6;

		public const ushort DEF_NULL_TYPE = 99;

		public const ushort DEF_EXPARAM_VER = 0;

		public const ushort EVENT_TORQULLL = 6;

		public const ushort EVENT_CURVESTARTTORQ = 22;

		public const ushort EVENT_PREVAILTORQSET = 23;

		public const ushort EVENT_SLOWDOWN = 24;

		public const ushort EVENT_HOLDTIME = 25;

		public const ushort EVENT_ANGDIFF = 26;

		public const ushort EVENT_STARTTORQOFSAMPLECURVE = 27;

		public const ushort EVENT_STARTTORQOFBITSLIP = 28;

		public const ushort EVENT_STARTTORQRATE = 29;

		public const ushort EVENT_PREVAILTORQLINKCLOSE = 51;

		public const ushort EVENT_FINALTORQULLL = 60;

		public const ushort EVENT_TG2NDSTAGETORQ = 61;

		public const ushort EVENT_TG2NDSTAGEACC = 62;

		public const ushort EVENT_TG2NDSTAGEMODE = 63;

		public const string ParamVer = "Ver01";

		public const string ParamSpecVer = "Ver99";

		public const int ParamVer0 = 1;

		public const int ParamVer1 = 2;
	}
}
