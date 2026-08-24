using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SeqConst
	{
		public const int EncpSeqGP = 500;

		public const int EncpParmEachGp = 100;

		public const int EncpSeqStr = 20;

		public const int EncpSeq = 500;

		public const int EncpSeq2 = 500;

		public const int GuideScrewNum = 100;

		public const int GuidePicNum = 30;

		public const int PicByteLen = 2000;

		public const int Axis_OffsetMode = 10;

		public const int LedSizeWH = 30;

		public const int Src_Mode = 1;

		public const int Src_Axis_X = 1;

		public const int Src_Axis_Y = 2;

		public const int Src_Axis_Mix = 3;

		public const int GuideReadAll = 0;

		public const int GuideEditTitleParam = 1;

		public const int GuideEditQty = 2;

		public const int GuideResetSingleLedPos = 3;

		public const int GuideResetAllLedPos = 4;

		public const int GuideOnlyReflashAllLed = 10;

		public const int GuideInsertLed = 97;

		public const int GuideDeleteLed = 98;

		public const int GuideDeleteRowLed = 99;

		public const int GuideImgWidth = 740;

		public const int GuideImgHeigh = 460;

		public const string SeqVer = "Ver01";

		public const string SeqSpecVer = "Ver99";
	}
}
