using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form524_RS485A : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private IContainer components = null;

		private GroupBox COMGB;

		private TextBox ZoffsTB;

		private Label lab_Zoffs;

		private TextBox YoffsTB;

		private Label lab_Yoffs;

		private TextBox XoffsTB;

		private Label lab_Xoffs;

		private TextBox TolerTB;

		private Label lab_Toler;

		private ComboBox Func_CB;

		private Button btn_ExportPortCSV;

		private Button btn_ImportPortCSV;

		private TextBox TolerZTB;

		private Label lab_TolerZ;

		private Label label1;

		private Label Func1_lab;

		private GroupBox P95GB;

		private TextBox L2TB;

		private TextBox L1TB;

		private Label label2;

		private Label L1_lab;

		private Button btnPortDownload;

		private Button btnPortUpload;

		private Label labNowX;

		private Label labNowY;

		private Label labNowZ;

		public Form524_RS485A(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			MultiLanguage.LoadLanguage(this, "FormCtrlBase");
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(btnPortDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnPortUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportPortCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportPortCSV, GB.UISys.ExportToCSV);
			L1TB.KeyPress += EVENT_L1L2_KeyPress;
			L1TB.LostFocus += GB.LostFocus_C0;
			L1TB.Leave += TBLeave;
			toolTip.SetToolTip(L1TB, GB.UISys.RangeStr + "100-5000");
			L2TB.KeyPress += EVENT_L1L2_KeyPress;
			L2TB.LostFocus += GB.LostFocus_C0;
			L2TB.Leave += TBLeave;
			toolTip.SetToolTip(L2TB, GB.UISys.RangeStr + "100-5000");
			TolerTB.KeyPress += EVENT_Toler_KeyPress;
			TolerTB.LostFocus += GB.LostFocus_C0;
			TolerTB.Leave += TBLeave;
			toolTip.SetToolTip(TolerTB, GB.UISys.RangeStr + "0-4294967295");
			TolerZTB.KeyPress += EVENT_Toler_KeyPress;
			TolerZTB.LostFocus += GB.LostFocus_C0;
			TolerZTB.Leave += TBLeave;
			toolTip.SetToolTip(TolerZTB, GB.UISys.RangeStr + "0-4294967295");
			XoffsTB.KeyPress += EVENT_Offs_KeyPress;
			XoffsTB.LostFocus += GB.LostFocus_C0;
			XoffsTB.Leave += TBLeave;
			toolTip.SetToolTip(XoffsTB, GB.UISys.RangeStr + "-2147483648-2147483647");
			YoffsTB.KeyPress += EVENT_Offs_KeyPress;
			YoffsTB.LostFocus += GB.LostFocus_C0;
			YoffsTB.Leave += TBLeave;
			toolTip.SetToolTip(YoffsTB, GB.UISys.RangeStr + "-2147483648-2147483647");
			ZoffsTB.KeyPress += EVENT_Offs_KeyPress;
			ZoffsTB.LostFocus += GB.LostFocus_C0;
			ZoffsTB.Leave += TBLeave;
			toolTip.SetToolTip(ZoffsTB, GB.UISys.RangeStr + "-2147483648-2147483647");
			TCP.FSIDRead_ByTCP(565, 0, 0, 0, 0, 0);
			UpdataUI();
			FormControlZoom.SetControls(this);
		}

		public void EVENT_L1L2_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned5000_100(sender, e);
			if (e.KeyChar == '\r')
			{
				TBLeave(sender, e);
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			}
		}

		public void EVENT_Toler_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned4294967295(sender, e);
			if (e.KeyChar == '\r')
			{
				TBLeave(sender, e);
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			}
		}

		public void EVENT_Offs_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned2147483647(sender, e);
			if (e.KeyChar == '\r')
			{
				TBLeave(sender, e);
				TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			}
		}

		private void UpdataUI()
		{
			Func_CB.SelectedIndexChanged -= Func_CB_SelectedIndexChanged;
			Func_CB.Items.Clear();
			if (GB.FSModelTypeInfo.MesModelType == 1)
			{
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncA"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncB"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncD"));
				if (GB.CheckHMIVer(171, 0))
				{
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncF"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncH"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncJ"));
				}
				if (GB.CheckHMIVer(172, 20))
				{
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncL"));
				}
				if ((GB.FSCtrlComPortFunction.RS485Function == 1 || GB.FSCtrlComPortFunction.RS485Function == 2) && Func_CB.Items.Count >= 1)
				{
					Func_CB.SelectedIndex = 1;
				}
				else if ((GB.FSCtrlComPortFunction.RS485Function == 3 || GB.FSCtrlComPortFunction.RS485Function == 4) && Func_CB.Items.Count >= 2)
				{
					Func_CB.SelectedIndex = 2;
				}
				else if ((GB.FSCtrlComPortFunction.RS485Function == 5 || GB.FSCtrlComPortFunction.RS485Function == 6) && Func_CB.Items.Count >= 3)
				{
					Func_CB.SelectedIndex = 3;
				}
				else if ((GB.FSCtrlComPortFunction.RS485Function == 7 || GB.FSCtrlComPortFunction.RS485Function == 8) && Func_CB.Items.Count >= 4)
				{
					Func_CB.SelectedIndex = 4;
				}
				else if ((GB.FSCtrlComPortFunction.RS485Function == 9 || GB.FSCtrlComPortFunction.RS485Function == 10) && Func_CB.Items.Count >= 5)
				{
					Func_CB.SelectedIndex = 5;
				}
				else if ((GB.FSCtrlComPortFunction.RS485Function == 11 || GB.FSCtrlComPortFunction.RS485Function == 12) && Func_CB.Items.Count >= 6)
				{
					Func_CB.SelectedIndex = 6;
				}
				else
				{
					Func_CB.SelectedIndex = 0;
				}
			}
			else
			{
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncA"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncB"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncC"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncD"));
				Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncE"));
				if (GB.CheckHMIVer(171, 0))
				{
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncF"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncG"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncH"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncI"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncJ"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncK"));
				}
				if (GB.CheckHMIVer(172, 20))
				{
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncL"));
					Func_CB.Items.Add(MultiLanguage.GetStr("FormCtrlBase", "tp_ComFuncM"));
				}
				if (GB.FSCtrlComPortFunction.RS485Function < Func_CB.Items.Count)
				{
					Func_CB.SelectedIndex = GB.FSCtrlComPortFunction.RS485Function;
				}
			}
			P95GB.Visible = ((GB.FSCtrlComPortFunction.RS485Function == 3 || GB.FSCtrlComPortFunction.RS485Function == 4) ? true : false);
			Label label = labNowX;
			Label label2 = labNowY;
			bool flag = (labNowZ.Visible = ((GB.FSCtrlComPortFunction.RS485Function > 0) ? true : false));
			bool visible = (label2.Visible = flag);
			label.Visible = visible;
			Func_CB.SelectedIndexChanged += Func_CB_SelectedIndexChanged;
			COMGB.Visible = ((GB.FSCtrlComPortFunction.RS485Function != 0) ? true : false);
			if (GB.FSCtrlComPortFunction.RS485Function == 1 || GB.FSCtrlComPortFunction.RS485Function == 3 || GB.FSCtrlComPortFunction.RS485Function == 5 || GB.FSCtrlComPortFunction.RS485Function == 7 || GB.FSCtrlComPortFunction.RS485Function == 9 || GB.FSCtrlComPortFunction.RS485Function == 11)
			{
				TolerTB.Text = (GB.FSCtrlComPortFunction.Arm1_PosErr_H * 65536 + GB.FSCtrlComPortFunction.Arm1_PosErr_L).ToString();
				TolerZTB.Text = (GB.FSCtrlComPortFunction.Arm1_PosZErr_H * 65536 + GB.FSCtrlComPortFunction.Arm1_PosZErr_L).ToString();
				XoffsTB.Text = (GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_L).ToString();
				YoffsTB.Text = (GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_L).ToString();
				ZoffsTB.Text = (GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_L).ToString();
			}
			else if (GB.FSCtrlComPortFunction.RS485Function == 2 || GB.FSCtrlComPortFunction.RS485Function == 4 || GB.FSCtrlComPortFunction.RS485Function == 6 || GB.FSCtrlComPortFunction.RS485Function == 8 || GB.FSCtrlComPortFunction.RS485Function == 10 || GB.FSCtrlComPortFunction.RS485Function == 12)
			{
				TolerTB.Text = (GB.FSCtrlComPortFunction.Arm2_PosErr_H * 65536 + GB.FSCtrlComPortFunction.Arm2_PosErr_L).ToString();
				TolerZTB.Text = (GB.FSCtrlComPortFunction.Arm2_PosZErr_H * 65536 + GB.FSCtrlComPortFunction.Arm2_PosZErr_L).ToString();
				XoffsTB.Text = (GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_L).ToString();
				YoffsTB.Text = (GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_L).ToString();
				ZoffsTB.Text = (GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_H * 65536 + GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_L).ToString();
			}
			L1TB.Text = GB.FSCtrlComPortFunction.P95A0_L1.ToString();
			L2TB.Text = GB.FSCtrlComPortFunction.P95A0_L2.ToString();
		}

		private void TBLeave(object sender, EventArgs e)
		{
			if (GB.FSCtrlComPortFunction.RS485Function == 1 || GB.FSCtrlComPortFunction.RS485Function == 3 || GB.FSCtrlComPortFunction.RS485Function == 5 || GB.FSCtrlComPortFunction.RS485Function == 7 || GB.FSCtrlComPortFunction.RS485Function == 9 || GB.FSCtrlComPortFunction.RS485Function == 11)
			{
				GB.FSCtrlComPortFunction.Arm1_PosErr_H = (ushort)(uint.Parse(TolerTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm1_PosErr_L = (ushort)(uint.Parse(TolerTB.Text) - GB.FSCtrlComPortFunction.Arm1_PosErr_H * 65536);
				GB.FSCtrlComPortFunction.Arm1_PosZErr_H = (ushort)(uint.Parse(TolerZTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm1_PosZErr_L = (ushort)(uint.Parse(TolerZTB.Text) - GB.FSCtrlComPortFunction.Arm1_PosZErr_H * 65536);
				GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_H = (ushort)(uint.Parse(XoffsTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_L = (ushort)(uint.Parse(XoffsTB.Text) - GB.FSCtrlComPortFunction.Arm1_CoordinateXOffs_H * 65536);
				GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_H = (ushort)(uint.Parse(YoffsTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_L = (ushort)(uint.Parse(YoffsTB.Text) - GB.FSCtrlComPortFunction.Arm1_CoordinateYOffs_H * 65536);
				GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_H = (ushort)(uint.Parse(ZoffsTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_L = (ushort)(uint.Parse(ZoffsTB.Text) - GB.FSCtrlComPortFunction.Arm1_CoordinateZOffs_H * 65536);
			}
			else if (GB.FSCtrlComPortFunction.RS485Function == 2 || GB.FSCtrlComPortFunction.RS485Function == 4 || GB.FSCtrlComPortFunction.RS485Function == 6 || GB.FSCtrlComPortFunction.RS485Function == 8 || GB.FSCtrlComPortFunction.RS485Function == 10 || GB.FSCtrlComPortFunction.RS485Function == 12)
			{
				GB.FSCtrlComPortFunction.Arm2_PosErr_H = (ushort)(uint.Parse(TolerTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm2_PosErr_L = (ushort)(uint.Parse(TolerTB.Text) - GB.FSCtrlComPortFunction.Arm2_PosErr_H * 65536);
				GB.FSCtrlComPortFunction.Arm2_PosZErr_H = (ushort)(uint.Parse(TolerZTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm2_PosZErr_L = (ushort)(uint.Parse(TolerZTB.Text) - GB.FSCtrlComPortFunction.Arm2_PosZErr_H * 65536);
				GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_H = (ushort)(uint.Parse(XoffsTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_L = (ushort)(uint.Parse(XoffsTB.Text) - GB.FSCtrlComPortFunction.Arm2_CoordinateXOffs_H * 65536);
				GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_H = (ushort)(uint.Parse(YoffsTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_L = (ushort)(uint.Parse(YoffsTB.Text) - GB.FSCtrlComPortFunction.Arm2_CoordinateYOffs_H * 65536);
				GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_H = (ushort)(uint.Parse(ZoffsTB.Text) / 65536);
				GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_L = (ushort)(uint.Parse(ZoffsTB.Text) - GB.FSCtrlComPortFunction.Arm2_CoordinateZOffs_H * 65536);
			}
			ushort L1 = ushort.Parse(L1TB.Text);
			GB.FSCtrlComPortFunction.P95A0_L1 = (ushort)((L1 < 1000) ? 475 : GB.FSCtrlComPortFunction.P95A0_L1);
			ushort L2 = ushort.Parse(L2TB.Text);
			GB.FSCtrlComPortFunction.P95A0_L2 = (ushort)((L2 < 1000) ? 475 : GB.FSCtrlComPortFunction.P95A0_L2);
		}

		public void ExportCSVPortFunction(string ExStr)
		{
			if (TrCSV.WriteCtrlPortFile(ExStr, true))
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
		}

		private void ImportCSVPortFunction()
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.InitialDirectory = "..\\ScrewInfo\\";
				dialog.Title = "Select *.csv";
				if (GB.FSModelTypeInfo.MesModelType == 0)
				{
					dialog.Filter = "CtrlPort files (*.csv)|*CtrlPort.csv";
				}
				else
				{
					dialog.Filter = "CtrlPort010 files (*.csv)|*CtrlPort010.csv";
				}
				dialog.Multiselect = true;
				if (dialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
				string[] fileNames = dialog.FileNames;
				foreach (string strFilename in fileNames)
				{
					bool IsCSV = strFilename.Contains(".csv");
					bool Rst = false;
					if (IsCSV)
					{
						Rst = TrCSV.ReadCtrlPortFile(strFilename);
						if (Rst)
						{
							UpdataUI();
						}
						else
						{
							Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
							Form995.Show(this);
						}
						if (GB.UISys.PCSoftSupport && Rst)
						{
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += AllDataWriteToCtrlPort;
							Form996.SetSubForm(FormType.MegCtrlWriteAll);
							Form996.ShowDialog(this);
						}
					}
				}
			}
		}

		private void btn_ExportPortCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportNonTitle, GB);
			Form997.CreateID += ExportCSVPortFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportPortCSV_Click(object sender, EventArgs e)
		{
			ImportCSVPortFunction();
		}

		private void Func_CB_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (GB.FSModelTypeInfo.MesModelType == 1)
			{
				if (Func_CB.SelectedIndex == 1)
				{
					GB.FSCtrlComPortFunction.RS485Function = 1;
				}
				else if (Func_CB.SelectedIndex == 2)
				{
					GB.FSCtrlComPortFunction.RS485Function = 3;
				}
				else if (Func_CB.SelectedIndex == 3)
				{
					GB.FSCtrlComPortFunction.RS485Function = 5;
				}
				else if (Func_CB.SelectedIndex == 4)
				{
					GB.FSCtrlComPortFunction.RS485Function = 7;
				}
				else if (Func_CB.SelectedIndex == 5)
				{
					GB.FSCtrlComPortFunction.RS485Function = 9;
				}
				else if (Func_CB.SelectedIndex == 6)
				{
					GB.FSCtrlComPortFunction.RS485Function = 11;
				}
				else
				{
					GB.FSCtrlComPortFunction.RS485Function = 0;
				}
			}
			else
			{
				GB.FSCtrlComPortFunction.RS485Function = (ushort)Func_CB.SelectedIndex;
			}
			TCP.FSIDWrite_ByTCP(521, 0, 0, 0, 0, 0);
			UpdataUI();
		}

		private void Form524_RS485A_Load(object sender, EventArgs e)
		{
			GB.GetPositionArmTimer = new Timer();
			GB.GetPositionArmTimer.Interval = 300;
			GB.GetPositionArmTimer.Tick += Timer_Tick;
			GB.GetPositionArmTimer.Start();
		}

		private void btnPortUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheCtrlPort;
			Form996.SetSubForm(FormType.MegCtrlReadAll);
			Form996.ShowDialog(this);
		}

		private void AllDataReadTheCtrlPort()
		{
			TrCSV.CtrlPortAllDataReadFromCtrl();
			UpdataUI();
		}

		private void btnPortDownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToCtrlPort;
			Form996.SetSubForm(FormType.MegCtrlWriteAll);
			Form996.ShowDialog(this);
		}

		private void AllDataWriteToCtrlPort()
		{
			GB.ALNGMsgStartStopFunction(false);
			int Err = TrCSV.CtrlPortAllDataWriteToCtrl(true);
			GB.ALNGMsgStartStopFunction(true);
			if (Err != -4 && Err > 0)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5005, " ErrCode:" + Err.ToString("D3"));
				Form995.Show(this);
			}
			else
			{
				Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, 1001, "");
				Form996.Show(this);
			}
		}

		private void Form524_RS485A_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (GB.GetPositionArmTimer != null)
			{
				GB.GetPositionArmTimer.Stop();
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			double PosX = 0.0;
			double PosY = 0.0;
			double PosZ = 0.0;
			if (GB.FSCtrlComPortFunction.RS485Function == 1 || GB.FSCtrlComPortFunction.RS485Function == 3 || GB.FSCtrlComPortFunction.RS485Function == 5 || GB.FSCtrlComPortFunction.RS485Function == 7 || GB.FSCtrlComPortFunction.RS485Function == 9 || GB.FSCtrlComPortFunction.RS485Function == 11)
			{
				PosX = (double)(GB.TcpStatus.Detail.T1StB.Fed_PositioningArmX_H_33 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmX_L_32) / 10.0;
				PosY = (double)(GB.TcpStatus.Detail.T1StB.Fed_PositioningArmY_H_35 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmY_L_34) / 10.0;
				PosZ = (double)(GB.TcpStatus.Detail.T1StB.Fed_PositioningArmZ_H_37 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmZ_L_36) / 10.0;
			}
			else if (GB.FSCtrlComPortFunction.RS485Function == 2 || GB.FSCtrlComPortFunction.RS485Function == 4 || GB.FSCtrlComPortFunction.RS485Function == 6 || GB.FSCtrlComPortFunction.RS485Function == 8 || GB.FSCtrlComPortFunction.RS485Function == 10 || GB.FSCtrlComPortFunction.RS485Function == 12)
			{
				PosX = (double)(GB.TcpStatus.Detail.T2StB.Fed_PositioningArmX_H_33 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmX_L_32) / 10.0;
				PosY = (double)(GB.TcpStatus.Detail.T2StB.Fed_PositioningArmY_H_35 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmY_L_34) / 10.0;
				PosZ = (double)(GB.TcpStatus.Detail.T2StB.Fed_PositioningArmZ_H_37 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmZ_L_36) / 10.0;
			}
			if (labNowX.InvokeRequired)
			{
				labNowX.Invoke((Action)delegate
				{
					labNowX.Text = "X: " + PosX.ToString("F1");
				});
			}
			else
			{
				labNowX.Text = "X: " + PosX.ToString("F1");
			}
			if (labNowY.InvokeRequired)
			{
				labNowY.Invoke((Action)delegate
				{
					labNowY.Text = "Y: " + PosY.ToString("F1");
				});
			}
			else
			{
				labNowY.Text = "Y: " + PosY.ToString("F1");
			}
			if (labNowZ.InvokeRequired)
			{
				labNowZ.Invoke((Action)delegate
				{
					labNowZ.Text = "Z: " + PosZ.ToString("F1");
				});
			}
			else
			{
				labNowZ.Text = "Z: " + PosZ.ToString("F1");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.COMGB = new System.Windows.Forms.GroupBox();
			this.ZoffsTB = new System.Windows.Forms.TextBox();
			this.lab_Zoffs = new System.Windows.Forms.Label();
			this.YoffsTB = new System.Windows.Forms.TextBox();
			this.lab_Yoffs = new System.Windows.Forms.Label();
			this.XoffsTB = new System.Windows.Forms.TextBox();
			this.lab_Xoffs = new System.Windows.Forms.Label();
			this.TolerZTB = new System.Windows.Forms.TextBox();
			this.TolerTB = new System.Windows.Forms.TextBox();
			this.lab_TolerZ = new System.Windows.Forms.Label();
			this.lab_Toler = new System.Windows.Forms.Label();
			this.P95GB = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.Func1_lab = new System.Windows.Forms.Label();
			this.L2TB = new System.Windows.Forms.TextBox();
			this.L1TB = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.L1_lab = new System.Windows.Forms.Label();
			this.Func_CB = new System.Windows.Forms.ComboBox();
			this.btn_ExportPortCSV = new System.Windows.Forms.Button();
			this.btn_ImportPortCSV = new System.Windows.Forms.Button();
			this.btnPortDownload = new System.Windows.Forms.Button();
			this.btnPortUpload = new System.Windows.Forms.Button();
			this.labNowX = new System.Windows.Forms.Label();
			this.labNowY = new System.Windows.Forms.Label();
			this.labNowZ = new System.Windows.Forms.Label();
			this.COMGB.SuspendLayout();
			this.P95GB.SuspendLayout();
			base.SuspendLayout();
			this.COMGB.Controls.Add(this.ZoffsTB);
			this.COMGB.Controls.Add(this.lab_Zoffs);
			this.COMGB.Controls.Add(this.YoffsTB);
			this.COMGB.Controls.Add(this.lab_Yoffs);
			this.COMGB.Controls.Add(this.XoffsTB);
			this.COMGB.Controls.Add(this.lab_Xoffs);
			this.COMGB.Controls.Add(this.TolerZTB);
			this.COMGB.Controls.Add(this.TolerTB);
			this.COMGB.Controls.Add(this.lab_TolerZ);
			this.COMGB.Controls.Add(this.lab_Toler);
			this.COMGB.Controls.Add(this.P95GB);
			this.COMGB.Font = new System.Drawing.Font("新細明體", 12f);
			this.COMGB.Location = new System.Drawing.Point(182, 108);
			this.COMGB.Name = "COMGB";
			this.COMGB.Size = new System.Drawing.Size(771, 303);
			this.COMGB.TabIndex = 5;
			this.COMGB.TabStop = false;
			this.ZoffsTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.ZoffsTB.Location = new System.Drawing.Point(372, 219);
			this.ZoffsTB.Name = "ZoffsTB";
			this.ZoffsTB.Size = new System.Drawing.Size(340, 31);
			this.ZoffsTB.TabIndex = 1;
			this.ZoffsTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Zoffs.AutoSize = true;
			this.lab_Zoffs.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Zoffs.Location = new System.Drawing.Point(219, 222);
			this.lab_Zoffs.Name = "lab_Zoffs";
			this.lab_Zoffs.Size = new System.Drawing.Size(147, 20);
			this.lab_Zoffs.TabIndex = 0;
			this.lab_Zoffs.Text = "Z coordinate offset";
			this.lab_Zoffs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.YoffsTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.YoffsTB.Location = new System.Drawing.Point(372, 179);
			this.YoffsTB.Name = "YoffsTB";
			this.YoffsTB.Size = new System.Drawing.Size(340, 31);
			this.YoffsTB.TabIndex = 1;
			this.YoffsTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Yoffs.AutoSize = true;
			this.lab_Yoffs.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Yoffs.Location = new System.Drawing.Point(216, 182);
			this.lab_Yoffs.Name = "lab_Yoffs";
			this.lab_Yoffs.Size = new System.Drawing.Size(150, 20);
			this.lab_Yoffs.TabIndex = 0;
			this.lab_Yoffs.Text = "Y coordinate offset";
			this.lab_Yoffs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.XoffsTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.XoffsTB.Location = new System.Drawing.Point(372, 138);
			this.XoffsTB.Name = "XoffsTB";
			this.XoffsTB.Size = new System.Drawing.Size(340, 31);
			this.XoffsTB.TabIndex = 1;
			this.XoffsTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Xoffs.AutoSize = true;
			this.lab_Xoffs.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Xoffs.Location = new System.Drawing.Point(216, 143);
			this.lab_Xoffs.Name = "lab_Xoffs";
			this.lab_Xoffs.Size = new System.Drawing.Size(150, 20);
			this.lab_Xoffs.TabIndex = 0;
			this.lab_Xoffs.Text = "X coordinate offset";
			this.lab_Xoffs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.TolerZTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TolerZTB.Location = new System.Drawing.Point(372, 96);
			this.TolerZTB.Name = "TolerZTB";
			this.TolerZTB.Size = new System.Drawing.Size(340, 31);
			this.TolerZTB.TabIndex = 1;
			this.TolerZTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TolerTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TolerTB.Location = new System.Drawing.Point(372, 59);
			this.TolerTB.Name = "TolerTB";
			this.TolerTB.Size = new System.Drawing.Size(340, 31);
			this.TolerTB.TabIndex = 1;
			this.TolerTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TolerZ.AutoSize = true;
			this.lab_TolerZ.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TolerZ.Location = new System.Drawing.Point(201, 101);
			this.lab_TolerZ.Name = "lab_TolerZ";
			this.lab_TolerZ.Size = new System.Drawing.Size(165, 20);
			this.lab_TolerZ.TabIndex = 0;
			this.lab_TolerZ.Text = "Target tolerance(Z) ±";
			this.lab_TolerZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Toler.AutoSize = true;
			this.lab_Toler.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Toler.Location = new System.Drawing.Point(184, 64);
			this.lab_Toler.Name = "lab_Toler";
			this.lab_Toler.Size = new System.Drawing.Size(182, 20);
			this.lab_Toler.TabIndex = 0;
			this.lab_Toler.Text = "Target tolerance(XY) ±";
			this.lab_Toler.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.P95GB.Controls.Add(this.label1);
			this.P95GB.Controls.Add(this.Func1_lab);
			this.P95GB.Controls.Add(this.L2TB);
			this.P95GB.Controls.Add(this.L1TB);
			this.P95GB.Controls.Add(this.label2);
			this.P95GB.Controls.Add(this.L1_lab);
			this.P95GB.Location = new System.Drawing.Point(13, 27);
			this.P95GB.Name = "P95GB";
			this.P95GB.Size = new System.Drawing.Size(237, 126);
			this.P95GB.TabIndex = 3;
			this.P95GB.TabStop = false;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("新細明體", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.label1.Location = new System.Drawing.Point(17, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(205, 14);
			this.label1.TabIndex = 2;
			this.label1.Text = "Py = L2*sin(θ2+θ1)+L1*sin(θ1)";
			this.Func1_lab.AutoSize = true;
			this.Func1_lab.Font = new System.Drawing.Font("新細明體", 7.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.Func1_lab.Location = new System.Drawing.Point(16, 1);
			this.Func1_lab.Name = "Func1_lab";
			this.Func1_lab.Size = new System.Drawing.Size(209, 14);
			this.Func1_lab.TabIndex = 2;
			this.Func1_lab.Text = "Px = L2*cos(θ2+θ1)+L1*cos(θ1)";
			this.L2TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.L2TB.Location = new System.Drawing.Point(43, 76);
			this.L2TB.Name = "L2TB";
			this.L2TB.Size = new System.Drawing.Size(79, 31);
			this.L2TB.TabIndex = 1;
			this.L2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.L1TB.Font = new System.Drawing.Font("新細明體", 12f);
			this.L1TB.Location = new System.Drawing.Point(43, 42);
			this.L1TB.Name = "L1TB";
			this.L1TB.Size = new System.Drawing.Size(79, 31);
			this.L1TB.TabIndex = 1;
			this.L1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("新細明體", 12f);
			this.label2.Location = new System.Drawing.Point(16, 81);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 20);
			this.label2.TabIndex = 0;
			this.label2.Text = "L2                 mm";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.L1_lab.AutoSize = true;
			this.L1_lab.Font = new System.Drawing.Font("新細明體", 12f);
			this.L1_lab.Location = new System.Drawing.Point(16, 47);
			this.L1_lab.Name = "L1_lab";
			this.L1_lab.Size = new System.Drawing.Size(144, 20);
			this.L1_lab.TabIndex = 0;
			this.L1_lab.Text = "L1                 mm";
			this.L1_lab.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.Func_CB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.Func_CB.Font = new System.Drawing.Font("新細明體", 12f);
			this.Func_CB.FormattingEnabled = true;
			this.Func_CB.Location = new System.Drawing.Point(311, 68);
			this.Func_CB.Name = "Func_CB";
			this.Func_CB.Size = new System.Drawing.Size(508, 28);
			this.Func_CB.TabIndex = 4;
			this.btn_ExportPortCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportPortCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportPortCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportPortCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportPortCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportPortCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportPortCSV.Location = new System.Drawing.Point(1130, 5);
			this.btn_ExportPortCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportPortCSV.Name = "btn_ExportPortCSV";
			this.btn_ExportPortCSV.Size = new System.Drawing.Size(45, 45);
			this.btn_ExportPortCSV.TabIndex = 168;
			this.btn_ExportPortCSV.UseVisualStyleBackColor = true;
			this.btn_ExportPortCSV.Click += new System.EventHandler(btn_ExportPortCSV_Click);
			this.btn_ImportPortCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportPortCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportPortCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportPortCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportPortCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportPortCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportPortCSV.Location = new System.Drawing.Point(1180, 5);
			this.btn_ImportPortCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportPortCSV.Name = "btn_ImportPortCSV";
			this.btn_ImportPortCSV.Size = new System.Drawing.Size(45, 45);
			this.btn_ImportPortCSV.TabIndex = 167;
			this.btn_ImportPortCSV.UseVisualStyleBackColor = true;
			this.btn_ImportPortCSV.Click += new System.EventHandler(btn_ImportPortCSV_Click);
			this.btnPortDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnPortDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnPortDownload.FlatAppearance.BorderSize = 0;
			this.btnPortDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPortDownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnPortDownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnPortDownload.Location = new System.Drawing.Point(1079, 5);
			this.btnPortDownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnPortDownload.Name = "btnPortDownload";
			this.btnPortDownload.Size = new System.Drawing.Size(45, 45);
			this.btnPortDownload.TabIndex = 174;
			this.btnPortDownload.UseVisualStyleBackColor = true;
			this.btnPortDownload.Click += new System.EventHandler(btnPortDownload_Click);
			this.btnPortUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnPortUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnPortUpload.FlatAppearance.BorderSize = 0;
			this.btnPortUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPortUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnPortUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnPortUpload.Location = new System.Drawing.Point(1028, 5);
			this.btnPortUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnPortUpload.Name = "btnPortUpload";
			this.btnPortUpload.Size = new System.Drawing.Size(45, 45);
			this.btnPortUpload.TabIndex = 173;
			this.btnPortUpload.UseVisualStyleBackColor = true;
			this.btnPortUpload.Click += new System.EventHandler(btnPortUpload_Click);
			this.labNowX.AutoSize = true;
			this.labNowX.Location = new System.Drawing.Point(841, 22);
			this.labNowX.Name = "labNowX";
			this.labNowX.Size = new System.Drawing.Size(21, 15);
			this.labNowX.TabIndex = 175;
			this.labNowX.Text = "X:";
			this.labNowY.AutoSize = true;
			this.labNowY.Location = new System.Drawing.Point(841, 50);
			this.labNowY.Name = "labNowY";
			this.labNowY.Size = new System.Drawing.Size(21, 15);
			this.labNowY.TabIndex = 175;
			this.labNowY.Text = "Y:";
			this.labNowZ.AutoSize = true;
			this.labNowZ.Location = new System.Drawing.Point(841, 76);
			this.labNowZ.Name = "labNowZ";
			this.labNowZ.Size = new System.Drawing.Size(20, 15);
			this.labNowZ.TabIndex = 175;
			this.labNowZ.Text = "Z:";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			base.ClientSize = new System.Drawing.Size(1800, 500);
			base.Controls.Add(this.labNowZ);
			base.Controls.Add(this.labNowY);
			base.Controls.Add(this.labNowX);
			base.Controls.Add(this.btnPortDownload);
			base.Controls.Add(this.btnPortUpload);
			base.Controls.Add(this.btn_ExportPortCSV);
			base.Controls.Add(this.btn_ImportPortCSV);
			base.Controls.Add(this.Func_CB);
			base.Controls.Add(this.COMGB);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form524_RS485A";
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form524_RS485A_FormClosed);
			base.Load += new System.EventHandler(Form524_RS485A_Load);
			this.COMGB.ResumeLayout(false);
			this.COMGB.PerformLayout();
			this.P95GB.ResumeLayout(false);
			this.P95GB.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
