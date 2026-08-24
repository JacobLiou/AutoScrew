using System.Runtime.InteropServices;

namespace SD3_Graph
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ScrewTcpConst
	{
		public const int Key = 99;

		public const int DeleteFolder = 100;

		public const int BinLoop = 10;

		public const int BinFileCreate = 900;

		public const int BinFileEnd = 999;

		public const int BinFileDelFail = 4000;

		public const int BinFileNonExist = 4001;

		public const int BinFileCRCError = 4002;

		public const int MaxDatagramSize = 2000;

		public const int ShortTimeOutCnt = 1000;

		public const int TimeOutCnt = 1500;

		public const int LongTimeOutCnt = 5000;

		public const int DoubleLongTimeOutCnt = 60000;

		public const int HanderByteSize = 20;

		public const int HanderWordSize = 10;

		public const int ReflashStatusSize = 10;

		public const int RetryCnt = 2;

		public const int NG = 2;

		public const int OK = 1;

		public const int NoErr = 0;

		public const int UnknowErr = -1;

		public const int TimeOut = -2;

		public const int ErrOKNG = -3;

		public const int NoDeviceConnect = -4;

		public const int OverRetryCnt = -5;

		public const int NoReponse = -6;
	}
}
