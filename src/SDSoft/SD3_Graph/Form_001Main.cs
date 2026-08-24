using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form_001Main : Form
	{
		private Form activeForm = null;

		public static GlobalVar GB = new GlobalVar();

		public static TCPclient TCP = new TCPclient(GB);

		public static TransferCSV TrCSV = new TransferCSV(GB, TCP);

		public static FTPServer FTPSlave = new FTPServer(GB);

		private Image[] LockUnLockImg = new Image[2];

		private Image[] OffOnImg = new Image[2];

		private Image[] IconA = new Image[8];

		private Image[] IconB = new Image[8];

		private Image[] OffOnlineImg = new Image[2];

		public uint Page_Axis = 0u;

		private string AlarmStr = "";

		private int ChagePage = 0;

		private bool SW = false;

		private uint OrgReportID = 0u;

		private ushort CurveAllPoint = 0;

		private ushort CurvePointP0to2 = 0;

		private ushort CurvePointP2to4 = 0;

		private ushort CurvePointP4to6 = 0;

		private ushort CurvePointP6to8 = 0;

		private ushort LastPupWindowID = 0;

		private Point _scrollPosition;

		public const int LangJPCB = 3;

		private string VerStr = "";

		private int CurrSDVer = 0;

		private IContainer components = null;

		private Panel panelChildForm;

		private ComboBox cbLanguage;

		private Button SettingBn;

		private Label lab_AlarmMsgBackgroud;

		private Button ParamBn;

		private Button SeqBn;

		private Button SrcBn;

		private Button ResultBn;

		private Button CtrlBn;

		private Button ToolBn;

		private Button ReportBn;

		private Button HelpBn;

		private Button RstResetBn;

		private PictureBox OnOfflinePB;

		private Label lab_HanderTitle;

		private Label label1;

		private Label lab_UserName;

		private PictureBox UserBn;

		private Button ParamBnT;

		private Button SeqBnT;

		private Button SrcBnT;

		private Button CtrlBnT;

		private Button ToolBnT;

		private Button UpdateFWBn;

		public Form_001Main()
		{
			InitializeComponent();
			UpdateFWBn.Visible = false;
			base.WindowState = FormWindowState.Maximized;
			VerStr = Process.GetCurrentProcess().ProcessName;
			Text = VerStr;
			string[] parts = VerStr.Split('.');
			if (parts.Length >= 4)
			{
				int part7 = int.Parse(parts[parts.Length - 2]);
				int part8 = int.Parse(parts[parts.Length - 1]);
				CurrSDVer = part7 * 100 + part8;
			}
			else
			{
				CurrSDVer = 999;
			}
			_scrollPosition = base.AutoScrollPosition;
			GB.FSModelTypeInfo.MesModelType = 1;
			GB.DefCtrlTable(true, 0);
			GB.DefToolTable(true, 0, 3);
			GB.DefToolTable(true, 1, 3);
			GB.FSReportWatchList.AngleType1 = 99;
			GB.FSReportWatchList.AngleType2 = 99;
			GB.FSReportWatchList.TorqueType1 = 99;
			GB.FSToolXActive.ActiveEnable = 1;
			GB.FSToolYActive.ActiveEnable = 0;
			OffOnImg[0] = Resources.Btn180W;
			OffOnImg[1] = Resources.Btn180B;
			IconA[0] = Resources.ParamA;
			IconA[1] = Resources.SeqA;
			IconA[2] = Resources.SrcA;
			IconA[3] = Resources.ResultA;
			IconA[4] = Resources.CtrlA;
			IconA[5] = Resources.ToolA;
			IconA[6] = Resources.ReportA;
			IconA[7] = Resources.HelpA;
			IconB[0] = Resources.ParamB;
			IconB[1] = Resources.SeqB;
			IconB[2] = Resources.SrcB;
			IconB[3] = Resources.ResultB;
			IconB[4] = Resources.CtrlB;
			IconB[5] = Resources.ToolB;
			IconB[6] = Resources.ReportB;
			IconB[7] = Resources.HelpB;
			OffOnlineImg[0] = Resources.Offline;
			OffOnlineImg[1] = Resources.Online;
			LockUnLockImg[0] = Resources.Prohibit_Big;
			LockUnLockImg[1] = null;
			GB.ExFSUser.LastUserID = 99u;
			cbLanguage.SelectedIndexChanged -= cbLanguage_SelectedIndexChanged;
			cbLanguage.Items.Add(new ComboBoxItem("0", "繁體中文"));
			cbLanguage.Items.Add(new ComboBoxItem("1", "English"));
			cbLanguage.Items.Add(new ComboBoxItem("2", "简体中文"));
			cbLanguage.Items.Add(new ComboBoxItem("3", "日本語"));
			switch (MultiLanguage.GetDefaultLanguage())
			{
			case "Chinese":
				cbLanguage.SelectedIndex = 0;
				break;
			case "Sample":
				cbLanguage.SelectedIndex = 2;
				break;
			case "Japan":
				cbLanguage.SelectedIndex = 3;
				break;
			default:
				cbLanguage.SelectedIndex = 1;
				break;
			}
			cbLanguage.SelectedIndexChanged += cbLanguage_SelectedIndexChanged;
			MultiLanguage.LoadLanguage(this);
			Process currentProcess = Process.GetCurrentProcess();
			Console.WriteLine("use system memory size:" + ((double)currentProcess.PrivateMemorySize64 / 1048576.0).ToString("F3"));
		}

		private void Form_001Main_Load(object sender, EventArgs e)
		{
			base.AutoScaleMode = AutoScaleMode.Dpi;
			Point point = (base.AutoScrollPosition = new Point(0, 0));
			_scrollPosition = point;
			if (!int.TryParse(MultiLanguage.GetDefaultIsAutoSize(), out var result))
			{
				result = 0;
			}
			GB.UISys.AutoFit = result;
			switch (result)
			{
			case 1:
				FormControlZoom.ScreenWidth = 1280;
				FormControlZoom.ScreenHeight = 720;
				AutoScroll = true;
				break;
			case 2:
				FormControlZoom.ScreenWidth = 1600;
				FormControlZoom.ScreenHeight = 960;
				AutoScroll = true;
				break;
			case 3:
				FormControlZoom.ScreenWidth = 1800;
				FormControlZoom.ScreenHeight = 1080;
				AutoScroll = true;
				break;
			default:
			{
				Screen currentScreen = Screen.FromControl(this);
				FormControlZoom.ScreenWidth = currentScreen.Bounds.Width;
				FormControlZoom.ScreenHeight = currentScreen.Bounds.Height;
				AutoScroll = false;
				break;
			}
			}
			FormControlZoom.ScreenWidthZoom = (float)FormControlZoom.ScreenWidth / 1600f;
			FormControlZoom.ScreenHeightZoom = (float)FormControlZoom.ScreenHeight / 900f;
			FormControlZoom.ScreenFontZoom = ((FormControlZoom.ScreenWidthZoom < FormControlZoom.ScreenHeightZoom) ? FormControlZoom.ScreenWidthZoom : FormControlZoom.ScreenHeightZoom);
			FormControlZoom.SetControlsMain(this);
			panelChildForm.Location = new Point(lab_HanderTitle.Location.X + lab_HanderTitle.Size.Width, lab_HanderTitle.Location.Y);
			int HorizontalScrollSize = (int)(1554f * FormControlZoom.ScreenWidthZoom);
			int VerticalScrollSize = (int)(852f * FormControlZoom.ScreenHeightZoom);
			if (base.Width <= HorizontalScrollSize)
			{
				base.HorizontalScroll.Enabled = true;
				base.HorizontalScroll.Visible = true;
			}
			else
			{
				base.HorizontalScroll.Enabled = false;
				base.HorizontalScroll.Visible = false;
			}
			if (base.Height <= VerticalScrollSize)
			{
				base.VerticalScroll.Enabled = true;
				base.VerticalScroll.Visible = true;
			}
			else
			{
				base.VerticalScroll.Enabled = false;
				base.VerticalScroll.Visible = false;
			}
			int WitdthSize = (int)(800f * FormControlZoom.ScreenWidthZoom);
			int HeightSize = (int)(600f * FormControlZoom.ScreenHeightZoom);
			if (base.Width <= WitdthSize)
			{
				base.Width = WitdthSize;
			}
			if (base.Height <= HeightSize)
			{
				base.Height = HeightSize;
			}
			GB.Form001TCPEvent = new AutoResetEvent(false);
			GB.Form001TCPFlag = true;
			ThreadStart MissionForm001TCP = Form001TCPThread;
			GB.MissionForm001TCPThread = new Thread(MissionForm001TCP);
			GB.MissionForm001TCPThread.Start();
			GB.BackGroundEvent = new AutoResetEvent(false);
			GB.BackGroundThreadFlag = true;
			ThreadStart MissionBackGround = BackGroundThread;
			GB.MissionBackGroundThread = new Thread(MissionBackGround);
			GB.MissionBackGroundThread.Start();
			GB.ReflashEvent = new AutoResetEvent(false);
			GB.ReflashThreadFlag = true;
			ThreadStart MissionReflash = ReflashThread;
			GB.MissionReflashThread = new Thread(MissionReflash);
			GB.MissionReflashThread.Start();
			GB.BackGroundRunningInfo();
			AlarmMsg(0);
			GB.ALNGMsgTimer = new System.Windows.Forms.Timer();
			StartTimerWithSleep();
			ThreadStart MissionFTP = FTPSlave.Start;
			GB.MissionFTPServerListenerThread = new Thread(MissionFTP);
			GB.MissionFTPServerListenerThread.Start();
			UpdateFWBn.Visible = false;
			Form011_Setting Form011 = new Form011_Setting(GB, TCP, TrCSV);
			Form011.CreateID += GetForm011;
			Form011.ShowDialog(this);
		}

		private void TimerDoingThing()
		{
			try
			{
				if (AlarmStr != "")
				{
					string newst = AlarmStr.Substring(0, 1);
					AlarmStr = AlarmStr.Substring(1, AlarmStr.Length - 1) + newst;
					lab_AlarmMsgBackgroud.Text = AlarmStr;
				}
				if (GB.ExFSUser.LastUserID != GB.ExFSUser.UserID)
				{
					GB.ExFSUser.LastUserID = GB.ExFSUser.UserID;
					if (GB.ExFSUser.UserID == 0)
					{
						lab_UserName.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, 0);
					}
					else if (GB.ExFSUser.UserID == 1)
					{
						lab_UserName.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, 1);
					}
					else if (GB.ExFSUser.UserID == 2)
					{
						lab_UserName.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, 2);
					}
					else if (GB.ExFSUser.UserID == 3)
					{
						lab_UserName.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, 3);
					}
					else if (GB.ExFSUser.UserID == 4)
					{
						lab_UserName.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, 4);
					}
					else if (GB.ExFSUser.UserID == 5)
					{
						lab_UserName.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, 5);
					}
					else if (GB.ExFSUser.UserID == 6)
					{
						lab_UserName.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, 6);
					}
					IsProhibitBtn();
				}
				if (GB.UISys.PCSoftSupport)
				{
					if (TCP.ConnectInterrupt)
					{
						TCP.RetryConnect();
					}
					if (TCP.MissKeepaliveCnt == 2 || TCP.MissKeepaliveCnt == 5 || TCP.MissKeepaliveCnt >= 8)
					{
						PingReply reply = TCP.ping.Send(GB.UISys.IPstr, 500);
						if (reply.Status != IPStatus.Success)
						{
							FormPublicFunction.SaveErrLog("Ping no response!");
							BreakConnect();
						}
						else
						{
							ForceFormClose(typeof(Form994_RemindPingNG));
							if (TCP.CommunicationType == 0)
							{
								TCP.FSIDWrite_ByTCP(10, 0, 0, 0, 0, 0);
							}
							else
							{
								TCP.FSIDWrite_ByTCP(12, 0, 0, 0, 0, 0);
							}
						}
					}
					if (TCP.MissKeepaliveCnt < 10)
					{
						TCP.MissKeepaliveCnt++;
					}
					else
					{
						FormPublicFunction.SaveErrLog("MissKeepaliveCnt Over Count");
						BreakConnect();
					}
				}
				ShowOnOffBtn(TCP.ConnectStatus, OnOfflinePB, OffOnlineImg);
			}
			catch
			{
				Console.WriteLine("Unable Ping Device!");
			}
		}

		private void Form_001Main_SizeChanged(object sender, EventArgs e)
		{
			if (base.AutoScaleMode == AutoScaleMode.Font && (FormControlZoom.ScreenWidth != 0 || FormControlZoom.ScreenHeight != 0))
			{
				Screen currentScreen = Screen.FromControl(this);
				int Width = currentScreen.Bounds.Width;
				int Height = currentScreen.Bounds.Height;
				if (FormControlZoom.ScreenWidth != Width || FormControlZoom.ScreenHeight != Height)
				{
					FormControlZoom.ScreenWidth = Width;
					FormControlZoom.ScreenHeight = Height;
					FormControlZoom.ScreenWidthZoom = (float)FormControlZoom.ScreenWidth / 1600f;
					FormControlZoom.ScreenHeightZoom = (float)FormControlZoom.ScreenHeight / 900f;
					FormControlZoom.ScreenFontZoom = ((FormControlZoom.ScreenWidthZoom < FormControlZoom.ScreenHeightZoom) ? FormControlZoom.ScreenWidthZoom : FormControlZoom.ScreenHeightZoom);
					FormControlZoom.SetControls(this);
					Console.WriteLine("視窗大小已改變: W={0},H={1} ", FormControlZoom.ScreenWidth, FormControlZoom.ScreenHeight);
				}
			}
			Point point = (base.AutoScrollPosition = new Point(0, 0));
			_scrollPosition = point;
		}

		private void StartTimerWithSleep()
		{
			try
			{
				Task.Run(async delegate
				{
					while (GB.StartTimerWithSleepWinformLive)
					{
						if (GB.StartTimerWithSleepFlag)
						{
							Invoke((Action)delegate
							{
								TimerDoingThing();
							});
							await Task.Delay(1000);
						}
					}
				});
			}
			catch
			{
			}
		}

		private void ALNGMsgTimer_Tick(object sender, EventArgs e)
		{
			TimerDoingThing();
		}

		private unsafe void BreakConnect()
		{
			try
			{
				TCP.ConnectInterrupt = true;
				Form994_RemindPingNG Form994 = new Form994_RemindPingNG(GB, 5003);
				if (!IsFormRunning(typeof(Form994_RemindPingNG)))
				{
					string MessageStr = "BreakConnect: ";
					MessageStr += "\r\n Write: ";
					for (int i = 0; i < 2000; i++)
					{
						MessageStr = MessageStr + GB.TcpWR.Data16[i] + ",";
					}
					MessageStr += "\r\n Read: ";
					for (int j = 0; j < 2000; j++)
					{
						MessageStr = MessageStr + GB.TcpRD.Data16[j] + ",";
					}
					FormPublicFunction.SaveErrLog(MessageStr);
					Form994.CreateYesAns += TCP.StopTCPConnect;
					Form994.CreateNoAns += TCP.RetryConnect;
					Form994.Show(this);
				}
			}
			catch
			{
			}
		}

		private void RetryTCPConnect()
		{
		}

		private void ShowOnOffBtn(bool val, PictureBox Picture, Image[] Img)
		{
			Picture.BackgroundImageLayout = ImageLayout.Stretch;
			Picture.BackgroundImage = ((!val) ? Img[0] : Img[1]);
		}

		private static bool FormDetect(Type formType)
		{
			bool TheSame = false;
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == formType)
				{
					TheSame = true;
					break;
				}
			}
			return TheSame;
		}

		private static void ForceFormClose(Type formType)
		{
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == formType)
				{
					form.Close();
					break;
				}
			}
		}

		private static bool IsFormOpen(Type formType)
		{
			bool Rst = false;
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == formType)
				{
					Rst = true;
				}
			}
			return false;
		}

		private static bool IsFormRunning(Type formType)
		{
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == formType)
				{
					return true;
				}
			}
			return false;
		}

		public void BackGroundThread()
		{
			while (GB.BackGroundThreadFlag)
			{
				GB.BackGroundThreadWait = true;
				GB.BackGroundEvent.WaitOne();
				if (!GB.BackGroundThreadFlag)
				{
					break;
				}
				try
				{
					Invoke((Action)delegate
					{
						GB.BackGroundRunningInfo();
						AlarmMsg(99);
						PupWindowMsg();
						ReadBinFile();
					});
				}
				catch (Exception ex)
				{
					string errorMessage = ex.Message + " Err No." + ex.StackTrace;
					FormPublicFunction.SaveErrLog(errorMessage);
					MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
			}
		}

		public void ReadBinFile()
		{
			if (!TCP.ConnectStatus || (GB.FSCtrlExportResultFile.Mode != 5 && GB.FSCtrlExportResultFile.Mode != 6) || !GB.CheckHMIVer(169, 9))
			{
				return;
			}
			bool ReminingSpace = true;
			uint ReportID = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_H_08 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_L_07);
			if (ReportID == 0 || OrgReportID == ReportID)
			{
				return;
			}
			GB.ALNGMsgStartStopFunction(false);
			OrgReportID = ReportID;
			ushort RemReportID = 0;
			ushort OrgRemReportID = (ushort)((ReportID - 1) % 10);
			if (!GB.UISys.IsReadSupportFTPServer)
			{
				TCP.FSIDRead_ByTCP(808, 1, 0, ushort.MaxValue, 65534, 0);
			}
			int BinExist = TCP.Bin_Status;
			bool CSVSW = GB.FSCtrlExportResultFile.Mode == 6;
			for (int n = OrgRemReportID + 10 - 1; n >= OrgRemReportID; n--)
			{
				RemReportID = (ushort)(n % 10);
				if (GB.UISys.IsReadSupportFTPServer)
				{
					List<ushort> LData = GB.UseFTPGetFile("ScrewInfo/CacheBin/ID" + RemReportID + ".bin");
					if (LData.Count() > 0)
					{
						long SystemFreeMB = GB.GetSystemFreeSpace();
						if (SystemFreeMB <= GB.UISys.NeedSpaceMBSize)
						{
							if (ReminingSpace)
							{
								Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3190, "(Remaining Space: " + SystemFreeMB + "MB)");
								Form995.Show(this);
								ReminingSpace = false;
								break;
							}
						}
						else
						{
							ushort[] Data16 = LData.ToArray();
							CurveCopyToCaheArrayAll(ushort.MaxValue, 0u, Data16);
							FileSystemBinW(CSVSW, ushort.MaxValue, 0u, Data16);
							if (GB.FSCtrlExportResultFile.Mode == 5)
							{
								TrCSV.WriteReportCurveScaleParam(uint.MaxValue, "", 0u);
							}
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, ushort.MaxValue, ushort.MaxValue, 0);
						}
					}
				}
				else if (((BinExist >> (int)RemReportID) & 1) == 1)
				{
					long SystemFreeMB2 = GB.GetSystemFreeSpace();
					if (SystemFreeMB2 <= GB.UISys.NeedSpaceMBSize)
					{
						if (ReminingSpace)
						{
							Form995_RemindOKNG Form996 = new Form995_RemindOKNG(GB, 3190, "(Remaining Space: " + SystemFreeMB2 + "MB)");
							Form996.Show(this);
							ReminingSpace = false;
							break;
						}
					}
					else
					{
						TCP.FSIDRead_ByTCP(808, 1, RemReportID, 0, 0, 300);
						TCP.FSIDRead_ByTCP(808, 1, RemReportID, 16300, 0, 1200);
						TCP.FSIDRead_ByTCP(808, 1, RemReportID, 65500, 0, 400);
						CurveCopyToCaheArray(0, 0u);
						CurveCopyToCaheArray(1, 0u);
						FileSystemBin(CSVSW, 0, 0u);
						if (TCP.FSBinCacheScaleParam[35] == 0 || TCP.FSBinCacheScaleParam[35] == 1)
						{
							CurveAllPoint = TCP.FSBinCacheScaleParam[28];
							if (CurveAllPoint > 0 && CurveAllPoint <= 2000)
							{
								CurvePointP0to2 = CurveAllPoint;
								CurvePointP2to4 = (CurvePointP4to6 = (CurvePointP6to8 = 0));
							}
						}
						else if (TCP.FSBinCacheScaleParam[35] == 2 || TCP.FSBinCacheScaleParam[35] == 3)
						{
							CurveAllPoint = TCP.FSBinCacheScaleParam[28];
							if (CurveAllPoint > 0 && CurveAllPoint <= 2000)
							{
								CurvePointP0to2 = CurveAllPoint;
								CurvePointP2to4 = (CurvePointP4to6 = (CurvePointP6to8 = 0));
							}
							else if (CurveAllPoint > 2000 && CurveAllPoint <= 4000)
							{
								CurvePointP0to2 = 2000;
								CurvePointP2to4 = (ushort)(CurveAllPoint - 2000);
								CurvePointP4to6 = (CurvePointP6to8 = 0);
							}
						}
						else
						{
							CurveAllPoint = TCP.FSBinCacheScaleParam[28];
							if (CurveAllPoint > 0 && CurveAllPoint <= 2000)
							{
								CurvePointP0to2 = CurveAllPoint;
								CurvePointP2to4 = (CurvePointP4to6 = (CurvePointP6to8 = 0));
							}
							else if (CurveAllPoint > 2000 && CurveAllPoint <= 4000)
							{
								CurvePointP0to2 = 2000;
								CurvePointP2to4 = (ushort)(CurveAllPoint - 2000);
								CurvePointP4to6 = (CurvePointP6to8 = 0);
							}
							else if (CurveAllPoint > 4000 && CurveAllPoint <= 6000)
							{
								CurvePointP0to2 = (CurvePointP2to4 = 2000);
								CurvePointP4to6 = (ushort)(CurveAllPoint - 4000);
								CurvePointP6to8 = 0;
							}
							else if (CurveAllPoint > 6000 && CurveAllPoint <= 8000)
							{
								CurvePointP0to2 = (CurvePointP2to4 = (CurvePointP4to6 = 2000));
								CurvePointP6to8 = (ushort)(CurveAllPoint - 6000);
							}
						}
						if (CurvePointP0to2 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 300, 0, (ushort)(CurvePointP0to2 * 2));
						}
						CurveCopyToCaheArray(100, CurvePointP0to2);
						FileSystemBin(CSVSW, 10, CurvePointP0to2);
						if (CurvePointP0to2 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 4300, 0, (ushort)(CurvePointP0to2 * 2));
						}
						CurveCopyToCaheArray(101, CurvePointP0to2);
						FileSystemBin(CSVSW, 11, CurvePointP0to2);
						if (CurvePointP0to2 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 8300, 0, (ushort)(CurvePointP0to2 * 2));
						}
						CurveCopyToCaheArray(102, CurvePointP0to2);
						FileSystemBin(CSVSW, 12, CurvePointP0to2);
						if (CurvePointP0to2 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 12300, 0, (ushort)(CurvePointP0to2 * 2));
						}
						CurveCopyToCaheArray(103, CurvePointP0to2);
						FileSystemBin(CSVSW, 13, CurvePointP0to2);
						FileSystemBin(CSVSW, 2, 0u);
						if (CurvePointP2to4 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 17500, 0, (ushort)(CurvePointP2to4 * 2));
						}
						CurveCopyToCaheArray(100, CurvePointP2to4);
						FileSystemBin(CSVSW, 20, CurvePointP2to4);
						if (CurvePointP2to4 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 21500, 0, (ushort)(CurvePointP2to4 * 2));
						}
						CurveCopyToCaheArray(101, CurvePointP2to4);
						FileSystemBin(CSVSW, 21, CurvePointP2to4);
						if (CurvePointP2to4 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 25500, 0, (ushort)(CurvePointP2to4 * 2));
						}
						CurveCopyToCaheArray(102, CurvePointP2to4);
						FileSystemBin(CSVSW, 22, CurvePointP2to4);
						if (CurvePointP2to4 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 29500, 0, (ushort)(CurvePointP2to4 * 2));
						}
						CurveCopyToCaheArray(103, CurvePointP2to4);
						FileSystemBin(CSVSW, 23, CurvePointP2to4);
						if (CurvePointP4to6 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 33500, 0, (ushort)(CurvePointP4to6 * 2));
						}
						CurveCopyToCaheArray(100, CurvePointP4to6);
						FileSystemBin(CSVSW, 30, CurvePointP4to6);
						if (CurvePointP4to6 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 37500, 0, (ushort)(CurvePointP4to6 * 2));
						}
						CurveCopyToCaheArray(101, CurvePointP4to6);
						FileSystemBin(CSVSW, 31, CurvePointP4to6);
						if (CurvePointP4to6 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 41500, 0, (ushort)(CurvePointP4to6 * 2));
						}
						CurveCopyToCaheArray(102, CurvePointP4to6);
						FileSystemBin(CSVSW, 32, CurvePointP4to6);
						if (CurvePointP4to6 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 45500, 0, (ushort)(CurvePointP4to6 * 2));
						}
						CurveCopyToCaheArray(103, CurvePointP4to6);
						FileSystemBin(CSVSW, 33, CurvePointP4to6);
						if (CurvePointP6to8 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 49500, 0, (ushort)(CurvePointP6to8 * 2));
						}
						CurveCopyToCaheArray(100, CurvePointP6to8);
						FileSystemBin(CSVSW, 40, CurvePointP6to8);
						if (CurvePointP6to8 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 53500, 0, (ushort)(CurvePointP6to8 * 2));
						}
						CurveCopyToCaheArray(101, CurvePointP6to8);
						FileSystemBin(CSVSW, 41, CurvePointP6to8);
						if (CurvePointP6to8 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 57500, 0, (ushort)(CurvePointP6to8 * 2));
						}
						CurveCopyToCaheArray(102, CurvePointP6to8);
						FileSystemBin(CSVSW, 42, CurvePointP6to8);
						if (CurvePointP6to8 > 0)
						{
							TCP.FSIDRead_ByTCP(808, 1, RemReportID, 61500, 0, (ushort)(CurvePointP6to8 * 2));
						}
						CurveCopyToCaheArray(103, CurvePointP6to8);
						FileSystemBin(CSVSW, 43, CurvePointP6to8);
						FileSystemBin(CSVSW, 999, 0u);
						if (GB.FSCtrlExportResultFile.Mode == 5)
						{
							TrCSV.WriteReportCurveScaleParam(uint.MaxValue, "", 0u);
						}
						TCP.FSIDRead_ByTCP(808, 1, RemReportID, ushort.MaxValue, ushort.MaxValue, 0);
					}
				}
			}
			GB.ALNGMsgStartStopFunction(true);
		}

		public void CurveCopyToCaheArray(ushort Mode, uint WordLen)
		{
			ushort[] Data16 = new ushort[1];
			CurveCopyToCaheArrayW(Mode, WordLen, Data16);
		}

		public void CurveCopyToCaheArrayAll(ushort Mode, uint WordLen, ushort[] Data16)
		{
			CurveCopyToCaheArrayW(5000, 0u, Data16);
			CurveCopyToCaheArrayW(5001, 0u, Data16);
			CurveCopyToCaheArrayW(5100, 0u, Data16);
			CurveCopyToCaheArrayW(5101, 0u, Data16);
			CurveCopyToCaheArrayW(5102, 0u, Data16);
			CurveCopyToCaheArrayW(5103, 0u, Data16);
			CurveCopyToCaheArrayW(5999, 0u, Data16);
		}

		public unsafe void CurveCopyToCaheArrayW(ushort Mode, uint WordLen, ushort[] Data16)
		{
			int Axis = 0;
			Axis = ((Mode < 5000) ? TCP.FSBinCacheSNReport[103] : Data16[103]);
			if (Mode == 0 || Mode == 5000)
			{
				if (Mode >= 5000)
				{
					for (uint i = 0u; i < 150; i++)
					{
						TCP.FSBinCacheSNReport[i] = Data16[i];
					}
				}
				DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)TCP.FSBinCacheSNReport[100]).AddSeconds(TCP.FSBinCacheSNReport[102] * 65536 + TCP.FSBinCacheSNReport[101]);
				if (Axis == 0)
				{
					for (uint i2 = 0u; i2 < 100; i2++)
					{
						TCP.CacheInfoX.Data16[i2] = TCP.FSBinCacheSNReport[i2];
					}
					for (uint i3 = 0u; i3 < 47; i3++)
					{
						TCP.CacheInfoX.Data16[106 + i3] = TCP.FSBinCacheSNReport[103 + i3];
					}
					TCP.CacheInfoX.Year = (ushort)OpTime.Year;
					TCP.CacheInfoX.Month = (ushort)OpTime.Month;
					TCP.CacheInfoX.Day = (ushort)OpTime.Day;
					TCP.CacheInfoX.Hour = (ushort)OpTime.Hour;
					TCP.CacheInfoX.Min = (ushort)OpTime.Minute;
					TCP.CacheInfoX.Sec = (ushort)OpTime.Second;
				}
				else
				{
					for (uint i4 = 0u; i4 < 100; i4++)
					{
						TCP.CacheInfoY.Data16[i4] = TCP.FSBinCacheSNReport[i4];
					}
					for (uint i5 = 0u; i5 < 47; i5++)
					{
						TCP.CacheInfoY.Data16[106 + i5] = TCP.FSBinCacheSNReport[103 + i5];
					}
					TCP.CacheInfoY.Year = (ushort)OpTime.Year;
					TCP.CacheInfoY.Month = (ushort)OpTime.Month;
					TCP.CacheInfoY.Day = (ushort)OpTime.Day;
					TCP.CacheInfoY.Hour = (ushort)OpTime.Hour;
					TCP.CacheInfoY.Min = (ushort)OpTime.Minute;
					TCP.CacheInfoY.Sec = (ushort)OpTime.Second;
				}
				if (Axis == 0)
				{
					GB.ResultLedStatusFunc(Axis, TCP.CacheInfoX.ScrewNo, TCP.CacheInfoX.Status);
				}
				else
				{
					GB.ResultLedStatusFunc(Axis, TCP.CacheInfoY.ScrewNo, TCP.CacheInfoY.Status);
				}
				if (Axis == 0)
				{
					TCP.CacheCurveTimeX.Clear();
					TCP.CacheCurveAngleX.Clear();
					TCP.CacheCurveTorqueX.Clear();
					TCP.CacheCurveTorqueRateX.Clear();
				}
				else
				{
					TCP.CacheCurveTimeY.Clear();
					TCP.CacheCurveAngleY.Clear();
					TCP.CacheCurveTorqueY.Clear();
					TCP.CacheCurveTorqueRateY.Clear();
				}
			}
			else if (Mode == 1 || Mode == 5001)
			{
				if (Mode >= 5000)
				{
					for (uint i6 = 0u; i6 < 600; i6++)
					{
						TCP.FSBinCacheScaleParam[i6] = Data16[8150 + i6];
					}
				}
				for (uint i7 = 0u; i7 < 50; i7++)
				{
					if (Axis == 0)
					{
						TCP.CacheScaleX.Data16[i7] = TCP.FSBinCacheScaleParam[i7];
					}
					else
					{
						TCP.CacheScaleY.Data16[i7] = TCP.FSBinCacheScaleParam[i7];
					}
				}
				for (uint i8 = 0u; i8 < 550; i8++)
				{
					if (Axis == 0)
					{
						TCP.CacheParamX[i8] = TCP.FSBinCacheScaleParam[50 + i8];
					}
					else
					{
						TCP.CacheParamY[i8] = TCP.FSBinCacheScaleParam[50 + i8];
					}
				}
			}
			else if (Mode == 100 || Mode == 5100)
			{
				if (Mode >= 5000)
				{
					for (int j = 0; j < 2000; j++)
					{
						TCP.FSBinCacheTimePoint[j] = Data16[150 + j];
					}
					for (int k = 0; k < 2000; k++)
					{
						TCP.FSBinCacheTimePoint[2000 + k] = Data16[8750 + k];
					}
					for (int l = 0; l < 2000; l++)
					{
						TCP.FSBinCacheTimePoint[4000 + l] = Data16[16750 + l];
					}
					for (int m = 0; m < 2000; m++)
					{
						TCP.FSBinCacheTimePoint[6000 + m] = Data16[24750 + m];
					}
					for (int n = 0; n < TCP.FSBinCacheScaleParam[28]; n++)
					{
						if (Axis == 0)
						{
							TCP.CacheCurveTimeX.Add(TCP.FSBinCacheTimePoint[n]);
						}
						else
						{
							TCP.CacheCurveTimeY.Add(TCP.FSBinCacheTimePoint[n]);
						}
					}
					return;
				}
				for (int num = 0; num < WordLen; num++)
				{
					if (Axis == 0)
					{
						TCP.CacheCurveTimeX.Add(TCP.FSBinData[num]);
					}
					else
					{
						TCP.CacheCurveTimeY.Add(TCP.FSBinData[num]);
					}
				}
			}
			else if (Mode == 101 || Mode == 5101)
			{
				if (Mode >= 5000)
				{
					for (int num2 = 0; num2 < 2000; num2++)
					{
						TCP.FSBinCacheAnglePoint[num2] = (short)Data16[2150 + num2];
					}
					for (int num3 = 0; num3 < 2000; num3++)
					{
						TCP.FSBinCacheAnglePoint[2000 + num3] = (short)Data16[10750 + num3];
					}
					for (int num4 = 0; num4 < 2000; num4++)
					{
						TCP.FSBinCacheAnglePoint[4000 + num4] = (short)Data16[18750 + num4];
					}
					for (int num5 = 0; num5 < 2000; num5++)
					{
						TCP.FSBinCacheAnglePoint[6000 + num5] = (short)Data16[26750 + num5];
					}
					for (int num6 = 0; num6 < TCP.FSBinCacheScaleParam[28]; num6++)
					{
						if (Axis == 0)
						{
							TCP.CacheCurveAngleX.Add(TCP.FSBinCacheAnglePoint[num6]);
						}
						else
						{
							TCP.CacheCurveAngleY.Add(TCP.FSBinCacheAnglePoint[num6]);
						}
					}
					return;
				}
				for (int num7 = 0; num7 < WordLen; num7++)
				{
					if (Axis == 0)
					{
						TCP.CacheCurveAngleX.Add((short)TCP.FSBinData[num7]);
					}
					else
					{
						TCP.CacheCurveAngleY.Add((short)TCP.FSBinData[num7]);
					}
				}
			}
			else if (Mode == 102 || Mode == 5102)
			{
				if (Mode >= 5000)
				{
					for (int num8 = 0; num8 < 2000; num8++)
					{
						TCP.FSBinCacheTorqPoint[num8] = (short)Data16[4150 + num8];
					}
					for (int num9 = 0; num9 < 2000; num9++)
					{
						TCP.FSBinCacheTorqPoint[2000 + num9] = (short)Data16[12750 + num9];
					}
					for (int num10 = 0; num10 < 2000; num10++)
					{
						TCP.FSBinCacheTorqPoint[4000 + num10] = (short)Data16[20750 + num10];
					}
					for (int num11 = 0; num11 < 2000; num11++)
					{
						TCP.FSBinCacheTorqPoint[6000 + num11] = (short)Data16[28750 + num11];
					}
					for (int num12 = 0; num12 < TCP.FSBinCacheScaleParam[28]; num12++)
					{
						if (Axis == 0)
						{
							TCP.CacheCurveTorqueX.Add(TCP.FSBinCacheTorqPoint[num12]);
						}
						else
						{
							TCP.CacheCurveTorqueY.Add(TCP.FSBinCacheTorqPoint[num12]);
						}
					}
					return;
				}
				for (int num13 = 0; num13 < WordLen; num13++)
				{
					if (Axis == 0)
					{
						TCP.CacheCurveTorqueX.Add((short)TCP.FSBinData[num13]);
					}
					else
					{
						TCP.CacheCurveTorqueY.Add((short)TCP.FSBinData[num13]);
					}
				}
			}
			else if (Mode == 103 || Mode == 5103)
			{
				if (Mode >= 5000)
				{
					for (int num14 = 0; num14 < 2000; num14++)
					{
						TCP.FSBinCacheTorqRatePoint[num14] = (short)Data16[6150 + num14];
					}
					for (int num15 = 0; num15 < 2000; num15++)
					{
						TCP.FSBinCacheTorqRatePoint[2000 + num15] = (short)Data16[14750 + num15];
					}
					for (int num16 = 0; num16 < 2000; num16++)
					{
						TCP.FSBinCacheTorqRatePoint[4000 + num16] = (short)Data16[22750 + num16];
					}
					for (int num17 = 0; num17 < 2000; num17++)
					{
						TCP.FSBinCacheTorqRatePoint[6000 + num17] = (short)Data16[30750 + num17];
					}
					for (int num18 = 0; num18 < TCP.FSBinCacheScaleParam[28]; num18++)
					{
						if (Axis == 0)
						{
							TCP.CacheCurveTorqueRateX.Add(TCP.FSBinCacheTorqRatePoint[num18]);
						}
						else
						{
							TCP.CacheCurveTorqueRateY.Add(TCP.FSBinCacheTorqRatePoint[num18]);
						}
					}
					return;
				}
				for (int num19 = 0; num19 < WordLen; num19++)
				{
					if (Axis == 0)
					{
						TCP.CacheCurveTorqueRateX.Add((short)TCP.FSBinData[num19]);
					}
					else
					{
						TCP.CacheCurveTorqueRateY.Add((short)TCP.FSBinData[num19]);
					}
				}
			}
			else if ((Mode == 999 || Mode == 5999) && Mode >= 5000)
			{
				for (int num20 = 0; num20 < 200; num20++)
				{
					TCP.FSBinCacheOtherInfo[num20] = Data16[32750 + num20];
				}
			}
		}

		public void FileSystemBin(bool SW, ushort Mode, uint WordLen)
		{
			ushort[] Data16 = new ushort[1];
			FileSystemBinW(SW, Mode, WordLen, Data16);
		}

		public void FileSystemBinW(bool SW, ushort Mode, uint WordLen, ushort[] Data16)
		{
			if (!SW)
			{
				return;
			}
			DateTime OpTime = ((Mode != ushort.MaxValue) ? new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)TCP.FSBinCacheSNReport[100]).AddSeconds(TCP.FSBinCacheSNReport[102] * 65536 + TCP.FSBinCacheSNReport[101]) : new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)Data16[100]).AddSeconds(Data16[102] * 65536 + Data16[101]));
			uint ReportID = ((TCP.FSBinCacheOtherInfo[104] * 65536 + TCP.FSBinCacheOtherInfo[103] != 0) ? ((uint)(TCP.FSBinCacheOtherInfo[104] * 65536 + TCP.FSBinCacheOtherInfo[103] - 1)) : 0u);
			string strA = ".\\ScrewInfo\\";
			string strB = "/Bin/" + $"{OpTime.Year:D4}{OpTime.Month:D2}{OpTime.Day:D2}" + "/";
			string strC = $"{OpTime.Year:D4}{OpTime.Month:D2}{OpTime.Day:D2}{OpTime.Hour:D2}{OpTime.Minute:D2}{OpTime.Second:D2}_{ReportID:D6}.bin";
			switch (Mode)
			{
			case 0:
			{
				if (!Directory.Exists(strA + strB))
				{
					Directory.CreateDirectory(strA + strB);
				}
				using (BinaryWriter BinW8 = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Create)))
				{
					for (int m = 0; m < 150; m++)
					{
						BinW8.Write(TCP.FSBinCacheSNReport[m]);
					}
					break;
				}
			}
			default:
				if (Mode != 40)
				{
					if (Mode == 11 || Mode == 21 || Mode == 31 || Mode == 41)
					{
						using (BinaryWriter BinW = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Append)))
						{
							for (uint i = 0u; i < WordLen; i++)
							{
								BinW.Write(TCP.FSBinCacheAnglePoint[((ushort)(Mode / 10) - 1) * 2000 + i]);
							}
							for (uint i2 = WordLen; i2 < 2000; i2++)
							{
								BinW.Write((ushort)0);
							}
							break;
						}
					}
					if (Mode == 12 || Mode == 22 || Mode == 32 || Mode == 42)
					{
						using (BinaryWriter BinW2 = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Append)))
						{
							for (uint i3 = 0u; i3 < WordLen; i3++)
							{
								BinW2.Write(TCP.FSBinCacheTorqPoint[((ushort)(Mode / 10) - 1) * 2000 + i3]);
							}
							for (uint i4 = WordLen; i4 < 2000; i4++)
							{
								BinW2.Write((ushort)0);
							}
							break;
						}
					}
					if (Mode == 13 || Mode == 23 || Mode == 33 || Mode == 43)
					{
						using (BinaryWriter BinW3 = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Append)))
						{
							for (uint i5 = 0u; i5 < WordLen; i5++)
							{
								BinW3.Write(TCP.FSBinCacheTorqRatePoint[((ushort)(Mode / 10) - 1) * 2000 + i5]);
							}
							for (uint i6 = WordLen; i6 < 2000; i6++)
							{
								BinW3.Write((ushort)0);
							}
							break;
						}
					}
					switch (Mode)
					{
					case 2:
					{
						using (BinaryWriter BinW5 = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Append)))
						{
							for (int k = 0; k < 600; k++)
							{
								BinW5.Write(TCP.FSBinCacheScaleParam[k]);
							}
							break;
						}
					}
					case 999:
					{
						using (BinaryWriter BinW6 = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Append)))
						{
							for (int l = 0; l < 200; l++)
							{
								BinW6.Write(TCP.FSBinCacheOtherInfo[l]);
							}
							break;
						}
					}
					case ushort.MaxValue:
					{
						if (!Directory.Exists(strA + strB))
						{
							Directory.CreateDirectory(strA + strB);
						}
						using (BinaryWriter BinW4 = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Create)))
						{
							for (int j = 0; j < Data16.Length; j++)
							{
								BinW4.Write(Data16[j]);
							}
							break;
						}
					}
					}
					break;
				}
				goto case 10;
			case 10:
			case 20:
			case 30:
			{
				using (BinaryWriter BinW7 = new BinaryWriter(File.Open(strA + strB + strC, FileMode.Append)))
				{
					for (uint i7 = 0u; i7 < WordLen; i7++)
					{
						BinW7.Write(TCP.FSBinCacheTimePoint[((ushort)(Mode / 10) - 1) * 2000 + i7]);
					}
					for (uint i8 = WordLen; i8 < 2000; i8++)
					{
						BinW7.Write((ushort)0);
					}
					break;
				}
			}
			}
		}

		public void ReflashThread()
		{
			while (GB.ReflashThreadFlag)
			{
				GB.ReflashThreadWait = true;
				GB.ReflashEvent.WaitOne();
				if (!GB.ReflashThreadFlag)
				{
					break;
				}
				Invoke((Action)delegate
				{
					if (GB.CheckHMIVer(171, 2))
					{
						GB.ALNGMsgStartStopFunction(false);
						ushort[] array = new ushort[10];
						ushort[] array2 = new ushort[10];
						int num = 0;
						for (int i = 0; i < 10; i++)
						{
							if (TCP.DoneReflashArray[i] != TCP.NowReflashArray[i] && TCP.NowReflashArray[i] > 0)
							{
								array[i] = TCP.NowReflashArray[i];
							}
						}
						ushort[] array3 = array.Distinct().ToArray();
						for (int j = 0; j < array3.Length; j++)
						{
							array2[j] = array3[j];
						}
						for (int k = 0; k < 10; k++)
						{
							ushort num2 = array2[k];
							if (num2 > 0)
							{
								if (num2 == 100 || num2 == 101)
								{
									int num3 = ((GB.FSModelTypeInfo.MesModelType == 1) ? 101 : 100);
									int num4 = ((GB.FSModelTypeInfo.MesModelType == 1) ? 100 : 101);
									if (num2 == num3)
									{
										num = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(10, 0u, 500u, 0) : TCP.FSIDRead_ByFTP(10));
										if (num != 0)
										{
											break;
										}
									}
									if (num2 == num4)
									{
										num = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(11, 0u, 500u, 0) : TCP.FSIDRead_ByFTP(11));
										if (num != 0)
										{
											break;
										}
									}
								}
								else if (num2 == 200)
								{
									num = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(20, 0u, 500u, 0) : TCP.FSIDRead_ByFTP(20));
									if (num != 0)
									{
										break;
									}
								}
								else if (num2 >= 300 && num2 <= 322)
								{
									GB.FSSrcMode.ActionMode = GB.TcpStatus.Detail.Comm.OperationMode_20;
									if (GB.FSSrcMode.ActionMode == 0)
									{
										int num5 = ((GB.FSModelTypeInfo.MesModelType == 1) ? 1 : 0);
										int num6 = ((GB.FSModelTypeInfo.MesModelType != 1) ? 1 : 0);
										if (num2 % 2 == num5 || num2 == 322)
										{
											GB.FSSrcMode.SwitchingMethodX = GB.TcpStatus.Detail.Comm.Tool1SwitchingMethod_21;
											num = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(30, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(30));
											if (num != 0)
											{
												break;
											}
										}
										if (num2 % 2 == num6 || num2 == 322)
										{
											GB.FSSrcMode.SwitchingMethodY = GB.TcpStatus.Detail.Comm.Tool2SwitchingMethod_22;
											num = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(35, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(35));
											if (num != 0)
											{
												break;
											}
										}
									}
									else if (GB.FSSrcMode.ActionMode == 1)
									{
										GB.FSSrcMode.SwitchingMethodX = GB.TcpStatus.Detail.Comm.Tool1SwitchingMethod_21;
										GB.FSSrcMode.SwitchingMethodY = GB.TcpStatus.Detail.Comm.Tool2SwitchingMethod_22;
										num = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(40, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(40));
										if (num != 0)
										{
											break;
										}
									}
									else if (GB.FSSrcMode.ActionMode == 2)
									{
										GB.FSSrcMode.SwitchingMethodX = GB.TcpStatus.Detail.Comm.Tool1SwitchingMethod_21;
										GB.FSSrcMode.SwitchingMethodY = GB.TcpStatus.Detail.Comm.Tool2SwitchingMethod_22;
										num = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(50, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(50));
										if (num != 0)
										{
											break;
										}
									}
								}
								ReflashUIScreen(num2);
								GB.BackGroundRunningInfo();
							}
						}
						Array.Copy(TCP.NowReflashArray, TCP.DoneReflashArray, 10);
						if (num != 0)
						{
							TCP.ConnectInterrupt = true;
						}
						GB.ALNGMsgStartStopFunction(true);
					}
				});
			}
		}

		public void ReflashUIScreen(ushort ReflashID)
		{
			ushort FormPage = 0;
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType() == typeof(Form100_Param))
				{
					FormPage = 100;
				}
				else if (form.GetType() == typeof(Form200_Seq))
				{
					FormPage = 200;
				}
				else if (form.GetType() == typeof(Form300_Source))
				{
					FormPage = 300;
				}
				else if (form.GetType() == typeof(Form400_Results))
				{
					FormPage = 400;
				}
				else if (form.GetType() == typeof(Form401_ResultsMixTool))
				{
					FormPage = 401;
				}
				else if (form.GetType() == typeof(Form402_ResultsDualTool))
				{
					FormPage = 402;
				}
				else if (form.GetType() == typeof(Form500_Controller))
				{
					FormPage = 500;
				}
				else if (form.GetType() == typeof(Form600_Tool))
				{
					FormPage = 600;
				}
				else if (form.GetType() == typeof(Form700_Report))
				{
					FormPage = 700;
				}
				else if (form.GetType() == typeof(Form800_Help))
				{
					FormPage = 800;
				}
			}
			if (FormPage == 100 && ((ReflashID >= 100 && ReflashID < 200) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(1, true);
			}
			else if (FormPage == 200 && ((ReflashID >= 200 && ReflashID < 300) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(2, true);
			}
			else if (FormPage == 300 && ((ReflashID >= 300 && ReflashID < 400) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(3, true);
			}
			else if (FormPage == 400 && ((ReflashID >= 400 && ReflashID < 500) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(4, true);
			}
			else if (FormPage == 500 && ((ReflashID >= 500 && ReflashID < 600) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(5, true);
			}
			else if (FormPage == 600 && ((ReflashID >= 600 && ReflashID < 700) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(6, true);
			}
			else if (FormPage == 700 && ((ReflashID >= 700 && ReflashID < 800) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(7, true);
			}
			else if (FormPage == 800 && ((ReflashID >= 800 && ReflashID < 900) || ReflashID == ushort.MaxValue))
			{
				ChangePageFunction(8, true);
			}
		}

		public void PupWindowMsg()
		{
			ushort PupWindowID = (ushort)(GB.CheckHMIVer(170, 11) ? GB.TcpStatus.Detail.Comm.PupWindowID_38 : 0);
			if (PupWindowID > 0 && LastPupWindowID != PupWindowID)
			{
				ForceFormClose(typeof(Form995_RemindOKNG));
				string MsgStr = "";
				if (GB.CheckHMIVer(171, 1))
				{
					if (PupWindowID == 400 || PupWindowID == 401)
					{
						TCP.FSIDRead_ByTCP(53, 0, 1, 15740, 0, 40);
						MsgStr = GB.GetNameTitleStr(FormType.SubLocalAddr, 100);
					}
					else
					{
						string text = (lab_HanderTitle.Text = MultiLanguage.GetStr(this, "tp_Remind" + PupWindowID.ToString("D4")));
						MsgStr = text;
					}
				}
				else
				{
					string text = (lab_HanderTitle.Text = MultiLanguage.GetStr(this, "tp_Remind" + PupWindowID.ToString("D4")));
					MsgStr = text;
				}
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3000 + PupWindowID, MsgStr);
				Form995.CreateCloseEvent += ClosePupWindowPage;
				Form995.Show(this);
			}
			if (PupWindowID == 0 && LastPupWindowID != PupWindowID)
			{
				ForceFormClose(typeof(Form995_RemindOKNG));
			}
			LastPupWindowID = PupWindowID;
		}

		public void ClosePupWindowPage()
		{
			if (GB.CheckHMIVer(170, 11))
			{
				TCP.FSIDWrite_ByTCP(5, 0, 0, 0, 0, 0);
			}
		}

		public void AlarmMsg(int SW)
		{
			if (SW == 0)
			{
				lab_AlarmMsgBackgroud.Visible = false;
				RstResetBn.Visible = false;
			}
			else
			{
				ushort AlarmCode = 0;
				if (GB.TcpStatus.Detail.Comm.Tool1ServoErrorWarning_00 > 0)
				{
					AlarmCode = GB.TcpStatus.Detail.Comm.Tool1ServoErrorWarning_00;
				}
				else if (GB.TcpStatus.Detail.Comm.Tool2ServoErrorWarning_01 > 0)
				{
					AlarmCode = GB.TcpStatus.Detail.Comm.Tool2ServoErrorWarning_01;
				}
				if (AlarmCode == 0)
				{
					lab_AlarmMsgBackgroud.Visible = false;
					RstResetBn.Visible = false;
				}
				else if (GB.FSCtrlWarningWindow.Enable == 1 && AlarmCode >= 20480 && AlarmCode < 28672)
				{
					lab_AlarmMsgBackgroud.Visible = false;
					RstResetBn.Visible = false;
				}
				else
				{
					AlarmStr = GB.ALWNNumberStr(AlarmCode) + "   " + GB.ALWNTitleStr(AlarmCode);
					if (AlarmCode >= 20480)
					{
						lab_AlarmMsgBackgroud.BackColor = Color.Gold;
						lab_AlarmMsgBackgroud.ForeColor = Color.Black;
					}
					else
					{
						lab_AlarmMsgBackgroud.BackColor = Color.Red;
						lab_AlarmMsgBackgroud.ForeColor = Color.White;
					}
					lab_AlarmMsgBackgroud.Visible = true;
					RstResetBn.Visible = true;
				}
			}
			base.AutoScrollPosition = _scrollPosition;
		}

		public unsafe void Form001TCPThread()
		{
			while (GB.Form001TCPFlag)
			{
				GB.Form001TCPWait = true;
				GB.Form001TCPEvent.WaitOne();
				if (!GB.Form001TCPFlag)
				{
					break;
				}
				Invoke((Action)delegate
				{
					int maxProcess = 60;
					int num = 0;
					int num2 = 0;
					bool flag = false;
					GB.ALNGMsgStartStopFunction(false);
					Form998_Wait form998_Wait = new Form998_Wait(GB);
					if (TCP.ConnectStatus && !TCP.ConnectInterrupt)
					{
						ForceFormClose(typeof(Form994_RemindPingNG));
					}
					num2 = TCP.FSIDRead_ByTCP(552, 0, 99, 0, 0, 99);
					if (num2 != 0)
					{
						TCP.ConnectInterrupt = true;
					}
					if (!GB.UISys.PCSoftSupport)
					{
						form998_Wait.Show(this);
						form998_Wait.Process(true, 0, maxProcess);
						TCP.DisConnectFunc();
						form998_Wait.Process(false, 0, 0);
						Form995_RemindOKNG form995_RemindOKNG = new Form995_RemindOKNG(GB, 5004, "");
						form995_RemindOKNG.Show(this);
					}
					else if (!TCP.ConnectStatus)
					{
						BreakConnect();
					}
					else if (!TCP.ConnectInterrupt)
					{
						form998_Wait.Show(this);
						form998_Wait.Process(true, 0, maxProcess);
						num2 = TCP.FSIDRead_ByTCP(552, 0, 99, 0, 0, 0);
						if (num2 == 0)
						{
							form998_Wait.Process(true, ++num, maxProcess);
							num2 = TCP.FSIDRead_ByTCP(51, 0, 1, 130, 0, 0);
							if (num2 == 0)
							{
								GB.TcpStatus.Detail.Comm.Keepalive_19 = GB.FSMesValue.Data16;
								for (int i = 0; i <= 49; i++)
								{
									num2 = TCP.FSIDRead_ByTCP(51, 0, 1, 130, 0, 0);
									if (num2 != 0 || GB.TcpStatus.Detail.Comm.Keepalive_19 != GB.FSMesValue.Data16)
									{
										break;
									}
								}
								num2 = ((TCP.CommunicationType != 0) ? TCP.FSIDWrite_ByTCP(13, 0, 0, 0, 0, 0) : TCP.FSIDWrite_ByTCP(11, 0, 0, 0, 0, 0));
								if (num2 == 0)
								{
									form998_Wait.Process(true, ++num, maxProcess);
									num2 = TCP.FSIDRead_ByTCP(699, 0, 0, 0, 0, 0);
									if (num2 == 0)
									{
										form998_Wait.Process(true, ++num, maxProcess);
										num2 = TCP.FSIDRead_ByTCP(699, 0, 1, 0, 0, 0);
										if (num2 == 0)
										{
											form998_Wait.Process(true, ++num, maxProcess);
											if (GB.FSCtrlTypeInfo.CtrlVer >= 3)
											{
												num2 = TCP.FSIDRead_ByTCP(599, 0, 99, 0, 0, 0);
												if (num2 != 0)
												{
													goto IL_1571;
												}
											}
											else
											{
												GB.FSModelTypeInfo.MesRawDataTorqUint = 0;
												GB.FSModelTypeInfo.MesParamUseNewVer = 0;
											}
											form998_Wait.Process(true, ++num, maxProcess);
											ReflashFormText(true, GB.UISys.PM101, GB.UISys.CtrlDualTool, GB.FSToolXModelInfo.ToolTorque_Nm, GB.FSToolYModelInfo.ToolTorque_Nm);
											num2 = TCP.FSIDRead_ByTCP(350, 0, 0, 0, 0, 0);
											if (num2 == 0)
											{
												form998_Wait.Process(true, ++num, maxProcess);
												num2 = TCP.FSIDRead_ByTCP(350, 0, 1, 0, 0, 0);
												if (num2 == 0)
												{
													form998_Wait.Process(true, ++num, maxProcess);
													bool flag2 = false;
													for (int j = 0; j <= 1; j++)
													{
														if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
														{
															Page_Axis = (uint)j;
															flag2 = true;
														}
														else
														{
															Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
															flag2 = false;
														}
														if (GB.FSSrcMode.ActionMode == 0)
														{
															if (Page_Axis == 0)
															{
																if (((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(30, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(30)) != 0)
																{
																	break;
																}
															}
															else if (((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(35, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(35)) != 0)
															{
																break;
															}
														}
														else if (GB.FSSrcMode.ActionMode == 1)
														{
															if (!flag)
															{
																if (((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(40, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(40)) != 0)
																{
																	break;
																}
																flag = true;
															}
														}
														else if (GB.FSSrcMode.ActionMode == 2 && !flag)
														{
															if (((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(50, 0u, 0u, 0) : TCP.FSIDRead_ByFTP(50)) != 0)
															{
																break;
															}
															flag = true;
														}
														form998_Wait.Process(true, ++num, maxProcess);
														if (((Page_Axis == 0) ? ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(10, 0u, 500u, 0) : TCP.FSIDRead_ByFTP(10)) : ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(11, 0u, 500u, 0) : TCP.FSIDRead_ByFTP(11))) != 0)
														{
															break;
														}
														form998_Wait.Process(true, ++num, maxProcess);
														if (TCP.FSIDRead_ByTCP(452, 0, (ushort)Page_Axis, 0, 0, 0) != 0)
														{
															break;
														}
														form998_Wait.Process(true, ++num, maxProcess);
														if (TCP.FSIDRead_ByTCP(453, 0, (ushort)Page_Axis, 0, 0, 0) != 0)
														{
															break;
														}
														form998_Wait.Process(true, ++num, maxProcess);
														if (GB.FSModelTypeInfo.MesModelType == 0)
														{
															if (Page_Axis == 0)
															{
																if (TCP.FSIDRead_ByTCP(553, 0, 0, 0, 0, 0) != 0)
																{
																	break;
																}
															}
															else if (TCP.FSIDRead_ByTCP(553, 0, 1, 0, 0, 0) != 0)
															{
																break;
															}
															form998_Wait.Process(true, ++num, maxProcess);
														}
														else
														{
															if (TCP.FSIDRead_ByTCP(553, 0, 0, 0, 0, 0) != 0)
															{
																break;
															}
															form998_Wait.Process(true, ++num, maxProcess);
														}
														if (TCP.FSIDRead_ByTCP(650, 0, (ushort)Page_Axis, 0, 0, 0) != 0)
														{
															break;
														}
														form998_Wait.Process(true, ++num, maxProcess);
														if (TCP.FSIDRead_ByTCP(657, 0, (ushort)Page_Axis, 0, 0, 0) != 0)
														{
															break;
														}
														form998_Wait.Process(true, ++num, maxProcess);
														if (!flag2)
														{
															break;
														}
													}
													num2 = ((!GB.UISys.IsReadSupportFTPClient) ? TCP.FSIDRead_ByFTP(20, 0u, 500u, 0) : TCP.FSIDRead_ByFTP(20));
													if (num2 == 0)
													{
														form998_Wait.Process(true, ++num, maxProcess);
														for (int k = 0; k < 500; k++)
														{
															if (GB.FSSeqGB[k].GeneralNavigatorMode > 0)
															{
																TrCSV.ReadPicFromController((uint)k, false, false);
															}
														}
														form998_Wait.Process(true, ++num, maxProcess);
														num2 = TCP.FSIDRead_ByTCP(550, 0, 0, 0, 0, 0);
														if (num2 == 0)
														{
															form998_Wait.Process(true, ++num, maxProcess);
															num2 = TCP.FSIDRead_ByTCP(551, 0, 0, 0, 0, 0);
															if (num2 == 0)
															{
																form998_Wait.Process(true, ++num, maxProcess);
																if (GB.CheckHMIVer(170, 6))
																{
																	for (int l = 0; l < sizeof(CtrlStaticReadStuc) / 2; l++)
																	{
																		GB.FSCtrlStaticRead.Data16[l] = 0;
																	}
																	GB.FSCtrlStaticRead.Data16[10] = 723;
																	GB.FSCtrlStaticRead.Data16[20] = 721;
																	GB.FSCtrlStaticRead.Data16[30] = 738;
																	GB.FSCtrlStaticRead.Data16[40] = 737;
																	GB.FSCtrlStaticRead.Data16[50] = 739;
																	GB.FSCtrlStaticRead.Data16[60] = 740;
																	GB.FSCtrlStaticRead.Data16[70] = 741;
																	GB.FSCtrlStaticRead.Data16[80] = 749;
																	GB.FSCtrlStaticRead.Data16[900] = 734;
																	GB.FSCtrlStaticRead.Data16[100] = 826;
																	GB.FSCtrlStaticRead.Data16[110] = 825;
																	GB.FSCtrlStaticRead.Data16[120] = 821;
																	GB.FSCtrlStaticRead.Data16[130] = 332;
																	GB.FSCtrlStaticRead.Data16[131] = 333;
																	GB.FSCtrlStaticRead.Data16[132] = 334;
																	num2 = TCP.FSIDRead_ByTCP(82, 0, 0, 0, 0, 0);
																	if (num2 == 0)
																	{
																		GB.FSCtrlAngleUnit.Mode = GB.FSCtrlStaticRead.Data16[10];
																		GB.FSCtrlTorqUnit.Mode = GB.FSCtrlStaticRead.Data16[20];
																		GB.FSCtrlTwoStageMode.Enable = GB.FSCtrlStaticRead.Data16[30];
																		GB.FSCtrlWarningWindow.Enable = GB.FSCtrlStaticRead.Data16[40];
																		GB.FSCtrlCurveStageUpLimit.Enable = GB.FSCtrlStaticRead.Data16[50];
																		GB.FSCtrlExportResultFile.Mode = GB.FSCtrlStaticRead.Data16[60];
																		GB.FSCtrlSamplingRate.Mode = GB.FSCtrlStaticRead.Data16[70];
																		GB.FSCtrlHomeStartPage.Mode = GB.FSCtrlStaticRead.Data16[80];
																		GB.FSCtrlCurveAllPositive.Enable = GB.FSCtrlStaticRead.Data16[90];
																		GB.FSCtrlKeyboardCursorBlinkingInResults.Enable = GB.FSCtrlStaticRead.Data16[100];
																		GB.FSCtrlTorqRateReplaceBySpeedCurve.Enable = GB.FSCtrlStaticRead.Data16[110];
																		GB.FSCtrlCurveScaleFromZero.Enable = GB.FSCtrlStaticRead.Data16[120];
																		if (GB.CheckHMIVer(171, 11))
																		{
																			GB.FSReportWatchList.AngleType1 = GB.FSCtrlStaticRead.Data16[130];
																			GB.FSReportWatchList.AngleType2 = GB.FSCtrlStaticRead.Data16[131];
																			GB.FSReportWatchList.TorqueType1 = GB.FSCtrlStaticRead.Data16[132];
																		}
																		else
																		{
																			GB.FSReportWatchList.AngleType1 = 0;
																			GB.FSReportWatchList.AngleType2 = 1;
																			GB.FSReportWatchList.TorqueType1 = 0;
																		}
																		form998_Wait.Process(true, ++num, maxProcess);
																		goto IL_1179;
																	}
																}
																else
																{
																	num2 = TCP.FSIDRead_ByTCP(568, 0, 0, 0, 0, 0);
																	if (num2 == 0)
																	{
																		form998_Wait.Process(true, ++num, maxProcess);
																		num2 = TCP.FSIDRead_ByTCP(555, 0, 0, 0, 0, 0);
																		if (num2 == 0)
																		{
																			form998_Wait.Process(true, ++num, maxProcess);
																			num2 = TCP.FSIDRead_ByTCP(558, 0, 0, 0, 0, 0);
																			if (num2 == 0)
																			{
																				form998_Wait.Process(true, ++num, maxProcess);
																				num2 = TCP.FSIDRead_ByTCP(559, 0, 0, 0, 0, 0);
																				if (num2 == 0)
																				{
																					form998_Wait.Process(true, ++num, maxProcess);
																					num2 = TCP.FSIDRead_ByTCP(560, 0, 0, 0, 0, 0);
																					if (num2 == 0)
																					{
																						form998_Wait.Process(true, ++num, maxProcess);
																						num2 = TCP.FSIDRead_ByTCP(561, 0, 0, 0, 0, 0);
																						if (num2 == 0)
																						{
																							form998_Wait.Process(true, ++num, maxProcess);
																							num2 = TCP.FSIDRead_ByTCP(562, 0, 0, 0, 0, 0);
																							if (num2 == 0)
																							{
																								form998_Wait.Process(true, ++num, maxProcess);
																								num2 = TCP.FSIDRead_ByTCP(571, 0, 0, 0, 0, 0);
																								if (num2 == 0)
																								{
																									form998_Wait.Process(true, ++num, maxProcess);
																									num2 = TCP.FSIDRead_ByTCP(573, 0, 0, 0, 0, 0);
																									if (num2 == 0)
																									{
																										form998_Wait.Process(true, ++num, maxProcess);
																										if (TCP.FSIDRead_ByTCP(575, 0, 0, 0, 0, 0) != 0)
																										{
																											num2 = 0;
																										}
																										form998_Wait.Process(true, ++num, maxProcess);
																										if (TCP.FSIDRead_ByTCP(576, 0, 0, 0, 0, 0) != 0)
																										{
																											num2 = 0;
																										}
																										form998_Wait.Process(true, ++num, maxProcess);
																										if (GB.CheckHMIVer(168, 0))
																										{
																											num2 = TCP.FSIDRead_ByTCP(581, 0, 0, 0, 0, 0);
																											form998_Wait.Process(true, ++num, maxProcess);
																										}
																										GB.FSReportWatchList.AngleType1 = 0;
																										GB.FSReportWatchList.AngleType2 = 1;
																										GB.FSReportWatchList.TorqueType1 = 0;
																										goto IL_1179;
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						goto IL_1571;
					}
					goto IL_157d;
					IL_1179:
					num2 = TCP.FSIDRead_ByTCP(556, 0, 0, 0, 0, 0);
					if (num2 == 0)
					{
						form998_Wait.Process(true, ++num, maxProcess);
						ushort num3 = 0;
						while (num3 <= 5 && TCP.FSIDRead_ByTCP(1572, 0, num3, 0, 0, 0) == 0)
						{
							form998_Wait.Process(true, ++num, maxProcess);
							num3++;
						}
						num2 = TCP.FSIDRead_ByTCP(1571, 0, 0, 0, 0, 0);
						if (num2 == 0)
						{
							form998_Wait.Process(true, ++num, maxProcess);
							if (TCP.FSIDRead_ByTCP(574, 0, 0, 0, 0, 0) != 0)
							{
								num2 = 0;
							}
							form998_Wait.Process(true, ++num, maxProcess);
							num2 = TCP.FSIDRead_ByTCP(565, 0, 0, 0, 0, 0);
							if (num2 == 0)
							{
								form998_Wait.Process(true, ++num, maxProcess);
								if (GB.CheckHMIVer(171, 1))
								{
									num2 = TCP.FSIDRead_ByTCP(53, 0, 1, 9516, 0, 1);
									GB.UISys.SpecCtrl = GB.FSCtrlLocalTable.Data16[0];
									form998_Wait.Process(true, ++num, maxProcess);
								}
								else
								{
									GB.UISys.SpecCtrl = 0;
									form998_Wait.Process(true, ++num, maxProcess);
								}
								if (GB.CheckHMIVer(170, 5))
								{
									if (TCP.FSIDRead_ByTCP(81, 1, 0, 0, 0, 0) != 0)
									{
										num2 = 0;
									}
									form998_Wait.Process(true, ++num, maxProcess);
								}
								else
								{
									for (ushort num4 = 0; num4 < 10; num4++)
									{
										if (TCP.FSIDRead_ByTCP(81, 0, (ushort)(num4 + 1), 0, 0, 0) != 0)
										{
											num2 = 0;
										}
										form998_Wait.Process(true, ++num, maxProcess);
									}
								}
								if (GB.CheckHMIVer(172, 0))
								{
									num2 = TCP.FSIDRead_ByTCP(813, 0, 299, 0, 0, 0);
									UpdateFWBn.Visible = ((TCP.FSNewFWInfo.UserSDVer > CurrSDVer && CurrSDVer != 999 && TCP.FSNewFWInfo.UserSDVer != 999) ? true : false);
								}
								else
								{
									UpdateFWBn.Visible = false;
								}
								num2 = ((TCP.CommunicationType != 0) ? TCP.FSIDWrite_ByTCP(12, 0, 0, 0, 0, 0) : TCP.FSIDWrite_ByTCP(10, 0, 0, 0, 0, 0));
								if (num2 == 0)
								{
									num2 = TCP.FSIDRead_ByTCP(1573, 0, 0, 0, 0, 0);
									if (num2 == 0)
									{
										if (GB.FSCtrlLanguage.Mode == 0)
										{
											cbLanguage.SelectedIndex = 0;
										}
										else if (GB.FSCtrlLanguage.Mode == 2)
										{
											cbLanguage.SelectedIndex = 2;
										}
										else if (GB.FSCtrlLanguage.Mode == 5)
										{
											cbLanguage.SelectedIndex = 3;
										}
										else
										{
											cbLanguage.SelectedIndex = 1;
										}
										UpdateLanguage();
										GB.ClearReportList(0);
										GB.ClearReportList(1);
										GB.ClearReportList(2);
										GB.ClearReportList(3);
										form998_Wait.Process(true, ++num, maxProcess);
									}
								}
							}
						}
					}
					goto IL_1571;
					IL_1571:
					form998_Wait.Process(false, 0, 0);
					goto IL_157d;
					IL_157d:
					if (num2 != 0)
					{
						TCP.ConnectInterrupt = true;
					}
					GB.BackGroundRunningInfo();
					ChangePageFunction(0);
					GB.ALNGMsgStartStopFunction(true);
				});
			}
		}

		private void IsProhibitBtn()
		{
			GB.PermissOfUserID_ShowPic(ref ParamBn, ref LockUnLockImg, 1);
			GB.PermissOfUserID_ShowPic(ref SeqBn, ref LockUnLockImg, 2);
			GB.PermissOfUserID_ShowPic(ref SrcBn, ref LockUnLockImg, 4);
			GB.PermissOfUserID_HidePic(ref CtrlBn, ref LockUnLockImg, 8);
			GB.PermissOfUserID_HidePic(ref ToolBn, ref LockUnLockImg, 16);
		}

		private void ShowHomeBtn(int val)
		{
			ParamBn.BackgroundImage = ((val == 1) ? IconA[0] : IconB[0]);
			SeqBn.BackgroundImage = ((val == 2) ? IconA[1] : IconB[1]);
			SrcBn.BackgroundImage = ((val == 3) ? IconA[2] : IconB[2]);
			ResultBn.BackgroundImage = ((val == 4) ? IconA[3] : IconB[3]);
			CtrlBn.BackgroundImage = ((val == 5) ? IconA[4] : IconB[4]);
			ToolBn.BackgroundImage = ((val == 6) ? IconA[5] : IconB[5]);
			ReportBn.BackgroundImage = ((val == 7) ? IconA[6] : IconB[6]);
			HelpBn.BackgroundImage = ((val == 8) ? IconA[7] : IconB[7]);
		}

		private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateLanguage();
			TCP.FSIDWrite_ByTCP(1513, 0, GB.FSCtrlLanguage.Mode, 0, 0, 0);
			ReflashUIScreen(ushort.MaxValue);
		}

		private void UpdateLanguage()
		{
			if (cbLanguage.SelectedIndex == 0)
			{
				MultiLanguage.SetDefaultLanguage("Chinese");
				GB.FSCtrlLanguage.Mode = 0;
			}
			else if (cbLanguage.SelectedIndex == 2)
			{
				MultiLanguage.SetDefaultLanguage("Sample");
				GB.FSCtrlLanguage.Mode = 2;
			}
			else if (cbLanguage.SelectedIndex == 3)
			{
				MultiLanguage.SetDefaultLanguage("Japan");
				GB.FSCtrlLanguage.Mode = 5;
			}
			else
			{
				MultiLanguage.SetDefaultLanguage("English");
				GB.FSCtrlLanguage.Mode = 1;
			}
			foreach (Form form in Application.OpenForms)
			{
				MultiLanguage.LoadLanguage(form);
			}
			foreach (Control con in base.Controls)
			{
				try
				{
					if (con is Button)
					{
						con.Font = new Font(con.Font.Name, con.Font.Size * FormControlZoom.ScreenFontZoom, con.Font.Style, con.Font.Unit);
					}
				}
				catch
				{
				}
			}
			GB.UISys.RangeStr = MultiLanguage.GetStr("ButtonBase", "lab_Range");
			GB.UISys.UploadToCtrl = MultiLanguage.GetStr("ButtonBase", "lab_UploadToCtrl");
			GB.UISys.DownloadFromCtrl = MultiLanguage.GetStr("ButtonBase", "lab_DownloadFromCtrl");
			GB.UISys.ImportFromCSV = MultiLanguage.GetStr("ButtonBase", "lab_ImportFromCSV");
			GB.UISys.ExportToCSV = MultiLanguage.GetStr("ButtonBase", "lab_ExportToCSV");
			GB.UISys.ExportResultInfoToCSV = MultiLanguage.GetStr("ButtonBase", "lab_ExportResultInfoToCSV");
			GB.UISys.ExportSingleResultAndCurveToCSV = MultiLanguage.GetStr("ButtonBase", "lab_ExportSingleResultAndCurveToCSV");
			GB.UISys.ShowFilterConditions = MultiLanguage.GetStr("ButtonBase", "lab_ShowFilterConditions");
			GB.UISys.StopFollowingTheLatestEntry = MultiLanguage.GetStr("ButtonBase", "lab_StopFollowingTheLatestEntry");
			GB.UISys.SelectMultipleReportItemsForAnalysis = MultiLanguage.GetStr("ButtonBase", "lab_SelectMultipleReportItemsForAnalysis");
			AlarmMsg(99);
		}

		private void OpenChildForm(Form childForm)
		{
			if (activeForm != null)
			{
				activeForm.Close();
			}
			activeForm = childForm;
			childForm.TopLevel = false;
			childForm.FormBorderStyle = FormBorderStyle.None;
			childForm.Dock = DockStyle.Fill;
			panelChildForm.Controls.Add(childForm);
			panelChildForm.Tag = childForm;
			childForm.BringToFront();
			childForm.Show();
			GB.UISys.UIPageNonSave = 0;
		}

		private void GetForm011(bool ConnectSW, uint CtrlVer, uint CtrlDual, uint Tool1MaxTorque, uint Tool2MaxTorque)
		{
			if (ConnectSW)
			{
				TCP.RetryConnect();
				if (!TCP.ConnectStatus)
				{
					Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 5003, "");
					Form995.Show(this);
				}
				return;
			}
			TCP.DisConnectFunc();
			GB.UISys.PCSoftSupport = false;
			ReflashFormText(ConnectSW, CtrlVer, CtrlDual, Tool1MaxTorque, Tool2MaxTorque);
			GB.ChangeDefaultTorqUnit(1);
			GB.DetectCtrlMode();
			GB.FSSrcAll.FSSrcManualX[0] = (GB.FSSrcAll.FSSrcManualY[0] = (GB.FSSrcAll.FSSrcManual_DualMix[0] = (GB.FSSrcAll.FSSrcManual_DualSync[0] = GB.SrcDeflaut(0))));
			for (int i = 0; i < 255; i++)
			{
				GB.FSSrcAll.FSSrcBitsX[i] = (GB.FSSrcAll.FSSrcBitsY[i] = (GB.FSSrcAll.FSSrcBits_DualMix[i] = (GB.FSSrcAll.FSSrcBits_DualSync[i] = GB.SrcDeflaut(1))));
			}
			for (int j = 0; j < 500; j++)
			{
				GB.FSSrcAll.FSSrcScannerX[j] = (GB.FSSrcAll.FSSrcScannerY[j] = (GB.FSSrcAll.FSSrcScanner_DualMix[j] = (GB.FSSrcAll.FSSrcScanner_DualSync[j] = GB.SrcDeflaut(2))));
			}
		}

		private void ReflashFormText(bool ConnectSW, uint CtrlVer, uint CtrlDual, uint Tool1MaxTorque, uint Tool2MaxTorque)
		{
			GB.SetModelNameType((int)CtrlVer);
			Page_Axis = GB.FirstDetectPageAxis(ref GB.UISys.PageAxisInfo);
			string CtrlTypeStr = "";
			if (CtrlVer == 1)
			{
				if (!ConnectSW)
				{
					GB.FSToolXActive.ActiveEnable = 1;
					GB.FSToolYActive.ActiveEnable = 0;
				}
				else
				{
					GB.FSToolXActive.ActiveEnable = (ushort)((GB.FSToolXModelInfo.ToolTorque_Nm > 0) ? 1 : 0);
					GB.FSToolYActive.ActiveEnable = (ushort)((GB.FSToolYModelInfo.ToolTorque_Nm > 0) ? 1 : 0);
				}
				CtrlTypeStr = MultiLanguage.GetStr("Form011_Setting", "lab_CtrlType0");
			}
			else if (CtrlVer == 3)
			{
				if (!ConnectSW)
				{
					GB.FSToolXActive.ActiveEnable = 1;
					GB.FSToolYActive.ActiveEnable = 0;
				}
				else
				{
					GB.FSToolXActive.ActiveEnable = (ushort)((GB.FSToolXModelInfo.ToolTorque_Nm > 0) ? 1 : 0);
					GB.FSToolYActive.ActiveEnable = (ushort)((GB.FSToolYModelInfo.ToolTorque_Nm > 0) ? 1 : 0);
				}
				CtrlTypeStr = MultiLanguage.GetStr("Form011_Setting", "lab_CtrlType1");
			}
			else if (CtrlVer == 0 || CtrlVer == 2)
			{
				if (!ConnectSW)
				{
					if (CtrlDual == 0)
					{
						GB.FSToolXActive.ActiveEnable = 1;
						GB.FSToolYActive.ActiveEnable = 0;
					}
					else
					{
						GB.FSToolXActive.ActiveEnable = (ushort)((GB.UISys.ToolTorqueSpec_X > 0) ? 1 : 0);
						GB.FSToolYActive.ActiveEnable = (ushort)((GB.UISys.ToolTorqueSpec_Y > 0) ? 1 : 0);
					}
				}
				else
				{
					GB.FSToolXActive.ActiveEnable = (ushort)((GB.FSToolXModelInfo.ToolTorque_Nm > 0) ? 1 : 0);
					GB.FSToolYActive.ActiveEnable = (ushort)((GB.FSToolYModelInfo.ToolTorque_Nm > 0) ? 1 : 0);
				}
				CtrlTypeStr = ((CtrlDual != 0) ? MultiLanguage.GetStr("Form011_Setting", "lab_CtrlType3") : MultiLanguage.GetStr("Form011_Setting", "lab_CtrlType2"));
			}
			string Tool1TypeStr = "";
			Tool1TypeStr = ((Tool1MaxTorque != 9999) ? MultiLanguage.GetStr("Form011_Setting", "lab_ToolType" + Tool1MaxTorque) : MultiLanguage.GetStr("Form011_Setting", "lab_ToolTypeNon"));
			string Tool2TypeStr = "";
			Tool2TypeStr = ((Tool2MaxTorque != 9999) ? MultiLanguage.GetStr("Form011_Setting", "lab_ToolType" + Tool2MaxTorque) : MultiLanguage.GetStr("Form011_Setting", "lab_ToolTypeNon"));
			if (GB.FSModelTypeInfo.MesModelType == 0)
			{
				if (GB.UISys.CtrlDualTool == 1)
				{
					Text = VerStr + ", " + GB.UISys.IPstr + ", " + CtrlTypeStr + "," + Tool1TypeStr + "," + Tool2TypeStr;
				}
				else
				{
					Text = VerStr + ", " + GB.UISys.IPstr + ", " + CtrlTypeStr + "," + Tool1TypeStr;
				}
			}
			else
			{
				Text = VerStr + ", " + GB.UISys.IPstr + ", " + CtrlTypeStr + "," + Tool1TypeStr;
			}
			GB.UISys.IsReadSupportFTPClient = false;
		}

		private void Form_001Main_FormClosed(object sender, FormClosedEventArgs e)
		{
			TCP.DisConnectFunc();
			if (GB.TCPHandshakeEvent != null)
			{
				GB.TCPHandshakeEvent.Close();
			}
			GB.Form001TCPFlag = false;
			GB.BackGroundThreadFlag = false;
			GB.ReflashThreadFlag = false;
			GB.Form100ThreadFlag = false;
			GB.Form200ThreadFlag = false;
			GB.Form300ThreadFlag = false;
			GB.Form400ThreadFlag = false;
			GB.Form409ThreadFlag = false;
			GB.Form500ThreadFlag = false;
			GB.Form592ThreadFlag = false;
			GB.Form600ThreadFlag = false;
			GB.Form700ThreadFlag = false;
			GB.FTPServerFlag = false;
			if (GB.BackGroundEvent != null)
			{
				if (GB.BackGroundThreadWait)
				{
					GB.BackGroundEvent.Set();
					GB.BackGroundThreadWait = false;
				}
				GB.BackGroundEvent.Close();
			}
			if (GB.ReflashEvent != null)
			{
				if (GB.ReflashThreadWait)
				{
					GB.ReflashEvent.Set();
					GB.ReflashThreadWait = false;
				}
				GB.ReflashEvent.Close();
			}
			if (GB.MissionForm001TCPThread != null)
			{
				GB.MissionForm001TCPThread.Abort();
			}
			if (GB.MissionBackGroundThread != null)
			{
				GB.MissionBackGroundThread.Abort();
			}
			if (GB.MissionReflashThread != null)
			{
				GB.MissionReflashThread.Abort();
			}
			if (GB.MissionForm100Thread != null)
			{
				GB.MissionForm100Thread.Abort();
			}
			if (GB.MissionForm200Thread != null)
			{
				GB.MissionForm200Thread.Abort();
			}
			if (GB.MissionForm300Thread != null)
			{
				GB.MissionForm300Thread.Abort();
			}
			if (GB.MissionForm400Thread != null)
			{
				GB.MissionForm400Thread.Abort();
			}
			if (GB.MissionForm409Thread != null)
			{
				GB.MissionForm409Thread.Abort();
			}
			if (GB.MissionForm500Thread != null)
			{
				GB.MissionForm500Thread.Abort();
			}
			if (GB.MissionForm592Thread != null)
			{
				GB.MissionForm592Thread.Abort();
			}
			if (GB.MissionForm600Thread != null)
			{
				GB.MissionForm600Thread.Abort();
			}
			if (GB.MissionForm700Thread != null)
			{
				GB.MissionForm700Thread.Abort();
			}
			if (GB.MissionFTPServerListenerThread != null)
			{
				GB.MissionFTPServerListenerThread.Abort();
			}
			GB.StartTimerWithSleepFlag = false;
			GB.StartTimerWithSleepWinformLive = false;
			if (GB.ALNGMsgTimer != null)
			{
				GB.ALNGMsgStartStopFunction(false);
			}
			if (GB.GetLevelTimer != null)
			{
				GB.GetLevelTimer.Stop();
			}
			if (GB.GetCommunTimer != null)
			{
				GB.GetCommunTimer.Stop();
			}
			if (GB.GetPositionArmTimer != null)
			{
				GB.GetPositionArmTimer.Stop();
			}
			if (GB.FTPServerListener != null)
			{
				GB.FTPServerListener.Stop();
			}
			ForceFormClose(typeof(Form100_Param));
			ForceFormClose(typeof(Form200_Seq));
			ForceFormClose(typeof(Form300_Source));
			ForceFormClose(typeof(Form400_Results));
			ForceFormClose(typeof(Form409_ResultsList));
			ForceFormClose(typeof(Form500_Controller));
			ForceFormClose(typeof(Form600_Tool));
			ForceFormClose(typeof(Form700_Report));
			Console.WriteLine("Closed Windows!");
		}

		private void ParamBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(1);
			}
			else
			{
				JumpMessagePage(1);
			}
		}

		private void SeqBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(2);
			}
			else
			{
				JumpMessagePage(2);
			}
		}

		private void SrcBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(3);
			}
			else
			{
				JumpMessagePage(3);
			}
		}

		private void ResultBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(4);
			}
			else
			{
				JumpMessagePage(4);
			}
		}

		private void CtrlBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(5);
			}
			else
			{
				JumpMessagePage(5);
			}
		}

		private void ToolBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(6);
			}
			else
			{
				JumpMessagePage(6);
			}
		}

		private void ReportBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(7);
			}
			else
			{
				JumpMessagePage(7);
			}
		}

		private void HelpBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(8);
			}
			else
			{
				JumpMessagePage(8);
			}
		}

		private void SettingBn_Click(object sender, EventArgs e)
		{
			if (GB.UISys.UIPageNonSave == 0)
			{
				ChangePageFunction(10);
			}
			else
			{
				JumpMessagePage(10);
			}
		}

		private void JumpMessagePage(ushort ChagePage)
		{
			this.ChagePage = ChagePage;
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += ChangePageFunction;
			if (GB.UISys.UIPageNonSave == 1100)
			{
				Form996.SetSubForm(FormType.MegParamNonSave);
			}
			else if (GB.UISys.UIPageNonSave == 1200)
			{
				Form996.SetSubForm(FormType.MegSeqNonSave);
			}
			Form996.ShowDialog(this);
		}

		public void ChangePageFunction(ushort ChagePage, bool SW)
		{
			this.SW = SW;
			this.ChagePage = ChagePage;
			ChangePageFunction();
		}

		public void ChangePageFunction(ushort ChagePage)
		{
			SW = false;
			this.ChagePage = ChagePage;
			ChangePageFunction();
		}

		public void ChangePageFunction()
		{
			if (ChagePage == 0)
			{
				if (!FormDetect(typeof(Form010_Idel)) || SW)
				{
					ShowHomeBtn(0);
					OpenChildForm(new Form010_Idel(GB));
				}
			}
			else if (ChagePage == 1)
			{
				if (!FormDetect(typeof(Form100_Param)) || SW)
				{
					ShowHomeBtn(1);
					OpenChildForm(new Form100_Param(GB, TCP, TrCSV));
				}
			}
			else if (ChagePage == 2)
			{
				if (!FormDetect(typeof(Form200_Seq)) || SW)
				{
					ShowHomeBtn(2);
					OpenChildForm(new Form200_Seq(GB, TCP, TrCSV));
				}
			}
			else if (ChagePage == 3)
			{
				if (!FormDetect(typeof(Form300_Source)) || SW)
				{
					ShowHomeBtn(3);
					OpenChildForm(new Form300_Source(GB, TCP, TrCSV));
				}
			}
			else if (ChagePage == 4)
			{
				if (GB.FSSrcMode.ActionMode == 2)
				{
					if (!FormDetect(typeof(Form402_ResultsDualTool)) || SW)
					{
						ShowHomeBtn(4);
						Form402_ResultsDualTool Form402 = new Form402_ResultsDualTool(GB, TCP, TrCSV);
						Form402.CreateJumpPageEvent += RstJumpPage;
						OpenChildForm(Form402);
					}
				}
				else if (GB.FSSrcMode.ActionMode == 1)
				{
					if (!FormDetect(typeof(Form401_ResultsMixTool)) || SW)
					{
						ShowHomeBtn(4);
						Form401_ResultsMixTool Form403 = new Form401_ResultsMixTool(GB, TCP, TrCSV);
						Form403.CreateJumpPageEvent += RstJumpPage;
						OpenChildForm(Form403);
					}
				}
				else if (!FormDetect(typeof(Form400_Results)) || SW)
				{
					ShowHomeBtn(4);
					Form400_Results Form404 = new Form400_Results(GB, TCP, TrCSV);
					Form404.CreateJumpPageEvent += RstJumpPage;
					OpenChildForm(Form404);
				}
			}
			else if (ChagePage == 5)
			{
				if (!FormDetect(typeof(Form500_Controller)) || SW)
				{
					ShowHomeBtn(5);
					OpenChildForm(new Form500_Controller(GB, TCP, TrCSV));
				}
			}
			else if (ChagePage == 6)
			{
				if (!FormDetect(typeof(Form600_Tool)) || SW)
				{
					ShowHomeBtn(6);
					OpenChildForm(new Form600_Tool(GB, TCP, TrCSV));
				}
			}
			else if (ChagePage == 7)
			{
				if (!FormDetect(typeof(Form700_Report)) || SW)
				{
					ShowHomeBtn(7);
					OpenChildForm(new Form700_Report(GB, TCP, TrCSV));
				}
			}
			else if (ChagePage == 8)
			{
				if (!FormDetect(typeof(Form800_Help)) || SW)
				{
					ShowHomeBtn(8);
					OpenChildForm(new Form800_Help(GB, TCP, TrCSV));
				}
			}
			else if (ChagePage == 10)
			{
				if (!FormDetect(typeof(Form010_Idel)) || SW)
				{
					ShowHomeBtn(0);
					OpenChildForm(new Form010_Idel(GB));
				}
				if (!FormDetect(typeof(Form011_Setting)) || SW)
				{
					UpdateFWBn.Visible = false;
					Form011_Setting Form405 = new Form011_Setting(GB, TCP, TrCSV);
					Form405.CreateID += GetForm011;
					Form405.ShowDialog(this);
				}
			}
			else if (ChagePage == 101)
			{
				if (!FormDetect(typeof(Form100_Param)) || SW)
				{
					ShowHomeBtn(1);
					Form100_Param Form406 = new Form100_Param(GB, TCP, TrCSV);
					OpenChildForm(Form406);
					Form406.GetFormCurrParam();
				}
			}
			else if (ChagePage == 102)
			{
				if (!FormDetect(typeof(Form200_Seq)) || SW)
				{
					ShowHomeBtn(2);
					Form200_Seq Form407 = new Form200_Seq(GB, TCP, TrCSV);
					OpenChildForm(Form407);
					Form407.GetFormCurrSeq();
				}
			}
			else if (ChagePage == 103 && (!FormDetect(typeof(Form300_Source)) || SW))
			{
				ShowHomeBtn(3);
				Form300_Source Form408 = new Form300_Source(GB, TCP, TrCSV);
				OpenChildForm(Form408);
				Form408.GetFormCurrSrc();
			}
			Point point = (base.AutoScrollPosition = new Point(0, 0));
			_scrollPosition = point;
			SW = false;
		}

		private void RstJumpPage(int Page)
		{
			ChangePageFunction((ushort)(100 + Page));
		}

		private void RstResetBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(402, 0, 0, 0, 0, 0);
		}

		private void UserBn_Click(object sender, EventArgs e)
		{
			Form507_LogIn Form507 = new Form507_LogIn(GB, TCP);
			Form507.CreateCloseEvent += GetForm507;
			Form507.ShowDialog(this);
		}

		public void GetForm507(uint ID)
		{
			if (TCP.ConnectStatus)
			{
				TCP.FSIDRead_ByTCP(1571, 0, 0, 0, 0, 0);
			}
			else
			{
				GB.ExFSUser.UserID = ID;
			}
			ChangePageFunction(0);
		}

		private void Form_001Main_Scroll(object sender, ScrollEventArgs e)
		{
			_scrollPosition = base.AutoScrollPosition;
		}

		private void GetForm996YesInfoDownloadNewFW()
		{
			GB.ALNGMsgStartStopFunction(false);
			if (GB.CheckHMIVer(172, 0) && GB.FSModelTypeInfo.VerHMIBIOS >= 125)
			{
				try
				{
					string desktopPath = Application.StartupPath;
					string SaveFileName = Path.Combine(desktopPath, "SDSoft.zip");
					int Err = 0;
					Err = TCP.FSIDRead_ByTCP(813, 0, 299, 0, 0, 0);
					int TotalRow = 0;
					if (TCP.FSNewFWInfo.UserRcp32Ver == 1)
					{
						TotalRow = 4096;
					}
					if (TotalRow == 0)
					{
						return;
					}
					if (File.Exists(SaveFileName))
					{
						File.Delete(SaveFileName);
					}
					int idx = 0;
					int RetryCmd = 0;
					uint LastTimeAllCRC = 0u;
					uint caluAllCRC = 0u;
					uint AllCRC = 0u;
					Form998_Wait Form998 = new Form998_Wait(GB);
					Form998.Show();
					Form998.Process(true, 0, TotalRow);
					using (MemoryStream memStream = new MemoryStream())
					{
						while (idx < TotalRow)
						{
							Form998.Process(true, idx, TotalRow);
							Err = TCP.FSIDRead_ByTCP(813, 0, 200, (ushort)(idx & 0xFFFF), (ushort)(idx / 65536), 1000);
							uint caluCRC = 0u;
							int EachRowByteSize = GB.TcpRD.Data2;
							int EachRowCRC = GB.TcpRD.Data3;
							for (int i = 0; i < EachRowByteSize; i += 4)
							{
								if (i < 8000)
								{
									uint Data32 = (uint)(TCP.FSNewFWInfo.RawData[i / 2 + 1] * 65536 + TCP.FSNewFWInfo.RawData[i / 2]);
									caluCRC += Data32;
									caluCRC &= 0xFFFF;
								}
							}
							if (caluCRC == EachRowCRC && EachRowByteSize > 0)
							{
								AllCRC = GB.TcpRD.Data4;
								caluAllCRC += caluCRC;
								caluAllCRC &= 0xFFFF;
							}
							if (caluCRC == EachRowCRC && EachRowByteSize > 0 && caluAllCRC == AllCRC)
							{
								for (int j = 0; j < EachRowByteSize; j += 4)
								{
									if (j < 8000)
									{
										if (j < EachRowByteSize)
										{
											memStream.WriteByte((byte)((TCP.FSNewFWInfo.RawData[j / 2] >> 8) & 0xFF));
										}
										if (j + 1 < EachRowByteSize)
										{
											memStream.WriteByte((byte)(TCP.FSNewFWInfo.RawData[j / 2] & 0xFF));
										}
										if (j + 2 < EachRowByteSize)
										{
											memStream.WriteByte((byte)((TCP.FSNewFWInfo.RawData[j / 2 + 1] >> 8) & 0xFF));
										}
										if (j + 3 < EachRowByteSize)
										{
											memStream.WriteByte((byte)(TCP.FSNewFWInfo.RawData[j / 2 + 1] & 0xFF));
										}
									}
								}
								RetryCmd = 0;
								idx++;
								LastTimeAllCRC = caluAllCRC;
							}
							else
							{
								if (RetryCmd >= 3)
								{
									break;
								}
								caluAllCRC = LastTimeAllCRC;
								RetryCmd++;
							}
							if (EachRowByteSize != 0 || caluAllCRC != AllCRC)
							{
								continue;
							}
							using (BinaryWriter PicW = new BinaryWriter(File.Open(SaveFileName, FileMode.Create)))
							{
								PicW.Write(memStream.ToArray());
							}
							string extractPath = Path.Combine(desktopPath, "Cache");
							string extractAndSDSoftPath = Path.Combine(extractPath, "SDSoft");
							if (!Directory.Exists(extractPath))
							{
								Directory.CreateDirectory(extractPath);
							}
							string exePath = "";
							if (File.Exists(SaveFileName))
							{
								ZipFile.ExtractToDirectory(SaveFileName, extractPath);
								string[] files = Directory.GetFiles(extractAndSDSoftPath);
								foreach (string filePath in files)
								{
									string fileName = Path.GetFileName(filePath);
									string destPath = Path.Combine(desktopPath, fileName);
									if (!File.Exists(destPath))
									{
										if (Path.GetExtension(filePath).Equals(".exe", StringComparison.OrdinalIgnoreCase))
										{
											exePath = destPath;
										}
										File.Move(filePath, destPath);
										Console.WriteLine("Move：" + fileName);
									}
								}
							}
							File.Delete(SaveFileName);
							Directory.Delete(extractPath, true);
							if (exePath != "")
							{
								Process.Start(exePath);
								Application.Exit();
							}
							break;
						}
						Form998.Process(false, 0, 0);
					}
				}
				catch
				{
				}
			}
			GB.ALNGMsgStartStopFunction(true);
		}

		private void UpdateFWBn_Click(object sender, EventArgs e)
		{
			Form996_JumpConfirmYesNo Form996 = new Form996_JumpConfirmYesNo(GB);
			Form996.CreateYesAns += GetForm996YesInfoDownloadNewFW;
			Form996.SetSubForm(FormType.MegDownloadNewFW);
			Form996.ShowDialog(this);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form_001Main));
			this.panelChildForm = new System.Windows.Forms.Panel();
			this.cbLanguage = new System.Windows.Forms.ComboBox();
			this.lab_AlarmMsgBackgroud = new System.Windows.Forms.Label();
			this.lab_HanderTitle = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.lab_UserName = new System.Windows.Forms.Label();
			this.UserBn = new System.Windows.Forms.PictureBox();
			this.OnOfflinePB = new System.Windows.Forms.PictureBox();
			this.RstResetBn = new System.Windows.Forms.Button();
			this.ToolBn = new System.Windows.Forms.Button();
			this.ReportBn = new System.Windows.Forms.Button();
			this.CtrlBn = new System.Windows.Forms.Button();
			this.HelpBn = new System.Windows.Forms.Button();
			this.ResultBn = new System.Windows.Forms.Button();
			this.SrcBn = new System.Windows.Forms.Button();
			this.SeqBn = new System.Windows.Forms.Button();
			this.SettingBn = new System.Windows.Forms.Button();
			this.ParamBn = new System.Windows.Forms.Button();
			this.ParamBnT = new System.Windows.Forms.Button();
			this.SeqBnT = new System.Windows.Forms.Button();
			this.SrcBnT = new System.Windows.Forms.Button();
			this.CtrlBnT = new System.Windows.Forms.Button();
			this.ToolBnT = new System.Windows.Forms.Button();
			this.UpdateFWBn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)this.UserBn).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.OnOfflinePB).BeginInit();
			base.SuspendLayout();
			resources.ApplyResources(this.panelChildForm, "panelChildForm");
			this.panelChildForm.BackColor = System.Drawing.Color.White;
			this.panelChildForm.Name = "panelChildForm";
			this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbLanguage, "cbLanguage");
			this.cbLanguage.FormattingEnabled = true;
			this.cbLanguage.Name = "cbLanguage";
			this.lab_AlarmMsgBackgroud.BackColor = System.Drawing.Color.Red;
			resources.ApplyResources(this.lab_AlarmMsgBackgroud, "lab_AlarmMsgBackgroud");
			this.lab_AlarmMsgBackgroud.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_AlarmMsgBackgroud.Name = "lab_AlarmMsgBackgroud";
			this.lab_HanderTitle.BackColor = System.Drawing.Color.FromArgb(0, 135, 220);
			resources.ApplyResources(this.lab_HanderTitle, "lab_HanderTitle");
			this.lab_HanderTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_HanderTitle.Name = "lab_HanderTitle";
			this.label1.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.label1.Name = "label1";
			this.lab_UserName.BackColor = System.Drawing.Color.LightGray;
			resources.ApplyResources(this.lab_UserName, "lab_UserName");
			this.lab_UserName.Name = "lab_UserName";
			this.lab_UserName.Click += new System.EventHandler(UserBn_Click);
			this.UserBn.BackColor = System.Drawing.Color.LightGray;
			this.UserBn.BackgroundImage = SD3Soft.Properties.Resources.登入;
			resources.ApplyResources(this.UserBn, "UserBn");
			this.UserBn.Name = "UserBn";
			this.UserBn.TabStop = false;
			this.UserBn.Click += new System.EventHandler(UserBn_Click);
			this.OnOfflinePB.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.OnOfflinePB, "OnOfflinePB");
			this.OnOfflinePB.Name = "OnOfflinePB";
			this.OnOfflinePB.TabStop = false;
			this.OnOfflinePB.Click += new System.EventHandler(SettingBn_Click);
			this.RstResetBn.BackColor = System.Drawing.Color.Transparent;
			this.RstResetBn.BackgroundImage = SD3Soft.Properties.Resources.A_重置_ICON_01;
			resources.ApplyResources(this.RstResetBn, "RstResetBn");
			this.RstResetBn.Name = "RstResetBn";
			this.RstResetBn.UseVisualStyleBackColor = false;
			this.RstResetBn.Click += new System.EventHandler(RstResetBn_Click);
			this.ToolBn.BackColor = System.Drawing.Color.Transparent;
			this.ToolBn.BackgroundImage = SD3Soft.Properties.Resources.ToolB;
			resources.ApplyResources(this.ToolBn, "ToolBn");
			this.ToolBn.ForeColor = System.Drawing.Color.White;
			this.ToolBn.Name = "ToolBn";
			this.ToolBn.UseVisualStyleBackColor = false;
			this.ToolBn.Click += new System.EventHandler(ToolBn_Click);
			this.ReportBn.BackColor = System.Drawing.Color.Transparent;
			this.ReportBn.BackgroundImage = SD3Soft.Properties.Resources.ReportB;
			resources.ApplyResources(this.ReportBn, "ReportBn");
			this.ReportBn.ForeColor = System.Drawing.Color.White;
			this.ReportBn.Name = "ReportBn";
			this.ReportBn.UseVisualStyleBackColor = false;
			this.ReportBn.Click += new System.EventHandler(ReportBn_Click);
			this.CtrlBn.BackColor = System.Drawing.Color.Transparent;
			this.CtrlBn.BackgroundImage = SD3Soft.Properties.Resources.CtrlB;
			resources.ApplyResources(this.CtrlBn, "CtrlBn");
			this.CtrlBn.ForeColor = System.Drawing.Color.White;
			this.CtrlBn.Name = "CtrlBn";
			this.CtrlBn.UseVisualStyleBackColor = false;
			this.CtrlBn.Click += new System.EventHandler(CtrlBn_Click);
			this.HelpBn.BackColor = System.Drawing.Color.Transparent;
			this.HelpBn.BackgroundImage = SD3Soft.Properties.Resources.HelpB;
			resources.ApplyResources(this.HelpBn, "HelpBn");
			this.HelpBn.ForeColor = System.Drawing.Color.White;
			this.HelpBn.Name = "HelpBn";
			this.HelpBn.UseVisualStyleBackColor = false;
			this.HelpBn.Click += new System.EventHandler(HelpBn_Click);
			this.ResultBn.BackColor = System.Drawing.Color.Transparent;
			this.ResultBn.BackgroundImage = SD3Soft.Properties.Resources.ResultB;
			resources.ApplyResources(this.ResultBn, "ResultBn");
			this.ResultBn.ForeColor = System.Drawing.Color.White;
			this.ResultBn.Name = "ResultBn";
			this.ResultBn.UseVisualStyleBackColor = false;
			this.ResultBn.Click += new System.EventHandler(ResultBn_Click);
			this.SrcBn.BackColor = System.Drawing.Color.Transparent;
			this.SrcBn.BackgroundImage = SD3Soft.Properties.Resources.SrcB;
			resources.ApplyResources(this.SrcBn, "SrcBn");
			this.SrcBn.ForeColor = System.Drawing.Color.White;
			this.SrcBn.Name = "SrcBn";
			this.SrcBn.UseVisualStyleBackColor = false;
			this.SrcBn.Click += new System.EventHandler(SrcBn_Click);
			this.SeqBn.BackColor = System.Drawing.Color.Transparent;
			this.SeqBn.BackgroundImage = SD3Soft.Properties.Resources.SeqB;
			resources.ApplyResources(this.SeqBn, "SeqBn");
			this.SeqBn.ForeColor = System.Drawing.Color.White;
			this.SeqBn.Name = "SeqBn";
			this.SeqBn.UseVisualStyleBackColor = false;
			this.SeqBn.Click += new System.EventHandler(SeqBn_Click);
			this.SettingBn.BackgroundImage = SD3Soft.Properties.Resources.B_設定_ICON_01;
			resources.ApplyResources(this.SettingBn, "SettingBn");
			this.SettingBn.FlatAppearance.BorderSize = 0;
			this.SettingBn.Name = "SettingBn";
			this.SettingBn.UseVisualStyleBackColor = true;
			this.SettingBn.Click += new System.EventHandler(SettingBn_Click);
			this.ParamBn.BackColor = System.Drawing.Color.Transparent;
			this.ParamBn.BackgroundImage = SD3Soft.Properties.Resources.ParamB;
			resources.ApplyResources(this.ParamBn, "ParamBn");
			this.ParamBn.ForeColor = System.Drawing.Color.White;
			this.ParamBn.Name = "ParamBn";
			this.ParamBn.UseVisualStyleBackColor = false;
			this.ParamBn.Click += new System.EventHandler(ParamBn_Click);
			this.ParamBnT.BackColor = System.Drawing.Color.Transparent;
			this.ParamBnT.BackgroundImage = SD3Soft.Properties.Resources.ParamB;
			resources.ApplyResources(this.ParamBnT, "ParamBnT");
			this.ParamBnT.ForeColor = System.Drawing.Color.White;
			this.ParamBnT.Name = "ParamBnT";
			this.ParamBnT.UseVisualStyleBackColor = false;
			this.SeqBnT.BackColor = System.Drawing.Color.Transparent;
			this.SeqBnT.BackgroundImage = SD3Soft.Properties.Resources.SeqB;
			resources.ApplyResources(this.SeqBnT, "SeqBnT");
			this.SeqBnT.ForeColor = System.Drawing.Color.White;
			this.SeqBnT.Name = "SeqBnT";
			this.SeqBnT.UseVisualStyleBackColor = false;
			this.SrcBnT.BackColor = System.Drawing.Color.Transparent;
			this.SrcBnT.BackgroundImage = SD3Soft.Properties.Resources.SrcB;
			resources.ApplyResources(this.SrcBnT, "SrcBnT");
			this.SrcBnT.ForeColor = System.Drawing.Color.White;
			this.SrcBnT.Name = "SrcBnT";
			this.SrcBnT.UseVisualStyleBackColor = false;
			this.CtrlBnT.BackColor = System.Drawing.Color.Transparent;
			this.CtrlBnT.BackgroundImage = SD3Soft.Properties.Resources.CtrlB;
			resources.ApplyResources(this.CtrlBnT, "CtrlBnT");
			this.CtrlBnT.ForeColor = System.Drawing.Color.White;
			this.CtrlBnT.Name = "CtrlBnT";
			this.CtrlBnT.UseVisualStyleBackColor = false;
			this.ToolBnT.BackColor = System.Drawing.Color.Transparent;
			this.ToolBnT.BackgroundImage = SD3Soft.Properties.Resources.ToolB;
			resources.ApplyResources(this.ToolBnT, "ToolBnT");
			this.ToolBnT.ForeColor = System.Drawing.Color.White;
			this.ToolBnT.Name = "ToolBnT";
			this.ToolBnT.UseVisualStyleBackColor = false;
			this.UpdateFWBn.BackColor = System.Drawing.Color.LightGray;
			this.UpdateFWBn.BackgroundImage = SD3Soft.Properties.Resources.New;
			resources.ApplyResources(this.UpdateFWBn, "UpdateFWBn");
			this.UpdateFWBn.FlatAppearance.BorderSize = 0;
			this.UpdateFWBn.ForeColor = System.Drawing.Color.Transparent;
			this.UpdateFWBn.Name = "UpdateFWBn";
			this.UpdateFWBn.UseVisualStyleBackColor = false;
			this.UpdateFWBn.Click += new System.EventHandler(UpdateFWBn_Click);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
			base.CausesValidation = false;
			base.Controls.Add(this.UpdateFWBn);
			base.Controls.Add(this.OnOfflinePB);
			base.Controls.Add(this.UserBn);
			base.Controls.Add(this.lab_UserName);
			base.Controls.Add(this.RstResetBn);
			base.Controls.Add(this.ToolBn);
			base.Controls.Add(this.ReportBn);
			base.Controls.Add(this.CtrlBn);
			base.Controls.Add(this.HelpBn);
			base.Controls.Add(this.ResultBn);
			base.Controls.Add(this.lab_AlarmMsgBackgroud);
			base.Controls.Add(this.SrcBn);
			base.Controls.Add(this.SeqBn);
			base.Controls.Add(this.SettingBn);
			base.Controls.Add(this.ParamBn);
			base.Controls.Add(this.cbLanguage);
			base.Controls.Add(this.panelChildForm);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.CtrlBnT);
			base.Controls.Add(this.ParamBnT);
			base.Controls.Add(this.SeqBnT);
			base.Controls.Add(this.SrcBnT);
			base.Controls.Add(this.ToolBnT);
			base.Controls.Add(this.lab_HanderTitle);
			base.Name = "Form_001Main";
			base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form_001Main_FormClosed);
			base.Load += new System.EventHandler(Form_001Main_Load);
			base.Scroll += new System.Windows.Forms.ScrollEventHandler(Form_001Main_Scroll);
			base.SizeChanged += new System.EventHandler(Form_001Main_SizeChanged);
			((System.ComponentModel.ISupportInitialize)this.UserBn).EndInit();
			((System.ComponentModel.ISupportInitialize)this.OnOfflinePB).EndInit();
			base.ResumeLayout(false);
		}
	}
}
