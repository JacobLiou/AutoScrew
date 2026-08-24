namespace SD3_Graph
{
	public struct ExReportStuc
	{
		public ReportInfoStuc[] Info;

		public ushort[] CurveTime;

		public short[] CurveAngle;

		public short[] CurveTorque;

		public short[] CurveTorqueRate;

		public ushort[] ReportParam;

		public ReportScaleStuc[] Scale;

		public AlarmWarningReportInfo[] AlarmInfo;

		public AlarmWarningReportInfo[] WarningInfo;

		public AlarmWarningReportInfo[] AlarmInfoOnlyAL;

		public AlarmWarningReportInfo[] AlarmInfoOnlyNG;

		public ButtonReportInfo[] ButtonInfo;

		public unsafe fixed bool Delete[200000];
	}
}
