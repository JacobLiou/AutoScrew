using System;
using System.Net.Sockets;

namespace SD3_Graph
{
	internal class Modbus
	{
		public uint w_ID = 0u;

		public static bool b_Connect = false;

		public static bool b_CheckTimeOut = false;

		public static bool b_SndRecflag = false;

		public static uint w_TimeOut = 0u;

		public static TcpClient TCP_Client;

		public static NetworkStream stream;

		public const ushort St = 1;

		public static byte[] Sndpacket = new byte[1024];

		public static byte[] Recpacket = new byte[1024];

		public static ushort[] SetArray = new ushort[1024];

		public static ushort[] GetArray = new ushort[1024];

		public static PacketCmd SndDefine = default(PacketCmd);

		public static PacketCmd RecDefine = default(PacketCmd);

		public void Connect(string Ip, string TCPport)
		{
			TCP_Client = new TcpClient(Ip, int.Parse(TCPport));
			stream = TCP_Client.GetStream();
		}

		public void Disconnect()
		{
			if (stream != null)
			{
				stream.Close();
				stream = null;
			}
			if (TCP_Client != null)
			{
				TCP_Client.Close();
				TCP_Client = null;
			}
		}

		public int Receive()
		{
			byte[] Receive = new byte[TCP_Client.ReceiveBufferSize];
			int Receivebytes = stream.Read(Receive, 0, Receive.Length);
			if (Receivebytes != 0)
			{
				Array.Clear(Recpacket, 0, Recpacket.Length);
				Array.Copy(Receive, Recpacket, Receivebytes);
				int Err = ParserFunc();
			}
			stream.Flush();
			return Receivebytes;
		}

		public int ParserFunc()
		{
			int Err = 1;
			uint St = Recpacket[6];
			if (St != SndDefine.St)
			{
				return -2;
			}
			uint CmdType = Recpacket[7];
			if (CmdType != SndDefine.Type)
			{
				return -3;
			}
			switch (CmdType)
			{
			case 3u:
				Err = ParserMod03Func();
				break;
			case 6u:
				Err = ParserMod06Func();
				break;
			case 16u:
				Err = ParserMod10Func();
				break;
			}
			b_SndRecflag = true;
			b_CheckTimeOut = false;
			w_TimeOut = 0u;
			return Err;
		}

		public int ParserMod03Func()
		{
			ushort FedNum = (ushort)(Recpacket[8] / 2);
			for (int j = 0; j < FedNum; j++)
			{
				GetArray[j] = Convert.ToUInt16((Recpacket[9 + 2 * j] << 8) + Recpacket[10 + 2 * j]);
			}
			RecDefine.Addr = SndDefine.Addr;
			return 1;
		}

		public int ParserMod06Func()
		{
			ushort Fedaddr = Convert.ToUInt16((Recpacket[8] << 8) + Recpacket[9]);
			ushort Feddata = Convert.ToUInt16((Recpacket[10] << 8) + Recpacket[11]);
			if (SndDefine.Addr != Fedaddr)
			{
				return -4;
			}
			if (SndDefine.Data != Feddata)
			{
				return -5;
			}
			RecDefine.Addr = SndDefine.Addr;
			return 1;
		}

		public int ParserMod10Func()
		{
			ushort Fedaddr = Convert.ToUInt16((Recpacket[8] << 8) + Recpacket[9]);
			ushort FedNum = Convert.ToUInt16((Recpacket[10] << 8) + Recpacket[11]);
			if (SndDefine.Addr != Fedaddr)
			{
				return -4;
			}
			if (SndDefine.Num != FedNum)
			{
				return -6;
			}
			RecDefine.Addr = SndDefine.Addr;
			return 1;
		}

		public void Mod03Func(ushort St, ushort Addr, ushort Num)
		{
			if (b_Connect && stream != null)
			{
				ushort SndLen = 6;
				Sndpacket[0] = Convert.ToByte((SndDefine.Id >> 8) & 0xFF);
				Sndpacket[1] = Convert.ToByte(SndDefine.Id & 0xFF);
				Sndpacket[2] = 0;
				Sndpacket[3] = 0;
				Sndpacket[4] = Convert.ToByte((SndLen >> 8) & 0xFF);
				Sndpacket[5] = Convert.ToByte(SndLen & 0xFF);
				Sndpacket[6] = Convert.ToByte(St & 0xFF);
				Sndpacket[7] = 3;
				Sndpacket[8] = Convert.ToByte((Addr >> 8) & 0xFF);
				Sndpacket[9] = Convert.ToByte(Addr & 0xFF);
				Sndpacket[10] = Convert.ToByte((Num >> 8) & 0xFF);
				Sndpacket[11] = Convert.ToByte(Num & 0xFF);
				w_ID = Identifer(w_ID, St, Addr, Num, 0, 3u);
				try
				{
					stream.Write(Sndpacket, 0, 6 + SndLen);
				}
				catch
				{
				}
			}
		}

		public void Mod06Func(ushort St, ushort Addr, ushort Data16)
		{
			if (b_Connect && stream != null)
			{
				ushort SndLen = 6;
				Sndpacket[0] = Convert.ToByte((SndDefine.Id >> 8) & 0xFF);
				Sndpacket[1] = Convert.ToByte(SndDefine.Id & 0xFF);
				Sndpacket[2] = 0;
				Sndpacket[3] = 0;
				Sndpacket[4] = Convert.ToByte((SndLen >> 8) & 0xFF);
				Sndpacket[5] = Convert.ToByte(SndLen & 0xFF);
				Sndpacket[6] = Convert.ToByte(St & 0xFF);
				Sndpacket[7] = 6;
				Sndpacket[8] = Convert.ToByte((Addr >> 8) & 0xFF);
				Sndpacket[9] = Convert.ToByte(Addr & 0xFF);
				Sndpacket[10] = Convert.ToByte((Data16 >> 8) & 0xFF);
				Sndpacket[11] = Convert.ToByte(Data16 & 0xFF);
				w_ID = Identifer(w_ID, St, Addr, 1, Data16, 6u);
				try
				{
					stream.Write(Sndpacket, 0, 6 + SndLen);
				}
				catch
				{
				}
			}
		}

		public void Mod10Func(ushort St, ushort Addr, ushort Num, ushort[] data)
		{
			if (b_Connect && stream != null)
			{
				ushort SndLen = (ushort)(7 + 2 * Num);
				Sndpacket[0] = Convert.ToByte((SndDefine.Id >> 8) & 0xFF);
				Sndpacket[1] = Convert.ToByte(SndDefine.Id & 0xFF);
				Sndpacket[2] = 0;
				Sndpacket[3] = 0;
				Sndpacket[4] = Convert.ToByte((SndLen >> 8) & 0xFF);
				Sndpacket[5] = Convert.ToByte(SndLen & 0xFF);
				Sndpacket[6] = Convert.ToByte(St & 0xFF);
				Sndpacket[7] = 16;
				Sndpacket[8] = Convert.ToByte((Addr >> 8) & 0xFF);
				Sndpacket[9] = Convert.ToByte(Addr & 0xFF);
				Sndpacket[10] = Convert.ToByte((Num >> 8) & 0xFF);
				Sndpacket[11] = Convert.ToByte(Num & 0xFF);
				Sndpacket[12] = Convert.ToByte((Num * 2) & 0xFF);
				for (int j = 0; j < Num; j++)
				{
					Sndpacket[13 + 2 * j] = Convert.ToByte((data[j] >> 8) & 0xFF);
					Sndpacket[14 + 2 * j] = Convert.ToByte(data[j] & 0xFF);
				}
				w_ID = Identifer(w_ID, St, Addr, Num, 0, 16u);
				try
				{
					stream.Write(Sndpacket, 0, 6 + SndLen);
				}
				catch
				{
				}
			}
		}

		public uint Identifer(uint cnt, ushort St, ushort Addr, ushort Num, ushort Data16, uint Type)
		{
			SndDefine.St = St;
			SndDefine.Addr = Addr;
			SndDefine.Num = Num;
			SndDefine.Data = Data16;
			SndDefine.Type = Type;
			SndDefine.Id = cnt;
			w_TimeOut = 15u;
			if (Type == 3)
			{
				b_CheckTimeOut = true;
			}
			else
			{
				b_CheckTimeOut = false;
			}
			cnt = ((cnt < 255) ? (cnt + 1) : 0u);
			return cnt;
		}
	}
}
