using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form105_Create : Form
	{
		public FormType FormTypeNum;

		private UI105 CaheVal = default(UI105);

		private Image[] OffOnImg = new Image[2];

		private GlobalVar GB = null;

		private IContainer components = null;

		private Label lab_InputTitle;

		private TextBox tb_CreateTitle;

		private Label label1;

		private TextBox tb_CreateID;

		private Button btn_OK;

		private Button btn_Cancel;

		private CheckBox QuickStartBn;

		private GroupBox gb_SetParam;

		private TextBox QuickStartTB;

		private Label lab_QuickStart;

		private Label lab_TighteningTorque;

		private Label lab_TorqUnit1;

		private Label lab_HanderTitle;

		public event CreateForm105_Handler CreateID;

		public Form105_Create(GlobalVar GB, int Axis)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			CaheVal.QuickStartSW = true;
			ToolTip toolTip = new ToolTip
			{
				AutoPopDelay = 3000,
				InitialDelay = 5
			};
			if (Axis == 0)
			{
				CaheVal.TargetTorqueWatch = (uint)((double)(float)(int)GB.UISys.ToolSetTorqueFW_X * GB.TorqUnitcoef(2 + GB.UISys.ParamPageAxis) / GB.TorqUnitcoef(1000 + GB.FSModelTypeInfo.MesRawDataTorqUint));
			}
			else
			{
				CaheVal.TargetTorqueWatch = (uint)((double)(float)(int)GB.UISys.ToolSetTorqueFW_Y * GB.TorqUnitcoef(2 + GB.UISys.ParamPageAxis) / GB.TorqUnitcoef(1000 + GB.FSModelTypeInfo.MesRawDataTorqUint));
			}
			tb_CreateTitle.Multiline = false;
			tb_CreateTitle.ShortcutsEnabled = false;
			tb_CreateTitle.KeyPress += GB.RangeASCIIInput;
			OffOnImg[0] = Resources.OFF_ICON;
			OffOnImg[1] = Resources.ON_ICON;
			QuickStartTB.KeyPress += GB.RangeToolTorque_000;
			QuickStartTB.LostFocus += GB.LostFocus_C3;
			toolTip.SetToolTip(QuickStartTB, GB.UISys.RangeStr + "0.000-" + GB.ToolTorqueWatchUnit().ToString("F3"));
			QuickStartBn.Click += Button_Click;
			GetFSParamToMessage();
			ShowTargetTorq(CaheVal.QuickStartSW);
			ShowTorqUnitText();
		}

		public void GetFSParamToMessage()
		{
			ShowOnOffBtn(CaheVal.QuickStartSW, QuickStartBn, OffOnImg);
			QuickStartTB.Text = (GB.Round(CaheVal.TargetTorqueWatch, 1) / 1000.0).ToString("F3");
		}

		private void ShowTorqUnitText()
		{
			string TorqStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + GB.UISys.ParmShowTorqueUnit);
			string TorqRateStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqRateUnit" + GB.UISys.ParmShowTorqueUnit);
			string AngStr = MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit" + GB.FSCtrlAngleUnit.Mode);
			lab_TorqUnit1.Text = TorqStr;
		}

		public void SetSubForm(UI105 id, bool Enable_Param, FormType FormNum)
		{
			lab_HanderTitle.Text = id.ShowHeaderTitle;
			tb_CreateID.Text = id.IDNum.ToString();
			tb_CreateTitle.Text = id.Title;
			FormTypeNum = FormNum;
			gb_SetParam.Visible = Enable_Param;
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.CreateID != null)
			{
				switch (FormTypeNum)
				{
				case FormType.Param:
					CaheVal.IDNum = Convert.ToInt32(tb_CreateID.Text);
					CaheVal.Title = tb_CreateTitle.Text;
					CaheVal.TargetTorqueWatch = (uint)(float.Parse(QuickStartTB.Text) * 1000f);
					this.CreateID(CaheVal);
					break;
				case FormType.Seq:
					CaheVal.IDNum = Convert.ToInt32(tb_CreateID.Text);
					CaheVal.Title = tb_CreateTitle.Text;
					this.CreateID(CaheVal);
					break;
				}
			}
			Close();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Button_Click(object sender, EventArgs e)
		{
			ref bool quickStartSW = ref CaheVal.QuickStartSW;
			quickStartSW = !quickStartSW;
			ShowOnOffBtn(CaheVal.QuickStartSW, QuickStartBn, OffOnImg);
			ShowTargetTorq(CaheVal.QuickStartSW);
		}

		private void Form105_Create_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void ShowTargetTorq(bool val)
		{
			lab_TighteningTorque.Visible = val;
			QuickStartTB.Visible = val;
			lab_TorqUnit1.Visible = val;
		}

		private void ShowOnOffBtn(bool val, CheckBox Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((!val) ? Img[0] : Img[1]);
		}

		private void Form105_Create_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form105_Create));
			this.lab_InputTitle = new System.Windows.Forms.Label();
			this.tb_CreateTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tb_CreateID = new System.Windows.Forms.TextBox();
			this.gb_SetParam = new System.Windows.Forms.GroupBox();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.QuickStartTB = new System.Windows.Forms.TextBox();
			this.lab_QuickStart = new System.Windows.Forms.Label();
			this.lab_TighteningTorque = new System.Windows.Forms.Label();
			this.QuickStartBn = new System.Windows.Forms.CheckBox();
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.btn_OK = new System.Windows.Forms.Button();
			this.gb_SetParam.SuspendLayout();
			base.SuspendLayout();
			this.lab_InputTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_InputTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_InputTitle.Location = new System.Drawing.Point(90, 68);
			this.lab_InputTitle.Name = "lab_InputTitle";
			this.lab_InputTitle.Size = new System.Drawing.Size(353, 16);
			this.lab_InputTitle.TabIndex = 55;
			this.lab_InputTitle.Text = "Input Title";
			this.lab_InputTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.tb_CreateTitle.Font = new System.Drawing.Font("新細明體", 10f);
			this.tb_CreateTitle.Location = new System.Drawing.Point(93, 100);
			this.tb_CreateTitle.Name = "tb_CreateTitle";
			this.tb_CreateTitle.Size = new System.Drawing.Size(350, 27);
			this.tb_CreateTitle.TabIndex = 56;
			this.tb_CreateTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(31, 68);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(31, 20);
			this.label1.TabIndex = 58;
			this.label1.Text = "ID";
			this.tb_CreateID.Font = new System.Drawing.Font("新細明體", 10f);
			this.tb_CreateID.Location = new System.Drawing.Point(17, 97);
			this.tb_CreateID.Multiline = true;
			this.tb_CreateID.Name = "tb_CreateID";
			this.tb_CreateID.Size = new System.Drawing.Size(54, 25);
			this.tb_CreateID.TabIndex = 57;
			this.tb_CreateID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.gb_SetParam.Controls.Add(this.lab_TorqUnit1);
			this.gb_SetParam.Controls.Add(this.QuickStartTB);
			this.gb_SetParam.Controls.Add(this.lab_QuickStart);
			this.gb_SetParam.Controls.Add(this.lab_TighteningTorque);
			this.gb_SetParam.Controls.Add(this.QuickStartBn);
			this.gb_SetParam.Location = new System.Drawing.Point(93, 128);
			this.gb_SetParam.Name = "gb_SetParam";
			this.gb_SetParam.Size = new System.Drawing.Size(350, 81);
			this.gb_SetParam.TabIndex = 122;
			this.gb_SetParam.TabStop = false;
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(248, 52);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 122;
			this.lab_TorqUnit1.Text = "N.m";
			this.QuickStartTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.QuickStartTB.Location = new System.Drawing.Point(168, 47);
			this.QuickStartTB.Name = "QuickStartTB";
			this.QuickStartTB.Size = new System.Drawing.Size(74, 27);
			this.QuickStartTB.TabIndex = 121;
			this.QuickStartTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_QuickStart.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_QuickStart.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_QuickStart.Location = new System.Drawing.Point(27, 19);
			this.lab_QuickStart.Name = "lab_QuickStart";
			this.lab_QuickStart.Size = new System.Drawing.Size(140, 16);
			this.lab_QuickStart.TabIndex = 59;
			this.lab_QuickStart.Text = "Quick Start";
			this.lab_QuickStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TighteningTorque.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TighteningTorque.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TighteningTorque.Location = new System.Drawing.Point(27, 52);
			this.lab_TighteningTorque.Name = "lab_TighteningTorque";
			this.lab_TighteningTorque.Size = new System.Drawing.Size(140, 16);
			this.lab_TighteningTorque.TabIndex = 120;
			this.lab_TighteningTorque.Text = "Tightening Torque";
			this.lab_TighteningTorque.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.QuickStartBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.QuickStartBn.AutoCheck = false;
			this.QuickStartBn.BackgroundImage = SD3Soft.Properties.Resources.OFF_ICON;
			this.QuickStartBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.QuickStartBn.FlatAppearance.BorderSize = 0;
			this.QuickStartBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.QuickStartBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.QuickStartBn.Location = new System.Drawing.Point(173, 16);
			this.QuickStartBn.Name = "QuickStartBn";
			this.QuickStartBn.Size = new System.Drawing.Size(60, 25);
			this.QuickStartBn.TabIndex = 119;
			this.QuickStartBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.QuickStartBn.UseVisualStyleBackColor = true;
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, 0);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(500, 35);
			this.lab_HanderTitle.TabIndex = 55;
			this.lab_HanderTitle.Text = "Title";
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btn_Cancel.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Cancel.BackgroundImage");
			this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Cancel.FlatAppearance.BorderSize = 0;
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Cancel.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Cancel.Location = new System.Drawing.Point(295, 225);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(92, 30);
			this.btn_Cancel.TabIndex = 61;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(108, 225);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 60;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_OK_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 280);
			base.Controls.Add(this.gb_SetParam);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.tb_CreateID);
			base.Controls.Add(this.tb_CreateTitle);
			base.Controls.Add(this.lab_HanderTitle);
			base.Controls.Add(this.lab_InputTitle);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form105_Create";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form105_Create_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form105_Create_Paint);
			this.gb_SetParam.ResumeLayout(false);
			this.gb_SetParam.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
