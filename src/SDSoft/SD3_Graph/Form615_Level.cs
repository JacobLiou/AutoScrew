using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form615_Level : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private int Mode = 0;

		private int Axis = 0;

		private bool isONLevelMouseDown = false;

		private bool isOFFLevelMouseDown = false;

		private IContainer components = null;

		private TrackBar OnTrB;

		private TrackBar OffTrB;

		private Label CloseBn;

		private Label lab_Title;

		private Label lab_On;

		private Label lab_Off;

		private TrackBar CurrTrB;

		private TextBox CurrTB;

		private TextBox OnTB;

		private TextBox OffTB;

		public Form615_Level(GlobalVar GB, TCPclient TCP, int Axis, int Mode)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.Axis = Axis;
			this.Mode = Mode;
			MultiLanguage.LoadLanguage(this);
			CurrTrB.Maximum = 4096;
			CurrTrB.Minimum = 0;
			OffTrB.Maximum = 4096;
			OffTrB.Minimum = 0;
			OnTrB.Maximum = 4096;
			OnTrB.Minimum = 0;
		}

		private void Form615_Level_Load(object sender, EventArgs e)
		{
			if (Mode == 0)
			{
				TCP.FSIDRead_ByTCP(651, 0, (ushort)Axis, 0, 0, 0);
			}
			else
			{
				TCP.FSIDRead_ByTCP(652, 0, (ushort)Axis, 0, 0, 0);
			}
			UpdateUI();
			GB.GetLevelTimer = new Timer();
			GB.GetLevelTimer.Interval = 1200;
			GB.GetLevelTimer.Tick += Timer_Tick;
			GB.GetLevelTimer.Start();
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void UpdateUI()
		{
			if (Mode == 0)
			{
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_Title1");
				if (Axis == 0)
				{
					CurrTrB.Value = GB.FSToolXLeverStartLevel.CurrentLevel;
					CurrTB.Text = GB.FSToolXLeverStartLevel.CurrentLevel.ToString();
					if (!isONLevelMouseDown)
					{
						OnTrB.Value = GB.FSToolXLeverStartLevel.OnLevel;
						OnTB.Text = GB.FSToolXLeverStartLevel.OnLevel.ToString();
					}
					if (!isOFFLevelMouseDown)
					{
						OffTrB.Value = GB.FSToolXLeverStartLevel.OffLevel;
						OffTB.Text = GB.FSToolXLeverStartLevel.OffLevel.ToString();
					}
				}
				else
				{
					CurrTrB.Value = GB.FSToolYLeverStartLevel.CurrentLevel;
					CurrTB.Text = GB.FSToolYLeverStartLevel.CurrentLevel.ToString();
					if (!isONLevelMouseDown)
					{
						OnTrB.Value = GB.FSToolYLeverStartLevel.OnLevel;
						OnTB.Text = GB.FSToolYLeverStartLevel.OnLevel.ToString();
					}
					if (!isOFFLevelMouseDown)
					{
						OffTrB.Value = GB.FSToolYLeverStartLevel.OffLevel;
						OffTB.Text = GB.FSToolYLeverStartLevel.OffLevel.ToString();
					}
				}
				return;
			}
			lab_Title.Text = MultiLanguage.GetStr(this, "tp_Title2");
			if (Axis == 0)
			{
				CurrTrB.Value = GB.FSToolXPushStartLevel.CurrentLevel;
				CurrTB.Text = GB.FSToolXPushStartLevel.CurrentLevel.ToString();
				if (!isONLevelMouseDown)
				{
					OnTrB.Value = GB.FSToolXPushStartLevel.OnLevel;
					if (OnTB.InvokeRequired)
					{
						OnTB.Invoke((Action)delegate
						{
							OnTB.Text = OnTrB.Value.ToString();
						});
					}
					else
					{
						OnTB.Text = OnTrB.Value.ToString();
					}
				}
				if (isOFFLevelMouseDown)
				{
					return;
				}
				OffTrB.Value = GB.FSToolXPushStartLevel.OffLevel;
				if (OffTB.InvokeRequired)
				{
					OffTB.Invoke((Action)delegate
					{
						OffTB.Text = OffTrB.Value.ToString();
					});
				}
				else
				{
					OffTB.Text = OffTrB.Value.ToString();
				}
				return;
			}
			CurrTrB.Value = GB.FSToolYPushStartLevel.CurrentLevel;
			CurrTB.Text = GB.FSToolYPushStartLevel.CurrentLevel.ToString();
			if (!isONLevelMouseDown)
			{
				OnTrB.Value = GB.FSToolYPushStartLevel.OnLevel;
				if (OnTB.InvokeRequired)
				{
					OnTB.Invoke((Action)delegate
					{
						OnTB.Text = OnTrB.Value.ToString();
					});
				}
				else
				{
					OnTB.Text = OnTrB.Value.ToString();
				}
			}
			if (isOFFLevelMouseDown)
			{
				return;
			}
			OffTrB.Value = GB.FSToolYPushStartLevel.OffLevel;
			if (OffTB.InvokeRequired)
			{
				OffTB.Invoke((Action)delegate
				{
					OffTB.Text = OffTrB.Value.ToString();
				});
			}
			else
			{
				OffTB.Text = OffTrB.Value.ToString();
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (Mode == 0)
			{
				TCP.FSIDRead_ByTCP(651, 0, (ushort)Axis, 88, 0, 0);
			}
			else
			{
				TCP.FSIDRead_ByTCP(652, 0, (ushort)Axis, 88, 0, 0);
			}
			UpdateUI();
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form615_LeverStart_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form615_Level_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (GB.GetLevelTimer != null)
			{
				GB.GetLevelTimer.Stop();
			}
		}

		private void OnTrB_MouseUp(object sender, MouseEventArgs e)
		{
			if (Mode == 0)
			{
				if (Axis == 0)
				{
					GB.FSToolXLeverStartLevel.OnLevel = (ushort)OnTrB.Value;
					OnTB.Text = GB.FSToolXLeverStartLevel.OnLevel.ToString();
					TCP.FSIDWrite_ByTCP(602, 0, (ushort)Axis, GB.FSToolXLeverStartLevel.OnLevel, GB.FSToolXLeverStartLevel.OffLevel, 0);
				}
				else
				{
					GB.FSToolYLeverStartLevel.OnLevel = (ushort)OnTrB.Value;
					OnTB.Text = GB.FSToolYLeverStartLevel.OnLevel.ToString();
					TCP.FSIDWrite_ByTCP(602, 0, (ushort)Axis, GB.FSToolYLeverStartLevel.OnLevel, GB.FSToolYLeverStartLevel.OffLevel, 0);
				}
			}
			else if (Axis == 0)
			{
				GB.FSToolXPushStartLevel.OnLevel = (ushort)OnTrB.Value;
				OnTB.Text = GB.FSToolXPushStartLevel.OnLevel.ToString();
				TCP.FSIDWrite_ByTCP(603, 0, (ushort)Axis, GB.FSToolXPushStartLevel.OnLevel, GB.FSToolXPushStartLevel.OffLevel, 0);
			}
			else
			{
				GB.FSToolYPushStartLevel.OnLevel = (ushort)OnTrB.Value;
				OnTB.Text = GB.FSToolYPushStartLevel.OnLevel.ToString();
				TCP.FSIDWrite_ByTCP(603, 0, (ushort)Axis, GB.FSToolYPushStartLevel.OnLevel, GB.FSToolYPushStartLevel.OffLevel, 0);
			}
			isONLevelMouseDown = false;
		}

		private void OffTrB_MouseUp(object sender, MouseEventArgs e)
		{
			if (Mode == 0)
			{
				if (Axis == 0)
				{
					GB.FSToolXLeverStartLevel.OffLevel = (ushort)OffTrB.Value;
					OffTB.Text = GB.FSToolXLeverStartLevel.OffLevel.ToString();
					TCP.FSIDWrite_ByTCP(602, 0, (ushort)Axis, GB.FSToolXLeverStartLevel.OnLevel, GB.FSToolXLeverStartLevel.OffLevel, 0);
				}
				else
				{
					GB.FSToolYLeverStartLevel.OffLevel = (ushort)OffTrB.Value;
					OffTB.Text = GB.FSToolYLeverStartLevel.OffLevel.ToString();
					TCP.FSIDWrite_ByTCP(602, 0, (ushort)Axis, GB.FSToolYLeverStartLevel.OnLevel, GB.FSToolYLeverStartLevel.OffLevel, 0);
				}
			}
			else if (Axis == 0)
			{
				GB.FSToolXPushStartLevel.OffLevel = (ushort)OffTrB.Value;
				OffTB.Text = GB.FSToolXPushStartLevel.OffLevel.ToString();
				TCP.FSIDWrite_ByTCP(603, 0, (ushort)Axis, GB.FSToolXPushStartLevel.OnLevel, GB.FSToolXPushStartLevel.OffLevel, 0);
			}
			else
			{
				GB.FSToolYPushStartLevel.OffLevel = (ushort)OffTrB.Value;
				OffTB.Text = GB.FSToolYPushStartLevel.OffLevel.ToString();
				TCP.FSIDWrite_ByTCP(603, 0, (ushort)Axis, GB.FSToolYPushStartLevel.OnLevel, GB.FSToolYPushStartLevel.OffLevel, 0);
			}
			isOFFLevelMouseDown = false;
		}

		private void OnTrB_MouseDown(object sender, MouseEventArgs e)
		{
			isONLevelMouseDown = true;
		}

		private void OffTrB_MouseDown(object sender, MouseEventArgs e)
		{
			isOFFLevelMouseDown = true;
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
			this.OnTrB = new System.Windows.Forms.TrackBar();
			this.OffTrB = new System.Windows.Forms.TrackBar();
			this.CloseBn = new System.Windows.Forms.Label();
			this.lab_Title = new System.Windows.Forms.Label();
			this.lab_On = new System.Windows.Forms.Label();
			this.lab_Off = new System.Windows.Forms.Label();
			this.CurrTrB = new System.Windows.Forms.TrackBar();
			this.CurrTB = new System.Windows.Forms.TextBox();
			this.OnTB = new System.Windows.Forms.TextBox();
			this.OffTB = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)this.OnTrB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.OffTrB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.CurrTrB).BeginInit();
			base.SuspendLayout();
			this.OnTrB.Location = new System.Drawing.Point(179, 105);
			this.OnTrB.Name = "OnTrB";
			this.OnTrB.Size = new System.Drawing.Size(251, 56);
			this.OnTrB.TabIndex = 10;
			this.OnTrB.MouseDown += new System.Windows.Forms.MouseEventHandler(OnTrB_MouseDown);
			this.OnTrB.MouseUp += new System.Windows.Forms.MouseEventHandler(OnTrB_MouseUp);
			this.OffTrB.Location = new System.Drawing.Point(179, 158);
			this.OffTrB.Name = "OffTrB";
			this.OffTrB.Size = new System.Drawing.Size(251, 56);
			this.OffTrB.TabIndex = 10;
			this.OffTrB.MouseDown += new System.Windows.Forms.MouseEventHandler(OffTrB_MouseDown);
			this.OffTrB.MouseUp += new System.Windows.Forms.MouseEventHandler(OffTrB_MouseUp);
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(469, 3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 129;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, 0);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(500, 35);
			this.lab_Title.TabIndex = 128;
			this.lab_Title.Text = "Title";
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_On.AutoSize = true;
			this.lab_On.Location = new System.Drawing.Point(37, 105);
			this.lab_On.Name = "lab_On";
			this.lab_On.Size = new System.Drawing.Size(60, 15);
			this.lab_On.TabIndex = 130;
			this.lab_On.Text = "On Level";
			this.lab_Off.AutoSize = true;
			this.lab_Off.Location = new System.Drawing.Point(35, 161);
			this.lab_Off.Name = "lab_Off";
			this.lab_Off.Size = new System.Drawing.Size(63, 15);
			this.lab_Off.TabIndex = 130;
			this.lab_Off.Text = "Off Level";
			this.CurrTrB.Enabled = false;
			this.CurrTrB.Location = new System.Drawing.Point(179, 54);
			this.CurrTrB.Name = "CurrTrB";
			this.CurrTrB.Size = new System.Drawing.Size(251, 56);
			this.CurrTrB.TabIndex = 10;
			this.CurrTrB.TickStyle = System.Windows.Forms.TickStyle.None;
			this.CurrTB.Location = new System.Drawing.Point(102, 54);
			this.CurrTB.Name = "CurrTB";
			this.CurrTB.Size = new System.Drawing.Size(68, 25);
			this.CurrTB.TabIndex = 131;
			this.CurrTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.OnTB.Font = new System.Drawing.Font("新細明體", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.OnTB.ForeColor = System.Drawing.Color.OrangeRed;
			this.OnTB.Location = new System.Drawing.Point(102, 102);
			this.OnTB.Name = "OnTB";
			this.OnTB.Size = new System.Drawing.Size(68, 25);
			this.OnTB.TabIndex = 131;
			this.OnTB.Text = "0";
			this.OnTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.OffTB.Font = new System.Drawing.Font("新細明體", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.OffTB.ForeColor = System.Drawing.Color.ForestGreen;
			this.OffTB.Location = new System.Drawing.Point(102, 158);
			this.OffTB.Name = "OffTB";
			this.OffTB.Size = new System.Drawing.Size(68, 25);
			this.OffTB.TabIndex = 131;
			this.OffTB.Text = "0";
			this.OffTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 222);
			base.Controls.Add(this.OffTB);
			base.Controls.Add(this.OnTB);
			base.Controls.Add(this.CurrTB);
			base.Controls.Add(this.lab_Off);
			base.Controls.Add(this.lab_On);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.Controls.Add(this.OffTrB);
			base.Controls.Add(this.CurrTrB);
			base.Controls.Add(this.OnTrB);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form615_Level";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form615_Level_FormClosed);
			base.Load += new System.EventHandler(Form615_Level_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form615_LeverStart_Paint);
			((System.ComponentModel.ISupportInitialize)this.OnTrB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.OffTrB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.CurrTrB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
