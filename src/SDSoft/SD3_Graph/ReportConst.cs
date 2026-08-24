using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ReportConst
	{
		public const int ProductReport = 0;

		public const int ErrorReport = 1;

		public const int WarningReport = 2;

		public const int ButtonReport = 3;

		public const int EncpReportLMGP = 10;

		public const int EncpReportGP = 200000;

		public const int EncpALWNGP = 6000;

		public const int EncpALWNNun = 6;

		public const int EncpBNGP = 6000;

		public const int EncpBNNun = 10;

		public const int EncpEachCurvePoint = 2000;

		public const int EncpEachParam = 550;

		public const int EncpScaleAndCurvePoint = 8600;

		public const int EncpBarcode = 100;

		public const int EncpReportDetail = 50;

		public const int EncpReportScale = 50;

		public const int EncpOtherInfo = 200;

		public const int EncpReportStatus = 1;

		public const int EncpReportCurveReadGP = 20;

		public const string ReportVer = "Ver01";

		public const string ReportVerSpec = "Ver99";

		public const string ReportVerSpec2 = "Ver98";

		public const string ReportVerSpec3 = "Ver97";

		public const string CurveVer = "Ver01";
	}
}
