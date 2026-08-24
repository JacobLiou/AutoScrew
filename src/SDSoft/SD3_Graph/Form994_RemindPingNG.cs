using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form994_RemindPingNG : Form
	{
		public int FormTypeNum;

		public GlobalVar GB;

		private IContainer components = null;

		private Button btn_Retry;

		private Label lab_HanderTitle;

		private TextBox RemindNoTB;

		private Button btn_Abort;

		public event CreateForm994_Handler CreateYesAns;

		public event CreateForm994_Handler CreateNoAns;

		public Form994_RemindPingNG(GlobalVar GB, int FormNum)
		{
			InitializeComponent();
			this.GB = GB;
			MultiLanguage.LoadLanguage(this);
			FormTypeNum = FormNum;
			lab_HanderTitle.ForeColor = Color.White;
			RemindNoTB.Text = FormNum.ToString("D4");
			lab_HanderTitle.Text = MultiLanguage.GetStr("Form995_RemindOKNG", "tp_Remind" + FormNum.ToString("D4"));
		}

		private void btn_Confirm_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form994_RemindPingNG_Paint(object sender, PaintEventArgs e)
		{
			lab_HanderTitle.BackColor = Color.Red;
			Pen pen1 = new Pen(Color.Red, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void btn_Retry_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateNoAns != null)
			{
				this.CreateNoAns();
			}
		}

		private void btn_Abort_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateYesAns != null)
			{
				this.CreateYesAns();
			}
		}

		private void Form994_RemindPingNG_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form994_RemindPingNG));
			this.btn_Retry = new System.Windows.Forms.Button();
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.RemindNoTB = new System.Windows.Forms.TextBox();
			this.btn_Abort = new System.Windows.Forms.Button();
			base.SuspendLayout();
			this.btn_Retry.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Retry.BackgroundImage");
			this.btn_Retry.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Retry.FlatAppearance.BorderSize = 0;
			this.btn_Retry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Retry.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Retry.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Retry.Location = new System.Drawing.Point(317, 162);
			this.btn_Retry.Name = "btn_Retry";
			this.btn_Retry.Size = new System.Drawing.Size(92, 30);
			this.btn_Retry.TabIndex = 67;
			this.btn_Retry.Text = "Retry";
			this.btn_Retry.UseVisualStyleBackColor = true;
			this.btn_Retry.Click += new System.EventHandler(btn_Retry_Click);
			this.lab_HanderTitle.BackColor = System.Drawing.Color.Red;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_HanderTitle.ForeColor = System.Drawing.Color.White;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(500, 86);
			this.lab_HanderTitle.TabIndex = 68;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.RemindNoTB.BackColor = System.Drawing.SystemColors.Control;
			this.RemindNoTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.RemindNoTB.ForeColor = System.Drawing.SystemColors.ButtonShadow;
			this.RemindNoTB.Location = new System.Drawing.Point(352, 198);
			this.RemindNoTB.Name = "RemindNoTB";
			this.RemindNoTB.Size = new System.Drawing.Size(136, 18);
			this.RemindNoTB.TabIndex = 69;
			this.RemindNoTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.btn_Abort.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Abort.BackgroundImage");
			this.btn_Abort.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Abort.FlatAppearance.BorderSize = 0;
			this.btn_Abort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Abort.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Abort.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Abort.Location = new System.Drawing.Point(87, 162);
			this.btn_Abort.Name = "btn_Abort";
			this.btn_Abort.Size = new System.Drawing.Size(92, 30);
			this.btn_Abort.TabIndex = 67;
			this.btn_Abort.Text = "Abort";
			this.btn_Abort.UseVisualStyleBackColor = true;
			this.btn_Abort.Click += new System.EventHandler(btn_Abort_Click);
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 225);
			base.Controls.Add(this.RemindNoTB);
			base.Controls.Add(this.lab_HanderTitle);
			base.Controls.Add(this.btn_Abort);
			base.Controls.Add(this.btn_Retry);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form994_RemindPingNG";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form994_RemindPingNG_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form994_RemindPingNG_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
