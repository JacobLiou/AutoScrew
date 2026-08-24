using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form593_DOTimerDelay : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private Image[] OffOnImg = new Image[2];

		private ushort Page_Axis = 0;

		private bool[] SW = new bool[8];

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private Button DO1Bn;

		private TextBox DO1TB;

		private Label lab_Station;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label7;

		private Button DO2Bn;

		private Button DO3Bn;

		private Button DO4Bn;

		private Button DO5Bn;

		private Button DO6Bn;

		private Button DO7Bn;

		private Button DO8Bn;

		private TextBox DO2TB;

		private TextBox DO3TB;

		private TextBox DO4TB;

		private TextBox DO5TB;

		private TextBox DO6TB;

		private TextBox DO7TB;

		private TextBox DO8TB;

		private Label label8;

		private Label label9;

		private Label label10;

		private Label label11;

		private Label label12;

		private Label label13;

		private Label label14;

		private Label label15;

		public event CreateForm593_Handler CreateID;

		public Form593_DOTimerDelay(GlobalVar GB, TCPclient TCP, ushort Axis)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
			OffOnImg[0] = Resources.OFF_ICON;
			OffOnImg[1] = Resources.ON_ICON;
			Page_Axis = Axis;
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(DO1TB, GB.UISys.RangeStr + "0-3000");
			toolTip.SetToolTip(DO2TB, GB.UISys.RangeStr + "0-3000");
			toolTip.SetToolTip(DO3TB, GB.UISys.RangeStr + "0-3000");
			toolTip.SetToolTip(DO4TB, GB.UISys.RangeStr + "0-3000");
			toolTip.SetToolTip(DO5TB, GB.UISys.RangeStr + "0-3000");
			toolTip.SetToolTip(DO6TB, GB.UISys.RangeStr + "0-3000");
			toolTip.SetToolTip(DO7TB, GB.UISys.RangeStr + "0-3000");
			toolTip.SetToolTip(DO8TB, GB.UISys.RangeStr + "0-3000");
			DO1TB.KeyPress += GB.RangeUnsigned3000;
			DO1TB.LostFocus += GB.LostFocus_C0;
			DO1TB.KeyUp += DO1Setting_KeyUp;
			DO2TB.KeyPress += GB.RangeUnsigned3000;
			DO2TB.LostFocus += GB.LostFocus_C0;
			DO2TB.KeyUp += DO2Setting_KeyUp;
			DO3TB.KeyPress += GB.RangeUnsigned3000;
			DO3TB.LostFocus += GB.LostFocus_C0;
			DO3TB.KeyUp += DO3Setting_KeyUp;
			DO4TB.KeyPress += GB.RangeUnsigned3000;
			DO4TB.LostFocus += GB.LostFocus_C0;
			DO4TB.KeyUp += DO4Setting_KeyUp;
			DO5TB.KeyPress += GB.RangeUnsigned3000;
			DO5TB.LostFocus += GB.LostFocus_C0;
			DO5TB.KeyUp += DO5Setting_KeyUp;
			DO6TB.KeyPress += GB.RangeUnsigned3000;
			DO6TB.LostFocus += GB.LostFocus_C0;
			DO6TB.KeyUp += DO6Setting_KeyUp;
			DO7TB.KeyPress += GB.RangeUnsigned3000;
			DO7TB.LostFocus += GB.LostFocus_C0;
			DO7TB.KeyUp += DO7Setting_KeyUp;
			DO8TB.KeyPress += GB.RangeUnsigned3000;
			DO8TB.LostFocus += GB.LostFocus_C0;
			DO8TB.KeyUp += DO8Setting_KeyUp;
			CtrlDOTimerStuc FSCtrlDITimer = default(CtrlDOTimerStuc);
			FSCtrlDITimer = ((Page_Axis == 0) ? GB.FSCtrlDOTimer_X : GB.FSCtrlDOTimer_Y);
			SW[0] = ((FSCtrlDITimer.DI1Timer > 0) ? true : false);
			SW[1] = ((FSCtrlDITimer.DI2Timer > 0) ? true : false);
			SW[2] = ((FSCtrlDITimer.DI3Timer > 0) ? true : false);
			SW[3] = ((FSCtrlDITimer.DI4Timer > 0) ? true : false);
			SW[4] = ((FSCtrlDITimer.DI5Timer > 0) ? true : false);
			SW[5] = ((FSCtrlDITimer.DI6Timer > 0) ? true : false);
			SW[6] = ((FSCtrlDITimer.DI7Timer > 0) ? true : false);
			SW[7] = ((FSCtrlDITimer.DI8Timer > 0) ? true : false);
			ShowOnOffBtn(SW[0], ref FSCtrlDITimer.DI1Timer, FSCtrlDITimer.DI1Timer.ToString(), DO1TB, DO1Bn, OffOnImg);
			ShowOnOffBtn(SW[1], ref FSCtrlDITimer.DI2Timer, FSCtrlDITimer.DI2Timer.ToString(), DO2TB, DO2Bn, OffOnImg);
			ShowOnOffBtn(SW[2], ref FSCtrlDITimer.DI3Timer, FSCtrlDITimer.DI3Timer.ToString(), DO3TB, DO3Bn, OffOnImg);
			ShowOnOffBtn(SW[3], ref FSCtrlDITimer.DI4Timer, FSCtrlDITimer.DI4Timer.ToString(), DO4TB, DO4Bn, OffOnImg);
			ShowOnOffBtn(SW[4], ref FSCtrlDITimer.DI5Timer, FSCtrlDITimer.DI5Timer.ToString(), DO5TB, DO5Bn, OffOnImg);
			ShowOnOffBtn(SW[5], ref FSCtrlDITimer.DI6Timer, FSCtrlDITimer.DI6Timer.ToString(), DO6TB, DO6Bn, OffOnImg);
			ShowOnOffBtn(SW[6], ref FSCtrlDITimer.DI7Timer, FSCtrlDITimer.DI7Timer.ToString(), DO7TB, DO7Bn, OffOnImg);
			ShowOnOffBtn(SW[7], ref FSCtrlDITimer.DI8Timer, FSCtrlDITimer.DI8Timer.ToString(), DO8TB, DO8Bn, OffOnImg);
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateID != null)
			{
				this.CreateID();
			}
		}

		private void ShowOnOffBtn(bool Sw, ref ushort val, string str, TextBox TB, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((!Sw) ? Img[0] : Img[1]);
			TB.Enabled = Sw;
			if (!Sw)
			{
				val = 0;
			}
			else if (str != "")
			{
				val = ushort.Parse(str);
			}
			TB.Text = val.ToString();
		}

		private void Form593_DOTimerDelay_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void Form593_DOTimerDelay_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void DO1Bn_Click(object sender, EventArgs e)
		{
			SW[0] = !SW[0];
			DOFunction(0u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO2Bn_Click(object sender, EventArgs e)
		{
			SW[1] = !SW[1];
			DOFunction(1u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO3Bn_Click(object sender, EventArgs e)
		{
			SW[2] = !SW[2];
			DOFunction(2u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO4Bn_Click(object sender, EventArgs e)
		{
			SW[3] = !SW[3];
			DOFunction(3u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO5Bn_Click(object sender, EventArgs e)
		{
			SW[4] = !SW[4];
			DOFunction(4u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO6Bn_Click(object sender, EventArgs e)
		{
			SW[5] = !SW[5];
			DOFunction(5u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO7Bn_Click(object sender, EventArgs e)
		{
			SW[6] = !SW[6];
			DOFunction(6u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO8Bn_Click(object sender, EventArgs e)
		{
			SW[7] = !SW[7];
			DOFunction(7u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DOFunction(uint GP)
		{
			if (Page_Axis == 0)
			{
				switch (GP)
				{
				case 0u:
					ShowOnOffBtn(SW[0], ref GB.FSCtrlDOTimer_X.DI1Timer, DO1TB.Text, DO1TB, DO1Bn, OffOnImg);
					break;
				case 1u:
					ShowOnOffBtn(SW[1], ref GB.FSCtrlDOTimer_X.DI2Timer, DO2TB.Text, DO2TB, DO2Bn, OffOnImg);
					break;
				case 2u:
					ShowOnOffBtn(SW[2], ref GB.FSCtrlDOTimer_X.DI3Timer, DO3TB.Text, DO3TB, DO3Bn, OffOnImg);
					break;
				case 3u:
					ShowOnOffBtn(SW[3], ref GB.FSCtrlDOTimer_X.DI4Timer, DO4TB.Text, DO4TB, DO4Bn, OffOnImg);
					break;
				case 4u:
					ShowOnOffBtn(SW[4], ref GB.FSCtrlDOTimer_X.DI5Timer, DO5TB.Text, DO5TB, DO5Bn, OffOnImg);
					break;
				case 5u:
					ShowOnOffBtn(SW[5], ref GB.FSCtrlDOTimer_X.DI6Timer, DO6TB.Text, DO6TB, DO6Bn, OffOnImg);
					break;
				case 6u:
					ShowOnOffBtn(SW[6], ref GB.FSCtrlDOTimer_X.DI7Timer, DO7TB.Text, DO7TB, DO7Bn, OffOnImg);
					break;
				case 7u:
					ShowOnOffBtn(SW[7], ref GB.FSCtrlDOTimer_X.DI8Timer, DO8TB.Text, DO8TB, DO8Bn, OffOnImg);
					break;
				}
			}
			else
			{
				switch (GP)
				{
				case 0u:
					ShowOnOffBtn(SW[0], ref GB.FSCtrlDOTimer_Y.DI1Timer, DO1TB.Text, DO1TB, DO1Bn, OffOnImg);
					break;
				case 1u:
					ShowOnOffBtn(SW[1], ref GB.FSCtrlDOTimer_Y.DI2Timer, DO2TB.Text, DO2TB, DO2Bn, OffOnImg);
					break;
				case 2u:
					ShowOnOffBtn(SW[2], ref GB.FSCtrlDOTimer_Y.DI3Timer, DO3TB.Text, DO3TB, DO3Bn, OffOnImg);
					break;
				case 3u:
					ShowOnOffBtn(SW[3], ref GB.FSCtrlDOTimer_Y.DI4Timer, DO4TB.Text, DO4TB, DO4Bn, OffOnImg);
					break;
				case 4u:
					ShowOnOffBtn(SW[4], ref GB.FSCtrlDOTimer_Y.DI5Timer, DO5TB.Text, DO5TB, DO5Bn, OffOnImg);
					break;
				case 5u:
					ShowOnOffBtn(SW[5], ref GB.FSCtrlDOTimer_Y.DI6Timer, DO6TB.Text, DO6TB, DO6Bn, OffOnImg);
					break;
				case 6u:
					ShowOnOffBtn(SW[6], ref GB.FSCtrlDOTimer_Y.DI7Timer, DO7TB.Text, DO7TB, DO7Bn, OffOnImg);
					break;
				case 7u:
					ShowOnOffBtn(SW[7], ref GB.FSCtrlDOTimer_Y.DI8Timer, DO8TB.Text, DO8TB, DO8Bn, OffOnImg);
					break;
				}
			}
		}

		private void DO1Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(0u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO2Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(1u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO3Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(2u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO4Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(3u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO5Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(4u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO6Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(5u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO7Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(6u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
		}

		private void DO8Setting_KeyUp(object sender, KeyEventArgs e)
		{
			DOFunction(7u);
			TCP.FSIDWrite_ByTCP(538, 0, Page_Axis, 0, 0, 0);
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
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.DO1Bn = new System.Windows.Forms.Button();
			this.DO1TB = new System.Windows.Forms.TextBox();
			this.lab_Station = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.DO2Bn = new System.Windows.Forms.Button();
			this.DO3Bn = new System.Windows.Forms.Button();
			this.DO4Bn = new System.Windows.Forms.Button();
			this.DO5Bn = new System.Windows.Forms.Button();
			this.DO6Bn = new System.Windows.Forms.Button();
			this.DO7Bn = new System.Windows.Forms.Button();
			this.DO8Bn = new System.Windows.Forms.Button();
			this.DO2TB = new System.Windows.Forms.TextBox();
			this.DO3TB = new System.Windows.Forms.TextBox();
			this.DO4TB = new System.Windows.Forms.TextBox();
			this.DO5TB = new System.Windows.Forms.TextBox();
			this.DO6TB = new System.Windows.Forms.TextBox();
			this.DO7TB = new System.Windows.Forms.TextBox();
			this.DO8TB = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, -2);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(500, 35);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.Text = "Title";
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(469, 1);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.DO1Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO1Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO1Bn.FlatAppearance.BorderSize = 0;
			this.DO1Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO1Bn.Location = new System.Drawing.Point(126, 102);
			this.DO1Bn.Name = "DO1Bn";
			this.DO1Bn.Size = new System.Drawing.Size(60, 25);
			this.DO1Bn.TabIndex = 129;
			this.DO1Bn.UseVisualStyleBackColor = true;
			this.DO1Bn.Click += new System.EventHandler(DO1Bn_Click);
			this.DO1TB.Location = new System.Drawing.Point(223, 102);
			this.DO1TB.Name = "DO1TB";
			this.DO1TB.Size = new System.Drawing.Size(152, 25);
			this.DO1TB.TabIndex = 131;
			this.DO1TB.Text = "0";
			this.DO1TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Station.AutoSize = true;
			this.lab_Station.Location = new System.Drawing.Point(56, 107);
			this.lab_Station.Name = "lab_Station";
			this.lab_Station.Size = new System.Drawing.Size(34, 15);
			this.lab_Station.TabIndex = 130;
			this.lab_Station.Text = "DO1";
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(56, 148);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 15);
			this.label1.TabIndex = 130;
			this.label1.Text = "DO2";
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(56, 189);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 15);
			this.label2.TabIndex = 130;
			this.label2.Text = "DO3";
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(56, 230);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(34, 15);
			this.label3.TabIndex = 130;
			this.label3.Text = "DO4";
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(56, 271);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 15);
			this.label4.TabIndex = 130;
			this.label4.Text = "DO5";
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(56, 312);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(34, 15);
			this.label5.TabIndex = 130;
			this.label5.Text = "DO6";
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(56, 353);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(34, 15);
			this.label6.TabIndex = 130;
			this.label6.Text = "DO7";
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(56, 394);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(34, 15);
			this.label7.TabIndex = 130;
			this.label7.Text = "DO8";
			this.DO2Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO2Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO2Bn.FlatAppearance.BorderSize = 0;
			this.DO2Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO2Bn.Location = new System.Drawing.Point(126, 143);
			this.DO2Bn.Name = "DO2Bn";
			this.DO2Bn.Size = new System.Drawing.Size(60, 25);
			this.DO2Bn.TabIndex = 129;
			this.DO2Bn.UseVisualStyleBackColor = true;
			this.DO2Bn.Click += new System.EventHandler(DO2Bn_Click);
			this.DO3Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO3Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO3Bn.FlatAppearance.BorderSize = 0;
			this.DO3Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO3Bn.Location = new System.Drawing.Point(126, 184);
			this.DO3Bn.Name = "DO3Bn";
			this.DO3Bn.Size = new System.Drawing.Size(60, 25);
			this.DO3Bn.TabIndex = 129;
			this.DO3Bn.UseVisualStyleBackColor = true;
			this.DO3Bn.Click += new System.EventHandler(DO3Bn_Click);
			this.DO4Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO4Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO4Bn.FlatAppearance.BorderSize = 0;
			this.DO4Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO4Bn.Location = new System.Drawing.Point(126, 225);
			this.DO4Bn.Name = "DO4Bn";
			this.DO4Bn.Size = new System.Drawing.Size(60, 25);
			this.DO4Bn.TabIndex = 129;
			this.DO4Bn.UseVisualStyleBackColor = true;
			this.DO4Bn.Click += new System.EventHandler(DO4Bn_Click);
			this.DO5Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO5Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO5Bn.FlatAppearance.BorderSize = 0;
			this.DO5Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO5Bn.Location = new System.Drawing.Point(126, 266);
			this.DO5Bn.Name = "DO5Bn";
			this.DO5Bn.Size = new System.Drawing.Size(60, 25);
			this.DO5Bn.TabIndex = 129;
			this.DO5Bn.UseVisualStyleBackColor = true;
			this.DO5Bn.Click += new System.EventHandler(DO5Bn_Click);
			this.DO6Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO6Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO6Bn.FlatAppearance.BorderSize = 0;
			this.DO6Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO6Bn.Location = new System.Drawing.Point(126, 307);
			this.DO6Bn.Name = "DO6Bn";
			this.DO6Bn.Size = new System.Drawing.Size(60, 25);
			this.DO6Bn.TabIndex = 129;
			this.DO6Bn.UseVisualStyleBackColor = true;
			this.DO6Bn.Click += new System.EventHandler(DO6Bn_Click);
			this.DO7Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO7Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO7Bn.FlatAppearance.BorderSize = 0;
			this.DO7Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO7Bn.Location = new System.Drawing.Point(126, 348);
			this.DO7Bn.Name = "DO7Bn";
			this.DO7Bn.Size = new System.Drawing.Size(60, 25);
			this.DO7Bn.TabIndex = 129;
			this.DO7Bn.UseVisualStyleBackColor = true;
			this.DO7Bn.Click += new System.EventHandler(DO7Bn_Click);
			this.DO8Bn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.DO8Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DO8Bn.FlatAppearance.BorderSize = 0;
			this.DO8Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DO8Bn.Location = new System.Drawing.Point(126, 389);
			this.DO8Bn.Name = "DO8Bn";
			this.DO8Bn.Size = new System.Drawing.Size(60, 25);
			this.DO8Bn.TabIndex = 129;
			this.DO8Bn.UseVisualStyleBackColor = true;
			this.DO8Bn.Click += new System.EventHandler(DO8Bn_Click);
			this.DO2TB.Location = new System.Drawing.Point(223, 143);
			this.DO2TB.Name = "DO2TB";
			this.DO2TB.Size = new System.Drawing.Size(152, 25);
			this.DO2TB.TabIndex = 131;
			this.DO2TB.Text = "0";
			this.DO2TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DO3TB.Location = new System.Drawing.Point(223, 184);
			this.DO3TB.Name = "DO3TB";
			this.DO3TB.Size = new System.Drawing.Size(152, 25);
			this.DO3TB.TabIndex = 131;
			this.DO3TB.Text = "0";
			this.DO3TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DO4TB.Location = new System.Drawing.Point(223, 225);
			this.DO4TB.Name = "DO4TB";
			this.DO4TB.Size = new System.Drawing.Size(152, 25);
			this.DO4TB.TabIndex = 131;
			this.DO4TB.Text = "0";
			this.DO4TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DO5TB.Location = new System.Drawing.Point(223, 266);
			this.DO5TB.Name = "DO5TB";
			this.DO5TB.Size = new System.Drawing.Size(152, 25);
			this.DO5TB.TabIndex = 131;
			this.DO5TB.Text = "0";
			this.DO5TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DO6TB.Location = new System.Drawing.Point(223, 307);
			this.DO6TB.Name = "DO6TB";
			this.DO6TB.Size = new System.Drawing.Size(152, 25);
			this.DO6TB.TabIndex = 131;
			this.DO6TB.Text = "0";
			this.DO6TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DO7TB.Location = new System.Drawing.Point(223, 348);
			this.DO7TB.Name = "DO7TB";
			this.DO7TB.Size = new System.Drawing.Size(152, 25);
			this.DO7TB.TabIndex = 131;
			this.DO7TB.Text = "0";
			this.DO7TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.DO8TB.Location = new System.Drawing.Point(223, 389);
			this.DO8TB.Name = "DO8TB";
			this.DO8TB.Size = new System.Drawing.Size(152, 25);
			this.DO8TB.TabIndex = 131;
			this.DO8TB.Text = "0";
			this.DO8TB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(408, 107);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(23, 15);
			this.label8.TabIndex = 130;
			this.label8.Text = "ms";
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(408, 148);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(23, 15);
			this.label9.TabIndex = 130;
			this.label9.Text = "ms";
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(408, 189);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(23, 15);
			this.label10.TabIndex = 130;
			this.label10.Text = "ms";
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(408, 230);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(23, 15);
			this.label11.TabIndex = 130;
			this.label11.Text = "ms";
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(408, 271);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(23, 15);
			this.label12.TabIndex = 130;
			this.label12.Text = "ms";
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(408, 312);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(23, 15);
			this.label13.TabIndex = 130;
			this.label13.Text = "ms";
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(408, 353);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(23, 15);
			this.label14.TabIndex = 130;
			this.label14.Text = "ms";
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(408, 394);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(23, 15);
			this.label15.TabIndex = 130;
			this.label15.Text = "ms";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 522);
			base.Controls.Add(this.label7);
			base.Controls.Add(this.label6);
			base.Controls.Add(this.label5);
			base.Controls.Add(this.label4);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label15);
			base.Controls.Add(this.label14);
			base.Controls.Add(this.label13);
			base.Controls.Add(this.label12);
			base.Controls.Add(this.label11);
			base.Controls.Add(this.label10);
			base.Controls.Add(this.label9);
			base.Controls.Add(this.label8);
			base.Controls.Add(this.lab_Station);
			base.Controls.Add(this.DO8TB);
			base.Controls.Add(this.DO7TB);
			base.Controls.Add(this.DO6TB);
			base.Controls.Add(this.DO5TB);
			base.Controls.Add(this.DO4TB);
			base.Controls.Add(this.DO3TB);
			base.Controls.Add(this.DO2TB);
			base.Controls.Add(this.DO1TB);
			base.Controls.Add(this.DO8Bn);
			base.Controls.Add(this.DO7Bn);
			base.Controls.Add(this.DO6Bn);
			base.Controls.Add(this.DO5Bn);
			base.Controls.Add(this.DO4Bn);
			base.Controls.Add(this.DO3Bn);
			base.Controls.Add(this.DO2Bn);
			base.Controls.Add(this.DO1Bn);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form593_DOTimerDelay";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form593_DOTimerDelay_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form593_DOTimerDelay_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
