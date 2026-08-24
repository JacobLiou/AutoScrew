using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form200_Seq : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		private UISeqStrc UI = default(UISeqStrc);

		public DataTable dt_Seq = new DataTable();

		public DataTable dt_SeqParam = new DataTable();

		private PictureBox GuidePicPB = new PictureBox();

		public static int Current_Index;

		public int stage_number = 0;

		public int stage_Index = 0;

		public UI105 CaheVal = default(UI105);

		private Image[] CircleImg = new Image[2];

		private Image[] ButtonImg = new Image[2];

		private Image[] OffOnImg = new Image[2];

		private int AssignedSubParamRow = 0;

		private int CaheRowIdx = 0;

		private int LastX = 0;

		private int LastY = 0;

		private uint Page_Axis = 0u;

		private float TextHScaleSize;

		private float TextWScaleSize;

		private Image[] GuideImgGP = new Image[30];

		private List<UISeqEachGuideStrc>[] SeqEachScrewList = new List<UISeqEachGuideStrc>[100];

		public Button[] COMbutton;

		private IContainer components = null;

		private DataGridView dataGridView_Seq;

		private TabControl tp_Seqence;

		private TabPage tpSeq_Normal;

		private DataGridView dataGridView_SeqParam;

		private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

		private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

		private TextBox tbSeqTitle;

		private Button btn_AddID;

		private Button btnDownload;

		private Button btnUpload;

		private Button btn_DelID;

		private Button btn_AddSubParam;

		private Button SaveBn;

		private TextBox tbCurrentID;

		private Button btn_ImportCSV;

		private Button btn_ExportCSV;

		private GroupBox groupBox1;

		private TabPage tpSeq_Navigator;

		private Label XY_lab;

		private Panel SeqPreGuidePL;

		private PictureBox PreLed5PB;

		private PictureBox PreLed4PB;

		private PictureBox PreLed3PB;

		private PictureBox PreLed2PB;

		private PictureBox PreLed1PB;

		private Panel SeqPicEditPL;

		private Button GuidePic5Bn;

		private Button GuidePic4Bn;

		private Button GuidePic3Bn;

		private Button GuidePic2Bn;

		private Button GuidePic1Bn;

		private Button GuidePicDown;

		private Button GuidePicUp;

		private Button RstAllGuideLedBn;

		private Button DelGuideLedBn;

		private CheckBox EnDisGuideBn;

		private Button InsertbackwardGuideLedBn;

		private Button InsertForwardGuideLedBn;

		private Button RstSingleGuideLedBn;

		private Panel ShowGuidePL;

		private Button OpenGuidePicBn;

		private Button NextLedPageBn;

		private Button PreLedPageBn;

		private CheckBox EnDisPositioningArmBn;

		private Button TeachArmBn;

		private Button RstPositionArmBn;

		private Button ShowAllPostionArmBn;

		private GroupBox PositionArmGB;

		private Label lab_CurrX;

		private Label lab_TargZ;

		private Label lab_TargY;

		private Label lab_CurrY;

		private Label lab_TargX;

		private Label lab_Teach;

		private Label lab_Now;

		private Label lab_PreName5;

		private Label lab_PreTool5;

		private Label lab_PreName4;

		private Label lab_PreTool4;

		private Label lab_PreName3;

		private Label lab_PreTool3;

		private Panel PreFlagPL5;

		private Panel PreFlagPL4;

		private Panel PreFlagPL3;

		private Panel PreFlagPL2;

		private Label lab_PreName2;

		private Label lab_PreTool2;

		private Panel PreFlagPL1;

		private Label lab_PreName1;

		private Label lab_PreTool1;

		private Panel PreFlagBPL5;

		private Panel PreFlagBPL4;

		private Panel PreFlagBPL3;

		private Panel PreFlagBPL2;

		private Panel PreFlagBPL1;

		private PictureBox pictureBox1;

		private Panel TargFlagPL;

		private Label labTargName;

		private Label labTargTool;

		private TextBox CurrNumTB;

		private Button ReportNextBn;

		private Button CurrNumPrevBn;

		private Panel CurrNumPL;

		private Label lab_CurrZ;

		public Form200_Seq(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			base.WindowState = FormWindowState.Maximized;
			MultiLanguage.LoadLanguage(this);
			if (!GB.CheckHMIVer(169, 14) && GB.UISys.PCSoftSupport)
			{
				tp_Seqence.TabPages.Remove(tpSeq_Navigator);
				GB.UISys.GuideFuncEnable = false;
			}
			else
			{
				GB.UISys.GuideFuncEnable = true;
			}
			if (GB.UISys.SpecCtrl == 1)
			{
				tp_Seqence.TabPages.Remove(tpSeq_Navigator);
			}
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			tbSeqTitle.Multiline = false;
			tbSeqTitle.ShortcutsEnabled = false;
			tbSeqTitle.KeyPress += GB.RangeASCIIInput;
			tbSeqTitle.KeyUp += TbSeqTitle_KeyUp;
			CircleImg[0] = Resources.ICON_01;
			CircleImg[1] = Resources.ICON_02;
			ButtonImg[0] = Resources.PageTP_G;
			ButtonImg[1] = Resources.PageTP_B;
			OffOnImg[0] = Resources.OFF_ICON;
			OffOnImg[1] = Resources.ON_ICON;
			ToolTip toolTip = new ToolTip();
			toolTip.AutoPopDelay = 3000;
			toolTip.InitialDelay = 5;
			toolTip.SetToolTip(btn_AddID, MultiLanguage.GetStr("ButtonBase", "lab_NewSeq"));
			toolTip.SetToolTip(btnDownload, GB.UISys.UploadToCtrl);
			toolTip.SetToolTip(btnUpload, GB.UISys.DownloadFromCtrl);
			toolTip.SetToolTip(btn_ImportCSV, GB.UISys.ImportFromCSV);
			toolTip.SetToolTip(btn_ExportCSV, GB.UISys.ExportToCSV);
			dataGridView_Seq.MouseClick += dataGridView_Seq_MouseClick;
			dataGridView_Seq.MouseDoubleClick += dataGridView_Seq_MouseClick;
			dataGridView_SeqParam.MouseClick += dataGridView_SeqParam_MouseClick;
			COMbutton = new Button[5] { GuidePic1Bn, GuidePic2Bn, GuidePic3Bn, GuidePic4Bn, GuidePic5Bn };
			for (int i = 0; i < 100; i++)
			{
				SeqEachScrewList[i] = new List<UISeqEachGuideStrc>();
			}
			dt_Seq.Columns.Add("SEL", typeof(Image));
			dt_Seq.Columns.Add("ID", typeof(int));
			dt_Seq.Columns.Add("Tool", typeof(string));
			dt_Seq.Columns.Add("Title", typeof(string));
			dataGridView_Seq.DataSource = dt_Seq;
			loadGrid1(dataGridView_Seq);
			dt_SeqParam.Columns.Add("DEL", typeof(Image));
			dt_SeqParam.Columns.Add("ID", typeof(uint));
			dt_SeqParam.Columns.Add("Tool Item", typeof(uint));
			dt_SeqParam.Columns.Add("Tool Title", typeof(string));
			dt_SeqParam.Columns.Add("Parameter Title", typeof(string));
			dt_SeqParam.Columns.Add("Qty.", typeof(uint));
			dt_SeqParam.Columns.Add("Bit ID", typeof(uint));
			dataGridView_SeqParam.DataSource = dt_SeqParam;
			loadGrid2(dataGridView_SeqParam);
			PreLed1PB.MouseMove += PreLedPic_MouseMove1;
			PreLed2PB.MouseMove += PreLedPic_MouseMove2;
			PreLed3PB.MouseMove += PreLedPic_MouseMove3;
			PreLed4PB.MouseMove += PreLedPic_MouseMove4;
			PreLed5PB.MouseMove += PreLedPic_MouseMove5;
			PreLed1PB.MouseUp += PreLedPB_MouseUp;
			PreLed2PB.MouseUp += PreLedPB_MouseUp;
			PreLed3PB.MouseUp += PreLedPB_MouseUp;
			PreLed4PB.MouseUp += PreLedPB_MouseUp;
			PreLed5PB.MouseUp += PreLedPB_MouseUp;
			CurrNumTB.KeyPress += GB.RangeUnsigned100;
			CurrNumTB.LostFocus += GB.LostFocus_C0;
			FormControlZoom.SetControls(this);
			TextHScaleSize = FormControlZoom.ScreenHeightZoom;
			TextWScaleSize = FormControlZoom.ScreenWidthZoom;
		}

		private void Form200_Seq_Load(object sender, EventArgs e)
		{
			UpdateUI();
			GB.Form200Event = new AutoResetEvent(false);
			GB.Form200ThreadFlag = true;
			ThreadStart MissionForm200 = Form200Thread;
			GB.MissionForm200Thread = new Thread(MissionForm200);
			GB.MissionForm200Thread.Start();
			GB.GetPositionArmTimer = new System.Windows.Forms.Timer();
			GB.GetPositionArmTimer.Interval = 500;
			GB.GetPositionArmTimer.Tick += Timer_Tick;
			GB.GetPositionArmTimer.Start();
			GB.IsProhibitOperation_Seq(this);
			PreFlagPL1.Paint += PreFlagPL_Paint;
			PreFlagPL2.Paint += PreFlagPL_Paint;
			PreFlagPL3.Paint += PreFlagPL_Paint;
			PreFlagPL4.Paint += PreFlagPL_Paint;
			PreFlagPL5.Paint += PreFlagPL_Paint;
			SeqPicEditPL.Paint += SeqPicEditPL_Paint;
			TargFlagPL.Paint += TargFlagPL_Paint;
			CurrNumPL.Paint += TargFlagPL_Paint;
		}

		private void ShowOnOffBtn(uint val, CheckBox Btn, Image[] Img)
		{
			Btn.FlatAppearance.BorderSize = 0;
			Btn.FlatStyle = FlatStyle.Flat;
			Btn.BackgroundImageLayout = ImageLayout.Stretch;
			Btn.BackgroundImage = ((val == 0) ? Img[0] : Img[1]);
			if (Btn.Name == "EnDisGuideBn")
			{
				ShowGuidePL.Visible = ((UI.CurrSeq.GeneralNavigatorMode != 0) ? true : false);
			}
			else if (Btn.Name == "EnDisPositioningArmBn")
			{
				UpdateUI_PositioningArm(0);
			}
		}

		private void UpdateUI()
		{
			ShowSeqTitle();
			ShowSeqParamIcon(false);
		}

		private void ShowGuideImage(int PicGP)
		{
			if (GB.UISys.GuideFuncEnable)
			{
				if (PicGP > 0)
				{
					PicGP--;
					int Base = PicGP / 5;
					GuidePic1Bn.Text = GB.PicSignStr[5 * Base];
					GuidePic2Bn.Text = GB.PicSignStr[5 * Base + 1];
					GuidePic3Bn.Text = GB.PicSignStr[5 * Base + 2];
					GuidePic4Bn.Text = GB.PicSignStr[5 * Base + 3];
					GuidePic5Bn.Text = GB.PicSignStr[5 * Base + 4];
					btnComBackColors(PicGP - 5 * Base);
					GuidePicPB.Dock = DockStyle.Fill;
					GuidePicPB.SizeMode = PictureBoxSizeMode.StretchImage;
					GuidePicPB.Image = ((GuideImgGP[PicGP] != null) ? GuideImgGP[PicGP] : Resources.WhiteBackImage);
					SeqPicEditPL.Controls.Add(GuidePicPB);
				}
				ShowGuideLedPostion();
			}
		}

		private void btnComBackColors(int ChooseNum)
		{
			for (int i = 0; i < COMbutton.Length; i++)
			{
				COMbutton[i].BackgroundImage = ((i == ChooseNum) ? ButtonImg[0] : ButtonImg[1]);
			}
		}

		private void TbSeqTitle_KeyUp(object sender, KeyEventArgs e)
		{
			SetNameTitleStr(tbSeqTitle.Text);
		}

		private void ShowSeqParamIcon(bool Switch)
		{
			tp_Seqence.Visible = Switch;
			SaveBn.Visible = Switch;
			tbSeqTitle.Visible = Switch;
			tbCurrentID.Visible = Switch;
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.ScrollBars = ScrollBars.Both;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
			dataGridView1.Columns[0].HeaderText = "▼";
			dataGridView1.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridView1.Columns[0].Width = 40;
			dataGridView1.Columns[1].Width = 40;
			dataGridView1.Columns[2].Width = 70;
			dataGridView1.Columns[3].Width = 400;
			dataGridView1.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
			((DataGridViewImageColumn)dataGridView1.Columns[0]).ImageLayout = DataGridViewImageCellLayout.Zoom;
		}

		public void loadGrid2(DataGridView dataGridView2)
		{
			dataGridView2.Columns["Tool Item"].Visible = false;
			dataGridView2.AllowUserToAddRows = false;
			dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView2.Columns[0].HeaderText = "▼";
			dataGridView2.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			dataGridView2.Columns[0].FillWeight = 5f;
			dataGridView2.Columns[1].FillWeight = 10f;
			dataGridView2.Columns[2].FillWeight = 10f;
			dataGridView2.Columns[3].FillWeight = 10f;
			dataGridView2.Columns[4].FillWeight = 60f;
			dataGridView2.Columns[5].FillWeight = 10f;
			dataGridView2.Columns[6].FillWeight = 10f;
			dataGridView2.DefaultCellStyle.Font = new Font("Arial", 12f * FormControlZoom.ScreenFontZoom);
			((DataGridViewImageColumn)dataGridView2.Columns[0]).ImageLayout = DataGridViewImageCellLayout.Zoom;
			for (int Idx = 0; Idx < dataGridView2.ColumnCount; Idx++)
			{
				dataGridView2.Columns[Idx].SortMode = DataGridViewColumnSortMode.NotSortable;
			}
		}

		public void GetFormCurrSeq()
		{
			int CurrSeqID = ((Page_Axis == 0) ? GB.TcpStatus.Detail.T1StA.SeqID_02 : GB.TcpStatus.Detail.T2StA.SeqID_02);
			for (int idx = 0; idx < dataGridView_Seq.Rows.Count; idx++)
			{
				if (Convert.ToInt32(dataGridView_Seq.Rows[idx].Cells["ID"].Value) == CurrSeqID && CurrSeqID > 0)
				{
					ReadSingleRowSeq(idx);
					GB.UISys.UIPageNonSave = 0;
				}
			}
		}

		public void Form200Thread()
		{
			while (GB.Form200ThreadFlag)
			{
				if (GB.Form200Event != null)
				{
					GB.Form200ThreadWait = true;
					GB.Form200Event.WaitOne();
					if (!GB.Form200ThreadFlag)
					{
						break;
					}
				}
				if (base.IsHandleCreated)
				{
					Invoke((Action)delegate
					{
					});
				}
			}
		}

		private void dataGridView_Seq_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				int currentMouseOverRow = dataGridView_Seq.HitTest(e.X, e.Y).RowIndex;
				int currentMouseOverCol = dataGridView_Seq.HitTest(e.X, e.Y).ColumnIndex;
				if (currentMouseOverRow == -1 && currentMouseOverCol == 0 && dt_Seq.Rows.Count > 0)
				{
					object CaheIconChoose = dt_Seq.Rows[0]["SEL"];
					foreach (DataGridViewRow SearchRow in (IEnumerable)dataGridView_Seq.Rows)
					{
						if (CaheIconChoose == CircleImg[1])
						{
							dt_Seq.Rows[SearchRow.Index]["SEL"] = CircleImg[0];
						}
						else
						{
							dt_Seq.Rows[SearchRow.Index]["SEL"] = CircleImg[1];
						}
					}
				}
				for (int SearchEachRaw_Idx = 0; SearchEachRaw_Idx < dataGridView_Seq.Rows.Count; SearchEachRaw_Idx++)
				{
					if (dataGridView_Seq.Rows[SearchEachRaw_Idx].Index == currentMouseOverRow)
					{
						if (dataGridView_Seq.Columns[currentMouseOverCol].Name == "SEL")
						{
							for (int i = 0; i < dt_Seq.Rows.Count; i++)
							{
								DataRow dr = dt_Seq.Rows[i];
								if (dr["ID"].ToString() == dataGridView_Seq.Rows[SearchEachRaw_Idx].Cells["ID"].Value.ToString())
								{
									dr["SEL"] = ((dr["SEL"] == CircleImg[1]) ? CircleImg[0] : CircleImg[1]);
								}
							}
						}
						else if (!MatchSequence(tbSeqTitle.Visible))
						{
							CaheRowIdx = Convert.ToInt32(dataGridView_Seq.Rows[SearchEachRaw_Idx].Cells["ID"].Value) - 1;
							Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
							Form996.CreateYesAns += GetForm996YesInfo_SeqNonSave;
							Form996.SetSubForm(FormType.MegSeqNonSave);
							Form996.ShowDialog(this);
						}
						else
						{
							ReadSingleRowSeq(Convert.ToInt32(dataGridView_Seq.Rows[SearchEachRaw_Idx].Cells["ID"].Value) - 1);
						}
						Current_Index = SearchEachRaw_Idx;
					}
					else
					{
						dataGridView_Seq.Rows[SearchEachRaw_Idx].Selected = false;
					}
				}
			}
			dataGridView_Seq.ClearSelection();
			if (dataGridView_Seq.Rows.Count > 0)
			{
				dataGridView_Seq.Rows[Current_Index].Selected = true;
			}
		}

		private void ShowTargInfo(int CurrID)
		{
			int SaveNum = 0;
			if (UI.CurrGuideLedEditID == 0)
			{
				TargFlagPL.Visible = false;
				return;
			}
			TargFlagPL.Visible = true;
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					SaveNum++;
					if (SaveNum == UI.CurrGuideLedEditID)
					{
						UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
						labTargName.Text = "Tool" + SeqEachGuide.ToolIDForSet;
						int ParamID = (int)((SeqEachGuide.ParameterIDForSet != 0) ? (SeqEachGuide.ParameterIDForSet - 1) : 0);
						if (SeqEachGuide.ToolIDForSet == 0)
						{
							labTargName.Text = GB.GetNameTitleStr(FormType.ParamX, ParamID);
						}
						else
						{
							labTargName.Text = GB.GetNameTitleStr(FormType.ParamY, ParamID);
						}
						break;
					}
				}
				if (SaveNum == UI.CurrGuideLedEditID)
				{
					break;
				}
			}
			CurrNumTB.Text = UI.CurrGuideLedEditID.ToString();
		}

		public unsafe void EditGuideInfo(int Mode, int Row, int Value1, int Value2)
		{
			if (!GB.UISys.GuideFuncEnable)
			{
				return;
			}
			switch (Mode)
			{
			case 0:
			{
				uint TotalLedNum = 1u;
				for (int n = 0; n < 100; n++)
				{
					if (SeqEachScrewList[n].Count > 0)
					{
						SeqEachScrewList[n].Clear();
					}
					uint GP = UI.CurrSeqBase;
					for (int l = 0; l < UI.CurrSeq.ScrewQuantityforSet[n]; l++)
					{
						if (TotalLedNum <= 100)
						{
							UISeqEachGuideStrc SeqEachGuide2 = default(UISeqEachGuideStrc);
							SeqEachGuide2.ToolIDForSet = UI.CurrSeq.ToolIDForSet[n];
							SeqEachGuide2.ParameterIDForSet = UI.CurrSeq.ParameterIDForSet[n];
							SeqEachGuide2.LedImg = new PictureBox();
							SeqEachGuide2.LedImg.Location = new Point((int)((float)(int)GB.FSSeqLedXY[GP].Data16[2 * (TotalLedNum - 1)] / 740f * (float)GuidePicPB.Size.Width), (int)((float)(int)GB.FSSeqLedXY[GP].Data16[2 * (TotalLedNum - 1) + 1] / 460f * (float)GuidePicPB.Size.Height));
							SeqEachGuide2.LedImg.MouseMove += LedPic_MouseMove;
							SeqEachGuide2.LedImg.MouseDown += LedPic_MouseDown;
							SeqEachGuide2.PictureIDForSet = GB.FSSeqPicABC[GP].ID[TotalLedNum - 1];
							SeqEachGuide2.PositionArmX = GB.FSSeqArmXYZ[GP].Data16[6 * (TotalLedNum - 1) + 1] * 65536 + GB.FSSeqArmXYZ[GP].Data16[6 * (TotalLedNum - 1)];
							SeqEachGuide2.PositionArmY = GB.FSSeqArmXYZ[GP].Data16[6 * (TotalLedNum - 1) + 3] * 65536 + GB.FSSeqArmXYZ[GP].Data16[6 * (TotalLedNum - 1) + 2];
							SeqEachGuide2.PositionArmZ = GB.FSSeqArmXYZ[GP].Data16[6 * (TotalLedNum - 1) + 5] * 65536 + GB.FSSeqArmXYZ[GP].Data16[6 * (TotalLedNum - 1) + 4];
							SeqEachScrewList[n].Add(SeqEachGuide2);
							TotalLedNum++;
						}
					}
				}
				break;
			}
			case 1:
			{
				for (int num3 = 0; num3 < UI.CurrSeq.ScrewQuantityforSet[Row]; num3++)
				{
					UISeqEachGuideStrc SeqEachGuide4 = SeqEachScrewList[Row][num3];
					SeqEachGuide4.ToolIDForSet = (ushort)Value1;
					SeqEachGuide4.ParameterIDForSet = (ushort)Value2;
					SeqEachScrewList[Row][num3] = SeqEachGuide4;
				}
				break;
			}
			case 2:
				if (Value2 > (uint)Value1)
				{
					for (int j = 0; j < Value2 - (uint)Value1; j++)
					{
						if (SeqEachScrewList[Row].Count > 0)
						{
							SeqEachScrewList[Row].RemoveAt(SeqEachScrewList[Row].Count - 1);
						}
					}
					UI.CurrGuideLedEditID = 1;
				}
				else if (Value2 < (uint)Value1)
				{
					for (int k = 0; k < (uint)Value1 - Value2; k++)
					{
						UISeqEachGuideStrc SeqEachGuide = default(UISeqEachGuideStrc);
						SeqEachGuide.ToolIDForSet = UI.CurrSeq.ToolIDForSet[Row];
						SeqEachGuide.ParameterIDForSet = UI.CurrSeq.ParameterIDForSet[Row];
						SeqEachGuide.LedImg = new PictureBox();
						SeqEachGuide.LedImg.Location = new Point(0, 0);
						SeqEachGuide.LedImg.MouseMove += LedPic_MouseMove;
						SeqEachGuide.LedImg.MouseDown += LedPic_MouseDown;
						SeqEachScrewList[Row].Add(SeqEachGuide);
					}
				}
				UI.CurrSeq.ScrewQuantityforSet[Row] = (uint)SeqEachScrewList[Row].Count;
				break;
			case 3:
			{
				for (int num6 = 0; num6 < 100; num6++)
				{
					for (int num7 = 0; num7 < SeqEachScrewList[num6].Count; num7++)
					{
						UISeqEachGuideStrc SeqEachGuide6 = SeqEachScrewList[num6][num7];
						if (SeqEachGuide6.LedImg.Name == Value1.ToString())
						{
							SeqEachGuide6.LedImg.Location = new Point(0, 0);
							SeqEachScrewList[num6][num7] = SeqEachGuide6;
							break;
						}
					}
				}
				break;
			}
			case 4:
			{
				for (int num = 0; num < 100; num++)
				{
					for (int num2 = 0; num2 < SeqEachScrewList[num].Count; num2++)
					{
						UISeqEachGuideStrc SeqEachGuide3 = SeqEachScrewList[num][num2];
						SeqEachGuide3.LedImg.Location = new Point(0, 0);
						SeqEachScrewList[num][num2] = SeqEachGuide3;
					}
				}
				break;
			}
			case 97:
			{
				int CurrRowTotalQty2 = 0;
				int NextRowTotalQty2 = 0;
				int CaheRowN = -1;
				int CaheInsertNum = 0;
				for (int num4 = 0; num4 < 100; num4++)
				{
					NextRowTotalQty2 = CurrRowTotalQty2 + SeqEachScrewList[num4].Count;
					if (Value1 >= CurrRowTotalQty2 && (Value1 <= NextRowTotalQty2 || NextRowTotalQty2 == CurrRowTotalQty2))
					{
						CaheInsertNum = ((Value2 != 0) ? (Value1 - CurrRowTotalQty2) : ((Value1 > CurrRowTotalQty2) ? (Value1 - CurrRowTotalQty2 - 1) : 0));
						CaheRowN = num4;
						break;
					}
					CurrRowTotalQty2 = NextRowTotalQty2;
				}
				uint TotalLedNum2 = 0u;
				for (int num5 = 0; num5 < 100; num5++)
				{
					TotalLedNum2 += UI.CurrSeq.ScrewQuantityforSet[num5];
					if (TotalLedNum2 >= 100)
					{
						TotalLedNum2 = 100u;
					}
				}
				if (CaheRowN > -1 && TotalLedNum2 < 100)
				{
					UISeqEachGuideStrc SeqEachGuide5 = default(UISeqEachGuideStrc);
					SeqEachGuide5.ToolIDForSet = UI.CurrSeq.ToolIDForSet[CaheRowN];
					SeqEachGuide5.ParameterIDForSet = UI.CurrSeq.ParameterIDForSet[CaheRowN];
					SeqEachGuide5.LedImg = new PictureBox();
					SeqEachGuide5.LedImg.Location = new Point(0, 0);
					SeqEachGuide5.LedImg.MouseMove += LedPic_MouseMove;
					SeqEachGuide5.LedImg.MouseDown += LedPic_MouseDown;
					if (SeqEachScrewList[CaheRowN].Count > 0)
					{
						SeqEachScrewList[CaheRowN].Insert(CaheInsertNum, SeqEachGuide5);
					}
					dt_SeqParam.Rows[CaheRowN]["Qty."] = (UI.CurrSeq.ScrewQuantityforSet[CaheRowN] = (uint)SeqEachScrewList[CaheRowN].Count);
				}
				break;
			}
			case 98:
			{
				int CurrRowTotalQty = 0;
				int NextRowTotalQty = 0;
				for (int m = 0; m < 100; m++)
				{
					NextRowTotalQty = CurrRowTotalQty + SeqEachScrewList[m].Count;
					if (Value1 >= CurrRowTotalQty && (Value1 <= NextRowTotalQty || NextRowTotalQty == CurrRowTotalQty))
					{
						if (SeqEachScrewList[m].Count > 0)
						{
							SeqEachScrewList[m].RemoveAt(Value1 - CurrRowTotalQty - 1);
						}
						dt_SeqParam.Rows[m]["Qty."] = (UI.CurrSeq.ScrewQuantityforSet[m] = (uint)SeqEachScrewList[m].Count);
						break;
					}
					CurrRowTotalQty = NextRowTotalQty;
				}
				UI.CurrGuideLedEditID = 1;
				break;
			}
			case 99:
			{
				for (int i = Row; i < 100; i++)
				{
					if (i < 99)
					{
						SeqEachScrewList[i] = SeqEachScrewList[i + 1];
					}
					else
					{
						SeqEachScrewList[i] = new List<UISeqEachGuideStrc>();
					}
				}
				UI.CurrGuideLedEditID = 1;
				break;
			}
			}
			uint CaluTotalLedNum = 0u;
			GuidePicPB.Controls.Clear();
			for (int num8 = 0; num8 < 100; num8++)
			{
				for (int num9 = 0; num9 < SeqEachScrewList[num8].Count; num9++)
				{
					CaluTotalLedNum++;
					UISeqEachGuideStrc SeqEachGuide7 = SeqEachScrewList[num8][num9];
					SeqEachGuide7.LedImg.Name = CaluTotalLedNum.ToString();
					SeqEachGuide7.LedImg.Size = new Size(30, 30);
					SeqEachGuide7.LedImg.SizeMode = PictureBoxSizeMode.Zoom;
					SeqEachGuide7.LedImg.Image = Resources.LEDEdit_Gray;
					SeqEachGuide7.LedImg.BorderStyle = BorderStyle.None;
					SeqEachGuide7.LedImg.BackColor = Color.Transparent;
					if (UI.CurrGuideLedEditID == CaluTotalLedNum)
					{
						SeqEachGuide7.LedImg.Image = GB.DrawNumber(CaluTotalLedNum.ToString(), Resources.LEDEdit_Org);
					}
					else
					{
						SeqEachGuide7.LedImg.Image = GB.DrawNumber(CaluTotalLedNum.ToString(), Resources.LEDEdit_Gray);
					}
					GuidePicPB.Controls.Add(SeqEachGuide7.LedImg);
					SeqEachScrewList[num8][num9] = SeqEachGuide7;
				}
			}
			ChangeMessageToFSSeq();
			ShowGuideLedPostion();
			ShowPreGuideLed();
			UpdateUI_PositioningArm(0);
			if (UI.CurrGuideLedEditID > 0)
			{
				UI.CurrGuidePicID = GB.FSSeqPicABC[UI.CurrSeqBase].ID[UI.CurrGuideLedEditID - 1];
				if (UI.CurrGuidePicID == 0)
				{
					UI.CurrGuidePicID = 1;
				}
				ShowGuideImage(UI.CurrGuidePicID);
			}
		}

		public void ReadSingleRowSeq(int Base)
		{
			UI.CurrSeqBase = (uint)Base;
			for (int i = 0; i < dataGridView_Seq.RowCount; i++)
			{
				if (Convert.ToUInt32(dataGridView_Seq.Rows[i].Cells["ID"].Value) == Base + 1)
				{
					tbSeqTitle.Text = dataGridView_Seq.Rows[i].Cells["Title"].Value.ToString();
					UI.CurrSeqBase = Convert.ToUInt32(dataGridView_Seq.Rows[i].Cells["ID"].Value) - 1;
				}
			}
			tbCurrentID.Text = (UI.CurrSeqBase + 1).ToString();
			UI.CurrSeq = GB.FSSeqGB[UI.CurrSeqBase];
			ShowSubSeqParam((int)UI.CurrSeqBase);
			UI.CurrGuidePicID = 1;
			UI.CurrGuideLedEditID = 1;
			UI.PreGuideBase5 = 0;
			ShowOnOffBtn(UI.CurrSeq.GeneralNavigatorMode, EnDisGuideBn, OffOnImg);
			ShowOnOffBtn(UI.CurrSeq.ArmPostioningMode, EnDisPositioningArmBn, OffOnImg);
			TCP.FSIDRead_ByTCP(251, 0, (ushort)(Base + 1), 0, 0, 0);
			TCP.FSIDRead_ByTCP(253, 0, (ushort)(Base + 1), 0, 0, 0);
			LoadGuideImgFromDisk("", (int)UI.CurrSeqBase);
			ShowGuideImage(UI.CurrGuidePicID);
			EditGuideInfo(0, 0, 0, 0);
			GB.IsProhibitOperation_Seq(this);
			tp_Seqence.SelectedIndex = 0;
		}

		public void GetForm996YesInfo_SeqNonSave()
		{
			ReadSingleRowSeq(CaheRowIdx);
			GB.UISys.UIPageNonSave = 0;
		}

		public void GetForm996YesInfo_CloseSeqNonSave()
		{
			ShowSeqParamIcon(false);
			AddNewSequence();
		}

		private void dataGridView_SeqParam_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = dataGridView_SeqParam.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = dataGridView_SeqParam.HitTest(e.X, e.Y).ColumnIndex;
			if (currentMouseOverRow != -1 && currentMouseOverCol != -1 && (currentMouseOverRow != -1 || currentMouseOverCol != 0))
			{
				if (dataGridView_SeqParam.Columns[currentMouseOverCol].Name == "DEL")
				{
					dt_SeqParam.Rows[currentMouseOverRow].Delete();
					dt_SeqParam.AcceptChanges();
					EditGuideInfo(99, currentMouseOverRow, 0, 0);
				}
				else if (dataGridView_SeqParam.Columns[currentMouseOverCol].Name == "Parameter Title")
				{
					AssignedSubParamRow = currentMouseOverRow;
					Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem((int)Page_Axis, GB);
					Form990.CreateChooseSeqParamSeqItem += GetForm990ChgSeqSubParamItem;
					Form990.SetSubForm(FormType.ChooseSeqSubParam);
					Form990.ShowDialog(this);
				}
			}
		}

		public void GetForm990AddSeqSubParamItem(uint RetAxis, int RetBase)
		{
			DataRow SeqParamRow = dt_SeqParam.NewRow();
			SeqParamRow[0] = Resources.B_Del_ICON_01;
			SeqParamRow[1] = RetBase + 1;
			if (RetAxis == 0)
			{
				SeqParamRow[2] = 0;
				SeqParamRow[3] = MultiLanguage.GetStr(this, "tp_Tool1");
				SeqParamRow[4] = GB.GetNameTitleStr(FormType.ParamX, RetBase);
			}
			else
			{
				SeqParamRow[2] = 1;
				SeqParamRow[3] = MultiLanguage.GetStr(this, "tp_Tool2");
				SeqParamRow[4] = GB.GetNameTitleStr(FormType.ParamY, RetBase);
			}
			SeqParamRow[5] = 0;
			SeqParamRow[6] = 0;
			dt_SeqParam.Rows.Add(SeqParamRow);
			dt_SeqParam.AcceptChanges();
		}

		public void GetForm990ChgSeqSubParamItem(uint ResAxis, int RetBase)
		{
			dt_SeqParam.Rows[AssignedSubParamRow]["ID"] = RetBase + 1;
			if (ResAxis == 0)
			{
				dt_SeqParam.Rows[AssignedSubParamRow]["Parameter Title"] = GB.GetNameTitleStr(FormType.ParamX, RetBase);
			}
			else
			{
				dt_SeqParam.Rows[AssignedSubParamRow]["Parameter Title"] = GB.GetNameTitleStr(FormType.ParamY, RetBase);
			}
			EditGuideInfo(1, AssignedSubParamRow, (int)ResAxis, RetBase);
		}

		private void btn_AddID_Click(object sender, EventArgs e)
		{
			if (!MatchSequence(tbSeqTitle.Visible))
			{
				Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
				Form996.CreateYesAns += GetForm996YesInfo_CloseSeqNonSave;
				Form996.SetSubForm(FormType.MegParamNonSave);
				Form996.ShowDialog(this);
			}
			else
			{
				AddNewSequence();
			}
		}

		private void AddNewSequence()
		{
			int ID_number = GB.SeqCreateNewRow();
			if (ID_number > 0)
			{
				Form105_Create Form105 = new Form105_Create(GB, (int)Page_Axis);
				Form105.CreateID += GetIDInfo;
				CaheVal.ShowHeaderTitle = MultiLanguage.GetStr(this, "tp_SeqTitle");
				CaheVal.IDNum = ID_number;
				CaheVal.Title = "";
				Form105.SetSubForm(CaheVal, false, FormType.Seq);
				Form105.ShowDialog(this);
			}
		}

		public void GetIDInfo(UI105 CaheVal)
		{
			int Err1 = SeqCheckNameRepeat(UI.CurrSeqBase, CaheVal.Title);
			if (Err1 > 0)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, Err1, "");
				Form995.ShowDialog(this);
				return;
			}
			ReadSingleRowSeq(CaheVal.IDNum - 1);
			tbSeqTitle.Text = CaheVal.Title;
			tbCurrentID.Text = CaheVal.IDNum.ToString();
			SetNameTitleStr(CaheVal.Title);
			GB.UISys.UIPageNonSave = 0;
		}

		public unsafe void SetNameTitleStr(string str)
		{
			for (uint n = 0u; n < 20; n++)
			{
				UI.CurrSeq.TitleChar[n] = 0;
			}
			byte[] Src = Encoding.ASCII.GetBytes(str);
			if (!(str != ""))
			{
				return;
			}
			int Size = Src.Length;
			for (uint n2 = 0u; n2 < (Size + 1) / 2; n2++)
			{
				if (n2 < Size / 2 || Size % 2 == 0)
				{
					UI.CurrSeq.TitleChar[n2] = Convert.ToUInt16((Src[2 * n2 + 1] << 8) + Src[2 * n2]);
				}
				else
				{
					UI.CurrSeq.TitleChar[n2] = Convert.ToUInt16(Src[2 * n2]);
				}
			}
		}

		private void btn_DelID_Click(object sender, EventArgs e)
		{
			SeqBaseStuc SeqZero = default(SeqBaseStuc);
			SeqNavigationCoordinateXY SeqCoordinateZero = default(SeqNavigationCoordinateXY);
			SeqNavigationPictureStuc SeqPictureZero = default(SeqNavigationPictureStuc);
			SeqArmPositionXYZ SeqArmZero = default(SeqArmPositionXYZ);
			GB.ALNGMsgStartStopFunction(false);
			for (int i = dt_Seq.Rows.Count - 1; i >= 0; i--)
			{
				if (dt_Seq.Rows[i]["SEL"] == CircleImg[1])
				{
					GB.FSSeqGB[i] = SeqZero;
					GB.FSSeqLedXY[i] = SeqCoordinateZero;
					GB.FSSeqPicABC[i] = SeqPictureZero;
					GB.FSSeqArmXYZ[i] = SeqArmZero;
					GB.ExSeqCalu(i);
					TCP.FSIDWrite_ByTCP(210, 0, ushort.Parse(dataGridView_Seq.Rows[i].Cells["ID"].Value.ToString()), 0, 0, 0);
					GB.SetNameTitleStr(FormType.Seq, i, "");
					dt_Seq.Rows[i].Delete();
				}
			}
			GB.ALNGMsgStartStopFunction(true);
			ShowSeqParamIcon(false);
			dt_Seq.AcceptChanges();
		}

		private void btn_AddSubParam_Click(object sender, EventArgs e)
		{
			if (dt_SeqParam.Rows.Count < 100)
			{
				AssignedSubParamRow = dt_SeqParam.Rows.Count;
				Form990_JumpPublicChooseItem Form990 = new Form990_JumpPublicChooseItem((int)Page_Axis, GB);
				Form990.CreateChooseSeqParamSeqItem += GetForm990AddSeqSubParamItem;
				Form990.SetSubForm(FormType.ChooseSeqSubParam);
				Form990.ShowDialog(this);
			}
		}

		private unsafe void ShowSeqTitle()
		{
			dt_Seq.Rows.Clear();
			for (int i = 0; i < 500; i++)
			{
				string title = GB.GetNameTitleStr(FormType.Seq, i);
				if (!string.IsNullOrEmpty(title))
				{
					DataRow SeqRow = dt_Seq.NewRow();
					SeqRow[0] = CircleImg[0];
					SeqRow[1] = i + 1;
					if (GB.ExFSSeq.EnableMode[i] % 10 == 1)
					{
						SeqRow[2] = MultiLanguage.GetStr(this, "tp_Tool1");
					}
					else if (GB.ExFSSeq.EnableMode[i] % 10 == 2)
					{
						SeqRow[2] = MultiLanguage.GetStr(this, "tp_Tool2");
					}
					else if (GB.ExFSSeq.EnableMode[i] % 10 == 3)
					{
						SeqRow[2] = MultiLanguage.GetStr(this, "tp_Mix");
					}
					SeqRow[3] = title;
					dt_Seq.Rows.Add(SeqRow);
				}
			}
		}

		private unsafe void ShowSubSeqParam(int Current_SeqBase)
		{
			dt_SeqParam.Rows.Clear();
			for (int i = 0; i < 100; i++)
			{
				int Current_ParamID = GB.FSSeqGB[Current_SeqBase].ParameterIDForSet[i];
				if (Current_ParamID <= 0)
				{
					continue;
				}
				DataRow SeqParamRow = dt_SeqParam.NewRow();
				SeqParamRow[0] = Resources.B_Del_ICON_01;
				SeqParamRow[1] = Current_ParamID;
				if (GB.FSSeqGB[Current_SeqBase].ToolIDForSet[i] == 0)
				{
					SeqParamRow[2] = 0;
					SeqParamRow[3] = MultiLanguage.GetStr(this, "tp_Tool1");
					if (GB.ExFSParamX.EnableGP[Current_ParamID - 1] > 0)
					{
						SeqParamRow[4] = GB.GetNameTitleStr(FormType.ParamX, Current_ParamID - 1);
					}
					else
					{
						SeqParamRow[4] = "(Non-Exist)";
					}
				}
				else
				{
					SeqParamRow[2] = 1;
					SeqParamRow[3] = MultiLanguage.GetStr(this, "tp_Tool2");
					if (GB.ExFSParamY.EnableGP[Current_ParamID - 1] > 0)
					{
						SeqParamRow[4] = GB.GetNameTitleStr(FormType.ParamY, Current_ParamID - 1);
					}
					else
					{
						SeqParamRow[4] = "(Non-Exist)";
					}
				}
				SeqParamRow[5] = GB.FSSeqGB[Current_SeqBase].ScrewQuantityforSet[i];
				SeqParamRow[6] = GB.FSSeqGB[Current_SeqBase].BitIDForSet[i];
				dt_SeqParam.Rows.Add(SeqParamRow);
				dt_SeqParam.AcceptChanges();
			}
			ShowSeqParamIcon(true);
		}

		private unsafe void ChangeMessageToFSSeq()
		{
			SetNameTitleStr(tbSeqTitle.Text);
			UI.TotalScrewNum = 0u;
			for (int i = 0; i < 100; i++)
			{
				if (i < dataGridView_SeqParam.RowCount)
				{
					UI.CurrSeq.ParameterIDForSet[i] = Convert.ToUInt16(dataGridView_SeqParam.Rows[i].Cells["ID"].Value);
					UI.CurrSeq.ToolIDForSet[i] = Convert.ToUInt16(dataGridView_SeqParam.Rows[i].Cells["Tool Item"].Value);
					UI.CurrSeq.ScrewQuantityforSet[i] = Convert.ToUInt32(dataGridView_SeqParam.Rows[i].Cells["Qty."].Value);
					UI.CurrSeq.BitIDForSet[i] = Convert.ToUInt16(dataGridView_SeqParam.Rows[i].Cells["Bit ID"].Value);
				}
				else
				{
					UI.CurrSeq.ParameterIDForSet[i] = 0;
					UI.CurrSeq.ToolIDForSet[i] = 0;
					UI.CurrSeq.ScrewQuantityforSet[i] = 0u;
					UI.CurrSeq.BitIDForSet[i] = 0;
				}
				UI.TotalScrewNum += UI.CurrSeq.ScrewQuantityforSet[i];
			}
		}

		private void WriteMessageToFSSeq(string SaveTitle)
		{
			GB.FSSeqGB[UI.CurrSeqBase] = UI.CurrSeq;
			GB.SetNameTitleStr(FormType.Seq, (int)UI.CurrSeqBase, SaveTitle);
			GB.ExSeqCalu((int)UI.CurrSeqBase);
		}

		private unsafe bool MatchSequence(bool Enable)
		{
			bool[] isEqual = new bool[1] { true };
			if (Enable)
			{
				ChangeMessageToFSSeq();
				for (int i = 0; i < 530; i++)
				{
					if (UI.CurrSeq.Data16[i] != GB.FSSeqGB[UI.CurrSeqBase].Data16[i])
					{
						isEqual[0] = false;
						break;
					}
				}
				return isEqual[0];
			}
			return true;
		}

		private int SeqCheckNameRepeat(uint MatchID, string Matchstr)
		{
			int ErrCode = 0;
			if (Matchstr == "")
			{
				return 3187;
			}
			for (int Gp = 0; Gp < 500; Gp++)
			{
				string SrcStr = GB.GetNameTitleStr(FormType.Seq, Gp);
				if (string.Equals(SrcStr, Matchstr) && SrcStr != "" && MatchID != Gp)
				{
					return 3188;
				}
			}
			foreach (char c in Matchstr)
			{
				if (c < '!' || c > '\u007f')
				{
					return 3187;
				}
			}
			return ErrCode;
		}

		private void SaveBn_Click(object sender, EventArgs e)
		{
			UI.CurrSeqBase = uint.Parse(tbCurrentID.Text) - 1;
			SaveSeqFunction(UI.CurrSeqBase, tbSeqTitle.Text, false);
		}

		private unsafe void SaveSeqFunction(uint SeqBase, string Title, bool ForceSave)
		{
			bool Remind = false;
			ChangeMessageToFSSeq();
			int Err1 = SeqCheckNameRepeat(SeqBase, Title);
			int Err2 = GB.SeqCheckSettingsRange(ref UI, dataGridView_SeqParam.Rows.Count);
			Remind = CheckRunningSeqID(SeqBase);
			if (Err1 > 0 && !ForceSave)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, Err1, "");
				Form995.ShowDialog(this);
				return;
			}
			if (Err2 > 0 && !ForceSave)
			{
				Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, Err2, "");
				Form996.ShowDialog(this);
				return;
			}
			if (Remind && !ForceSave)
			{
				Form996_JumpConfirmYesNo Form997 = new Form996_JumpConfirmYesNo(GB);
				Form997.CreateYesAns += GetForm996YesInfo_ResetScrewProcess;
				Form997.SetSubForm(FormType.MegResultResetProcess);
				Form997.ShowDialog(this);
				return;
			}
			int Err3 = 0;
			Form998_Wait Form998 = new Form998_Wait(GB);
			Form998.Show(this);
			Form998.Process(true, 0, 1);
			GB.SendReadSeqStucVer0 = UI.CurrSeq;
			GB.ALNGMsgStartStopFunction(false);
			Err3 = TCP.FSIDWrite_ByTCP(200, 0, (ushort)(SeqBase + 1), 0, 0, 0);
			int CaluScrewNum = 0;
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					if (CaluScrewNum < 100)
					{
						UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
						GB.FSSeqLedXY[SeqBase].Data16[2 * CaluScrewNum] = (ushort)((float)SeqEachGuide.LedImg.Location.X * 740f / (float)GuidePicPB.Size.Width);
						GB.FSSeqLedXY[SeqBase].Data16[2 * CaluScrewNum + 1] = (ushort)((float)SeqEachGuide.LedImg.Location.Y * 460f / (float)GuidePicPB.Size.Height);
						GB.FSSeqPicABC[SeqBase].ID[CaluScrewNum] = SeqEachGuide.PictureIDForSet;
						GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum] = (ushort)(SeqEachGuide.PositionArmX & 0xFFFF);
						GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 1] = (ushort)((SeqEachGuide.PositionArmX >> 16) & 0xFFFF);
						GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 2] = (ushort)(SeqEachGuide.PositionArmY & 0xFFFF);
						GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 3] = (ushort)((SeqEachGuide.PositionArmY >> 16) & 0xFFFF);
						GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 4] = (ushort)(SeqEachGuide.PositionArmZ & 0xFFFF);
						GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 5] = (ushort)((SeqEachGuide.PositionArmZ >> 16) & 0xFFFF);
						CaluScrewNum++;
					}
				}
			}
			for (int j = CaluScrewNum; j < 100; j++)
			{
				GB.FSSeqLedXY[SeqBase].Data16[2 * j] = 0;
				GB.FSSeqLedXY[SeqBase].Data16[2 * j + 1] = 0;
				GB.FSSeqPicABC[SeqBase].ID[j] = 0;
				GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum] = 0;
				GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 1] = 0;
				GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 2] = 0;
				GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 3] = 0;
				GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 4] = 0;
				GB.FSSeqArmXYZ[SeqBase].Data16[6 * CaluScrewNum + 5] = 0;
			}
			Err3 = TCP.FSIDWrite_ByTCP(201, 0, (ushort)(SeqBase + 1), 0, 0, 0);
			Err3 = TCP.FSIDWrite_ByTCP(202, 0, (ushort)(SeqBase + 1), 0, 0, 0);
			Err3 = TCP.FSIDWrite_ByTCP(203, 0, (ushort)(SeqBase + 1), 0, 0, 0);
			SaveGuideImgToDisk();
			TrCSV.WritePicToController((int)SeqBase, false, ref GuideImgGP, true);
			GB.ALNGMsgStartStopFunction(true);
			Form998.Process(false, 0, 0);
			if (Err3 != -4 && Err3 > 0)
			{
				Form995_RemindOKNG Form999 = new Form995_RemindOKNG(GB, 5005, "  ErrCode:" + Err3.ToString("D3"));
				Form999.Show(this);
				return;
			}
			WriteMessageToFSSeq(Title);
			bool RepeatDefine = false;
			for (int search_i = 0; search_i < dataGridView_Seq.Rows.Count; search_i++)
			{
				if ((SeqBase + 1).ToString() == dataGridView_Seq.Rows[search_i].Cells["ID"].Value.ToString())
				{
					RepeatDefine = true;
				}
			}
			string ToolStr = "";
			if (GB.ExFSSeq.EnableMode[SeqBase] % 10 == 1)
			{
				ToolStr = MultiLanguage.GetStr(this, "tp_Tool1");
			}
			else if (GB.ExFSSeq.EnableMode[SeqBase] % 10 == 2)
			{
				ToolStr = MultiLanguage.GetStr(this, "tp_Tool2");
			}
			else if (GB.ExFSSeq.EnableMode[SeqBase] % 10 == 3)
			{
				ToolStr = MultiLanguage.GetStr(this, "tp_Mix");
			}
			GB.UISys.UIPageNonSave = 0;
			if (!RepeatDefine)
			{
				DataRow SeqRow = dt_Seq.NewRow();
				SeqRow[0] = CircleImg[0];
				SeqRow[1] = SeqBase + 1;
				SeqRow[2] = ToolStr;
				SeqRow[3] = Title;
				if (SeqBase + 1 <= dt_Seq.Rows.Count)
				{
					dt_Seq.Rows.InsertAt(SeqRow, (int)SeqBase);
				}
				else
				{
					dt_Seq.Rows.Add(SeqRow);
				}
				dt_Seq.AcceptChanges();
			}
			GB.BackGroundRunningInfo();
			Form995_RemindOKNG Form1000 = new Form995_RemindOKNG(GB, 3002, "");
			Form1000.ShowDialog(this);
		}

		private bool CheckRunningSeqID(uint SeqBaseID)
		{
			bool Err = false;
			if (dataGridView_Seq.Rows.Count > 0 && SeqBaseID < dataGridView_Seq.Rows.Count)
			{
				uint CurrSeqProcess = ((Page_Axis == 0) ? GB.TcpStatus.Detail.T1StA.SeqID_02 : GB.TcpStatus.Detail.T2StA.SeqID_02);
				uint CurrScrewProcess = (uint)((Page_Axis == 0) ? (GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09) : (GB.TcpStatus.Detail.T2StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T2StA.CurrentSequence_L_09));
				Err = ((dataGridView_Seq.Rows[(int)SeqBaseID].Cells["ID"].Value.ToString() == CurrSeqProcess.ToString() && CurrScrewProcess != 0 && CurrScrewProcess != 999999) ? true : false);
			}
			return Err;
		}

		public void GetForm996YesInfo_ResetScrewProcess()
		{
			SaveSeqFunction(UI.CurrSeqBase, tbSeqTitle.Text, true);
		}

		private unsafe void dataGridView_SeqParam_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			int OrgQty = 0;
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dataGridView_SeqParam.Columns[e.ColumnIndex].Name == "Qty.")
			{
				AssignedSubParamRow = e.RowIndex;
				OrgQty = (int)UI.CurrSeq.ScrewQuantityforSet[AssignedSubParamRow];
			}
			ChangeMessageToFSSeq();
			if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dataGridView_SeqParam.Columns[e.ColumnIndex].Name == "Qty.")
			{
				AssignedSubParamRow = e.RowIndex;
				EditGuideInfo(2, AssignedSubParamRow, Convert.ToInt32(dataGridView_SeqParam.Rows[AssignedSubParamRow].Cells["Qty."].Value), OrgQty);
			}
			if (!MatchSequence(tbSeqTitle.Visible))
			{
				GB.UISys.UIPageNonSave = 1200;
			}
		}

		private void Form200_Seq_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
		}

		private void Form_closed()
		{
			if (GB.GetPositionArmTimer != null)
			{
				GB.GetPositionArmTimer.Stop();
			}
			GB.Form200ThreadFlag = false;
			if (GB.MissionForm200Thread != null)
			{
				GB.MissionForm200Thread.Abort();
			}
			if (GB.Form200Event != null)
			{
				if (GB.Form200ThreadWait)
				{
					GB.Form200Event.Set();
					GB.Form200ThreadWait = false;
				}
				GB.Form200Event.Close();
			}
		}

		public void ExportCSVFunction(string ExportStr)
		{
			for (int i = 0; i < 500; i++)
			{
				if (i < dt_Seq.Rows.Count)
				{
					GB.SeqChooseIcon[i] = (ushort)((dt_Seq.Rows[i]["SEL"] == CircleImg[1]) ? 1 : 0);
				}
				else
				{
					GB.SeqChooseIcon[i] = 0;
				}
			}
			bool RetItem = TrCSV.WriteSeqFile(ExportStr, true);
			bool RetGuide = TrCSV.WriteSeqGuideFile(ExportStr, true);
			bool RetPicture = TrCSV.WriteSeqPictureFile(ExportStr, true);
			bool RetArm = TrCSV.WriteSeqArmFile(ExportStr, true);
			bool RetImg = TrCSV.WriteSeqImageFile(ExportStr, true);
			if (RetItem || RetGuide || RetPicture || RetArm)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3041, "");
				Form995.Show(this);
			}
			else
			{
				Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, 6001, "");
				Form996.Show(this);
			}
		}

		private void btn_ExportCSV_Click(object sender, EventArgs e)
		{
			Form997_ExportTitle Form997 = new Form997_ExportTitle(FormType.ExportSeqTitle, GB);
			Form997.CreateID += ExportCSVFunction;
			Form997.ShowDialog(this);
		}

		private void btn_ImportCSV_Click(object sender, EventArgs e)
		{
			bool ReadFlag = false;
			bool RetItem = true;
			bool RetGuide = true;
			bool RetPicture = true;
			bool RetArm = true;
			using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
			{
				folderBrowserDialog.Description = "Select the Seq folder (including SeqItem, SeqGuide, SeqPicture, SeqArm)";
				folderBrowserDialog.ShowNewFolderButton = true;
				folderBrowserDialog.SelectedPath = Application.StartupPath;
				if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
				{
					ReadFlag = true;
					string DirPath = folderBrowserDialog.SelectedPath;
					string[] files = Directory.GetFiles(DirPath);
					foreach (string strFilename in files)
					{
						if (GB.FSModelTypeInfo.MesModelType == 0)
						{
							if (strFilename.Contains("SeqItem.csv"))
							{
								RetItem = TrCSV.ReadSeqFile(strFilename);
							}
							if (strFilename.Contains("SeqGuide.csv"))
							{
								RetGuide = TrCSV.ReadSeqGuideFile(strFilename);
							}
							if (strFilename.Contains("SeqPicture.csv"))
							{
								RetPicture = TrCSV.ReadSeqPictureFile(strFilename);
							}
							if (strFilename.Contains("SeqArm.csv"))
							{
								RetArm = TrCSV.ReadSeqArmFile(strFilename);
							}
						}
						else
						{
							if (strFilename.Contains("SeqItem010.csv"))
							{
								RetItem = TrCSV.ReadSeqFile(strFilename);
							}
							if (strFilename.Contains("SeqGuide010.csv"))
							{
								RetGuide = TrCSV.ReadSeqGuideFile(strFilename);
							}
							if (strFilename.Contains("SeqPicture010.csv"))
							{
								RetPicture = TrCSV.ReadSeqPictureFile(strFilename);
							}
							if (strFilename.Contains("SeqArm010.csv"))
							{
								RetArm = TrCSV.ReadSeqArmFile(strFilename);
							}
						}
					}
					TrCSV.ReadSeqImageFile(DirPath);
				}
			}
			if (ReadFlag)
			{
				if (RetItem || RetGuide || RetPicture || RetArm)
				{
					UpdateUI();
				}
				else
				{
					Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3192, "");
					Form995.Show(this);
				}
				if (GB.UISys.PCSoftSupport && (RetItem || RetGuide || RetPicture || RetArm))
				{
					Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
					Form996.CreateYesAns += AllDataWriteToCtrl;
					Form996.SetSubForm(FormType.MegSeqWriteAll);
					Form996.ShowDialog(this);
				}
			}
		}

		private void AllDataWriteToCtrl()
		{
			int Err = TrCSV.SeqAllDataWriteToCtrl(true);
			Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 1001, "");
			Form995.Show(this);
		}

		private void AllDataReadTheCtrl()
		{
			GB.ALNGMsgStartStopFunction(false);
			ShowSeqParamIcon(false);
			if (GB.UISys.IsReadSupportFTPClient)
			{
				TCP.FSIDRead_ByFTP(20);
			}
			else
			{
				TCP.FSIDRead_ByFTP(20, 0u, 500u, 0);
			}
			for (int i = 0; i < 500; i++)
			{
				TCP.FSIDRead_ByTCP(251, 0, (ushort)(i + 1), 0, 0, 0);
				TCP.FSIDRead_ByTCP(253, 0, (ushort)(i + 1), 0, 0, 0);
				Form998_Wait Form998 = new Form998_Wait(GB);
				Form998.Show(this);
				Form998.Process(true, i + 1, 500);
				TrCSV.ReadPicFromController((uint)i, true, false);
				Form998.Process(false, 0, 0);
			}
			GB.ALNGMsgStartStopFunction(true);
			UpdateUI();
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataWriteToCtrl;
			Form996.SetSubForm(FormType.MegSeqWriteAll);
			Form996.ShowDialog(this);
		}

		private void btnUpload_Click(object sender, EventArgs e)
		{
			if (!GB.UISys.PCSoftSupport)
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5001, "");
				Form995.Show(this);
				return;
			}
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += AllDataReadTheCtrl;
			Form996.SetSubForm(FormType.MegSeqReadAll);
			Form996.ShowDialog(this);
		}

		private void ReadPicFromController(bool ForceDone)
		{
			if (GB.UISys.GuideFuncEnable)
			{
				GB.ALNGMsgStartStopFunction(false);
				Form998_Wait Form998 = new Form998_Wait(GB);
				Form998.Show(this);
				Form998.Process(true, 1, 2);
				TrCSV.ReadPicFromController(UI.CurrSeqBase, ForceDone, true);
				Form998.Process(false, 0, 0);
				GB.ALNGMsgStartStopFunction(true);
			}
		}

		private void LoadGuideImgFromDisk(string Path, int Base)
		{
			string GetPath = "";
			for (int n = 0; n < 30; n++)
			{
				GetPath = Path;
				if (GetPath == "")
				{
					GetPath = ".\\ScrewInfo\\Seq\\Picture\\" + $"{GB.PicSignStr[n]}{Base + 1:000}.png";
				}
				GuideImgGP[n] = (File.Exists(GetPath) ? GB.LoadPicture(GetPath) : null);
			}
		}

		private void SaveGuideImgToDisk()
		{
			string DirPath = ".\\ScrewInfo\\Seq\\Picture\\";
			string FileName = "";
			GB.ALNGMsgStartStopFunction(false);
			for (int PicBase = 0; PicBase < 30; PicBase++)
			{
				FileName = $"{GB.PicSignStr[PicBase]}{UI.CurrSeqBase + 1:000}.png";
				if (GuideImgGP[PicBase] == null && File.Exists(DirPath + FileName))
				{
					TCP.FSIDWrite_ByTCP(211, 0, (ushort)((PicBase + 1) * 1000 + (UI.CurrSeqBase + 1)), ushort.MaxValue, ushort.MaxValue, 0);
				}
			}
			GB.ALNGMsgStartStopFunction(true);
			for (int i = 0; i < 30; i++)
			{
				FileName = $"{GB.PicSignStr[i]}{UI.CurrSeqBase + 1:000}.png";
				try
				{
					if (GuideImgGP[i] != null)
					{
						GuideImgGP[i] = ReduceImage(GuideImgGP[i], DirPath, FileName);
						continue;
					}
					GuideImgGP[i] = null;
					File.Delete(DirPath + FileName);
				}
				catch
				{
				}
			}
		}

		private void LedPic_MouseDown(object sender, MouseEventArgs e)
		{
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
					SeqEachGuide.LedImg.Image = GB.DrawNumber(SeqEachGuide.LedImg.Name, Resources.LEDEdit_Gray);
				}
			}
		}

		private void LedPic_MouseMove(object sender, MouseEventArgs e)
		{
			PictureBox pb = (PictureBox)sender;
			pb.Visible = true;
			if (e.Button == MouseButtons.Left)
			{
				LedMouseMove(ref pb, e.X, e.Y);
				pb.Image = GB.DrawNumber(pb.Name, Resources.LEDEdit_Org);
				UI.CurrGuideLedEditID = int.Parse(pb.Name);
				ShowTargInfo(UI.CurrGuideLedEditID);
			}
		}

		public void ShowGuideLedPostion()
		{
			if (!GB.UISys.GuideFuncEnable)
			{
				return;
			}
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
					SeqEachGuide.LedImg.Visible = (((SeqEachGuide.LedImg.Location.X != 0 || SeqEachGuide.LedImg.Location.Y != 0) && SeqEachGuide.PictureIDForSet == UI.CurrGuidePicID) ? true : false);
				}
			}
			ShowTargInfo(UI.CurrGuideLedEditID);
		}

		public void ShowPreGuideLed()
		{
			if (!GB.UISys.GuideFuncEnable)
			{
				return;
			}
			int TotalNum = 0;
			int SaveNum = 0;
			string[] SaveStr = new string[5] { "", "", "", "", "" };
			string[] SaveTool = new string[5] { "", "", "", "", "" };
			string[] SaveName = new string[5] { "", "", "", "", "" };
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
					if (SeqEachGuide.LedImg.Location.X == 0 && SeqEachGuide.LedImg.Location.Y == 0)
					{
						if (TotalNum >= UI.PreGuideBase5)
						{
							SaveStr[SaveNum] = SeqEachGuide.LedImg.Name;
							SaveTool[SaveNum] = "Tool" + (SeqEachGuide.ToolIDForSet + 1);
							int ParamID = (int)((SeqEachGuide.ParameterIDForSet != 0) ? (SeqEachGuide.ParameterIDForSet - 1) : 0);
							if (SeqEachGuide.ToolIDForSet == 0)
							{
								SaveName[SaveNum] = GB.GetNameTitleStr(FormType.ParamX, ParamID);
							}
							else
							{
								SaveName[SaveNum] = GB.GetNameTitleStr(FormType.ParamY, ParamID);
							}
							SaveNum++;
						}
						TotalNum++;
					}
					if (SaveNum >= 5)
					{
						break;
					}
				}
				if (SaveNum >= 5)
				{
					break;
				}
			}
			for (int j = 0; j < 5; j++)
			{
				switch (j)
				{
				case 0:
					if (UI.PreGuideBase5 + j < TotalNum)
					{
						PreLed1PB.Name = SaveStr[j];
						if (UI.CurrGuideLedEditID.ToString() == SaveStr[j])
						{
							PreLed1PB.Image = GB.DrawNumber(PreLed1PB.Name, Resources.LEDEdit_Org);
						}
						else
						{
							PreLed1PB.Image = GB.DrawNumber(PreLed1PB.Name, Resources.LEDEdit_Gray);
						}
						lab_PreTool1.Text = SaveTool[j];
						lab_PreName1.Text = SaveName[j];
						Panel preFlagPL5 = PreFlagPL1;
						Panel preFlagBPL5 = PreFlagBPL1;
						bool flag = (PreLed1PB.Visible = true);
						bool visible = (preFlagBPL5.Visible = flag);
						preFlagPL5.Visible = visible;
					}
					else
					{
						Panel preFlagPL6 = PreFlagPL1;
						Panel preFlagBPL6 = PreFlagBPL1;
						bool flag = (PreLed1PB.Visible = false);
						bool visible = (preFlagBPL6.Visible = flag);
						preFlagPL6.Visible = visible;
					}
					break;
				case 1:
					if (UI.PreGuideBase5 + j < TotalNum)
					{
						PreLed2PB.Name = SaveStr[j];
						if (UI.CurrGuideLedEditID.ToString() == SaveStr[j])
						{
							PreLed2PB.Image = GB.DrawNumber(PreLed2PB.Name, Resources.LEDEdit_Org);
						}
						else
						{
							PreLed2PB.Image = GB.DrawNumber(PreLed2PB.Name, Resources.LEDEdit_Gray);
						}
						lab_PreTool2.Text = SaveTool[j];
						lab_PreName2.Text = SaveName[j];
						Panel preFlagPL7 = PreFlagPL2;
						Panel preFlagBPL7 = PreFlagBPL2;
						bool flag = (PreLed2PB.Visible = true);
						bool visible = (preFlagBPL7.Visible = flag);
						preFlagPL7.Visible = visible;
					}
					else
					{
						Panel preFlagPL8 = PreFlagPL2;
						Panel preFlagBPL8 = PreFlagBPL2;
						bool flag = (PreLed2PB.Visible = false);
						bool visible = (preFlagBPL8.Visible = flag);
						preFlagPL8.Visible = visible;
					}
					break;
				case 2:
					if (UI.PreGuideBase5 + j < TotalNum)
					{
						PreLed3PB.Name = SaveStr[j];
						if (UI.CurrGuideLedEditID.ToString() == SaveStr[j])
						{
							PreLed3PB.Image = GB.DrawNumber(PreLed3PB.Name, Resources.LEDEdit_Org);
						}
						else
						{
							PreLed3PB.Image = GB.DrawNumber(PreLed3PB.Name, Resources.LEDEdit_Gray);
						}
						lab_PreTool3.Text = SaveTool[j];
						lab_PreName3.Text = SaveName[j];
						Panel preFlagPL9 = PreFlagPL3;
						Panel preFlagBPL9 = PreFlagBPL3;
						bool flag = (PreLed3PB.Visible = true);
						bool visible = (preFlagBPL9.Visible = flag);
						preFlagPL9.Visible = visible;
					}
					else
					{
						Panel preFlagPL10 = PreFlagPL3;
						Panel preFlagBPL10 = PreFlagBPL3;
						bool flag = (PreLed3PB.Visible = false);
						bool visible = (preFlagBPL10.Visible = flag);
						preFlagPL10.Visible = visible;
					}
					break;
				case 3:
					if (UI.PreGuideBase5 + j < TotalNum)
					{
						PreLed4PB.Name = SaveStr[j];
						if (UI.CurrGuideLedEditID.ToString() == SaveStr[j])
						{
							PreLed4PB.Image = GB.DrawNumber(PreLed4PB.Name, Resources.LEDEdit_Org);
						}
						else
						{
							PreLed4PB.Image = GB.DrawNumber(PreLed4PB.Name, Resources.LEDEdit_Gray);
						}
						lab_PreTool4.Text = SaveTool[j];
						lab_PreName4.Text = SaveName[j];
						Panel preFlagPL3 = PreFlagPL4;
						Panel preFlagBPL3 = PreFlagBPL4;
						bool flag = (PreLed4PB.Visible = true);
						bool visible = (preFlagBPL3.Visible = flag);
						preFlagPL3.Visible = visible;
					}
					else
					{
						Panel preFlagPL4 = PreFlagPL4;
						Panel preFlagBPL4 = PreFlagBPL4;
						bool flag = (PreLed4PB.Visible = false);
						bool visible = (preFlagBPL4.Visible = flag);
						preFlagPL4.Visible = visible;
					}
					break;
				case 4:
					if (UI.PreGuideBase5 + j < TotalNum)
					{
						PreLed5PB.Name = SaveStr[j];
						if (UI.CurrGuideLedEditID.ToString() == SaveStr[j])
						{
							PreLed5PB.Image = GB.DrawNumber(PreLed5PB.Name, Resources.LEDEdit_Org);
						}
						else
						{
							PreLed5PB.Image = GB.DrawNumber(PreLed5PB.Name, Resources.LEDEdit_Gray);
						}
						lab_PreTool5.Text = SaveTool[j];
						lab_PreName5.Text = SaveName[j];
						Panel preFlagPL = PreFlagPL5;
						Panel preFlagBPL = PreFlagBPL5;
						bool flag = (PreLed5PB.Visible = true);
						bool visible = (preFlagBPL.Visible = flag);
						preFlagPL.Visible = visible;
					}
					else
					{
						Panel preFlagPL2 = PreFlagPL5;
						Panel preFlagBPL2 = PreFlagBPL5;
						bool flag = (PreLed5PB.Visible = false);
						bool visible = (preFlagBPL2.Visible = flag);
						preFlagPL2.Visible = visible;
					}
					break;
				}
			}
		}

		private void LedMouseMove(ref PictureBox pb, int MouseX, int MouseY)
		{
			int X = pb.Location.X + MouseX - pb.Width / 2;
			int Y = pb.Location.Y + MouseY - pb.Height / 2;
			XY_lab.Text = "X:" + pb.Location.X + " Y:" + pb.Location.Y;
			float Width = SeqPicEditPL.Width - pb.Width;
			float Higth = SeqPicEditPL.Height - pb.Height;
			X = ((X <= 0) ? 1 : X);
			Y = ((Y <= 0) ? 1 : Y);
			X = ((X > (int)Width) ? ((int)Width) : X);
			Y = ((Y > (int)Higth) ? ((int)Higth) : Y);
			pb.Location = new Point(X, Y);
		}

		private void PreLedPicFunction(object sender, MouseEventArgs e, ref PictureBox PreLed)
		{
			if (!GB.UISys.GuideFuncEnable)
			{
				return;
			}
			bool RstVal = false;
			PictureBox pb = (PictureBox)sender;
			if (e.Button == MouseButtons.Left)
			{
				LedPic_MouseDown(sender, e);
				pb.Image = GB.DrawNumber(pb.Name, Resources.LEDEdit_Org);
				UI.CurrGuideLedEditID = int.Parse(pb.Name);
				for (int n = 0; n < 100; n++)
				{
					for (int i = 0; i < SeqEachScrewList[n].Count; i++)
					{
						UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
						if (SeqEachGuide.LedImg.Name == UI.CurrGuideLedEditID.ToString())
						{
							int X = PreLed.Location.X - SeqPicEditPL.Location.X + 15 + e.X;
							int Y = (int)(-1.5f * (float)PreLed.Size.Height) + e.Y;
							if (X > 0 && Y > 0)
							{
								if (SeqEachGuide.LedImg.Location.X == 0 && SeqEachGuide.LedImg.Location.Y == 0)
								{
									LedMouseMove(ref SeqEachGuide.LedImg, X, Y);
									SeqEachGuide.PictureIDForSet = UI.CurrGuidePicID;
								}
								else
								{
									LedMouseMove(ref SeqEachGuide.LedImg, 15 + e.X - LastX, 15 + e.Y - LastY);
								}
								SeqEachGuide.LedImg.Image = GB.DrawNumber(pb.Name, Resources.LEDEdit_Org);
								SeqEachGuide.LedImg.Visible = ((SeqEachGuide.LedImg.Location.X != 0 || SeqEachGuide.LedImg.Location.Y != 0) ? true : false);
								SeqEachScrewList[n][i] = SeqEachGuide;
							}
							LastX = e.X;
							LastY = e.Y;
							RstVal = true;
							break;
						}
						if (RstVal)
						{
							break;
						}
					}
					if (RstVal)
					{
						break;
					}
				}
				ShowTargInfo(UI.CurrGuideLedEditID);
			}
			else if (e.Button != MouseButtons.None)
			{
			}
		}

		private void PreLedPic_MouseMove1(object sender, MouseEventArgs e)
		{
			PreLedPicFunction(sender, e, ref PreLed1PB);
		}

		private void PreLedPic_MouseMove2(object sender, MouseEventArgs e)
		{
			PreLedPicFunction(sender, e, ref PreLed2PB);
		}

		private void PreLedPic_MouseMove3(object sender, MouseEventArgs e)
		{
			PreLedPicFunction(sender, e, ref PreLed3PB);
		}

		private void PreLedPic_MouseMove4(object sender, MouseEventArgs e)
		{
			PreLedPicFunction(sender, e, ref PreLed4PB);
		}

		private void PreLedPic_MouseMove5(object sender, MouseEventArgs e)
		{
			PreLedPicFunction(sender, e, ref PreLed5PB);
		}

		private void dataGridView_SeqParam_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (dataGridView_SeqParam.Columns[e.ColumnIndex].Name == "Tool Title")
			{
				int i = e.RowIndex;
				if (dataGridView_SeqParam.Rows[i].Cells["Tool Title"].Value.ToString() == MultiLanguage.GetStr(this, "tp_Tool1"))
				{
					dataGridView_SeqParam.Rows[i].Cells["Tool Title"].Style.BackColor = Color.FromArgb(160, 217, 246);
				}
				else if (dataGridView_SeqParam.Rows[i].Cells["Tool Title"].Value.ToString() == MultiLanguage.GetStr(this, "tp_Tool2"))
				{
					dataGridView_SeqParam.Rows[i].Cells["Tool Title"].Style.BackColor = Color.FromArgb(218, 228, 145);
				}
				else
				{
					dataGridView_SeqParam.Rows[i].Cells["Tool Title"].Style.BackColor = Color.White;
				}
			}
		}

		private void Form200_Seq_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
		}

		private void GuidePic1Bn_Click(object sender, EventArgs e)
		{
			UI.CurrGuidePicID = (ushort)((UI.CurrGuidePicID - 1) / 5 * 5 + 1);
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void GuidePic2Bn_Click(object sender, EventArgs e)
		{
			UI.CurrGuidePicID = (ushort)((UI.CurrGuidePicID - 1) / 5 * 5 + 2);
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void GuidePic3Bn_Click(object sender, EventArgs e)
		{
			UI.CurrGuidePicID = (ushort)((UI.CurrGuidePicID - 1) / 5 * 5 + 3);
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void GuidePic4Bn_Click(object sender, EventArgs e)
		{
			UI.CurrGuidePicID = (ushort)((UI.CurrGuidePicID - 1) / 5 * 5 + 4);
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void GuidePic5Bn_Click(object sender, EventArgs e)
		{
			UI.CurrGuidePicID = (ushort)((UI.CurrGuidePicID - 1) / 5 * 5 + 5);
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void GuidePicUp_Click(object sender, EventArgs e)
		{
			UI.CurrGuidePicID = (ushort)((UI.CurrGuidePicID <= 5) ? 1 : ((ushort)((ushort)((UI.CurrGuidePicID - 1) / 5) * 5 - 5 + 1)));
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void GuidePicDown_Click(object sender, EventArgs e)
		{
			UI.CurrGuidePicID = (ushort)((UI.CurrGuidePicID >= 25) ? 26 : ((ushort)((ushort)((UI.CurrGuidePicID - 1) / 5) * 5 + 5 + 1)));
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void PreLedPB_MouseUp(object sender, MouseEventArgs e)
		{
			PictureBox pb = (PictureBox)sender;
			pb.Image = GB.DrawNumber(pb.Name, Resources.LEDEdit_Gray);
			ShowPreGuideLed();
		}

		private void DelGuideLedBn_Click(object sender, EventArgs e)
		{
			EditGuideInfo(98, 0, UI.CurrGuideLedEditID, 0);
		}

		private void RstAllGuidePicBn_Click(object sender, EventArgs e)
		{
			EditGuideInfo(4, 0, 0, 0);
		}

		private void RstSingleGuidePicBn_Click(object sender, EventArgs e)
		{
			EditGuideInfo(3, 0, UI.CurrGuideLedEditID, 0);
		}

		private void InsertForwardGuideLedBn_Click(object sender, EventArgs e)
		{
			EditGuideInfo(97, 0, UI.CurrGuideLedEditID, 0);
		}

		private void InsertbackwardGuideLedBn_Click(object sender, EventArgs e)
		{
			EditGuideInfo(97, 0, UI.CurrGuideLedEditID, 1);
		}

		private void EnDisableGuideBn_Click(object sender, EventArgs e)
		{
			UI.CurrSeq.GeneralNavigatorMode ^= 1;
			ShowOnOffBtn(UI.CurrSeq.GeneralNavigatorMode, EnDisGuideBn, OffOnImg);
		}

		private void OpenGuidePicBn_Click(object sender, EventArgs e)
		{
			Image Img = ((UI.CurrGuidePicID > 0) ? GuideImgGP[UI.CurrGuidePicID - 1] : null);
			Form210_SeqPicture Form210 = new Form210_SeqPicture(GB, TCP, ref Img);
			Form210.CreateCloseEvent += GetForm210;
			Form210.Show();
		}

		private void GetForm210(Image Img)
		{
			if (UI.CurrGuidePicID > 0)
			{
				GuideImgGP[UI.CurrGuidePicID - 1] = Img;
			}
			ShowGuideImage(UI.CurrGuidePicID);
		}

		private void NextLedPageBn_Click(object sender, EventArgs e)
		{
			UI.PreGuideBase5 = ((UI.PreGuideBase5 >= 95) ? 95 : (UI.PreGuideBase5 + 5));
			ShowPreGuideLed();
		}

		private void PreLedPageBn_Click(object sender, EventArgs e)
		{
			UI.PreGuideBase5 = ((UI.PreGuideBase5 >= 5) ? (UI.PreGuideBase5 - 5) : 0);
			ShowPreGuideLed();
		}

		private void EnDisPositioningArmBn_Click(object sender, EventArgs e)
		{
			UI.CurrSeq.ArmPostioningMode ^= 1;
			ShowOnOffBtn(UI.CurrSeq.ArmPostioningMode, EnDisPositioningArmBn, OffOnImg);
		}

		private void dataGridView_Seq_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (dataGridView_Seq.Columns[e.ColumnIndex].Name == "Tool")
			{
				int i = e.RowIndex;
				if (dataGridView_Seq.Rows[i].Cells["Tool"].Value.ToString() == MultiLanguage.GetStr(this, "tp_Tool1"))
				{
					dataGridView_Seq.Rows[i].Cells["Tool"].Style.BackColor = Color.FromArgb(160, 217, 246);
				}
				else if (dataGridView_Seq.Rows[i].Cells["Tool"].Value.ToString() == MultiLanguage.GetStr(this, "tp_Tool2"))
				{
					dataGridView_Seq.Rows[i].Cells["Tool"].Style.BackColor = Color.FromArgb(218, 228, 145);
				}
				else if (dataGridView_Seq.Rows[i].Cells["Tool"].Value.ToString() == MultiLanguage.GetStr(this, "tp_Mix"))
				{
					dataGridView_Seq.Rows[i].Cells["Tool"].Style.BackColor = Color.FromArgb(196, 196, 255);
				}
				else
				{
					dataGridView_Seq.Rows[i].Cells["Tool"].Style.BackColor = Color.White;
				}
			}
		}

		private void UpdateUI_PositioningArm(int Mode)
		{
			try
			{
				if (!GB.UISys.GuideFuncEnable)
				{
					return;
				}
				if (Mode == 0)
				{
					if (UI.CurrSeq.ArmPostioningMode == 1)
					{
						Button showAllPostionArmBn = ShowAllPostionArmBn;
						Button teachArmBn = TeachArmBn;
						bool flag = (RstPositionArmBn.Visible = true);
						bool visible = (teachArmBn.Visible = flag);
						showAllPostionArmBn.Visible = visible;
						Label label = lab_Now;
						visible = (lab_Teach.Visible = true);
						label.Visible = visible;
						Label label2 = lab_CurrX;
						Label label3 = lab_CurrY;
						flag = (lab_CurrZ.Visible = true);
						visible = (label3.Visible = flag);
						label2.Visible = visible;
						Label label4 = lab_TargX;
						Label label5 = lab_TargY;
						flag = (lab_TargZ.Visible = true);
						visible = (label5.Visible = flag);
						label4.Visible = visible;
						bool RstSuccess = false;
						int[] PosArm = new int[3];
						int CaluScrewNum = 0;
						for (int n = 0; n < 100; n++)
						{
							for (int i = 0; i < SeqEachScrewList[n].Count; i++)
							{
								CaluScrewNum++;
								if (UI.CurrGuideLedEditID == CaluScrewNum)
								{
									PosArm[0] = SeqEachScrewList[n][i].PositionArmX;
									PosArm[1] = SeqEachScrewList[n][i].PositionArmY;
									PosArm[2] = SeqEachScrewList[n][i].PositionArmZ;
									RstSuccess = true;
									break;
								}
							}
							if (UI.CurrGuideLedEditID == CaluScrewNum)
							{
								break;
							}
						}
						if (lab_TargX.InvokeRequired)
						{
							lab_TargX.Invoke((Action)delegate
							{
								lab_TargX.Text = "X: " + ((double)PosArm[0] / 10.0).ToString("F1");
							});
						}
						else
						{
							lab_TargX.Text = "X: " + ((double)PosArm[0] / 10.0).ToString("F1");
						}
						if (lab_TargY.InvokeRequired)
						{
							lab_TargY.Invoke((Action)delegate
							{
								lab_TargY.Text = "Y: " + ((double)PosArm[1] / 10.0).ToString("F1");
							});
						}
						else
						{
							lab_TargY.Text = "Y: " + ((double)PosArm[1] / 10.0).ToString("F1");
						}
						if (lab_TargZ.InvokeRequired)
						{
							lab_TargZ.Invoke((Action)delegate
							{
								lab_TargZ.Text = "Z: " + ((double)PosArm[2] / 10.0).ToString("F1");
							});
						}
						else
						{
							lab_TargZ.Text = "Z: " + ((double)PosArm[2] / 10.0).ToString("F1");
						}
					}
					else
					{
						Button showAllPostionArmBn2 = ShowAllPostionArmBn;
						Button teachArmBn2 = TeachArmBn;
						bool flag = (RstPositionArmBn.Visible = false);
						bool visible = (teachArmBn2.Visible = flag);
						showAllPostionArmBn2.Visible = visible;
						Label label6 = lab_Now;
						visible = (lab_Teach.Visible = false);
						label6.Visible = visible;
						Label label7 = lab_CurrX;
						Label label8 = lab_CurrY;
						flag = (lab_CurrZ.Visible = false);
						visible = (label8.Visible = flag);
						label7.Visible = visible;
						Label label9 = lab_TargX;
						Label label10 = lab_TargY;
						flag = (lab_TargZ.Visible = false);
						visible = (label10.Visible = flag);
						label9.Visible = visible;
					}
					return;
				}
				if (GB.FSCtrlComPortFunction.RS485Function == 1 || GB.FSCtrlComPortFunction.RS485Function == 3 || GB.FSCtrlComPortFunction.RS485Function == 5 || GB.FSCtrlComPortFunction.RS485Function == 7 || GB.FSCtrlComPortFunction.RS485Function == 9 || GB.FSCtrlComPortFunction.RS485Function == 11)
				{
					UI.PositionArmX = GB.TcpStatus.Detail.T1StB.Fed_PositioningArmX_H_33 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmX_L_32;
					UI.PositionArmY = GB.TcpStatus.Detail.T1StB.Fed_PositioningArmY_H_35 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmY_L_34;
					UI.PositionArmZ = GB.TcpStatus.Detail.T1StB.Fed_PositioningArmZ_H_37 * 65536 + GB.TcpStatus.Detail.T1StB.Fed_PositioningArmZ_L_36;
				}
				else if (GB.FSCtrlComPortFunction.RS485Function == 2 || GB.FSCtrlComPortFunction.RS485Function == 4 || GB.FSCtrlComPortFunction.RS485Function == 6 || GB.FSCtrlComPortFunction.RS485Function == 8 || GB.FSCtrlComPortFunction.RS485Function == 10 || GB.FSCtrlComPortFunction.RS485Function == 12)
				{
					UI.PositionArmX = GB.TcpStatus.Detail.T2StB.Fed_PositioningArmX_H_33 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmX_L_32;
					UI.PositionArmY = GB.TcpStatus.Detail.T2StB.Fed_PositioningArmY_H_35 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmY_L_34;
					UI.PositionArmZ = GB.TcpStatus.Detail.T2StB.Fed_PositioningArmZ_H_37 * 65536 + GB.TcpStatus.Detail.T2StB.Fed_PositioningArmZ_L_36;
				}
				else
				{
					UI.PositionArmX = 0;
					UI.PositionArmY = 0;
					UI.PositionArmZ = 0;
				}
				if (lab_CurrX.InvokeRequired)
				{
					lab_CurrX.Invoke((Action)delegate
					{
						lab_CurrX.Text = "X: " + ((double)UI.PositionArmX / 10.0).ToString("F1");
					});
				}
				else
				{
					lab_CurrX.Text = "X: " + ((double)UI.PositionArmX / 10.0).ToString("F1");
				}
				if (lab_CurrY.InvokeRequired)
				{
					lab_CurrY.Invoke((Action)delegate
					{
						lab_CurrY.Text = "Y: " + ((double)UI.PositionArmY / 10.0).ToString("F1");
					});
				}
				else
				{
					lab_CurrY.Text = "Y: " + ((double)UI.PositionArmY / 10.0).ToString("F1");
				}
				if (lab_CurrZ.InvokeRequired)
				{
					lab_CurrZ.Invoke((Action)delegate
					{
						lab_CurrZ.Text = "Z: " + ((double)UI.PositionArmZ / 10.0).ToString("F1");
					});
				}
				else
				{
					lab_CurrZ.Text = "Z: " + ((double)UI.PositionArmZ / 10.0).ToString("F1");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error updating lab_TargX.Text: " + ex.Message);
			}
		}

		private void TeachArmBn_Click(object sender, EventArgs e)
		{
			int CaluScrewNum = 0;
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					CaluScrewNum++;
					if (UI.CurrGuideLedEditID == CaluScrewNum)
					{
						UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
						SeqEachGuide.PositionArmX = UI.PositionArmX;
						SeqEachGuide.PositionArmY = UI.PositionArmY;
						SeqEachGuide.PositionArmZ = UI.PositionArmZ;
						SeqEachScrewList[n][i] = SeqEachGuide;
						break;
					}
				}
				if (UI.CurrGuideLedEditID == CaluScrewNum)
				{
					break;
				}
			}
			UpdateUI_PositioningArm(0);
		}

		private void RstPositionArmBn_Click(object sender, EventArgs e)
		{
			int CaluScrewNum = 0;
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					CaluScrewNum++;
					if (UI.CurrGuideLedEditID == CaluScrewNum)
					{
						UISeqEachGuideStrc SeqEachGuide = SeqEachScrewList[n][i];
						SeqEachGuide.PositionArmX = 0;
						SeqEachGuide.PositionArmY = 0;
						SeqEachGuide.PositionArmZ = 0;
						SeqEachScrewList[n][i] = SeqEachGuide;
						break;
					}
				}
				if (UI.CurrGuideLedEditID == CaluScrewNum)
				{
					break;
				}
			}
			UpdateUI_PositioningArm(0);
		}

		private void ShowAllPostionArmBn_Click(object sender, EventArgs e)
		{
			int[] Data32Table = new int[300];
			int CaluScrewNum = 0;
			for (int n = 0; n < 100; n++)
			{
				for (int i = 0; i < SeqEachScrewList[n].Count; i++)
				{
					if (CaluScrewNum < 100)
					{
						Data32Table[3 * CaluScrewNum] = SeqEachScrewList[n][i].PositionArmX;
						Data32Table[3 * CaluScrewNum + 1] = SeqEachScrewList[n][i].PositionArmY;
						Data32Table[3 * CaluScrewNum + 2] = SeqEachScrewList[n][i].PositionArmZ;
						CaluScrewNum++;
					}
				}
			}
			Form211_SeqPositioningArm Form211 = new Form211_SeqPositioningArm(GB, TCP, Data32Table);
			Form211.Show();
		}

		private void PreFlagPL_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.Cyan, 2f);
			Control control = sender as Control;
			e.Graphics.DrawRectangle(pen1, 0, 0, control.Width - 1, control.Height - 1);
		}

		private void TargFlagPL_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.FromArgb(237, 110, 0), 2f);
			Control control = sender as Control;
			e.Graphics.DrawRectangle(pen1, 0, 0, control.Width - 1, control.Height - 1);
		}

		private void SeqPicEditPL_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.FromArgb(0, 255, 255), 5f);
			Control control = sender as Control;
			e.Graphics.DrawRectangle(pen1, 0, 0, control.Width - 4, control.Height - 4);
		}

		private void CurrNumPrevBn_Click(object sender, EventArgs e)
		{
			if (UI.CurrGuideLedEditID <= 1)
			{
				UI.CurrGuideLedEditID = 1;
			}
			else
			{
				UI.CurrGuideLedEditID--;
			}
			EditGuideInfo(10, 0, 0, 0);
		}

		private void CurrNumNextBn_Click(object sender, EventArgs e)
		{
			if (UI.CurrGuideLedEditID >= UI.TotalScrewNum)
			{
				UI.CurrGuideLedEditID = (int)UI.TotalScrewNum;
			}
			else
			{
				UI.CurrGuideLedEditID++;
			}
			EditGuideInfo(10, 0, 0, 0);
		}

		private void CurrNumTB_KeyPress(object sender, KeyPressEventArgs e)
		{
			int CurrNum = 0;
			if (int.TryParse(CurrNumTB.Text, out CurrNum))
			{
				if (UI.TotalScrewNum != 0 && CurrNum > UI.TotalScrewNum)
				{
					UI.CurrGuideLedEditID = (int)UI.TotalScrewNum;
				}
				else if (CurrNum > 100)
				{
					UI.CurrGuideLedEditID = 100;
				}
				else if (CurrNum > 0)
				{
					UI.CurrGuideLedEditID = CurrNum;
				}
				else
				{
					UI.CurrGuideLedEditID = 1;
				}
			}
			EditGuideInfo(10, 0, 0, 0);
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			UpdateUI_PositioningArm(99);
		}

		public Image ReduceImage(Image pic, string DirPath, string FileName)
		{
			long targetSize = 184320L;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (!Directory.Exists(DirPath))
				{
					Directory.CreateDirectory(DirPath);
				}
				pic.Save(memoryStream, ImageFormat.Png);
				if (memoryStream.Length <= targetSize)
				{
					pic.Save(DirPath + FileName, ImageFormat.Png);
					return pic;
				}
				int newWidth = pic.Width;
				int newHeight = pic.Height;
				while (true)
				{
					try
					{
						using (Bitmap resizedImage = new Bitmap(pic, newWidth, newHeight))
						{
							using (MemoryStream compressedStream = new MemoryStream())
							{
								resizedImage.Save(compressedStream, ImageFormat.Png);
								if (compressedStream.Length <= targetSize)
								{
									File.WriteAllBytes(DirPath + FileName, compressedStream.ToArray());
									compressedStream.Position = 0L;
									return Image.FromStream(compressedStream);
								}
								newWidth = (int)((double)newWidth * 0.9);
								newHeight = (int)((double)newHeight * 0.9);
								if (newWidth < 370 || newHeight < 230)
								{
									File.WriteAllBytes(DirPath + FileName, compressedStream.ToArray());
									compressedStream.Position = 0L;
									return Image.FromStream(compressedStream);
								}
							}
						}
					}
					catch
					{
					}
				}
			}
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form200_Seq));
			this.dataGridView_Seq = new System.Windows.Forms.DataGridView();
			this.tp_Seqence = new System.Windows.Forms.TabControl();
			this.tpSeq_Normal = new System.Windows.Forms.TabPage();
			this.btn_AddSubParam = new System.Windows.Forms.Button();
			this.dataGridView_SeqParam = new System.Windows.Forms.DataGridView();
			this.tpSeq_Navigator = new System.Windows.Forms.TabPage();
			this.EnDisGuideBn = new System.Windows.Forms.CheckBox();
			this.ShowGuidePL = new System.Windows.Forms.Panel();
			this.ReportNextBn = new System.Windows.Forms.Button();
			this.CurrNumPrevBn = new System.Windows.Forms.Button();
			this.CurrNumPL = new System.Windows.Forms.Panel();
			this.CurrNumTB = new System.Windows.Forms.TextBox();
			this.TargFlagPL = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.labTargName = new System.Windows.Forms.Label();
			this.labTargTool = new System.Windows.Forms.Label();
			this.PreFlagBPL5 = new System.Windows.Forms.Panel();
			this.PreFlagBPL4 = new System.Windows.Forms.Panel();
			this.PreFlagBPL3 = new System.Windows.Forms.Panel();
			this.PreFlagBPL2 = new System.Windows.Forms.Panel();
			this.PreFlagBPL1 = new System.Windows.Forms.Panel();
			this.PreFlagPL5 = new System.Windows.Forms.Panel();
			this.lab_PreName5 = new System.Windows.Forms.Label();
			this.lab_PreTool5 = new System.Windows.Forms.Label();
			this.PreFlagPL4 = new System.Windows.Forms.Panel();
			this.lab_PreName4 = new System.Windows.Forms.Label();
			this.lab_PreTool4 = new System.Windows.Forms.Label();
			this.PreFlagPL3 = new System.Windows.Forms.Panel();
			this.lab_PreName3 = new System.Windows.Forms.Label();
			this.lab_PreTool3 = new System.Windows.Forms.Label();
			this.PreFlagPL2 = new System.Windows.Forms.Panel();
			this.lab_PreName2 = new System.Windows.Forms.Label();
			this.lab_PreTool2 = new System.Windows.Forms.Label();
			this.PreFlagPL1 = new System.Windows.Forms.Panel();
			this.lab_PreName1 = new System.Windows.Forms.Label();
			this.lab_PreTool1 = new System.Windows.Forms.Label();
			this.PositionArmGB = new System.Windows.Forms.GroupBox();
			this.lab_TargZ = new System.Windows.Forms.Label();
			this.lab_TargY = new System.Windows.Forms.Label();
			this.lab_CurrZ = new System.Windows.Forms.Label();
			this.lab_CurrY = new System.Windows.Forms.Label();
			this.lab_TargX = new System.Windows.Forms.Label();
			this.lab_Teach = new System.Windows.Forms.Label();
			this.lab_Now = new System.Windows.Forms.Label();
			this.lab_CurrX = new System.Windows.Forms.Label();
			this.EnDisPositioningArmBn = new System.Windows.Forms.CheckBox();
			this.TeachArmBn = new System.Windows.Forms.Button();
			this.RstPositionArmBn = new System.Windows.Forms.Button();
			this.InsertbackwardGuideLedBn = new System.Windows.Forms.Button();
			this.InsertForwardGuideLedBn = new System.Windows.Forms.Button();
			this.GuidePicDown = new System.Windows.Forms.Button();
			this.GuidePicUp = new System.Windows.Forms.Button();
			this.RstSingleGuideLedBn = new System.Windows.Forms.Button();
			this.GuidePic5Bn = new System.Windows.Forms.Button();
			this.GuidePic4Bn = new System.Windows.Forms.Button();
			this.GuidePic3Bn = new System.Windows.Forms.Button();
			this.GuidePic2Bn = new System.Windows.Forms.Button();
			this.DelGuideLedBn = new System.Windows.Forms.Button();
			this.GuidePic1Bn = new System.Windows.Forms.Button();
			this.SeqPreGuidePL = new System.Windows.Forms.Panel();
			this.OpenGuidePicBn = new System.Windows.Forms.Button();
			this.PreLed5PB = new System.Windows.Forms.PictureBox();
			this.PreLed4PB = new System.Windows.Forms.PictureBox();
			this.ShowAllPostionArmBn = new System.Windows.Forms.Button();
			this.RstAllGuideLedBn = new System.Windows.Forms.Button();
			this.PreLed3PB = new System.Windows.Forms.PictureBox();
			this.NextLedPageBn = new System.Windows.Forms.Button();
			this.PreLedPageBn = new System.Windows.Forms.Button();
			this.PreLed2PB = new System.Windows.Forms.PictureBox();
			this.PreLed1PB = new System.Windows.Forms.PictureBox();
			this.XY_lab = new System.Windows.Forms.Label();
			this.SeqPicEditPL = new System.Windows.Forms.Panel();
			this.tbSeqTitle = new System.Windows.Forms.TextBox();
			this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tbCurrentID = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SaveBn = new System.Windows.Forms.Button();
			this.btn_ImportCSV = new System.Windows.Forms.Button();
			this.btn_ExportCSV = new System.Windows.Forms.Button();
			this.btn_DelID = new System.Windows.Forms.Button();
			this.btn_AddID = new System.Windows.Forms.Button();
			this.btnDownload = new System.Windows.Forms.Button();
			this.btnUpload = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Seq).BeginInit();
			this.tp_Seqence.SuspendLayout();
			this.tpSeq_Normal.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqParam).BeginInit();
			this.tpSeq_Navigator.SuspendLayout();
			this.ShowGuidePL.SuspendLayout();
			this.CurrNumPL.SuspendLayout();
			this.TargFlagPL.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
			this.PreFlagPL5.SuspendLayout();
			this.PreFlagPL4.SuspendLayout();
			this.PreFlagPL3.SuspendLayout();
			this.PreFlagPL2.SuspendLayout();
			this.PreFlagPL1.SuspendLayout();
			this.PositionArmGB.SuspendLayout();
			this.SeqPreGuidePL.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.PreLed5PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed4PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed3PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed2PB).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed1PB).BeginInit();
			this.groupBox1.SuspendLayout();
			base.SuspendLayout();
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("新細明體", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView_Seq.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView_Seq.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("新細明體", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView_Seq.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridView_Seq.Location = new System.Drawing.Point(16, 72);
			this.dataGridView_Seq.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_Seq.Name = "dataGridView_Seq";
			this.dataGridView_Seq.ReadOnly = true;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("新細明體", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView_Seq.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dataGridView_Seq.RowHeadersVisible = false;
			this.dataGridView_Seq.RowHeadersWidth = 51;
			this.dataGridView_Seq.RowTemplate.Height = 24;
			this.dataGridView_Seq.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridView_Seq.Size = new System.Drawing.Size(364, 862);
			this.dataGridView_Seq.TabIndex = 14;
			this.dataGridView_Seq.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(dataGridView_Seq_CellFormatting);
			this.tp_Seqence.Controls.Add(this.tpSeq_Normal);
			this.tp_Seqence.Controls.Add(this.tpSeq_Navigator);
			this.tp_Seqence.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.tp_Seqence.ItemSize = new System.Drawing.Size(40, 25);
			this.tp_Seqence.Location = new System.Drawing.Point(1, 65);
			this.tp_Seqence.Margin = new System.Windows.Forms.Padding(4);
			this.tp_Seqence.Name = "tp_Seqence";
			this.tp_Seqence.Padding = new System.Drawing.Point(50, 4);
			this.tp_Seqence.SelectedIndex = 0;
			this.tp_Seqence.Size = new System.Drawing.Size(1399, 759);
			this.tp_Seqence.TabIndex = 26;
			this.tpSeq_Normal.Controls.Add(this.btn_AddSubParam);
			this.tpSeq_Normal.Controls.Add(this.dataGridView_SeqParam);
			this.tpSeq_Normal.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.tpSeq_Normal.Location = new System.Drawing.Point(4, 29);
			this.tpSeq_Normal.Margin = new System.Windows.Forms.Padding(4);
			this.tpSeq_Normal.Name = "tpSeq_Normal";
			this.tpSeq_Normal.Padding = new System.Windows.Forms.Padding(4);
			this.tpSeq_Normal.Size = new System.Drawing.Size(1391, 726);
			this.tpSeq_Normal.TabIndex = 0;
			this.tpSeq_Normal.Text = "General";
			this.tpSeq_Normal.UseVisualStyleBackColor = true;
			this.btn_AddSubParam.BackgroundImage = SD3Soft.Properties.Resources.B_新增_ICON_01;
			this.btn_AddSubParam.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_AddSubParam.FlatAppearance.BorderSize = 0;
			this.btn_AddSubParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_AddSubParam.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_AddSubParam.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_AddSubParam.Location = new System.Drawing.Point(13, 15);
			this.btn_AddSubParam.Margin = new System.Windows.Forms.Padding(4);
			this.btn_AddSubParam.Name = "btn_AddSubParam";
			this.btn_AddSubParam.Size = new System.Drawing.Size(53, 50);
			this.btn_AddSubParam.TabIndex = 62;
			this.btn_AddSubParam.UseVisualStyleBackColor = true;
			this.btn_AddSubParam.Click += new System.EventHandler(btn_AddSubParam_Click);
			this.dataGridView_SeqParam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_SeqParam.Location = new System.Drawing.Point(8, 72);
			this.dataGridView_SeqParam.Margin = new System.Windows.Forms.Padding(4);
			this.dataGridView_SeqParam.Name = "dataGridView_SeqParam";
			this.dataGridView_SeqParam.RowHeadersVisible = false;
			this.dataGridView_SeqParam.RowHeadersWidth = 51;
			this.dataGridView_SeqParam.RowTemplate.Height = 24;
			this.dataGridView_SeqParam.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridView_SeqParam.Size = new System.Drawing.Size(1373, 645);
			this.dataGridView_SeqParam.TabIndex = 15;
			this.dataGridView_SeqParam.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(dataGridView_SeqParam_CellFormatting);
			this.dataGridView_SeqParam.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView_SeqParam_CellValueChanged);
			this.tpSeq_Navigator.Controls.Add(this.EnDisGuideBn);
			this.tpSeq_Navigator.Controls.Add(this.ShowGuidePL);
			this.tpSeq_Navigator.Location = new System.Drawing.Point(4, 29);
			this.tpSeq_Navigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tpSeq_Navigator.Name = "tpSeq_Navigator";
			this.tpSeq_Navigator.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.tpSeq_Navigator.Size = new System.Drawing.Size(1391, 726);
			this.tpSeq_Navigator.TabIndex = 1;
			this.tpSeq_Navigator.Text = "Navigator";
			this.tpSeq_Navigator.UseVisualStyleBackColor = true;
			this.EnDisGuideBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.EnDisGuideBn.AutoCheck = false;
			this.EnDisGuideBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("EnDisGuideBn.BackgroundImage");
			this.EnDisGuideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.EnDisGuideBn.FlatAppearance.BorderSize = 0;
			this.EnDisGuideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.EnDisGuideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.EnDisGuideBn.Location = new System.Drawing.Point(1289, 14);
			this.EnDisGuideBn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.EnDisGuideBn.Name = "EnDisGuideBn";
			this.EnDisGuideBn.Size = new System.Drawing.Size(84, 34);
			this.EnDisGuideBn.TabIndex = 227;
			this.EnDisGuideBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.EnDisGuideBn.UseVisualStyleBackColor = true;
			this.EnDisGuideBn.Click += new System.EventHandler(EnDisableGuideBn_Click);
			this.ShowGuidePL.Controls.Add(this.ReportNextBn);
			this.ShowGuidePL.Controls.Add(this.CurrNumPrevBn);
			this.ShowGuidePL.Controls.Add(this.CurrNumPL);
			this.ShowGuidePL.Controls.Add(this.TargFlagPL);
			this.ShowGuidePL.Controls.Add(this.PreFlagBPL5);
			this.ShowGuidePL.Controls.Add(this.PreFlagBPL4);
			this.ShowGuidePL.Controls.Add(this.PreFlagBPL3);
			this.ShowGuidePL.Controls.Add(this.PreFlagBPL2);
			this.ShowGuidePL.Controls.Add(this.PreFlagBPL1);
			this.ShowGuidePL.Controls.Add(this.PreFlagPL5);
			this.ShowGuidePL.Controls.Add(this.PreFlagPL4);
			this.ShowGuidePL.Controls.Add(this.PreFlagPL3);
			this.ShowGuidePL.Controls.Add(this.PreFlagPL2);
			this.ShowGuidePL.Controls.Add(this.PreFlagPL1);
			this.ShowGuidePL.Controls.Add(this.PositionArmGB);
			this.ShowGuidePL.Controls.Add(this.InsertbackwardGuideLedBn);
			this.ShowGuidePL.Controls.Add(this.InsertForwardGuideLedBn);
			this.ShowGuidePL.Controls.Add(this.GuidePicDown);
			this.ShowGuidePL.Controls.Add(this.GuidePicUp);
			this.ShowGuidePL.Controls.Add(this.RstSingleGuideLedBn);
			this.ShowGuidePL.Controls.Add(this.GuidePic5Bn);
			this.ShowGuidePL.Controls.Add(this.GuidePic4Bn);
			this.ShowGuidePL.Controls.Add(this.GuidePic3Bn);
			this.ShowGuidePL.Controls.Add(this.GuidePic2Bn);
			this.ShowGuidePL.Controls.Add(this.DelGuideLedBn);
			this.ShowGuidePL.Controls.Add(this.GuidePic1Bn);
			this.ShowGuidePL.Controls.Add(this.SeqPreGuidePL);
			this.ShowGuidePL.Controls.Add(this.XY_lab);
			this.ShowGuidePL.Controls.Add(this.SeqPicEditPL);
			this.ShowGuidePL.Location = new System.Drawing.Point(12, 14);
			this.ShowGuidePL.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ShowGuidePL.Name = "ShowGuidePL";
			this.ShowGuidePL.Size = new System.Drawing.Size(1271, 706);
			this.ShowGuidePL.TabIndex = 228;
			this.ReportNextBn.BackgroundImage = SD3Soft.Properties.Resources.Next;
			this.ReportNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ReportNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ReportNextBn.ForeColor = System.Drawing.Color.Transparent;
			this.ReportNextBn.Location = new System.Drawing.Point(962, 166);
			this.ReportNextBn.Margin = new System.Windows.Forms.Padding(4);
			this.ReportNextBn.Name = "ReportNextBn";
			this.ReportNextBn.Size = new System.Drawing.Size(47, 39);
			this.ReportNextBn.TabIndex = 233;
			this.ReportNextBn.UseVisualStyleBackColor = true;
			this.ReportNextBn.Click += new System.EventHandler(CurrNumNextBn_Click);
			this.CurrNumPrevBn.BackgroundImage = SD3Soft.Properties.Resources.Last;
			this.CurrNumPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.CurrNumPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CurrNumPrevBn.ForeColor = System.Drawing.Color.Transparent;
			this.CurrNumPrevBn.Location = new System.Drawing.Point(822, 166);
			this.CurrNumPrevBn.Margin = new System.Windows.Forms.Padding(4);
			this.CurrNumPrevBn.Name = "CurrNumPrevBn";
			this.CurrNumPrevBn.Size = new System.Drawing.Size(47, 39);
			this.CurrNumPrevBn.TabIndex = 234;
			this.CurrNumPrevBn.UseVisualStyleBackColor = true;
			this.CurrNumPrevBn.Click += new System.EventHandler(CurrNumPrevBn_Click);
			this.CurrNumPL.BackColor = System.Drawing.Color.White;
			this.CurrNumPL.Controls.Add(this.CurrNumTB);
			this.CurrNumPL.Location = new System.Drawing.Point(870, 166);
			this.CurrNumPL.Margin = new System.Windows.Forms.Padding(4);
			this.CurrNumPL.Name = "CurrNumPL";
			this.CurrNumPL.Size = new System.Drawing.Size(90, 39);
			this.CurrNumPL.TabIndex = 236;
			this.CurrNumTB.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.CurrNumTB.Location = new System.Drawing.Point(6, 8);
			this.CurrNumTB.Margin = new System.Windows.Forms.Padding(4);
			this.CurrNumTB.Name = "CurrNumTB";
			this.CurrNumTB.Size = new System.Drawing.Size(77, 24);
			this.CurrNumTB.TabIndex = 235;
			this.CurrNumTB.Text = "1";
			this.CurrNumTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.CurrNumTB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CurrNumTB_KeyPress);
			this.TargFlagPL.BackColor = System.Drawing.Color.White;
			this.TargFlagPL.Controls.Add(this.pictureBox1);
			this.TargFlagPL.Controls.Add(this.labTargName);
			this.TargFlagPL.Controls.Add(this.labTargTool);
			this.TargFlagPL.Location = new System.Drawing.Point(825, 116);
			this.TargFlagPL.Margin = new System.Windows.Forms.Padding(4);
			this.TargFlagPL.Name = "TargFlagPL";
			this.TargFlagPL.Size = new System.Drawing.Size(428, 38);
			this.TargFlagPL.TabIndex = 232;
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.Image = SD3Soft.Properties.Resources.目前螺絲1;
			this.pictureBox1.Location = new System.Drawing.Point(4, 3);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(35, 32);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 233;
			this.pictureBox1.TabStop = false;
			this.labTargName.BackColor = System.Drawing.Color.White;
			this.labTargName.Font = new System.Drawing.Font("新細明體", 9f);
			this.labTargName.Location = new System.Drawing.Point(115, 10);
			this.labTargName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labTargName.Name = "labTargName";
			this.labTargName.Size = new System.Drawing.Size(227, 19);
			this.labTargName.TabIndex = 230;
			this.labTargName.Text = "Name";
			this.labTargName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labTargTool.BackColor = System.Drawing.Color.Black;
			this.labTargTool.Font = new System.Drawing.Font("新細明體", 9f);
			this.labTargTool.ForeColor = System.Drawing.Color.White;
			this.labTargTool.Location = new System.Drawing.Point(48, 10);
			this.labTargTool.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labTargTool.Name = "labTargTool";
			this.labTargTool.Size = new System.Drawing.Size(59, 19);
			this.labTargTool.TabIndex = 229;
			this.labTargTool.Text = "Tool 1";
			this.labTargTool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PreFlagBPL5.BackColor = System.Drawing.Color.Cyan;
			this.PreFlagBPL5.Location = new System.Drawing.Point(423, 156);
			this.PreFlagBPL5.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagBPL5.Name = "PreFlagBPL5";
			this.PreFlagBPL5.Size = new System.Drawing.Size(4, 12);
			this.PreFlagBPL5.TabIndex = 231;
			this.PreFlagBPL4.BackColor = System.Drawing.Color.Cyan;
			this.PreFlagBPL4.Location = new System.Drawing.Point(353, 128);
			this.PreFlagBPL4.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagBPL4.Name = "PreFlagBPL4";
			this.PreFlagBPL4.Size = new System.Drawing.Size(4, 38);
			this.PreFlagBPL4.TabIndex = 231;
			this.PreFlagBPL3.BackColor = System.Drawing.Color.Cyan;
			this.PreFlagBPL3.Location = new System.Drawing.Point(284, 99);
			this.PreFlagBPL3.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagBPL3.Name = "PreFlagBPL3";
			this.PreFlagBPL3.Size = new System.Drawing.Size(4, 69);
			this.PreFlagBPL3.TabIndex = 231;
			this.PreFlagBPL2.BackColor = System.Drawing.Color.Cyan;
			this.PreFlagBPL2.Location = new System.Drawing.Point(216, 70);
			this.PreFlagBPL2.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagBPL2.Name = "PreFlagBPL2";
			this.PreFlagBPL2.Size = new System.Drawing.Size(4, 95);
			this.PreFlagBPL2.TabIndex = 231;
			this.PreFlagBPL1.BackColor = System.Drawing.Color.Cyan;
			this.PreFlagBPL1.Location = new System.Drawing.Point(147, 34);
			this.PreFlagBPL1.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagBPL1.Name = "PreFlagBPL1";
			this.PreFlagBPL1.Size = new System.Drawing.Size(4, 132);
			this.PreFlagBPL1.TabIndex = 231;
			this.PreFlagPL5.BackColor = System.Drawing.Color.White;
			this.PreFlagPL5.Controls.Add(this.lab_PreName5);
			this.PreFlagPL5.Controls.Add(this.lab_PreTool5);
			this.PreFlagPL5.Location = new System.Drawing.Point(423, 133);
			this.PreFlagPL5.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagPL5.Name = "PreFlagPL5";
			this.PreFlagPL5.Size = new System.Drawing.Size(293, 30);
			this.PreFlagPL5.TabIndex = 231;
			this.lab_PreName5.BackColor = System.Drawing.Color.White;
			this.lab_PreName5.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreName5.Location = new System.Drawing.Point(63, 6);
			this.lab_PreName5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreName5.Name = "lab_PreName5";
			this.lab_PreName5.Size = new System.Drawing.Size(225, 17);
			this.lab_PreName5.TabIndex = 230;
			this.lab_PreName5.Text = "Name";
			this.lab_PreName5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_PreTool5.BackColor = System.Drawing.Color.Black;
			this.lab_PreTool5.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreTool5.ForeColor = System.Drawing.Color.White;
			this.lab_PreTool5.Location = new System.Drawing.Point(4, 6);
			this.lab_PreTool5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreTool5.Name = "lab_PreTool5";
			this.lab_PreTool5.Size = new System.Drawing.Size(59, 17);
			this.lab_PreTool5.TabIndex = 229;
			this.lab_PreTool5.Text = "Tool 1";
			this.lab_PreTool5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PreFlagPL4.BackColor = System.Drawing.Color.White;
			this.PreFlagPL4.Controls.Add(this.lab_PreName4);
			this.PreFlagPL4.Controls.Add(this.lab_PreTool4);
			this.PreFlagPL4.Location = new System.Drawing.Point(353, 101);
			this.PreFlagPL4.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagPL4.Name = "PreFlagPL4";
			this.PreFlagPL4.Size = new System.Drawing.Size(293, 30);
			this.PreFlagPL4.TabIndex = 231;
			this.lab_PreName4.BackColor = System.Drawing.Color.White;
			this.lab_PreName4.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreName4.Location = new System.Drawing.Point(63, 6);
			this.lab_PreName4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreName4.Name = "lab_PreName4";
			this.lab_PreName4.Size = new System.Drawing.Size(225, 17);
			this.lab_PreName4.TabIndex = 230;
			this.lab_PreName4.Text = "Name";
			this.lab_PreName4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_PreTool4.BackColor = System.Drawing.Color.Black;
			this.lab_PreTool4.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreTool4.ForeColor = System.Drawing.Color.White;
			this.lab_PreTool4.Location = new System.Drawing.Point(4, 6);
			this.lab_PreTool4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreTool4.Name = "lab_PreTool4";
			this.lab_PreTool4.Size = new System.Drawing.Size(59, 17);
			this.lab_PreTool4.TabIndex = 229;
			this.lab_PreTool4.Text = "Tool 1";
			this.lab_PreTool4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PreFlagPL3.BackColor = System.Drawing.Color.White;
			this.PreFlagPL3.Controls.Add(this.lab_PreName3);
			this.PreFlagPL3.Controls.Add(this.lab_PreTool3);
			this.PreFlagPL3.Location = new System.Drawing.Point(284, 70);
			this.PreFlagPL3.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagPL3.Name = "PreFlagPL3";
			this.PreFlagPL3.Size = new System.Drawing.Size(293, 30);
			this.PreFlagPL3.TabIndex = 231;
			this.lab_PreName3.BackColor = System.Drawing.Color.White;
			this.lab_PreName3.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreName3.Location = new System.Drawing.Point(63, 7);
			this.lab_PreName3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreName3.Name = "lab_PreName3";
			this.lab_PreName3.Size = new System.Drawing.Size(225, 17);
			this.lab_PreName3.TabIndex = 230;
			this.lab_PreName3.Text = "Name";
			this.lab_PreName3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_PreTool3.BackColor = System.Drawing.Color.Black;
			this.lab_PreTool3.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreTool3.ForeColor = System.Drawing.Color.White;
			this.lab_PreTool3.Location = new System.Drawing.Point(4, 7);
			this.lab_PreTool3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreTool3.Name = "lab_PreTool3";
			this.lab_PreTool3.Size = new System.Drawing.Size(59, 17);
			this.lab_PreTool3.TabIndex = 229;
			this.lab_PreTool3.Text = "Tool 1";
			this.lab_PreTool3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PreFlagPL2.BackColor = System.Drawing.Color.White;
			this.PreFlagPL2.Controls.Add(this.lab_PreName2);
			this.PreFlagPL2.Controls.Add(this.lab_PreTool2);
			this.PreFlagPL2.Location = new System.Drawing.Point(216, 39);
			this.PreFlagPL2.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagPL2.Name = "PreFlagPL2";
			this.PreFlagPL2.Size = new System.Drawing.Size(293, 30);
			this.PreFlagPL2.TabIndex = 231;
			this.lab_PreName2.BackColor = System.Drawing.Color.White;
			this.lab_PreName2.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreName2.Location = new System.Drawing.Point(64, 6);
			this.lab_PreName2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreName2.Name = "lab_PreName2";
			this.lab_PreName2.Size = new System.Drawing.Size(225, 17);
			this.lab_PreName2.TabIndex = 230;
			this.lab_PreName2.Text = "Name";
			this.lab_PreName2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_PreTool2.BackColor = System.Drawing.Color.Black;
			this.lab_PreTool2.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreTool2.ForeColor = System.Drawing.Color.White;
			this.lab_PreTool2.Location = new System.Drawing.Point(5, 6);
			this.lab_PreTool2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreTool2.Name = "lab_PreTool2";
			this.lab_PreTool2.Size = new System.Drawing.Size(59, 17);
			this.lab_PreTool2.TabIndex = 229;
			this.lab_PreTool2.Text = "Tool 1";
			this.lab_PreTool2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PreFlagPL1.BackColor = System.Drawing.Color.White;
			this.PreFlagPL1.Controls.Add(this.lab_PreName1);
			this.PreFlagPL1.Controls.Add(this.lab_PreTool1);
			this.PreFlagPL1.Location = new System.Drawing.Point(147, 8);
			this.PreFlagPL1.Margin = new System.Windows.Forms.Padding(4);
			this.PreFlagPL1.Name = "PreFlagPL1";
			this.PreFlagPL1.Size = new System.Drawing.Size(293, 30);
			this.PreFlagPL1.TabIndex = 231;
			this.lab_PreName1.BackColor = System.Drawing.Color.White;
			this.lab_PreName1.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreName1.Location = new System.Drawing.Point(64, 6);
			this.lab_PreName1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreName1.Name = "lab_PreName1";
			this.lab_PreName1.Size = new System.Drawing.Size(225, 17);
			this.lab_PreName1.TabIndex = 230;
			this.lab_PreName1.Text = "Name";
			this.lab_PreName1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.lab_PreTool1.BackColor = System.Drawing.Color.Black;
			this.lab_PreTool1.Font = new System.Drawing.Font("新細明體", 9f);
			this.lab_PreTool1.ForeColor = System.Drawing.Color.White;
			this.lab_PreTool1.Location = new System.Drawing.Point(5, 6);
			this.lab_PreTool1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_PreTool1.Name = "lab_PreTool1";
			this.lab_PreTool1.Size = new System.Drawing.Size(59, 17);
			this.lab_PreTool1.TabIndex = 229;
			this.lab_PreTool1.Text = "Tool 1";
			this.lab_PreTool1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.PositionArmGB.Controls.Add(this.lab_TargZ);
			this.PositionArmGB.Controls.Add(this.lab_TargY);
			this.PositionArmGB.Controls.Add(this.lab_CurrZ);
			this.PositionArmGB.Controls.Add(this.lab_CurrY);
			this.PositionArmGB.Controls.Add(this.lab_TargX);
			this.PositionArmGB.Controls.Add(this.lab_Teach);
			this.PositionArmGB.Controls.Add(this.lab_Now);
			this.PositionArmGB.Controls.Add(this.lab_CurrX);
			this.PositionArmGB.Controls.Add(this.EnDisPositioningArmBn);
			this.PositionArmGB.Controls.Add(this.TeachArmBn);
			this.PositionArmGB.Controls.Add(this.RstPositionArmBn);
			this.PositionArmGB.Location = new System.Drawing.Point(801, 555);
			this.PositionArmGB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PositionArmGB.Name = "PositionArmGB";
			this.PositionArmGB.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PositionArmGB.Size = new System.Drawing.Size(452, 125);
			this.PositionArmGB.TabIndex = 228;
			this.PositionArmGB.TabStop = false;
			this.PositionArmGB.Text = "Positioning arm";
			this.lab_TargZ.AutoSize = true;
			this.lab_TargZ.Location = new System.Drawing.Point(325, 95);
			this.lab_TargZ.Name = "lab_TargZ";
			this.lab_TargZ.Size = new System.Drawing.Size(35, 20);
			this.lab_TargZ.TabIndex = 228;
			this.lab_TargZ.Text = "Z : ";
			this.lab_TargY.AutoSize = true;
			this.lab_TargY.Location = new System.Drawing.Point(325, 68);
			this.lab_TargY.Name = "lab_TargY";
			this.lab_TargY.Size = new System.Drawing.Size(33, 20);
			this.lab_TargY.TabIndex = 228;
			this.lab_TargY.Text = "Y: ";
			this.lab_CurrZ.AutoSize = true;
			this.lab_CurrZ.Location = new System.Drawing.Point(229, 95);
			this.lab_CurrZ.Name = "lab_CurrZ";
			this.lab_CurrZ.Size = new System.Drawing.Size(30, 20);
			this.lab_CurrZ.TabIndex = 228;
			this.lab_CurrZ.Text = "Z :";
			this.lab_CurrZ.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_CurrY.AutoSize = true;
			this.lab_CurrY.Location = new System.Drawing.Point(229, 68);
			this.lab_CurrY.Name = "lab_CurrY";
			this.lab_CurrY.Size = new System.Drawing.Size(33, 20);
			this.lab_CurrY.TabIndex = 228;
			this.lab_CurrY.Text = "Y: ";
			this.lab_CurrY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lab_TargX.AutoSize = true;
			this.lab_TargX.Location = new System.Drawing.Point(325, 42);
			this.lab_TargX.Name = "lab_TargX";
			this.lab_TargX.Size = new System.Drawing.Size(33, 20);
			this.lab_TargX.TabIndex = 228;
			this.lab_TargX.Text = "X: ";
			this.lab_Teach.AutoSize = true;
			this.lab_Teach.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Teach.ForeColor = System.Drawing.Color.Red;
			this.lab_Teach.Location = new System.Drawing.Point(325, 18);
			this.lab_Teach.Name = "lab_Teach";
			this.lab_Teach.Size = new System.Drawing.Size(58, 20);
			this.lab_Teach.TabIndex = 228;
			this.lab_Teach.Text = "Teach";
			this.lab_Now.AutoSize = true;
			this.lab_Now.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 136);
			this.lab_Now.ForeColor = System.Drawing.Color.Blue;
			this.lab_Now.Location = new System.Drawing.Point(229, 18);
			this.lab_Now.Name = "lab_Now";
			this.lab_Now.Size = new System.Drawing.Size(49, 20);
			this.lab_Now.TabIndex = 228;
			this.lab_Now.Text = "Now";
			this.lab_CurrX.AutoSize = true;
			this.lab_CurrX.Location = new System.Drawing.Point(229, 42);
			this.lab_CurrX.Name = "lab_CurrX";
			this.lab_CurrX.Size = new System.Drawing.Size(33, 20);
			this.lab_CurrX.TabIndex = 228;
			this.lab_CurrX.Text = "X: ";
			this.lab_CurrX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.EnDisPositioningArmBn.Appearance = System.Windows.Forms.Appearance.Button;
			this.EnDisPositioningArmBn.AutoCheck = false;
			this.EnDisPositioningArmBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("EnDisPositioningArmBn.BackgroundImage");
			this.EnDisPositioningArmBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.EnDisPositioningArmBn.FlatAppearance.BorderSize = 0;
			this.EnDisPositioningArmBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.EnDisPositioningArmBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.EnDisPositioningArmBn.Location = new System.Drawing.Point(12, 30);
			this.EnDisPositioningArmBn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.EnDisPositioningArmBn.Name = "EnDisPositioningArmBn";
			this.EnDisPositioningArmBn.Size = new System.Drawing.Size(84, 34);
			this.EnDisPositioningArmBn.TabIndex = 227;
			this.EnDisPositioningArmBn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.EnDisPositioningArmBn.UseVisualStyleBackColor = true;
			this.EnDisPositioningArmBn.Click += new System.EventHandler(EnDisPositioningArmBn_Click);
			this.TeachArmBn.BackgroundImage = SD3Soft.Properties.Resources.Teach;
			this.TeachArmBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.TeachArmBn.FlatAppearance.BorderSize = 0;
			this.TeachArmBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.TeachArmBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.TeachArmBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.TeachArmBn.Location = new System.Drawing.Point(103, 26);
			this.TeachArmBn.Margin = new System.Windows.Forms.Padding(4);
			this.TeachArmBn.Name = "TeachArmBn";
			this.TeachArmBn.Size = new System.Drawing.Size(53, 50);
			this.TeachArmBn.TabIndex = 173;
			this.TeachArmBn.UseVisualStyleBackColor = true;
			this.TeachArmBn.Click += new System.EventHandler(TeachArmBn_Click);
			this.RstPositionArmBn.BackgroundImage = SD3Soft.Properties.Resources.單顆回復_灰;
			this.RstPositionArmBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.RstPositionArmBn.FlatAppearance.BorderSize = 0;
			this.RstPositionArmBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPositionArmBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPositionArmBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPositionArmBn.Location = new System.Drawing.Point(161, 26);
			this.RstPositionArmBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstPositionArmBn.Name = "RstPositionArmBn";
			this.RstPositionArmBn.Size = new System.Drawing.Size(53, 50);
			this.RstPositionArmBn.TabIndex = 173;
			this.RstPositionArmBn.UseVisualStyleBackColor = true;
			this.RstPositionArmBn.Click += new System.EventHandler(RstPositionArmBn_Click);
			this.InsertbackwardGuideLedBn.BackColor = System.Drawing.Color.Transparent;
			this.InsertbackwardGuideLedBn.BackgroundImage = SD3Soft.Properties.Resources.複製並向後插入_灰;
			this.InsertbackwardGuideLedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.InsertbackwardGuideLedBn.FlatAppearance.BorderSize = 0;
			this.InsertbackwardGuideLedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.InsertbackwardGuideLedBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.InsertbackwardGuideLedBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.InsertbackwardGuideLedBn.Location = new System.Drawing.Point(1081, 166);
			this.InsertbackwardGuideLedBn.Margin = new System.Windows.Forms.Padding(4);
			this.InsertbackwardGuideLedBn.Name = "InsertbackwardGuideLedBn";
			this.InsertbackwardGuideLedBn.Size = new System.Drawing.Size(53, 50);
			this.InsertbackwardGuideLedBn.TabIndex = 173;
			this.InsertbackwardGuideLedBn.UseVisualStyleBackColor = false;
			this.InsertbackwardGuideLedBn.Click += new System.EventHandler(InsertbackwardGuideLedBn_Click);
			this.InsertForwardGuideLedBn.BackgroundImage = SD3Soft.Properties.Resources.複製並向前插入_灰;
			this.InsertForwardGuideLedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.InsertForwardGuideLedBn.FlatAppearance.BorderSize = 0;
			this.InsertForwardGuideLedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.InsertForwardGuideLedBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.InsertForwardGuideLedBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.InsertForwardGuideLedBn.Location = new System.Drawing.Point(1020, 166);
			this.InsertForwardGuideLedBn.Margin = new System.Windows.Forms.Padding(4);
			this.InsertForwardGuideLedBn.Name = "InsertForwardGuideLedBn";
			this.InsertForwardGuideLedBn.Size = new System.Drawing.Size(53, 50);
			this.InsertForwardGuideLedBn.TabIndex = 173;
			this.InsertForwardGuideLedBn.UseVisualStyleBackColor = true;
			this.InsertForwardGuideLedBn.Click += new System.EventHandler(InsertForwardGuideLedBn_Click);
			this.GuidePicDown.FlatAppearance.BorderSize = 0;
			this.GuidePicDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GuidePicDown.Image = SD3Soft.Properties.Resources.GuideNextPage;
			this.GuidePicDown.Location = new System.Drawing.Point(9, 652);
			this.GuidePicDown.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.GuidePicDown.Name = "GuidePicDown";
			this.GuidePicDown.Size = new System.Drawing.Size(45, 28);
			this.GuidePicDown.TabIndex = 171;
			this.GuidePicDown.UseVisualStyleBackColor = true;
			this.GuidePicDown.Click += new System.EventHandler(GuidePicDown_Click);
			this.GuidePicUp.FlatAppearance.BorderSize = 0;
			this.GuidePicUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GuidePicUp.Image = SD3Soft.Properties.Resources.GuidePrePage;
			this.GuidePicUp.Location = new System.Drawing.Point(9, 222);
			this.GuidePicUp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.GuidePicUp.Name = "GuidePicUp";
			this.GuidePicUp.Size = new System.Drawing.Size(45, 28);
			this.GuidePicUp.TabIndex = 171;
			this.GuidePicUp.UseVisualStyleBackColor = true;
			this.GuidePicUp.Click += new System.EventHandler(GuidePicUp_Click);
			this.RstSingleGuideLedBn.BackgroundImage = SD3Soft.Properties.Resources.單顆回復_灰;
			this.RstSingleGuideLedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.RstSingleGuideLedBn.FlatAppearance.BorderSize = 0;
			this.RstSingleGuideLedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstSingleGuideLedBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstSingleGuideLedBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstSingleGuideLedBn.Location = new System.Drawing.Point(1200, 166);
			this.RstSingleGuideLedBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstSingleGuideLedBn.Name = "RstSingleGuideLedBn";
			this.RstSingleGuideLedBn.Size = new System.Drawing.Size(53, 50);
			this.RstSingleGuideLedBn.TabIndex = 173;
			this.RstSingleGuideLedBn.UseVisualStyleBackColor = true;
			this.RstSingleGuideLedBn.Click += new System.EventHandler(RstSingleGuidePicBn_Click);
			this.GuidePic5Bn.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			this.GuidePic5Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("GuidePic5Bn.BackgroundImage");
			this.GuidePic5Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.GuidePic5Bn.FlatAppearance.BorderSize = 0;
			this.GuidePic5Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GuidePic5Bn.Font = new System.Drawing.Font("新細明體", 12f);
			this.GuidePic5Bn.Location = new System.Drawing.Point(9, 574);
			this.GuidePic5Bn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.GuidePic5Bn.Name = "GuidePic5Bn";
			this.GuidePic5Bn.Size = new System.Drawing.Size(45, 79);
			this.GuidePic5Bn.TabIndex = 170;
			this.GuidePic5Bn.UseVisualStyleBackColor = false;
			this.GuidePic5Bn.Click += new System.EventHandler(GuidePic5Bn_Click);
			this.GuidePic4Bn.BackColor = System.Drawing.SystemColors.HighlightText;
			this.GuidePic4Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("GuidePic4Bn.BackgroundImage");
			this.GuidePic4Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.GuidePic4Bn.FlatAppearance.BorderSize = 0;
			this.GuidePic4Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GuidePic4Bn.Font = new System.Drawing.Font("新細明體", 12f);
			this.GuidePic4Bn.Location = new System.Drawing.Point(9, 494);
			this.GuidePic4Bn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.GuidePic4Bn.Name = "GuidePic4Bn";
			this.GuidePic4Bn.Size = new System.Drawing.Size(45, 79);
			this.GuidePic4Bn.TabIndex = 170;
			this.GuidePic4Bn.UseVisualStyleBackColor = false;
			this.GuidePic4Bn.Click += new System.EventHandler(GuidePic4Bn_Click);
			this.GuidePic3Bn.BackColor = System.Drawing.SystemColors.HighlightText;
			this.GuidePic3Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("GuidePic3Bn.BackgroundImage");
			this.GuidePic3Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.GuidePic3Bn.FlatAppearance.BorderSize = 0;
			this.GuidePic3Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GuidePic3Bn.Font = new System.Drawing.Font("新細明體", 12f);
			this.GuidePic3Bn.Location = new System.Drawing.Point(9, 414);
			this.GuidePic3Bn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.GuidePic3Bn.Name = "GuidePic3Bn";
			this.GuidePic3Bn.Size = new System.Drawing.Size(45, 79);
			this.GuidePic3Bn.TabIndex = 170;
			this.GuidePic3Bn.UseVisualStyleBackColor = false;
			this.GuidePic3Bn.Click += new System.EventHandler(GuidePic3Bn_Click);
			this.GuidePic2Bn.BackColor = System.Drawing.SystemColors.HighlightText;
			this.GuidePic2Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("GuidePic2Bn.BackgroundImage");
			this.GuidePic2Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.GuidePic2Bn.FlatAppearance.BorderSize = 0;
			this.GuidePic2Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GuidePic2Bn.Font = new System.Drawing.Font("新細明體", 12f);
			this.GuidePic2Bn.Location = new System.Drawing.Point(9, 334);
			this.GuidePic2Bn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.GuidePic2Bn.Name = "GuidePic2Bn";
			this.GuidePic2Bn.Size = new System.Drawing.Size(45, 79);
			this.GuidePic2Bn.TabIndex = 170;
			this.GuidePic2Bn.UseVisualStyleBackColor = false;
			this.GuidePic2Bn.Click += new System.EventHandler(GuidePic2Bn_Click);
			this.DelGuideLedBn.BackgroundImage = SD3Soft.Properties.Resources.B_Del_ICON_01;
			this.DelGuideLedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DelGuideLedBn.FlatAppearance.BorderSize = 0;
			this.DelGuideLedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DelGuideLedBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.DelGuideLedBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DelGuideLedBn.Location = new System.Drawing.Point(1140, 166);
			this.DelGuideLedBn.Margin = new System.Windows.Forms.Padding(4);
			this.DelGuideLedBn.Name = "DelGuideLedBn";
			this.DelGuideLedBn.Size = new System.Drawing.Size(53, 50);
			this.DelGuideLedBn.TabIndex = 172;
			this.DelGuideLedBn.UseVisualStyleBackColor = true;
			this.DelGuideLedBn.Click += new System.EventHandler(DelGuideLedBn_Click);
			this.GuidePic1Bn.BackColor = System.Drawing.SystemColors.HighlightText;
			this.GuidePic1Bn.BackgroundImage = (System.Drawing.Image)resources.GetObject("GuidePic1Bn.BackgroundImage");
			this.GuidePic1Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.GuidePic1Bn.FlatAppearance.BorderSize = 0;
			this.GuidePic1Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.GuidePic1Bn.Font = new System.Drawing.Font("新細明體", 12f);
			this.GuidePic1Bn.Location = new System.Drawing.Point(9, 254);
			this.GuidePic1Bn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.GuidePic1Bn.Name = "GuidePic1Bn";
			this.GuidePic1Bn.Size = new System.Drawing.Size(45, 79);
			this.GuidePic1Bn.TabIndex = 170;
			this.GuidePic1Bn.UseVisualStyleBackColor = false;
			this.GuidePic1Bn.Click += new System.EventHandler(GuidePic1Bn_Click);
			this.SeqPreGuidePL.BackColor = System.Drawing.Color.FromArgb(70, 70, 70);
			this.SeqPreGuidePL.Controls.Add(this.OpenGuidePicBn);
			this.SeqPreGuidePL.Controls.Add(this.PreLed5PB);
			this.SeqPreGuidePL.Controls.Add(this.PreLed4PB);
			this.SeqPreGuidePL.Controls.Add(this.ShowAllPostionArmBn);
			this.SeqPreGuidePL.Controls.Add(this.RstAllGuideLedBn);
			this.SeqPreGuidePL.Controls.Add(this.PreLed3PB);
			this.SeqPreGuidePL.Controls.Add(this.NextLedPageBn);
			this.SeqPreGuidePL.Controls.Add(this.PreLedPageBn);
			this.SeqPreGuidePL.Controls.Add(this.PreLed2PB);
			this.SeqPreGuidePL.Controls.Add(this.PreLed1PB);
			this.SeqPreGuidePL.Location = new System.Drawing.Point(9, 166);
			this.SeqPreGuidePL.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.SeqPreGuidePL.Name = "SeqPreGuidePL";
			this.SeqPreGuidePL.Size = new System.Drawing.Size(787, 55);
			this.SeqPreGuidePL.TabIndex = 168;
			this.OpenGuidePicBn.BackgroundImage = SD3Soft.Properties.Resources.開啟舊檔_灰;
			this.OpenGuidePicBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.OpenGuidePicBn.FlatAppearance.BorderSize = 0;
			this.OpenGuidePicBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.OpenGuidePicBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.OpenGuidePicBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.OpenGuidePicBn.Location = new System.Drawing.Point(597, 2);
			this.OpenGuidePicBn.Margin = new System.Windows.Forms.Padding(4);
			this.OpenGuidePicBn.Name = "OpenGuidePicBn";
			this.OpenGuidePicBn.Size = new System.Drawing.Size(53, 50);
			this.OpenGuidePicBn.TabIndex = 177;
			this.OpenGuidePicBn.UseVisualStyleBackColor = true;
			this.OpenGuidePicBn.Click += new System.EventHandler(OpenGuidePicBn_Click);
			this.PreLed5PB.Image = (System.Drawing.Image)resources.GetObject("PreLed5PB.Image");
			this.PreLed5PB.Location = new System.Drawing.Point(392, 8);
			this.PreLed5PB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PreLed5PB.Name = "PreLed5PB";
			this.PreLed5PB.Size = new System.Drawing.Size(45, 45);
			this.PreLed5PB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PreLed5PB.TabIndex = 163;
			this.PreLed5PB.TabStop = false;
			this.PreLed4PB.Image = (System.Drawing.Image)resources.GetObject("PreLed4PB.Image");
			this.PreLed4PB.Location = new System.Drawing.Point(323, 8);
			this.PreLed4PB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PreLed4PB.Name = "PreLed4PB";
			this.PreLed4PB.Size = new System.Drawing.Size(45, 45);
			this.PreLed4PB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PreLed4PB.TabIndex = 164;
			this.PreLed4PB.TabStop = false;
			this.ShowAllPostionArmBn.BackgroundImage = SD3Soft.Properties.Resources.PageCurr;
			this.ShowAllPostionArmBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ShowAllPostionArmBn.FlatAppearance.BorderSize = 0;
			this.ShowAllPostionArmBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ShowAllPostionArmBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ShowAllPostionArmBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ShowAllPostionArmBn.Location = new System.Drawing.Point(711, 2);
			this.ShowAllPostionArmBn.Margin = new System.Windows.Forms.Padding(4);
			this.ShowAllPostionArmBn.Name = "ShowAllPostionArmBn";
			this.ShowAllPostionArmBn.Size = new System.Drawing.Size(53, 50);
			this.ShowAllPostionArmBn.TabIndex = 173;
			this.ShowAllPostionArmBn.UseVisualStyleBackColor = true;
			this.ShowAllPostionArmBn.Click += new System.EventHandler(ShowAllPostionArmBn_Click);
			this.RstAllGuideLedBn.BackgroundImage = SD3Soft.Properties.Resources.全部回復_灰;
			this.RstAllGuideLedBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.RstAllGuideLedBn.FlatAppearance.BorderSize = 0;
			this.RstAllGuideLedBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstAllGuideLedBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstAllGuideLedBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstAllGuideLedBn.Location = new System.Drawing.Point(653, 2);
			this.RstAllGuideLedBn.Margin = new System.Windows.Forms.Padding(4);
			this.RstAllGuideLedBn.Name = "RstAllGuideLedBn";
			this.RstAllGuideLedBn.Size = new System.Drawing.Size(53, 50);
			this.RstAllGuideLedBn.TabIndex = 173;
			this.RstAllGuideLedBn.UseVisualStyleBackColor = true;
			this.RstAllGuideLedBn.Click += new System.EventHandler(RstAllGuidePicBn_Click);
			this.PreLed3PB.Image = (System.Drawing.Image)resources.GetObject("PreLed3PB.Image");
			this.PreLed3PB.Location = new System.Drawing.Point(253, 8);
			this.PreLed3PB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PreLed3PB.Name = "PreLed3PB";
			this.PreLed3PB.Size = new System.Drawing.Size(45, 45);
			this.PreLed3PB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PreLed3PB.TabIndex = 165;
			this.PreLed3PB.TabStop = false;
			this.NextLedPageBn.BackgroundImage = SD3Soft.Properties.Resources.下頁;
			this.NextLedPageBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.NextLedPageBn.FlatAppearance.BorderSize = 0;
			this.NextLedPageBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.NextLedPageBn.Location = new System.Drawing.Point(443, 0);
			this.NextLedPageBn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.NextLedPageBn.Name = "NextLedPageBn";
			this.NextLedPageBn.Size = new System.Drawing.Size(55, 55);
			this.NextLedPageBn.TabIndex = 171;
			this.NextLedPageBn.UseVisualStyleBackColor = true;
			this.NextLedPageBn.Click += new System.EventHandler(NextLedPageBn_Click);
			this.PreLedPageBn.BackgroundImage = SD3Soft.Properties.Resources.上頁;
			this.PreLedPageBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.PreLedPageBn.FlatAppearance.BorderSize = 0;
			this.PreLedPageBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.PreLedPageBn.Location = new System.Drawing.Point(51, 0);
			this.PreLedPageBn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PreLedPageBn.Name = "PreLedPageBn";
			this.PreLedPageBn.Size = new System.Drawing.Size(55, 55);
			this.PreLedPageBn.TabIndex = 171;
			this.PreLedPageBn.UseVisualStyleBackColor = true;
			this.PreLedPageBn.Click += new System.EventHandler(PreLedPageBn_Click);
			this.PreLed2PB.Image = (System.Drawing.Image)resources.GetObject("PreLed2PB.Image");
			this.PreLed2PB.Location = new System.Drawing.Point(185, 8);
			this.PreLed2PB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PreLed2PB.Name = "PreLed2PB";
			this.PreLed2PB.Size = new System.Drawing.Size(45, 45);
			this.PreLed2PB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.PreLed2PB.TabIndex = 166;
			this.PreLed2PB.TabStop = false;
			this.PreLed1PB.Image = SD3Soft.Properties.Resources.LEDEdit_Gray;
			this.PreLed1PB.Location = new System.Drawing.Point(116, 8);
			this.PreLed1PB.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.PreLed1PB.Name = "PreLed1PB";
			this.PreLed1PB.Size = new System.Drawing.Size(45, 45);
			this.PreLed1PB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.PreLed1PB.TabIndex = 167;
			this.PreLed1PB.TabStop = false;
			this.XY_lab.AutoSize = true;
			this.XY_lab.Location = new System.Drawing.Point(8, 134);
			this.XY_lab.Name = "XY_lab";
			this.XY_lab.Size = new System.Drawing.Size(0, 20);
			this.XY_lab.TabIndex = 162;
			this.SeqPicEditPL.BackColor = System.Drawing.Color.Transparent;
			this.SeqPicEditPL.Location = new System.Drawing.Point(59, 222);
			this.SeqPicEditPL.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.SeqPicEditPL.Name = "SeqPicEditPL";
			this.SeqPicEditPL.Size = new System.Drawing.Size(736, 460);
			this.SeqPicEditPL.TabIndex = 169;
			this.tbSeqTitle.Font = new System.Drawing.Font("Arial Narrow", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.tbSeqTitle.Location = new System.Drawing.Point(91, 30);
			this.tbSeqTitle.Margin = new System.Windows.Forms.Padding(4);
			this.tbSeqTitle.Name = "tbSeqTitle";
			this.tbSeqTitle.Size = new System.Drawing.Size(672, 30);
			this.tbSeqTitle.TabIndex = 20;
			this.tbSeqTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.dataGridViewCheckBoxColumn1.HeaderText = "";
			this.dataGridViewCheckBoxColumn1.MinimumWidth = 6;
			this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
			this.dataGridViewCheckBoxColumn1.Width = 20;
			this.dataGridViewTextBoxColumn1.HeaderText = "ID";
			this.dataGridViewTextBoxColumn1.MinimumWidth = 6;
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.Width = 50;
			this.dataGridViewTextBoxColumn2.HeaderText = "Title";
			this.dataGridViewTextBoxColumn2.MinimumWidth = 6;
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.Width = 125;
			this.tbCurrentID.Font = new System.Drawing.Font("Arial Narrow", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.tbCurrentID.Location = new System.Drawing.Point(7, 30);
			this.tbCurrentID.Margin = new System.Windows.Forms.Padding(4);
			this.tbCurrentID.Name = "tbCurrentID";
			this.tbCurrentID.Size = new System.Drawing.Size(75, 30);
			this.tbCurrentID.TabIndex = 157;
			this.tbCurrentID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.groupBox1.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.groupBox1.Controls.Add(this.tbCurrentID);
			this.groupBox1.Controls.Add(this.SaveBn);
			this.groupBox1.Controls.Add(this.tbSeqTitle);
			this.groupBox1.Controls.Add(this.tp_Seqence);
			this.groupBox1.Location = new System.Drawing.Point(392, 24);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
			this.groupBox1.Size = new System.Drawing.Size(1467, 911);
			this.groupBox1.TabIndex = 160;
			this.groupBox1.TabStop = false;
			this.SaveBn.BackgroundImage = SD3Soft.Properties.Resources.存檔A;
			this.SaveBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SaveBn.FlatAppearance.BorderSize = 0;
			this.SaveBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SaveBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.SaveBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SaveBn.Location = new System.Drawing.Point(612, 822);
			this.SaveBn.Margin = new System.Windows.Forms.Padding(4);
			this.SaveBn.Name = "SaveBn";
			this.SaveBn.Size = new System.Drawing.Size(200, 50);
			this.SaveBn.TabIndex = 155;
			this.SaveBn.Text = "      Save";
			this.SaveBn.UseVisualStyleBackColor = true;
			this.SaveBn.Click += new System.EventHandler(SaveBn_Click);
			this.btn_ImportCSV.BackgroundImage = SD3Soft.Properties.Resources.FileWrite;
			this.btn_ImportCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ImportCSV.FlatAppearance.BorderSize = 0;
			this.btn_ImportCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ImportCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ImportCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ImportCSV.Location = new System.Drawing.Point(327, 15);
			this.btn_ImportCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ImportCSV.Name = "btn_ImportCSV";
			this.btn_ImportCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ImportCSV.TabIndex = 158;
			this.btn_ImportCSV.UseVisualStyleBackColor = true;
			this.btn_ImportCSV.Click += new System.EventHandler(btn_ImportCSV_Click);
			this.btn_ExportCSV.BackgroundImage = SD3Soft.Properties.Resources.FileRead;
			this.btn_ExportCSV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_ExportCSV.FlatAppearance.BorderSize = 0;
			this.btn_ExportCSV.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_ExportCSV.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_ExportCSV.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_ExportCSV.Location = new System.Drawing.Point(265, 15);
			this.btn_ExportCSV.Margin = new System.Windows.Forms.Padding(4);
			this.btn_ExportCSV.Name = "btn_ExportCSV";
			this.btn_ExportCSV.Size = new System.Drawing.Size(53, 50);
			this.btn_ExportCSV.TabIndex = 159;
			this.btn_ExportCSV.UseVisualStyleBackColor = true;
			this.btn_ExportCSV.Click += new System.EventHandler(btn_ExportCSV_Click);
			this.btn_DelID.BackgroundImage = SD3Soft.Properties.Resources.B_Del_ICON_01;
			this.btn_DelID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_DelID.FlatAppearance.BorderSize = 0;
			this.btn_DelID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_DelID.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_DelID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_DelID.Location = new System.Drawing.Point(15, 15);
			this.btn_DelID.Margin = new System.Windows.Forms.Padding(4);
			this.btn_DelID.Name = "btn_DelID";
			this.btn_DelID.Size = new System.Drawing.Size(53, 50);
			this.btn_DelID.TabIndex = 62;
			this.btn_DelID.UseVisualStyleBackColor = true;
			this.btn_DelID.Click += new System.EventHandler(btn_DelID_Click);
			this.btn_AddID.BackgroundImage = SD3Soft.Properties.Resources.B_新增_ICON_01;
			this.btn_AddID.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_AddID.FlatAppearance.BorderSize = 0;
			this.btn_AddID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_AddID.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_AddID.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_AddID.Location = new System.Drawing.Point(76, 15);
			this.btn_AddID.Margin = new System.Windows.Forms.Padding(4);
			this.btn_AddID.Name = "btn_AddID";
			this.btn_AddID.Size = new System.Drawing.Size(53, 50);
			this.btn_AddID.TabIndex = 61;
			this.btn_AddID.UseVisualStyleBackColor = true;
			this.btn_AddID.Click += new System.EventHandler(btn_AddID_Click);
			this.btnDownload.BackgroundImage = SD3Soft.Properties.Resources.PCUpload;
			this.btnDownload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnDownload.FlatAppearance.BorderSize = 0;
			this.btnDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnDownload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnDownload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnDownload.Location = new System.Drawing.Point(201, 15);
			this.btnDownload.Margin = new System.Windows.Forms.Padding(4);
			this.btnDownload.Name = "btnDownload";
			this.btnDownload.Size = new System.Drawing.Size(53, 50);
			this.btnDownload.TabIndex = 60;
			this.btnDownload.UseVisualStyleBackColor = true;
			this.btnDownload.Click += new System.EventHandler(btnDownload_Click);
			this.btnUpload.BackgroundImage = SD3Soft.Properties.Resources.PCDownload;
			this.btnUpload.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btnUpload.FlatAppearance.BorderSize = 0;
			this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnUpload.Font = new System.Drawing.Font("新細明體", 12f);
			this.btnUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnUpload.Location = new System.Drawing.Point(139, 15);
			this.btnUpload.Margin = new System.Windows.Forms.Padding(4);
			this.btnUpload.Name = "btnUpload";
			this.btnUpload.Size = new System.Drawing.Size(53, 50);
			this.btnUpload.TabIndex = 59;
			this.btnUpload.UseVisualStyleBackColor = true;
			this.btnUpload.Click += new System.EventHandler(btnUpload_Click);
			base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 15f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.ClientSize = new System.Drawing.Size(2000, 1000);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.btn_ImportCSV);
			base.Controls.Add(this.btn_ExportCSV);
			base.Controls.Add(this.btn_DelID);
			base.Controls.Add(this.btn_AddID);
			base.Controls.Add(this.btnDownload);
			base.Controls.Add(this.btnUpload);
			base.Controls.Add(this.dataGridView_Seq);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Margin = new System.Windows.Forms.Padding(4);
			base.Name = "Form200_Seq";
			this.Text = "Form1";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form200_Seq_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form200_Seq_FormClosed);
			base.Load += new System.EventHandler(Form200_Seq_Load);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Seq).EndInit();
			this.tp_Seqence.ResumeLayout(false);
			this.tpSeq_Normal.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_SeqParam).EndInit();
			this.tpSeq_Navigator.ResumeLayout(false);
			this.ShowGuidePL.ResumeLayout(false);
			this.ShowGuidePL.PerformLayout();
			this.CurrNumPL.ResumeLayout(false);
			this.CurrNumPL.PerformLayout();
			this.TargFlagPL.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
			this.PreFlagPL5.ResumeLayout(false);
			this.PreFlagPL4.ResumeLayout(false);
			this.PreFlagPL3.ResumeLayout(false);
			this.PreFlagPL2.ResumeLayout(false);
			this.PreFlagPL1.ResumeLayout(false);
			this.PositionArmGB.ResumeLayout(false);
			this.PositionArmGB.PerformLayout();
			this.SeqPreGuidePL.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)this.PreLed5PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed4PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed3PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed2PB).EndInit();
			((System.ComponentModel.ISupportInitialize)this.PreLed1PB).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			base.ResumeLayout(false);
		}
	}
}
