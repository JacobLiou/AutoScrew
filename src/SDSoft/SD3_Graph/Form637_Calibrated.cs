using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form637_Calibrated : Form
	{
		private IContainer components = null;

		private Label lab_Title;

		private Button btn_Cancel;

		private Button btn_OK;

		private Label CloseBn;

		private Label lab_Before;

		private Label lab_Reminder1;

		private PictureBox pictureBox1;

		private Label lab_After;

		private Label label1;

		private Label label2;

		private TextBox TorqBeforeTB;

		private TextBox TorqAfterTB;

		private TextBox GainBeforeTB;

		private TextBox GainAfterTB;

		public event CreateForm637_YesHandler CreateYesAns;

		public Form637_Calibrated(float ToolHMITorque, float Tool3rdPartyTorque, ushort OrgGain, ushort NewGain)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			TorqBeforeTB.Text = ToolHMITorque.ToString("F3");
			TorqAfterTB.Text = Tool3rdPartyTorque.ToString("F3");
			GainBeforeTB.Text = OrgGain.ToString();
			GainAfterTB.Text = NewGain.ToString();
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form637_Calibrated_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void btn_Confirm_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateYesAns != null)
			{
				this.CreateYesAns();
			}
		}

		private void Form637_Calibrated_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form637_Calibrated));
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.lab_Before = new System.Windows.Forms.Label();
			this.lab_Reminder1 = new System.Windows.Forms.Label();
			this.btn_OK = new System.Windows.Forms.Button();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.lab_After = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.TorqBeforeTB = new System.Windows.Forms.TextBox();
			this.TorqAfterTB = new System.Windows.Forms.TextBox();
			this.GainBeforeTB = new System.Windows.Forms.TextBox();
			this.GainAfterTB = new System.Windows.Forms.TextBox();
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
			this.lab_Before.BackColor = System.Drawing.Color.Red;
			this.lab_Before.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Before.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Before.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Before.Location = new System.Drawing.Point(123, 84);
			this.lab_Before.Name = "lab_Before";
			this.lab_Before.Size = new System.Drawing.Size(86, 31);
			this.lab_Before.TabIndex = 62;
			this.lab_Before.Text = "Before";
			this.lab_Before.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_Reminder1.BackColor = System.Drawing.Color.Transparent;
			this.lab_Reminder1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Reminder1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.lab_Reminder1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Reminder1.Location = new System.Drawing.Point(141, 46);
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
			this.btn_OK.Location = new System.Drawing.Point(111, 168);
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
			this.btn_Cancel.Location = new System.Drawing.Point(293, 168);
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
			this.lab_After.BackColor = System.Drawing.Color.Red;
			this.lab_After.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_After.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_After.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_After.Location = new System.Drawing.Point(305, 84);
			this.lab_After.Name = "lab_After";
			this.lab_After.Size = new System.Drawing.Size(86, 31);
			this.lab_After.TabIndex = 62;
			this.lab_After.Text = "After";
			this.lab_After.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label1.Font = new System.Drawing.Font("新細明體", 12f);
			this.label1.Location = new System.Drawing.Point(123, 126);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 25);
			this.label1.TabIndex = 129;
			this.label1.Text = "Gain:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label2.Font = new System.Drawing.Font("新細明體", 12f);
			this.label2.Location = new System.Drawing.Point(305, 126);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 25);
			this.label2.TabIndex = 129;
			this.label2.Text = "Gain:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TorqBeforeTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TorqBeforeTB.Location = new System.Drawing.Point(211, 84);
			this.TorqBeforeTB.Name = "TorqBeforeTB";
			this.TorqBeforeTB.ReadOnly = true;
			this.TorqBeforeTB.Size = new System.Drawing.Size(75, 31);
			this.TorqBeforeTB.TabIndex = 130;
			this.TorqBeforeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TorqAfterTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.TorqAfterTB.Location = new System.Drawing.Point(393, 84);
			this.TorqAfterTB.Name = "TorqAfterTB";
			this.TorqAfterTB.ReadOnly = true;
			this.TorqAfterTB.Size = new System.Drawing.Size(75, 31);
			this.TorqAfterTB.TabIndex = 130;
			this.TorqAfterTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.GainBeforeTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.GainBeforeTB.Location = new System.Drawing.Point(211, 126);
			this.GainBeforeTB.Name = "GainBeforeTB";
			this.GainBeforeTB.ReadOnly = true;
			this.GainBeforeTB.Size = new System.Drawing.Size(75, 31);
			this.GainBeforeTB.TabIndex = 130;
			this.GainBeforeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.GainAfterTB.Font = new System.Drawing.Font("新細明體", 12f);
			this.GainAfterTB.Location = new System.Drawing.Point(393, 126);
			this.GainAfterTB.Name = "GainAfterTB";
			this.GainAfterTB.ReadOnly = true;
			this.GainAfterTB.Size = new System.Drawing.Size(75, 31);
			this.GainAfterTB.TabIndex = 130;
			this.GainAfterTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 222);
			base.Controls.Add(this.GainAfterTB);
			base.Controls.Add(this.GainBeforeTB);
			base.Controls.Add(this.TorqAfterTB);
			base.Controls.Add(this.TorqBeforeTB);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.pictureBox1);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.lab_Reminder1);
			base.Controls.Add(this.lab_After);
			base.Controls.Add(this.lab_Before);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form637_Calibrated";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form637_Calibrated_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form637_Calibrated_Paint);
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
