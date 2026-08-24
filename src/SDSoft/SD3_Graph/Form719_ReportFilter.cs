using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form719_ReportFilter : Form
	{
		private bool DropDownStartFlag = false;

		private bool DropDownEndFlag = false;

		private GlobalVar GB = null;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private Label CloseBn;

		private CheckBox DateCB;

		private CheckBox Tool1CB;

		private CheckBox TGOKCB;

		private DateTimePicker StartDatePI;

		private DateTimePicker EndDatePI;

		private Label label1;

		private PictureBox TGOKPB;

		private PictureBox TGNGPB;

		private PictureBox LOOKPB;

		private PictureBox LONGPB;

		private PictureBox PASSPB;

		private CheckBox Tool2CB;

		private CheckBox TGNGCB;

		private CheckBox LOOKCB;

		private CheckBox LONGCB;

		private CheckBox PASSCB;

		private Label TOOL1TB;

		private Label TOOL2TB;

		private Button btn_OK;

		private Label label2;

		private Label label3;

		public event CreateForm719_Handler CreateID;

		public Form719_ReportFilter(GlobalVar GB)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this, "ButtonBase");
			this.GB = GB;
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form719_ReportFilter_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
			TOOL1TB.Text = MultiLanguage.GetStr("Form700_Report", "tp_Tool1");
			TOOL2TB.Text = MultiLanguage.GetStr("Form700_Report", "tp_Tool2");
			StartDatePI.Value = new DateTime(GB.UISys.StartYY, GB.UISys.StartMM, GB.UISys.StartDD);
			EndDatePI.Value = new DateTime(GB.UISys.EndYY, GB.UISys.EndMM, GB.UISys.EndDD);
			DateTimePicker startDatePI = StartDatePI;
			DateTime maxDate = (EndDatePI.MaxDate = DateTime.Today);
			startDatePI.MaxDate = maxDate;
			CheckBox dateCB = DateCB;
			DateTimePicker startDatePI2 = StartDatePI;
			bool flag = (EndDatePI.Enabled = GB.UISys.EnDisFTDate == 1);
			bool flag3 = (startDatePI2.Enabled = flag);
			dateCB.Checked = flag3;
			Tool1CB.Checked = (GB.UISys.EnDisFTTool & 1) == 1;
			Tool2CB.Checked = (GB.UISys.EnDisFTTool & 2) == 2;
			TGOKCB.Checked = (GB.UISys.EnDisFTStatus & 1) == 1;
			TGNGCB.Checked = (GB.UISys.EnDisFTStatus & 2) == 2;
			LOOKCB.Checked = (GB.UISys.EnDisFTStatus & 4) == 4;
			LONGCB.Checked = (GB.UISys.EnDisFTStatus & 8) == 8;
		}

		private void Form719_ReportFilter_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void btn_Confirm_Click(object sender, EventArgs e)
		{
			GB.UISys.EnDisFTDate = (ushort)(DateCB.Checked ? 1 : 0);
			GB.UISys.EnDisFTTool = 0;
			if (Tool1CB.Checked)
			{
				GB.UISys.EnDisFTTool |= 1;
			}
			if (Tool2CB.Checked)
			{
				GB.UISys.EnDisFTTool |= 2;
			}
			GB.UISys.EnDisFTStatus = 0;
			if (TGOKCB.Checked)
			{
				GB.UISys.EnDisFTStatus |= 1;
			}
			if (TGNGCB.Checked)
			{
				GB.UISys.EnDisFTStatus |= 2;
			}
			if (LOOKCB.Checked)
			{
				GB.UISys.EnDisFTStatus |= 4;
			}
			if (LONGCB.Checked)
			{
				GB.UISys.EnDisFTStatus |= 8;
			}
			if (PASSCB.Checked)
			{
				GB.UISys.EnDisFTStatus |= 16;
			}
			Close();
			if (this.CreateID != null)
			{
				this.CreateID();
			}
		}

		private void DateCB_CheckedChanged(object sender, EventArgs e)
		{
			DateTimePicker startDatePI = StartDatePI;
			bool enabled = (EndDatePI.Enabled = DateCB.Checked);
			startDatePI.Enabled = enabled;
		}

		private void StartDatePI_ValueChanged(object sender, EventArgs e)
		{
			if (DropDownStartFlag)
			{
				DropDownStartFlag = false;
				if (EndDatePI.Value < StartDatePI.Value)
				{
					EndDatePI.Value = StartDatePI.Value;
				}
			}
			EndDatePI.MinDate = StartDatePI.Value;
		}

		private void EndDatePI_ValueChanged(object sender, EventArgs e)
		{
			if (DropDownEndFlag)
			{
				DropDownEndFlag = false;
				if (EndDatePI.Value < StartDatePI.Value)
				{
					StartDatePI.Value = EndDatePI.Value;
				}
			}
			StartDatePI.MaxDate = EndDatePI.Value;
		}

		private void StartDatePI_DropDown(object sender, EventArgs e)
		{
			DropDownStartFlag = true;
		}

		private void EndDatePI_DropDown(object sender, EventArgs e)
		{
			DropDownEndFlag = true;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form719_ReportFilter));
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.DateCB = new System.Windows.Forms.CheckBox();
			this.Tool1CB = new System.Windows.Forms.CheckBox();
			this.TGOKCB = new System.Windows.Forms.CheckBox();
			this.StartDatePI = new System.Windows.Forms.DateTimePicker();
			this.EndDatePI = new System.Windows.Forms.DateTimePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.TGOKPB = new System.Windows.Forms.PictureBox();
			this.TGNGPB = new System.Windows.Forms.PictureBox();
			this.LOOKPB = new System.Windows.Forms.PictureBox();
			this.LONGPB = new System.Windows.Forms.PictureBox();
			this.PASSPB = new System.Windows.Forms.PictureBox();
			this.Tool2CB = new System.Windows.Forms.CheckBox();
			this.TGNGCB = new System.Windows.Forms.CheckBox();
			this.LOOKCB = new System.Windows.Forms.CheckBox();
			this.LONGCB = new System.Windows.Forms.CheckBox();
			this.PASSCB = new System.Windows.Forms.CheckBox();
			this.TOOL1TB = new System.Windows.Forms.Label();
			this.TOOL2TB = new System.Windows.Forms.Label();
			this.btn_OK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)this.TGOKPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.TGNGPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.LOOKPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.LONGPB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.PASSPB).BeginInit();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, 0);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(600, 35);
			this.lab_HanderTitle.TabIndex = 58;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(564, 0);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 124;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.DateCB.AutoSize = true;
			this.DateCB.Location = new System.Drawing.Point(30, 121);
			this.DateCB.Name = "DateCB";
			this.DateCB.Size = new System.Drawing.Size(18, 17);
			this.DateCB.TabIndex = 126;
			this.DateCB.UseVisualStyleBackColor = true;
			this.DateCB.CheckedChanged += new System.EventHandler(DateCB_CheckedChanged);
			this.Tool1CB.AutoSize = true;
			this.Tool1CB.Location = new System.Drawing.Point(30, 204);
			this.Tool1CB.Name = "Tool1CB";
			this.Tool1CB.Size = new System.Drawing.Size(18, 17);
			this.Tool1CB.TabIndex = 126;
			this.Tool1CB.UseVisualStyleBackColor = true;
			this.TGOKCB.AutoSize = true;
			this.TGOKCB.Location = new System.Drawing.Point(30, 283);
			this.TGOKCB.Name = "TGOKCB";
			this.TGOKCB.Size = new System.Drawing.Size(18, 17);
			this.TGOKCB.TabIndex = 126;
			this.TGOKCB.UseVisualStyleBackColor = true;
			this.StartDatePI.Location = new System.Drawing.Point(61, 117);
			this.StartDatePI.Name = "StartDatePI";
			this.StartDatePI.Size = new System.Drawing.Size(153, 25);
			this.StartDatePI.TabIndex = 127;
			this.StartDatePI.ValueChanged += new System.EventHandler(StartDatePI_ValueChanged);
			this.StartDatePI.DropDown += new System.EventHandler(StartDatePI_DropDown);
			this.EndDatePI.Location = new System.Drawing.Point(257, 117);
			this.EndDatePI.Name = "EndDatePI";
			this.EndDatePI.Size = new System.Drawing.Size(153, 25);
			this.EndDatePI.TabIndex = 127;
			this.EndDatePI.ValueChanged += new System.EventHandler(EndDatePI_ValueChanged);
			this.EndDatePI.DropDown += new System.EventHandler(EndDatePI_DropDown);
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(228, 122);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(15, 15);
			this.label1.TabIndex = 128;
			this.label1.Text = "~";
			this.TGOKPB.BackgroundImage = SD3Soft.Properties.Resources.TG_OK;
			this.TGOKPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.TGOKPB.Location = new System.Drawing.Point(64, 280);
			this.TGOKPB.Name = "TGOKPB";
			this.TGOKPB.Size = new System.Drawing.Size(60, 30);
			this.TGOKPB.TabIndex = 129;
			this.TGOKPB.TabStop = false;
			this.TGNGPB.BackgroundImage = SD3Soft.Properties.Resources.TG_NG;
			this.TGNGPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.TGNGPB.Location = new System.Drawing.Point(178, 280);
			this.TGNGPB.Name = "TGNGPB";
			this.TGNGPB.Size = new System.Drawing.Size(60, 30);
			this.TGNGPB.TabIndex = 129;
			this.TGNGPB.TabStop = false;
			this.LOOKPB.BackgroundImage = SD3Soft.Properties.Resources.Loos_OK;
			this.LOOKPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.LOOKPB.Location = new System.Drawing.Point(290, 280);
			this.LOOKPB.Name = "LOOKPB";
			this.LOOKPB.Size = new System.Drawing.Size(60, 30);
			this.LOOKPB.TabIndex = 129;
			this.LOOKPB.TabStop = false;
			this.LONGPB.BackgroundImage = SD3Soft.Properties.Resources.Loos_NG;
			this.LONGPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.LONGPB.Location = new System.Drawing.Point(402, 280);
			this.LONGPB.Name = "LONGPB";
			this.LONGPB.Size = new System.Drawing.Size(60, 30);
			this.LONGPB.TabIndex = 129;
			this.LONGPB.TabStop = false;
			this.PASSPB.BackgroundImage = SD3Soft.Properties.Resources.Pass;
			this.PASSPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.PASSPB.Location = new System.Drawing.Point(514, 280);
			this.PASSPB.Name = "PASSPB";
			this.PASSPB.Size = new System.Drawing.Size(60, 30);
			this.PASSPB.TabIndex = 129;
			this.PASSPB.TabStop = false;
			this.Tool2CB.AutoSize = true;
			this.Tool2CB.Location = new System.Drawing.Point(148, 204);
			this.Tool2CB.Name = "Tool2CB";
			this.Tool2CB.Size = new System.Drawing.Size(18, 17);
			this.Tool2CB.TabIndex = 126;
			this.Tool2CB.UseVisualStyleBackColor = true;
			this.TGNGCB.AutoSize = true;
			this.TGNGCB.Location = new System.Drawing.Point(148, 283);
			this.TGNGCB.Name = "TGNGCB";
			this.TGNGCB.Size = new System.Drawing.Size(18, 17);
			this.TGNGCB.TabIndex = 126;
			this.TGNGCB.UseVisualStyleBackColor = true;
			this.LOOKCB.AutoSize = true;
			this.LOOKCB.Location = new System.Drawing.Point(257, 283);
			this.LOOKCB.Name = "LOOKCB";
			this.LOOKCB.Size = new System.Drawing.Size(18, 17);
			this.LOOKCB.TabIndex = 126;
			this.LOOKCB.UseVisualStyleBackColor = true;
			this.LONGCB.AutoSize = true;
			this.LONGCB.Location = new System.Drawing.Point(369, 283);
			this.LONGCB.Name = "LONGCB";
			this.LONGCB.Size = new System.Drawing.Size(18, 17);
			this.LONGCB.TabIndex = 126;
			this.LONGCB.UseVisualStyleBackColor = true;
			this.PASSCB.AutoSize = true;
			this.PASSCB.Location = new System.Drawing.Point(481, 283);
			this.PASSCB.Name = "PASSCB";
			this.PASSCB.Size = new System.Drawing.Size(18, 17);
			this.PASSCB.TabIndex = 126;
			this.PASSCB.UseVisualStyleBackColor = true;
			this.TOOL1TB.BackColor = System.Drawing.Color.FromArgb(160, 217, 246);
			this.TOOL1TB.Location = new System.Drawing.Point(68, 200);
			this.TOOL1TB.Name = "TOOL1TB";
			this.TOOL1TB.Size = new System.Drawing.Size(60, 25);
			this.TOOL1TB.TabIndex = 130;
			this.TOOL1TB.Text = "Tool1";
			this.TOOL1TB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.TOOL2TB.BackColor = System.Drawing.Color.FromArgb(218, 228, 145);
			this.TOOL2TB.Location = new System.Drawing.Point(175, 200);
			this.TOOL2TB.Name = "TOOL2TB";
			this.TOOL2TB.Size = new System.Drawing.Size(60, 25);
			this.TOOL2TB.TabIndex = 130;
			this.TOOL2TB.Text = "Tool2";
			this.TOOL2TB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(245, 374);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 131;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_Confirm_Click);
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(102, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 15);
			this.label2.TabIndex = 132;
			this.label2.Text = "Start Date";
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(301, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 15);
			this.label3.TabIndex = 132;
			this.label3.Text = "End Date";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(600, 445);
			base.Controls.Add(this.label3);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.TOOL2TB);
			base.Controls.Add(this.TOOL1TB);
			base.Controls.Add(this.PASSPB);
			base.Controls.Add(this.LONGPB);
			base.Controls.Add(this.LOOKPB);
			base.Controls.Add(this.TGNGPB);
			base.Controls.Add(this.TGOKPB);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.EndDatePI);
			base.Controls.Add(this.PASSCB);
			base.Controls.Add(this.LONGCB);
			base.Controls.Add(this.LOOKCB);
			base.Controls.Add(this.TGNGCB);
			base.Controls.Add(this.Tool2CB);
			base.Controls.Add(this.StartDatePI);
			base.Controls.Add(this.TGOKCB);
			base.Controls.Add(this.Tool1CB);
			base.Controls.Add(this.DateCB);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_HanderTitle);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form719_ReportFilter";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form710_ReportInfo";
			base.Load += new System.EventHandler(Form719_ReportFilter_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form719_ReportFilter_Paint);
			((System.ComponentModel.ISupportInitialize)this.TGOKPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.TGNGPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.LOOKPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.LONGPB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.PASSPB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
