using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SD3_Graph
{
	internal class ReaderCSV
	{
		private OpenFileDialog dialog;

		private GlobalVar GB;

		private string[] subs_Title;

		private string[] subs_Info;

		private string[] subs_ts;

		private string[] subs_A;

		private string[] subs_T;

		private string[] subs_TR;

		private string[] subs_SC;

		private string[] subs_Param;

		public ReaderCSV(OpenFileDialog dialog, GlobalVar GB)
		{
			this.dialog = dialog;
			this.GB = GB;
		}

		public bool ReadFile(string CStr)
		{
			string line_Title = ReadLine(CStr, 1);
			subs_Title = line_Title.Split(',');
			int LastOneIdx = ((subs_Title.Count() > 0) ? (subs_Title.Count() - 1) : 0);
			bool SingleResultCSV = subs_Title[0].Contains("Ver01") || subs_Title[0].Contains("Ver02");
			bool ReportResultCSV = subs_Title[0].Contains("Year") && subs_Title[LastOneIdx].Contains("Ver01");
			if (SingleResultCSV)
			{
				string line_Info = ReadLine(CStr, 5);
				string line_ts = ReadLine(CStr, 7);
				string line_A = ReadLine(CStr, 9);
				string line_T = ReadLine(CStr, 11);
				string line_TR = ReadLine(CStr, 13);
				string line_Scale = ReadLine(CStr, 15);
				string line_Param = ReadLine(CStr, 3);
				subs_Info = line_Info.Split(',');
				subs_ts = line_ts.Split(',');
				subs_A = line_A.Split(',');
				subs_T = line_T.Split(',');
				subs_TR = line_TR.Split(',');
				subs_SC = line_Scale.Split(',');
				subs_Param = line_Param.Split(',');
				ParseFileCSV();
			}
			if (ReportResultCSV)
			{
				string line_Info2 = ReadLine(CStr, 2);
				string line_ts2 = ReadLine(CStr, 6);
				string line_A2 = ReadLine(CStr, 10);
				string line_T2 = ReadLine(CStr, 14);
				string line_TR2 = ReadLine(CStr, 18);
				string line_Scale2 = ReadLine(CStr, 22);
				string line_Param2 = ReadLine(CStr, 27);
				subs_Info = line_Info2.Split(',');
				subs_ts = line_ts2.Split(',');
				subs_A = line_A2.Split(',');
				subs_T = line_T2.Split(',');
				subs_TR = line_TR2.Split(',');
				subs_SC = line_Scale2.Split(',');
				subs_Param = line_Param2.Split(',');
				ParseFileCSV();
			}
			return SingleResultCSV || ReportResultCSV;
		}

		public void ParseFileCSV()
		{
			ushort RawCoef = 0;
			string RawStr = subs_Info[47];
			RawCoef = (ushort)((!(MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit0") == RawStr)) ? ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit1") == RawStr) ? 1 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit2") == RawStr) ? 2 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit3") == RawStr) ? 3 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit4") == RawStr) ? 4 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit5") == RawStr) ? 5 : ((!(MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit6") == RawStr)) ? (ushort.TryParse(RawStr?.ToString(), out RawCoef) ? RawCoef : 0) : 6)))))) : 0);
			ushort UserCoef = 0;
			string UserStr = subs_Info[24];
			UserCoef = (ushort)((!(MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit0") == UserStr)) ? ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit1") == UserStr) ? 1 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit2") == UserStr) ? 2 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit3") == UserStr) ? 3 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit4") == UserStr) ? 4 : ((MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit5") == UserStr) ? 5 : ((!(MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit6") == UserStr)) ? (ushort.TryParse(UserStr?.ToString(), out UserCoef) ? UserCoef : 0) : 6)))))) : 0);
			double ParamUserUnitCoefThenConvertSystemUnit = GB.TorqUnitcoef(1000 + RawCoef) / GB.TorqUnitcoef(1000 + UserCoef);
			ushort CurveType = ushort.Parse(subs_SC[34]);
			GB.UISys.List_Scale.Stage1Angle = short.Parse(subs_SC[0]);
			GB.UISys.List_Scale.Stage2Angle = short.Parse(subs_SC[1]);
			GB.UISys.List_Scale.Stage3Angle = short.Parse(subs_SC[2]);
			GB.UISys.List_Scale.Stage4Angle = short.Parse(subs_SC[3]);
			GB.UISys.List_Scale.Stage5Angle = short.Parse(subs_SC[4]);
			GB.UISys.List_Scale.Stage6Angle = short.Parse(subs_SC[5]);
			GB.UISys.List_Scale.Loosening1Angle = short.Parse(subs_SC[6]);
			GB.UISys.List_Scale.Loosening2Angle = short.Parse(subs_SC[7]);
			GB.UISys.List_Scale.Stage1Torque = (short)(double.Parse(subs_SC[8]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Stage2Torque = (short)(double.Parse(subs_SC[9]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Stage3Torque = (short)(double.Parse(subs_SC[10]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Stage4Torque = (short)(double.Parse(subs_SC[11]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Stage5Torque = (short)(double.Parse(subs_SC[12]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Stage6Torque = (short)(double.Parse(subs_SC[13]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Loosening1Torque = (short)(double.Parse(subs_SC[14]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Loosening2Torque = (short)(double.Parse(subs_SC[15]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Stage1Time = ushort.Parse(subs_SC[16]);
			GB.UISys.List_Scale.Stage2Time = ushort.Parse(subs_SC[17]);
			GB.UISys.List_Scale.Stage3Time = ushort.Parse(subs_SC[18]);
			GB.UISys.List_Scale.Stage4Time = ushort.Parse(subs_SC[19]);
			GB.UISys.List_Scale.Stage5Time = ushort.Parse(subs_SC[20]);
			GB.UISys.List_Scale.Stage6Time = ushort.Parse(subs_SC[21]);
			GB.UISys.List_Scale.Loosening1Time = ushort.Parse(subs_SC[22]);
			GB.UISys.List_Scale.Loosening2Time = ushort.Parse(subs_SC[23]);
			GB.UISys.List_Scale.Curve_MaxTime = short.Parse(subs_SC[24]);
			GB.UISys.List_Scale.Curve_MaxAngle = short.Parse(subs_SC[25]);
			GB.UISys.List_Scale.Curve_MaxTorque = (short)(double.Parse(subs_SC[26]) * ParamUserUnitCoefThenConvertSystemUnit);
			if (CurveType == 2)
			{
				GB.UISys.List_Scale.Curve_MaxTorqueRate = short.Parse(subs_SC[15]);
			}
			else
			{
				GB.UISys.List_Scale.Curve_MaxTorqueRate = (short)(double.Parse(subs_SC[15]) * ParamUserUnitCoefThenConvertSystemUnit);
			}
			GB.UISys.List_Scale.Curve_TotalPoint = ushort.Parse(subs_SC[28]);
			GB.UISys.List_Scale.SetMaxTorque = (ushort)(double.Parse(subs_SC[29]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.SetMinTorque = (ushort)(double.Parse(subs_SC[30]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.SetMaxTorqRate = (ushort)(double.Parse(subs_SC[31]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.SetMaxAngle = ushort.Parse(subs_SC[32]);
			GB.UISys.List_Scale.SetMinAngle = ushort.Parse(subs_SC[33]);
			GB.UISys.List_Scale.CurveVer = ushort.Parse(subs_SC[34]);
			GB.UISys.List_Scale.CurveFreqModeVer = ushort.Parse(subs_SC[35]);
			GB.UISys.List_Scale.CurveMaxTorqueRate = (short)(double.Parse(subs_SC[36]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Curve_MinTime = short.Parse(subs_SC[37]);
			GB.UISys.List_Scale.Curve_MinAngle = short.Parse(subs_SC[38]);
			GB.UISys.List_Scale.Curve_MinTorque = (short)(double.Parse(subs_SC[39]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Scale.Curve_MinTorqueRate = (short)(double.Parse(subs_SC[40]) * ParamUserUnitCoefThenConvertSystemUnit);
			if (CurveType == 2)
			{
				GB.UISys.List_Scale.Stage1SwitchTorq = short.Parse(subs_SC[41]);
				GB.UISys.List_Scale.Stage2SwitchTorq = short.Parse(subs_SC[42]);
				GB.UISys.List_Scale.Stage3SwitchTorq = short.Parse(subs_SC[43]);
				GB.UISys.List_Scale.Stage4SwitchTorq = short.Parse(subs_SC[44]);
				GB.UISys.List_Scale.Stage5SwitchTorq = short.Parse(subs_SC[45]);
				GB.UISys.List_Scale.Stage6SwitchTorq = short.Parse(subs_SC[46]);
			}
			else
			{
				GB.UISys.List_Scale.Stage1SwitchTorq = (short)(double.Parse(subs_SC[41]) * ParamUserUnitCoefThenConvertSystemUnit);
				GB.UISys.List_Scale.Stage2SwitchTorq = (short)(double.Parse(subs_SC[42]) * ParamUserUnitCoefThenConvertSystemUnit);
				GB.UISys.List_Scale.Stage3SwitchTorq = (short)(double.Parse(subs_SC[43]) * ParamUserUnitCoefThenConvertSystemUnit);
				GB.UISys.List_Scale.Stage4SwitchTorq = (short)(double.Parse(subs_SC[44]) * ParamUserUnitCoefThenConvertSystemUnit);
				GB.UISys.List_Scale.Stage5SwitchTorq = (short)(double.Parse(subs_SC[45]) * ParamUserUnitCoefThenConvertSystemUnit);
				GB.UISys.List_Scale.Stage6SwitchTorq = (short)(double.Parse(subs_SC[46]) * ParamUserUnitCoefThenConvertSystemUnit);
			}
			for (int j = 0; j < GB.UISys.List_Scale.Curve_TotalPoint; j++)
			{
				long TorqVal = long.Parse(subs_T[j]);
				if (TorqVal > 65536)
				{
					TorqVal -= uint.MaxValue;
				}
				long TorqRateVal = long.Parse(subs_TR[j]);
				if (TorqRateVal > 65536)
				{
					TorqRateVal -= uint.MaxValue;
				}
				GB.UISys.List_Time.Add(short.Parse(subs_ts[j]));
				GB.UISys.List_Angle.Add(short.Parse(subs_A[j]));
				GB.UISys.List_Torq.Add((short)((double)TorqVal * ParamUserUnitCoefThenConvertSystemUnit));
				GB.UISys.List_TorqRate.Add((short)((double)TorqRateVal * ParamUserUnitCoefThenConvertSystemUnit));
			}
			GB.UISys.List_Info.Year = ushort.Parse(subs_Info[0]);
			GB.UISys.List_Info.Month = ushort.Parse(subs_Info[1]);
			GB.UISys.List_Info.Day = ushort.Parse(subs_Info[2]);
			GB.UISys.List_Info.Hour = ushort.Parse(subs_Info[3]);
			GB.UISys.List_Info.Min = ushort.Parse(subs_Info[4]);
			GB.UISys.List_Info.Sec = ushort.Parse(subs_Info[5]);
			GB.SetNameTitleStr(FormType.SubSNFromBinFile, 0, subs_Info[6]);
			GB.UISys.List_Info.Tool = ushort.Parse(subs_Info[7]);
			GB.UISys.List_Info.ScrewNo = uint.Parse(subs_Info[8]);
			GB.UISys.List_Info.ParmID = (ushort)(ushort.TryParse(subs_Info[9]?.ToString(), out var SeqID) ? SeqID : 0);
			GB.UISys.List_Info.ParmID = (ushort)(ushort.TryParse(subs_Info[10]?.ToString(), out var ParamID) ? ParamID : 0);
			GB.UISys.List_Info.TargetTorque = (ushort)(double.Parse(subs_Info[11]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.TargetAngle = ushort.Parse(subs_Info[12]);
			GB.UISys.List_Info.TargetTorqueRate = (ushort)(double.Parse(subs_Info[13]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.FinalTorque = (ushort)(double.Parse(subs_Info[14]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.TighteningAngle = ushort.Parse(subs_Info[15]);
			GB.UISys.List_Info.TotalAngle = short.Parse(subs_Info[16]);
			ushort StatusID = 0;
			string StatusStr = subs_Info[17];
			switch (StatusStr)
			{
			case "Tightening OK":
				GB.UISys.List_Info.Status = 1;
				break;
			case "Tightening NOK":
				GB.UISys.List_Info.Status = 2;
				break;
			case "Loosening OK":
				GB.UISys.List_Info.Status = 3;
				break;
			case "Loosening NOK":
				GB.UISys.List_Info.Status = 4;
				break;
			case "Pass":
				GB.UISys.List_Info.Status = 5;
				break;
			default:
				GB.UISys.List_Info.Status = (ushort)(ushort.TryParse(StatusStr?.ToString(), out StatusID) ? StatusID : 0);
				break;
			}
			GB.UISys.List_Info.CT_Time = ushort.Parse(subs_Info[18]);
			GB.UISys.List_Info.ErrorCode = ushort.Parse(subs_Info[19]);
			GB.UISys.List_Info.MaxTighteningAngle = ushort.Parse(subs_Info[20]);
			GB.UISys.List_Info.MinTighteningAngle = ushort.Parse(subs_Info[21]);
			GB.UISys.List_Info.MaxTorque = (ushort)(double.Parse(subs_Info[22]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.MinTorque = (ushort)(double.Parse(subs_Info[23]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.TorqueUnit = UserCoef;
			GB.UISys.List_Info.ToolMaxTorque_NM = ushort.Parse(subs_Info[25]);
			GB.UISys.List_Info.ToolProtectTorque = (ushort)(double.Parse(subs_Info[26]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.PreTighteningTorque = (ushort)(double.Parse(subs_Info[27]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.SetMaxTime = ushort.Parse(subs_Info[28]);
			GB.UISys.List_Info.SetMaxAngle = ushort.Parse(subs_Info[29]);
			GB.UISys.List_Info.FinalStage_SetMaxTorque = (ushort)(double.Parse(subs_Info[30]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.FinalStage_SetMinTorque = (ushort)(double.Parse(subs_Info[31]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.FinalStage_SetMaxAngle = ushort.Parse(subs_Info[32]);
			GB.UISys.List_Info.FinalStage_SetMinAngle = ushort.Parse(subs_Info[33]);
			GB.UISys.List_Info.FinalStage_SetMaxTime = ushort.Parse(subs_Info[34]);
			GB.UISys.List_Info.FinalStage_SetMinTime = ushort.Parse(subs_Info[35]);
			GB.UISys.List_Info.PrevailTorque = (ushort)(double.Parse(subs_Info[36]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.AppliedTorque = (ushort)(double.Parse(subs_Info[37]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.FinalCurrent = ushort.Parse(subs_Info[38]);
			GB.UISys.List_Info.ClampTorque = (ushort)(double.Parse(subs_Info[39]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.SetMaxClampTorque = (ushort)(double.Parse(subs_Info[40]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.SetMinClampTorque = (ushort)(double.Parse(subs_Info[41]) * ParamUserUnitCoefThenConvertSystemUnit);
			GB.UISys.List_Info.ClampAngle = ushort.Parse(subs_Info[42]);
			GB.UISys.List_Info.SetMaxClampAngle = ushort.Parse(subs_Info[43]);
			GB.UISys.List_Info.SetMinClampAngle = ushort.Parse(subs_Info[44]);
			GB.UISys.List_Info.SetMinAngle = ushort.Parse(subs_Info[45]);
			ushort UserID = 0;
			GB.UISys.List_Info.UserID = (ushort)(ushort.TryParse(subs_Info[46]?.ToString(), out UserID) ? UserID : 0);
			GB.UISys.List_Info.FWSystemCoef = RawCoef;
			GB.UISys.List_Info.TargetYield = ushort.Parse(subs_Info[48]);
			for (int i = 0; i < 550; i++)
			{
				GB.UISys.List_Param_Unit.Add(ushort.Parse(subs_Param[i]));
			}
			GB.UISys.List_OtherInfo = default(OtherInfo);
		}

		public string ReadLine(string FilePath, int LineNumber)
		{
			string result = "";
			try
			{
				if (File.Exists(FilePath))
				{
					using (StreamReader _StreamReader = new StreamReader(FilePath))
					{
						for (int a = 0; a < LineNumber; a++)
						{
							result = _StreamReader.ReadLine();
						}
					}
				}
			}
			catch
			{
			}
			return result;
		}
	}
}
