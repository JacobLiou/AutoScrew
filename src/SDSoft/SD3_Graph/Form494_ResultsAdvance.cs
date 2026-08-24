using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form494_ResultsAdvance : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private int Axis = 0;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private TextBox StartSave;

		private TextBox EndSave;

		private TextBox StartMatched;

		private TextBox EndMatched;

		private Label lab_Save;

		private Label lab_Matched;

		private Button btn_Cancel;

		private Button btn_OK;

		private Label lab_1;

		private Label lab_2;

		private Label lab_TarArm;

		private Label lab_FedArm;

		private Label lab_Gryo;

		private TextBox TarArmXTB;

		private TextBox TarArmYTB;

		private TextBox TarArmZTB;

		private TextBox FedArmXTB;

		private TextBox FedArmYTB;

		private TextBox FedArmZTB;

		private TextBox GryoValTB;

		private TextBox MaxRotaTB;

		private Label lab_MaxRota;

		public Form494_ResultsAdvance(int Axis, GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.Axis = Axis;
			MultiLanguage.LoadLanguage(this);
			StartSave.KeyPress += GB.RangeUnsigned200to1;
			StartSave.LostFocus += GB.LostFocus_C0;
			EndSave.KeyPress += GB.RangeUnsigned200to1;
			EndSave.LostFocus += GB.LostFocus_C0;
			StartMatched.KeyPress += GB.RangeUnsigned200to1;
			StartMatched.LostFocus += GB.LostFocus_C0;
			EndMatched.KeyPress += GB.RangeUnsigned200to1;
			EndMatched.LostFocus += GB.LostFocus_C0;
			UpdateUI(false);
			UpdateUI(true);
		}

		public unsafe void UpdateUI(bool Online)
		{
			if (!Online)
			{
				if (Axis == 0)
				{
					StartSave.Text = GB.FSResultBarcodeAdvanceSettingX.SaveStartChar.ToString();
					EndSave.Text = GB.FSResultBarcodeAdvanceSettingX.SaveEndChar.ToString();
					StartMatched.Text = GB.FSResultBarcodeAdvanceSettingX.MatchStartChar.ToString();
					EndMatched.Text = GB.FSResultBarcodeAdvanceSettingX.MatchEndChar.ToString();
					lab_Matched.Visible = GB.UISys.RunningSrcMode.SwitchingMethodX == 2;
					StartMatched.Visible = GB.UISys.RunningSrcMode.SwitchingMethodX == 2;
					lab_2.Visible = GB.UISys.RunningSrcMode.SwitchingMethodX == 2;
					EndMatched.Visible = GB.UISys.RunningSrcMode.SwitchingMethodX == 2;
				}
				else
				{
					StartSave.Text = GB.FSResultBarcodeAdvanceSettingY.SaveStartChar.ToString();
					EndSave.Text = GB.FSResultBarcodeAdvanceSettingY.SaveEndChar.ToString();
					StartMatched.Text = GB.FSResultBarcodeAdvanceSettingY.MatchStartChar.ToString();
					EndMatched.Text = GB.FSResultBarcodeAdvanceSettingY.MatchEndChar.ToString();
					lab_Matched.Visible = GB.UISys.RunningSrcMode.SwitchingMethodY == 2;
					StartMatched.Visible = GB.UISys.RunningSrcMode.SwitchingMethodY == 2;
					lab_2.Visible = GB.UISys.RunningSrcMode.SwitchingMethodY == 2;
					EndMatched.Visible = GB.UISys.RunningSrcMode.SwitchingMethodY == 2;
				}
				return;
			}
			bool flag;
			bool flag3;
			bool flag5;
			bool flag7;
			bool flag9;
			bool flag11;
			bool visible;
			if (Axis == 0)
			{
				TarArmXTB.Text = ((float)(GB.TcpStatus.Detail.T1StB.Target_PositioningArmX_H_39 * 65536 + GB.TcpStatus.Detail.T1StB.Target_PositioningArmX_L_38) / 100f).ToString();
				TarArmYTB.Text = ((float)(GB.TcpStatus.Detail.T1StB.Target_PositioningArmY_H_41 * 65536 + GB.TcpStatus.Detail.T1StB.Target_PositioningArmY_L_40) / 100f).ToString();
				TarArmZTB.Text = ((float)(GB.TcpStatus.Detail.T1StB.Target_PositioningArmZ_H_43 * 65536 + GB.TcpStatus.Detail.T1StB.Target_PositioningArmZ_L_42) / 100f).ToString();
				FedArmXTB.Text = ((float)(GB.TcpStatus.Detail.T1StB.Fed_PositioningArmX_H_33 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmX_L_32) / 100f).ToString();
				FedArmYTB.Text = ((float)(GB.TcpStatus.Detail.T1StB.Fed_PositioningArmY_H_35 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmY_L_34) / 100f).ToString();
				FedArmZTB.Text = ((float)(GB.TcpStatus.Detail.T1StB.Fed_PositioningArmZ_H_37 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmZ_L_36) / 100f).ToString();
				Label label = lab_TarArm;
				TextBox tarArmXTB = TarArmXTB;
				TextBox tarArmYTB = TarArmYTB;
				TextBox tarArmZTB = TarArmZTB;
				Label label2 = lab_FedArm;
				TextBox fedArmXTB = FedArmXTB;
				TextBox fedArmYTB = FedArmYTB;
				flag = (FedArmZTB.Visible = GB.UISys.RunningSeqX.ArmPostioningMode == 1);
				flag3 = (fedArmYTB.Visible = flag);
				flag5 = (fedArmXTB.Visible = flag3);
				flag7 = (label2.Visible = flag5);
				flag9 = (tarArmZTB.Visible = flag7);
				flag11 = (tarArmYTB.Visible = flag9);
				visible = (tarArmXTB.Visible = flag11);
				label.Visible = visible;
				if (GB.UISys.RunningParamX.Comm.GyroAllowError_45 > 0 && GB.CheckHMIVer(172, 0))
				{
					Label label3 = lab_Gryo;
					visible = (GryoValTB.Visible = true);
					label3.Visible = visible;
					TCP.FSIDRead_ByTCP(94, 0, 0, 0, 0, 0);
					GryoValTB.Text = GB.FSResultPerpendicularityX.Value.ToString();
				}
				else
				{
					Label label4 = lab_Gryo;
					visible = (GryoValTB.Visible = false);
					label4.Visible = visible;
					GryoValTB.Text = "0";
				}
				if ((GB.UISys.RunningParamX.Comm.MultiAdvance_49 & 1) > 0 && GB.CheckHMIVer(172, 5))
				{
					Label label5 = lab_MaxRota;
					visible = (MaxRotaTB.Visible = true);
					label5.Visible = visible;
					if (GB.FSModelTypeInfo.MesModelType == 1)
					{
						TCP.FSIDRead_ByTCP(53, 0, 1, 7863, 0, 1);
					}
					else
					{
						TCP.FSIDRead_ByTCP(53, 0, 1, 7763, 0, 1);
					}
					MaxRotaTB.Text = ((short)GB.FSCtrlLocalTable.Data16[0]).ToString();
				}
				else
				{
					Label label6 = lab_MaxRota;
					visible = (MaxRotaTB.Visible = false);
					label6.Visible = visible;
					MaxRotaTB.Text = "0";
				}
				return;
			}
			TarArmXTB.Text = ((float)(GB.TcpStatus.Detail.T2StB.Target_PositioningArmX_H_39 * 65536 + GB.TcpStatus.Detail.T2StB.Target_PositioningArmX_L_38) / 100f).ToString();
			TarArmYTB.Text = ((float)(GB.TcpStatus.Detail.T2StB.Target_PositioningArmY_H_41 * 65536 + GB.TcpStatus.Detail.T2StB.Target_PositioningArmY_L_40) / 100f).ToString();
			TarArmZTB.Text = ((float)(GB.TcpStatus.Detail.T2StB.Target_PositioningArmZ_H_43 * 65536 + GB.TcpStatus.Detail.T2StB.Target_PositioningArmZ_L_42) / 100f).ToString();
			FedArmXTB.Text = ((float)(GB.TcpStatus.Detail.T2StB.Fed_PositioningArmX_H_33 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmX_L_32) / 100f).ToString();
			FedArmYTB.Text = ((float)(GB.TcpStatus.Detail.T2StB.Fed_PositioningArmY_H_35 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmY_L_34) / 100f).ToString();
			FedArmZTB.Text = ((float)(GB.TcpStatus.Detail.T2StB.Fed_PositioningArmZ_H_37 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmZ_L_36) / 100f).ToString();
			Label label7 = lab_TarArm;
			TextBox tarArmXTB2 = TarArmXTB;
			TextBox tarArmYTB2 = TarArmYTB;
			TextBox tarArmZTB2 = TarArmZTB;
			Label label8 = lab_FedArm;
			TextBox fedArmXTB2 = FedArmXTB;
			TextBox fedArmYTB2 = FedArmYTB;
			flag = (FedArmZTB.Visible = GB.UISys.RunningSeqY.ArmPostioningMode == 1);
			flag3 = (fedArmYTB2.Visible = flag);
			flag5 = (fedArmXTB2.Visible = flag3);
			flag7 = (label8.Visible = flag5);
			flag9 = (tarArmZTB2.Visible = flag7);
			flag11 = (tarArmYTB2.Visible = flag9);
			visible = (tarArmXTB2.Visible = flag11);
			label7.Visible = visible;
			if (GB.UISys.RunningParamY.Comm.GyroAllowError_45 > 0 && GB.CheckHMIVer(172, 0))
			{
				Label label9 = lab_Gryo;
				visible = (GryoValTB.Visible = true);
				label9.Visible = visible;
				TCP.FSIDRead_ByTCP(94, 0, 1, 0, 0, 0);
				GryoValTB.Text = GB.FSResultPerpendicularityY.Value.ToString();
			}
			else
			{
				Label label10 = lab_Gryo;
				visible = (GryoValTB.Visible = false);
				label10.Visible = visible;
				GryoValTB.Text = "0";
			}
			if ((GB.UISys.RunningParamY.Comm.MultiAdvance_49 & 1) > 0 && GB.CheckHMIVer(172, 5))
			{
				Label label11 = lab_MaxRota;
				visible = (MaxRotaTB.Visible = true);
				label11.Visible = visible;
				if (GB.FSModelTypeInfo.MesModelType == 1)
				{
					TCP.FSIDRead_ByTCP(53, 0, 1, 7763, 0, 1);
				}
				else
				{
					TCP.FSIDRead_ByTCP(53, 0, 1, 7863, 0, 1);
				}
				MaxRotaTB.Text = ((short)GB.FSCtrlLocalTable.Data16[0]).ToString();
			}
			else
			{
				Label label12 = lab_MaxRota;
				visible = (MaxRotaTB.Visible = false);
				label12.Visible = visible;
				MaxRotaTB.Text = "0";
			}
		}

		private void Form494_ResultAdvance_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (Axis == 0)
			{
				GB.FSResultBarcodeAdvanceSettingX.SaveStartChar = ushort.Parse(StartSave.Text);
				GB.FSResultBarcodeAdvanceSettingX.SaveEndChar = ushort.Parse(EndSave.Text);
				GB.FSResultBarcodeAdvanceSettingX.MatchStartChar = ushort.Parse(StartMatched.Text);
				GB.FSResultBarcodeAdvanceSettingX.MatchEndChar = ushort.Parse(EndMatched.Text);
			}
			else
			{
				GB.FSResultBarcodeAdvanceSettingY.SaveStartChar = ushort.Parse(StartSave.Text);
				GB.FSResultBarcodeAdvanceSettingY.SaveEndChar = ushort.Parse(EndSave.Text);
				GB.FSResultBarcodeAdvanceSettingY.MatchStartChar = ushort.Parse(StartMatched.Text);
				GB.FSResultBarcodeAdvanceSettingY.MatchEndChar = ushort.Parse(EndMatched.Text);
			}
			TCP.FSIDWrite_ByTCP(408, 0, (ushort)Axis, 0, 0, 0);
			Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form494_ResultsAdvance_Load(object sender, EventArgs e)
		{
			GB.GetPositionArmTimer = new Timer();
			GB.GetPositionArmTimer.Interval = 800;
			GB.GetPositionArmTimer.Tick += Timer_Tick;
			GB.GetPositionArmTimer.Start();
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			UpdateUI(true);
		}

		private void Form494_ResultsAdvance_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (GB.GetPositionArmTimer != null)
			{
				GB.GetPositionArmTimer.Stop();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form494_ResultsAdvance));
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.StartSave = new System.Windows.Forms.TextBox();
			this.EndSave = new System.Windows.Forms.TextBox();
			this.StartMatched = new System.Windows.Forms.TextBox();
			this.EndMatched = new System.Windows.Forms.TextBox();
			this.lab_Save = new System.Windows.Forms.Label();
			this.lab_Matched = new System.Windows.Forms.Label();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_OK = new System.Windows.Forms.Button();
			this.lab_1 = new System.Windows.Forms.Label();
			this.lab_2 = new System.Windows.Forms.Label();
			this.lab_TarArm = new System.Windows.Forms.Label();
			this.lab_FedArm = new System.Windows.Forms.Label();
			this.lab_Gryo = new System.Windows.Forms.Label();
			this.TarArmXTB = new System.Windows.Forms.TextBox();
			this.TarArmYTB = new System.Windows.Forms.TextBox();
			this.TarArmZTB = new System.Windows.Forms.TextBox();
			this.FedArmXTB = new System.Windows.Forms.TextBox();
			this.FedArmYTB = new System.Windows.Forms.TextBox();
			this.FedArmZTB = new System.Windows.Forms.TextBox();
			this.GryoValTB = new System.Windows.Forms.TextBox();
			this.MaxRotaTB = new System.Windows.Forms.TextBox();
			this.lab_MaxRota = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(650, 50);
			this.lab_HanderTitle.TabIndex = 56;
			this.lab_HanderTitle.Text = "Scanner Advanced Settings";
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.StartSave.Font = new System.Drawing.Font("新細明體", 10f);
			this.StartSave.Location = new System.Drawing.Point(481, 191);
			this.StartSave.Name = "StartSave";
			this.StartSave.Size = new System.Drawing.Size(60, 27);
			this.StartSave.TabIndex = 154;
			this.StartSave.Text = "1";
			this.StartSave.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EndSave.Font = new System.Drawing.Font("新細明體", 10f);
			this.EndSave.Location = new System.Drawing.Point(569, 191);
			this.EndSave.Name = "EndSave";
			this.EndSave.Size = new System.Drawing.Size(60, 27);
			this.EndSave.TabIndex = 154;
			this.EndSave.Text = "200";
			this.EndSave.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.StartMatched.Font = new System.Drawing.Font("新細明體", 10f);
			this.StartMatched.Location = new System.Drawing.Point(481, 228);
			this.StartMatched.Name = "StartMatched";
			this.StartMatched.Size = new System.Drawing.Size(60, 27);
			this.StartMatched.TabIndex = 154;
			this.StartMatched.Text = "1";
			this.StartMatched.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EndMatched.Font = new System.Drawing.Font("新細明體", 10f);
			this.EndMatched.Location = new System.Drawing.Point(569, 228);
			this.EndMatched.Name = "EndMatched";
			this.EndMatched.Size = new System.Drawing.Size(60, 27);
			this.EndMatched.TabIndex = 154;
			this.EndMatched.Text = "200";
			this.EndMatched.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Save.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Save.Location = new System.Drawing.Point(12, 194);
			this.lab_Save.Name = "lab_Save";
			this.lab_Save.Size = new System.Drawing.Size(463, 28);
			this.lab_Save.TabIndex = 155;
			this.lab_Save.Text = "The Scanner String Position (char.) to be Saved";
			this.lab_Save.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_Matched.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Matched.Location = new System.Drawing.Point(12, 231);
			this.lab_Matched.Name = "lab_Matched";
			this.lab_Matched.Size = new System.Drawing.Size(465, 24);
			this.lab_Matched.TabIndex = 155;
			this.lab_Matched.Text = "The Scanner String Position (char.) to be Matched";
			this.lab_Matched.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btn_Cancel.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Cancel.BackgroundImage");
			this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Cancel.FlatAppearance.BorderSize = 0;
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Cancel.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Cancel.Location = new System.Drawing.Point(376, 282);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(92, 30);
			this.btn_Cancel.TabIndex = 157;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(189, 282);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 156;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_OK_Click);
			this.lab_1.AutoSize = true;
			this.lab_1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_1.Location = new System.Drawing.Point(547, 191);
			this.lab_1.Name = "lab_1";
			this.lab_1.Size = new System.Drawing.Size(19, 20);
			this.lab_1.TabIndex = 155;
			this.lab_1.Text = "~";
			this.lab_2.AutoSize = true;
			this.lab_2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_2.Location = new System.Drawing.Point(547, 228);
			this.lab_2.Name = "lab_2";
			this.lab_2.Size = new System.Drawing.Size(19, 20);
			this.lab_2.TabIndex = 155;
			this.lab_2.Text = "~";
			this.lab_TarArm.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_TarArm.Location = new System.Drawing.Point(12, 53);
			this.lab_TarArm.Name = "lab_TarArm";
			this.lab_TarArm.Size = new System.Drawing.Size(417, 29);
			this.lab_TarArm.TabIndex = 155;
			this.lab_TarArm.Text = "Positioning arm target coordinates";
			this.lab_TarArm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_FedArm.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_FedArm.Location = new System.Drawing.Point(12, 87);
			this.lab_FedArm.Name = "lab_FedArm";
			this.lab_FedArm.Size = new System.Drawing.Size(417, 29);
			this.lab_FedArm.TabIndex = 155;
			this.lab_FedArm.Text = "Positioning arm current coordinates";
			this.lab_FedArm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_Gryo.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Gryo.Location = new System.Drawing.Point(12, 123);
			this.lab_Gryo.Name = "lab_Gryo";
			this.lab_Gryo.Size = new System.Drawing.Size(442, 29);
			this.lab_Gryo.TabIndex = 155;
			this.lab_Gryo.Text = "Tool Perpendicularity Detection";
			this.lab_Gryo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.TarArmXTB.Enabled = false;
			this.TarArmXTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TarArmXTB.Location = new System.Drawing.Point(437, 56);
			this.TarArmXTB.Name = "TarArmXTB";
			this.TarArmXTB.Size = new System.Drawing.Size(60, 27);
			this.TarArmXTB.TabIndex = 154;
			this.TarArmXTB.Text = "0";
			this.TarArmXTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TarArmYTB.Enabled = false;
			this.TarArmYTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TarArmYTB.Location = new System.Drawing.Point(503, 56);
			this.TarArmYTB.Name = "TarArmYTB";
			this.TarArmYTB.Size = new System.Drawing.Size(60, 27);
			this.TarArmYTB.TabIndex = 154;
			this.TarArmYTB.Text = "0";
			this.TarArmYTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TarArmZTB.Enabled = false;
			this.TarArmZTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.TarArmZTB.Location = new System.Drawing.Point(569, 56);
			this.TarArmZTB.Name = "TarArmZTB";
			this.TarArmZTB.Size = new System.Drawing.Size(60, 27);
			this.TarArmZTB.TabIndex = 154;
			this.TarArmZTB.Text = "0";
			this.TarArmZTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FedArmXTB.Enabled = false;
			this.FedArmXTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.FedArmXTB.Location = new System.Drawing.Point(437, 88);
			this.FedArmXTB.Name = "FedArmXTB";
			this.FedArmXTB.Size = new System.Drawing.Size(60, 27);
			this.FedArmXTB.TabIndex = 154;
			this.FedArmXTB.Text = "0";
			this.FedArmXTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FedArmYTB.Enabled = false;
			this.FedArmYTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.FedArmYTB.Location = new System.Drawing.Point(503, 88);
			this.FedArmYTB.Name = "FedArmYTB";
			this.FedArmYTB.Size = new System.Drawing.Size(60, 27);
			this.FedArmYTB.TabIndex = 154;
			this.FedArmYTB.Text = "0";
			this.FedArmYTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.FedArmZTB.Enabled = false;
			this.FedArmZTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.FedArmZTB.Location = new System.Drawing.Point(569, 88);
			this.FedArmZTB.Name = "FedArmZTB";
			this.FedArmZTB.Size = new System.Drawing.Size(60, 27);
			this.FedArmZTB.TabIndex = 154;
			this.FedArmZTB.Text = "0";
			this.FedArmZTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.GryoValTB.Enabled = false;
			this.GryoValTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.GryoValTB.Location = new System.Drawing.Point(569, 121);
			this.GryoValTB.Name = "GryoValTB";
			this.GryoValTB.Size = new System.Drawing.Size(60, 27);
			this.GryoValTB.TabIndex = 154;
			this.GryoValTB.Text = "0";
			this.GryoValTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.MaxRotaTB.Enabled = false;
			this.MaxRotaTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.MaxRotaTB.Location = new System.Drawing.Point(569, 156);
			this.MaxRotaTB.Name = "MaxRotaTB";
			this.MaxRotaTB.Size = new System.Drawing.Size(60, 27);
			this.MaxRotaTB.TabIndex = 154;
			this.MaxRotaTB.Text = "0";
			this.MaxRotaTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_MaxRota.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_MaxRota.Location = new System.Drawing.Point(12, 158);
			this.lab_MaxRota.Name = "lab_MaxRota";
			this.lab_MaxRota.Size = new System.Drawing.Size(442, 29);
			this.lab_MaxRota.TabIndex = 155;
			this.lab_MaxRota.Text = "Max. Angle for Tool Rotation Detection";
			this.lab_MaxRota.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(650, 350);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lab_Matched);
			base.Controls.Add(this.lab_2);
			base.Controls.Add(this.lab_1);
			base.Controls.Add(this.lab_TarArm);
			base.Controls.Add(this.lab_MaxRota);
			base.Controls.Add(this.lab_Gryo);
			base.Controls.Add(this.lab_FedArm);
			base.Controls.Add(this.lab_Save);
			base.Controls.Add(this.EndMatched);
			base.Controls.Add(this.StartMatched);
			base.Controls.Add(this.EndSave);
			base.Controls.Add(this.FedArmZTB);
			base.Controls.Add(this.MaxRotaTB);
			base.Controls.Add(this.GryoValTB);
			base.Controls.Add(this.FedArmYTB);
			base.Controls.Add(this.TarArmZTB);
			base.Controls.Add(this.FedArmXTB);
			base.Controls.Add(this.TarArmYTB);
			base.Controls.Add(this.TarArmXTB);
			base.Controls.Add(this.StartSave);
			base.Controls.Add(this.lab_HanderTitle);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form494_ResultsAdvance";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form494_ResultsAdvance_FormClosed);
			base.Load += new System.EventHandler(Form494_ResultsAdvance_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form494_ResultAdvance_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
