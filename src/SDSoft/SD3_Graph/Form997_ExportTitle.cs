using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form997_ExportTitle : Form
	{
		private GlobalVar GB = null;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private Button btn_Cancel;

		private Button btn_OK;

		private TextBox InputTitleTB;

		private RadioButton RB_ParamVer0;

		private RadioButton RB_ParamVer1;

		private GroupBox ParamGB;

		private GroupBox ReportGB;

		private CheckBox Report3CB;

		private CheckBox Report2CB;

		private CheckBox Report1CB;

		public event CreateForm997_Handler CreateID;

		public event CreateForm997_ParamAdHandler CreateParam;

		public event CreateForm997_ReportAdHandler CreateReport;

		public Form997_ExportTitle(FormType FormNum, GlobalVar GB)
		{
			InitializeComponent();
			this.GB = GB;
			MultiLanguage.LoadLanguage(this);
			InputTitleTB.KeyPress += GB.RangeASCIIInput;
			InputTitleTB.Multiline = false;
			InputTitleTB.ShortcutsEnabled = false;
			DateTime currentDate = DateTime.Now;
			switch (FormNum)
			{
			case FormType.ExportParamTitle:
				IconVisible(true, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle0");
				break;
			case FormType.ExportSeqTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle1");
				break;
			case FormType.ExportSrcHandleTitle:
				IconVisible(false, false, false, true);
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle2");
				break;
			case FormType.ExportSrcBitsTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle3");
				break;
			case FormType.ExportSrcScanTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle4");
				break;
			case FormType.ExportCtrlSystemTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle7");
				break;
			case FormType.ExportCtrlDIOTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle8");
				break;
			case FormType.ExportCtrlTableTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle9");
				break;
			case FormType.ExportToolSystemTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle10");
				break;
			case FormType.ExportToolSensityTitle:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle11");
				break;
			case FormType.ExportAllReportTitle:
				IconVisible(false, true, true, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle5");
				break;
			case FormType.ExportReportCurveTitle:
				IconVisible(false, true, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = MultiLanguage.GetStr(this, "lab_HanderTitle6");
				break;
			default:
				IconVisible(false, false, false, true);
				InputTitleTB.Text = currentDate.ToString("yyyyMMdd");
				lab_HanderTitle.Text = "";
				break;
			}
		}

		private void IconVisible(bool IsParamGB, bool IsReportGB, bool IsAllReportGB, bool IsInputTitleTB)
		{
			ParamGB.Visible = IsParamGB;
			ReportGB.Visible = IsReportGB;
			Report3CB.Visible = IsAllReportGB;
			InputTitleTB.Enabled = IsInputTitleTB;
		}

		private void Form997_ExportTitle_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateID != null)
			{
				this.CreateID(InputTitleTB.Text);
			}
			if (this.CreateParam != null)
			{
				int RetVal = 0;
				if (RB_ParamVer0.Checked)
				{
					RetVal |= 1;
				}
				if (RB_ParamVer1.Checked)
				{
					RetVal |= 2;
				}
				this.CreateParam(InputTitleTB.Text, RetVal);
			}
			if (this.CreateReport != null)
			{
				uint RetVal2 = 0u;
				if (Report1CB.Checked)
				{
					RetVal2 |= 1;
				}
				if (Report2CB.Checked)
				{
					RetVal2 |= 2;
				}
				if (Report3CB.Checked)
				{
					RetVal2 |= 4;
				}
				this.CreateReport(InputTitleTB.Text, RetVal2, 0u);
			}
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateID != null)
			{
				this.CreateID("Cancel_Message");
			}
			if (this.CreateParam != null)
			{
				this.CreateParam("Cancel_Message", 0);
			}
			if (this.CreateReport != null)
			{
				this.CreateReport("Cancel_Message", 0u, 0u);
			}
		}

		private void Form997_ExportTitle_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form997_ExportTitle));
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_OK = new System.Windows.Forms.Button();
			this.InputTitleTB = new System.Windows.Forms.TextBox();
			this.RB_ParamVer0 = new System.Windows.Forms.RadioButton();
			this.RB_ParamVer1 = new System.Windows.Forms.RadioButton();
			this.ParamGB = new System.Windows.Forms.GroupBox();
			this.ReportGB = new System.Windows.Forms.GroupBox();
			this.Report3CB = new System.Windows.Forms.CheckBox();
			this.Report2CB = new System.Windows.Forms.CheckBox();
			this.Report1CB = new System.Windows.Forms.CheckBox();
			this.ParamGB.SuspendLayout();
			this.ReportGB.SuspendLayout();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(500, 35);
			this.lab_HanderTitle.TabIndex = 56;
			this.lab_HanderTitle.Text = "Input Title";
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btn_Cancel.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Cancel.BackgroundImage");
			this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Cancel.FlatAppearance.BorderSize = 0;
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Cancel.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Cancel.Location = new System.Drawing.Point(287, 242);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(92, 30);
			this.btn_Cancel.TabIndex = 160;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(101, 242);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 159;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_OK_Click);
			this.InputTitleTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.InputTitleTB.Location = new System.Drawing.Point(54, 84);
			this.InputTitleTB.Name = "InputTitleTB";
			this.InputTitleTB.Size = new System.Drawing.Size(375, 27);
			this.InputTitleTB.TabIndex = 158;
			this.InputTitleTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.RB_ParamVer0.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.RB_ParamVer0.Location = new System.Drawing.Point(6, 12);
			this.RB_ParamVer0.Name = "RB_ParamVer0";
			this.RB_ParamVer0.Size = new System.Drawing.Size(362, 28);
			this.RB_ParamVer0.TabIndex = 161;
			this.RB_ParamVer0.Text = "Ver0, Torque value output is Nm unit";
			this.RB_ParamVer0.UseMnemonic = false;
			this.RB_ParamVer0.UseVisualStyleBackColor = true;
			this.RB_ParamVer1.Checked = true;
			this.RB_ParamVer1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.RB_ParamVer1.Location = new System.Drawing.Point(6, 42);
			this.RB_ParamVer1.Name = "RB_ParamVer1";
			this.RB_ParamVer1.Size = new System.Drawing.Size(362, 27);
			this.RB_ParamVer1.TabIndex = 161;
			this.RB_ParamVer1.TabStop = true;
			this.RB_ParamVer1.Text = "Ver1, Torque value output is user-defined";
			this.RB_ParamVer1.UseMnemonic = false;
			this.RB_ParamVer1.UseVisualStyleBackColor = true;
			this.ParamGB.Controls.Add(this.RB_ParamVer1);
			this.ParamGB.Controls.Add(this.RB_ParamVer0);
			this.ParamGB.Location = new System.Drawing.Point(54, 111);
			this.ParamGB.Name = "ParamGB";
			this.ParamGB.Size = new System.Drawing.Size(374, 76);
			this.ParamGB.TabIndex = 162;
			this.ParamGB.TabStop = false;
			this.ReportGB.Controls.Add(this.Report3CB);
			this.ReportGB.Controls.Add(this.Report2CB);
			this.ReportGB.Controls.Add(this.Report1CB);
			this.ReportGB.Location = new System.Drawing.Point(12, 117);
			this.ReportGB.Name = "ReportGB";
			this.ReportGB.Size = new System.Drawing.Size(476, 113);
			this.ReportGB.TabIndex = 162;
			this.ReportGB.TabStop = false;
			this.Report3CB.AutoSize = true;
			this.Report3CB.Location = new System.Drawing.Point(27, 89);
			this.Report3CB.Name = "Report3CB";
			this.Report3CB.Size = new System.Drawing.Size(176, 19);
			this.Report3CB.TabIndex = 0;
			this.Report3CB.Text = "Results Info + Scales Info";
			this.Report3CB.UseVisualStyleBackColor = true;
			this.Report2CB.Location = new System.Drawing.Point(27, 43);
			this.Report2CB.Name = "Report2CB";
			this.Report2CB.Size = new System.Drawing.Size(412, 41);
			this.Report2CB.TabIndex = 0;
			this.Report2CB.Text = "\"Seq ID\", \"Param ID\", \"Current Status\", \"Torque Unit\",\"User ID\" output as Ascii ";
			this.Report2CB.UseVisualStyleBackColor = true;
			this.Report1CB.AutoSize = true;
			this.Report1CB.Location = new System.Drawing.Point(27, 19);
			this.Report1CB.Name = "Report1CB";
			this.Report1CB.Size = new System.Drawing.Size(244, 19);
			this.Report1CB.TabIndex = 0;
			this.Report1CB.Text = "\"Seq ID\", \"Param ID\" output as Ascii";
			this.Report1CB.UseVisualStyleBackColor = true;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 295);
			base.Controls.Add(this.ReportGB);
			base.Controls.Add(this.ParamGB);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.InputTitleTB);
			base.Controls.Add(this.lab_HanderTitle);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form997_ExportTitle";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form997_ExportTitle_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form997_ExportTitle_Paint);
			this.ParamGB.ResumeLayout(false);
			this.ReportGB.ResumeLayout(false);
			this.ReportGB.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
