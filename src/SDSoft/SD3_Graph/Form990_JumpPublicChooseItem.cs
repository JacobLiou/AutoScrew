using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form990_JumpPublicChooseItem : Form
	{
		public DataTable dt_ChooseItem = new DataTable();

		private Image[] AxisChooseImg = new Image[2];

		public FormType FormTypeNum;

		private GlobalVar GB;

		private uint Page_Axis = 0u;

		private bool First = false;

		private FormType AssignedMode = FormType.None;

		private IContainer components = null;

		private DataGridView dataGridView_ChooseItem;

		private Button btn_Cancel;

		private Label lab_HanderTitle;

		private Button SeqBn;

		private Button ParamBn;

		private Button AxisY_Bn;

		private Button AxisX_Bn;

		public event CreateForm990_ChooseHandler CreateChooseItem;

		public event CreateForm990_ChooseSeqParamSeqHandler CreateChooseSeqParamSeqItem;

		public event CreateForm990_ChooseSrcParamSeqHandler CreateChooseSrcParamSeqItem;

		public event CreateForm990_ChooseSrcAllParamSeqHandler CreateChooseSrcAllParamSeqItem;

		public event CreateForm990_ChooseCtrlHandler CreateChooseCtrlItem;

		public Form990_JumpPublicChooseItem(int Axis, GlobalVar GB)
		{
			InitializeComponent();
			this.GB = GB;
			Page_Axis = (uint)Axis;
			MultiLanguage.LoadLanguage(this);
			dataGridView_ChooseItem.MouseClick += dataGridView_ChooseItem_MouseClick;
			dataGridView_ChooseItem.MouseDoubleClick += dataGridView_ChooseItem_MouseClick;
			AxisChooseImg[0] = Resources.GrayButton;
			AxisChooseImg[1] = Resources.BlueButton;
			AxisX_Bn.Visible = false;
			AxisY_Bn.Visible = false;
			First = true;
			dt_ChooseItem.Columns.Add("ID", typeof(int));
			dt_ChooseItem.Columns.Add("Item", typeof(string));
			ParamBn.Text = MultiLanguage.GetStr("Form_001Main", "ParamBn");
			SeqBn.Text = MultiLanguage.GetStr("Form_001Main", "SeqBn");
			AxisX_Bn.Text = MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ToolX");
			AxisY_Bn.Text = MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ToolY");
			dataGridView_ChooseItem.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.CreateChooseItem = null;
			this.CreateChooseSeqParamSeqItem = null;
			this.CreateChooseSrcParamSeqItem = null;
			this.CreateChooseSrcAllParamSeqItem = null;
			this.CreateChooseCtrlItem = null;
			Close();
		}

		private void ParamSeqItem(ref string[] ChooseItemStr, FormType FormNum)
		{
			AssignedMode = FormNum;
			switch (FormNum)
			{
			case FormType.Param:
			{
				ParamBn.BackColor = SystemColors.GradientInactiveCaption;
				SeqBn.BackColor = SystemColors.AppWorkspace;
				ChooseItemStr = new string[500];
				for (int j = 0; j < 500; j++)
				{
					if (Page_Axis == 0)
					{
						ChooseItemStr[j] = GB.GetNameTitleStr(FormType.ParamX, j);
					}
					else
					{
						ChooseItemStr[j] = GB.GetNameTitleStr(FormType.ParamY, j);
					}
				}
				break;
			}
			case FormType.Seq:
			{
				ParamBn.BackColor = SystemColors.AppWorkspace;
				SeqBn.BackColor = SystemColors.GradientInactiveCaption;
				ChooseItemStr = new string[500];
				for (int i = 0; i < 500; i++)
				{
					ChooseItemStr[i] = GB.GetNameTitleStr(FormType.Seq, i);
				}
				break;
			}
			}
		}

		private void AllSeqParamItem(ref string[] ChooseItemStr, FormType FormNum, uint Axis)
		{
			AssignedMode = FormNum;
			Page_Axis = Axis;
			ParamBn.Visible = true;
			SeqBn.Visible = true;
			AxisX_Bn.Visible = true;
			AxisY_Bn.Visible = true;
			switch (FormNum)
			{
			case FormType.Param:
			{
				ParamBn.BackColor = SystemColors.GradientInactiveCaption;
				SeqBn.BackColor = SystemColors.AppWorkspace;
				ChooseItemStr = new string[500];
				for (int j = 0; j < 500; j++)
				{
					if (Page_Axis == 0)
					{
						ChooseItemStr[j] = GB.GetNameTitleStr(FormType.ParamX, j);
					}
					else
					{
						ChooseItemStr[j] = GB.GetNameTitleStr(FormType.ParamY, j);
					}
				}
				break;
			}
			case FormType.Seq:
			{
				ParamBn.BackColor = SystemColors.AppWorkspace;
				SeqBn.BackColor = SystemColors.GradientInactiveCaption;
				ChooseItemStr = new string[500];
				for (int i = 0; i < 500; i++)
				{
					ChooseItemStr[i] = GB.GetNameTitleStr(FormType.Seq, i);
				}
				break;
			}
			}
		}

		public void SetSubForm(FormType FormNum)
		{
			bool ShowJmp = true;
			FormTypeNum = FormNum;
			string[] ChooseItemStr = null;
			ParamBn.Visible = false;
			SeqBn.Visible = false;
			switch (FormNum)
			{
			case FormType.ChooseParamStage:
				ChooseItemStr = ((GB.UISys.SpecCtrl != 1) ? ((GB.CheckHMIVer(172, 10) && GB.FSModelTypeInfo.VerMotionFW >= 374) ? new string[7]
				{
					MultiLanguage.GetStr("Form100_Param", "tp_Angle"),
					MultiLanguage.GetStr("Form100_Param", "tp_Torque"),
					MultiLanguage.GetStr("Form100_Param", "tp_TorqueRate"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampTorque"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampAngle"),
					MultiLanguage.GetStr("Form100_Param", "tp_Yield"),
					MultiLanguage.GetStr("Form100_Param", "tp_AngOrTorq")
				} : ((!GB.CheckHMIVer(169, 0) || GB.FSModelTypeInfo.VerMotionFW < 257) ? new string[5]
				{
					MultiLanguage.GetStr("Form100_Param", "tp_Angle"),
					MultiLanguage.GetStr("Form100_Param", "tp_Torque"),
					MultiLanguage.GetStr("Form100_Param", "tp_TorqueRate"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampTorque"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampAngle")
				} : new string[6]
				{
					MultiLanguage.GetStr("Form100_Param", "tp_Angle"),
					MultiLanguage.GetStr("Form100_Param", "tp_Torque"),
					MultiLanguage.GetStr("Form100_Param", "tp_TorqueRate"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampTorque"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampAngle"),
					MultiLanguage.GetStr("Form100_Param", "tp_Yield")
				})) : new string[4]
				{
					MultiLanguage.GetStr("Form100_Param", "tp_Angle"),
					MultiLanguage.GetStr("Form100_Param", "tp_Torque"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampTorque"),
					MultiLanguage.GetStr("Form100_Param", "tp_ClampAngle")
				});
				break;
			case FormType.ChooseSeqSubParam:
				if (First)
				{
					Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
					AxisX_Bn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
					AxisY_Bn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
					PageAxisButton(Page_Axis);
					First = false;
				}
				ChooseItemStr = new string[500];
				ParamSeqItem(ref ChooseItemStr, FormType.Param);
				break;
			case FormType.ChooseSrcOfSeqParam:
			case FormType.ChooseSrcOfBitSeqParam:
			{
				Button paramBn = ParamBn;
				bool visible = (SeqBn.Visible = true);
				paramBn.Visible = visible;
				ChooseItemStr = new string[500];
				ParamSeqItem(ref ChooseItemStr, FormType.Param);
				break;
			}
			case FormType.ChooseSrcOfMixSeq:
				ChooseItemStr = new string[500];
				ParamSeqItem(ref ChooseItemStr, FormType.Seq);
				break;
			case FormType.ChooseSrcOfAllSeqParam:
			case FormType.ChooseSrcOfAllBitSeqParam:
			{
				if (First)
				{
					Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
					AxisX_Bn.Visible = GB.UISys.PageAxisInfo.Tool1Visable;
					AxisY_Bn.Visible = GB.UISys.PageAxisInfo.Tool2Visable;
					PageAxisButton(Page_Axis);
					First = false;
				}
				Button paramBn2 = ParamBn;
				bool visible = (SeqBn.Visible = true);
				paramBn2.Visible = visible;
				ChooseItemStr = new string[500];
				ParamSeqItem(ref ChooseItemStr, FormType.Param);
				break;
			}
			case FormType.ChooseCtrlDefaultTorque:
				ChooseItemStr = new string[7]
				{
					MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit0"),
					MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit1"),
					MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit2"),
					MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit3"),
					MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit4"),
					MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit5"),
					MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit6")
				};
				break;
			case FormType.ChooseCtrlDefaultAngle:
				ChooseItemStr = new string[2]
				{
					MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit_0"),
					MultiLanguage.GetStr("Form500_Controller", "tp_AngleUnit_1")
				};
				break;
			case FormType.ChooseCtrlDefaultStartCondition_Normal:
				ChooseItemStr = new string[7]
				{
					MultiLanguage.GetStr("Form500_Controller", "tp_StartType1"),
					MultiLanguage.GetStr("Form500_Controller", "tp_StartType2"),
					MultiLanguage.GetStr("Form500_Controller", "tp_StartType3"),
					MultiLanguage.GetStr("Form500_Controller", "tp_StartType4"),
					MultiLanguage.GetStr("Form500_Controller", "tp_StartType5"),
					null,
					null
				};
				if (GB.CheckHMIVer(169, 6))
				{
					ChooseItemStr[3] = MultiLanguage.GetStr("Form500_Controller", "tp_StartType6");
					ChooseItemStr[4] = MultiLanguage.GetStr("Form500_Controller", "tp_StartType7");
				}
				break;
			case FormType.ChooseCtrlDefaultStartCondition_NonPush:
				ChooseItemStr = new string[7]
				{
					MultiLanguage.GetStr("Form500_Controller", "tp_StartType3"),
					MultiLanguage.GetStr("Form500_Controller", "tp_StartType2"),
					null,
					null,
					null,
					null,
					null
				};
				if (GB.CheckHMIVer(169, 6))
				{
					ChooseItemStr[2] = MultiLanguage.GetStr("Form500_Controller", "tp_StartType7");
				}
				break;
			case FormType.ChooseReportAngleList:
				ChooseItemStr = new string[11]
				{
					MultiLanguage.GetStr("Form700_Report", "tp_AngType0"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType1"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType2"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType3"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType4"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType5"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType6"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType7"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType8"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType9"),
					MultiLanguage.GetStr("Form700_Report", "tp_AngType10")
				};
				break;
			case FormType.ChooseReportTorqueList:
				ChooseItemStr = new string[11]
				{
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType0"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType1"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType2"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType3"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType4"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType5"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType6"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType7"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType8"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType9"),
					MultiLanguage.GetStr("Form700_Report", "tp_TorqType10")
				};
				break;
			default:
				ShowJmp = false;
				break;
			}
			if (ShowJmp)
			{
				UpdateDataGrid(ref ChooseItemStr);
			}
		}

		private unsafe void UpdateDataGrid(ref string[] ChooseItemStr)
		{
			dataGridView_ChooseItem.DataSource = null;
			LoadPage loadPage = new LoadPage(ChooseItemStr.Length, 10);
			dt_ChooseItem.Rows.Clear();
			for (int i = 0; i < ChooseItemStr.Length; i++)
			{
				if (!string.IsNullOrEmpty(ChooseItemStr[i]))
				{
					bool Enable = false;
					if ((FormTypeNum == FormType.ChooseSrcOfSeqParam) ? (AssignedMode == FormType.Param || (AssignedMode == FormType.Seq && GB.ExFSSeq.EnableMode[i] % 10 == Page_Axis + 1)) : ((FormTypeNum == FormType.ChooseSrcOfBitSeqParam) ? (AssignedMode == FormType.Param || (AssignedMode == FormType.Seq && GB.ExFSSeq.EnableMode[i] == Page_Axis + 1 + 10)) : ((FormTypeNum == FormType.ChooseSrcOfMixSeq) ? (AssignedMode == FormType.Seq && GB.ExFSSeq.EnableMode[i] % 10 == 3) : ((FormTypeNum == FormType.ChooseSrcOfAllSeqParam) ? (AssignedMode == FormType.Param || (AssignedMode == FormType.Seq && GB.ExFSSeq.EnableMode[i] % 10 == Page_Axis + 1)) : (FormTypeNum != FormType.ChooseSrcOfAllBitSeqParam || (AssignedMode == FormType.Seq && GB.ExFSSeq.EnableMode[i] % 10 == 3))))))
					{
						DataRow row = dt_ChooseItem.NewRow();
						row[0] = i + 1;
						row[1] = ChooseItemStr[i];
						dt_ChooseItem.Rows.Add(row);
					}
				}
			}
			dataGridView_ChooseItem.DataSource = dt_ChooseItem;
			loadGrid1(dataGridView_ChooseItem);
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.ScrollBars = ScrollBars.Vertical;
			dataGridView1.Columns[0].HeaderText = "▼";
			dataGridView1.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[0].FillWeight = 20f;
			dataGridView1.Columns[1].FillWeight = 80f;
		}

		private void dataGridView_ChooseItem_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = dataGridView_ChooseItem.HitTest(e.X, e.Y).RowIndex;
			if (currentMouseOverRow < 0 || dataGridView_ChooseItem.Rows.Count <= 0)
			{
				return;
			}
			dataGridView_ChooseItem.ClearSelection();
			dataGridView_ChooseItem.Rows[currentMouseOverRow].Selected = true;
			if (this.CreateChooseItem != null)
			{
				if (GB.UISys.SpecCtrl == 1 && FormTypeNum == FormType.ChooseParamStage)
				{
					uint RowNo = Convert.ToUInt16(dataGridView_ChooseItem.Rows[currentMouseOverRow].Cells["ID"].Value);
					switch (RowNo)
					{
					case 1u:
						RowNo = 1u;
						break;
					case 2u:
						RowNo = 2u;
						break;
					case 3u:
						RowNo = 4u;
						break;
					case 4u:
						RowNo = 5u;
						break;
					}
					this.CreateChooseItem((int)Page_Axis, (int)(RowNo - 1));
				}
				else
				{
					uint RowNo2 = Convert.ToUInt16(dataGridView_ChooseItem.Rows[currentMouseOverRow].Cells["ID"].Value);
					this.CreateChooseItem((int)Page_Axis, (int)(RowNo2 - 1));
				}
			}
			if (this.CreateChooseSeqParamSeqItem != null)
			{
				uint RowNo3 = Convert.ToUInt16(dataGridView_ChooseItem.Rows[currentMouseOverRow].Cells["ID"].Value);
				this.CreateChooseSeqParamSeqItem(Page_Axis, (int)(RowNo3 - 1));
			}
			if (this.CreateChooseSrcParamSeqItem != null)
			{
				uint RowNo4 = Convert.ToUInt16(dataGridView_ChooseItem.Rows[currentMouseOverRow].Cells["ID"].Value);
				this.CreateChooseSrcParamSeqItem(AssignedMode, (int)(RowNo4 - 1));
			}
			if (this.CreateChooseSrcAllParamSeqItem != null)
			{
				uint RowNo5 = Convert.ToUInt16(dataGridView_ChooseItem.Rows[currentMouseOverRow].Cells["ID"].Value);
				this.CreateChooseSrcAllParamSeqItem(Page_Axis, AssignedMode, (int)(RowNo5 - 1));
			}
			if (this.CreateChooseCtrlItem != null)
			{
				uint RowNo6 = Convert.ToUInt16(dataGridView_ChooseItem.Rows[currentMouseOverRow].Cells["ID"].Value);
				this.CreateChooseCtrlItem((ushort)(RowNo6 - 1));
			}
			Close();
		}

		private void Form990_JumpPublicChooseItem_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void ParamBn_Click(object sender, EventArgs e)
		{
			string[] ChooseItemStr = null;
			ParamSeqItem(ref ChooseItemStr, FormType.Param);
			UpdateDataGrid(ref ChooseItemStr);
		}

		private void SeqBn_Click(object sender, EventArgs e)
		{
			string[] ChooseItemStr = null;
			ParamSeqItem(ref ChooseItemStr, FormType.Seq);
			UpdateDataGrid(ref ChooseItemStr);
		}

		private void AxisX_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 0u;
			PageAxisButton(Page_Axis);
			SetSubForm(FormTypeNum);
		}

		private void AxisY_Bn_Click(object sender, EventArgs e)
		{
			Page_Axis = 1u;
			PageAxisButton(Page_Axis);
			SetSubForm(FormTypeNum);
		}

		private void PageAxisButton(uint Page_Axis)
		{
			GB.UISys.ParamPageAxis = (int)Page_Axis;
			if (Page_Axis == 0)
			{
				ShowOnOffBtn(1u, AxisX_Bn, AxisChooseImg);
				ShowOnOffBtn(0u, AxisY_Bn, AxisChooseImg);
			}
			else
			{
				ShowOnOffBtn(0u, AxisX_Bn, AxisChooseImg);
				ShowOnOffBtn(1u, AxisY_Bn, AxisChooseImg);
			}
		}

		private void ShowOnOffBtn(uint val, Button Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
		}

		private void Form990_JumpPublicChooseItem_Load(object sender, EventArgs e)
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form990_JumpPublicChooseItem));
			this.dataGridView_ChooseItem = new System.Windows.Forms.DataGridView();
			this.btn_Cancel = new System.Windows.Forms.Button();
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.SeqBn = new System.Windows.Forms.Button();
			this.ParamBn = new System.Windows.Forms.Button();
			this.AxisY_Bn = new System.Windows.Forms.Button();
			this.AxisX_Bn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ChooseItem).BeginInit();
			base.SuspendLayout();
			this.dataGridView_ChooseItem.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_ChooseItem.Location = new System.Drawing.Point(56, 108);
			this.dataGridView_ChooseItem.Name = "dataGridView_ChooseItem";
			this.dataGridView_ChooseItem.ReadOnly = true;
			this.dataGridView_ChooseItem.RowHeadersVisible = false;
			this.dataGridView_ChooseItem.RowHeadersWidth = 51;
			this.dataGridView_ChooseItem.RowTemplate.Height = 24;
			this.dataGridView_ChooseItem.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.dataGridView_ChooseItem.Size = new System.Drawing.Size(370, 493);
			this.dataGridView_ChooseItem.TabIndex = 1;
			this.btn_Cancel.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_Cancel.BackgroundImage");
			this.btn_Cancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_Cancel.FlatAppearance.BorderSize = 0;
			this.btn_Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_Cancel.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_Cancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_Cancel.Location = new System.Drawing.Point(202, 615);
			this.btn_Cancel.Name = "btn_Cancel";
			this.btn_Cancel.Size = new System.Drawing.Size(92, 30);
			this.btn_Cancel.TabIndex = 64;
			this.btn_Cancel.Text = "Cancel";
			this.btn_Cancel.UseVisualStyleBackColor = true;
			this.btn_Cancel.Click += new System.EventHandler(btn_Cancel_Click);
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(0, 1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(500, 35);
			this.lab_HanderTitle.TabIndex = 66;
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.SeqBn.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.SeqBn.FlatAppearance.BorderSize = 0;
			this.SeqBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SeqBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.SeqBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SeqBn.Location = new System.Drawing.Point(241, 77);
			this.SeqBn.Name = "SeqBn";
			this.SeqBn.Size = new System.Drawing.Size(185, 30);
			this.SeqBn.TabIndex = 67;
			this.SeqBn.UseVisualStyleBackColor = false;
			this.SeqBn.Click += new System.EventHandler(SeqBn_Click);
			this.ParamBn.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.ParamBn.FlatAppearance.BorderSize = 0;
			this.ParamBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ParamBn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.ParamBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ParamBn.Location = new System.Drawing.Point(56, 77);
			this.ParamBn.Name = "ParamBn";
			this.ParamBn.Size = new System.Drawing.Size(185, 30);
			this.ParamBn.TabIndex = 68;
			this.ParamBn.UseVisualStyleBackColor = false;
			this.ParamBn.Click += new System.EventHandler(ParamBn_Click);
			this.AxisY_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisY_Bn.FlatAppearance.BorderSize = 0;
			this.AxisY_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisY_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisY_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisY_Bn.Location = new System.Drawing.Point(241, 45);
			this.AxisY_Bn.Name = "AxisY_Bn";
			this.AxisY_Bn.Size = new System.Drawing.Size(185, 33);
			this.AxisY_Bn.TabIndex = 159;
			this.AxisY_Bn.Text = "Tool2";
			this.AxisY_Bn.UseVisualStyleBackColor = false;
			this.AxisY_Bn.Click += new System.EventHandler(AxisY_Bn_Click);
			this.AxisX_Bn.BackColor = System.Drawing.SystemColors.ControlLight;
			this.AxisX_Bn.FlatAppearance.BorderSize = 0;
			this.AxisX_Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.AxisX_Bn.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold);
			this.AxisX_Bn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.AxisX_Bn.Location = new System.Drawing.Point(56, 45);
			this.AxisX_Bn.Name = "AxisX_Bn";
			this.AxisX_Bn.Size = new System.Drawing.Size(185, 33);
			this.AxisX_Bn.TabIndex = 160;
			this.AxisX_Bn.Text = "Tool1";
			this.AxisX_Bn.UseVisualStyleBackColor = false;
			this.AxisX_Bn.Click += new System.EventHandler(AxisX_Bn_Click);
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 680);
			base.Controls.Add(this.SeqBn);
			base.Controls.Add(this.ParamBn);
			base.Controls.Add(this.lab_HanderTitle);
			base.Controls.Add(this.btn_Cancel);
			base.Controls.Add(this.dataGridView_ChooseItem);
			base.Controls.Add(this.AxisY_Bn);
			base.Controls.Add(this.AxisX_Bn);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form990_JumpPublicChooseItem";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form990_JumpPublicChooseItem_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form990_JumpPublicChooseItem_Paint);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ChooseItem).EndInit();
			base.ResumeLayout(false);
		}
	}
}
