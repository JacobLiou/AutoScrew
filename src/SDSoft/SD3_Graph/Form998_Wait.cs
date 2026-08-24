using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form998_Wait : Form
	{
		public GlobalVar GB;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private CircleProgressBar1 circleProgressBar11;

		public Form998_Wait(GlobalVar GB)
		{
			InitializeComponent();
			this.GB = GB;
			circleProgressBar11.Progress = 0;
			circleProgressBar11.MaxValue = 0;
		}

		public void Process(bool SwitchOpen, int Process, int MaxProcess)
		{
			if (!SwitchOpen || MaxProcess == 0)
			{
				Close();
				return;
			}
			circleProgressBar11.Progress = Process;
			circleProgressBar11.MaxValue = MaxProcess;
			if (Process >= MaxProcess)
			{
				Close();
			}
		}

		public void ProcessStart(int MaxProcess)
		{
			circleProgressBar11.Progress = 0;
			circleProgressBar11.MaxValue = MaxProcess;
		}

		private void Form998_Wait_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form998_Wait_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
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
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.circleProgressBar11 = new SD3_Graph.CircleProgressBar1();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(-3, -2);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(500, 35);
			this.lab_HanderTitle.TabIndex = 56;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.circleProgressBar11.BackColor = System.Drawing.Color.White;
			this.circleProgressBar11.BottomColor = System.Drawing.Color.FromArgb(224, 224, 224);
			this.circleProgressBar11.FinishedColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.circleProgressBar11.Location = new System.Drawing.Point(99, 60);
			this.circleProgressBar11.MaxValue = 999999;
			this.circleProgressBar11.Name = "circleProgressBar11";
			this.circleProgressBar11.Progress = 999999;
			this.circleProgressBar11.Size = new System.Drawing.Size(296, 210);
			this.circleProgressBar11.TabIndex = 57;
			this.circleProgressBar11.Text = "circleProgressBar11";
			this.circleProgressBar11.TopColor = System.Drawing.Color.FromArgb(78, 134, 239);
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(495, 320);
			base.Controls.Add(this.circleProgressBar11);
			base.Controls.Add(this.lab_HanderTitle);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form998_Wait";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form998_Wait_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form998_Wait_Paint);
			base.ResumeLayout(false);
		}
	}
}
