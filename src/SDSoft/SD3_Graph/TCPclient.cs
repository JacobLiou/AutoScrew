using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace SD3_Graph
{
	public class TCPclient
	{
		private Socket clientSocket;

		private UdpClient udpClient;

		private IPEndPoint endPoint;

		private IPEndPoint PCPoint;

		private IPAddress MatchIP;

		public Ping ping = new Ping();

		private GlobalVar GB;

		private bool FirstConnect = true;

		private uint OrgProductID = 0u;

		private uint OrgAlarmCode_X = 0u;

		private uint OrgAlarmCode_Y = 0u;

		private uint OrgTotalProcessNum_X = 0u;

		private uint OrgTotalProcessNum_Y = 0u;

		private uint OrgRunningIDNum_X = 0u;

		private uint OrgRunningIDNum_Y = 0u;

		private uint OrgRunningParamNum_X = 0u;

		private uint OrgRunningParamNum_Y = 0u;

		private uint OrgRunningSeqNum_X = 0u;

		private uint OrgRunningSeqNum_Y = 0u;

		private uint OrgTighteningStatus_X = 0u;

		private uint OrgTighteningStatus_Y = 0u;

		private uint OrgLooseningStatus_X = 0u;

		private uint OrgLooseningStatus_Y = 0u;

		private uint OrgTorqueDW_X = 0u;

		private uint OrgTorqueDW_Y = 0u;

		private uint OrgCurveCntX = 0u;

		private uint OrgCurveCntY = 0u;

		private uint OrgReportID = 0u;

		private uint OrgReportAlarmID = 0u;

		private uint OrgReportWarningID = 0u;

		private uint OrgReportButtonID = 0u;

		private uint OrgPupWindowID = 0u;

		private ushort OrgDOStatus = 0;

		private ushort OrgDIStatus = 0;

		private string OrgResultBar_X = "";

		private string OrgResultBar_Y = "";

		private ushort OrgOPTimeH_X = 0;

		private ushort OrgOPTimeL_X = 0;

		private ushort OrgTGNOKCntH_X = 0;

		private ushort OrgTGNOKCntL_X = 0;

		private ushort OrgLONOKCntH_X = 0;

		private ushort OrgLONOKCntL_X = 0;

		private ushort OrgOPTimeH_Y = 0;

		private ushort OrgOPTimeL_Y = 0;

		private ushort OrgTGNOKCntH_Y = 0;

		private ushort OrgTGNOKCntL_Y = 0;

		private ushort OrgLONOKCntH_Y = 0;

		private ushort OrgLONOKCntL_Y = 0;

		public ushort[] OrgReflashArray = new ushort[10];

		public ushort[] NowReflashArray = new ushort[10];

		public ushort[] DoneReflashArray = new ushort[10];

		public bool[] AlreadyReflashFlag = new bool[10];

		public uint CurrALRow = 0u;

		public uint CurrNGRow = 0u;

		public ushort SendBitComb = 0;

		public bool SendSetFlag = true;

		public int CommunicationType = 1;

		private byte[] receivedBytes;

		public bool ConnectStatus = false;

		public bool ConnectInterrupt = false;

		public int WaitPage = 0;

		public ushort LastKeepalive = 0;

		public ushort MissKeepaliveCnt = 0;

		public bool DetectDiffReflashFlag = false;

		public int Bin_Status = 0;

		public ushort[] FSBinCacheSNReport = new ushort[150];

		public ushort[] FSBinCacheScaleParam = new ushort[600];

		public ushort[] FSBinCacheOtherInfo = new ushort[200];

		public ushort[] FSBinData = new ushort[8600];

		public ushort[] FSBinCacheTimePoint = new ushort[8000];

		public short[] FSBinCacheAnglePoint = new short[8000];

		public short[] FSBinCacheTorqPoint = new short[8000];

		public short[] FSBinCacheTorqRatePoint = new short[8000];

		public ReportInfoStuc CacheInfoX = default(ReportInfoStuc);

		public ReportScaleStuc CacheScaleX = default(ReportScaleStuc);

		public ushort[] CacheParamX = new ushort[550];

		public List<ushort> CacheCurveTimeX = new List<ushort>();

		public List<short> CacheCurveAngleX = new List<short>();

		public List<short> CacheCurveTorqueX = new List<short>();

		public List<short> CacheCurveTorqueRateX = new List<short>();

		public ReportInfoStuc CacheInfoY = default(ReportInfoStuc);

		public ReportScaleStuc CacheScaleY = default(ReportScaleStuc);

		public ushort[] CacheParamY = new ushort[550];

		public List<ushort> CacheCurveTimeY = new List<ushort>();

		public List<short> CacheCurveAngleY = new List<short>();

		public List<short> CacheCurveTorqueY = new List<short>();

		public List<short> CacheCurveTorqueRateY = new List<short>();

		public UpdateFWStc FSNewFWInfo = default(UpdateFWStc);

		public bool BackGroundfg = false;

		public bool Form400fg = false;

		public bool Form409fg = false;

		public TCPclient(GlobalVar GB)
		{
			this.GB = GB;
			receivedBytes = new byte[14400];
			FSNewFWInfo.RawData = new ushort[2000];
		}

		private void ReceiveEventTCP(Socket client)
		{
			try
			{
				StateObject state = new StateObject();
				state.workSocket = client;
				state.workSocket.BeginReceive(state.aData8, 0, 65535, SocketFlags.None, ReceiveCallbackTCP, state);
				if (GB.Form001TCPEvent != null)
				{
					if (GB.Form001TCPWait)
					{
						GB.Form001TCPEvent.Set();
						GB.Form001TCPWait = false;
					}
					GB.Form001TCPEvent.Close();
				}
			}
			catch
			{
				Console.WriteLine("Tcp ReceiveEvent out!");
				RetryConnect();
			}
		}

		public bool GetFirstConnect()
		{
			return FirstConnect;
		}

		private int ParseType(byte[] data8)
		{
			int Type = BitConverter.ToInt16(data8, 14);
			int OKNG = BitConverter.ToInt16(data8, 16);
			int ErrCode = BitConverter.ToInt16(data8, 18);
			if (Type <= 0 || OKNG != 1 || ErrCode != 0)
			{
				Console.WriteLine("Type:{0},Err:{1}", Type, ErrCode);
				Type = -3;
			}
			return Type;
		}

		public ushort[] CopyStr2Ushort(uint idx, ushort[] Des, string str)
		{
			byte[] Src = Encoding.ASCII.GetBytes(str);
			int Size = Src.Length;
			for (uint n = 0u; n < (Size + 1) / 2; n++)
			{
				if (n < Size / 2 || Size % 2 == 0)
				{
					Des[idx + n] = Convert.ToUInt16((Src[2 * n + 1] << 8) + Src[2 * n]);
				}
				else
				{
					Des[idx + n] = Convert.ToUInt16(Src[2 * n]);
				}
			}
			return Des;
		}

		private unsafe void ReceiveCallbackUDP(IAsyncResult ar)
		{
			try
			{
				byte[] receivedBytes = udpClient.EndReceive(ar, ref endPoint);
				if (endPoint.Address.Equals(MatchIP))
				{
					int UDPByteSize = receivedBytes.Length - 6;
					if (receivedBytes.Length >= 4)
					{
						int Header = BitConverter.ToInt16(receivedBytes, 0);
						int DatagramLen = BitConverter.ToInt16(receivedBytes, 2) - 3;
						if (receivedBytes.Length >= (DatagramLen + 3) * 2)
						{
							int Footer = BitConverter.ToInt16(receivedBytes, 4 + DatagramLen * 2);
							if (Header == 17509 && Footer == 27764)
							{
								int Type = BitConverter.ToInt16(receivedBytes, 18);
								int OKNG = BitConverter.ToInt16(receivedBytes, 20);
								int ErrCode = BitConverter.ToInt16(receivedBytes, 22);
								if (Type == 10 || Type == 50)
								{
									bool CheckSum = true;
									if (GB.CheckHMIVer(168, 6))
									{
										CheckSum = ((receivedBytes[284] > 0) ? true : false);
									}
									if (CheckSum)
									{
										for (int j = 0; j < UDPByteSize; j++)
										{
											GB.TcpStatus.Data8[j] = receivedBytes[j + 4];
										}
										if (GB.TcpStatus.Detail.Comm.UserID_30 > 0)
										{
											GB.ExFSUser.UserID = (uint)(GB.TcpStatus.Detail.Comm.UserID_30 - 1);
										}
										ParseTCPComm();
									}
								}
								else if (Type > 0)
								{
									if (OKNG == 1)
									{
										for (int i = 0; i < UDPByteSize; i++)
										{
											GB.TcpRD.Data8[i] = receivedBytes[i + 4];
										}
										if (GB.TcpRD.CmdFunc == 810)
										{
											if (GB.TcpRD.Data1 == 0 && GB.TcpRD.Data2 == GB.TcpWR.Data2 && GB.TcpRD.Data3 == GB.TcpWR.Data3)
											{
												SendBitComb = 1;
												for (int k = 0; k < UDPByteSize / 2 - 10; k++)
												{
													FSBinData[k] = GB.TcpRD.Data16[10 + k];
												}
											}
											else if (GB.TcpRD.Data1 == 1 && GB.TcpRD.Data2 == GB.TcpWR.Data2 && GB.TcpRD.Data3 == GB.TcpWR.Data3)
											{
												if (SendBitComb == 1)
												{
													SendBitComb |= 2;
												}
												else
												{
													SendBitComb = 0;
												}
												for (int l = 0; l < UDPByteSize / 2 - 10; l++)
												{
													FSBinData[l + 2000] = GB.TcpRD.Data16[10 + l];
												}
											}
											else if (GB.TcpRD.Data1 == 2 && GB.TcpRD.Data2 == GB.TcpWR.Data2 && GB.TcpRD.Data3 == GB.TcpWR.Data3)
											{
												if (SendBitComb == 3)
												{
													SendBitComb |= 4;
												}
												else
												{
													SendBitComb = 0;
												}
												for (int m = 0; m < UDPByteSize / 2 - 10; m++)
												{
													FSBinData[m + 4000] = GB.TcpRD.Data16[10 + m];
												}
											}
											else if (GB.TcpRD.Data1 == 3 && GB.TcpRD.Data2 == GB.TcpWR.Data2 && GB.TcpRD.Data3 == GB.TcpWR.Data3)
											{
												if (SendBitComb == 7)
												{
													SendBitComb |= 8;
												}
												else
												{
													SendBitComb = 0;
												}
												for (int n = 0; n < UDPByteSize / 2 - 10; n++)
												{
													FSBinData[n + 6000] = GB.TcpRD.Data16[10 + n];
												}
											}
											else if (GB.TcpRD.Data1 == 4 && GB.TcpRD.Data2 == GB.TcpWR.Data2 && GB.TcpRD.Data3 == GB.TcpWR.Data3)
											{
												if (SendBitComb == 15)
												{
													SendBitComb |= 16;
												}
												else
												{
													SendBitComb = 0;
												}
												for (int num = 0; num < UDPByteSize / 2 - 10; num++)
												{
													FSBinData[num + 8000] = GB.TcpRD.Data16[10 + num];
												}
											}
											if (SendBitComb == 31)
											{
												SendBitComb = 0;
												SendSetFlag = true;
											}
											else
											{
												SendSetFlag = false;
											}
										}
										else
										{
											SendBitComb = 0;
											SendSetFlag = true;
										}
										if (SendSetFlag && GB.TCPHandshakeEvent != null && GB.TCPHandshakeWait)
										{
											MissKeepaliveCnt = 0;
											GB.TCPHandshakeEvent.Set();
											GB.TCPHandshakeWait = false;
										}
									}
									else
									{
										for (int num2 = 0; num2 < UDPByteSize; num2++)
										{
											GB.TcpRD.Data8[num2] = receivedBytes[num2 + 4];
										}
										if (GB.TCPHandshakeEvent != null && GB.TCPHandshakeWait)
										{
											MissKeepaliveCnt = 0;
											GB.TCPHandshakeEvent.Set();
											GB.TCPHandshakeWait = false;
										}
										Console.WriteLine("Type:{0},Err:{1}", Type, ErrCode);
									}
								}
							}
						}
					}
				}
				udpClient.BeginReceive(ReceiveCallbackUDP, endPoint);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Udp ReceiveCallback out!" + ex.Message + " Err No." + ex.StackTrace);
			}
		}

		private void ParseTCPComm()
		{
			if (FirstConnect)
			{
				FirstConnect = false;
				GB.UISys.LastCurveCnt = GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_23;
			}
			if (OrgDIStatus != GB.TcpStatus.Detail.Comm.DIStatus_03 || OrgDOStatus != GB.TcpStatus.Detail.Comm.DOStatus_02)
			{
				if (GB.Form500Event != null && GB.Form500ThreadWait)
				{
					GB.Form500Event.Set();
					GB.Form500ThreadWait = false;
				}
				OrgDIStatus = GB.TcpStatus.Detail.Comm.DIStatus_03;
				OrgDOStatus = GB.TcpStatus.Detail.Comm.DOStatus_02;
			}
			uint ReportID = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_H_08 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_L_07);
			uint ReportAlarmID = GB.TcpStatus.Detail.Comm.CurrentNoOfErrorReportEntries_05;
			uint ReportWarningID = GB.TcpStatus.Detail.Comm.CurrentNoOfWarningReportEntries_06;
			uint ReportButtonID = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfButtonReportEntries_H_10 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfButtonReportEntries_L_09);
			if (OrgReportID != ReportID || OrgReportAlarmID != ReportAlarmID || OrgReportWarningID != ReportWarningID || OrgReportButtonID != ReportButtonID)
			{
				if (GB.Form700Event != null && GB.Form700ThreadWait)
				{
					GB.Form700Event.Set();
					GB.Form700ThreadWait = false;
				}
				OrgReportID = ReportID;
				OrgReportAlarmID = ReportAlarmID;
				OrgReportWarningID = ReportWarningID;
				OrgReportButtonID = ReportButtonID;
			}
			uint CurrentAlarmCode_X = GB.TcpStatus.Detail.Comm.Tool1ServoErrorWarning_00;
			uint CurrentAlarmCode_Y = GB.TcpStatus.Detail.Comm.Tool2ServoErrorWarning_01;
			uint CurrentRunningIDNum_X = GB.TcpStatus.Detail.T1StA.TighteningIDset_00;
			uint CurrentRunningIDNum_Y = GB.TcpStatus.Detail.T2StA.TighteningIDset_00;
			uint CurrentRunningParamNum_X = GB.TcpStatus.Detail.T1StA.SeqID_02;
			uint CurrentRunningParamNum_Y = GB.TcpStatus.Detail.T2StA.SeqID_02;
			uint CurrentRunningSeqNum_X = GB.TcpStatus.Detail.T1StA.ParamID_03;
			uint CurrentRunningSeqNum_Y = GB.TcpStatus.Detail.T2StA.ParamID_03;
			uint TotalProcessNum_X = (uint)(GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09);
			uint TotalProcessNum_Y = (uint)(GB.TcpStatus.Detail.T2StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T2StA.CurrentSequence_L_09);
			uint CurrentRunningLooseningStatus_X = GB.TcpStatus.Detail.T1StB.LooseningResultOKNOKAutoClearNextRun_30;
			uint CurrentRunningLooseningStatus_Y = GB.TcpStatus.Detail.T2StB.LooseningResultOKNOKAutoClearNextRun_30;
			uint CurrentRunningTighteningStatus_X = GB.TcpStatus.Detail.T1StB.TighteningResultOKNOKAutoClearNextRun_29;
			uint CurrentRunningTighteningStatus_Y = GB.TcpStatus.Detail.T2StB.TighteningResultOKNOKAutoClearNextRun_29;
			uint CurrentPupWindowID = GB.TcpStatus.Detail.Comm.PupWindowID_38;
			string ResultBar_X = GB.GetNameTitleStr(FormType.SubResultBarcodeX, 0);
			string ResultBar_Y = GB.GetNameTitleStr(FormType.SubResultBarcodeY, 0);
			if (OrgResultBar_X != ResultBar_X || OrgAlarmCode_X != CurrentAlarmCode_X || OrgRunningIDNum_X != CurrentRunningIDNum_X || OrgRunningParamNum_X != CurrentRunningParamNum_X || OrgRunningSeqNum_X != CurrentRunningSeqNum_X || OrgCurveCntX != GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_23 || OrgTotalProcessNum_X != TotalProcessNum_X || OrgTighteningStatus_X != CurrentRunningTighteningStatus_X || OrgLooseningStatus_X != CurrentRunningLooseningStatus_X || OrgResultBar_Y != ResultBar_Y || OrgAlarmCode_Y != CurrentAlarmCode_Y || OrgRunningIDNum_Y != CurrentRunningIDNum_Y || OrgRunningParamNum_Y != CurrentRunningParamNum_Y || OrgRunningSeqNum_Y != CurrentRunningSeqNum_Y || OrgCurveCntY != GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_24 || OrgTotalProcessNum_Y != TotalProcessNum_Y || OrgTighteningStatus_Y != CurrentRunningTighteningStatus_Y || OrgLooseningStatus_Y != CurrentRunningLooseningStatus_Y || OrgPupWindowID != CurrentPupWindowID)
			{
				BackGroundfg = true;
				Form400fg = true;
				Form409fg = true;
				OrgAlarmCode_X = CurrentAlarmCode_X;
				OrgRunningIDNum_X = CurrentRunningIDNum_X;
				OrgRunningParamNum_X = CurrentRunningParamNum_X;
				OrgRunningSeqNum_X = CurrentRunningSeqNum_X;
				OrgAlarmCode_Y = CurrentAlarmCode_Y;
				OrgRunningIDNum_Y = CurrentRunningIDNum_Y;
				OrgRunningParamNum_Y = CurrentRunningParamNum_Y;
				OrgRunningSeqNum_Y = CurrentRunningSeqNum_Y;
				OrgCurveCntX = GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_23;
				OrgCurveCntY = GB.TcpStatus.Detail.Comm.ResultCurveReflashCount_24;
				OrgTotalProcessNum_X = TotalProcessNum_X;
				OrgTotalProcessNum_Y = TotalProcessNum_Y;
				OrgTighteningStatus_X = CurrentRunningTighteningStatus_X;
				OrgTighteningStatus_Y = CurrentRunningTighteningStatus_Y;
				OrgLooseningStatus_X = CurrentRunningLooseningStatus_X;
				OrgLooseningStatus_Y = CurrentRunningLooseningStatus_Y;
				OrgPupWindowID = CurrentPupWindowID;
				OrgResultBar_X = ResultBar_X;
				OrgResultBar_Y = ResultBar_Y;
			}
			ushort OPTimeH_X = GB.TcpStatus.Detail.T1StA.RemainingOperationTime_H_49;
			ushort OPTimeL_X = GB.TcpStatus.Detail.T1StA.RemainingOperationTime_L_48;
			ushort TGNOKCntH_X = GB.TcpStatus.Detail.T1StA.TighteningNOKCnt_H_14;
			ushort TGNOKCntL_X = GB.TcpStatus.Detail.T1StA.TighteningNOKCnt_L_13;
			ushort LONOKCntH_X = GB.TcpStatus.Detail.T1StA.LooseningOKCnt_H_16;
			ushort LONOKCntL_X = GB.TcpStatus.Detail.T1StA.LooseningNOKCnt_L_17;
			ushort OPTimeH_Y = GB.TcpStatus.Detail.T2StA.RemainingOperationTime_H_49;
			ushort OPTimeL_Y = GB.TcpStatus.Detail.T2StA.RemainingOperationTime_L_48;
			ushort TGNOKCntH_Y = GB.TcpStatus.Detail.T2StA.TighteningNOKCnt_H_14;
			ushort TGNOKCntL_Y = GB.TcpStatus.Detail.T2StA.TighteningNOKCnt_L_13;
			ushort LONOKCntH_Y = GB.TcpStatus.Detail.T2StA.LooseningOKCnt_H_16;
			ushort LONOKCntL_Y = GB.TcpStatus.Detail.T2StA.LooseningNOKCnt_L_17;
			if (OrgOPTimeH_X != OPTimeH_X || OrgOPTimeL_X != OPTimeL_X || OrgTGNOKCntH_X != TGNOKCntH_X || OrgTGNOKCntL_X != TGNOKCntL_X || OrgLONOKCntH_X != LONOKCntH_X || OrgLONOKCntL_X != LONOKCntL_X || OrgOPTimeH_Y != OPTimeH_Y || OrgOPTimeL_Y != OPTimeL_Y || OrgTGNOKCntH_Y != TGNOKCntH_Y || OrgTGNOKCntL_Y != TGNOKCntL_Y || OrgLONOKCntH_Y != LONOKCntH_Y || OrgLONOKCntL_Y != LONOKCntL_Y)
			{
				Form409fg = true;
				OrgOPTimeH_X = OPTimeH_X;
				OrgOPTimeL_X = OPTimeL_X;
				OrgTGNOKCntH_X = TGNOKCntH_X;
				OrgTGNOKCntL_X = TGNOKCntL_X;
				OrgLONOKCntH_X = LONOKCntH_X;
				OrgLONOKCntL_X = LONOKCntL_X;
				OrgOPTimeH_Y = OPTimeH_Y;
				OrgOPTimeL_Y = OPTimeL_Y;
				OrgTGNOKCntH_Y = TGNOKCntH_Y;
				OrgTGNOKCntL_Y = TGNOKCntL_Y;
				OrgLONOKCntH_Y = LONOKCntH_Y;
				OrgLONOKCntL_Y = LONOKCntL_Y;
			}
			if (GB.BackGroundEvent != null && GB.BackGroundThreadWait && BackGroundfg)
			{
				GB.BackGroundEvent.Set();
				GB.BackGroundThreadWait = false;
				BackGroundfg = false;
			}
			if (GB.Form400Event != null && GB.Form400ThreadWait && Form400fg)
			{
				GB.Form400Event.Set();
				GB.Form400ThreadWait = false;
				Form400fg = false;
			}
			if (GB.Form409Event != null && GB.Form409ThreadWait && Form409fg)
			{
				GB.Form409Event.Set();
				GB.Form409ThreadWait = false;
				Form409fg = false;
			}
			uint CurrentTorqueDW_X = (uint)(GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_L_06);
			uint CurrentTorqueDW_Y = (uint)(GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_L_06);
			if (OrgTorqueDW_X != CurrentTorqueDW_X || OrgTorqueDW_Y != CurrentTorqueDW_Y)
			{
				if (GB.Form600Event != null && GB.Form600ThreadWait)
				{
					GB.Form600Event.Set();
					GB.Form600ThreadWait = false;
				}
				OrgTorqueDW_X = CurrentTorqueDW_X;
				OrgTorqueDW_Y = CurrentTorqueDW_Y;
			}
			ushort ReflashAddr = (ushort)(GB.TcpStatus.Detail.Comm.HMIReflashAddrAndVal_26 / 1000);
			ushort ReflashVal = (ushort)(GB.TcpStatus.Detail.Comm.HMIReflashAddrAndVal_26 - ReflashAddr * 1000);
			if (ReflashAddr < 10)
			{
				if (OrgReflashArray[ReflashAddr] != ReflashVal)
				{
					OrgReflashArray[ReflashAddr] = ReflashVal;
					Array.Clear(AlreadyReflashFlag, 0, 10);
					Array.Clear(NowReflashArray, 0, 10);
					DetectDiffReflashFlag = true;
				}
				if (DetectDiffReflashFlag)
				{
					NowReflashArray[ReflashAddr] = ReflashVal;
					AlreadyReflashFlag[ReflashAddr] = true;
				}
				bool StartReflashFlag = true;
				for (int i = 0; i < 10; i++)
				{
					StartReflashFlag &= AlreadyReflashFlag[i];
				}
				if (StartReflashFlag && DetectDiffReflashFlag)
				{
					DetectDiffReflashFlag = false;
					Console.WriteLine("== 0:" + NowReflashArray[0] + "== 1:" + NowReflashArray[1] + "== 2:" + NowReflashArray[2] + "== 3:" + NowReflashArray[3] + "== 4:" + NowReflashArray[4] + "== 5:" + NowReflashArray[5] + "== 6:" + NowReflashArray[6] + "== 7:" + NowReflashArray[7] + "== 8:" + NowReflashArray[8] + "== 9:" + NowReflashArray[9]);
					if (GB.ReflashEvent != null && GB.ReflashThreadWait)
					{
						GB.ReflashEvent.Set();
						GB.ReflashThreadWait = false;
					}
				}
			}
			if (LastKeepalive != GB.TcpStatus.Detail.Comm.Keepalive_19)
			{
				LastKeepalive = GB.TcpStatus.Detail.Comm.Keepalive_19;
				MissKeepaliveCnt = 0;
			}
		}

		private unsafe void ReceiveCallbackTCP(IAsyncResult ar)
		{
			try
			{
				StateObject state = null;
				state = (StateObject)ar.AsyncState;
				Socket server = state.workSocket;
				int TcpByteSize = server.EndReceive(ar);
				if (TcpByteSize > 0)
				{
					int Type = BitConverter.ToInt16(state.aData8, 14);
					int OKNG = BitConverter.ToInt16(state.aData8, 16);
					int ErrCode = BitConverter.ToInt16(state.aData8, 18);
					if (Type == 10 || Type == 50)
					{
						for (int j = 0; j < TcpByteSize; j++)
						{
							GB.TcpStatus.Data8[j] = state.aData8[j];
						}
						ParseTCPComm();
					}
					else if (Type > 0)
					{
						if (OKNG == 1)
						{
							for (int i = 0; i < TcpByteSize; i++)
							{
								GB.TcpRD.Data8[i] = state.aData8[i];
							}
							if (GB.TCPHandshakeEvent != null && GB.TCPHandshakeWait)
							{
								MissKeepaliveCnt = 0;
								GB.TCPHandshakeEvent.Set();
								GB.TCPHandshakeWait = false;
							}
						}
						else
						{
							for (int k = 0; k < 10; k++)
							{
								GB.TcpRD.Data8[k] = state.aData8[k];
							}
							if (GB.TCPHandshakeEvent != null && GB.TCPHandshakeWait)
							{
								MissKeepaliveCnt = 0;
								GB.TCPHandshakeEvent.Set();
								GB.TCPHandshakeWait = false;
							}
							Console.WriteLine("Type:{0},Err:{1}", Type, ErrCode);
						}
					}
				}
				state.workSocket.BeginReceive(state.aData8, 0, 65535, SocketFlags.None, ReceiveCallbackTCP, state);
			}
			catch
			{
				Console.WriteLine("Tcp ReceiveCallback out!");
				RetryConnect();
			}
		}

		public void RetryConnect()
		{
			if (CommunicationType == 0)
			{
				ConnectStatus = false;
				if (clientSocket != null)
				{
					clientSocket.Close();
				}
				ConnectFunc();
			}
			else
			{
				ConnectStatus = false;
				if (udpClient != null)
				{
					udpClient.Close();
				}
				ConnectFunc();
			}
			GB.ClearReportList(0);
		}

		private static byte[] BuildUdpHeader(IPEndPoint endPoint, int dataLength)
		{
			byte[] udpHeader = new byte[8];
			ushort sourcePort = 12347;
			udpHeader[0] = (byte)(sourcePort >> 8);
			udpHeader[1] = (byte)(sourcePort & 0xFF);
			udpHeader[2] = (byte)(endPoint.Port >> 8);
			udpHeader[3] = (byte)(endPoint.Port & 0xFF);
			ushort udpLength = (ushort)(8 + dataLength);
			udpHeader[4] = (byte)(udpLength >> 8);
			udpHeader[5] = (byte)(udpLength & 0xFF);
			udpHeader[6] = 0;
			udpHeader[7] = 0;
			return udpHeader;
		}

		public unsafe void ConnectFunc()
		{
			try
			{
				if (CommunicationType == 0)
				{
					clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					clientSocket.ReceiveTimeout = 3000;
					clientSocket.SendTimeout = 3000;
					IAsyncResult result = clientSocket.BeginConnect(GB.UISys.IPstr, 1001, null, null);
					if (result.AsyncWaitHandle.WaitOne(5000, true))
					{
						FirstConnect = true;
						ConnectStatus = true;
						ConnectInterrupt = false;
						clientSocket.EndConnect(result);
						ReceiveEventTCP(clientSocket);
					}
					else
					{
						Console.WriteLine("Tcp ConnectFunc Timeout!");
						RetryConnect();
					}
					return;
				}
				GB.FSFTPIP.IP[0] = 255;
				GB.FSFTPIP.IP[1] = 255;
				GB.FSFTPIP.IP[2] = 255;
				GB.FSFTPIP.IP[3] = 255;
				if (GB.UISys.IsReadSupportFTPServer)
				{
					NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
					foreach (NetworkInterface networkInterface in allNetworkInterfaces)
					{
						IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();
						List<string> ipv4Addresses = (from ip in ipProperties.UnicastAddresses
							where ip.Address.AddressFamily == AddressFamily.InterNetwork
							select ip.Address.ToString()).ToList();
						foreach (string ipv4 in ipv4Addresses)
						{
							IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(GB.UISys.IPstr), 602);
							IPEndPoint PCPoint = new IPEndPoint(IPAddress.Parse(ipv4), 602);
							using (UdpClient udpTestClient = new UdpClient())
							{
								try
								{
									udpTestClient.Client.SendTimeout = 100;
									udpTestClient.Client.ReceiveTimeout = 300;
									udpTestClient.Client.Bind(PCPoint);
									byte[] SendStr = new byte[26]
									{
										101, 68, 13, 0, 13, 0, 0, 0, 0, 0,
										0, 0, 0, 0, 0, 0, 1, 0, 0, 0,
										0, 0, 0, 0, 116, 108
									};
									byte[] expectedData = new byte[26]
									{
										101, 68, 13, 0, 13, 0, 10, 0, 0, 0,
										0, 0, 0, 0, 0, 0, 0, 0, 13, 0,
										1, 0, 0, 0, 116, 108
									};
									int RstByteLen = udpTestClient.Send(SendStr, SendStr.Length, endPoint);
									byte[] receivedData = udpTestClient.Receive(ref PCPoint);
									if (receivedData.SequenceEqual(expectedData))
									{
										byte[] ipBytes = IPAddress.Parse(ipv4).GetAddressBytes();
										ref ushort iP = ref GB.FSFTPIP.IP[0];
										iP = ipBytes[0];
										GB.FSFTPIP.IP[1] = ipBytes[1];
										GB.FSFTPIP.IP[2] = ipBytes[2];
										GB.FSFTPIP.IP[3] = ipBytes[3];
										Console.WriteLine("Host Port: " + ipv4);
										break;
									}
								}
								catch (SocketException ex)
								{
									if (ex.SocketErrorCode != SocketError.TimedOut)
									{
									}
								}
							}
						}
					}
				}
				this.endPoint = new IPEndPoint(MatchIP = IPAddress.Parse(GB.UISys.IPstr), 602);
				this.PCPoint = new IPEndPoint(IPAddress.Any, 602);
				udpClient = new UdpClient();
				udpClient.Client.SendTimeout = 3000;
				udpClient.Client.ReceiveTimeout = 3000;
				udpClient.Client.Bind(this.PCPoint);
				IAsyncResult result2 = udpClient.BeginReceive(ReceiveCallbackUDP, this.endPoint);
				PingReply reply = ping.Send(GB.UISys.IPstr, 500);
				if (reply.Status == IPStatus.Success)
				{
					FirstConnect = true;
					ConnectStatus = true;
					ConnectInterrupt = false;
					if (GB.Form001TCPWait)
					{
						GB.Form001TCPEvent.Set();
						GB.Form001TCPWait = false;
					}
				}
			}
			catch (SocketException ex2)
			{
				if (CommunicationType == 0)
				{
					Console.WriteLine("Tcp ConnectFunc out!");
					RetryConnect();
				}
				else
				{
					Console.WriteLine("Udp ConnectFunc out!");
					Console.WriteLine("SocketException: " + ex2.Message);
				}
			}
		}

		public void StopTCPConnect()
		{
			DisConnectFunc();
			GB.UISys.PCSoftSupport = false;
			ConnectStatus = false;
			ConnectInterrupt = false;
		}

		public void DisConnectFunc()
		{
			try
			{
				if (CommunicationType == 0)
				{
					FSIDWrite_ByTCP(11, 0, 0, 0, 0, 0);
					ConnectStatus = false;
					ConnectInterrupt = false;
					if (clientSocket != null)
					{
						clientSocket.Shutdown(SocketShutdown.Both);
						clientSocket.Close();
					}
				}
				else
				{
					FSIDWrite_ByTCP(13, 0, 0, 0, 0, 0);
					ConnectStatus = false;
					ConnectInterrupt = false;
					if (udpClient != null)
					{
						udpClient.Close();
					}
				}
			}
			catch
			{
			}
		}

		public unsafe bool SendUDP(uint DataSize)
		{
			bool ret = false;
			try
			{
				GB.TcpWR.Flag = 1;
				byte[] SendStr = new byte[(3 + DataSize) * 2];
				SendStr[0] = 101;
				SendStr[1] = 68;
				SendStr[2] = (byte)(3 + DataSize);
				SendStr[3] = (byte)(3 + DataSize >> 8);
				for (int j = 0; j < 2 * DataSize; j++)
				{
					SendStr[4 + j] = GB.TcpWR.Data8[j];
				}
				SendStr[4 + DataSize * 2] = 116;
				SendStr[4 + DataSize * 2 + 1] = 108;
				if (DataSize != 0 && udpClient != null && ConnectStatus)
				{
					int RstByteLen = udpClient.Send(SendStr, SendStr.Length, endPoint);
					ret = true;
				}
			}
			catch (SocketException ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return ret;
		}

		public unsafe bool SendTCP(uint DataSize)
		{
			bool ret = false;
			try
			{
				GB.TcpWR.Flag = 1;
				List<byte> LData = new List<byte>();
				for (int j = 0; j < 2 * DataSize; j++)
				{
					LData.Add(GB.TcpWR.Data8[j]);
				}
				if (DataSize != 0 && clientSocket != null && ConnectStatus)
				{
					clientSocket.Send(LData.ToArray());
					ret = true;
				}
			}
			catch
			{
			}
			return ret;
		}

		public unsafe void CaheClear(int WordSize)
		{
			for (int i = 0; i < WordSize; i++)
			{
				GB.TcpWR.Data16[i] = 0;
				GB.TcpRD.Data16[i] = 0;
			}
		}

		public void BinFileFolderDelete()
		{
			string[] files = Directory.GetFiles(GB.UISys.FTPSavePath);
			foreach (string file in files)
			{
				File.Delete(file);
			}
		}

		public unsafe int FSIDWrite_ByTCP(ushort TcpType, ushort Ver, ushort D1, ushort D2, ushort D3, ushort D4)
		{
			uint DataSize = 10u;
			int Err = 0;
			CaheClear(10);
			GB.TcpWR.CmdFunc = TcpType;
			GB.TcpWR.ID = Ver;
			GB.TcpWR.Data1 = D1;
			GB.TcpWR.Data2 = D2;
			GB.TcpWR.Data3 = D3;
			GB.TcpWR.Data4 = D4;
			try
			{
				switch (TcpType)
				{
				case 21:
				{
					DataSize = (uint)(10 + sizeof(CtrlMappingTableStuc) / 2);
					for (uint i9 = 0u; i9 < sizeof(CtrlMappingTableStuc) / 2; i9++)
					{
						GB.TcpWR.Data16[i9 + 10] = GB.FSCtrlMappingTable.Data16[i9];
					}
					break;
				}
				case 54:
				{
					DataSize = (uint)(10 + D4);
					for (uint i15 = 0u; i15 < D4; i15++)
					{
						GB.TcpWR.Data16[i15 + 10] = GB.FSCtrlLocalTable.Data16[i15];
					}
					break;
				}
				case 100:
					if (Ver == 0)
					{
						DataSize = (uint)(10 + sizeof(ParamStucVer0) / 2);
						for (uint i24 = 0u; i24 < sizeof(ParamStucVer0) / 2; i24++)
						{
							GB.TcpWR.Data16[i24 + 10] = GB.SendReadParamStucVer0.Data16[i24];
						}
					}
					else
					{
						DataSize = (uint)(10 + sizeof(ParamStucVer1) / 2);
						for (uint i25 = 0u; i25 < sizeof(ParamStucVer1) / 2; i25++)
						{
							GB.TcpWR.Data16[i25 + 10] = GB.SendReadParamStucVer1.Data16[i25];
						}
					}
					break;
				case 200:
				{
					DataSize = (uint)(10 + sizeof(SeqBaseStuc) / 2);
					for (uint i12 = 0u; i12 < sizeof(SeqBaseStuc) / 2; i12++)
					{
						GB.TcpWR.Data16[i12 + 10] = GB.SendReadSeqStucVer0.Data16[i12];
					}
					break;
				}
				case 201:
				{
					DataSize = (uint)(10 + sizeof(SeqNavigationCoordinateXY) / 2);
					for (uint i5 = 0u; i5 < sizeof(SeqNavigationCoordinateXY) / 2; i5++)
					{
						GB.TcpWR.Data16[i5 + 10] = GB.FSSeqLedXY[D1 - 1].Data16[i5];
					}
					break;
				}
				case 202:
				{
					DataSize = (uint)(10 + sizeof(SeqNavigationPictureStuc) / 2);
					for (uint i20 = 0u; i20 < sizeof(SeqNavigationPictureStuc) / 2; i20++)
					{
						GB.TcpWR.Data16[i20 + 10] = GB.FSSeqPicABC[D1 - 1].ID[i20];
					}
					break;
				}
				case 203:
				{
					DataSize = (uint)(10 + sizeof(SeqArmPositionXYZ) / 2);
					for (uint i16 = 0u; i16 < sizeof(SeqArmPositionXYZ) / 2; i16++)
					{
						GB.TcpWR.Data16[i16 + 10] = GB.FSSeqArmXYZ[D1 - 1].Data16[i16];
					}
					break;
				}
				case 211:
				{
					DataSize = 10 + (uint)D4 / 2u;
					for (uint i10 = 0u; i10 < D4 / 2; i10++)
					{
						GB.TcpWR.Data16[i10 + 10] = GB.FSPicBitMap[i10];
					}
					break;
				}
				case 301:
				{
					DataSize = (uint)(10 + sizeof(SrcStuc) / 2);
					for (uint i7 = 0u; i7 < sizeof(SrcStuc) / 2; i7++)
					{
						switch (D3)
						{
						case 0:
							if (D1 == 0)
							{
								switch (D4)
								{
								case 0:
									GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcManualX[D2 - 1].Data16[i7];
									break;
								case 1:
									GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcBitsX[D2 - 1].Data16[i7];
									break;
								default:
									GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcScannerX[D2 - 1].Data16[i7];
									break;
								}
							}
							else
							{
								switch (D4)
								{
								case 0:
									GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcManualY[D2 - 1].Data16[i7];
									break;
								case 1:
									GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcBitsY[D2 - 1].Data16[i7];
									break;
								default:
									GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcScannerY[D2 - 1].Data16[i7];
									break;
								}
							}
							break;
						case 1:
							switch (D4)
							{
							case 0:
								GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcManual_DualMix[D2 - 1].Data16[i7];
								break;
							case 1:
								GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcBits_DualMix[D2 - 1].Data16[i7];
								break;
							default:
								GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcScanner_DualMix[D2 - 1].Data16[i7];
								break;
							}
							break;
						default:
							switch (D4)
							{
							case 0:
								GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcManual_DualSync[D2 - 1].Data16[i7];
								break;
							case 1:
								GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcBits_DualSync[D2 - 1].Data16[i7];
								break;
							default:
								GB.TcpWR.Data16[i7 + 10] = GB.FSSrcAll.FSSrcScanner_DualSync[D2 - 1].Data16[i7];
								break;
							}
							break;
						}
					}
					break;
				}
				case 401:
				{
					DataSize = (uint)(10 + sizeof(ResultBarcodeStuc) / 2);
					for (uint i6 = 0u; i6 < sizeof(ResultBarcodeStuc) / 2; i6++)
					{
						if (D1 == 0)
						{
							GB.TcpWR.Data16[i6 + 10] = GB.FSResultBarcodeX.Data16[i6];
						}
						else
						{
							GB.TcpWR.Data16[i6 + 10] = GB.FSResultBarcodeY.Data16[i6];
						}
					}
					break;
				}
				case 405:
					GB.ResultLedStatusFunc(D1, (uint)(D3 * 65535 + D2 + 1), 5);
					break;
				case 408:
				{
					DataSize = (uint)(10 + sizeof(ResultBarcodeAdvanceSettingStuc) / 2);
					for (uint i22 = 0u; i22 < sizeof(ResultBarcodeAdvanceSettingStuc) / 2; i22++)
					{
						if (D1 == 0)
						{
							GB.TcpWR.Data16[i22 + 10] = GB.FSResultBarcodeAdvanceSettingX.Data16[i22];
						}
						else
						{
							GB.TcpWR.Data16[i22 + 10] = GB.FSResultBarcodeAdvanceSettingY.Data16[i22];
						}
					}
					break;
				}
				case 500:
				{
					DataSize = (uint)(10 + sizeof(CtrlUserLogInStuc) / 2);
					for (uint i18 = 0u; i18 < sizeof(CtrlUserLogInStuc) / 2; i18++)
					{
						GB.TcpWR.Data16[i18 + 10] = GB.FSCtrlUserLogIn.Data16[i18];
					}
					break;
				}
				case 501:
				{
					DataSize = (uint)(10 + sizeof(CtrlUserPasswordStuc) / 2);
					for (uint i14 = 0u; i14 < sizeof(CtrlUserPasswordStuc) / 2; i14++)
					{
						GB.TcpWR.Data16[i14 + 10] = GB.FSCtrlUserPassword.Data16[i14];
					}
					break;
				}
				case 503:
				{
					DataSize = (uint)(10 + sizeof(CtrlPageAuthorityStuc) / 2);
					for (uint i13 = 0u; i13 < sizeof(CtrlPageAuthorityStuc) / 2; i13++)
					{
						GB.TcpWR.Data16[i13 + 10] = GB.FSCtrlPageAuthority.Data16[i13];
					}
					break;
				}
				case 504:
				{
					DataSize = (uint)(10 + sizeof(CtrlEthernetStuc) / 2);
					for (uint i11 = 0u; i11 < sizeof(CtrlEthernetStuc) / 2; i11++)
					{
						GB.TcpWR.Data16[i11 + 10] = GB.FSCtrlEthernet.Data16[i11];
					}
					if (GB.CheckHMIVer(170, 3))
					{
						GB.TcpWR.Data2 = GB.FSCtrlEthernet.TCPServerPort;
					}
					break;
				}
				case 507:
				{
					DataSize = (uint)(10 + sizeof(CtrlDIOFunctionStuc) / 2);
					for (uint i8 = 0u; i8 < sizeof(CtrlDIOFunctionStuc) / 2; i8++)
					{
						if (D1 == 0)
						{
							GB.TcpWR.Data16[i8 + 10] = GB.FSCtrlDIOFunction_X.Data16[i8];
						}
						else
						{
							GB.TcpWR.Data16[i8 + 10] = GB.FSCtrlDIOFunction_Y.Data16[i8];
						}
					}
					break;
				}
				case 508:
				{
					DataSize = (uint)(10 + sizeof(CtrlDIOTableStuc) / 2);
					for (uint i4 = 0u; i4 < sizeof(CtrlDIOTableStuc) / 2; i4++)
					{
						if (D1 == 0)
						{
							switch (D2)
							{
							case 0:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOBitsTable_X.Data16[i4];
								break;
							case 1:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDIBitsTable_X.Data16[i4];
								break;
							case 2:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOParamTable_X.Data16[i4];
								break;
							case 4:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOScrewTable_X.Data16[i4];
								break;
							case 6:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOSeqTable_X.Data16[i4];
								break;
							}
						}
						else
						{
							switch (D2)
							{
							case 0:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOBitsTable_Y.Data16[i4];
								break;
							case 1:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDIBitsTable_Y.Data16[i4];
								break;
							case 2:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOParamTable_Y.Data16[i4];
								break;
							case 4:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOScrewTable_Y.Data16[i4];
								break;
							case 6:
								GB.TcpWR.Data16[i4 + 10] = GB.FSCtrlDOSeqTable_Y.Data16[i4];
								break;
							}
						}
					}
					break;
				}
				case 521:
				{
					DataSize = (uint)(10 + sizeof(CtrlComPortFunctionStuc) / 2);
					for (uint i2 = 0u; i2 < sizeof(CtrlComPortFunctionStuc) / 2; i2++)
					{
						GB.TcpWR.Data16[i2 + 10] = GB.FSCtrlComPortFunction.Data16[i2];
					}
					break;
				}
				case 528:
				{
					DataSize = (uint)(10 + sizeof(CtrlRS485FunctionStuc) / 2);
					for (uint i26 = 0u; i26 < sizeof(CtrlRS485FunctionStuc) / 2; i26++)
					{
						GB.TcpWR.Data16[i26 + 10] = GB.FSCtrlRS485Function.Data16[i26];
					}
					break;
				}
				case 535:
				{
					DataSize = (uint)(10 + sizeof(CtrlModelNameStuc) / 2);
					for (uint i23 = 0u; i23 < sizeof(CtrlModelNameStuc) / 2; i23++)
					{
						GB.TcpWR.Data16[i23 + 10] = GB.FSCtrlModelName.Data16[i23];
					}
					break;
				}
				case 538:
				{
					DataSize = (uint)(10 + sizeof(CtrlDOTimerStuc) / 2);
					for (uint i21 = 0u; i21 < sizeof(CtrlDOTimerStuc) / 2; i21++)
					{
						if (D1 == 0)
						{
							GB.TcpWR.Data16[i21 + 10] = GB.FSCtrlDOTimer_X.Data16[i21];
						}
						else
						{
							GB.TcpWR.Data16[i21 + 10] = GB.FSCtrlDOTimer_Y.Data16[i21];
						}
					}
					break;
				}
				case 606:
				{
					DataSize = (uint)(10 + sizeof(ToolLEDlightStuc) / 2);
					for (uint i19 = 0u; i19 < sizeof(ToolLEDlightStuc) / 2; i19++)
					{
						if (D1 == 0)
						{
							GB.TcpWR.Data16[i19 + 10] = GB.FSToolXLedLight.Data16[i19];
						}
						else
						{
							GB.TcpWR.Data16[i19 + 10] = GB.FSToolYLedLight.Data16[i19];
						}
					}
					break;
				}
				case 607:
				{
					DataSize = (uint)(10 + sizeof(ToolCalibrationVer1Stuc) / 2);
					for (uint i17 = 0u; i17 < sizeof(ToolCalibrationVer1Stuc) / 2; i17++)
					{
						GB.TcpWR.Data16[i17 + 10] = GB.FSToolCalibrationVer1.Data16[i17];
					}
					break;
				}
				case 805:
					if (D1 == 199)
					{
						GB.TcpWR.Data1 = (ushort)(D1 - 100);
						string[] files = Directory.GetFiles(GB.UISys.FTPSavePath);
						foreach (string file in files)
						{
							File.Delete(file);
						}
					}
					if (D4 == 2 || D4 == 99)
					{
						GB.TcpWR.ID = (ushort)(GB.CheckHMIVer(170, 6) ? 1 : 0);
						DataSize = (uint)(10 + sizeof(ExFTPIPStuc) / 2);
						for (uint i3 = 0u; i3 <= 3; i3++)
						{
							GB.TcpWR.Data16[i3 + 10] = GB.FSFTPIP.IP[i3];
						}
						GB.TcpWR.Data16[14] = (ushort)(GB.CheckHMIVer(170, 6) ? ((ushort)GB.UISys.passivePort) : 603);
					}
					break;
				case 1514:
				{
					DataSize = 20u;
					for (uint i = 0u; i < 10; i++)
					{
						switch (D1)
						{
						case 0:
							GB.TcpWR.Data16[10 + i] = GB.ExFSUser.User1Name[i];
							break;
						case 1:
							GB.TcpWR.Data16[10 + i] = GB.ExFSUser.User2Name[i];
							break;
						case 2:
							GB.TcpWR.Data16[10 + i] = GB.ExFSUser.User3Name[i];
							break;
						case 3:
							GB.TcpWR.Data16[10 + i] = GB.ExFSUser.User4Name[i];
							break;
						case 4:
							GB.TcpWR.Data16[10 + i] = GB.ExFSUser.User5Name[i];
							break;
						case 5:
							GB.TcpWR.Data16[10 + i] = GB.ExFSUser.User6Name[i];
							break;
						}
					}
					break;
				}
				}
				bool SndRst = false;
				if (!((CommunicationType != 0) ? SendUDP(DataSize) : SendTCP(DataSize)))
				{
					return -4;
				}
				for (int Loop = 0; Loop <= 2; Loop++)
				{
					bool rst = false;
					if (TcpType == 10 || TcpType == 12)
					{
						rst = true;
					}
					else
					{
						GB.TCPHandshakeWait = true;
						switch (TcpType)
						{
						case 211:
						case 300:
						case 301:
							rst = GB.TCPHandshakeEvent.WaitOne(5000);
							break;
						case 505:
						case 805:
							rst = ((D4 != 99) ? GB.TCPHandshakeEvent.WaitOne(60000) : GB.TCPHandshakeEvent.WaitOne(1500));
							break;
						case 607:
							rst = ((D4 != 19) ? GB.TCPHandshakeEvent.WaitOne(1500) : GB.TCPHandshakeEvent.WaitOne(60000));
							break;
						default:
							rst = GB.TCPHandshakeEvent.WaitOne(1500);
							break;
						}
					}
					if (rst)
					{
						Err = ((GB.TcpRD.OKNG != 1) ? GB.TcpRD.ErrCode : 0);
						break;
					}
					if (TcpType == 805)
					{
						rst = true;
						Err = -2;
					}
					else if (Loop >= 2)
					{
						Err = -5;
					}
				}
			}
			catch (Exception ex)
			{
				Err = -6;
				string errorMessage = ex.Message + " Err No." + ex.StackTrace;
				FormPublicFunction.SaveErrLog(errorMessage);
			}
			return Err;
		}

		public unsafe int FSIDRead_ByTCP(ushort TcpType, ushort Ver, ushort D1, ushort D2, ushort D3, ushort D4)
		{
			uint DataSize = 10u;
			int Err = 0;
			CaheClear(10);
			GB.TcpWR.CmdFunc = TcpType;
			GB.TcpWR.ID = Ver;
			GB.TcpWR.Data1 = D1;
			GB.TcpWR.Data2 = D2;
			GB.TcpWR.Data3 = D3;
			GB.TcpWR.Data4 = D4;
			GB.TcpWR.FedFunc = 0;
			GB.TcpWR.OKNG = 0;
			GB.TcpWR.ErrCode = 0;
			if (TcpType == 82)
			{
				for (uint i = 0u; i < sizeof(CtrlStaticReadStuc) / 2; i++)
				{
					if (GB.FSCtrlStaticRead.Data16[i] > 0)
					{
						GB.TcpWR.Data16[10 + i] = (ushort)(GB.FSCtrlStaticRead.Data16[i] + 1);
					}
				}
				DataSize = 10 + (uint)sizeof(CtrlStaticReadStuc) / 2u;
			}
			else
			{
				DataSize = 10u;
			}
			for (int Loop = 0; Loop <= 2; Loop++)
			{
				bool SndRst = false;
				if (!((CommunicationType != 0) ? SendUDP(DataSize) : SendTCP(DataSize)))
				{
					return -4;
				}
				int TimeOut = 1500;
				switch (GB.TcpWR.CmdFunc)
				{
				case 552:
					TimeOut = ((D4 != 99) ? 1500 : 1000);
					break;
				case 808:
					TimeOut = ((D2 != ushort.MaxValue || (D3 != 65534 && D3 != ushort.MaxValue)) ? 5000 : 1000);
					break;
				case 261:
				case 661:
				case 807:
				case 809:
				case 810:
					TimeOut = 5000;
					break;
				default:
					TimeOut = 1500;
					break;
				}
				bool rst = false;
				if (GB.TCPHandshakeEvent != null)
				{
					GB.TCPHandshakeWait = true;
					rst = GB.TCPHandshakeEvent.WaitOne(TimeOut);
				}
				if (!rst)
				{
					Err = -2;
				}
				if (rst && GB.TcpRD.OKNG == 1)
				{
					if (GB.TcpRD.CmdFunc == 51)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							switch (D1)
							{
							case 1:
								GB.FSMesValue.Data16 = GB.TcpRD.Data3;
								break;
							case 2:
								GB.FSMesValue.Data32 = (uint)(GB.TcpRD.Data4 * 65536 + GB.TcpRD.Data3);
								break;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 52)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i2 = 0u; i2 < GB.TcpRD.Size - 10; i2++)
							{
								GB.FSCtrlMappingTable.Data16[i2] = GB.TcpRD.Data16[10 + i2];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 53)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i3 = 0u; i3 < GB.TcpRD.Size - 10; i3++)
							{
								GB.FSCtrlLocalTable.Data16[i3] = GB.TcpRD.Data16[10 + i3];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 81)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							ushort ReflashAddr = 0;
							ushort ReflashVal = 0;
							if (Ver == 1)
							{
								for (int j = 0; j < 10; j++)
								{
									ReflashAddr = (ushort)(GB.TcpRD.Data16[10 + j] / 1000);
									ReflashVal = (ushort)(GB.TcpRD.Data16[10 + j] - ReflashAddr * 1000);
									if (ReflashAddr < 10)
									{
										NowReflashArray[ReflashAddr] = ReflashVal;
										OrgReflashArray[ReflashAddr] = ReflashVal;
										DoneReflashArray[ReflashAddr] = ReflashVal;
									}
								}
							}
							else
							{
								ReflashAddr = (ushort)(GB.TcpRD.Data2 / 1000);
								ReflashVal = (ushort)(GB.TcpRD.Data2 - ReflashAddr * 1000);
								if (ReflashAddr < 10)
								{
									NowReflashArray[ReflashAddr] = ReflashVal;
									OrgReflashArray[ReflashAddr] = ReflashVal;
									DoneReflashArray[ReflashAddr] = ReflashVal;
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 82)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (int k = 0; k < sizeof(CtrlStaticReadStuc) / 2; k++)
							{
								GB.FSCtrlStaticRead.Data16[k] = 0;
							}
							for (uint i4 = 0u; i4 < GB.TcpRD.Size - 10; i4++)
							{
								GB.FSCtrlStaticRead.Data16[i4] = GB.TcpRD.Data16[10 + i4];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 94)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								GB.FSResultPerpendicularityX.Value = GB.TcpRD.Data2;
							}
							else
							{
								GB.FSResultPerpendicularityY.Value = GB.TcpRD.Data2;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 150)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i5 = 0u; i5 < GB.TcpRD.Size - 10; i5++)
							{
								if (D1 == 0)
								{
									GB.FSParamX[D2 - 1].Data16[i5] = GB.TcpRD.Data16[10 + i5];
								}
								else
								{
									GB.FSParamY[D2 - 1].Data16[i5] = GB.TcpRD.Data16[10 + i5];
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 160)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i6 = 0u; i6 < GB.TcpRD.Size - 10; i6++)
							{
								GB.FSParamIDUsed[i6] = GB.TcpRD.Data16[10 + i6];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 250)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i7 = 0u; i7 < GB.TcpRD.Size - 10; i7++)
							{
								GB.FSSeqGB[D1 - 1].Data16[i7] = GB.TcpRD.Data16[10 + i7];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 251)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i8 = 0u; i8 < GB.TcpRD.Size - 10; i8++)
							{
								GB.FSSeqLedXY[D1 - 1].Data16[i8] = GB.TcpRD.Data16[10 + i8];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 252)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i9 = 0u; i9 < GB.TcpRD.Size - 10; i9++)
							{
								GB.FSSeqPicABC[D1 - 1].ID[i9] = GB.TcpRD.Data16[10 + i9];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 253)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i10 = 0u; i10 < GB.TcpRD.Size - 10; i10++)
							{
								GB.FSSeqArmXYZ[D1 - 1].Data16[i10] = GB.TcpRD.Data16[10 + i10];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 260)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i11 = 0u; i11 < GB.TcpRD.Size - 10; i11++)
							{
								GB.FSSeqIDUsed[i11] = GB.TcpRD.Data16[10 + i11];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 261)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D2 == ushort.MaxValue && D3 == ushort.MaxValue)
							{
								GB.SeqPicFileByteLen = (uint)(GB.TcpRD.Data3 * 65536 + GB.TcpRD.Data2);
							}
							else
							{
								for (uint i12 = 0u; i12 < GB.TcpRD.Size - 10; i12++)
								{
									GB.FSPicBitMap[i12] = GB.TcpRD.Data16[10 + i12];
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 262)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.SeqRemainSpaceSize = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 350)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSSrcMode.ActionMode = GB.TcpRD.Data16[3];
							if (D1 == 0)
							{
								GB.FSSrcMode.SwitchingMethodX = GB.TcpRD.Data16[4];
							}
							else
							{
								GB.FSSrcMode.SwitchingMethodY = GB.TcpRD.Data16[4];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 451)
					{
						for (uint i13 = 0u; i13 < GB.TcpRD.Size - 10; i13++)
						{
							if (D1 == 0)
							{
								GB.FSResultBarcodeX.Data16[i13] = GB.TcpRD.Data16[10 + i13];
							}
							else
							{
								GB.FSResultBarcodeY.Data16[i13] = GB.TcpRD.Data16[10 + i13];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 452)
					{
						for (uint i14 = 0u; i14 < GB.TcpRD.Size - 10; i14++)
						{
							if (D1 == 0)
							{
								GB.FSResultBarcodeAdvanceSettingX.Data16[i14] = GB.TcpRD.Data16[10 + i14];
							}
							else
							{
								GB.FSResultBarcodeAdvanceSettingY.Data16[i14] = GB.TcpRD.Data16[10 + i14];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 453)
					{
						for (uint i15 = 0u; i15 < GB.TcpRD.Size - 10; i15++)
						{
							if (D1 == 0)
							{
								GB.FSResultLedStatusX.Data16[i15] = GB.TcpRD.Data16[10 + i15];
							}
							else
							{
								GB.FSResultLedStatusY.Data16[i15] = GB.TcpRD.Data16[10 + i15];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 498)
					{
						for (uint i16 = 0u; i16 < GB.TcpRD.Size - 10; i16++)
						{
							if (D1 == 0)
							{
								switch (i16)
								{
								case 0u:
								case 1u:
								case 2u:
								case 3u:
								case 4u:
								case 5u:
								case 6u:
								case 7u:
								case 8u:
								case 9u:
								case 10u:
								case 11u:
								case 12u:
								case 13u:
								case 14u:
								case 15u:
								case 16u:
								case 17u:
								case 18u:
								case 19u:
								case 20u:
								case 21u:
								case 22u:
								case 23u:
								case 24u:
								case 25u:
								case 26u:
								case 27u:
								case 28u:
								case 29u:
								case 30u:
								case 31u:
								case 32u:
								case 33u:
								case 34u:
								case 35u:
								case 36u:
								case 37u:
								case 38u:
								case 39u:
								case 40u:
								case 41u:
								case 42u:
								case 43u:
								case 44u:
								case 45u:
								case 46u:
								case 47u:
								case 48u:
								case 49u:
								case 50u:
								case 51u:
								case 52u:
								case 53u:
								case 54u:
								case 55u:
								case 56u:
								case 57u:
								case 58u:
								case 59u:
								case 60u:
								case 61u:
								case 62u:
								case 63u:
								case 64u:
								case 65u:
								case 66u:
								case 67u:
								case 68u:
								case 69u:
								case 70u:
								case 71u:
								case 72u:
								case 73u:
								case 74u:
								case 75u:
								case 76u:
								case 77u:
								case 78u:
								case 79u:
								case 80u:
								case 81u:
								case 82u:
								case 83u:
								case 84u:
								case 85u:
								case 86u:
								case 87u:
								case 88u:
								case 89u:
								case 90u:
								case 91u:
								case 92u:
								case 93u:
								case 94u:
								case 95u:
								case 96u:
								case 97u:
								case 98u:
								case 99u:
									GB.UISys.RunningInfoX.Data16[i16] = GB.TcpRD.Data16[10 + i16];
									break;
								case 100u:
								case 101u:
								case 102u:
								case 103u:
								case 104u:
								case 105u:
								case 106u:
								case 107u:
								case 108u:
								case 109u:
								case 110u:
								case 111u:
								case 112u:
								case 113u:
								case 114u:
								case 115u:
								case 116u:
								case 117u:
								case 118u:
								case 119u:
								case 120u:
								case 121u:
								case 122u:
								case 123u:
								case 124u:
								case 125u:
								case 126u:
								case 127u:
								case 128u:
								case 129u:
								case 130u:
								case 131u:
								case 132u:
								case 133u:
								case 134u:
								case 135u:
								case 136u:
								case 137u:
								case 138u:
								case 139u:
								case 140u:
								case 141u:
								case 142u:
								case 143u:
								case 144u:
								case 145u:
								case 146u:
								case 147u:
								case 148u:
								case 149u:
									GB.UISys.RunningInfoX.Data16[i16 + 3] = GB.TcpRD.Data16[10 + i16];
									break;
								case 150u:
								case 151u:
								case 152u:
								case 153u:
								case 154u:
								case 155u:
								case 156u:
								case 157u:
								case 158u:
								case 159u:
								case 160u:
								case 161u:
								case 162u:
								case 163u:
								case 164u:
								case 165u:
								case 166u:
								case 167u:
								case 168u:
								case 169u:
								case 170u:
								case 171u:
								case 172u:
								case 173u:
								case 174u:
								case 175u:
								case 176u:
								case 177u:
								case 178u:
								case 179u:
								case 180u:
								case 181u:
								case 182u:
								case 183u:
								case 184u:
								case 185u:
								case 186u:
								case 187u:
								case 188u:
								case 189u:
								case 190u:
								case 191u:
								case 192u:
								case 193u:
								case 194u:
								case 195u:
								case 196u:
								case 197u:
								case 198u:
								case 199u:
									GB.UISys.RunningScaleX.Data16[i16 - 150] = GB.TcpRD.Data16[10 + i16];
									break;
								}
							}
							else
							{
								switch (i16)
								{
								case 0u:
								case 1u:
								case 2u:
								case 3u:
								case 4u:
								case 5u:
								case 6u:
								case 7u:
								case 8u:
								case 9u:
								case 10u:
								case 11u:
								case 12u:
								case 13u:
								case 14u:
								case 15u:
								case 16u:
								case 17u:
								case 18u:
								case 19u:
								case 20u:
								case 21u:
								case 22u:
								case 23u:
								case 24u:
								case 25u:
								case 26u:
								case 27u:
								case 28u:
								case 29u:
								case 30u:
								case 31u:
								case 32u:
								case 33u:
								case 34u:
								case 35u:
								case 36u:
								case 37u:
								case 38u:
								case 39u:
								case 40u:
								case 41u:
								case 42u:
								case 43u:
								case 44u:
								case 45u:
								case 46u:
								case 47u:
								case 48u:
								case 49u:
								case 50u:
								case 51u:
								case 52u:
								case 53u:
								case 54u:
								case 55u:
								case 56u:
								case 57u:
								case 58u:
								case 59u:
								case 60u:
								case 61u:
								case 62u:
								case 63u:
								case 64u:
								case 65u:
								case 66u:
								case 67u:
								case 68u:
								case 69u:
								case 70u:
								case 71u:
								case 72u:
								case 73u:
								case 74u:
								case 75u:
								case 76u:
								case 77u:
								case 78u:
								case 79u:
								case 80u:
								case 81u:
								case 82u:
								case 83u:
								case 84u:
								case 85u:
								case 86u:
								case 87u:
								case 88u:
								case 89u:
								case 90u:
								case 91u:
								case 92u:
								case 93u:
								case 94u:
								case 95u:
								case 96u:
								case 97u:
								case 98u:
								case 99u:
									GB.UISys.RunningInfoY.Data16[i16] = GB.TcpRD.Data16[10 + i16];
									break;
								case 100u:
								case 101u:
								case 102u:
								case 103u:
								case 104u:
								case 105u:
								case 106u:
								case 107u:
								case 108u:
								case 109u:
								case 110u:
								case 111u:
								case 112u:
								case 113u:
								case 114u:
								case 115u:
								case 116u:
								case 117u:
								case 118u:
								case 119u:
								case 120u:
								case 121u:
								case 122u:
								case 123u:
								case 124u:
								case 125u:
								case 126u:
								case 127u:
								case 128u:
								case 129u:
								case 130u:
								case 131u:
								case 132u:
								case 133u:
								case 134u:
								case 135u:
								case 136u:
								case 137u:
								case 138u:
								case 139u:
								case 140u:
								case 141u:
								case 142u:
								case 143u:
								case 144u:
								case 145u:
								case 146u:
								case 147u:
								case 148u:
								case 149u:
									GB.UISys.RunningInfoY.Data16[i16 + 3] = GB.TcpRD.Data16[10 + i16];
									break;
								case 150u:
								case 151u:
								case 152u:
								case 153u:
								case 154u:
								case 155u:
								case 156u:
								case 157u:
								case 158u:
								case 159u:
								case 160u:
								case 161u:
								case 162u:
								case 163u:
								case 164u:
								case 165u:
								case 166u:
								case 167u:
								case 168u:
								case 169u:
								case 170u:
								case 171u:
								case 172u:
								case 173u:
								case 174u:
								case 175u:
								case 176u:
								case 177u:
								case 178u:
								case 179u:
								case 180u:
								case 181u:
								case 182u:
								case 183u:
								case 184u:
								case 185u:
								case 186u:
								case 187u:
								case 188u:
								case 189u:
								case 190u:
								case 191u:
								case 192u:
								case 193u:
								case 194u:
								case 195u:
								case 196u:
								case 197u:
								case 198u:
								case 199u:
									GB.UISys.RunningScaleY.Data16[i16 - 150] = GB.TcpRD.Data16[10 + i16];
									break;
								}
							}
						}
						if (D1 == 0)
						{
							GB.ResultLedStatusFunc(D1, GB.UISys.RunningInfoX.ScrewNo, GB.UISys.RunningInfoX.Status);
						}
						else
						{
							GB.ResultLedStatusFunc(D1, GB.UISys.RunningInfoY.ScrewNo, GB.UISys.RunningInfoY.Status);
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 499)
					{
						if (D1 == 0)
						{
							switch (D2)
							{
							case 0:
								GB.UISys.RunningCurveTimeX.Clear();
								break;
							case 1:
								GB.UISys.RunningCurveAngleX.Clear();
								break;
							case 2:
								GB.UISys.RunningCurveTorqueX.Clear();
								break;
							case 3:
								GB.UISys.RunningCurveTorqueRateX.Clear();
								break;
							}
						}
						else
						{
							switch (D2)
							{
							case 0:
								GB.UISys.RunningCurveTimeY.Clear();
								break;
							case 1:
								GB.UISys.RunningCurveAngleY.Clear();
								break;
							case 2:
								GB.UISys.RunningCurveTorqueY.Clear();
								break;
							case 3:
								GB.UISys.RunningCurveTorqueRateY.Clear();
								break;
							}
						}
						for (uint i17 = 0u; i17 < GB.TcpRD.Size - 10; i17++)
						{
							if (D1 == 0)
							{
								if (D2 == 0 || D2 == 10 || D2 == 20 || D2 == 30)
								{
									GB.UISys.RunningCurveTimeX.Add((short)GB.TcpRD.Data16[10 + i17]);
								}
								else if (D2 == 1 || D2 == 11 || D2 == 21 || D2 == 31)
								{
									GB.UISys.RunningCurveAngleX.Add((short)GB.TcpRD.Data16[10 + i17]);
								}
								else if (D2 == 2 || D2 == 12 || D2 == 22 || D2 == 32)
								{
									GB.UISys.RunningCurveTorqueX.Add((short)GB.TcpRD.Data16[10 + i17]);
								}
								else if (D2 == 3 || D2 == 13 || D2 == 23 || D2 == 33)
								{
									GB.UISys.RunningCurveTorqueRateX.Add((short)GB.TcpRD.Data16[10 + i17]);
								}
							}
							else if (D2 == 0 || D2 == 10 || D2 == 20 || D2 == 30)
							{
								GB.UISys.RunningCurveTimeY.Add((short)GB.TcpRD.Data16[10 + i17]);
							}
							else if (D2 == 1 || D2 == 11 || D2 == 21 || D2 == 31)
							{
								GB.UISys.RunningCurveAngleY.Add((short)GB.TcpRD.Data16[10 + i17]);
							}
							else if (D2 == 2 || D2 == 12 || D2 == 22 || D2 == 32)
							{
								GB.UISys.RunningCurveTorqueY.Add((short)GB.TcpRD.Data16[10 + i17]);
							}
							else if (D2 == 3 || D2 == 13 || D2 == 23 || D2 == 33)
							{
								GB.UISys.RunningCurveTorqueRateY.Add((short)GB.TcpRD.Data16[10 + i17]);
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 550)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i18 = 0u; i18 < GB.TcpRD.Size - 10; i18++)
							{
								GB.FSCtrlEthernet.Data16[i18] = GB.TcpRD.Data16[10 + i18];
							}
							if (GB.CheckHMIVer(170, 3))
							{
								GB.FSCtrlEthernet.TCPServerPort = GB.TcpRD.Data2;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 551)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i19 = 0u; i19 < GB.TcpRD.Size - 10; i19++)
							{
								GB.FSCtrlPageAuthority.Data16[i19] = GB.TcpRD.Data16[10 + i19];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 552)
					{
						GB.UISys.PCSoftSupport = false;
						if (GB.TcpRD.OKNG == 1)
						{
							if (D4 != 99)
							{
								for (int l = 0; l < 40; l++)
								{
									GB.FSCtrlVersion.Data16[l] = 0;
								}
								for (int m = 0; m < GB.TcpRD.Size - 10; m++)
								{
									GB.FSCtrlVersion.Data16[m] = GB.TcpRD.Data16[10 + m];
								}
							}
							if (GB.TcpRD.Data1 == 88)
							{
								GB.UISys.PCSoftSupport = true;
								GB.UISys.CtrlDualTool = GB.TcpRD.Data3;
								GB.UISys.PM101 = GB.TcpRD.Data2;
							}
							else if (GB.TcpRD.Data1 == 84)
							{
								GB.UISys.PCSoftSupport = true;
								GB.UISys.CtrlDualTool = GB.TcpRD.Data3;
								GB.UISys.PM101 = 4;
							}
							else
							{
								GB.UISys.PCSoftSupport = false;
								GB.UISys.CtrlDualTool = 0;
								GB.UISys.PM101 = 3;
							}
							GB.SetModelNameType(GB.UISys.PM101);
						}
						Err = ((D4 != 99) ? ((GB.GetNameTitleStr(FormType.SubCtrlFWVersion, 0) == "") ? (-2) : 0) : 0);
						break;
					}
					if (GB.TcpRD.CmdFunc == 553)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i20 = 0u; i20 < GB.TcpRD.Size - 10; i20++)
							{
								if (D1 == 0)
								{
									GB.FSCtrlDIOFunction_X.Data16[i20] = GB.TcpRD.Data16[10 + i20];
								}
								else
								{
									GB.FSCtrlDIOFunction_Y.Data16[i20] = GB.TcpRD.Data16[10 + i20];
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 554)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i21 = 0u; i21 < GB.TcpRD.Size - 10; i21++)
							{
								if (D1 == 0)
								{
									if (GB.TcpRD.Data2 == 0)
									{
										GB.FSCtrlDOBitsTable_X.Data16[i21] = GB.TcpRD.Data16[10 + i21];
									}
									else if (GB.TcpRD.Data2 == 1)
									{
										GB.FSCtrlDIBitsTable_X.Data16[i21] = GB.TcpRD.Data16[10 + i21];
									}
									else if (GB.TcpRD.Data2 == 2)
									{
										GB.FSCtrlDOParamTable_X.Data16[i21] = GB.TcpRD.Data16[10 + i21];
									}
									else if (GB.TcpRD.Data2 == 4)
									{
										GB.FSCtrlDOScrewTable_X.Data16[i21] = GB.TcpRD.Data16[10 + i21];
									}
									else if (GB.TcpRD.Data2 == 6)
									{
										GB.FSCtrlDOSeqTable_X.Data16[i21] = GB.TcpRD.Data16[10 + i21];
									}
								}
								else if (GB.TcpRD.Data2 == 0)
								{
									GB.FSCtrlDOBitsTable_Y.Data16[i21] = GB.TcpRD.Data16[10 + i21];
								}
								else if (GB.TcpRD.Data2 == 1)
								{
									GB.FSCtrlDIBitsTable_Y.Data16[i21] = GB.TcpRD.Data16[10 + i21];
								}
								else if (GB.TcpRD.Data2 == 2)
								{
									GB.FSCtrlDOParamTable_Y.Data16[i21] = GB.TcpRD.Data16[10 + i21];
								}
								else if (GB.TcpRD.Data2 == 4)
								{
									GB.FSCtrlDOScrewTable_Y.Data16[i21] = GB.TcpRD.Data16[10 + i21];
								}
								else if (GB.TcpRD.Data2 == 6)
								{
									GB.FSCtrlDOSeqTable_Y.Data16[i21] = GB.TcpRD.Data16[10 + i21];
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 555)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlTorqUnit.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 556)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (GB.UISys.NonPushStartTypeX == 1 || GB.UISys.NonPushStartTypeY == 1)
							{
								if (GB.TcpRD.Data1 == 1)
								{
									GB.FSCtrlStartCondition.Mode = 1;
								}
								else if (GB.TcpRD.Data1 == 6)
								{
									GB.FSCtrlStartCondition.Mode = 6;
								}
								else
								{
									GB.FSCtrlStartCondition.Mode = 2;
								}
							}
							else
							{
								GB.FSCtrlStartCondition.Mode = GB.TcpRD.Data1;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 557)
					{
						if (D1 == 0)
						{
							if (GB.TcpRD.Data2 == 0)
							{
								if (GB.TcpRD.Data3 == 0)
								{
									GB.FSCtrlDIOFunction_X.DO1_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 1)
								{
									GB.FSCtrlDIOFunction_X.DO2_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 2)
								{
									GB.FSCtrlDIOFunction_X.DO3_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 3)
								{
									GB.FSCtrlDIOFunction_X.DO4_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 4)
								{
									GB.FSCtrlDIOFunction_X.DO5_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 5)
								{
									GB.FSCtrlDIOFunction_X.DO6_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 6)
								{
									GB.FSCtrlDIOFunction_X.DO7_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 7)
								{
									GB.FSCtrlDIOFunction_X.DO8_NONC = GB.TcpRD.Data4;
								}
							}
							else if (GB.TcpRD.Data2 == 1)
							{
								if (GB.TcpRD.Data3 == 0)
								{
									GB.FSCtrlDIOFunction_X.DI1_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 1)
								{
									GB.FSCtrlDIOFunction_X.DI2_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 2)
								{
									GB.FSCtrlDIOFunction_X.DI3_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 3)
								{
									GB.FSCtrlDIOFunction_X.DI4_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 4)
								{
									GB.FSCtrlDIOFunction_X.DI5_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 5)
								{
									GB.FSCtrlDIOFunction_X.DI6_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 6)
								{
									GB.FSCtrlDIOFunction_X.DI7_NONC = GB.TcpRD.Data4;
								}
								else if (GB.TcpRD.Data3 == 7)
								{
									GB.FSCtrlDIOFunction_X.DI8_NONC = GB.TcpRD.Data4;
								}
							}
						}
						else if (GB.TcpRD.Data2 == 0)
						{
							if (GB.TcpRD.Data3 == 0)
							{
								GB.FSCtrlDIOFunction_Y.DO1_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 1)
							{
								GB.FSCtrlDIOFunction_Y.DO2_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 2)
							{
								GB.FSCtrlDIOFunction_Y.DO3_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 3)
							{
								GB.FSCtrlDIOFunction_Y.DO4_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 4)
							{
								GB.FSCtrlDIOFunction_Y.DO5_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 5)
							{
								GB.FSCtrlDIOFunction_Y.DO6_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 6)
							{
								GB.FSCtrlDIOFunction_Y.DO7_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 7)
							{
								GB.FSCtrlDIOFunction_Y.DO8_NONC = GB.TcpRD.Data4;
							}
						}
						else if (GB.TcpRD.Data2 == 1)
						{
							if (GB.TcpRD.Data3 == 0)
							{
								GB.FSCtrlDIOFunction_Y.DI1_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 1)
							{
								GB.FSCtrlDIOFunction_Y.DI2_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 2)
							{
								GB.FSCtrlDIOFunction_Y.DI3_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 3)
							{
								GB.FSCtrlDIOFunction_Y.DI4_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 4)
							{
								GB.FSCtrlDIOFunction_Y.DI5_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 5)
							{
								GB.FSCtrlDIOFunction_Y.DI6_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 6)
							{
								GB.FSCtrlDIOFunction_Y.DI7_NONC = GB.TcpRD.Data4;
							}
							else if (GB.TcpRD.Data3 == 7)
							{
								GB.FSCtrlDIOFunction_Y.DI8_NONC = GB.TcpRD.Data4;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 558)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlTwoStageMode.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 559)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlWarningWindow.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 560)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlCurveStageUpLimit.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 561)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlExportResultFile.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 562)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlSamplingRate.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 563)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlMonitorToolCurrent.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 564)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlCompensationForToolTemp.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 565)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i22 = 0u; i22 < GB.TcpRD.Size - 10; i22++)
							{
								GB.FSCtrlComPortFunction.Data16[i22] = GB.TcpRD.Data16[10 + i22];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 566)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlSendResultTCP.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 567)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlParamNotMatchToolSpec.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 568)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlAngleUnit.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 569)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlBuzzerVolume.MsgVolume = GB.TcpRD.Data1;
							GB.FSCtrlBuzzerVolume.KeyBoardVolum = GB.TcpRD.Data2;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 570)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlDisplayHDMI.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 571)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlHomeStartPage.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 572)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i23 = 0u; i23 < GB.TcpRD.Size - 10; i23++)
							{
								GB.FSCtrlRS485Function.Data16[i23] = GB.TcpRD.Data16[10 + i23];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 573)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlCurveAllPositive.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 574)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlDefLoosSpeed.Value = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 575)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlKeyboardCursorBlinkingInResults.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 576)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 577)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlProhibitOperationNC.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 578)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlDIResponseFilterTime.Value = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 579)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i24 = 0u; i24 < GB.TcpRD.Size - 10; i24++)
							{
								GB.FSCtrlMAC.Data16[i24] = GB.TcpRD.Data16[10 + i24];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 580)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i25 = 0u; i25 < GB.TcpRD.Size - 10; i25++)
							{
								GB.FSCtrlModelName.Data16[i25] = GB.TcpRD.Data16[10 + i25];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 581)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlCurveScaleFromZero.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 582)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlCurveCheckMCURange.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 583)
					{
						for (uint i26 = 0u; i26 < GB.TcpRD.Size - 10; i26++)
						{
							if (D1 == 0)
							{
								GB.FSCtrlDOTimer_X.Data16[i26] = GB.TcpRD.Data16[10 + i26];
							}
							else
							{
								GB.FSCtrlDOTimer_Y.Data16[i26] = GB.TcpRD.Data16[10 + i26];
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 584)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlEarlyWindow.WNALForm = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 585)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlBuzzerMode.Error = GB.TcpRD.Data1;
							GB.FSCtrlBuzzerMode.EachFinish = GB.TcpRD.Data2;
							GB.FSCtrlBuzzerMode.AllFinish = GB.TcpRD.Data3;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 586)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlCurveCutoffPoint.Mode = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 587)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlCurveCheckMCUSwitch.Value = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 588)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlProhibitToolAlarmClear.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 589)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlSpeedLimit.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 590)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlHealthCheck.Enable = GB.TcpRD.Data1;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 599)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							uint Cnt = 0u;
							for (uint i27 = 0u; i27 < sizeof(FSModelTypeInfoStuc) / 2; i27++)
							{
								GB.FSModelTypeInfo.Data16[i27] = 0;
								Cnt = i27;
							}
							for (uint i28 = 0u; i28 < GB.TcpRD.Size - 10; i28++)
							{
								GB.FSModelTypeInfo.Data16[i28] = GB.TcpRD.Data16[10 + i28];
							}
							GB.DetectCtrlMode();
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 650)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i29 = 0u; i29 < GB.TcpRD.Size - 10; i29++)
							{
								if (D1 == 0)
								{
									GB.FSToolXInfo.Data16[i29] = GB.TcpRD.Data16[10 + i29];
								}
								else
								{
									GB.FSToolYInfo.Data16[i29] = GB.TcpRD.Data16[10 + i29];
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 651)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								if (GB.TcpRD.Data2 <= 4096)
								{
									GB.FSToolXLeverStartLevel.CurrentLevel = GB.TcpRD.Data2;
								}
								else
								{
									GB.FSToolXLeverStartLevel.CurrentLevel = 4096;
								}
								if (D2 != 88)
								{
									if (GB.TcpRD.Data3 <= 4096)
									{
										GB.FSToolXLeverStartLevel.OnLevel = GB.TcpRD.Data3;
									}
									else
									{
										GB.FSToolXLeverStartLevel.OnLevel = 4096;
									}
									if (GB.TcpRD.Data4 <= 4096)
									{
										GB.FSToolXLeverStartLevel.OffLevel = GB.TcpRD.Data4;
									}
									else
									{
										GB.FSToolXLeverStartLevel.OffLevel = 4096;
									}
								}
							}
							else
							{
								if (GB.TcpRD.Data2 <= 4096)
								{
									GB.FSToolYLeverStartLevel.CurrentLevel = GB.TcpRD.Data2;
								}
								else
								{
									GB.FSToolYLeverStartLevel.CurrentLevel = 4096;
								}
								if (D2 != 88)
								{
									if (GB.TcpRD.Data3 <= 4096)
									{
										GB.FSToolYLeverStartLevel.OnLevel = GB.TcpRD.Data3;
									}
									else
									{
										GB.FSToolYLeverStartLevel.OnLevel = 4096;
									}
									if (GB.TcpRD.Data4 <= 4096)
									{
										GB.FSToolYLeverStartLevel.OffLevel = GB.TcpRD.Data4;
									}
									else
									{
										GB.FSToolYLeverStartLevel.OffLevel = 4096;
									}
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 652)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								if (GB.TcpRD.Data2 <= 4096)
								{
									GB.FSToolXPushStartLevel.CurrentLevel = GB.TcpRD.Data2;
								}
								else
								{
									GB.FSToolXPushStartLevel.CurrentLevel = 4096;
								}
								if (D2 != 88)
								{
									if (GB.TcpRD.Data3 <= 4096)
									{
										GB.FSToolXPushStartLevel.OnLevel = GB.TcpRD.Data3;
									}
									else
									{
										GB.FSToolXPushStartLevel.OnLevel = 4096;
									}
									if (GB.TcpRD.Data4 <= 4096)
									{
										GB.FSToolXPushStartLevel.OffLevel = GB.TcpRD.Data4;
									}
									else
									{
										GB.FSToolXPushStartLevel.OffLevel = 4096;
									}
								}
							}
							else
							{
								if (GB.TcpRD.Data2 <= 4096)
								{
									GB.FSToolYPushStartLevel.CurrentLevel = GB.TcpRD.Data2;
								}
								else
								{
									GB.FSToolYPushStartLevel.CurrentLevel = 4096;
								}
								if (D2 != 88)
								{
									if (GB.TcpRD.Data3 <= 4096)
									{
										GB.FSToolYPushStartLevel.OnLevel = GB.TcpRD.Data3;
									}
									else
									{
										GB.FSToolYPushStartLevel.OnLevel = 4096;
									}
									if (GB.TcpRD.Data4 <= 4096)
									{
										GB.FSToolYPushStartLevel.OffLevel = GB.TcpRD.Data4;
									}
									else
									{
										GB.FSToolYPushStartLevel.OffLevel = 4096;
									}
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 653)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								if (GB.TcpRD.Data1 <= 100)
								{
									GB.FSToolXWorkLight.Value = GB.TcpRD.Data1;
								}
								else
								{
									GB.FSToolXWorkLight.Value = 100;
								}
							}
							else if (GB.TcpRD.Data1 <= 100)
							{
								GB.FSToolYWorkLight.Value = GB.TcpRD.Data1;
							}
							else
							{
								GB.FSToolYWorkLight.Value = 100;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 655)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i30 = 0u; i30 < GB.TcpRD.Size - 10; i30++)
							{
								if (D1 == 0)
								{
									GB.FSToolXLedLight.Data16[i30] = GB.TcpRD.Data16[10 + i30];
								}
								else
								{
									GB.FSToolYLedLight.Data16[i30] = GB.TcpRD.Data16[10 + i30];
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 656)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								GB.FSToolXCalibration.Sensitivity = GB.TcpRD.Data2;
							}
							else
							{
								GB.FSToolYCalibration.Sensitivity = GB.TcpRD.Data2;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 657)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i31 = 0u; i31 < GB.TcpRD.Size - 10; i31++)
							{
								if (D1 == 0)
								{
									GB.FSToolXVersion.Data16[i31] = GB.TcpRD.Data16[10 + i31];
								}
								else
								{
									GB.FSToolYVersion.Data16[i31] = GB.TcpRD.Data16[10 + i31];
								}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 658)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								GB.FSToolXActive.ServiceReminderEnable = GB.TcpRD.Data2;
							}
							else
							{
								GB.FSToolYActive.ServiceReminderEnable = GB.TcpRD.Data2;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 659)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								GB.FSToolXActive.ActiveEnable = (ushort)((GB.TcpRD.Data2 > 0) ? 1 : 0);
							}
							else
							{
								GB.FSToolYActive.ActiveEnable = (ushort)((GB.TcpRD.Data2 > 0) ? 1 : 0);
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 660)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSToolRemindCnt_DW = (uint)(GB.TcpRD.Data2 * 65536 + GB.TcpRD.Data1);
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 661)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							for (uint i32 = 0u; i32 < GB.TcpRD.Size - 10; i32++)
							{
								GB.FSToolTeachRecord.Data16[i32] = GB.TcpRD.Data16[10 + i32];
							}
							GB.FSToolTeachRecordPage = (ushort)(GB.TcpRD.Data4 + 1);
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 662)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								GB.FSToolXMaxAngForRotationDetect.Value = GB.TcpRD.Data2;
							}
							else
							{
								GB.FSToolYMaxAngForRotationDetect.Value = GB.TcpRD.Data2;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 663)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								GB.FSToolXTempLevel.WNLevel = GB.TcpRD.Data2;
								GB.FSToolXTempLevel.ALLevel = GB.TcpRD.Data3;
							}
							else
							{
								GB.FSToolYTempLevel.WNLevel = GB.TcpRD.Data2;
								GB.FSToolYTempLevel.ALLevel = GB.TcpRD.Data3;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 664)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							if (D1 == 0)
							{
								GB.FSToolXLedDelayTmr.Value = GB.TcpRD.Data2;
							}
							else
							{
								GB.FSToolYLedDelayTmr.Value = GB.TcpRD.Data2;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 699)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							GB.FSCtrlTypeInfo.CtrlVer = GB.TcpRD.Data2;
							GB.FSModelTypeInfo.MesModelType = GB.TcpRD.Data3;
							GB.FSModelTypeInfo.MesRawDataTorqUint = GB.TcpRD.Data4;
							for (uint i33 = 0u; i33 < GB.TcpRD.Size - 10; i33++)
							{
								if (D1 == 0)
								{
									GB.FSToolXModelInfo.Data16[i33] = GB.TcpRD.Data16[10 + i33];
								}
								else
								{
									GB.FSToolYModelInfo.Data16[i33] = GB.TcpRD.Data16[10 + i33];
								}
							}
							if (D1 == 0)
							{
								if (GB.FSToolXModelInfo.ToolTorque_Nm == 0)
								{
									GB.FSToolXModelInfo.ToolTorque_Nm = (ushort)((double)(int)GB.FSToolXModelInfo.MaxTorque / GB.TorqUnitcoef(1000 + GB.FSModelTypeInfo.MesRawDataTorqUint));
								}
							}
							else if (GB.FSToolYModelInfo.ToolTorque_Nm == 0)
							{
								GB.FSToolYModelInfo.ToolTorque_Nm = (ushort)((double)(int)GB.FSToolYModelInfo.MaxTorque / GB.TorqUnitcoef(1000 + GB.FSModelTypeInfo.MesRawDataTorqUint));
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 750)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							int ReportID = GB.TcpRD.Data2 * 65536 + GB.TcpRD.Data1 - 1;
							for (uint i34 = 0u; i34 < GB.TcpRD.Size - 10; i34++)
							{
								GB.ExFSReport.Info[ReportID].Data16[i34] = GB.TcpRD.Data16[10 + i34];
							}
							if (GB.ExFSReport.Info[ReportID].Year == 0 && GB.ExFSReport.Info[ReportID].Month == 0 && GB.ExFSReport.Info[ReportID].Day == 0)
							{
								GB.ExFSReport.Info[ReportID].Year = 1970;
								GB.ExFSReport.Info[ReportID].Month = 1;
								GB.ExFSReport.Info[ReportID].Day = 1;
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 751)
					{
						if (GB.TcpRD.OKNG == 1)
						{
							switch (D3)
							{
							case 0:
							{
								for (uint i54 = 0u; i54 < GB.TcpRD.Size - 10; i54++)
								{
									GB.ExFSReport.CurveTime[i54] = GB.TcpRD.Data16[10 + i54];
								}
								break;
							}
							case 1:
							{
								for (uint i38 = 0u; i38 < GB.TcpRD.Size - 10; i38++)
								{
									GB.ExFSReport.CurveAngle[i38] = (short)GB.TcpRD.Data16[10 + i38];
								}
								break;
							}
							case 4:
							{
								ushort CoefFWUnit9 = (ushort)(D4 / 100);
								ushort CoefUnit9 = (ushort)(D4 - CoefFWUnit9 * 100);
								double coef9 = 1.0;
								for (uint i46 = 0u; i46 < (GB.TcpRD.Size - 10) / 2; i46++)
								{
									GB.ExFSReport.CurveTorque[i46] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i46 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i46]) * coef9);
								}
								break;
							}
							case 5:
							{
								ushort CoefFWUnit15 = (ushort)(D4 / 100);
								ushort CoefUnit15 = (ushort)(D4 - CoefFWUnit15 * 100);
								double coef15 = 1.0;
								for (uint i58 = 0u; i58 < (GB.TcpRD.Size - 10) / 2; i58++)
								{
									GB.ExFSReport.CurveTorque[i58 + 1000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i58 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i58]) * coef15);
								}
								break;
							}
							case 6:
							{
								ushort CoefFWUnit12 = (ushort)(D4 / 100);
								ushort CoefUnit12 = (ushort)(D4 - CoefFWUnit12 * 100);
								double coef12 = 1.0;
								for (uint i50 = 0u; i50 < (GB.TcpRD.Size - 10) / 2; i50++)
								{
									GB.ExFSReport.CurveTorqueRate[i50] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i50 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i50]) * coef12);
								}
								break;
							}
							case 7:
							{
								ushort CoefFWUnit7 = (ushort)(D4 / 100);
								ushort CoefUnit7 = (ushort)(D4 - CoefFWUnit7 * 100);
								double coef7 = 1.0;
								for (uint i42 = 0u; i42 < (GB.TcpRD.Size - 10) / 2; i42++)
								{
									GB.ExFSReport.CurveTorqueRate[i42 + 1000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i42 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i42]) * coef7);
								}
								break;
							}
							case 10:
							{
								int ReportID2 = GB.TcpRD.Data2 * 65536 + GB.TcpRD.Data1 - 1;
								for (uint i60 = 0u; i60 < GB.TcpRD.Size - 10; i60++)
								{
									GB.ExFSReport.Scale[ReportID2].Data16[i60] = GB.TcpRD.Data16[10 + i60];
								}
								break;
							}
							case 11:
							{
								for (uint i56 = 0u; i56 < GB.TcpRD.Size - 10; i56++)
								{
									GB.ExFSReport.ReportParam[i56] = GB.TcpRD.Data16[10 + i56];
								}
								break;
							}
							case 20:
							{
								for (uint i52 = 0u; i52 < GB.TcpRD.Size - 10; i52++)
								{
									GB.ExFSReport.CurveTime[i52 + 2000] = GB.TcpRD.Data16[10 + i52];
								}
								break;
							}
							case 21:
							{
								for (uint i48 = 0u; i48 < GB.TcpRD.Size - 10; i48++)
								{
									GB.ExFSReport.CurveAngle[i48 + 2000] = (short)GB.TcpRD.Data16[10 + i48];
								}
								break;
							}
							case 24:
							{
								ushort CoefFWUnit8 = (ushort)(D4 / 100);
								ushort CoefUnit8 = (ushort)(D4 - CoefFWUnit8 * 100);
								double coef8 = 1.0;
								for (uint i44 = 0u; i44 < (GB.TcpRD.Size - 10) / 2; i44++)
								{
									GB.ExFSReport.CurveTorque[i44 + 2000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i44 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i44]) * coef8);
								}
								break;
							}
							case 25:
							{
								ushort CoefFWUnit5 = (ushort)(D4 / 100);
								ushort CoefUnit5 = (ushort)(D4 - CoefFWUnit5 * 100);
								double coef5 = 1.0;
								for (uint i40 = 0u; i40 < (GB.TcpRD.Size - 10) / 2; i40++)
								{
									GB.ExFSReport.CurveTorque[i40 + 3000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i40 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i40]) * coef5);
								}
								break;
							}
							case 26:
							{
								ushort CoefFWUnit2 = (ushort)(D4 / 100);
								ushort CoefUnit2 = (ushort)(D4 - CoefFWUnit2 * 100);
								double coef2 = 1.0;
								for (uint i36 = 0u; i36 < (GB.TcpRD.Size - 10) / 2; i36++)
								{
									GB.ExFSReport.CurveTorqueRate[i36 + 2000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i36 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i36]) * coef2);
								}
								break;
							}
							case 27:
							{
								ushort CoefFWUnit16 = (ushort)(D4 / 100);
								ushort CoefUnit16 = (ushort)(D4 - CoefFWUnit16 * 100);
								double coef16 = 1.0;
								for (uint i59 = 0u; i59 < (GB.TcpRD.Size - 10) / 2; i59++)
								{
									GB.ExFSReport.CurveTorqueRate[i59 + 3000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i59 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i59]) * coef16);
								}
								break;
							}
							case 30:
							{
								for (uint i57 = 0u; i57 < GB.TcpRD.Size - 10; i57++)
								{
									GB.ExFSReport.CurveTime[i57 + 4000] = GB.TcpRD.Data16[10 + i57];
								}
								break;
							}
							case 31:
							{
								for (uint i55 = 0u; i55 < GB.TcpRD.Size - 10; i55++)
								{
									GB.ExFSReport.CurveAngle[i55 + 4000] = (short)GB.TcpRD.Data16[10 + i55];
								}
								break;
							}
							case 34:
							{
								ushort CoefFWUnit14 = (ushort)(D4 / 100);
								ushort CoefUnit14 = (ushort)(D4 - CoefFWUnit14 * 100);
								double coef14 = 1.0;
								for (uint i53 = 0u; i53 < (GB.TcpRD.Size - 10) / 2; i53++)
								{
									GB.ExFSReport.CurveTorque[i53 + 4000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i53 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i53]) * coef14);
								}
								break;
							}
							case 35:
							{
								ushort CoefFWUnit13 = (ushort)(D4 / 100);
								ushort CoefUnit13 = (ushort)(D4 - CoefFWUnit13 * 100);
								double coef13 = 1.0;
								for (uint i51 = 0u; i51 < (GB.TcpRD.Size - 10) / 2; i51++)
								{
									GB.ExFSReport.CurveTorque[i51 + 5000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i51 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i51]) * coef13);
								}
								break;
							}
							case 36:
							{
								ushort CoefFWUnit11 = (ushort)(D4 / 100);
								ushort CoefUnit11 = (ushort)(D4 - CoefFWUnit11 * 100);
								double coef11 = 1.0;
								for (uint i49 = 0u; i49 < (GB.TcpRD.Size - 10) / 2; i49++)
								{
									GB.ExFSReport.CurveTorqueRate[i49 + 4000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i49 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i49]) * coef11);
								}
								break;
							}
							case 37:
							{
								ushort CoefFWUnit10 = (ushort)(D4 / 100);
								ushort CoefUnit10 = (ushort)(D4 - CoefFWUnit10 * 100);
								double coef10 = 1.0;
								for (uint i47 = 0u; i47 < (GB.TcpRD.Size - 10) / 2; i47++)
								{
									GB.ExFSReport.CurveTorqueRate[i47 + 5000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i47 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i47]) * coef10);
								}
								break;
							}
							case 40:
							{
								for (uint i45 = 0u; i45 < GB.TcpRD.Size - 10; i45++)
								{
									GB.ExFSReport.CurveTime[i45 + 6000] = GB.TcpRD.Data16[10 + i45];
								}
								break;
							}
							case 41:
							{
								for (uint i43 = 0u; i43 < GB.TcpRD.Size - 10; i43++)
								{
									GB.ExFSReport.CurveAngle[i43 + 6000] = (short)GB.TcpRD.Data16[10 + i43];
								}
								break;
							}
							case 44:
							{
								ushort CoefFWUnit6 = (ushort)(D4 / 100);
								ushort CoefUnit6 = (ushort)(D4 - CoefFWUnit6 * 100);
								double coef6 = 1.0;
								for (uint i41 = 0u; i41 < (GB.TcpRD.Size - 10) / 2; i41++)
								{
									GB.ExFSReport.CurveTorque[i41 + 6000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i41 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i41]) * coef6);
								}
								break;
							}
							case 45:
							{
								ushort CoefFWUnit4 = (ushort)(D4 / 100);
								ushort CoefUnit4 = (ushort)(D4 - CoefFWUnit4 * 100);
								double coef4 = 1.0;
								for (uint i39 = 0u; i39 < (GB.TcpRD.Size - 10) / 2; i39++)
								{
									GB.ExFSReport.CurveTorque[i39 + 7000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i39 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i39]) * coef4);
								}
								break;
							}
							case 46:
							{
								ushort CoefFWUnit3 = (ushort)(D4 / 100);
								ushort CoefUnit3 = (ushort)(D4 - CoefFWUnit3 * 100);
								double coef3 = 1.0;
								for (uint i37 = 0u; i37 < (GB.TcpRD.Size - 10) / 2; i37++)
								{
									GB.ExFSReport.CurveTorqueRate[i37 + 6000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i37 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i37]) * coef3);
								}
								break;
							}
							case 47:
							{
								ushort CoefFWUnit = (ushort)(D4 / 100);
								ushort CoefUnit = (ushort)(D4 - CoefFWUnit * 100);
								double coef = 1.0;
								for (uint i35 = 0u; i35 < (GB.TcpRD.Size - 10) / 2; i35++)
								{
									GB.ExFSReport.CurveTorqueRate[i35 + 7000] = (short)((double)(GB.TcpRD.Data16[10 + 2 * i35 + 1] * 65536 + GB.TcpRD.Data16[10 + 2 * i35]) * coef);
								}
								break;
							}
							}
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 752)
					{
						switch (D2)
						{
						case 1:
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].Year = GB.TcpRD.Data16[10];
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].Month = GB.TcpRD.Data16[11];
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].Day = GB.TcpRD.Data16[12];
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].Hour = GB.TcpRD.Data16[13];
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].Min = GB.TcpRD.Data16[14];
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].Sec = GB.TcpRD.Data16[15];
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].Code = GB.TcpRD.Data16[16];
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].ReportID = (uint)(GB.TcpRD.Data16[18] * 65536 + GB.TcpRD.Data16[17]);
							GB.ExFSReport.AlarmInfoOnlyAL[GB.TcpRD.Data1 - 1].LifeTime = 0u;
							if (D1 == 0)
							{
								CurrALRow = GB.TcpRD.Data1;
							}
							break;
						case 2:
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].Year = GB.TcpRD.Data16[10];
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].Month = GB.TcpRD.Data16[11];
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].Day = GB.TcpRD.Data16[12];
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].Hour = GB.TcpRD.Data16[13];
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].Min = GB.TcpRD.Data16[14];
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].Sec = GB.TcpRD.Data16[15];
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].Code = GB.TcpRD.Data16[16];
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].ReportID = (uint)(GB.TcpRD.Data16[18] * 65536 + GB.TcpRD.Data16[17]);
							GB.ExFSReport.AlarmInfoOnlyNG[GB.TcpRD.Data1 - 1].LifeTime = 0u;
							if (D1 == 0)
							{
								CurrNGRow = GB.TcpRD.Data1;
							}
							break;
						default:
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].Year = GB.TcpRD.Data16[10];
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].Month = GB.TcpRD.Data16[11];
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].Day = GB.TcpRD.Data16[12];
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].Hour = GB.TcpRD.Data16[13];
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].Min = GB.TcpRD.Data16[14];
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].Sec = GB.TcpRD.Data16[15];
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].Code = GB.TcpRD.Data16[16];
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].ReportID = (uint)(GB.TcpRD.Data16[18] * 65536 + GB.TcpRD.Data16[17]);
							GB.ExFSReport.AlarmInfo[GB.TcpRD.Data1 - 1].LifeTime = (uint)(GB.TcpRD.Data16[20] * 65536 + GB.TcpRD.Data16[19]);
							break;
						}
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 753)
					{
						GB.ExFSReport.WarningInfo[GB.TcpRD.Data1 - 1].Year = GB.TcpRD.Data16[10];
						GB.ExFSReport.WarningInfo[GB.TcpRD.Data1 - 1].Month = GB.TcpRD.Data16[11];
						GB.ExFSReport.WarningInfo[GB.TcpRD.Data1 - 1].Day = GB.TcpRD.Data16[12];
						GB.ExFSReport.WarningInfo[GB.TcpRD.Data1 - 1].Hour = GB.TcpRD.Data16[13];
						GB.ExFSReport.WarningInfo[GB.TcpRD.Data1 - 1].Min = GB.TcpRD.Data16[14];
						GB.ExFSReport.WarningInfo[GB.TcpRD.Data1 - 1].Sec = GB.TcpRD.Data16[15];
						GB.ExFSReport.WarningInfo[GB.TcpRD.Data1 - 1].Code = GB.TcpRD.Data16[16];
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 754)
					{
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].Year = GB.TcpRD.Data16[10];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].Month = GB.TcpRD.Data16[11];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].Day = GB.TcpRD.Data16[12];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].Hour = GB.TcpRD.Data16[13];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].Min = GB.TcpRD.Data16[14];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].Sec = GB.TcpRD.Data16[15];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].ID = GB.TcpRD.Data16[16];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].User = GB.TcpRD.Data16[21];
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].Before = (uint)(GB.TcpRD.Data16[18] * 65536 + GB.TcpRD.Data16[17]);
						GB.ExFSReport.ButtonInfo[GB.TcpRD.Data1 - 1].After = (uint)(GB.TcpRD.Data16[20] * 65536 + GB.TcpRD.Data16[19]);
						Err = 0;
						break;
					}
					if (GB.TcpRD.CmdFunc == 807)
					{
						if (GB.TcpRD.Data1 == GB.TcpWR.Data1 && GB.TcpRD.Data2 == GB.TcpWR.Data2 && GB.TcpRD.Data3 == GB.TcpWR.Data3)
						{
							for (uint i61 = 0u; i61 < GB.TcpRD.Size - 10; i61++)
							{
								if (i61 < 2000)
								{
									FSBinData[i61] = GB.TcpRD.Data16[10 + i61];
								}
							}
							Err = 0;
							break;
						}
						Err = -6;
					}
					else
					{
						if (GB.TcpRD.CmdFunc == 808)
						{
							short Status = (short)GB.TcpRD.Data4;
							if (GB.TcpRD.Data2 == ushort.MaxValue && GB.TcpRD.Data3 == ushort.MaxValue && Status > 0)
							{
								Bin_Status = 999;
							}
							else if (GB.TcpRD.Data2 == ushort.MaxValue && GB.TcpRD.Data3 == ushort.MaxValue && Status == 0)
							{
								Bin_Status = 4000;
							}
							else if (GB.TcpRD.Data2 == ushort.MaxValue && GB.TcpRD.Data3 == 65534)
							{
								Bin_Status = Status;
							}
							else if (Status == 0)
							{
								Bin_Status = 999;
							}
							else if (Status > 0)
							{
								uint ByteLen = (uint)Status;
								ushort CRC = (ushort)(GB.TcpWR.Data1 + GB.TcpRD.Data16[10] + GB.TcpRD.Data16[10 + ByteLen / 2 - 1]);
								if (GB.TcpRD.Data1 != CRC)
								{
									Bin_Status = 4002;
								}
								else
								{
									uint Addr = (uint)(GB.TcpRD.Data3 * 65536 + GB.TcpRD.Data2);
									switch (Addr)
									{
									case 0u:
									{
										for (uint i67 = 0u; i67 < GB.TcpRD.Size - 10; i67++)
										{
											if (i67 < 2000)
											{
												FSBinCacheSNReport[i67] = GB.TcpRD.Data16[10 + i67];
											}
										}
										break;
									}
									case 300u:
									{
										for (uint i75 = 0u; i75 < GB.TcpRD.Size - 10; i75++)
										{
											if (i75 < 2000)
											{
												FSBinCacheTimePoint[i75] = GB.TcpRD.Data16[10 + i75];
											}
										}
										break;
									}
									case 4300u:
									{
										for (uint i79 = 0u; i79 < GB.TcpRD.Size - 10; i79++)
										{
											if (i79 < 2000)
											{
												FSBinCacheAnglePoint[i79] = (short)GB.TcpRD.Data16[10 + i79];
											}
										}
										break;
									}
									case 8300u:
									{
										for (uint i71 = 0u; i71 < GB.TcpRD.Size - 10; i71++)
										{
											if (i71 < 2000)
											{
												FSBinCacheTorqPoint[i71] = (short)GB.TcpRD.Data16[10 + i71];
											}
										}
										break;
									}
									case 12300u:
									{
										for (uint i63 = 0u; i63 < GB.TcpRD.Size - 10; i63++)
										{
											if (i63 < 2000)
											{
												FSBinCacheTorqRatePoint[i63] = (short)GB.TcpRD.Data16[10 + i63];
											}
										}
										break;
									}
									case 16300u:
									{
										for (uint i77 = 0u; i77 < GB.TcpRD.Size - 10; i77++)
										{
											if (i77 < 2000)
											{
												FSBinCacheScaleParam[i77] = GB.TcpRD.Data16[10 + i77];
											}
										}
										break;
									}
									case 17500u:
									{
										for (uint i73 = 0u; i73 < GB.TcpRD.Size - 10; i73++)
										{
											if (i73 < 2000)
											{
												FSBinCacheTimePoint[2000 + i73] = GB.TcpRD.Data16[10 + i73];
											}
										}
										break;
									}
									case 21500u:
									{
										for (uint i69 = 0u; i69 < GB.TcpRD.Size - 10; i69++)
										{
											if (i69 < 2000)
											{
												FSBinCacheAnglePoint[2000 + i69] = (short)GB.TcpRD.Data16[10 + i69];
											}
										}
										break;
									}
									case 25500u:
									{
										for (uint i65 = 0u; i65 < GB.TcpRD.Size - 10; i65++)
										{
											if (i65 < 2000)
											{
												FSBinCacheTorqPoint[2000 + i65] = (short)GB.TcpRD.Data16[10 + i65];
											}
										}
										break;
									}
									case 29500u:
									{
										for (uint i80 = 0u; i80 < GB.TcpRD.Size - 10; i80++)
										{
											if (i80 < 2000)
											{
												FSBinCacheTorqRatePoint[2000 + i80] = (short)GB.TcpRD.Data16[10 + i80];
											}
										}
										break;
									}
									case 33500u:
									{
										for (uint i78 = 0u; i78 < GB.TcpRD.Size - 10; i78++)
										{
											if (i78 < 2000)
											{
												FSBinCacheTimePoint[4000 + i78] = GB.TcpRD.Data16[10 + i78];
											}
										}
										break;
									}
									case 37500u:
									{
										for (uint i76 = 0u; i76 < GB.TcpRD.Size - 10; i76++)
										{
											if (i76 < 2000)
											{
												FSBinCacheAnglePoint[4000 + i76] = (short)GB.TcpRD.Data16[10 + i76];
											}
										}
										break;
									}
									case 41500u:
									{
										for (uint i74 = 0u; i74 < GB.TcpRD.Size - 10; i74++)
										{
											if (i74 < 2000)
											{
												FSBinCacheTorqPoint[4000 + i74] = (short)GB.TcpRD.Data16[10 + i74];
											}
										}
										break;
									}
									case 45500u:
									{
										for (uint i72 = 0u; i72 < GB.TcpRD.Size - 10; i72++)
										{
											if (i72 < 2000)
											{
												FSBinCacheTorqRatePoint[4000 + i72] = (short)GB.TcpRD.Data16[10 + i72];
											}
										}
										break;
									}
									case 49500u:
									{
										for (uint i70 = 0u; i70 < GB.TcpRD.Size - 10; i70++)
										{
											if (i70 < 2000)
											{
												FSBinCacheTimePoint[6000 + i70] = GB.TcpRD.Data16[10 + i70];
											}
										}
										break;
									}
									case 53500u:
									{
										for (uint i68 = 0u; i68 < GB.TcpRD.Size - 10; i68++)
										{
											if (i68 < 2000)
											{
												FSBinCacheAnglePoint[6000 + i68] = (short)GB.TcpRD.Data16[10 + i68];
											}
										}
										break;
									}
									case 57500u:
									{
										for (uint i66 = 0u; i66 < GB.TcpRD.Size - 10; i66++)
										{
											if (i66 < 2000)
											{
												FSBinCacheTorqPoint[6000 + i66] = (short)GB.TcpRD.Data16[10 + i66];
											}
										}
										break;
									}
									case 61500u:
									{
										for (uint i64 = 0u; i64 < GB.TcpRD.Size - 10; i64++)
										{
											if (i64 < 2000)
											{
												FSBinCacheTorqRatePoint[6000 + i64] = (short)GB.TcpRD.Data16[10 + i64];
											}
										}
										break;
									}
									case 65500u:
									{
										for (uint i62 = 0u; i62 < GB.TcpRD.Size - 10; i62++)
										{
											if (i62 < 2000)
											{
												FSBinCacheOtherInfo[i62] = GB.TcpRD.Data16[10 + i62];
											}
										}
										break;
									}
									}
									if (Addr == 65500)
									{
										Bin_Status = 999;
									}
									else
									{
										Bin_Status = 900;
									}
								}
							}
							GB.TcpRD.Data1 = (GB.TcpRD.Data2 = (GB.TcpRD.Data3 = (GB.TcpRD.Data4 = 0)));
							Err = 0;
							break;
						}
						if (GB.TcpRD.CmdFunc == 810)
						{
							if (GB.TcpRD.Data2 == GB.TcpWR.Data2 && GB.TcpRD.Data3 == GB.TcpWR.Data3)
							{
								Err = 0;
								break;
							}
							Err = -6;
						}
						else
						{
							if (GB.TcpRD.CmdFunc == 813)
							{
								ushort MesID = GB.TcpRD.Data1;
								ushort MesGp = (ushort)(GB.TcpRD.Data1 / 100);
								ushort MesCmd = (ushort)(GB.TcpRD.Data1 - (ushort)(MesGp * 100));
								if (MesID != 0)
								{
									switch (MesCmd)
									{
									case 99:
										FSNewFWInfo.UserFWVer = GB.TcpRD.Data2;
										FSNewFWInfo.UserSDVer = GB.TcpRD.Data3;
										FSNewFWInfo.UserRcp32Ver = GB.TcpRD.Data4;
										break;
									case 0:
									{
										Array.Clear(FSNewFWInfo.RawData, 0, FSNewFWInfo.RawData.Length);
										for (uint i81 = 0u; i81 < GB.TcpRD.Size - 10; i81++)
										{
											if (i81 < 2000)
											{
												FSNewFWInfo.RawData[i81] = GB.TcpRD.Data16[10 + i81];
											}
										}
										break;
									}
									}
								}
								Err = 0;
								break;
							}
							if (GB.TcpRD.CmdFunc == 1571)
							{
								if (GB.TcpRD.Data1 <= 7)
								{
									GB.ExFSUser.UserID = GB.TcpRD.Data1;
									GB.ExFSUser.LastUserID = 99u;
								}
								Err = 0;
								break;
							}
							if (GB.TcpRD.CmdFunc == 1572)
							{
								for (uint i82 = 0u; i82 < 10; i82++)
								{
									switch (D1)
									{
									case 0:
										GB.ExFSUser.User1Name[i82] = GB.TcpRD.Data16[10 + i82];
										break;
									case 1:
										GB.ExFSUser.User2Name[i82] = GB.TcpRD.Data16[10 + i82];
										break;
									case 2:
										GB.ExFSUser.User3Name[i82] = GB.TcpRD.Data16[10 + i82];
										break;
									case 3:
										GB.ExFSUser.User4Name[i82] = GB.TcpRD.Data16[10 + i82];
										break;
									case 4:
										GB.ExFSUser.User5Name[i82] = GB.TcpRD.Data16[10 + i82];
										break;
									case 5:
										GB.ExFSUser.User6Name[i82] = GB.TcpRD.Data16[10 + i82];
										break;
									}
								}
								Err = 0;
								break;
							}
							if (GB.TcpRD.CmdFunc == 1573)
							{
								GB.FSCtrlLanguage.Mode = GB.TcpRD.Data1;
								Err = 0;
								break;
							}
						}
					}
					if (Err != 0)
					{
						Console.WriteLine("TcpType:{0}, Retry:{1}, Err:{2}", TcpType, Loop, Err);
					}
				}
				else
				{
					if (GB.TcpRD.ErrCode != 0)
					{
						Err = GB.TcpRD.ErrCode;
					}
					if (Loop >= 2)
					{
						Err = -5;
						Console.WriteLine("TcpType:{0}, Err:{1}", TcpType, Err);
					}
				}
			}
			return Err;
		}

		public int FSIDRead_ByFTP(ushort FormNum, uint StartBase, uint EndBase, int Message)
		{
			int Err = 0;
			switch (FormNum)
			{
			case 10:
				Err = ReadParamFile_ByComm(0);
				break;
			case 11:
				Err = ReadParamFile_ByComm(1);
				break;
			case 20:
				Err = ReadSeqFile_ByComm();
				break;
			case 30:
				Err = ReadSrcFileMode_ByComm(30, GB.FSSrcMode.SwitchingMethodX);
				break;
			case 31:
				Err = ReadSrcFileMode_ByComm(30, 0);
				break;
			case 32:
				Err = ReadSrcFileMode_ByComm(30, 1);
				break;
			case 33:
				Err = ReadSrcFileMode_ByComm(30, 2);
				break;
			case 35:
				Err = ReadSrcFileMode_ByComm(35, GB.FSSrcMode.SwitchingMethodY);
				break;
			case 36:
				Err = ReadSrcFileMode_ByComm(35, 0);
				break;
			case 37:
				Err = ReadSrcFileMode_ByComm(35, 1);
				break;
			case 38:
				Err = ReadSrcFileMode_ByComm(35, 2);
				break;
			case 40:
				Err = ReadSrcFileMode_ByComm(40, GB.FSSrcMode.SwitchingMethodX);
				break;
			case 41:
				Err = ReadSrcFileMode_ByComm(40, 0);
				break;
			case 42:
				Err = ReadSrcFileMode_ByComm(40, 1);
				break;
			case 43:
				Err = ReadSrcFileMode_ByComm(40, 2);
				break;
			case 50:
				Err = ReadSrcFileMode_ByComm(50, GB.FSSrcMode.SwitchingMethodX);
				break;
			case 51:
				Err = ReadSrcFileMode_ByComm(50, 0);
				break;
			case 52:
				Err = ReadSrcFileMode_ByComm(50, 1);
				break;
			case 53:
				Err = ReadSrcFileMode_ByComm(50, 2);
				break;
			case 70:
				Err = ((Message != 1) ? ReadReportSNFile_ByComm(StartBase, EndBase, false) : ReadReportSNFile_ByComm(StartBase, EndBase, true));
				break;
			case 80:
				Err = ((Message != 1) ? ReadReportDetailFile_ByComm(StartBase, EndBase, false) : ReadReportDetailFile_ByComm(StartBase, EndBase, true));
				break;
			case 82:
				Err = ((Message != 1) ? ReadReportScale_ByComm(StartBase, EndBase, false) : ReadReportScale_ByComm(StartBase, EndBase, true));
				break;
			case 83:
				Err = ((Message != 1) ? ReadReportCurveScaleParam_ByComm(StartBase, EndBase, false) : ReadReportCurveScaleParam_ByComm(StartBase, EndBase, true));
				break;
			case 84:
				Err = ((Message != 1) ? ReadReportCurveScaleParam_BySpec(StartBase, EndBase, false) : ReadReportCurveScaleParam_BySpec(StartBase, EndBase, true));
				break;
			case 100:
				Err = ReadALFile_ByComm(StartBase);
				break;
			case 101:
				Err = ReadWNFile_ByComm(StartBase);
				break;
			case 102:
				Err = ReadBNFile_ByComm(StartBase);
				break;
			case 110:
				Err = ReadCurveFile_ByComm(StartBase, EndBase);
				break;
			case 81:
				Err = ReadReportStatusFile_ByComm(StartBase, EndBase);
				break;
			}
			return Err;
		}

		public int FSIDRead_ByFTP(ushort FormNum)
		{
			ushort UseHMIDisk = (ushort)((!GB.UISys.PCFTPMaster) ? 2 : 0);
			int Err = 0;
			switch (FormNum)
			{
			case 10:
				Err = ((GB.FSModelTypeInfo.MesModelType != 1) ? ((GB.FSModelTypeInfo.MesModelType != 2) ? FSIDWrite_ByTCP(805, 0, 199, FormNum, 0, UseHMIDisk) : FSIDWrite_ByTCP(805, 0, 199, 10, 0, UseHMIDisk)) : FSIDWrite_ByTCP(805, 0, 199, 11, 0, UseHMIDisk));
				break;
			case 11:
				Err = ((GB.FSModelTypeInfo.MesModelType != 1) ? ((GB.FSModelTypeInfo.MesModelType != 2) ? FSIDWrite_ByTCP(805, 0, 199, FormNum, 0, UseHMIDisk) : FSIDWrite_ByTCP(805, 0, 199, 11, 0, UseHMIDisk)) : FSIDWrite_ByTCP(805, 0, 199, 10, 0, UseHMIDisk));
				break;
			case 30:
				Err = ((GB.FSModelTypeInfo.MesModelType != 1) ? ((GB.FSModelTypeInfo.MesModelType != 2) ? FSIDWrite_ByTCP(805, 0, 199, FormNum, 0, UseHMIDisk) : FSIDWrite_ByTCP(805, 0, 199, 30, 0, UseHMIDisk)) : FSIDWrite_ByTCP(805, 0, 199, 35, 0, UseHMIDisk));
				break;
			case 35:
				Err = ((GB.FSModelTypeInfo.MesModelType != 1) ? ((GB.FSModelTypeInfo.MesModelType != 2) ? FSIDWrite_ByTCP(805, 0, 199, FormNum, 0, UseHMIDisk) : FSIDWrite_ByTCP(805, 0, 199, 35, 0, UseHMIDisk)) : FSIDWrite_ByTCP(805, 0, 199, 30, 0, UseHMIDisk));
				break;
			default:
				Err = FSIDWrite_ByTCP(805, 0, 199, FormNum, 0, UseHMIDisk);
				break;
			}
			if (Err == 0)
			{
				switch (FormNum)
				{
				case 10:
					GB.ReadParamFTPFile(0);
					break;
				case 11:
					GB.ReadParamFTPFile(1);
					break;
				case 20:
					GB.ReadSeqFTPFile();
					break;
				case 30:
					GB.ReadSrcFTPFile(0, 30);
					break;
				case 35:
					GB.ReadSrcFTPFile(1, 35);
					break;
				case 40:
					GB.ReadSrcFTPFile(0, 40);
					break;
				case 50:
					GB.ReadSrcFTPFile(0, 50);
					break;
				case 70:
					GB.ReadReportSNFTPFile();
					break;
				case 80:
					GB.ReadReportFTPFile();
					break;
				case 100:
					GB.ReadALFTPFile();
					break;
				case 101:
					GB.ReadWNFTPFile();
					break;
				case 102:
					GB.ReadBNFTPFile();
					break;
				case 110:
					GB.ReadCurveFTPFile(0);
					break;
				case 111:
					GB.ReadCurveFTPFile(1);
					break;
				case 112:
					GB.ReadCurveFTPFile(2);
					break;
				case 113:
					GB.ReadCurveFTPFile(3);
					break;
				case 114:
					GB.ReadCurveFTPFile(4);
					break;
				case 115:
					GB.ReadCurveFTPFile(5);
					break;
				case 116:
					GB.ReadCurveFTPFile(6);
					break;
				case 117:
					GB.ReadCurveFTPFile(7);
					break;
				case 118:
					GB.ReadCurveFTPFile(8);
					break;
				case 119:
					GB.ReadCurveFTPFile(9);
					break;
				}
			}
			if ((short)Err == -62 || (short)Err == -63)
			{
				Err = 0;
			}
			return Err;
		}

		public int ReadParamFile_ByComm(int Axis)
		{
			int Err = 0;
			List<ushort> FSList = new List<ushort>();
			uint FSOffs = 250000u;
			Err = ((GB.FSModelTypeInfo.MesModelType == 1) ? FSIDRead_ByTCP(807, 0, 4, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 500) : ((GB.FSModelTypeInfo.MesModelType != 2) ? FSIDRead_ByTCP(807, 0, (ushort)(Axis + 3), (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 500) : FSIDRead_ByTCP(807, 0, 3, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 500)));
			if (Err == -6)
			{
				return Err;
			}
			ushort[] BinParamEn = new ushort[500];
			Array.Copy(FSBinData, BinParamEn, 500);
			ushort[,] BinParam = new ushort[500, 670];
			FSOffs = 0u;
			for (int n = 0; n < 500; n++)
			{
				FSOffs = (uint)(n * 70);
				if (BinParamEn[n] > 0)
				{
					Err = ((GB.FSModelTypeInfo.MesModelType == 1) ? FSIDRead_ByTCP(807, 0, 2, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 70) : ((GB.FSModelTypeInfo.MesModelType != 2) ? FSIDRead_ByTCP(807, 0, (ushort)(Axis + 1), (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 70) : FSIDRead_ByTCP(807, 0, 1, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 70)));
					if (Err == -6)
					{
						return Err;
					}
					for (int i = 0; i < 70; i++)
					{
						BinParam[n, i] = FSBinData[i];
					}
				}
			}
			FSOffs = 0u;
			for (int j = 0; j < 500; j++)
			{
				FSOffs = (uint)(j * 600);
				if (BinParamEn[j] > 0)
				{
					Err = ((GB.FSModelTypeInfo.MesModelType == 1) ? FSIDRead_ByTCP(807, 0, 53, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 600) : ((GB.FSModelTypeInfo.MesModelType != 2) ? FSIDRead_ByTCP(807, 0, (ushort)(Axis + 52), (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 600) : FSIDRead_ByTCP(807, 0, 52, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 600)));
					if (Err == -6)
					{
						return Err;
					}
					for (int k = 0; k < 600; k++)
					{
						BinParam[j, 70 + k] = FSBinData[k];
					}
				}
			}
			GB.ParseFSParamToTCPDataBase(Axis, ref BinParam);
			return Err;
		}

		public int ReadSeqFile_ByComm()
		{
			ushort[] BinSeqEn = new ushort[500];
			int Err = 0;
			List<ushort> FSList = new List<ushort>();
			uint FSOffs = 250000u;
			Err = FSIDRead_ByTCP(807, 0, 5, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 500);
			if (Err == -6)
			{
				return Err;
			}
			Array.Copy(FSBinData, BinSeqEn, 500);
			ushort[,] BinSeqBase = new ushort[500, 500];
			FSOffs = 0u;
			for (int n = 0; n < 500; n++)
			{
				FSOffs = (uint)(n * 500);
				if (BinSeqEn[n] > 0)
				{
					Err = FSIDRead_ByTCP(807, 0, 5, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 500);
					if (Err == -6)
					{
						return Err;
					}
					for (int i = 0; i < 500; i++)
					{
						BinSeqBase[n, i] = FSBinData[i];
					}
				}
			}
			GB.ParseFSSeqToTCPDataBase(ref BinSeqBase);
			return Err;
		}

		public int ReadSrcFileMode_ByComm(int SrcMode, int MethodMode)
		{
			int Err = 0;
			int FSID = 0;
			List<ushort> FSList = new List<ushort>();
			uint FSOffs = 0u;
			uint SaveFSOffs = 0u;
			uint SrcSize = 0u;
			ushort[,] BinSrcFS = new ushort[3, 76800];
			if (SrcMode == 30)
			{
				FSID = ((GB.FSModelTypeInfo.MesModelType == 1) ? (7 + 2 * MethodMode) : ((GB.FSModelTypeInfo.MesModelType != 2) ? (6 + 2 * MethodMode) : (6 + 2 * MethodMode)));
			}
			switch (SrcMode)
			{
			case 35:
				FSID = ((GB.FSModelTypeInfo.MesModelType != 1) ? ((GB.FSModelTypeInfo.MesModelType != 2) ? (7 + 2 * MethodMode) : (6 + 2 * MethodMode)) : (7 + 2 * MethodMode));
				break;
			case 40:
				FSID = 12 + MethodMode;
				break;
			case 50:
				FSID = 15 + MethodMode;
				break;
			}
			switch (MethodMode)
			{
			case 0:
				SrcSize = 0u;
				break;
			case 1:
				SrcSize = 0u;
				break;
			case 2:
				SrcSize = 50000u;
				break;
			}
			if (SrcSize != 0)
			{
				FSList.Clear();
				for (int i = 0; i <= SrcSize / 2000 + 1; i++)
				{
					int DetectSize = (int)(SrcSize - FSOffs);
					if (DetectSize >= 2000)
					{
						Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
						if (Err == -6)
						{
							return Err;
						}
						FSOffs += 2000;
						for (int n = 0; n < 2000; n++)
						{
							FSList.Add(FSBinData[n]);
						}
						continue;
					}
					if (DetectSize > 0 && DetectSize <= 2000)
					{
						Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), (ushort)DetectSize);
						if (Err == -6)
						{
							return Err;
						}
						FSOffs += (uint)DetectSize;
						for (int j = 0; j < DetectSize; j++)
						{
							FSList.Add(FSBinData[j]);
						}
						continue;
					}
					break;
				}
				ushort[] FS1Data16 = FSList.ToArray();
				for (int k = 0; k < FSList.Count(); k++)
				{
					if (k < SrcSize)
					{
						BinSrcFS[MethodMode, k] = FS1Data16[k];
					}
				}
			}
			FSOffs = 50000u;
			Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 1100);
			if (Err == -6)
			{
				return Err;
			}
			for (int l = 0; l < 1100; l++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(l + FSOffs))] = FSBinData[l];
			}
			FSOffs = 51100u;
			Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 1500);
			if (Err == -6)
			{
				return Err;
			}
			for (int m = 0; m < 1500; m++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(m + FSOffs))] = FSBinData[m];
			}
			FSOffs = 52600u;
			Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
			if (Err == -6)
			{
				return Err;
			}
			for (int num = 0; num < 2000; num++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(num + FSOffs))] = FSBinData[num];
			}
			FSOffs = 54600u;
			Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 1000);
			if (Err == -6)
			{
				return Err;
			}
			for (int num2 = 0; num2 < 1000; num2++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(num2 + FSOffs))] = FSBinData[num2];
			}
			FSOffs = 57100u;
			Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
			if (Err == -6)
			{
				return Err;
			}
			for (int num3 = 0; num3 < 2000; num3++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(num3 + FSOffs))] = FSBinData[num3 + 500];
			}
			FSOffs = 59100u;
			Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 1500);
			if (Err == -6)
			{
				return Err;
			}
			for (int num4 = 0; num4 < 500; num4++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(num4 + FSOffs))] = FSBinData[num4];
			}
			SaveFSOffs = ((GB.FSModelTypeInfo.MesModelType != 1) ? 59600u : 60100u);
			for (int num5 = 0; num5 < 500; num5++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(num5 + SaveFSOffs))] = FSBinData[num5 + 500];
			}
			FSOffs = 60100u;
			SaveFSOffs = ((GB.FSModelTypeInfo.MesModelType != 1) ? 60100u : 59600u);
			for (int num6 = 0; num6 < 500; num6++)
			{
				BinSrcFS[MethodMode, (int)(long)checked((IntPtr)unchecked(num6 + SaveFSOffs))] = FSBinData[num6 + 1000];
			}
			switch (SrcMode)
			{
			case 30:
				switch (MethodMode)
				{
				case 0:
					GB.ParseFSSrcToTCPDataBase(0, 0, ref GB.FSSrcAll.FSSrcManualX, ref BinSrcFS);
					break;
				case 1:
					GB.ParseFSSrcToTCPDataBase(0, 1, ref GB.FSSrcAll.FSSrcBitsX, ref BinSrcFS);
					break;
				case 2:
					GB.ParseFSSrcToTCPDataBase(0, 2, ref GB.FSSrcAll.FSSrcScannerX, ref BinSrcFS);
					break;
				}
				break;
			case 35:
				switch (MethodMode)
				{
				case 0:
					GB.ParseFSSrcToTCPDataBase(1, 0, ref GB.FSSrcAll.FSSrcManualY, ref BinSrcFS);
					break;
				case 1:
					GB.ParseFSSrcToTCPDataBase(1, 1, ref GB.FSSrcAll.FSSrcBitsY, ref BinSrcFS);
					break;
				case 2:
					GB.ParseFSSrcToTCPDataBase(1, 2, ref GB.FSSrcAll.FSSrcScannerY, ref BinSrcFS);
					break;
				}
				break;
			case 40:
				switch (MethodMode)
				{
				case 0:
					GB.ParseFSSrcToTCPDataBase(1, 0, ref GB.FSSrcAll.FSSrcManual_DualMix, ref BinSrcFS);
					break;
				case 1:
					GB.ParseFSSrcToTCPDataBase(1, 1, ref GB.FSSrcAll.FSSrcBits_DualMix, ref BinSrcFS);
					break;
				case 2:
					GB.ParseFSSrcToTCPDataBase(1, 2, ref GB.FSSrcAll.FSSrcScanner_DualMix, ref BinSrcFS);
					break;
				}
				break;
			case 50:
				switch (MethodMode)
				{
				case 0:
					GB.ParseFSSrcToTCPDataBase(1, 0, ref GB.FSSrcAll.FSSrcManual_DualSync, ref BinSrcFS);
					break;
				case 1:
					GB.ParseFSSrcToTCPDataBase(1, 1, ref GB.FSSrcAll.FSSrcBits_DualSync, ref BinSrcFS);
					break;
				case 2:
					GB.ParseFSSrcToTCPDataBase(1, 2, ref GB.FSSrcAll.FSSrcScanner_DualSync, ref BinSrcFS);
					break;
				}
				break;
			}
			return Err;
		}

		public int ReadReportStatusFile_ByComm(uint StartBase, uint EndBase)
		{
			int Err = 0;
			uint Datagridview_Row = EndBase - StartBase;
			List<ushort> FSList = new List<ushort>();
			uint ReportSize = StartBase + Datagridview_Row;
			uint FSOffs = StartBase;
			uint Rep = Datagridview_Row / 2000 + 1;
			uint RepLoop = 0u;
			for (int i = 0; i <= Rep; i++)
			{
				RepLoop++;
				int DetectSize = (int)(ReportSize - FSOffs);
				if (DetectSize >= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 44, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += 2000;
					for (int n = 0; n < 2000; n++)
					{
						FSList.Add(FSBinData[n]);
					}
					continue;
				}
				if (DetectSize > 0 && DetectSize <= 2000)
				{
					FSIDRead_ByTCP(807, 0, 44, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), (ushort)DetectSize);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += (uint)DetectSize;
					for (int j = 0; j < DetectSize; j++)
					{
						FSList.Add(FSBinData[j]);
					}
					continue;
				}
				break;
			}
			uint FS1ID = StartBase;
			ushort[] FS1Data16 = FSList.ToArray();
			ushort[] BinReportStatus = new ushort[200000];
			for (int k = 0; k < FSList.Count(); k++)
			{
				if (FS1ID < 200000)
				{
					BinReportStatus[FS1ID] = FS1Data16[k];
					FS1ID++;
				}
			}
			GB.ParseFSReportStatusToTCPDataBase(StartBase, StartBase + Datagridview_Row, ref BinReportStatus);
			return Err;
		}

		public int ReadReportSNFile_ByComm(uint StartBase, uint EndBase, bool JumpMsg)
		{
			int Err = 0;
			uint Datagridview_Row = EndBase - StartBase;
			List<ushort> FSList = new List<ushort>();
			uint ReportSize = (StartBase + Datagridview_Row) * 100;
			uint FSOffs = StartBase * 100;
			uint Rep = Datagridview_Row * 100 / 2000 + 1;
			uint RepLoop = 0u;
			Form998_Wait Form998 = new Form998_Wait(GB);
			WaitPage = 10000;
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, WaitPage);
			}
			for (int i = 0; i <= Rep; i++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, i + 1, WaitPage);
				}
				RepLoop++;
				int DetectSize = (int)(ReportSize - FSOffs);
				if (DetectSize >= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 20, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += 2000;
					for (int n = 0; n < 2000; n++)
					{
						FSList.Add(FSBinData[n]);
					}
					continue;
				}
				if (DetectSize > 0 && DetectSize <= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 20, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), (ushort)DetectSize);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += (uint)DetectSize;
					for (int j = 0; j < DetectSize; j++)
					{
						FSList.Add(FSBinData[j]);
					}
					continue;
				}
				break;
			}
			uint FS1ID = StartBase;
			uint FS1Idx = 0u;
			ushort[] FS1Data16 = FSList.ToArray();
			ushort[,] BinReportSN = new ushort[200000, 100];
			for (int k = 0; k < FSList.Count(); k++)
			{
				if (FS1ID < 200000)
				{
					BinReportSN[FS1ID, FS1Idx] = FS1Data16[k];
					FS1Idx++;
					if (FS1Idx >= 100)
					{
						FS1ID++;
						FS1Idx = 0u;
					}
				}
			}
			GB.ParseFSReportSNToTCPDataBase(StartBase, StartBase + Datagridview_Row, ref BinReportSN);
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int ReadReportDetailFile_ByComm(uint StartBase, uint EndBase, bool JumpMsg)
		{
			int Err = 0;
			uint Datagridview_Row = EndBase - StartBase;
			List<ushort> FSList = new List<ushort>();
			uint ReportSNSize = (StartBase + Datagridview_Row) * 50;
			uint FSOffs = StartBase * 50;
			Form998_Wait Form998 = new Form998_Wait(GB);
			WaitPage = 5000;
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, WaitPage);
			}
			for (int i = 0; i <= Datagridview_Row * 50 / 2000 + 1; i++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, 1 + i, WaitPage);
				}
				int DetectSize = (int)(ReportSNSize - FSOffs);
				if (DetectSize >= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 21, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += 2000;
					for (int n = 0; n < 2000; n++)
					{
						FSList.Add(FSBinData[n]);
					}
					continue;
				}
				if (DetectSize > 0 && DetectSize <= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 21, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), (ushort)DetectSize);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += (uint)DetectSize;
					for (int j = 0; j < DetectSize; j++)
					{
						FSList.Add(FSBinData[j]);
					}
					continue;
				}
				break;
			}
			uint FS1ID = StartBase;
			uint FS1Idx = 0u;
			ushort[] FS1Data16 = FSList.ToArray();
			ushort[,] BinReportDetail = new ushort[200000, 50];
			for (int k = 0; k < FSList.Count(); k++)
			{
				if (FS1ID < 200000)
				{
					BinReportDetail[FS1ID, FS1Idx] = FS1Data16[k];
					FS1Idx++;
					if (FS1Idx >= 50)
					{
						FS1ID++;
						FS1Idx = 0u;
					}
				}
			}
			GB.ParseFSReportToTCPDataBase(StartBase, StartBase + Datagridview_Row, ref BinReportDetail);
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int ReadReportScale_ByComm(uint StartBase, uint EndBase, bool JumpMsg)
		{
			int Err = 0;
			uint FSOffs = 0u;
			List<ushort> FSList = new List<ushort>();
			Form998_Wait Form998 = new Form998_Wait(GB);
			WaitPage = (int)EndBase;
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, WaitPage);
			}
			for (uint idx = StartBase; idx < EndBase; idx++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, (int)idx, WaitPage);
				}
				uint FSID = 30 + StartBase / 20000;
				FSOffs = idx * 8600 + 8000;
				Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 50);
				if (Err == -6)
				{
					return Err;
				}
				GB.ParseFSScaleToTCPDataBase(idx, ref FSBinData);
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int ReadReportCurveScaleParam_ByComm(uint StartBase, uint EndBase, bool JumpMsg)
		{
			int Err = 0;
			uint FSOffs = 0u;
			uint FSID = 0u;
			uint FSSize = 0u;
			uint FSShift = 0u;
			List<ushort> FSList = new List<ushort>();
			Form998_Wait Form998 = new Form998_Wait(GB);
			WaitPage = (int)EndBase;
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, WaitPage);
			}
			for (uint idx = StartBase; idx < EndBase; idx++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, (int)idx, WaitPage);
				}
				FSID = 30 + idx / 20000;
				FSOffs = 0u;
				for (uint n = 0u; n <= 4; n++)
				{
					FSSize = ((8600 - FSOffs > 2000) ? 2000u : (8600 - FSOffs));
					FSShift = idx * 8600 + FSOffs;
					Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), (ushort)FSSize);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, n, ref FSBinData);
					FSOffs += 2000;
				}
				if (GB.ExFSReport.Scale[idx].CurveFreqModeVer == 2 || GB.ExFSReport.Scale[idx].CurveFreqModeVer == 3)
				{
					FSID = 30 + (StartBase + 100000) / 20000;
					FSOffs = 0u;
					for (uint n2 = 0u; n2 <= 4; n2++)
					{
						FSSize = ((8600 - FSOffs > 2000) ? 2000u : (8600 - FSOffs));
						FSShift = idx * 8600 + FSOffs;
						Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), (ushort)FSSize);
						if (Err == -6)
						{
							return Err;
						}
						GB.ParseFSCurveScaleParamToTCPDataBase(idx, n2, ref FSBinData);
						FSOffs += 2000;
					}
				}
				if (GB.ExFSReport.Scale[idx].CurveFreqModeVer != 4)
				{
					continue;
				}
				FSID = 30 + (idx + 50000) / 20000;
				FSOffs = 0u;
				for (uint n3 = 0u; n3 <= 4; n3++)
				{
					FSSize = ((8600 - FSOffs > 2000) ? 2000u : (8600 - FSOffs));
					FSShift = idx * 8600 + FSOffs;
					Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), (ushort)FSSize);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, n3, ref FSBinData);
					FSOffs += 2000;
				}
				FSID = 30 + (idx + 100000) / 20000;
				FSOffs = 0u;
				for (uint n4 = 0u; n4 <= 4; n4++)
				{
					FSSize = ((8600 - FSOffs > 2000) ? 2000u : (8600 - FSOffs));
					FSShift = idx * 8600 + FSOffs;
					Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), (ushort)FSSize);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, n4, ref FSBinData);
					FSOffs += 2000;
				}
				FSID = 30 + (idx + 150000) / 20000;
				FSOffs = 0u;
				for (uint n5 = 0u; n5 <= 4; n5++)
				{
					FSSize = ((8600 - FSOffs > 2000) ? 2000u : (8600 - FSOffs));
					FSShift = idx * 8600 + FSOffs;
					Err = FSIDRead_ByTCP(807, 0, (ushort)FSID, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), (ushort)FSSize);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, n5, ref FSBinData);
					FSOffs += 2000;
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int ReadCurveFile_ByComm(uint StartBase, uint EndBase)
		{
			return 0;
		}

		public int ReadReportCurveScaleParam_BySpec(uint StartBase, uint EndBase, bool JumpMsg)
		{
			int Err = 0;
			uint FSShift = 0u;
			List<ushort> FSList = new List<ushort>();
			Form998_Wait Form998 = new Form998_Wait(GB);
			WaitPage = (int)EndBase;
			if (JumpMsg)
			{
				Form998.Show();
			}
			if (JumpMsg)
			{
				Form998.Process(true, 0, WaitPage);
			}
			for (uint idx = StartBase; idx < EndBase; idx++)
			{
				if (JumpMsg)
				{
					Form998.Process(true, (int)idx, WaitPage);
				}
				FSShift = idx;
				Err = FSIDRead_ByTCP(810, 0, 99, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), 1);
				if (Err == -6)
				{
					return Err;
				}
				GB.ParseFSCurveScaleParamToTCPDataBase(idx, 1000u, ref FSBinData);
				if (GB.ExFSReport.Scale[idx].CurveFreqModeVer == 2 || GB.ExFSReport.Scale[idx].CurveFreqModeVer == 3)
				{
					FSShift = idx + 100000;
					Err = FSIDRead_ByTCP(810, 0, 99, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), 1);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, 2000u, ref FSBinData);
				}
				if (GB.ExFSReport.Scale[idx].CurveFreqModeVer == 4)
				{
					FSShift = idx + 50000;
					Err = FSIDRead_ByTCP(810, 0, 99, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), 1);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, 2000u, ref FSBinData);
					FSShift = idx + 100000;
					Err = FSIDRead_ByTCP(810, 0, 99, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), 1);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, 3000u, ref FSBinData);
					FSShift = idx + 150000;
					Err = FSIDRead_ByTCP(810, 0, 99, (ushort)(FSShift & 0xFFFF), (ushort)((FSShift >> 16) & 0xFFFF), 1);
					if (Err == -6)
					{
						return Err;
					}
					GB.ParseFSCurveScaleParamToTCPDataBase(idx, 4000u, ref FSBinData);
				}
			}
			if (JumpMsg)
			{
				Form998.Process(false, 0, 0);
			}
			return Err;
		}

		public int ReadALFile_ByComm(uint StartBase)
		{
			int Err = 0;
			uint Datagridview_Row = 10u;
			List<ushort> FSList = new List<ushort>();
			uint ReportSize = (StartBase + Datagridview_Row) * 6;
			uint FSOffs = StartBase * 6;
			for (int i = 0; i <= Datagridview_Row * 6 / 2000 + 1; i++)
			{
				int DetectSize = (int)(ReportSize - FSOffs);
				if (DetectSize >= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 26, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += 2000;
					for (int n = 0; n < 2000; n++)
					{
						FSList.Add(FSBinData[n]);
					}
					continue;
				}
				if (DetectSize > 0 && DetectSize <= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 26, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), (ushort)DetectSize);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += (uint)DetectSize;
					for (int j = 0; j < DetectSize; j++)
					{
						FSList.Add(FSBinData[j]);
					}
					continue;
				}
				break;
			}
			uint FS1ID = StartBase;
			uint FS1Idx = 0u;
			ushort[] FS1Data16 = FSList.ToArray();
			ushort[,] BinALDetail = new ushort[6000, 6];
			for (int k = 0; k < FSList.Count(); k++)
			{
				if (FS1ID < 6000)
				{
					BinALDetail[FS1ID, FS1Idx] = FS1Data16[k];
					FS1Idx++;
					if (FS1Idx >= 6)
					{
						FS1ID++;
						FS1Idx = 0u;
					}
				}
			}
			GB.ParseFSAlarmWarningDataBase(0, StartBase, StartBase + Datagridview_Row, ref BinALDetail);
			return Err;
		}

		public int ReadWNFile_ByComm(uint StartBase)
		{
			int Err = 0;
			uint Datagridview_Row = 10u;
			List<ushort> FSList = new List<ushort>();
			uint ReportSize = (StartBase + Datagridview_Row) * 6;
			uint FSOffs = StartBase * 6;
			for (int i = 0; i <= Datagridview_Row * 6 / 2000 + 1; i++)
			{
				int DetectSize = (int)(ReportSize - FSOffs);
				if (DetectSize >= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 27, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += 2000;
					for (int n = 0; n < 2000; n++)
					{
						FSList.Add(FSBinData[n]);
					}
					continue;
				}
				if (DetectSize > 0 && DetectSize <= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 27, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), (ushort)DetectSize);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += (uint)DetectSize;
					for (int j = 0; j < DetectSize; j++)
					{
						FSList.Add(FSBinData[j]);
					}
					continue;
				}
				break;
			}
			uint FS1ID = StartBase;
			uint FS1Idx = 0u;
			ushort[] FS1Data16 = FSList.ToArray();
			ushort[,] BinWNDetail = new ushort[6000, 6];
			for (int k = 0; k < FSList.Count(); k++)
			{
				if (FS1ID < 6000)
				{
					BinWNDetail[FS1ID, FS1Idx] = FS1Data16[k];
					FS1Idx++;
					if (FS1Idx >= 6)
					{
						FS1ID++;
						FS1Idx = 0u;
					}
				}
			}
			GB.ParseFSAlarmWarningDataBase(1, StartBase, StartBase + Datagridview_Row, ref BinWNDetail);
			return Err;
		}

		public int ReadBNFile_ByComm(uint StartBase)
		{
			int Err = 0;
			uint Datagridview_Row = 10u;
			List<ushort> FSList = new List<ushort>();
			uint ReportSize = (StartBase + Datagridview_Row) * 10;
			uint FSOffs = StartBase * 10;
			for (int i = 0; i <= Datagridview_Row * 10 / 2000 + 1; i++)
			{
				int DetectSize = (int)(ReportSize - FSOffs);
				if (DetectSize >= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 28, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), 2000);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += 2000;
					for (int n = 0; n < 2000; n++)
					{
						FSList.Add(FSBinData[n]);
					}
					continue;
				}
				if (DetectSize > 0 && DetectSize <= 2000)
				{
					Err = FSIDRead_ByTCP(807, 0, 28, (ushort)(FSOffs & 0xFFFF), (ushort)((FSOffs >> 16) & 0xFFFF), (ushort)DetectSize);
					if (Err == -6)
					{
						return Err;
					}
					FSOffs += (uint)DetectSize;
					for (int j = 0; j < DetectSize; j++)
					{
						FSList.Add(FSBinData[j]);
					}
					continue;
				}
				break;
			}
			uint FS1ID = StartBase;
			uint FS1Idx = 0u;
			ushort[] FS1Data16 = FSList.ToArray();
			ushort[,] BinBNDetail = new ushort[6000, 10];
			for (int k = 0; k < FSList.Count(); k++)
			{
				if (FS1ID < 6000)
				{
					BinBNDetail[FS1ID, FS1Idx] = FS1Data16[k];
					FS1Idx++;
					if (FS1Idx >= 10)
					{
						FS1ID++;
						FS1Idx = 0u;
					}
				}
			}
			GB.ParseFSButtonDataBase(StartBase, StartBase + Datagridview_Row, ref BinBNDetail);
			return Err;
		}
	}
}
