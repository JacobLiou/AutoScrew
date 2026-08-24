using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form995_RemindOKNG : Form
	{
		public int FormTypeNum;

		private Timer timer;

		private int countdown = 12;

		public GlobalVar GB;

		private IContainer components = null;

		private Button btn_OK;

		private Label lab_HanderTitle;

		private TextBox RemindNoTB;

		public event CreateForm995_CloseHandler CreateCloseEvent;

		public Form995_RemindOKNG(GlobalVar GB, int FormNum, string ExStr)
		{
			InitializeComponent();
			this.GB = GB;
			MultiLanguage.LoadLanguage(this);
			FormTypeNum = FormNum;
			lab_HanderTitle.ForeColor = Color.White;
			RemindNoTB.Text = FormNum.ToString("D4");
			if (FormNum == 3400 || FormNum == 3401)
			{
				lab_HanderTitle.Text = ExStr;
			}
			else
			{
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "tp_Remind" + FormNum.ToString("D4")) + ExStr;
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			countdown--;
			if (countdown <= 0)
			{
				this.CreateCloseEvent = null;
				Close();
				timer.Stop();
			}
		}

		private void btn_Confirm_Click(object sender, EventArgs e)
		{
			if (this.CreateCloseEvent != null)
			{
				this.CreateCloseEvent();
			}
			Close();
			if (timer != null)
			{
				timer.Stop();
			}
		}

		private void Form995_RemindOKNG_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = null;
			if (FormTypeNum == 3400)
			{
				Color MesNG = Color.FromArgb(255, 69, 0);
				lab_HanderTitle.BackColor = MesNG;
				pen1 = new Pen(MesNG, 8f);
			}
			else if (FormTypeNum == 3401)
			{
				Color MesOK = Color.FromArgb(46, 139, 87);
				lab_HanderTitle.BackColor = MesOK;
				pen1 = new Pen(MesOK, 8f);
			}
			else if (FormTypeNum < 3100)
			{
				lab_HanderTitle.BackColor = Color.DodgerBlue;
				pen1 = new Pen(Color.DodgerBlue, 8f);
			}
			else
			{
				lab_HanderTitle.BackColor = Color.Red;
				pen1 = new Pen(Color.Red, 8f);
			}
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form995_RemindOKNG_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (timer != null)
			{
				timer.Stop();
			}
		}

		private void Form995_RemindOKNG_Load(object sender, EventArgs e)
		{
			if (FormTypeNum < 3100)
			{
				timer = new Timer();
				timer.Interval = 100;
				timer.Tick += Timer_Tick;
				timer.Start();
			}
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form995_RemindOKNG));
			this.btn_OK = new System.Windows.Forms.Button();
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.RemindNoTB = new System.Windows.Forms.TextBox();
			base.SuspendLayout();
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(202, 164);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 67;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_Confirm_Click);
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
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
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 225);
			base.Controls.Add(this.RemindNoTB);
			base.Controls.Add(this.lab_HanderTitle);
			base.Controls.Add(this.btn_OK);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form995_RemindOKNG";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form995_RemindOKNG_FormClosed);
			base.Load += new System.EventHandler(Form995_RemindOKNG_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form995_RemindOKNG_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
