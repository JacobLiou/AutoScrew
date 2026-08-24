using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form503_Format : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private IContainer components = null;

		private Label lab_Title;

		private Button btn_Cancel;

		private Button btn_OK;

		private Label CloseBn;

		private Label lab_Reminder2;

		private Label lab_Reminder1;

		private PictureBox pictureBox1;

		public Form503_Format(GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form503_Format_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void btn_Confirm_Click(object sender, EventArgs e)
		{
			GB.ALNGMsgStartStopFunction(false);
			TCP.FSIDWrite_ByTCP(505, 0, 99, 88, 34, 0);
			GB.ALNGMsgStartStopFunction(true);
			Close();
		}

		private void Form503_Format_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form503_Format));
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.lab_Reminder2 = new System.Windows.Forms.Label();
			this.lab_Reminder1 = new System.Windows.Forms.Label();
			this.btn_OK = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, -1);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(500, 35);
			this.lab_Title.TabIndex = 62;
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(469, 2);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.lab_Reminder2.BackColor = System.Drawing.Color.Red;
			this.lab_Reminder2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Reminder2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Reminder2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Reminder2.Location = new System.Drawing.Point(142, 84);
			this.lab_Reminder2.Name = "lab_Reminder2";
			this.lab_Reminder2.Size = new System.Drawing.Size(315, 35);
			this.lab_Reminder2.TabIndex = 62;
			this.lab_Reminder2.Text = "All data will be deleted!";
			this.lab_Reminder2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Reminder1.BackColor = System.Drawing.Color.Transparent;
			this.lab_Reminder1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Reminder1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.lab_Reminder1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Reminder1.Location = new System.Drawing.Point(142, 49);
			this.lab_Reminder1.Name = "lab_Reminder1";
			this.lab_Reminder1.Size = new System.Drawing.Size(315, 35);
			this.lab_Reminder1.TabIndex = 62;
			this.lab_Reminder1.Text = "Confirm factory reset?";
			this.lab_Reminder1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(111, 160);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 65;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_Confirm_Click);
			this.btn_Cancel.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Cancel.BackgroundImage");
			this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Cancel.FlatAppearance.BorderSize = 0;
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Cancel.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Cancel.Location = new System.Drawing.Point(293, 160);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(92, 30);
			this.btn_Cancel.TabIndex = 64;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.Image = SD3Soft.Properties.Resources.mag;
			this.pictureBox1.Location = new System.Drawing.Point(41, 53);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(70, 65);
			this.pictureBox1.TabIndex = 128;
			this.pictureBox1.TabStop = false;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 222);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.lab_Reminder1);
			base.Controls.Add(this.lab_Reminder2);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form503_Format";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form503_Format_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form503_Format_Paint);
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
