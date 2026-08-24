using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form614_TempLevel : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private int Axis = 0;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private TextBox WNTB;

		private TextBox ALTB;

		private Label lab_WN;

		private Label lab_AL;

		private Button btn_Cancel;

		private Button btn_OK;

		private Label label1;

		private Label label2;

		public Form614_TempLevel(int Axis, GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.Axis = Axis;
			MultiLanguage.LoadLanguage(this);
			if (GB.CheckHMIVer(172, 0))
			{
				TCP.FSIDRead_ByTCP(663, 0, (ushort)Axis, 0, 0, 0);
			}
			WNTB.KeyPress += GB.RangeUnsigned65535;
			WNTB.LostFocus += GB.LostFocus_C0;
			ALTB.KeyPress += GB.RangeUnsigned65535;
			ALTB.LostFocus += GB.LostFocus_C0;
			UpdateUI();
		}

		public void UpdateUI()
		{
			if (Axis == 0)
			{
				WNTB.Text = GB.FSToolXTempLevel.WNLevel.ToString();
				ALTB.Text = GB.FSToolXTempLevel.ALLevel.ToString();
			}
			else
			{
				WNTB.Text = GB.FSToolYTempLevel.WNLevel.ToString();
				ALTB.Text = GB.FSToolYTempLevel.ALLevel.ToString();
			}
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (Axis == 0)
			{
				GB.FSToolXTempLevel.WNLevel = ushort.Parse(WNTB.Text);
				GB.FSToolXTempLevel.ALLevel = ushort.Parse(ALTB.Text);
				if (GB.FSToolXTempLevel.WNLevel > GB.FSToolXTempLevel.ALLevel)
				{
					GB.FSToolXTempLevel.WNLevel = GB.FSToolXTempLevel.ALLevel;
				}
				if (GB.CheckHMIVer(172, 0))
				{
					TCP.FSIDWrite_ByTCP(610, 0, (ushort)Axis, GB.FSToolXTempLevel.WNLevel, GB.FSToolXTempLevel.ALLevel, 0);
				}
			}
			else
			{
				GB.FSToolYTempLevel.WNLevel = ushort.Parse(WNTB.Text);
				GB.FSToolYTempLevel.ALLevel = ushort.Parse(ALTB.Text);
				if (GB.FSToolYTempLevel.WNLevel > GB.FSToolYTempLevel.ALLevel)
				{
					GB.FSToolYTempLevel.WNLevel = GB.FSToolYTempLevel.ALLevel;
				}
				if (GB.CheckHMIVer(172, 0))
				{
					TCP.FSIDWrite_ByTCP(610, 0, (ushort)Axis, GB.FSToolYTempLevel.WNLevel, GB.FSToolYTempLevel.ALLevel, 0);
				}
			}
			Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form614_TempLevel_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void Form614_TempLevel_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form614_TempLevel));
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.WNTB = new System.Windows.Forms.TextBox();
			this.ALTB = new System.Windows.Forms.TextBox();
			this.lab_WN = new System.Windows.Forms.Label();
			this.lab_AL = new System.Windows.Forms.Label();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_OK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(650, 50);
			this.lab_HanderTitle.TabIndex = 56;
			this.lab_HanderTitle.Text = "Tool Temperature";
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.WNTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.WNTB.Location = new System.Drawing.Point(400, 121);
			this.WNTB.Name = "WNTB";
			this.WNTB.Size = new System.Drawing.Size(60, 31);
			this.WNTB.TabIndex = 154;
			this.WNTB.Text = "0";
			this.WNTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ALTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.ALTB.Location = new System.Drawing.Point(400, 173);
			this.ALTB.Name = "ALTB";
			this.ALTB.Size = new System.Drawing.Size(60, 31);
			this.ALTB.TabIndex = 154;
			this.ALTB.Text = "0";
			this.ALTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_WN.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_WN.Location = new System.Drawing.Point(58, 121);
			this.lab_WN.Name = "lab_WN";
			this.lab_WN.Size = new System.Drawing.Size(281, 28);
			this.lab_WN.TabIndex = 155;
			this.lab_WN.Text = "Warning Level";
			this.lab_AL.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_AL.Location = new System.Drawing.Point(57, 180);
			this.lab_AL.Name = "lab_AL";
			this.lab_AL.Size = new System.Drawing.Size(283, 24);
			this.lab_AL.TabIndex = 155;
			this.lab_AL.Text = "Alarm Level";
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
			this.label1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.label1.Location = new System.Drawing.Point(502, 124);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 28);
			this.label1.TabIndex = 155;
			this.label1.Text = "°C";
			this.label2.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.label2.Location = new System.Drawing.Point(502, 176);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 28);
			this.label2.TabIndex = 155;
			this.label2.Text = "°C";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(650, 350);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lab_AL);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.lab_WN);
			base.Controls.Add(this.ALTB);
			base.Controls.Add(this.WNTB);
			base.Controls.Add(this.lab_HanderTitle);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form614_TempLevel";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form614_TempLevel_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form614_TempLevel_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
