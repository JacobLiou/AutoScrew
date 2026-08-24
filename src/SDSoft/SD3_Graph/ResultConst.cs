using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ResultConst
	{
		public const int EncpBarcode = 100;

		public const int TotalLedNum = 1000;

		public const int TargetTextSpace = 35;

		public const int TargetTextSpace2 = 1;

		public const float CurveScaleOffs = -0.001f;

		public const float MinCurve = -500f;

		public const int ChartInnrXoffs = 12;

		public const int ChartInnrYoffs = 3;

		public const int ChartXoffs = 14;

		public const int ChartY1offs = 1;

		public const int ChartY2offs = 7;

		public const int ChartY3offs = 21;

		public const int ChartRowPitch = 6;

		public const int ChartTextSize = 10;

		public const int LED_Non = 0;

		public const int LED_Gray = 1;

		public const int LED_Green = 2;

		public const int LED_Yellow = 4;

		public const int LED_Red = 8;
	}
}
