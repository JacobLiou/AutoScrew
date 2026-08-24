using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form996_JumpConfirmYesNo : Form
	{
		public FormType FormTypeNum;

		public GlobalVar GB;

		private IContainer components = null;

		private Button btn_Cancel;

		private Button btn_OK;

		private Label lab_Title;

		public event CreateForm996_YesHandler CreateYesAns;

		public event CreateForm996_NoHandler CreateNoAns;

		public Form996_JumpConfirmYesNo(GlobalVar GB)
		{
			InitializeComponent();
			this.GB = GB;
			MultiLanguage.LoadLanguage(this);
			lab_Title.ForeColor = Color.White;
		}

		public void SetSubForm(FormType FormNum)
		{
			FormTypeNum = FormNum;
			switch (FormNum)
			{
			case FormType.MegParamNonSave:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegParamNonSave");
				break;
			case FormType.MegParamDel:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegParamDel");
				break;
			case FormType.MegParamWriteAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegParamUpload");
				break;
			case FormType.MegParamReadAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegParamDownload");
				break;
			case FormType.MegSeqNonSave:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegSeqNonSave");
				break;
			case FormType.MegSeqDel:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegSeqDel");
				break;
			case FormType.MegSeqWriteAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegSeqUpload");
				break;
			case FormType.MegSeqReadAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegSeqDownload");
				break;
			case FormType.MegSrcWriteAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegSrcUpload");
				break;
			case FormType.MegSrcReadAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegSrcDownload");
				break;
			case FormType.MegResultResetProcess:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegResultResetProcess");
				break;
			case FormType.MegCtrlWriteAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegCtrlUpload");
				break;
			case FormType.MegCtrlReadAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegCtrlDownload");
				break;
			case FormType.MegCtrlCurveFrq:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegAllReportDel");
				break;
			case FormType.MegToolSensitivityFactory:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegToolSensitivityFactory");
				break;
			case FormType.MegToolWriteAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegToolUpload");
				break;
			case FormType.MegToolReadAll:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegToolDownload");
				break;
			case FormType.MegAllReportDel:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegAllReportDel");
				break;
			case FormType.MegReportFileDel:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegReportFileDel");
				break;
			case FormType.MegErrorReportDel:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegErrorDel");
				break;
			case FormType.MegWarningReportDel:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegWarningDel");
				break;
			case FormType.MegDownloadNewFW:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_MegDownloadNewFW");
				break;
			default:
				lab_Title.Text = "";
				break;
			}
		}

		private void btn_Yes_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateYesAns != null)
			{
				this.CreateYesAns();
			}
		}

		private void btn_No_Click(object sender, EventArgs e)
		{
			Close();
			if (this.CreateNoAns != null)
			{
				this.CreateNoAns();
			}
		}

		private void Form996_JumpConfirmYesNo_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form996_JumpConfirmYesNo_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form996_JumpConfirmYesNo));
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_OK = new System.Windows.Forms.Button();
			this.lab_Title = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.btn_Cancel.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Cancel.BackgroundImage");
			this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Cancel.FlatAppearance.BorderSize = 0;
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Cancel.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Cancel.Location = new System.Drawing.Point(293, 157);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(92, 30);
			this.btn_Cancel.TabIndex = 63;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(btn_No_Click);
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(106, 157);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 62;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_Yes_Click);
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Title.ForeColor = System.Drawing.Color.White;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, 0);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(500, 86);
			this.lab_Title.TabIndex = 69;
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 225);
			base.Controls.Add(this.lab_Title);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form996_JumpConfirmYesNo";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form996_JumpConfirmYesNo_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form996_JumpConfirmYesNo_Paint);
			base.ResumeLayout(false);
		}
	}
}
