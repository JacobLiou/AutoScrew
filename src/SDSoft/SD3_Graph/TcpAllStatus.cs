namespace SD3_Graph
{
	public struct TcpAllStatus
	{
		public ushort CodeCmd;

		public ushort ID;

		public ushort SetData1;

		public ushort SetData2;

		public ushort SetData3;

		public ushort SetData4;

		public ushort Flag;

		public ushort CodeFed;

		public ushort OKNG;

		public ushort ErrCode;

		public TcpStatus1 T1StA;

		public TcpStatus1 T2StA;

		public TcpStatus2 Comm;

		public TcpStatus3 T1StB;

		public TcpStatus3 T2StB;
	}
}
