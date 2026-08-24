using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form592_ExportImport : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		public DataTable dt_ExportImport = new DataTable();

		private Image[] SWImg = new Image[2];

		private Image[] StatusImg = new Image[4];

		private bool ExportEventFlag = false;

		private bool ImportEventFlag = false;

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private Label lab_Param;

		private Label lab_Seq;

		private Label lab_Source;

		private Label lab_Ctrl;

		private Label lab_Tool;

		private Label lab_Report;

		private Label lab_Export;

		private Label lab_Import;

		private DataGridView dataGridView_ExportImport;

		private Button ExportBn;

		private Button ImportBn;

		public Form592_ExportImport(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			MultiLanguage.LoadLanguage(this);
			dataGridView_ExportImport.MouseClick += dataGridView_ExportImport_MouseClick;
			dataGridView_ExportImport.MouseDoubleClick += dataGridView_ExportImport_MouseClick;
			SWImg[0] = Resources.UnTick;
			SWImg[1] = Resources.Tick;
			StatusImg[0] = Resources.ExportNone;
			StatusImg[1] = Resources.ExportWait;
			StatusImg[2] = Resources.ExportOK;
			StatusImg[3] = Resources.ExportNG;
			dt_ExportImport.Columns.Add("ExportOption", typeof(Image));
			dt_ExportImport.Columns.Add("ExportStatus", typeof(Image));
			dt_ExportImport.Columns.Add("ImportOption", typeof(Image));
			dt_ExportImport.Columns.Add("ImportStatus", typeof(Image));
			dt_ExportImport.Rows.Clear();
			for (int i = 0; i < 6; i++)
			{
				DataRow UserRow = dt_ExportImport.NewRow();
				UserRow[0] = SWImg[1];
				UserRow[1] = StatusImg[0];
				if (i < 5)
				{
					UserRow[2] = SWImg[1];
					UserRow[3] = StatusImg[0];
				}
				else
				{
					UserRow[2] = StatusImg[0];
					UserRow[3] = StatusImg[0];
				}
				dt_ExportImport.Rows.Add(UserRow);
			}
			dataGridView_ExportImport.DataSource = dt_ExportImport;
			loadGrid1(dataGridView_ExportImport);
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.RowTemplate.Height = (int)(52f * FormControlZoom.ScreenHeightZoom);
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.Columns[0].HeaderText = "Option";
			dataGridView1.Columns[1].HeaderText = "Status";
			dataGridView1.Columns[2].HeaderText = "Option";
			dataGridView1.Columns[3].HeaderText = "Status";
			for (int i = 0; i < 4; i++)
			{
				((DataGridViewImageColumn)dataGridView1.Columns[i]).ImageLayout = DataGridViewImageCellLayout.Zoom;
			}
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form592_ExportImport_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void dataGridView_ExportImport_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = dataGridView_ExportImport.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = dataGridView_ExportImport.HitTest(e.X, e.Y).ColumnIndex;
			if (currentMouseOverRow >= 0 && currentMouseOverCol >= 0)
			{
				DataRow dr = dt_ExportImport.Rows[currentMouseOverRow];
				if (dataGridView_ExportImport.Columns[currentMouseOverCol].Name == "ExportOption")
				{
					dr["ExportOption"] = ((dr["ExportOption"] == SWImg[1]) ? SWImg[0] : SWImg[1]);
					dr["ExportStatus"] = StatusImg[0];
				}
				else if (dataGridView_ExportImport.Columns[currentMouseOverCol].Name == "ImportOption" && currentMouseOverRow < 5)
				{
					dr["ImportOption"] = ((dr["ImportOption"] == SWImg[1]) ? SWImg[0] : SWImg[1]);
					dr["ImportStatus"] = StatusImg[0];
				}
			}
		}

		private void ExportBn_Click(object sender, EventArgs e)
		{
			ExportEventFlag = true;
			ImportEventFlag = false;
			if (GB.Form592Event != null && GB.Form592ThreadWait)
			{
				GB.Form592Event.Set();
				GB.Form592ThreadWait = false;
			}
		}

		private void ImportBn_Click(object sender, EventArgs e)
		{
			ExportEventFlag = false;
			ImportEventFlag = true;
			if (GB.Form592Event != null && GB.Form592ThreadWait)
			{
				GB.Form592Event.Set();
				GB.Form592ThreadWait = false;
			}
		}

		private void Form592_ExportImport_Load(object sender, EventArgs e)
		{
			GB.Form592Event = new AutoResetEvent(false);
			GB.Form592ThreadFlag = true;
			ThreadStart MissionForm592 = Form592Thread;
			GB.MissionForm592Thread = new Thread(MissionForm592);
			GB.MissionForm592Thread.Start();
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		public void Form592Thread()
		{
			while (GB.Form592ThreadFlag)
			{
				if (GB.Form592Event != null)
				{
					GB.Form592ThreadWait = true;
					GB.Form592Event.WaitOne();
					if (!GB.Form592ThreadFlag)
					{
						break;
					}
				}
				if (!base.IsHandleCreated)
				{
					continue;
				}
				Invoke((Action)delegate
				{
					if (ExportEventFlag)
					{
						ExportEventFlag = false;
						bool flag = false;
						bool flag2 = false;
						string text = "InfoAll";
						GB.ALNGMsgStartStopFunction(false);
						for (int i = 0; i < dataGridView_ExportImport.Rows.Count; i++)
						{
							dt_ExportImport.Rows[i]["ExportStatus"] = StatusImg[0];
							if (dt_ExportImport.Rows[i]["ExportOption"] == SWImg[1])
							{
								switch (i)
								{
								case 0:
									dt_ExportImport.Rows[i]["ExportStatus"] = StatusImg[1];
									flag2 = TrCSV.WriteParamFile(0u, text, 1, flag);
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										flag2 = TrCSV.WriteParamFile(1u, text, 1, flag);
									}
									dt_ExportImport.Rows[i]["ExportStatus"] = (flag2 ? StatusImg[2] : StatusImg[3]);
									break;
								case 1:
									dt_ExportImport.Rows[i]["ExportStatus"] = StatusImg[1];
									flag2 = TrCSV.WriteSeqFile(text, flag);
									flag2 = TrCSV.WriteSeqGuideFile(text, flag);
									flag2 = TrCSV.WriteSeqPictureFile(text, flag);
									flag2 = TrCSV.WriteSeqArmFile(text, flag);
									flag2 = TrCSV.WriteSeqImageFile(text, flag);
									dt_ExportImport.Rows[i]["ExportStatus"] = (flag2 ? StatusImg[2] : StatusImg[3]);
									break;
								case 2:
									dt_ExportImport.Rows[i]["ExportStatus"] = StatusImg[1];
									if (GB.UISys.IsReadSupportFTPClient)
									{
										TCP.FSIDRead_ByFTP(30);
									}
									else
									{
										TCP.FSIDRead_ByFTP(31, 0u, 0u, 0);
										TCP.FSIDRead_ByFTP(32, 0u, 0u, 0);
										TCP.FSIDRead_ByFTP(33, 0u, 0u, 0);
									}
									flag2 = TrCSV.WriteSrcFile(0, 0, 0, text, flag);
									flag2 = TrCSV.WriteSrcFile(0, 0, 1, text, flag);
									flag2 = TrCSV.WriteSrcFile(0, 0, 2, text, flag);
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										if (GB.UISys.IsReadSupportFTPClient)
										{
											TCP.FSIDRead_ByFTP(35);
										}
										else
										{
											TCP.FSIDRead_ByFTP(36, 0u, 0u, 0);
											TCP.FSIDRead_ByFTP(37, 0u, 0u, 0);
											TCP.FSIDRead_ByFTP(38, 0u, 0u, 0);
										}
										flag2 = TrCSV.WriteSrcFile(1, 0, 0, text, flag);
										flag2 = TrCSV.WriteSrcFile(1, 0, 1, text, flag);
										flag2 = TrCSV.WriteSrcFile(1, 0, 2, text, flag);
										if (GB.UISys.IsReadSupportFTPClient)
										{
											TCP.FSIDRead_ByFTP(40);
										}
										else
										{
											TCP.FSIDRead_ByFTP(41, 0u, 0u, 0);
											TCP.FSIDRead_ByFTP(43, 0u, 0u, 0);
										}
										flag2 = TrCSV.WriteSrcFile(0, 1, 0, text, flag);
										flag2 = TrCSV.WriteSrcFile(0, 1, 2, text, flag);
										if (GB.UISys.IsReadSupportFTPClient)
										{
											TCP.FSIDRead_ByFTP(50);
										}
										else
										{
											TCP.FSIDRead_ByFTP(51, 0u, 0u, 0);
											TCP.FSIDRead_ByFTP(52, 0u, 0u, 0);
											TCP.FSIDRead_ByFTP(53, 0u, 0u, 0);
										}
										flag2 = TrCSV.WriteSrcFile(0, 2, 0, text, flag);
										flag2 = TrCSV.WriteSrcFile(0, 2, 1, text, flag);
										flag2 = TrCSV.WriteSrcFile(0, 2, 2, text, flag);
									}
									TCP.FSIDRead_ByTCP(350, 0, 0, 0, 0, 0);
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										TCP.FSIDRead_ByTCP(350, 0, 1, 0, 0, 0);
									}
									TrCSV.WriteSrcModeFile(text, flag);
									dt_ExportImport.Rows[i]["ExportStatus"] = (flag2 ? StatusImg[2] : StatusImg[3]);
									break;
								case 3:
									dt_ExportImport.Rows[i]["ExportStatus"] = StatusImg[1];
									TrCSV.CtrlSystemAllDataReadFromCtrl();
									flag2 = TrCSV.WriteCtrlSystemFile(text, flag);
									TrCSV.CtrlDIOAllDataReadFromCtrl(0);
									flag2 = TrCSV.WriteCtrlDIOFile(0, text, flag);
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										TrCSV.CtrlDIOAllDataReadFromCtrl(1);
										flag2 = TrCSV.WriteCtrlDIOFile(1, text, flag);
									}
									TrCSV.CtrlTableAllDataReadFromCtrl(0, 99);
									flag2 = TrCSV.WriteCtrlTableFile(0, text, flag);
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										TrCSV.CtrlTableAllDataReadFromCtrl(1, 99);
										flag2 = TrCSV.WriteCtrlTableFile(1, text, flag);
									}
									TrCSV.CtrlPortAllDataReadFromCtrl();
									flag2 = TrCSV.WriteCtrlPortFile(text, flag);
									TrCSV.CtrlCommunicationAllDataReadFromCtrl();
									flag2 = TrCSV.WriteCtrlCommunicationFile(text, flag);
									dt_ExportImport.Rows[i]["ExportStatus"] = (flag2 ? StatusImg[2] : StatusImg[3]);
									break;
								case 4:
									dt_ExportImport.Rows[i]["ExportStatus"] = StatusImg[1];
									flag2 = TrCSV.WriteToolSystemFile(0, text, flag);
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										flag2 = TrCSV.WriteToolSystemFile(1, text, flag);
									}
									flag2 = TrCSV.WriteToolSensitivityFile(0, text, flag);
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										flag2 = TrCSV.WriteToolSensitivityFile(1, text, flag);
									}
									dt_ExportImport.Rows[i]["ExportStatus"] = (flag2 ? StatusImg[2] : StatusImg[3]);
									break;
								case 5:
								{
									dt_ExportImport.Rows[i]["ExportStatus"] = StatusImg[1];
									uint num = (uint)(GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_H_08 * 65536 + GB.TcpStatus.Detail.Comm.CurrentNoOfProductionReportEntries_L_07);
									if (num + 1 >= 200000)
									{
										flag2 = false;
										num = 0u;
									}
									else
									{
										TCP.FSIDRead_ByFTP(81, num, num + 1, 0);
										if (GB.FSReportStatus[num] > 0)
										{
											num = 200000u;
										}
									}
									if (num != 0)
									{
										if (GB.UISys.IsReadSupportFTPClient)
										{
											TrCSV.GetSNReportBinFile(true);
										}
										else
										{
											int message = (flag ? 1 : 0);
											TCP.FSIDRead_ByFTP(70, 0u, num, message);
											TCP.FSIDRead_ByFTP(80, 0u, num, message);
										}
										flag2 = TrCSV.WriteReportInfoFile(num, text, 0u, flag);
									}
									dt_ExportImport.Rows[i]["ExportStatus"] = (flag2 ? StatusImg[2] : StatusImg[3]);
									break;
								}
								}
							}
						}
						GB.ALNGMsgStartStopFunction(true);
					}
					if (ImportEventFlag)
					{
						ImportEventFlag = false;
						bool jumpMsg = false;
						bool flag3 = false;
						int num2 = 0;
						string text2 = "./ScrewInfo/InfoAll";
						string text3 = "";
						GB.ALNGMsgStartStopFunction(false);
						for (int j = 0; j < dataGridView_ExportImport.Rows.Count; j++)
						{
							dt_ExportImport.Rows[j]["ImportStatus"] = StatusImg[0];
							if (dt_ExportImport.Rows[j]["ImportOption"] == SWImg[1])
							{
								switch (j)
								{
								case 0:
									dt_ExportImport.Rows[j]["ImportStatus"] = StatusImg[1];
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? ("/Parm/Tool" + 1 + "Parm.csv") : "/Parm/ToolParm010.csv");
									flag3 = TrCSV.ReadParamFile(0, text2 + text3);
									if (flag3)
									{
										for (int l = 0; l < 500; l++)
										{
											GB.ParamChooseIconX[l] = 0;
										}
										TrCSV.ParamAllDataWriteToCtrl(0, jumpMsg);
									}
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										text3 = "/Parm/Tool" + 2 + "Parm.csv";
										flag3 = TrCSV.ReadParamFile(1, text2 + text3);
									}
									if (flag3)
									{
										for (int m = 0; m < 500; m++)
										{
											GB.ParamChooseIconY[m] = 0;
										}
										TrCSV.ParamAllDataWriteToCtrl(1, jumpMsg);
									}
									dt_ExportImport.Rows[j]["ImportStatus"] = (flag3 ? StatusImg[2] : StatusImg[3]);
									break;
								case 1:
								{
									dt_ExportImport.Rows[j]["ImportStatus"] = StatusImg[1];
									for (int k = 0; k < 500; k++)
									{
										GB.SeqChooseIcon[k] = 0;
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Seq/SeqItem.csv" : "/Seq/SeqItem010.csv");
									flag3 = TrCSV.ReadSeqFile(text2 + text3);
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Seq/SeqGuide.csv" : "/Seq/SeqGuide010.csv");
									flag3 = TrCSV.ReadSeqGuideFile(text2 + text3);
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Seq/SeqPicture.csv" : "/Seq/SeqPicture010.csv");
									flag3 = TrCSV.ReadSeqPictureFile(text2 + text3);
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Seq/SeqArm.csv" : "/Seq/SeqArm010.csv");
									flag3 = TrCSV.ReadSeqArmFile(text2 + text3);
									text3 = "/Seq";
									flag3 = TrCSV.ReadSeqImageFile(text2 + text3);
									if (flag3)
									{
										TrCSV.SeqAllDataWriteToCtrl(jumpMsg);
									}
									dt_ExportImport.Rows[j]["ImportStatus"] = (flag3 ? StatusImg[2] : StatusImg[3]);
									break;
								}
								case 2:
									dt_ExportImport.Rows[j]["ImportStatus"] = StatusImg[1];
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Src/SrcActionMode.csv" : "/Src/SrcActionMode010.csv");
									if (TrCSV.ReadSrcModeFile(text2 + text3))
									{
										TrCSV.SrcActionModeWriteToCtrl(GB.UISys.RunningSrcMode.ActionMode, GB.UISys.RunningSrcMode.SwitchingMethodX, GB.UISys.RunningSrcMode.SwitchingMethodY, jumpMsg);
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? ("/Src/Tool" + 1 + "Handle_S.csv") : "/Src/ToolHandle010_S.csv");
									if (TrCSV.ReadSrcFile(0, 0, 0, text2 + text3))
									{
										TrCSV.SrcAllDataWriteToCtrl(0, 0, 0, jumpMsg);
										num2++;
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? ("/Src/Tool" + 1 + "Bits_S.csv") : "/Src/ToolBits010_S.csv");
									if (TrCSV.ReadSrcFile(0, 0, 1, text2 + text3))
									{
										TrCSV.SrcAllDataWriteToCtrl(0, 0, 1, jumpMsg);
										num2++;
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? ("/Src/Tool" + 1 + "Scan_S.csv") : "/Src/ToolScan010_S.csv");
									flag3 = TrCSV.ReadSrcFile(0, 0, 2, text2 + text3);
									if (flag3)
									{
										TrCSV.SrcAllDataWriteToCtrl(0, 0, 2, jumpMsg);
										num2++;
									}
									if (num2 > 0)
									{
										num2 = 0;
										if (!GB.CheckSrcOverRange(GB.TcpStatus.Detail.Comm.Tool1SwitchingMethod_21, GB.TcpStatus.Detail.T1StA.TighteningIDset_00))
										{
											TCP.FSIDWrite_ByTCP(301, 0, 0, GB.TcpStatus.Detail.T1StA.TighteningIDset_00, GB.TcpStatus.Detail.Comm.OperationMode_20, GB.TcpStatus.Detail.Comm.Tool1SwitchingMethod_21);
										}
									}
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										text3 = "/Src/Tool" + 2 + "Handle_S.csv";
										if (TrCSV.ReadSrcFile(1, 0, 0, text2 + text3))
										{
											TrCSV.SrcAllDataWriteToCtrl(1, 0, 0, jumpMsg);
											num2++;
										}
										text3 = "/Src/Tool" + 2 + "Bits_S.csv";
										if (TrCSV.ReadSrcFile(1, 0, 1, text2 + text3))
										{
											TrCSV.SrcAllDataWriteToCtrl(1, 0, 1, jumpMsg);
											num2++;
										}
										text3 = "/Src/Tool" + 2 + "Scan_S.csv";
										if (TrCSV.ReadSrcFile(1, 0, 2, text2 + text3))
										{
											TrCSV.SrcAllDataWriteToCtrl(1, 0, 2, jumpMsg);
											num2++;
										}
										text3 = "/Src/ToolHandle_M.csv";
										if (TrCSV.ReadSrcFile(0, 1, 0, text2 + text3))
										{
											TrCSV.SrcAllDataWriteToCtrl(0, 1, 0, jumpMsg);
											num2++;
										}
										text3 = "/Src/ToolScan_M.csv";
										if (TrCSV.ReadSrcFile(0, 1, 2, text2 + text3))
										{
											TrCSV.SrcAllDataWriteToCtrl(0, 1, 2, jumpMsg);
											num2++;
										}
										text3 = "/Src/ToolHandle_C.csv";
										if (TrCSV.ReadSrcFile(0, 2, 0, text2 + text3))
										{
											TrCSV.SrcAllDataWriteToCtrl(0, 2, 0, jumpMsg);
											num2++;
										}
										text3 = "/Src/ToolBits_C.csv";
										if (TrCSV.ReadSrcFile(0, 2, 1, text2 + text3))
										{
											TrCSV.SrcAllDataWriteToCtrl(0, 2, 1, jumpMsg);
											num2++;
										}
										text3 = "/Src/ToolScan_C.csv";
										flag3 = TrCSV.ReadSrcFile(0, 2, 2, text2 + text3);
										if (flag3)
										{
											TrCSV.SrcAllDataWriteToCtrl(0, 2, 2, jumpMsg);
											num2++;
										}
										if (num2 > 0)
										{
											num2 = 0;
											if (!GB.CheckSrcOverRange(GB.TcpStatus.Detail.Comm.Tool2SwitchingMethod_22, GB.TcpStatus.Detail.T2StA.TighteningIDset_00))
											{
												TCP.FSIDWrite_ByTCP(301, 0, 1, GB.TcpStatus.Detail.T2StA.TighteningIDset_00, GB.TcpStatus.Detail.Comm.OperationMode_20, GB.TcpStatus.Detail.Comm.Tool2SwitchingMethod_22);
											}
										}
									}
									dt_ExportImport.Rows[j]["ImportStatus"] = (flag3 ? StatusImg[2] : StatusImg[3]);
									break;
								case 3:
									dt_ExportImport.Rows[j]["ImportStatus"] = StatusImg[1];
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Ctrl/CtrlSystem.csv" : "/Ctrl/CtrlSystem010.csv");
									if (TrCSV.ReadCtrlSystemFile(text2 + text3))
									{
										TrCSV.CtrlSystemAllDataWriteToCtrl(jumpMsg);
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? ("/Ctrl/Ctrl" + 1 + "DIO.csv") : "/Ctrl/CtrlDIO010.csv");
									if (TrCSV.ReadCtrlDIOFile(0, text2 + text3))
									{
										TrCSV.CtrlDIOAllDataWriteToCtrl(0, jumpMsg);
									}
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										text3 = "/Ctrl/Ctrl" + 2 + "DIO.csv";
										if (TrCSV.ReadCtrlDIOFile(1, text2 + text3))
										{
											TrCSV.CtrlDIOAllDataWriteToCtrl(1, jumpMsg);
										}
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? ("/Ctrl/Ctrl" + 1 + "Table.csv") : "/Ctrl/CtrlTable010.csv");
									if (TrCSV.ReadCtrlTableFile(0, text2 + text3))
									{
										TrCSV.CtrlTableAllDataWriteToCtrl(0, jumpMsg);
									}
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										text3 = "/Ctrl/Ctrl" + 2 + "Table.csv";
										if (TrCSV.ReadCtrlTableFile(1, text2 + text3))
										{
											TrCSV.CtrlTableAllDataWriteToCtrl(1, jumpMsg);
										}
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Ctrl/CtrlPort.csv" : "/Ctrl/CtrlPort010.csv");
									if (TrCSV.ReadCtrlPortFile(text2 + text3))
									{
										TrCSV.CtrlPortAllDataWriteToCtrl(jumpMsg);
									}
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? "/Ctrl/CtrlCommunication.csv" : "/Ctrl/CtrlCommunication010.csv");
									flag3 = TrCSV.ReadCtrlCommunicationFile(text2 + text3);
									if (flag3)
									{
										TrCSV.CtrlCommunicationAllDataWriteToCtrl(jumpMsg);
									}
									dt_ExportImport.Rows[j]["ImportStatus"] = (flag3 ? StatusImg[2] : StatusImg[3]);
									break;
								case 4:
									dt_ExportImport.Rows[j]["ImportStatus"] = StatusImg[1];
									text3 = ((GB.FSModelTypeInfo.MesModelType == 0) ? ("/Tool/Tool" + 1 + "System.csv") : "/Tool/System010.csv");
									flag3 = TrCSV.ReadToolSystemFile(0, text2 + text3);
									if (flag3)
									{
										TrCSV.ToolAllDataWriteToCtrl(0, jumpMsg);
									}
									if (GB.FSModelTypeInfo.MesModelType == 0 && GB.UISys.CtrlDualTool == 1)
									{
										text3 = "/Tool/Tool" + 2 + "System.csv";
										flag3 = TrCSV.ReadToolSystemFile(1, text2 + text3);
										if (flag3)
										{
											TrCSV.ToolAllDataWriteToCtrl(1, jumpMsg);
										}
									}
									dt_ExportImport.Rows[j]["ImportStatus"] = (flag3 ? StatusImg[2] : StatusImg[3]);
									break;
								}
							}
						}
						GB.ALNGMsgStartStopFunction(true);
					}
				});
			}
		}

		private void Form592_ExportImport_FormClosed(object sender, FormClosedEventArgs e)
		{
			GB.Form592ThreadFlag = false;
			if (GB.MissionForm592Thread != null)
			{
				GB.MissionForm592Thread.Abort();
			}
			if (GB.Form592Event != null)
			{
				if (GB.Form592ThreadWait)
				{
					GB.Form592Event.Set();
					GB.Form592ThreadWait = false;
				}
				GB.Form592Event.Close();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form592_ExportImport));
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.lab_Param = new System.Windows.Forms.Label();
			this.lab_Seq = new System.Windows.Forms.Label();
			this.lab_Source = new System.Windows.Forms.Label();
			this.lab_Ctrl = new System.Windows.Forms.Label();
			this.lab_Tool = new System.Windows.Forms.Label();
			this.lab_Report = new System.Windows.Forms.Label();
			this.lab_Export = new System.Windows.Forms.Label();
			this.lab_Import = new System.Windows.Forms.Label();
			this.dataGridView_ExportImport = new System.Windows.Forms.DataGridView();
			this.ExportBn = new System.Windows.Forms.Button();
			this.ImportBn = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ExportImport).BeginInit();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, -1);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(500, 35);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.Text = "Title";
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
			this.lab_Param.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Param.Location = new System.Drawing.Point(21, 115);
			this.lab_Param.Name = "lab_Param";
			this.lab_Param.Size = new System.Drawing.Size(120, 30);
			this.lab_Param.TabIndex = 131;
			this.lab_Param.Text = "Parameter";
			this.lab_Param.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Seq.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Seq.Location = new System.Drawing.Point(21, 167);
			this.lab_Seq.Name = "lab_Seq";
			this.lab_Seq.Size = new System.Drawing.Size(120, 30);
			this.lab_Seq.TabIndex = 131;
			this.lab_Seq.Text = "Sequence";
			this.lab_Seq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Source.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Source.Location = new System.Drawing.Point(21, 220);
			this.lab_Source.Name = "lab_Source";
			this.lab_Source.Size = new System.Drawing.Size(120, 30);
			this.lab_Source.TabIndex = 131;
			this.lab_Source.Text = "Sources";
			this.lab_Source.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Ctrl.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Ctrl.Location = new System.Drawing.Point(21, 272);
			this.lab_Ctrl.Name = "lab_Ctrl";
			this.lab_Ctrl.Size = new System.Drawing.Size(120, 30);
			this.lab_Ctrl.TabIndex = 131;
			this.lab_Ctrl.Text = "Controller";
			this.lab_Ctrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Tool.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Tool.Location = new System.Drawing.Point(21, 325);
			this.lab_Tool.Name = "lab_Tool";
			this.lab_Tool.Size = new System.Drawing.Size(120, 30);
			this.lab_Tool.TabIndex = 131;
			this.lab_Tool.Text = "Tool";
			this.lab_Tool.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Report.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Report.Location = new System.Drawing.Point(4, 378);
			this.lab_Report.Name = "lab_Report";
			this.lab_Report.Size = new System.Drawing.Size(137, 49);
			this.lab_Report.TabIndex = 131;
			this.lab_Report.Text = "Production Report";
			this.lab_Report.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Export.AutoSize = true;
			this.lab_Export.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Export.Location = new System.Drawing.Point(197, 55);
			this.lab_Export.Name = "lab_Export";
			this.lab_Export.Size = new System.Drawing.Size(58, 20);
			this.lab_Export.TabIndex = 131;
			this.lab_Export.Text = "Export";
			this.lab_Import.AutoSize = true;
			this.lab_Import.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Import.Location = new System.Drawing.Point(373, 55);
			this.lab_Import.Name = "lab_Import";
			this.lab_Import.Size = new System.Drawing.Size(59, 20);
			this.lab_Import.TabIndex = 131;
			this.lab_Import.Text = "Import";
			this.dataGridView_ExportImport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_ExportImport.Location = new System.Drawing.Point(156, 91);
			this.dataGridView_ExportImport.Name = "dataGridView_ExportImport";
			this.dataGridView_ExportImport.RowHeadersWidth = 51;
			this.dataGridView_ExportImport.RowTemplate.Height = 24;
			this.dataGridView_ExportImport.Size = new System.Drawing.Size(325, 362);
			this.dataGridView_ExportImport.TabIndex = 132;
			this.ExportBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ExportBn.BackgroundImage");
			this.ExportBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ExportBn.FlatAppearance.BorderSize = 0;
			this.ExportBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ExportBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ExportBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ExportBn.Location = new System.Drawing.Point(188, 469);
			this.ExportBn.Name = "ExportBn";
			this.ExportBn.Size = new System.Drawing.Size(92, 30);
			this.ExportBn.TabIndex = 134;
			this.ExportBn.Text = "Run";
			this.ExportBn.UseVisualStyleBackColor = true;
			this.ExportBn.Click += new System.EventHandler(ExportBn_Click);
			this.ImportBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ImportBn.BackgroundImage");
			this.ImportBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ImportBn.FlatAppearance.BorderSize = 0;
			this.ImportBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ImportBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ImportBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ImportBn.Location = new System.Drawing.Point(350, 469);
			this.ImportBn.Name = "ImportBn";
			this.ImportBn.Size = new System.Drawing.Size(92, 30);
			this.ImportBn.TabIndex = 134;
			this.ImportBn.Text = "Run";
			this.ImportBn.UseVisualStyleBackColor = true;
			this.ImportBn.Click += new System.EventHandler(ImportBn_Click);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 522);
			base.Controls.Add(this.ImportBn);
			base.Controls.Add(this.ExportBn);
			base.Controls.Add(this.dataGridView_ExportImport);
			base.Controls.Add(this.lab_Report);
			base.Controls.Add(this.lab_Tool);
			base.Controls.Add(this.lab_Ctrl);
			base.Controls.Add(this.lab_Source);
			base.Controls.Add(this.lab_Seq);
			base.Controls.Add(this.lab_Import);
			base.Controls.Add(this.lab_Export);
			base.Controls.Add(this.lab_Param);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form592_ExportImport";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(Form592_ExportImport_FormClosed);
			base.Load += new System.EventHandler(Form592_ExportImport_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form592_ExportImport_Paint);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_ExportImport).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
