using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace SD3_Graph
{
	internal class ReaderBin
	{
		private OpenFileDialog dialog;

		private GlobalVar GB;

		public ReaderBin(OpenFileDialog dialog, GlobalVar GB)
		{
			this.dialog = dialog;
			this.GB = GB;
		}

		public bool ReadFile(string CStr, int Mode)
		{
			if (Mode == 0)
			{
				FileStream BinFile = File.Open(CStr, FileMode.Open, FileAccess.ReadWrite);
				BinaryReader ReaderBin = new BinaryReader(BinFile);
				int Len8 = Convert.ToInt32(BinFile.Length);
				byte[] RawD8 = ReaderBin.ReadBytes(Len8);
				ushort[] RawD16 = new ushort[Len8 / 2];
				for (int i = 0; i < Len8 / 2; i++)
				{
					RawD16[i] = BitConverter.ToUInt16(RawD8, i * 2);
				}
				ReaderBin.Close();
				BinFile.Close();
				ParseFileBin(RawD8, RawD16);
			}
			else
			{
				byte[] compressedData = File.ReadAllBytes(CStr);
				byte[] RawD17 = decompress(compressedData);
				int Len9 = Convert.ToInt32(RawD17.Length);
				ushort[] RawD18 = new ushort[Len9 / 2];
				for (int j = 0; j < Len9 / 2; j++)
				{
					RawD18[j] = BitConverter.ToUInt16(RawD17, j * 2);
				}
				ParseFileBin(RawD17, RawD18);
			}
			return true;
		}

		private static byte[] compress(string text)
		{
			byte[] RstArr = new byte[10];
			if (string.IsNullOrEmpty(text))
			{
				return RstArr;
			}
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			using (MemoryStream outStream = new MemoryStream())
			{
				using (GZipStream zip = new GZipStream(outStream, CompressionMode.Compress))
				{
					zip.Write(buffer, 0, buffer.Length);
					zip.Close();
					return outStream.ToArray();
				}
			}
		}

		private static byte[] decompress(byte[] compressed)
		{
			if (compressed == null || compressed.Length == 0)
			{
				return compressed;
			}
			using (MemoryStream inStream = new MemoryStream(compressed))
			{
				using (MemoryStream outStream = new MemoryStream())
				{
					using (GZipStream zip = new GZipStream(inStream, CompressionMode.Decompress))
					{
						zip.CopyTo(outStream);
						zip.Close();
						return outStream.ToArray();
					}
				}
			}
		}

		public unsafe void ParseFileBin(byte[] Data8, ushort[] Data16)
		{
			for (int i = 0; i < 50; i++)
			{
				GB.UISys.List_Scale.Data16[i] = Data16[8150 + i];
			}
			for (int j = 0; j < GB.UISys.List_Scale.Curve_TotalPoint; j++)
			{
				if (j < 2000)
				{
					GB.UISys.List_Time.Add(Data16[150 + j]);
					GB.UISys.List_Angle.Add((short)Data16[2150 + j]);
					GB.UISys.List_Torq.Add((short)Data16[4150 + j]);
					GB.UISys.List_TorqRate.Add((short)Data16[6150 + j]);
				}
				else if (j < 4000)
				{
					GB.UISys.List_Time.Add(Data16[8750 + j - 2000]);
					GB.UISys.List_Angle.Add((short)Data16[10750 + j - 2000]);
					GB.UISys.List_Torq.Add((short)Data16[12750 + j - 2000]);
					GB.UISys.List_TorqRate.Add((short)Data16[14750 + j - 2000]);
				}
				else if (j < 6000)
				{
					GB.UISys.List_Time.Add(Data16[16750 + j - 4000]);
					GB.UISys.List_Angle.Add((short)Data16[18750 + j - 4000]);
					GB.UISys.List_Torq.Add((short)Data16[20750 + j - 4000]);
					GB.UISys.List_TorqRate.Add((short)Data16[22750 + j - 4000]);
				}
				else if (j < 8000)
				{
					GB.UISys.List_Time.Add(Data16[24750 + j - 6000]);
					GB.UISys.List_Angle.Add((short)Data16[26750 + j - 6000]);
					GB.UISys.List_Torq.Add((short)Data16[28750 + j - 6000]);
					GB.UISys.List_TorqRate.Add((short)Data16[30750 + j - 6000]);
				}
			}
			for (int k = 0; k < 100; k++)
			{
				GB.UISys.List_Info.Data16[k] = Data16[k];
			}
			DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)Data16[100]).AddSeconds(Data16[102] * 65536 + Data16[101]);
			GB.UISys.List_Info.Year = (ushort)OpTime.Year;
			GB.UISys.List_Info.Month = (ushort)OpTime.Month;
			GB.UISys.List_Info.Day = (ushort)OpTime.Day;
			GB.UISys.List_Info.Hour = (ushort)OpTime.Hour;
			GB.UISys.List_Info.Min = (ushort)OpTime.Minute;
			GB.UISys.List_Info.Sec = (ushort)OpTime.Second;
			for (int l = 0; l < 47; l++)
			{
				GB.UISys.List_Info.Data16[l + 100 + 6] = Data16[100 + l + 3];
			}
			for (int m = 0; m < 550; m++)
			{
				GB.UISys.List_Param_Unit.Add(Data16[8200 + m]);
			}
			if (Data16.Length >= 32950)
			{
				for (int n = 0; n < 200; n++)
				{
					GB.UISys.List_OtherInfo.Data16[n] = Data8[32750 + n];
				}
			}
		}
	}
}
