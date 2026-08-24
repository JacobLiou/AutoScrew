using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form616_ToolLedDelayTimer : Form
	{
		private GlobalVar GB;

		private TCPclient TCP;

		private int Page_Axis = 0;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private TextBox LedDelayTmrTB;

		private Label lab_LedDelayTmr;

		private Label lab_ms;

		private Button btn_Cancel;

		private Button btn_OK;

		public Form616_ToolLedDelayTimer(GlobalVar GB, TCPclient TCP, int Axis)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			Page_Axis = Axis;
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(LedDelayTmrTB, GB.UISys.RangeStr + "300-10000");
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form616_ToolLedDelayTimer_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form616_ToolLedDelayTimer_Load(object sender, EventArgs e)
		{
			LedDelayTmrTB.KeyPress += EVENT_KeyPress;
			LedDelayTmrTB.LostFocus += LostFocus_C0;
			TCP.FSIDRead_ByTCP(664, 0, (ushort)Page_Axis, 0, 0, 0);
			LedDelayTmrTB.Text = ((Page_Axis == 0) ? GB.FSToolXLedDelayTmr.Value.ToString() : GB.FSToolYLedDelayTmr.Value.ToString());
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		public void EVENT_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeUnsigned300_10000(sender, e);
		}

		public void LostFocus_C0(object sender, EventArgs e)
		{
			if (((TextBox)sender).Text == "")
			{
				((TextBox)sender).Text = "300";
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (ushort.TryParse(LedDelayTmrTB.Text, out var result) && result >= 300 && result <= 10000)
			{
				if (Page_Axis == 0)
				{
					GB.FSToolXLedDelayTmr.Value = result;
				}
				else
				{
					GB.FSToolYLedDelayTmr.Value = result;
				}
				TCP.FSIDWrite_ByTCP(611, 0, (ushort)Page_Axis, result, 0, 0);
				Close();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form616_ToolLedDelayTimer));
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.LedDelayTmrTB = new System.Windows.Forms.TextBox();
			this.lab_LedDelayTmr = new System.Windows.Forms.Label();
			this.lab_ms = new System.Windows.Forms.Label();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_OK = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(1, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(600, 35);
			this.lab_HanderTitle.TabIndex = 60;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.LedDelayTmrTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.LedDelayTmrTB.Location = new System.Drawing.Point(400, 100);
			this.LedDelayTmrTB.Margin = new System.Windows.Forms.Padding(4);
			this.LedDelayTmrTB.Name = "LedDelayTmrTB";
			this.LedDelayTmrTB.Size = new System.Drawing.Size(100, 27);
			this.LedDelayTmrTB.TabIndex = 156;
			this.LedDelayTmrTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_LedDelayTmr.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_LedDelayTmr.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_LedDelayTmr.Location = new System.Drawing.Point(13, 100);
			this.lab_LedDelayTmr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_LedDelayTmr.Name = "lab_LedDelayTmr";
			this.lab_LedDelayTmr.Size = new System.Drawing.Size(379, 27);
			this.lab_LedDelayTmr.TabIndex = 155;
			this.lab_LedDelayTmr.Text = "Tightening Status LED Hold Time";
			this.lab_LedDelayTmr.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_ms.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_ms.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_ms.Location = new System.Drawing.Point(508, 100);
			this.lab_ms.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_ms.Name = "lab_ms";
			this.lab_ms.Size = new System.Drawing.Size(58, 27);
			this.lab_ms.TabIndex = 155;
			this.lab_ms.Text = "ms";
			this.lab_ms.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btn_Cancel.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Cancel.BackgroundImage");
			this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Cancel.FlatAppearance.BorderSize = 0;
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Cancel.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Cancel.Location = new System.Drawing.Point(348, 212);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(92, 30);
			this.btn_Cancel.TabIndex = 159;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(161, 212);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 158;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_OK_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(600, 279);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.LedDelayTmrTB);
			base.Controls.Add(this.lab_ms);
			base.Controls.Add(this.lab_LedDelayTmr);
			base.Controls.Add(this.lab_HanderTitle);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form616_ToolLedDelayTimer";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form616_ToolLedDelayTimer";
			base.Load += new System.EventHandler(Form616_ToolLedDelayTimer_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form616_ToolLedDelayTimer_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
