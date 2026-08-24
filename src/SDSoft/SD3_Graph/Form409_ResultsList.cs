using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form409_ResultsList : Form
	{
		private GlobalVar GB;

		private TCPclient TCP;

		public DataTable dt_LED = new DataTable();

		private Image[] LedImg = new Image[10];

		private Image[] LockUnLockImg = new Image[2];

		private int PageBase = 1;

		private int Page_Axis = 0;

		private IContainer components = null;

		private Label lab_HanderTitle;

		private Label CloseBn;

		private DataGridView dataGridView_ResultsList;

		private Button RstNextBn;

		private Button RstPrevBn;

		private TextBox PageTB;

		private Button ResetTGSTBnT;

		private Button ResetOPTimeBnT;

		private Button ResetTGNOKCntBnT;

		private Button ResetLONOKCntBnT;

		private Label lab_TGStatus;

		private Label lab_OPTime;

		private Label lab_TGNOKCnt;

		private Label lab_LONOKCnt;

		private TextBox LONOKCntTB;

		private TextBox TGNOKCntTB;

		private TextBox OPTimeTB;

		private Button ResetTGSTBn;

		private Button ResetOPTimeBn;

		private Button ResetTGNOKCntBn;

		private Button ResetLONOKCntBn;

		private Panel ResetTGSTPL;

		private Panel ResetOPTimePL;

		private Panel ResetTGNOKCntPL;

		private Panel ResetLONOKCntPL;

		public Form409_ResultsList(GlobalVar GB, TCPclient TCP, int Axis)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			LedImg[0] = Resources.NonLed;
			LedImg[1] = Resources.GrayLed;
			LedImg[2] = Resources.GreenLed;
			LedImg[3] = Resources.NonLed;
			LedImg[4] = Resources.YellowLed;
			LedImg[5] = Resources.NonLed;
			LedImg[6] = Resources.NonLed;
			LedImg[7] = Resources.NonLed;
			LedImg[8] = Resources.RedLed;
			LedImg[9] = Resources.NonLed;
			LockUnLockImg[0] = Resources.Prohibit;
			LockUnLockImg[1] = null;
			dt_LED.Columns.Add("LED1", typeof(Image));
			dt_LED.Columns.Add("LED2", typeof(Image));
			dt_LED.Columns.Add("LED3", typeof(Image));
			dt_LED.Columns.Add("LED4", typeof(Image));
			dt_LED.Columns.Add("LED5", typeof(Image));
			dt_LED.Columns.Add("LED6", typeof(Image));
			dt_LED.Columns.Add("LED7", typeof(Image));
			dt_LED.Columns.Add("LED8", typeof(Image));
			dt_LED.Columns.Add("LED9", typeof(Image));
			dt_LED.Columns.Add("LED10", typeof(Image));
			PageBase = 1;
			UpdateUI(Axis);
			Page_Axis = Axis;
			GB.Form409Event = new AutoResetEvent(false);
			GB.Form409ThreadFlag = true;
			ThreadStart MissionForm409 = Form409Thread;
			GB.MissionForm409Thread = new Thread(MissionForm409);
			GB.MissionForm409Thread.Start();
			IsProhibitBtn();
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			dataGridView1.BackgroundColor = Color.White;
			dataGridView1.DefaultCellStyle.BackColor = Color.White;
			dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
			dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
			dataGridView1.RowTemplate.Height = 30;
			dataGridView1.RowHeadersVisible = false;
			for (int i = 0; i < 10; i++)
			{
				dataGridView1.Columns[i].HeaderText = ((PageBase - 1) * 100 + i + 1).ToString();
				((DataGridViewImageColumn)dataGridView1.Columns[i]).ImageLayout = DataGridViewImageCellLayout.Zoom;
			}
		}

		public void UpdateUI(int Axis)
		{
			TCP.FSIDRead_ByTCP(453, 0, (ushort)Page_Axis, 0, 0, 0);
			dt_LED.Rows.Clear();
			int PageNumNo = 0;
			int CurrScrewNum = 0;
			int TotalScrewNum = 0;
			if (Axis == 0)
			{
				CurrScrewNum = GB.TcpStatus.Detail.T1StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T1StA.CurrentSequence_L_09;
				TotalScrewNum = GB.TcpStatus.Detail.T1StA.TotalScrewQty_H_27 * 65536 + GB.TcpStatus.Detail.T1StA.TotalScrewQty_L_26;
			}
			else
			{
				CurrScrewNum = GB.TcpStatus.Detail.T2StA.CurrentSequence_H_10 * 65536 + GB.TcpStatus.Detail.T2StA.CurrentSequence_L_09;
				TotalScrewNum = GB.TcpStatus.Detail.T2StA.TotalScrewQty_H_27 * 65536 + GB.TcpStatus.Detail.T2StA.TotalScrewQty_L_26;
			}
			for (int n = 0; n < 10; n++)
			{
				DataRow LEDRow = dt_LED.NewRow();
				for (int i = 0; i < 10; i++)
				{
					PageNumNo = (PageBase - 1) * 100 + n * 10 + i;
					if (TotalScrewNum == 999999)
					{
						LEDRow[i] = LedImg[2];
					}
					else if (PageNumNo < CurrScrewNum)
					{
						if (PageBase * 10 < 1000)
						{
							int Oxy = GB.ResultLedST(Axis, PageNumNo);
							LEDRow[i] = LedImg[Oxy];
						}
						else
						{
							LEDRow[i] = LedImg[2];
						}
					}
					else if (PageNumNo == CurrScrewNum)
					{
						LEDRow[i] = LedImg[4];
					}
					else if (PageNumNo < TotalScrewNum)
					{
						LEDRow[i] = LedImg[1];
					}
					else
					{
						LEDRow[i] = LedImg[0];
					}
				}
				dt_LED.Rows.Add(LEDRow);
			}
			dataGridView_ResultsList.DataSource = dt_LED;
			loadGrid1(dataGridView_ResultsList);
			PageTB.Text = PageBase.ToString();
			int seconds = 0;
			if (Axis == 0)
			{
				seconds = GB.TcpStatus.Detail.T1StA.RemainingOperationTime_H_49 * 65536 + GB.TcpStatus.Detail.T1StA.RemainingOperationTime_L_48;
				TGNOKCntTB.Text = (GB.TcpStatus.Detail.T1StA.TighteningNOKCnt_H_14 * 65536 + GB.TcpStatus.Detail.T1StA.TighteningNOKCnt_L_13).ToString();
				LONOKCntTB.Text = (GB.TcpStatus.Detail.T1StA.LooseningOKCnt_H_16 * 65536 + GB.TcpStatus.Detail.T1StA.LooseningNOKCnt_L_17).ToString();
			}
			else
			{
				seconds = GB.TcpStatus.Detail.T2StA.RemainingOperationTime_H_49 * 65536 + GB.TcpStatus.Detail.T2StA.RemainingOperationTime_L_48;
				TGNOKCntTB.Text = (GB.TcpStatus.Detail.T2StA.TighteningNOKCnt_H_14 * 65536 + GB.TcpStatus.Detail.T2StA.TighteningNOKCnt_L_13).ToString();
				LONOKCntTB.Text = (GB.TcpStatus.Detail.T2StA.LooseningOKCnt_H_16 * 65536 + GB.TcpStatus.Detail.T2StA.LooseningNOKCnt_L_17).ToString();
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
			DateTime dateTime = DateTime.MinValue.Add(timeSpan);
			OPTimeTB.Text = dateTime.ToString("HH:mm:ss");
			uint AdvancedSettings = ((Axis != 0) ? GB.UISys.RunningSrcY.AdvancedSettings : GB.UISys.RunningSrcX.AdvancedSettings);
			if ((AdvancedSettings & 3) == 0)
			{
				lab_TGStatus.Visible = false;
				ResetTGSTPL.Visible = false;
			}
			else
			{
				lab_TGStatus.Visible = true;
				ResetTGSTPL.Visible = true;
			}
			if ((AdvancedSettings & 4) == 0)
			{
				lab_TGNOKCnt.Visible = false;
				TGNOKCntTB.Visible = false;
				ResetTGNOKCntPL.Visible = false;
			}
			else
			{
				lab_TGNOKCnt.Visible = true;
				TGNOKCntTB.Visible = true;
				ResetTGNOKCntPL.Visible = true;
			}
			if ((AdvancedSettings & 8) == 0)
			{
				lab_LONOKCnt.Visible = false;
				LONOKCntTB.Visible = false;
				ResetLONOKCntPL.Visible = false;
			}
			else
			{
				lab_LONOKCnt.Visible = true;
				LONOKCntTB.Visible = true;
				ResetLONOKCntPL.Visible = true;
			}
			if ((AdvancedSettings & 0x200) == 0)
			{
				lab_OPTime.Visible = false;
				OPTimeTB.Visible = false;
				ResetOPTimePL.Visible = false;
			}
			else
			{
				lab_OPTime.Visible = true;
				OPTimeTB.Visible = true;
				ResetOPTimePL.Visible = true;
			}
		}

		public void Form409Thread()
		{
			while (GB.Form409ThreadFlag)
			{
				if (GB.Form409Event != null)
				{
					GB.Form409Event.WaitOne();
					GB.Form409ThreadWait = true;
					if (!GB.Form409ThreadFlag)
					{
						break;
					}
				}
				if (base.IsHandleCreated)
				{
					Invoke((Action)delegate
					{
						UpdateUI(Page_Axis);
					});
				}
			}
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form409_ResultsList_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void RstNextBn_Click(object sender, EventArgs e)
		{
			if (PageBase < 99999)
			{
				PageBase++;
				UpdateUI(Page_Axis);
			}
		}

		private void RstPrevBn_Click(object sender, EventArgs e)
		{
			if (PageBase > 0)
			{
				PageBase--;
				UpdateUI(Page_Axis);
			}
		}

		private void ResetTGSTBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(412, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void ResetOPTimeBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(412, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void ResetTGNOKCntBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(409, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void ResetLONOKCntBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(410, 0, (ushort)Page_Axis, 0, 0, 0);
		}

		private void Form409_ResultsList_FormClosed(object sender, FormClosedEventArgs e)
		{
			Form_closed();
		}

		private void Form_closed()
		{
			GB.Form409ThreadFlag = false;
			if (GB.MissionForm409Thread != null)
			{
				GB.MissionForm409Thread.Abort();
			}
			if (GB.Form409Event != null)
			{
				if (GB.Form409ThreadWait)
				{
					GB.Form409Event.Set();
					GB.Form409ThreadWait = false;
				}
				GB.Form409Event.Close();
			}
		}

		private void IsProhibitBtn()
		{
			GB.PermissOfUserID_HidePic(ref ResetTGSTBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref ResetOPTimeBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref ResetTGNOKCntBn, ref LockUnLockImg, 32);
			GB.PermissOfUserID_HidePic(ref ResetLONOKCntBn, ref LockUnLockImg, 32);
		}

		private void Form409_ResultsList_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void Form409_ResultsList_FormClosing(object sender, FormClosingEventArgs e)
		{
			Form_closed();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form409_ResultsList));
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.dataGridView_ResultsList = new System.Windows.Forms.DataGridView();
			this.PageTB = new System.Windows.Forms.TextBox();
			this.lab_TGStatus = new System.Windows.Forms.Label();
			this.lab_OPTime = new System.Windows.Forms.Label();
			this.lab_TGNOKCnt = new System.Windows.Forms.Label();
			this.lab_LONOKCnt = new System.Windows.Forms.Label();
			this.LONOKCntTB = new System.Windows.Forms.TextBox();
			this.TGNOKCntTB = new System.Windows.Forms.TextBox();
			this.OPTimeTB = new System.Windows.Forms.TextBox();
			this.ResetLONOKCntBnT = new System.Windows.Forms.Button();
			this.ResetTGNOKCntBnT = new System.Windows.Forms.Button();
			this.ResetOPTimeBnT = new System.Windows.Forms.Button();
			this.ResetTGSTBnT = new System.Windows.Forms.Button();
			this.RstNextBn = new System.Windows.Forms.Button();
			this.RstPrevBn = new System.Windows.Forms.Button();
			this.ResetTGSTBn = new System.Windows.Forms.Button();
			this.ResetOPTimeBn = new System.Windows.Forms.Button();
			this.ResetTGNOKCntBn = new System.Windows.Forms.Button();
			this.ResetLONOKCntBn = new System.Windows.Forms.Button();
			this.ResetTGSTPL = new System.Windows.Forms.Panel();
			this.ResetOPTimePL = new System.Windows.Forms.Panel();
			this.ResetTGNOKCntPL = new System.Windows.Forms.Panel();
			this.ResetLONOKCntPL = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ResultsList).BeginInit();
			this.ResetTGSTPL.SuspendLayout();
			this.ResetOPTimePL.SuspendLayout();
			this.ResetTGNOKCntPL.SuspendLayout();
			this.ResetLONOKCntPL.SuspendLayout();
			base.SuspendLayout();
			this.lab_HanderTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_HanderTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_HanderTitle.Location = new System.Drawing.Point(1, -1);
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.lab_HanderTitle.Size = new System.Drawing.Size(600, 35);
			this.lab_HanderTitle.TabIndex = 60;
			this.lab_HanderTitle.Text = "Title";
			this.lab_HanderTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(569, 2);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.dataGridView_ResultsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_ResultsList.Location = new System.Drawing.Point(96, 47);
			this.dataGridView_ResultsList.Name = "dataGridView_ResultsList";
			this.dataGridView_ResultsList.RowHeadersWidth = 51;
			this.dataGridView_ResultsList.RowTemplate.Height = 24;
			this.dataGridView_ResultsList.Size = new System.Drawing.Size(400, 328);
			this.dataGridView_ResultsList.TabIndex = 133;
			this.PageTB.Enabled = false;
			this.PageTB.Font = new System.Drawing.Font("新細明體", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 136);
			this.PageTB.Location = new System.Drawing.Point(261, 553);
			this.PageTB.Name = "PageTB";
			this.PageTB.Size = new System.Drawing.Size(63, 31);
			this.PageTB.TabIndex = 160;
			this.PageTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_TGStatus.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TGStatus.Location = new System.Drawing.Point(75, 376);
			this.lab_TGStatus.Name = "lab_TGStatus";
			this.lab_TGStatus.Size = new System.Drawing.Size(421, 35);
			this.lab_TGStatus.TabIndex = 162;
			this.lab_TGStatus.Text = "Tighening Status";
			this.lab_TGStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_OPTime.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_OPTime.Location = new System.Drawing.Point(75, 421);
			this.lab_OPTime.Name = "lab_OPTime";
			this.lab_OPTime.Size = new System.Drawing.Size(280, 35);
			this.lab_OPTime.TabIndex = 162;
			this.lab_OPTime.Text = "Remaining Operation Time";
			this.lab_OPTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TGNOKCnt.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TGNOKCnt.Location = new System.Drawing.Point(75, 463);
			this.lab_TGNOKCnt.Name = "lab_TGNOKCnt";
			this.lab_TGNOKCnt.Size = new System.Drawing.Size(340, 35);
			this.lab_TGNOKCnt.TabIndex = 162;
			this.lab_TGNOKCnt.Text = "Single Screw Tightening NOK Count";
			this.lab_TGNOKCnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_LONOKCnt.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_LONOKCnt.Location = new System.Drawing.Point(75, 509);
			this.lab_LONOKCnt.Name = "lab_LONOKCnt";
			this.lab_LONOKCnt.Size = new System.Drawing.Size(340, 35);
			this.lab_LONOKCnt.TabIndex = 162;
			this.lab_LONOKCnt.Text = "Single Screw Loosening NOK Count";
			this.lab_LONOKCnt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.LONOKCntTB.Font = new System.Drawing.Font("Arial", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.LONOKCntTB.Location = new System.Drawing.Point(421, 509);
			this.LONOKCntTB.Name = "LONOKCntTB";
			this.LONOKCntTB.ReadOnly = true;
			this.LONOKCntTB.Size = new System.Drawing.Size(75, 35);
			this.LONOKCntTB.TabIndex = 163;
			this.LONOKCntTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TGNOKCntTB.Font = new System.Drawing.Font("Arial", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.TGNOKCntTB.Location = new System.Drawing.Point(421, 463);
			this.TGNOKCntTB.Name = "TGNOKCntTB";
			this.TGNOKCntTB.ReadOnly = true;
			this.TGNOKCntTB.Size = new System.Drawing.Size(75, 35);
			this.TGNOKCntTB.TabIndex = 163;
			this.TGNOKCntTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.OPTimeTB.Font = new System.Drawing.Font("Arial", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.OPTimeTB.Location = new System.Drawing.Point(361, 421);
			this.OPTimeTB.Name = "OPTimeTB";
			this.OPTimeTB.ReadOnly = true;
			this.OPTimeTB.Size = new System.Drawing.Size(135, 35);
			this.OPTimeTB.TabIndex = 163;
			this.OPTimeTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.ResetLONOKCntBnT.BackColor = System.Drawing.Color.Transparent;
			this.ResetLONOKCntBnT.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetLONOKCntBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetLONOKCntBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetLONOKCntBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetLONOKCntBnT.Image = (System.Drawing.Image)resources.GetObject("ResetLONOKCntBnT.Image");
			this.ResetLONOKCntBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetLONOKCntBnT.Location = new System.Drawing.Point(6, 1);
			this.ResetLONOKCntBnT.Name = "ResetLONOKCntBnT";
			this.ResetLONOKCntBnT.Size = new System.Drawing.Size(50, 36);
			this.ResetLONOKCntBnT.TabIndex = 161;
			this.ResetLONOKCntBnT.UseVisualStyleBackColor = false;
			this.ResetTGNOKCntBnT.BackColor = System.Drawing.Color.Transparent;
			this.ResetTGNOKCntBnT.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetTGNOKCntBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetTGNOKCntBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetTGNOKCntBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetTGNOKCntBnT.Image = (System.Drawing.Image)resources.GetObject("ResetTGNOKCntBnT.Image");
			this.ResetTGNOKCntBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetTGNOKCntBnT.Location = new System.Drawing.Point(6, 1);
			this.ResetTGNOKCntBnT.Name = "ResetTGNOKCntBnT";
			this.ResetTGNOKCntBnT.Size = new System.Drawing.Size(50, 36);
			this.ResetTGNOKCntBnT.TabIndex = 161;
			this.ResetTGNOKCntBnT.UseVisualStyleBackColor = false;
			this.ResetOPTimeBnT.BackColor = System.Drawing.Color.Transparent;
			this.ResetOPTimeBnT.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetOPTimeBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetOPTimeBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetOPTimeBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetOPTimeBnT.Image = (System.Drawing.Image)resources.GetObject("ResetOPTimeBnT.Image");
			this.ResetOPTimeBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetOPTimeBnT.Location = new System.Drawing.Point(6, 0);
			this.ResetOPTimeBnT.Name = "ResetOPTimeBnT";
			this.ResetOPTimeBnT.Size = new System.Drawing.Size(50, 36);
			this.ResetOPTimeBnT.TabIndex = 161;
			this.ResetOPTimeBnT.UseVisualStyleBackColor = false;
			this.ResetTGSTBnT.BackColor = System.Drawing.Color.Transparent;
			this.ResetTGSTBnT.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetTGSTBnT.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetTGSTBnT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetTGSTBnT.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetTGSTBnT.Image = (System.Drawing.Image)resources.GetObject("ResetTGSTBnT.Image");
			this.ResetTGSTBnT.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetTGSTBnT.Location = new System.Drawing.Point(6, 0);
			this.ResetTGSTBnT.Name = "ResetTGSTBnT";
			this.ResetTGSTBnT.Size = new System.Drawing.Size(50, 36);
			this.ResetTGSTBnT.TabIndex = 161;
			this.ResetTGSTBnT.UseVisualStyleBackColor = false;
			this.RstNextBn.BackColor = System.Drawing.Color.Transparent;
			this.RstNextBn.BackgroundImage = SD3Soft.Properties.Resources.下頁按鍵02;
			this.RstNextBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstNextBn.FlatAppearance.BorderSize = 0;
			this.RstNextBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstNextBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstNextBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstNextBn.Location = new System.Drawing.Point(334, 553);
			this.RstNextBn.Name = "RstNextBn";
			this.RstNextBn.Size = new System.Drawing.Size(52, 40);
			this.RstNextBn.TabIndex = 159;
			this.RstNextBn.UseVisualStyleBackColor = false;
			this.RstNextBn.Click += new System.EventHandler(RstNextBn_Click);
			this.RstPrevBn.BackColor = System.Drawing.Color.Transparent;
			this.RstPrevBn.BackgroundImage = SD3Soft.Properties.Resources.上頁按鍵02;
			this.RstPrevBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.RstPrevBn.FlatAppearance.BorderSize = 0;
			this.RstPrevBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.RstPrevBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.RstPrevBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RstPrevBn.Location = new System.Drawing.Point(199, 553);
			this.RstPrevBn.Name = "RstPrevBn";
			this.RstPrevBn.Size = new System.Drawing.Size(52, 40);
			this.RstPrevBn.TabIndex = 158;
			this.RstPrevBn.UseVisualStyleBackColor = false;
			this.RstPrevBn.Click += new System.EventHandler(RstPrevBn_Click);
			this.ResetTGSTBn.BackColor = System.Drawing.Color.Transparent;
			this.ResetTGSTBn.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetTGSTBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetTGSTBn.FlatAppearance.BorderSize = 0;
			this.ResetTGSTBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetTGSTBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetTGSTBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetTGSTBn.Location = new System.Drawing.Point(6, 0);
			this.ResetTGSTBn.Name = "ResetTGSTBn";
			this.ResetTGSTBn.Size = new System.Drawing.Size(50, 36);
			this.ResetTGSTBn.TabIndex = 161;
			this.ResetTGSTBn.UseVisualStyleBackColor = false;
			this.ResetTGSTBn.Click += new System.EventHandler(ResetTGSTBn_Click);
			this.ResetOPTimeBn.BackColor = System.Drawing.Color.Transparent;
			this.ResetOPTimeBn.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetOPTimeBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetOPTimeBn.FlatAppearance.BorderSize = 0;
			this.ResetOPTimeBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetOPTimeBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetOPTimeBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetOPTimeBn.Location = new System.Drawing.Point(6, 0);
			this.ResetOPTimeBn.Name = "ResetOPTimeBn";
			this.ResetOPTimeBn.Size = new System.Drawing.Size(50, 36);
			this.ResetOPTimeBn.TabIndex = 161;
			this.ResetOPTimeBn.UseVisualStyleBackColor = false;
			this.ResetOPTimeBn.Click += new System.EventHandler(ResetOPTimeBn_Click);
			this.ResetTGNOKCntBn.BackColor = System.Drawing.Color.Transparent;
			this.ResetTGNOKCntBn.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetTGNOKCntBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetTGNOKCntBn.FlatAppearance.BorderSize = 0;
			this.ResetTGNOKCntBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetTGNOKCntBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetTGNOKCntBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetTGNOKCntBn.Location = new System.Drawing.Point(6, 1);
			this.ResetTGNOKCntBn.Name = "ResetTGNOKCntBn";
			this.ResetTGNOKCntBn.Size = new System.Drawing.Size(50, 36);
			this.ResetTGNOKCntBn.TabIndex = 161;
			this.ResetTGNOKCntBn.UseVisualStyleBackColor = false;
			this.ResetTGNOKCntBn.Click += new System.EventHandler(ResetTGNOKCntBn_Click);
			this.ResetLONOKCntBn.BackColor = System.Drawing.Color.Transparent;
			this.ResetLONOKCntBn.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			this.ResetLONOKCntBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ResetLONOKCntBn.FlatAppearance.BorderSize = 0;
			this.ResetLONOKCntBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ResetLONOKCntBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ResetLONOKCntBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ResetLONOKCntBn.Location = new System.Drawing.Point(6, 1);
			this.ResetLONOKCntBn.Name = "ResetLONOKCntBn";
			this.ResetLONOKCntBn.Size = new System.Drawing.Size(50, 36);
			this.ResetLONOKCntBn.TabIndex = 161;
			this.ResetLONOKCntBn.UseVisualStyleBackColor = false;
			this.ResetLONOKCntBn.Click += new System.EventHandler(ResetLONOKCntBn_Click);
			this.ResetTGSTPL.Controls.Add(this.ResetTGSTBn);
			this.ResetTGSTPL.Controls.Add(this.ResetTGSTBnT);
			this.ResetTGSTPL.Location = new System.Drawing.Point(507, 373);
			this.ResetTGSTPL.Name = "ResetTGSTPL";
			this.ResetTGSTPL.Size = new System.Drawing.Size(63, 40);
			this.ResetTGSTPL.TabIndex = 164;
			this.ResetOPTimePL.Controls.Add(this.ResetOPTimeBn);
			this.ResetOPTimePL.Controls.Add(this.ResetOPTimeBnT);
			this.ResetOPTimePL.Location = new System.Drawing.Point(507, 418);
			this.ResetOPTimePL.Name = "ResetOPTimePL";
			this.ResetOPTimePL.Size = new System.Drawing.Size(63, 40);
			this.ResetOPTimePL.TabIndex = 164;
			this.ResetTGNOKCntPL.Controls.Add(this.ResetTGNOKCntBn);
			this.ResetTGNOKCntPL.Controls.Add(this.ResetTGNOKCntBnT);
			this.ResetTGNOKCntPL.Location = new System.Drawing.Point(507, 460);
			this.ResetTGNOKCntPL.Name = "ResetTGNOKCntPL";
			this.ResetTGNOKCntPL.Size = new System.Drawing.Size(63, 40);
			this.ResetTGNOKCntPL.TabIndex = 164;
			this.ResetLONOKCntPL.Controls.Add(this.ResetLONOKCntBn);
			this.ResetLONOKCntPL.Controls.Add(this.ResetLONOKCntBnT);
			this.ResetLONOKCntPL.Location = new System.Drawing.Point(507, 506);
			this.ResetLONOKCntPL.Name = "ResetLONOKCntPL";
			this.ResetLONOKCntPL.Size = new System.Drawing.Size(63, 40);
			this.ResetLONOKCntPL.TabIndex = 164;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(600, 623);
			base.Controls.Add(this.OPTimeTB);
			base.Controls.Add(this.TGNOKCntTB);
			base.Controls.Add(this.LONOKCntTB);
			base.Controls.Add(this.lab_LONOKCnt);
			base.Controls.Add(this.lab_TGNOKCnt);
			base.Controls.Add(this.lab_OPTime);
			base.Controls.Add(this.lab_TGStatus);
			base.Controls.Add(this.PageTB);
			base.Controls.Add(this.RstNextBn);
			base.Controls.Add(this.RstPrevBn);
			base.Controls.Add(this.dataGridView_ResultsList);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_HanderTitle);
			base.Controls.Add(this.ResetLONOKCntPL);
			base.Controls.Add(this.ResetTGNOKCntPL);
			base.Controls.Add(this.ResetOPTimePL);
			base.Controls.Add(this.ResetTGSTPL);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form409_ResultsList";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form409_ResultsList";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form409_ResultsList_FormClosing);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form409_ResultsList_FormClosed);
			base.Load += new System.EventHandler(Form409_ResultsList_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form409_ResultsList_Paint);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ResultsList).EndInit();
			this.ResetTGSTPL.ResumeLayout(false);
			this.ResetOPTimePL.ResumeLayout(false);
			this.ResetTGNOKCntPL.ResumeLayout(false);
			this.ResetLONOKCntPL.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
