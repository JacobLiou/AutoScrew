using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class GlobalVar
	{
		public UIVar UISys = new UIVar();

		public static OpenFileDialog dialog = new OpenFileDialog();

		public TcpHandShake_Union TcpWR = default(TcpHandShake_Union);

		public TcpHandShake_Union TcpRD = default(TcpHandShake_Union);

		public TcpStatus_Union TcpStatus = default(TcpStatus_Union);

		public bool Form001TCPWait = false;

		public bool Form001TCPFlag = true;

		public Thread MissionForm001TCPThread;

		public AutoResetEvent Form001TCPEvent;

		public bool ReflashThreadWait = false;

		public bool ReflashThreadFlag = true;

		public Thread MissionReflashThread;

		public AutoResetEvent ReflashEvent;

		public bool Form100ThreadWait = false;

		public bool Form100ThreadFlag = true;

		public Thread MissionForm100Thread;

		public AutoResetEvent Form100Event;

		public bool Form200ThreadWait = false;

		public bool Form200ThreadFlag = true;

		public Thread MissionForm200Thread;

		public AutoResetEvent Form200Event;

		public bool Form300ThreadWait = false;

		public bool Form300ThreadFlag = true;

		public Thread MissionForm300Thread;

		public AutoResetEvent Form300Event;

		public bool Form400ThreadWait = false;

		public bool Form400ThreadFlag = true;

		public Thread MissionForm400Thread;

		public AutoResetEvent Form400Event;

		public bool Form409ThreadWait = false;

		public bool Form409ThreadFlag = true;

		public Thread MissionForm409Thread;

		public AutoResetEvent Form409Event;

		public bool Form500ThreadWait = false;

		public bool Form500ThreadFlag = true;

		public Thread MissionForm500Thread;

		public AutoResetEvent Form500Event;

		public bool Form592ThreadWait = false;

		public bool Form592ThreadFlag = true;

		public Thread MissionForm592Thread;

		public AutoResetEvent Form592Event;

		public bool Form600ThreadWait = false;

		public bool Form600ThreadFlag = true;

		public Thread MissionForm600Thread;

		public AutoResetEvent Form600Event;

		public bool Form700ThreadWait = false;

		public bool Form700ThreadFlag = true;

		public Thread MissionForm700Thread;

		public AutoResetEvent Form700Event;

		public bool BackGroundThreadWait = false;

		public bool BackGroundThreadFlag = true;

		public Thread MissionBackGroundThread;

		public AutoResetEvent BackGroundEvent;

		public bool FTPServerFlag = true;

		public TcpListener FTPServerListener;

		public Thread MissionFTPServerListenerThread;

		public System.Windows.Forms.Timer GetPositionArmTimer;

		public System.Windows.Forms.Timer GetCommunTimer;

		public System.Windows.Forms.Timer GetLevelTimer;

		public System.Windows.Forms.Timer ALNGMsgTimer;

		public bool StartTimerWithSleepFlag = false;

		public bool StartTimerWithSleepWinformLive = true;

		public bool TCPHandshakeWait = false;

		public AutoResetEvent TCPHandshakeEvent = new AutoResetEvent(false);

		public ParamStucVer1[] FSParamX = new ParamStucVer1[500];

		public ParamStucVer1[] FSParamY = new ParamStucVer1[500];

		public ushort[] FSParamIDUsed = new ushort[500];

		public ParamStucVer1 SendReadParamStucVer1 = default(ParamStucVer1);

		public ParamStucVer0 SendReadParamStucVer0 = default(ParamStucVer0);

		public SeqBaseStuc SendReadSeqStucVer0 = default(SeqBaseStuc);

		public SeqBaseStuc[] FSSeqGB = new SeqBaseStuc[500];

		public ushort[] FSSeqIDUsed = new ushort[500];

		public uint SeqPicFileByteLen = 0u;

		public uint SeqRemainSpaceSize = 0u;

		public ushort[] FSPicBitMap = new ushort[2000];

		public SeqNavigationPictureStuc[] FSSeqPicABC = new SeqNavigationPictureStuc[500];

		public SeqNavigationCoordinateXY[] FSSeqLedXY = new SeqNavigationCoordinateXY[500];

		public SeqArmPositionXYZ[] FSSeqArmXYZ = new SeqArmPositionXYZ[500];

		public SrcAll FSSrcAll = default(SrcAll);

		public ExParamStuc ExFSParamX = default(ExParamStuc);

		public ExParamStuc ExFSParamY = default(ExParamStuc);

		public ExSeqStuc ExFSSeq = default(ExSeqStuc);

		public SrcMode FSSrcMode = default(SrcMode);

		public ResultBarcodeStuc FSResultBarcodeX = default(ResultBarcodeStuc);

		public ResultBarcodeStuc FSResultBarcodeY = default(ResultBarcodeStuc);

		public ResultBarcodeAdvanceSettingStuc FSResultBarcodeAdvanceSettingX = default(ResultBarcodeAdvanceSettingStuc);

		public ResultBarcodeAdvanceSettingStuc FSResultBarcodeAdvanceSettingY = default(ResultBarcodeAdvanceSettingStuc);

		public ResultPerpendicularityStuc FSResultPerpendicularityX = default(ResultPerpendicularityStuc);

		public ResultPerpendicularityStuc FSResultPerpendicularityY = default(ResultPerpendicularityStuc);

		public ResultLedStatusStuc FSResultLedStatusX = default(ResultLedStatusStuc);

		public ResultLedStatusStuc FSResultLedStatusY = default(ResultLedStatusStuc);

		public MesValueStuc FSMesValue = default(MesValueStuc);

		public CtrlUserLogInStuc FSCtrlUserLogIn = default(CtrlUserLogInStuc);

		public CtrlEthernetStuc FSCtrlEthernet = default(CtrlEthernetStuc);

		public CtrlUserPasswordStuc FSCtrlUserPassword = default(CtrlUserPasswordStuc);

		public CtrlPageAuthorityStuc FSCtrlPageAuthority = default(CtrlPageAuthorityStuc);

		public CtrlVersionStuc FSCtrlVersion = default(CtrlVersionStuc);

		public CtrlDIOFunctionStuc FSCtrlDIOFunction_X = default(CtrlDIOFunctionStuc);

		public CtrlDIOFunctionStuc FSCtrlDIOFunction_Y = default(CtrlDIOFunctionStuc);

		public CtrlDOTimerStuc FSCtrlDOTimer_X = default(CtrlDOTimerStuc);

		public CtrlDOTimerStuc FSCtrlDOTimer_Y = default(CtrlDOTimerStuc);

		public CtrlDIOTableStuc FSCtrlDIBitsTable_X = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDIBitsTable_Y = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOBitsTable_X = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOBitsTable_Y = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOParamTable_X = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOParamTable_Y = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOScrewTable_X = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOScrewTable_Y = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOSeqTable_X = default(CtrlDIOTableStuc);

		public CtrlDIOTableStuc FSCtrlDOSeqTable_Y = default(CtrlDIOTableStuc);

		public CtrlTorqUnitStuc FSCtrlTorqUnit = default(CtrlTorqUnitStuc);

		public CtrlEarlyWindowStuc FSCtrlEarlyWindow = default(CtrlEarlyWindowStuc);

		public CtrlLocalTableStuc FSCtrlLocalTable = default(CtrlLocalTableStuc);

		public CtrlMappingTableStuc FSCtrlMappingTable = default(CtrlMappingTableStuc);

		public CtrlStaticReadStuc FSCtrlStaticRead = default(CtrlStaticReadStuc);

		public CtrlStartConditionStuc FSCtrlStartCondition = default(CtrlStartConditionStuc);

		public CtrlSingleDIOStuc FSCtrlSingleDIO = default(CtrlSingleDIOStuc);

		public CtrlTwoStageModeStuc FSCtrlTwoStageMode = default(CtrlTwoStageModeStuc);

		public CtrlCurveStageUpLimitStuc FSCtrlCurveStageUpLimit = default(CtrlCurveStageUpLimitStuc);

		public CtrlWarningWindowStuc FSCtrlWarningWindow = default(CtrlWarningWindowStuc);

		public CtrlExportResultFileStuc FSCtrlExportResultFile = default(CtrlExportResultFileStuc);

		public CtrlSamplingRateStuc FSCtrlSamplingRate = default(CtrlSamplingRateStuc);

		public CtrlCurveCutoffPointStuc FSCtrlCurveCutoffPoint = default(CtrlCurveCutoffPointStuc);

		public CtrlDefLoosSpeedStuc FSCtrlDefLoosSpeed = default(CtrlDefLoosSpeedStuc);

		public CtrlKeyboardCursorBlinkingInResultsStuc FSCtrlKeyboardCursorBlinkingInResults = default(CtrlKeyboardCursorBlinkingInResultsStuc);

		public CtrlTorqRateReplaceBySpeedCurveStuc FSCtrlTorqRateReplaceBySpeedCurve = default(CtrlTorqRateReplaceBySpeedCurveStuc);

		public CtrlProhibitOperationNCStuc FSCtrlProhibitOperationNC = default(CtrlProhibitOperationNCStuc);

		public CtrlAlarmClearProhibitToolStuc FSCtrlProhibitToolAlarmClear = default(CtrlAlarmClearProhibitToolStuc);

		public CtrlSpeedLimitStuc FSCtrlSpeedLimit = default(CtrlSpeedLimitStuc);

		public CtrlHealthCheckStuc FSCtrlHealthCheck = default(CtrlHealthCheckStuc);

		public CtrlCurveAllPositiveStuc FSCtrlCurveAllPositive = default(CtrlCurveAllPositiveStuc);

		public CtrlCurveScaleFromZeroStuc FSCtrlCurveScaleFromZero = default(CtrlCurveScaleFromZeroStuc);

		public CtrlCurveCheckMCURangeStuc FSCtrlCurveCheckMCURange = default(CtrlCurveCheckMCURangeStuc);

		public CtrlCurveCheckMCUTempStuc FSCtrlCurveCheckMCUSwitch = default(CtrlCurveCheckMCUTempStuc);

		public CtrlDIResponseFilterTimeStuc FSCtrlDIResponseFilterTime = default(CtrlDIResponseFilterTimeStuc);

		public CtrlMonitorToolCurrentStuc FSCtrlMonitorToolCurrent = default(CtrlMonitorToolCurrentStuc);

		public CtrlCompensationForToolTempStuc FSCtrlCompensationForToolTemp = default(CtrlCompensationForToolTempStuc);

		public CtrlComPortFunctionStuc FSCtrlComPortFunction = default(CtrlComPortFunctionStuc);

		public CtrlSendResultTCPStuc FSCtrlSendResultTCP = default(CtrlSendResultTCPStuc);

		public CtrlParamNotMatchToolSpecStuc FSCtrlParamNotMatchToolSpec = default(CtrlParamNotMatchToolSpecStuc);

		public CtrlLanguageStuc FSCtrlLanguage = default(CtrlLanguageStuc);

		public CtrlAngleUnitStuc FSCtrlAngleUnit = default(CtrlAngleUnitStuc);

		public CtrlBuzzerModeStuc FSCtrlBuzzerMode = default(CtrlBuzzerModeStuc);

		public CtrlBuzzerVolumeStuc FSCtrlBuzzerVolume = default(CtrlBuzzerVolumeStuc);

		public CtrlHDMIShowStuc FSCtrlDisplayHDMI = default(CtrlHDMIShowStuc);

		public CtrlHomeStartPageStuc FSCtrlHomeStartPage = default(CtrlHomeStartPageStuc);

		public CtrlRS485FunctionStuc FSCtrlRS485Function = default(CtrlRS485FunctionStuc);

		public CtrlModelNameStuc FSCtrlModelName = default(CtrlModelNameStuc);

		public CtrlMacStuc FSCtrlMAC = default(CtrlMacStuc);

		public ReportWatchListStuc FSReportWatchList = default(ReportWatchListStuc);

		public ToolActiveStuc FSToolXActive = default(ToolActiveStuc);

		public ToolActiveStuc FSToolYActive = default(ToolActiveStuc);

		public ToolReminderStuc FSToolXReminder = default(ToolReminderStuc);

		public ToolReminderStuc FSToolYReminder = default(ToolReminderStuc);

		public ToolMaxAngleForRotationDetectStuc FSToolXMaxAngForRotationDetect = default(ToolMaxAngleForRotationDetectStuc);

		public ToolMaxAngleForRotationDetectStuc FSToolYMaxAngForRotationDetect = default(ToolMaxAngleForRotationDetectStuc);

		public ToolInfoStuc FSToolXInfo = default(ToolInfoStuc);

		public ToolInfoStuc FSToolYInfo = default(ToolInfoStuc);

		public uint FSToolRemindCnt_DW = 0u;

		public ToolTeachRecordStuc FSToolTeachRecord = default(ToolTeachRecordStuc);

		public ushort FSToolTeachRecordPage = 0;

		public ToolLeverStartLevelStuc FSToolXLeverStartLevel = default(ToolLeverStartLevelStuc);

		public ToolLeverStartLevelStuc FSToolYLeverStartLevel = default(ToolLeverStartLevelStuc);

		public ToolPushStartLevelStuc FSToolXPushStartLevel = default(ToolPushStartLevelStuc);

		public ToolPushStartLevelStuc FSToolYPushStartLevel = default(ToolPushStartLevelStuc);

		public ToolWorkLightStuc FSToolXWorkLight = default(ToolWorkLightStuc);

		public ToolWorkLightStuc FSToolYWorkLight = default(ToolWorkLightStuc);

		public ToolLEDlightStuc FSToolXLedLight = default(ToolLEDlightStuc);

		public ToolLEDlightStuc FSToolYLedLight = default(ToolLEDlightStuc);

		public ToolCalibrationStuc FSToolXCalibration = default(ToolCalibrationStuc);

		public ToolCalibrationStuc FSToolYCalibration = default(ToolCalibrationStuc);

		public ToolCalibrationVer1Stuc FSToolCalibrationVer1 = default(ToolCalibrationVer1Stuc);

		public ToolVersionStuc FSToolXVersion = default(ToolVersionStuc);

		public ToolVersionStuc FSToolYVersion = default(ToolVersionStuc);

		public FSCtrlModelStuc FSCtrlTypeInfo = default(FSCtrlModelStuc);

		public FSModelTypeInfoStuc FSModelTypeInfo = default(FSModelTypeInfoStuc);

		public ToolModelInfoStuc FSToolXModelInfo = default(ToolModelInfoStuc);

		public ToolModelInfoStuc FSToolYModelInfo = default(ToolModelInfoStuc);

		public ToolTempLevelStuc FSToolXTempLevel = default(ToolTempLevelStuc);

		public ToolTempLevelStuc FSToolYTempLevel = default(ToolTempLevelStuc);

		public ToolWorkLightStuc FSToolXLedDelayTmr = default(ToolWorkLightStuc);

		public ToolWorkLightStuc FSToolYLedDelayTmr = default(ToolWorkLightStuc);

		public ushort[] FSReportStatus = new ushort[200000];

		public ExUserIDNameStuc ExFSUser = default(ExUserIDNameStuc);

		public ExReportStuc ExFSReport = default(ExReportStuc);

		public ExFTPIPStuc FSFTPIP = default(ExFTPIPStuc);

		public ushort[] ParamChooseIconX = new ushort[500];

		public ushort[] ParamChooseIconY = new ushort[500];

		public ushort[] SeqChooseIcon = new ushort[500];

		public ushort[] SeqCaheItem = new ushort[500];

		public int[] UI_Marvel = new int[25];

		public string[] PicSignStr = new string[30]
		{
			"A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
			"K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
			"U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD"
		};

		public event CreateSometingSave_Handler CreateSaveSomething;

		public event CreateUIMarvel_Handler CreateUI100;

		public event CreateUIMarvel_Handler CreateUI110;

		public event CreateUIMarvel_Handler CreateUI111;

		public event CreateUIMarvel_Handler CreateUI112;

		public event CreateUIMarvel_Handler CreateUI113;

		public event CreateUIMarvel_Handler CreateUI140;

		public event CreateUIMarvel_Handler CreateUI141;

		public event CreateUIMarvel_Handler CreateUI142;

		public event CreateUIMarvel_Handler CreateUI143;

		public event CreateUIMarvel_Handler CreateUI144;

		public event CreateUIMarvel_Handler CreateUI145;

		public event CreateUIMarvel_Handler CreateUI146;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI100;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI110;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI111;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI112;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI113;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI140;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI141;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI142;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI143;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI144;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI145;

		public event UpdateUIMarvel_Handler OnlyUpdateScreenUI146;

		public GlobalVar()
		{
			FSSrcAll.FSSrcManualX = new SrcStuc[1];
			FSSrcAll.FSSrcBitsX = new SrcStuc[256];
			FSSrcAll.FSSrcScannerX = new SrcStuc[500];
			FSSrcAll.FSSrcManualY = new SrcStuc[1];
			FSSrcAll.FSSrcBitsY = new SrcStuc[256];
			FSSrcAll.FSSrcScannerY = new SrcStuc[500];
			FSSrcAll.FSSrcManual_DualMix = new SrcStuc[1];
			FSSrcAll.FSSrcBits_DualMix = new SrcStuc[256];
			FSSrcAll.FSSrcScanner_DualMix = new SrcStuc[500];
			FSSrcAll.FSSrcManual_DualSync = new SrcStuc[1];
			FSSrcAll.FSSrcBits_DualSync = new SrcStuc[256];
			FSSrcAll.FSSrcScanner_DualSync = new SrcStuc[500];
			ExFSReport.Info = new ReportInfoStuc[200000];
			ExFSReport.Scale = new ReportScaleStuc[200000];
			ExFSReport.CurveTime = new ushort[8000];
			ExFSReport.CurveAngle = new short[8000];
			ExFSReport.CurveTorque = new short[8000];
			ExFSReport.CurveTorqueRate = new short[8000];
			ExFSReport.ReportParam = new ushort[550];
			ExFSReport.AlarmInfo = new AlarmWarningReportInfo[6000];
			ExFSReport.WarningInfo = new AlarmWarningReportInfo[6000];
			ExFSReport.AlarmInfoOnlyAL = new AlarmWarningReportInfo[6000];
			ExFSReport.AlarmInfoOnlyNG = new AlarmWarningReportInfo[6000];
			ExFSReport.ButtonInfo = new ButtonReportInfo[6000];
			UISys.StartYY = (UISys.EndYY = (ushort)DateTime.Today.Year);
			UISys.StartMM = (UISys.EndMM = (ushort)DateTime.Today.Month);
			UISys.StartDD = (UISys.EndDD = (ushort)DateTime.Today.Day);
		}

		public void CloseSometingSaveDelegate()
		{
			this.CreateSaveSomething = null;
		}

		public void CloseMarvelDelegate(bool AllSwitch)
		{
			if (AllSwitch)
			{
				this.CreateUI100 = null;
			}
			this.CreateUI110 = null;
			this.CreateUI111 = null;
			this.CreateUI112 = null;
			this.CreateUI113 = null;
			this.CreateUI140 = null;
			this.CreateUI141 = null;
			this.CreateUI142 = null;
			this.CreateUI143 = null;
			this.CreateUI144 = null;
			this.CreateUI145 = null;
			this.CreateUI146 = null;
		}

		public void CloseOnlyUpdateDelegate(bool AllSwitch)
		{
			if (AllSwitch)
			{
				this.OnlyUpdateScreenUI100 = null;
			}
			this.OnlyUpdateScreenUI110 = null;
			this.OnlyUpdateScreenUI111 = null;
			this.OnlyUpdateScreenUI112 = null;
			this.OnlyUpdateScreenUI113 = null;
			this.OnlyUpdateScreenUI140 = null;
			this.OnlyUpdateScreenUI141 = null;
			this.OnlyUpdateScreenUI142 = null;
			this.OnlyUpdateScreenUI143 = null;
			this.OnlyUpdateScreenUI144 = null;
			this.OnlyUpdateScreenUI145 = null;
			this.OnlyUpdateScreenUI146 = null;
		}

		public void ClearList()
		{
			UISys.List_Time.Clear();
			UISys.List_Angle.Clear();
			UISys.List_Torq.Clear();
			UISys.List_TorqRate.Clear();
			UISys.List_Stage.Clear();
			UISys.List_Param_Unit.Clear();
		}

		public void UIMarvelClear()
		{
			Array.Clear(UI_Marvel, 0, UI_Marvel.Length);
		}

		public bool UIMarvelGetBit(int Mode, int Addr)
		{
			return (UI_Marvel[Mode] & (1 << Addr)) != 0;
		}

		public void UIMarvelSetBit(int Mode, int Addr)
		{
			UI_Marvel[Mode] |= 1 << Addr;
		}

		public void UIMarvelResetBit(int Mode, int Addr)
		{
			UI_Marvel[Mode] &= ~(1 << Addr);
		}

		public void UIMarveHeader(ref UIParamStrc UI)
		{
			if (UI_Marvel[0] > 0 || UIMarvelGetBit(1, 6))
			{
				UIMarvelSetBit(12, 0);
			}
			else
			{
				UIMarvelResetBit(12, 0);
			}
			int MaskFFFF = 0;
			for (int i = 0; i <= 6; i++)
			{
				MaskFFFF |= UI_Marvel[2 + i];
			}
			if (MaskFFFF > 0)
			{
				UIMarvelSetBit(12, 1);
			}
			else
			{
				UIMarvelResetBit(12, 1);
			}
			if (UIMarvelGetBit(1, 4) || UIMarvelGetBit(1, 5))
			{
				UIMarvelSetBit(12, 2);
			}
			else
			{
				UIMarvelResetBit(12, 2);
			}
			if (UI_Marvel[2] > 0 && UI.CurrWAItem[0].RotationSpeed_3 != 0)
			{
				UIMarvelSetBit(12, 3);
			}
			else
			{
				UIMarvelResetBit(12, 3);
			}
			if (UI_Marvel[3] > 0 && UI.CurrWAItem[1].RotationSpeed_3 != 0)
			{
				UIMarvelSetBit(12, 4);
			}
			else
			{
				UIMarvelResetBit(12, 4);
			}
			if (UI_Marvel[4] > 0 && UI.CurrWAItem[2].RotationSpeed_3 != 0)
			{
				UIMarvelSetBit(12, 5);
			}
			else
			{
				UIMarvelResetBit(12, 5);
			}
			if (UI_Marvel[5] > 0 && UI.CurrWAItem[3].RotationSpeed_3 != 0)
			{
				UIMarvelSetBit(12, 6);
			}
			else
			{
				UIMarvelResetBit(12, 6);
			}
			if (UI_Marvel[6] > 0 && UI.CurrWAItem[4].RotationSpeed_3 != 0)
			{
				UIMarvelSetBit(12, 7);
			}
			else
			{
				UIMarvelResetBit(12, 7);
			}
			if (UI_Marvel[7] > 0 && UI.CurrWAItem[5].RotationSpeed_3 != 0)
			{
				UIMarvelSetBit(12, 8);
			}
			else
			{
				UIMarvelResetBit(12, 8);
			}
		}

		public void ALNGMsgStartStopFunction(bool En)
		{
			StartTimerWithSleepFlag = En;
		}

		public unsafe string GetNameTitleStr(FormType FormNum, int GP)
		{
			List<byte> TitleChar = new List<byte>();
			TitleChar.Clear();
			switch (FormNum)
			{
			case FormType.ParamX:
			case FormType.ParamNonSpaceX:
			{
				for (uint u12 = 0u; u12 < 20; u12++)
				{
					TitleChar.Add((byte)(FSParamX[GP].Comm.TitleChar[u12] & 0xFF));
					TitleChar.Add((byte)((FSParamX[GP].Comm.TitleChar[u12] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.ParamY:
			case FormType.ParamNonSpaceY:
			{
				for (uint u35 = 0u; u35 < 20; u35++)
				{
					TitleChar.Add((byte)(FSParamY[GP].Comm.TitleChar[u35] & 0xFF));
					TitleChar.Add((byte)((FSParamY[GP].Comm.TitleChar[u35] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.Seq:
			case FormType.SeqNonSpace:
			{
				for (uint u4 = 0u; u4 < 20; u4++)
				{
					TitleChar.Add((byte)(FSSeqGB[GP].TitleChar[u4] & 0xFF));
					TitleChar.Add((byte)((FSSeqGB[GP].TitleChar[u4] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubSrcManualX:
			{
				for (uint u16 = 0u; u16 < 100; u16++)
				{
					if (FSSrcMode.ActionMode == 0)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcManualX[0].BarcodeString[u16] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcManualX[0].BarcodeString[u16] & 0xFF00) >> 8));
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[u16] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[u16] & 0xFF00) >> 8));
					}
					else
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[u16] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[u16] & 0xFF00) >> 8));
					}
				}
				break;
			}
			case FormType.SubSrcSelectBitX:
			{
				for (uint u8 = 0u; u8 < 100; u8++)
				{
					if (FSSrcMode.ActionMode == 0)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcBitsX[GP].BarcodeString[u8] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcBitsX[GP].BarcodeString[u8] & 0xFF00) >> 8));
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[u8] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[u8] & 0xFF00) >> 8));
					}
					else
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[u8] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[u8] & 0xFF00) >> 8));
					}
				}
				break;
			}
			case FormType.SubSrcBarcodeX:
			{
				for (uint u37 = 0u; u37 < 100; u37++)
				{
					if (FSSrcMode.ActionMode == 0)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcScannerX[GP].BarcodeString[u37] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcScannerX[GP].BarcodeString[u37] & 0xFF00) >> 8));
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[u37] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[u37] & 0xFF00) >> 8));
					}
					else
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[u37] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[u37] & 0xFF00) >> 8));
					}
				}
				break;
			}
			case FormType.SubSrcManualY:
			{
				for (uint u18 = 0u; u18 < 100; u18++)
				{
					if (FSSrcMode.ActionMode == 0)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcManualY[0].BarcodeString[u18] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcManualY[0].BarcodeString[u18] & 0xFF00) >> 8));
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[u18] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[u18] & 0xFF00) >> 8));
					}
					else
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[u18] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[u18] & 0xFF00) >> 8));
					}
				}
				break;
			}
			case FormType.SubSrcSelectBitY:
			{
				for (uint u14 = 0u; u14 < 100; u14++)
				{
					if (FSSrcMode.ActionMode == 0)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcBitsY[GP].BarcodeString[u14] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcBitsY[GP].BarcodeString[u14] & 0xFF00) >> 8));
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[u14] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[u14] & 0xFF00) >> 8));
					}
					else
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[u14] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[u14] & 0xFF00) >> 8));
					}
				}
				break;
			}
			case FormType.SubSrcBarcodeY:
			{
				for (uint u10 = 0u; u10 < 100; u10++)
				{
					if (FSSrcMode.ActionMode == 0)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcScannerY[GP].BarcodeString[u10] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcScannerY[GP].BarcodeString[u10] & 0xFF00) >> 8));
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[u10] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[u10] & 0xFF00) >> 8));
					}
					else
					{
						TitleChar.Add((byte)(FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[u10] & 0xFF));
						TitleChar.Add((byte)((FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[u10] & 0xFF00) >> 8));
					}
				}
				break;
			}
			case FormType.SubResultBarcodeX:
			{
				for (uint u6 = 0u; u6 < 100; u6++)
				{
					TitleChar.Add((byte)(TcpStatus.Detail.T1StB.Barcode[u6] & 0xFF));
					TitleChar.Add((byte)((TcpStatus.Detail.T1StB.Barcode[u6] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubResultBarcodeY:
			{
				for (uint u2 = 0u; u2 < 100; u2++)
				{
					TitleChar.Add((byte)(TcpStatus.Detail.T2StB.Barcode[u2] & 0xFF));
					TitleChar.Add((byte)((TcpStatus.Detail.T2StB.Barcode[u2] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubCtrlFWVersion:
			{
				for (uint u36 = 0u; u36 < 20; u36++)
				{
					TitleChar.Add((byte)(FSCtrlVersion.Data16[u36] & 0xFF));
					TitleChar.Add((byte)((FSCtrlVersion.Data16[u36] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubCtrlModelName:
			{
				for (uint u34 = 0u; u34 < 20; u34++)
				{
					TitleChar.Add((byte)(FSCtrlModelName.Data16[u34] & 0xFF));
					TitleChar.Add((byte)((FSCtrlModelName.Data16[u34] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubCtrlUserName:
				switch (GP)
				{
				case 0:
					if (ExFSUser.User1Name[0] == 0 && ExFSUser.User1Name[1] == 0)
					{
						byte[] Src9 = Encoding.ASCII.GetBytes("User1");
						int Size9 = Src9.Length;
						for (uint u32 = 0u; u32 < Size9; u32++)
						{
							TitleChar.Add(Src9[u32]);
						}
					}
					else
					{
						for (uint u33 = 0u; u33 < 10; u33++)
						{
							TitleChar.Add((byte)(ExFSUser.User1Name[u33] & 0xFF));
							TitleChar.Add((byte)((ExFSUser.User1Name[u33] & 0xFF00) >> 8));
						}
					}
					break;
				case 1:
					if (ExFSUser.User2Name[0] == 0 && ExFSUser.User2Name[1] == 0)
					{
						byte[] Src6 = Encoding.ASCII.GetBytes("User2");
						int Size6 = Src6.Length;
						for (uint u26 = 0u; u26 < Size6; u26++)
						{
							TitleChar.Add(Src6[u26]);
						}
					}
					else
					{
						for (uint u27 = 0u; u27 < 10; u27++)
						{
							TitleChar.Add((byte)(ExFSUser.User2Name[u27] & 0xFF));
							TitleChar.Add((byte)((ExFSUser.User2Name[u27] & 0xFF00) >> 8));
						}
					}
					break;
				case 2:
					if (ExFSUser.User3Name[0] == 0 && ExFSUser.User3Name[1] == 0)
					{
						byte[] Src2 = Encoding.ASCII.GetBytes("User3");
						int Size2 = Src2.Length;
						for (uint u20 = 0u; u20 < Size2; u20++)
						{
							TitleChar.Add(Src2[u20]);
						}
					}
					else
					{
						for (uint u21 = 0u; u21 < 10; u21++)
						{
							TitleChar.Add((byte)(ExFSUser.User3Name[u21] & 0xFF));
							TitleChar.Add((byte)((ExFSUser.User3Name[u21] & 0xFF00) >> 8));
						}
					}
					break;
				case 3:
					if (ExFSUser.User4Name[0] == 0 && ExFSUser.User4Name[1] == 0)
					{
						byte[] Src8 = Encoding.ASCII.GetBytes("User4");
						int Size8 = Src8.Length;
						for (uint u30 = 0u; u30 < Size8; u30++)
						{
							TitleChar.Add(Src8[u30]);
						}
					}
					else
					{
						for (uint u31 = 0u; u31 < 10; u31++)
						{
							TitleChar.Add((byte)(ExFSUser.User4Name[u31] & 0xFF));
							TitleChar.Add((byte)((ExFSUser.User4Name[u31] & 0xFF00) >> 8));
						}
					}
					break;
				case 4:
					if (ExFSUser.User5Name[0] == 0 && ExFSUser.User5Name[1] == 0)
					{
						byte[] Src4 = Encoding.ASCII.GetBytes("User5");
						int Size4 = Src4.Length;
						for (uint u23 = 0u; u23 < Size4; u23++)
						{
							TitleChar.Add(Src4[u23]);
						}
					}
					else
					{
						for (uint u24 = 0u; u24 < 10; u24++)
						{
							TitleChar.Add((byte)(ExFSUser.User5Name[u24] & 0xFF));
							TitleChar.Add((byte)((ExFSUser.User5Name[u24] & 0xFF00) >> 8));
						}
					}
					break;
				case 5:
					if (ExFSUser.User6Name[0] == 0 && ExFSUser.User6Name[1] == 0)
					{
						byte[] Src7 = Encoding.ASCII.GetBytes("Admin");
						int Size7 = Src7.Length;
						for (uint u28 = 0u; u28 < Size7; u28++)
						{
							TitleChar.Add(Src7[u28]);
						}
					}
					else
					{
						for (uint u29 = 0u; u29 < 10; u29++)
						{
							TitleChar.Add((byte)(ExFSUser.User6Name[u29] & 0xFF));
							TitleChar.Add((byte)((ExFSUser.User6Name[u29] & 0xFF00) >> 8));
						}
					}
					break;
				case 6:
				{
					byte[] Src5 = Encoding.ASCII.GetBytes("UserRD");
					int Size5 = Src5.Length;
					for (uint u25 = 0u; u25 < Size5; u25++)
					{
						TitleChar.Add(Src5[u25]);
					}
					break;
				}
				case 7:
				{
					byte[] Src3 = Encoding.ASCII.GetBytes("SystemRecord");
					int Size3 = Src3.Length;
					for (uint u22 = 0u; u22 < Size3; u22++)
					{
						TitleChar.Add(Src3[u22]);
					}
					break;
				}
				default:
				{
					byte[] Src = Encoding.ASCII.GetBytes("Unknow");
					int Size = Src.Length;
					for (uint u19 = 0u; u19 < Size; u19++)
					{
						TitleChar.Add(Src[u19]);
					}
					break;
				}
				}
				break;
			case FormType.SubToolXModelName:
			{
				for (uint u17 = 0u; u17 < 10; u17++)
				{
					byte data9 = (byte)(FSToolXInfo.ModelNameChar[u17] & 0xFF);
					byte data10 = (byte)((FSToolXInfo.ModelNameChar[u17] & 0xFF00) >> 8);
					if (data9 != 0)
					{
						TitleChar.Add(data9);
					}
					if (data10 != 0)
					{
						TitleChar.Add(data10);
					}
				}
				break;
			}
			case FormType.SubToolYModelName:
			{
				for (uint u15 = 0u; u15 < 10; u15++)
				{
					byte data7 = (byte)(FSToolYInfo.ModelNameChar[u15] & 0xFF);
					byte data8 = (byte)((FSToolYInfo.ModelNameChar[u15] & 0xFF00) >> 8);
					if (data7 != 0)
					{
						TitleChar.Add(data7);
					}
					if (data8 != 0)
					{
						TitleChar.Add(data8);
					}
				}
				break;
			}
			case FormType.SubToolXProductionNumber:
			{
				for (uint u13 = 0u; u13 < 10; u13++)
				{
					byte data5 = (byte)(FSToolXInfo.ProductionNumberChar[u13] & 0xFF);
					byte data6 = (byte)((FSToolXInfo.ProductionNumberChar[u13] & 0xFF00) >> 8);
					if (data5 != 0)
					{
						TitleChar.Add(data5);
					}
					if (data6 != 0)
					{
						TitleChar.Add(data6);
					}
				}
				break;
			}
			case FormType.SubToolYProductionNumber:
			{
				for (uint u11 = 0u; u11 < 10; u11++)
				{
					byte data3 = (byte)(FSToolYInfo.ProductionNumberChar[u11] & 0xFF);
					byte data4 = (byte)((FSToolYInfo.ProductionNumberChar[u11] & 0xFF00) >> 8);
					if (data3 != 0)
					{
						TitleChar.Add(data3);
					}
					if (data4 != 0)
					{
						TitleChar.Add(data4);
					}
				}
				break;
			}
			case FormType.SubToolXVersion:
			{
				for (uint u9 = 0u; u9 < 20; u9++)
				{
					TitleChar.Add((byte)(FSToolXVersion.Data16[u9] & 0xFF));
					TitleChar.Add((byte)((FSToolXVersion.Data16[u9] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubToolYVersion:
			{
				for (uint u7 = 0u; u7 < 20; u7++)
				{
					TitleChar.Add((byte)(FSToolYVersion.Data16[u7] & 0xFF));
					TitleChar.Add((byte)((FSToolYVersion.Data16[u7] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubToolDateTime:
			{
				for (uint u5 = 0u; u5 < 10; u5++)
				{
					byte data1 = 0;
					byte data2 = 0;
					switch (GP)
					{
					case 0:
						data1 = (byte)(FSToolTeachRecord.DateTime1[u5] & 0xFF);
						data2 = (byte)((FSToolTeachRecord.DateTime1[u5] & 0xFF00) >> 8);
						break;
					case 1:
						data1 = (byte)(FSToolTeachRecord.DateTime2[u5] & 0xFF);
						data2 = (byte)((FSToolTeachRecord.DateTime2[u5] & 0xFF00) >> 8);
						break;
					case 2:
						data1 = (byte)(FSToolTeachRecord.DateTime3[u5] & 0xFF);
						data2 = (byte)((FSToolTeachRecord.DateTime3[u5] & 0xFF00) >> 8);
						break;
					case 3:
						data1 = (byte)(FSToolTeachRecord.DateTime4[u5] & 0xFF);
						data2 = (byte)((FSToolTeachRecord.DateTime4[u5] & 0xFF00) >> 8);
						break;
					case 4:
						data1 = (byte)(FSToolTeachRecord.DateTime5[u5] & 0xFF);
						data2 = (byte)((FSToolTeachRecord.DateTime5[u5] & 0xFF00) >> 8);
						break;
					}
					if (data1 != 0)
					{
						TitleChar.Add(data1);
					}
					if (data2 != 0)
					{
						TitleChar.Add(data2);
					}
				}
				break;
			}
			case FormType.SubReportSN:
			{
				for (uint u3 = 0u; u3 < 20; u3++)
				{
					TitleChar.Add((byte)(ExFSReport.Info[GP].SaveStr[u3] & 0xFF));
					TitleChar.Add((byte)((ExFSReport.Info[GP].SaveStr[u3] & 0xFF00) >> 8));
				}
				break;
			}
			case FormType.SubLocalAddr:
			{
				for (uint u = 0u; u < GP; u++)
				{
					TitleChar.Add((byte)(FSCtrlLocalTable.Data16[u] & 0xFF));
					TitleChar.Add((byte)((FSCtrlLocalTable.Data16[u] & 0xFF00) >> 8));
				}
				break;
			}
			}
			string RetStr = Encoding.ASCII.GetString(TitleChar.ToArray()).Trim().TrimEnd(default(char));
			if ((uint)(FormNum - 20) <= 2u && RetStr == "")
			{
				RetStr = "(Non-Exist)";
			}
			return RetStr;
		}

		public ushort UserIDDetect()
		{
			ushort Vlock = 0;
			switch (ExFSUser.UserID)
			{
			case 0u:
				return FSCtrlPageAuthority.User1;
			case 1u:
				return FSCtrlPageAuthority.User2;
			case 2u:
				return FSCtrlPageAuthority.User3;
			case 3u:
				return FSCtrlPageAuthority.User4;
			case 4u:
				return FSCtrlPageAuthority.User5;
			case 5u:
				return 0;
			default:
				return 0;
			}
		}

		public void PermissOfUserID_ShowPic(ref Button Bn, ref Image[] Img, int Match)
		{
			ushort Vlock = UserIDDetect();
			Bn.Image = (((Vlock & Match) > 0) ? Img[0] : Img[1]);
		}

		public void PermissOfUserID_HidePic(ref Button Bn, ref Image[] Img, int Match)
		{
			ushort Vlock = UserIDDetect();
			Bn.Visible = (((Vlock & Match) <= 0) ? true : false);
			Bn.Enabled = (((Vlock & Match) <= 0) ? true : false);
			Bn.Image = (((Vlock & Match) > 0) ? Img[0] : Img[1]);
		}

		public void PermissOfUserID(ref Button Bn, ref Image[] Img, int Match)
		{
			ushort Vlock = UserIDDetect();
			Bn.Enabled = (((Vlock & Match) <= 0) ? true : false);
			Bn.Image = (((Vlock & Match) > 0) ? Img[0] : Img[1]);
		}

		public void PermissOfUserID(ref TextBox TB, ref Image[] Img, int Match)
		{
			ushort Vlock = UserIDDetect();
			TB.Enabled = (((Vlock & Match) <= 0) ? true : false);
		}

		private string ReplaceInvalidCharacters(string text)
		{
			StringBuilder result = new StringBuilder();
			foreach (char c in text)
			{
				if (c < '!' || c > '\u007f')
				{
					result.Append(' ');
				}
				else
				{
					result.Append(c);
				}
			}
			return result.ToString();
		}

		public unsafe void SetNameTitleStr(FormType FormNum, int GP, string OrgStr)
		{
			string str = ReplaceInvalidCharacters(OrgStr);
			switch (FormNum)
			{
			case FormType.ParamX:
			{
				for (uint n29 = 0u; n29 < 20; n29++)
				{
					FSParamX[GP].Comm.TitleChar[n29] = 0;
				}
				break;
			}
			case FormType.ParamY:
			{
				for (uint n28 = 0u; n28 < 20; n28++)
				{
					FSParamY[GP].Comm.TitleChar[n28] = 0;
				}
				break;
			}
			case FormType.Seq:
			{
				for (uint n10 = 0u; n10 < 20; n10++)
				{
					FSSeqGB[GP].TitleChar[n10] = 0;
				}
				break;
			}
			case FormType.SubSrcManualX:
				if (FSSrcMode.ActionMode == 0)
				{
					for (uint n12 = 0u; n12 < 100; n12++)
					{
						FSSrcAll.FSSrcManualX[0].BarcodeString[n12] = 0;
					}
				}
				else if (FSSrcMode.ActionMode == 1)
				{
					for (uint n13 = 0u; n13 < 100; n13++)
					{
						FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[n13] = 0;
					}
				}
				else
				{
					for (uint n14 = 0u; n14 < 100; n14++)
					{
						FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[n14] = 0;
					}
				}
				break;
			case FormType.SubSrcSelectBitX:
				if (FSSrcMode.ActionMode == 0)
				{
					for (uint n6 = 0u; n6 < 100; n6++)
					{
						FSSrcAll.FSSrcBitsX[GP].BarcodeString[n6] = 0;
					}
				}
				else if (FSSrcMode.ActionMode == 1)
				{
					for (uint n7 = 0u; n7 < 100; n7++)
					{
						FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[n7] = 0;
					}
				}
				else
				{
					for (uint n8 = 0u; n8 < 100; n8++)
					{
						FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[n8] = 0;
					}
				}
				break;
			case FormType.SubSrcBarcodeX:
				if (FSSrcMode.ActionMode == 0)
				{
					for (uint n2 = 0u; n2 < 100; n2++)
					{
						FSSrcAll.FSSrcScannerX[GP].BarcodeString[n2] = 0;
					}
				}
				else if (FSSrcMode.ActionMode == 1)
				{
					for (uint n3 = 0u; n3 < 100; n3++)
					{
						FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[n3] = 0;
					}
				}
				else
				{
					for (uint n4 = 0u; n4 < 100; n4++)
					{
						FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[n4] = 0;
					}
				}
				break;
			case FormType.SubResultBarcodeX:
			{
				for (uint n24 = 0u; n24 < 100; n24++)
				{
					FSResultBarcodeX.Data16[n24] = 0;
				}
				break;
			}
			case FormType.SubSrcManualY:
				if (FSSrcMode.ActionMode == 0)
				{
					for (uint n16 = 0u; n16 < 100; n16++)
					{
						FSSrcAll.FSSrcManualY[0].BarcodeString[n16] = 0;
					}
				}
				else if (FSSrcMode.ActionMode == 1)
				{
					for (uint n17 = 0u; n17 < 100; n17++)
					{
						FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[n17] = 0;
					}
				}
				else
				{
					for (uint n18 = 0u; n18 < 100; n18++)
					{
						FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[n18] = 0;
					}
				}
				break;
			case FormType.SubSrcSelectBitY:
				if (FSSrcMode.ActionMode == 0)
				{
					for (uint n20 = 0u; n20 < 100; n20++)
					{
						FSSrcAll.FSSrcBitsY[GP].BarcodeString[n20] = 0;
					}
				}
				else if (FSSrcMode.ActionMode == 1)
				{
					for (uint n21 = 0u; n21 < 100; n21++)
					{
						FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[n21] = 0;
					}
				}
				else
				{
					for (uint n22 = 0u; n22 < 100; n22++)
					{
						FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[n22] = 0;
					}
				}
				break;
			case FormType.SubSrcBarcodeY:
				if (FSSrcMode.ActionMode == 0)
				{
					for (uint n25 = 0u; n25 < 100; n25++)
					{
						FSSrcAll.FSSrcScannerY[GP].BarcodeString[n25] = 0;
					}
				}
				else if (FSSrcMode.ActionMode == 1)
				{
					for (uint n26 = 0u; n26 < 100; n26++)
					{
						FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[n26] = 0;
					}
				}
				else
				{
					for (uint n27 = 0u; n27 < 100; n27++)
					{
						FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[n27] = 0;
					}
				}
				break;
			case FormType.SubResultBarcodeY:
			{
				for (uint n23 = 0u; n23 < 100; n23++)
				{
					FSResultBarcodeY.Data16[n23] = 0;
				}
				break;
			}
			case FormType.SubCtrlUserName:
			{
				for (uint n19 = 0u; n19 < 10; n19++)
				{
					switch (GP)
					{
					case 0:
						ExFSUser.User1Name[n19] = 0;
						break;
					case 1:
						ExFSUser.User2Name[n19] = 0;
						break;
					case 2:
						ExFSUser.User3Name[n19] = 0;
						break;
					case 3:
						ExFSUser.User4Name[n19] = 0;
						break;
					case 4:
						ExFSUser.User5Name[n19] = 0;
						break;
					case 5:
						ExFSUser.User6Name[n19] = 0;
						break;
					}
				}
				break;
			}
			case FormType.SubCtrlLogInPassword:
			{
				for (uint n15 = 0u; n15 < 10; n15++)
				{
					FSCtrlUserLogIn.Password[n15] = 0;
				}
				break;
			}
			case FormType.SubCtrlCurrentPassword:
			{
				for (uint n11 = 0u; n11 < 10; n11++)
				{
					FSCtrlUserPassword.OldPassword[n11] = 0;
				}
				break;
			}
			case FormType.SubCtrlNewPassword:
			{
				for (uint n9 = 0u; n9 < 10; n9++)
				{
					FSCtrlUserPassword.NewPassword[n9] = 0;
				}
				break;
			}
			case FormType.SubCtrlModelName:
			{
				for (uint n5 = 0u; n5 < 20; n5++)
				{
					FSCtrlModelName.Data16[n5] = 0;
				}
				break;
			}
			case FormType.SubSNFromBinFile:
			{
				for (uint n = 0u; n < 100; n++)
				{
					UISys.List_Info.SaveStr[n] = 0;
				}
				break;
			}
			}
			byte[] Src = Encoding.ASCII.GetBytes(str);
			if (!(str != ""))
			{
				return;
			}
			int Size = Src.Length;
			for (uint n30 = 0u; n30 < (Size + 1) / 2; n30++)
			{
				switch (FormNum)
				{
				case FormType.ParamX:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSParamX[GP].Comm.TitleChar[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSParamX[GP].Comm.TitleChar[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.ParamY:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSParamY[GP].Comm.TitleChar[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSParamY[GP].Comm.TitleChar[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.Seq:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSSeqGB[GP].TitleChar[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSSeqGB[GP].TitleChar[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubSrcManualX:
					if (FSSrcMode.ActionMode == 0)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcManualX[0].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcManualX[0].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubSrcSelectBitX:
					if (FSSrcMode.ActionMode == 0)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcBitsX[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcBitsX[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubSrcBarcodeX:
					if (FSSrcMode.ActionMode == 0)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcScannerX[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcScannerX[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubSrcManualY:
					if (FSSrcMode.ActionMode == 0)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcManualY[0].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcManualY[0].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcManual_DualMix[0].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSSrcAll.FSSrcManual_DualSync[0].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubSrcSelectBitY:
					if (FSSrcMode.ActionMode == 0)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcBitsY[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcBitsY[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcBits_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSSrcAll.FSSrcBits_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubSrcBarcodeY:
					if (FSSrcMode.ActionMode == 0)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcScannerY[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcScannerY[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (FSSrcMode.ActionMode == 1)
					{
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							FSSrcAll.FSSrcScanner_DualMix[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
					}
					else if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSSrcAll.FSSrcScanner_DualSync[GP].BarcodeString[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubResultBarcodeX:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSResultBarcodeX.Data16[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSResultBarcodeX.Data16[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubResultBarcodeY:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSResultBarcodeY.Data16[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSResultBarcodeY.Data16[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubCtrlLogInPassword:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSCtrlUserLogIn.Password[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSCtrlUserLogIn.Password[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubCtrlUserName:
					switch (GP)
					{
					case 0:
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							ExFSUser.User1Name[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							ExFSUser.User1Name[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
						break;
					case 1:
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							ExFSUser.User2Name[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							ExFSUser.User2Name[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
						break;
					case 2:
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							ExFSUser.User3Name[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							ExFSUser.User3Name[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
						break;
					case 3:
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							ExFSUser.User4Name[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							ExFSUser.User4Name[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
						break;
					case 4:
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							ExFSUser.User5Name[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							ExFSUser.User5Name[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
						break;
					case 5:
						if (n30 < Size / 2 || Size % 2 == 0)
						{
							ExFSUser.User6Name[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
						}
						else
						{
							ExFSUser.User6Name[n30] = Convert.ToUInt16(Src[2 * n30]);
						}
						break;
					}
					break;
				case FormType.SubCtrlCurrentPassword:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSCtrlUserPassword.OldPassword[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSCtrlUserPassword.OldPassword[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubCtrlNewPassword:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSCtrlUserPassword.NewPassword[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSCtrlUserPassword.NewPassword[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubCtrlModelName:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						FSCtrlModelName.Data16[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						FSCtrlModelName.Data16[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				case FormType.SubSNFromBinFile:
					if (n30 < Size / 2 || Size % 2 == 0)
					{
						UISys.List_Info.SaveStr[n30] = Convert.ToUInt16((Src[2 * n30 + 1] << 8) + Src[2 * n30]);
					}
					else
					{
						UISys.List_Info.SaveStr[n30] = Convert.ToUInt16(Src[2 * n30]);
					}
					break;
				}
			}
		}

		public void ReadALFTPFile()
		{
			ushort[,] BinALDetail = new ushort[6000, 6];
			List<byte> FS1List = ReadFSBinFile(1001);
			byte[] FS1Data8 = FS1List.ToArray();
			uint FS1ID = 0u;
			uint FS1Idx = 0u;
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				BinALDetail[FS1ID, FS1Idx] = BitConverter.ToUInt16(FS1Data8, i * 2);
				FS1Idx++;
				if (FS1Idx >= 6)
				{
					FS1ID++;
					FS1Idx = 0u;
				}
			}
			ParseFSAlarmWarningDataBase(0, 0u, 6000u, ref BinALDetail);
		}

		public void ReadWNFTPFile()
		{
			ushort[,] BinWNDetail = new ushort[6000, 6];
			List<byte> FS1List = ReadFSBinFile(1011);
			byte[] FS1Data8 = FS1List.ToArray();
			uint FS1ID = 0u;
			uint FS1Idx = 0u;
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				BinWNDetail[FS1ID, FS1Idx] = BitConverter.ToUInt16(FS1Data8, i * 2);
				FS1Idx++;
				if (FS1Idx >= 6)
				{
					FS1ID++;
					FS1Idx = 0u;
				}
			}
			ParseFSAlarmWarningDataBase(1, 0u, 6000u, ref BinWNDetail);
		}

		public void ParseFSAlarmWarningDataBase(int mode, uint StartItem, uint EndItem, ref ushort[,] Data16)
		{
			for (uint Gp = StartItem; Gp < EndItem; Gp++)
			{
				if (mode == 0)
				{
					if (Data16[Gp, 3] > 0)
					{
						DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)Data16[Gp, 0]).AddSeconds(Data16[Gp, 2] * 65536 + Data16[Gp, 1]);
						ExFSReport.AlarmInfo[Gp].Year = (uint)OpTime.Year;
						ExFSReport.AlarmInfo[Gp].Month = (uint)OpTime.Month;
						ExFSReport.AlarmInfo[Gp].Day = (uint)OpTime.Day;
						ExFSReport.AlarmInfo[Gp].Hour = (uint)OpTime.Hour;
						ExFSReport.AlarmInfo[Gp].Min = (uint)OpTime.Minute;
						ExFSReport.AlarmInfo[Gp].Sec = (uint)OpTime.Second;
						ExFSReport.AlarmInfo[Gp].Code = Data16[Gp, 3];
						ExFSReport.AlarmInfo[Gp].ReportID = (uint)(Data16[Gp, 5] * 65536 + Data16[Gp, 4]);
					}
				}
				else if (Data16[Gp, 3] > 0)
				{
					DateTime OpTime2 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)Data16[Gp, 0]).AddSeconds(Data16[Gp, 2] * 65536 + Data16[Gp, 1]);
					ExFSReport.WarningInfo[Gp].Year = (uint)OpTime2.Year;
					ExFSReport.WarningInfo[Gp].Month = (uint)OpTime2.Month;
					ExFSReport.WarningInfo[Gp].Day = (uint)OpTime2.Day;
					ExFSReport.WarningInfo[Gp].Hour = (uint)OpTime2.Hour;
					ExFSReport.WarningInfo[Gp].Min = (uint)OpTime2.Minute;
					ExFSReport.WarningInfo[Gp].Sec = (uint)OpTime2.Second;
					ExFSReport.WarningInfo[Gp].Code = Data16[Gp, 3];
					ExFSReport.WarningInfo[Gp].ReportID = (uint)(Data16[Gp, 5] * 65536 + Data16[Gp, 4]);
				}
			}
		}

		public void ReadBNFTPFile()
		{
			ushort[,] BinBNDetail = new ushort[6000, 10];
			List<byte> FS1List = ReadFSBinFile(1021);
			byte[] FS1Data8 = FS1List.ToArray();
			uint FS1ID = 0u;
			uint FS1Idx = 0u;
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				BinBNDetail[FS1ID, FS1Idx] = BitConverter.ToUInt16(FS1Data8, i * 2);
				FS1Idx++;
				if (FS1Idx >= 10)
				{
					FS1ID++;
					FS1Idx = 0u;
				}
			}
			ParseFSButtonDataBase(0u, 6000u, ref BinBNDetail);
		}

		public void ParseFSButtonDataBase(uint StartItem, uint EndItem, ref ushort[,] Data16)
		{
			for (uint Gp = StartItem; Gp < EndItem; Gp++)
			{
				if (Data16[Gp, 3] > 0)
				{
					DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)Data16[Gp, 0]).AddSeconds(Data16[Gp, 2] * 65536 + Data16[Gp, 1]);
					ExFSReport.ButtonInfo[Gp].Year = (uint)OpTime.Year;
					ExFSReport.ButtonInfo[Gp].Month = (uint)OpTime.Month;
					ExFSReport.ButtonInfo[Gp].Day = (uint)OpTime.Day;
					ExFSReport.ButtonInfo[Gp].Hour = (uint)OpTime.Hour;
					ExFSReport.ButtonInfo[Gp].Min = (uint)OpTime.Minute;
					ExFSReport.ButtonInfo[Gp].Sec = (uint)OpTime.Second;
					ExFSReport.ButtonInfo[Gp].ID = Data16[Gp, 3];
					ExFSReport.ButtonInfo[Gp].User = Data16[Gp, 8];
					ExFSReport.ButtonInfo[Gp].Before = (uint)(Data16[Gp, 5] * 65536 + Data16[Gp, 4]);
					ExFSReport.ButtonInfo[Gp].After = (uint)(Data16[Gp, 7] * 65536 + Data16[Gp, 6]);
				}
			}
		}

		public void ReadReportSNFTPFile()
		{
			ushort[,] BinReportSN = new ushort[200000, 100];
			List<byte> FS1List = ReadFSBinFile(701);
			byte[] FS1Data8 = FS1List.ToArray();
			uint FS1ID = 0u;
			uint FS1Idx = 0u;
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				BinReportSN[FS1ID, FS1Idx] = BitConverter.ToUInt16(FS1Data8, i * 2);
				FS1Idx++;
				if (FS1Idx >= 100)
				{
					FS1ID++;
					FS1Idx = 0u;
				}
			}
			ParseFSReportSNToTCPDataBase(0u, 200000u, ref BinReportSN);
		}

		public void ParseFSReportStatusToTCPDataBase(uint StartItem, uint EndItem, ref ushort[] Data16)
		{
			for (uint Gp = StartItem; Gp < EndItem; Gp++)
			{
				FSReportStatus[Gp] = Data16[Gp];
			}
		}

		public unsafe void ParseFSReportSNToTCPDataBase(uint StartItem, uint EndItem, ref ushort[,] Data16)
		{
			for (uint Gp = StartItem; Gp < EndItem; Gp++)
			{
				for (int i = 0; i < 100; i++)
				{
					ExFSReport.Info[Gp].SaveStr[i] = Data16[Gp, i];
				}
			}
		}

		public void ReadReportFTPFile()
		{
			ushort[,] BinReportDetail = new ushort[200000, 50];
			List<byte> FS1List = ReadFSBinFile(801);
			byte[] FS1Data8 = FS1List.ToArray();
			uint FS1ID = 0u;
			uint FS1Idx = 0u;
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				BinReportDetail[FS1ID, FS1Idx] = BitConverter.ToUInt16(FS1Data8, i * 2);
				FS1Idx++;
				if (FS1Idx >= 50)
				{
					FS1ID++;
					FS1Idx = 0u;
				}
			}
			ParseFSReportToTCPDataBase(0u, 200000u, ref BinReportDetail);
		}

		public unsafe void ParseFSReportToTCPDataBase(uint StartItem, uint EndItem, ref ushort[,] Data16)
		{
			for (uint Gp = StartItem; Gp < EndItem; Gp++)
			{
				DateTime OpTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays((int)Data16[Gp, 0]).AddSeconds(Data16[Gp, 2] * 65536 + Data16[Gp, 1]);
				ExFSReport.Info[Gp].Year = (ushort)OpTime.Year;
				ExFSReport.Info[Gp].Month = (ushort)OpTime.Month;
				ExFSReport.Info[Gp].Day = (ushort)OpTime.Day;
				ExFSReport.Info[Gp].Hour = (ushort)OpTime.Hour;
				ExFSReport.Info[Gp].Min = (ushort)OpTime.Minute;
				ExFSReport.Info[Gp].Sec = (ushort)OpTime.Second;
				for (int i = 0; i < 47; i++)
				{
					ExFSReport.Info[Gp].Data16[106 + i] = Data16[Gp, 3 + i];
				}
				if (FSModelTypeInfo.MesModelType == 1)
				{
					int ChageErr = 0;
					int Err = ExFSReport.Info[Gp].ErrorCode;
					if (Err >= 8192 && Err < 12288)
					{
						ChageErr = 4096 + (Err & 0xFFF);
					}
					else if (Err >= 16384 && Err < 20480)
					{
						ChageErr = 12288 + (Err & 0xFFF);
					}
					else if (Err >= 24576 && Err < 28672)
					{
						ChageErr = 20480 + (Err & 0xFFF);
					}
					ExFSReport.Info[Gp].ErrorCode = (ushort)ChageErr;
				}
				else if (FSModelTypeInfo.MesModelType == 2)
				{
					int ChageErr2 = 0;
					int Err2 = ExFSReport.Info[Gp].ErrorCode;
					if (Err2 >= 4096 && Err2 < 8192)
					{
						ChageErr2 = 8192 + (Err2 & 0xFFF);
					}
					else if (Err2 >= 12288 && Err2 < 16384)
					{
						ChageErr2 = 16384 + (Err2 & 0xFFF);
					}
					else if (Err2 >= 20480 && Err2 < 24576)
					{
						ChageErr2 = 24576 + (Err2 & 0xFFF);
					}
					ExFSReport.Info[Gp].ErrorCode = (ushort)ChageErr2;
				}
				else
				{
					ExFSReport.Info[Gp].ErrorCode = ExFSReport.Info[Gp].ErrorCode;
				}
			}
		}

		public ReportInfoStuc ReportInfoTransferCoef(ReportInfoStuc Info)
		{
			double coef = TorqUnitcoef(1000 + Info.TorqueUnit) / TorqUnitcoef(1000 + Info.FWSystemCoef);
			Info.TargetTorque_DW = (uint)((double)(int)Info.TargetTorque * coef);
			Info.TargetTorqueRate_DW = (uint)((double)(int)Info.TargetTorqueRate * coef);
			Info.FinalTorque_DW = (uint)((double)(int)Info.FinalTorque * coef);
			Info.MaxTorque_DW = (uint)((double)(int)Info.MaxTorque * coef);
			Info.MinTorque_DW = (uint)((double)(int)Info.MinTorque * coef);
			Info.PreTighteningTorque_DW = (uint)((double)(int)Info.PreTighteningTorque * coef);
			Info.FinalStage_SetMaxTorque_DW = (uint)((double)(int)Info.FinalStage_SetMaxTorque * coef);
			Info.FinalStage_SetMinTorque_DW = (uint)((double)(int)Info.FinalStage_SetMinTorque * coef);
			Info.PrevailTorque_DW = (uint)((double)(int)Info.PrevailTorque * coef);
			Info.AppliedTorque_DW = (uint)((double)(int)Info.AppliedTorque * coef);
			Info.ClampTorque_DW = (uint)((double)(int)Info.ClampTorque * coef);
			Info.SetMaxClampTorque_DW = (uint)((double)(int)Info.SetMaxClampTorque * coef);
			return Info;
		}

		public ReportScaleStuc ReportScaleTransferCoef(ReportInfoStuc Info, ReportScaleStuc Scale)
		{
			double coef = TorqUnitcoef(1000 + Info.TorqueUnit) / TorqUnitcoef(1000 + Info.FWSystemCoef);
			Scale.Stage1Torque_DW = (int)((double)Scale.Stage1Torque * coef);
			Scale.Stage2Torque_DW = (int)((double)Scale.Stage2Torque * coef);
			Scale.Stage3Torque_DW = (int)((double)Scale.Stage3Torque * coef);
			Scale.Stage4Torque_DW = (int)((double)Scale.Stage4Torque * coef);
			Scale.Stage5Torque_DW = (int)((double)Scale.Stage5Torque * coef);
			Scale.Stage6Torque_DW = (int)((double)Scale.Stage6Torque * coef);
			Scale.Loosening1Torque_DW = (int)((double)Scale.Loosening1Torque * coef);
			Scale.Loosening2Torque_DW = (int)((double)Scale.Loosening2Torque * coef);
			Scale.Curve_MaxTorque_DW = (int)((double)Scale.Curve_MaxTorque * coef);
			if (Scale.CurveVer == 2)
			{
				Scale.Curve_MaxTorqueRate_DW = Scale.Curve_MaxTorqueRate;
			}
			else
			{
				Scale.Curve_MaxTorqueRate_DW = (int)((double)Scale.Curve_MaxTorqueRate * coef);
			}
			Scale.SetMaxTorque_DW = (int)((double)(int)Scale.SetMaxTorque * coef);
			Scale.SetMinTorque_DW = (int)((double)(int)Scale.SetMinTorque * coef);
			if (Scale.CurveVer == 2)
			{
				Scale.SetMaxTorqRate_DW = Scale.SetMaxTorqRate;
			}
			else
			{
				Scale.SetMaxTorqRate_DW = (int)((double)(int)Scale.SetMaxTorqRate * coef);
			}
			if (Scale.CurveVer == 2)
			{
				Scale.CurveMaxTorqueRate_DW = (uint)Scale.CurveMaxTorqueRate;
			}
			else
			{
				Scale.CurveMaxTorqueRate_DW = (uint)((double)Scale.CurveMaxTorqueRate * coef);
			}
			Scale.Curve_MinTorque_DW = (int)((double)Scale.Curve_MinTorque * coef);
			if (Scale.CurveVer == 2)
			{
				Scale.Curve_MinTorqueRate_DW = Scale.Curve_MinTorqueRate;
			}
			else
			{
				Scale.Curve_MinTorqueRate_DW = (int)((double)Scale.Curve_MinTorqueRate * coef);
			}
			double ChangeCoef = ((Scale.CurveVer == 2) ? 1.0 : coef);
			Scale.Stage1SwitchTorq_DW = (int)((double)Scale.Stage1SwitchTorq * ChangeCoef);
			Scale.Stage2SwitchTorq_DW = (int)((double)Scale.Stage2SwitchTorq * ChangeCoef);
			Scale.Stage3SwitchTorq_DW = (int)((double)Scale.Stage3SwitchTorq * ChangeCoef);
			Scale.Stage4SwitchTorq_DW = (int)((double)Scale.Stage4SwitchTorq * ChangeCoef);
			Scale.Stage5SwitchTorq_DW = (int)((double)Scale.Stage5SwitchTorq * ChangeCoef);
			Scale.Stage6SwitchTorq_DW = (int)((double)Scale.Stage6SwitchTorq * ChangeCoef);
			return Scale;
		}

		public unsafe void ParseFSScaleToTCPDataBase(uint Gp, ref ushort[] Data16)
		{
			for (int i = 0; i < 50; i++)
			{
				ExFSReport.Scale[Gp].Data16[i] = Data16[i];
			}
		}

		public unsafe void ParseFSCurveScaleParamToTCPDataBase(uint Gp, uint Mode, ref ushort[] Data16)
		{
			if (Mode == 0 || Mode == 10 || Mode == 20 || Mode == 30)
			{
				for (int i = 0; i < 2000; i++)
				{
					ExFSReport.CurveTime[(int)(Mode / 10 * 2000) + i] = Data16[i];
				}
				return;
			}
			if (Mode == 1 || Mode == 11 || Mode == 21 || Mode == 31)
			{
				for (int j = 0; j < 2000; j++)
				{
					ExFSReport.CurveAngle[(int)(Mode / 10 * 2000) + j] = (short)Data16[j];
				}
				return;
			}
			if (Mode == 2 || Mode == 12 || Mode == 22 || Mode == 32)
			{
				for (int k = 0; k < 2000; k++)
				{
					ExFSReport.CurveTorque[(int)(Mode / 10 * 2000) + k] = (short)Data16[k];
				}
				return;
			}
			if (Mode == 3 || Mode == 13 || Mode == 23 || Mode == 33)
			{
				for (int l = 0; l < 2000; l++)
				{
					ExFSReport.CurveTorqueRate[(int)(Mode / 10 * 2000) + l] = (short)Data16[l];
				}
				return;
			}
			switch (Mode)
			{
			case 4u:
			{
				for (int num5 = 0; num5 < 50; num5++)
				{
					ExFSReport.Scale[Gp].Data16[num5] = Data16[num5];
				}
				for (int num6 = 0; num6 < 550; num6++)
				{
					ExFSReport.ReportParam[num6] = Data16[50 + num6];
				}
				break;
			}
			default:
				if (Mode != 4000)
				{
					break;
				}
				goto case 1000u;
			case 1000u:
			case 2000u:
			case 3000u:
			{
				for (int m = 0; m < 2000; m++)
				{
					ExFSReport.CurveTime[(int)((Mode - 1) / 1000 * 2000) + m] = Data16[m];
				}
				for (int n = 0; n < 2000; n++)
				{
					ExFSReport.CurveAngle[(int)((Mode - 1) / 1000 * 2000) + n] = (short)Data16[n + 2000];
				}
				for (int num = 0; num < 2000; num++)
				{
					ExFSReport.CurveTorque[(int)((Mode - 1) / 1000 * 2000) + num] = (short)Data16[num + 4000];
				}
				for (int num2 = 0; num2 < 2000; num2++)
				{
					ExFSReport.CurveTorqueRate[(int)((Mode - 1) / 1000 * 2000) + num2] = (short)Data16[num2 + 6000];
				}
				if (Mode == 1000)
				{
					for (int num3 = 0; num3 < 50; num3++)
					{
						ExFSReport.Scale[Gp].Data16[num3] = Data16[num3 + 8000];
					}
					for (int num4 = 0; num4 < 550; num4++)
					{
						ExFSReport.ReportParam[num4] = Data16[num4 + 50 + 8000];
					}
				}
				break;
			}
			}
		}

		public void ReadCurveFTPFile(int mode)
		{
		}

		public void ParseFSCurveToTCPDataBase(uint StartItem, uint EndItem)
		{
		}

		public void ReadSrcFTPFile(int Axis, ushort SrcMode)
		{
			ushort[,] BinSrcFS = new ushort[3, 76800];
			int GetFSNum = 0;
			switch (SrcMode)
			{
			case 50:
				GetFSNum = 500;
				break;
			case 40:
				GetFSNum = 400;
				break;
			default:
				GetFSNum = ((FSModelTypeInfo.MesModelType != 1) ? ((FSModelTypeInfo.MesModelType != 2) ? (300 + Axis * 50) : 300) : 350);
				break;
			}
			List<byte> FS1List = ReadFSBinFile(GetFSNum + 1);
			byte[] FS1Data8 = FS1List.ToArray();
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				if (i < 76800)
				{
					BinSrcFS[0, i] = BitConverter.ToUInt16(FS1Data8, i * 2);
				}
			}
			List<byte> FS2List = ReadFSBinFile(GetFSNum + 2);
			byte[] FS2Data8 = FS2List.ToArray();
			for (int j = 0; j < FS2List.Count() / 2; j++)
			{
				if (j < 76800)
				{
					BinSrcFS[1, j] = BitConverter.ToUInt16(FS2Data8, j * 2);
				}
			}
			List<byte> FS3List = ReadFSBinFile(GetFSNum + 3);
			byte[] FS3Data8 = FS3List.ToArray();
			for (int k = 0; k < FS3List.Count() / 2; k++)
			{
				if (k < 76800)
				{
					BinSrcFS[2, k] = BitConverter.ToUInt16(FS3Data8, k * 2);
				}
			}
			switch (SrcMode)
			{
			case 30:
				if (FS1List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 0, ref FSSrcAll.FSSrcManualX, ref BinSrcFS);
				}
				if (FS2List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 1, ref FSSrcAll.FSSrcBitsX, ref BinSrcFS);
				}
				if (FS3List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 2, ref FSSrcAll.FSSrcScannerX, ref BinSrcFS);
				}
				break;
			case 35:
				if (FS1List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 0, ref FSSrcAll.FSSrcManualY, ref BinSrcFS);
				}
				if (FS2List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 1, ref FSSrcAll.FSSrcBitsY, ref BinSrcFS);
				}
				if (FS3List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 2, ref FSSrcAll.FSSrcScannerY, ref BinSrcFS);
				}
				break;
			case 40:
				if (FS1List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 0, ref FSSrcAll.FSSrcManual_DualMix, ref BinSrcFS);
				}
				if (FS2List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 1, ref FSSrcAll.FSSrcBits_DualMix, ref BinSrcFS);
				}
				if (FS3List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 2, ref FSSrcAll.FSSrcScanner_DualMix, ref BinSrcFS);
				}
				break;
			case 50:
				if (FS1List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 0, ref FSSrcAll.FSSrcManual_DualSync, ref BinSrcFS);
				}
				if (FS2List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 1, ref FSSrcAll.FSSrcBits_DualSync, ref BinSrcFS);
				}
				if (FS3List.Count > 0)
				{
					ParseFSSrcToTCPDataBase(Axis, 2, ref FSSrcAll.FSSrcScanner_DualSync, ref BinSrcFS);
				}
				break;
			}
		}

		public unsafe void ParseFSSrcToTCPDataBase(int Axis, int Mode, ref SrcStuc[] FSSrc, ref ushort[,] Data16)
		{
			int Loop = 0;
			switch (Mode)
			{
			case 0:
				Loop = 1;
				break;
			case 1:
				Loop = 255;
				break;
			default:
				Loop = 500;
				break;
			}
			for (int GP = 0; GP < Loop; GP++)
			{
				try
				{
					for (int i = 0; i < 100; i++)
					{
						FSSrc[GP].BarcodeString[i] = Data16[Mode, GP * 100 + i];
					}
					FSSrc[GP].ParamSeqSetForTheSwitchingMethod = Data16[Mode, 50100 + GP];
					FSSrc[GP].ParamSeqIDForTheSwitchingMethod = Data16[Mode, 50600 + GP];
					FSSrc[GP].TotalScrewQuantity = (uint)(Data16[Mode, 51100 + 2 * GP + 1] * 65536 + Data16[Mode, 51100 + 2 * GP]);
					FSSrc[GP].BitID = Data16[Mode, 52100 + GP];
					FSSrc[GP].AdvancedSettings = (uint)(Data16[Mode, 52600 + 2 * GP + 1] * 65536 + Data16[Mode, 52600 + 2 * GP]);
					FSSrc[GP].SingleScrewTighteningNOKcount = (uint)(Data16[Mode, 53600 + 2 * GP + 1] * 65536 + Data16[Mode, 53600 + 2 * GP]);
					FSSrc[GP].SingleScrewLooseningNOKcount = (uint)(Data16[Mode, 54600 + 2 * GP + 1] * 65536 + Data16[Mode, 54600 + 2 * GP]);
					FSSrc[GP].CheckScannerStringLength = Data16[Mode, 57100 + GP];
					FSSrc[GP].MaxOperationTime = (uint)(Data16[Mode, 57600 + 2 * GP + 1] * 65536 + Data16[Mode, 57600 + 2 * GP]);
					FSSrc[GP].TheParametersToBeUsedUnderDualToolAlternationMode = Data16[Mode, 58600 + GP];
					FSSrc[GP].TorqueUnit = Data16[Mode, 59100 + GP];
					FSSrc[GP].StartConditionForTool1 = Data16[Mode, 59600 + GP];
					FSSrc[GP].StartConditionForTool2 = Data16[Mode, 60100 + GP];
				}
				catch (Exception ex)
				{
					string errorMessage = ex.Message + " Err No." + ex.StackTrace;
					FormPublicFunction.SaveErrLog(errorMessage);
				}
			}
		}

		public SrcStuc SrcDeflaut(int SwitchMode)
		{
			SrcStuc SrcDef = default(SrcStuc);
			if (SwitchMode == 0 || SwitchMode == 1)
			{
				SrcDef.AdvancedSettings = 1024u;
			}
			else
			{
				SrcDef.AdvancedSettings = 320u;
			}
			SrcDef.SingleScrewTighteningNOKcount = 999999u;
			SrcDef.SingleScrewLooseningNOKcount = 999999u;
			SrcDef.CheckScannerStringLength = 200;
			SrcDef.MaxOperationTime = 9999999u;
			SrcDef.TorqueUnit = FSCtrlTorqUnit.Mode;
			SrcDef.StartConditionForTool1 = FSCtrlStartCondition.Mode;
			SrcDef.StartConditionForTool2 = FSCtrlStartCondition.Mode;
			return SrcDef;
		}

		public void ReadSeqFTPFile()
		{
			ushort[,] BinSeqBase = new ushort[500, 500];
			ushort[] BinSeqEn = new ushort[500];
			List<byte> FS1List = ReadFSBinFile(201);
			byte[] FS1Data8 = FS1List.ToArray();
			uint FS1ID = 0u;
			uint FS1Idx = 0u;
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				if (FS1ID < 500)
				{
					BinSeqBase[FS1ID, FS1Idx] = BitConverter.ToUInt16(FS1Data8, i * 2);
					FS1Idx++;
					if (FS1Idx >= 500)
					{
						FS1ID++;
						FS1Idx = 0u;
					}
				}
			}
			FS1ID = 0u;
			int StartBase = 250000;
			for (int j = StartBase; j < StartBase + 500; j++)
			{
				if (j < FS1List.Count() / 2)
				{
					BinSeqEn[FS1ID] = BitConverter.ToUInt16(FS1Data8, j * 2);
				}
				else
				{
					BinSeqEn[FS1ID] = 0;
				}
				FS1ID++;
			}
			if (FS1List.Count > 0)
			{
				ParseFSSeqToTCPDataBase(ref BinSeqBase);
			}
		}

		public unsafe void ParseFSSeqToTCPDataBase(ref ushort[,] Data16)
		{
			for (int GP = 0; GP < 500; GP++)
			{
				for (int i = 0; i < 20; i++)
				{
					FSSeqGB[GP].TitleChar[i] = Data16[GP, i];
				}
				FSSeqGB[GP].GeneralNavigatorMode = Data16[GP, 23];
				FSSeqGB[GP].ArmPostioningMode = Data16[GP, 24];
				for (int j = 0; j < 10; j++)
				{
					uint ParaEnGp = Data16[GP, 80 + j];
					uint ParaAxisGp = Data16[GP, 90 + j];
					for (int n = 0; n < 10; n++)
					{
						if (FSModelTypeInfo.MesModelType == 1)
						{
							FSSeqGB[GP].ToolIDForSet[j * 10 + n] = 0;
						}
						else if (FSModelTypeInfo.MesModelType == 2)
						{
							FSSeqGB[GP].ToolIDForSet[j * 10 + n] = 1;
						}
						else if ((((ParaEnGp & ParaAxisGp) >> n) & 1) == 0)
						{
							FSSeqGB[GP].ToolIDForSet[j * 10 + n] = 0;
						}
						else
						{
							FSSeqGB[GP].ToolIDForSet[j * 10 + n] = 1;
						}
					}
				}
				for (int k = 0; k < 100; k++)
				{
					FSSeqGB[GP].ParameterIDForSet[k] = Data16[GP, 100 + k];
					FSSeqGB[GP].ScrewQuantityforSet[k] = (uint)(Data16[GP, 200 + 2 * k + 1] * 65536 + Data16[GP, 200 + 2 * k]);
					FSSeqGB[GP].BitIDForSet[k] = Data16[GP, 400 + k];
				}
				ExSeqCalu(GP);
			}
		}

		public void ReadParamFTPFile(int Axis)
		{
			ushort[,] BinParam = new ushort[500, 670];
			ushort[] BinParamEn = new ushort[500];
			ushort ParamMode = 0;
			ParamMode = (ushort)((FSModelTypeInfo.MesModelType == 1) ? 110 : ((FSModelTypeInfo.MesModelType != 2) ? ((ushort)(100 + Axis * 10)) : 100));
			List<byte> FS1List = ReadFSBinFile(ParamMode + 1);
			byte[] FS1Data8 = FS1List.ToArray();
			uint FS1ID = 0u;
			uint FS1Idx = 0u;
			for (int i = 0; i < FS1List.Count() / 2; i++)
			{
				if (FS1ID < 500)
				{
					BinParam[FS1ID, FS1Idx] = BitConverter.ToUInt16(FS1Data8, i * 2);
					FS1Idx++;
					if (FS1Idx >= 70)
					{
						FS1ID++;
						FS1Idx = 0u;
					}
				}
			}
			List<byte> FS2List = ReadFSBinFile(ParamMode + 2);
			byte[] FS2Data8 = FS2List.ToArray();
			uint FS2ID = 0u;
			int StartBase = 250000;
			for (int j = StartBase; j < StartBase + 500; j++)
			{
				if (j < FS2List.Count() / 2)
				{
					BinParamEn[FS2ID] = BitConverter.ToUInt16(FS2Data8, j * 2);
				}
				else
				{
					BinParamEn[FS2ID] = 0;
				}
				FS2ID++;
			}
			List<byte> FS3List = ReadFSBinFile(ParamMode + 3);
			byte[] FS3Data8 = FS3List.ToArray();
			uint FS3ID = 0u;
			uint FS3Idx = 0u;
			for (int k = 0; k < FS3List.Count() / 2; k++)
			{
				if (FS3ID < 500)
				{
					BinParam[FS3ID, 70 + FS3Idx] = BitConverter.ToUInt16(FS3Data8, k * 2);
					FS3Idx++;
					if (FS3Idx >= 600)
					{
						FS3ID++;
						FS3Idx = 0u;
					}
				}
			}
			if (FS1List.Count > 0 && FS2List.Count > 0 && FS3List.Count > 0)
			{
				ParseFSParamToTCPDataBase(Axis, ref BinParam);
			}
		}

		public void ParseFSParamToTCPDataBase(int Axis, ref ushort[,] Data16)
		{
			for (int GP = 0; GP < 500; GP++)
			{
				if (Axis == 0)
				{
					ParseFSParamCommLooseningToTCPDataBase(ref FSParamX[GP].Comm, ref FSParamX[GP].Loos, ref Data16, GP);
					for (int i = 0; i <= 5; i++)
					{
						switch (i)
						{
						case 0:
							ParseFSParamItemToTCPDataBase(ref FSParamX[GP].Comm, ref FSParamX[GP].Item1, ref Data16, GP, i);
							break;
						case 1:
							ParseFSParamItemToTCPDataBase(ref FSParamX[GP].Comm, ref FSParamX[GP].Item2, ref Data16, GP, i);
							break;
						case 2:
							ParseFSParamItemToTCPDataBase(ref FSParamX[GP].Comm, ref FSParamX[GP].Item3, ref Data16, GP, i);
							break;
						case 3:
							ParseFSParamItemToTCPDataBase(ref FSParamX[GP].Comm, ref FSParamX[GP].Item4, ref Data16, GP, i);
							break;
						case 4:
							ParseFSParamItemToTCPDataBase(ref FSParamX[GP].Comm, ref FSParamX[GP].Item5, ref Data16, GP, i);
							break;
						case 5:
							ParseFSParamItemToTCPDataBase(ref FSParamX[GP].Comm, ref FSParamX[GP].Item6, ref Data16, GP, i);
							break;
						}
					}
					if (Data16[GP, 23] == 0)
					{
						FSParamX[GP].Item3.MaxTorque_DW_12 = FSParamX[GP].Item4.TargetTorque_DW_4;
					}
					ExParamCalu((uint)Axis, (uint)GP, Data16[GP, 23], Data16[GP, 39], Data16[GP, 38], FSParamX[GP].Item1.RotationSpeed_3);
					continue;
				}
				ParseFSParamCommLooseningToTCPDataBase(ref FSParamY[GP].Comm, ref FSParamY[GP].Loos, ref Data16, GP);
				for (int j = 0; j <= 5; j++)
				{
					switch (j)
					{
					case 0:
						ParseFSParamItemToTCPDataBase(ref FSParamY[GP].Comm, ref FSParamY[GP].Item1, ref Data16, GP, j);
						break;
					case 1:
						ParseFSParamItemToTCPDataBase(ref FSParamY[GP].Comm, ref FSParamY[GP].Item2, ref Data16, GP, j);
						break;
					case 2:
						ParseFSParamItemToTCPDataBase(ref FSParamY[GP].Comm, ref FSParamY[GP].Item3, ref Data16, GP, j);
						break;
					case 3:
						ParseFSParamItemToTCPDataBase(ref FSParamY[GP].Comm, ref FSParamY[GP].Item4, ref Data16, GP, j);
						break;
					case 4:
						ParseFSParamItemToTCPDataBase(ref FSParamY[GP].Comm, ref FSParamY[GP].Item5, ref Data16, GP, j);
						break;
					case 5:
						ParseFSParamItemToTCPDataBase(ref FSParamY[GP].Comm, ref FSParamY[GP].Item6, ref Data16, GP, j);
						break;
					}
				}
				if (Data16[GP, 23] == 0)
				{
					FSParamY[GP].Item3.MaxTorque_DW_12 = FSParamY[GP].Item4.TargetTorque_DW_4;
				}
				ExParamCalu((uint)Axis, (uint)GP, Data16[GP, 23], Data16[GP, 39], Data16[GP, 38], FSParamY[GP].Item1.RotationSpeed_3);
			}
		}

		public unsafe List<ushort> ReadBinFileFunction(string filePath, uint StartAddr, uint Wordsize)
		{
			List<ushort> RetList16 = new List<ushort>();
			try
			{
				if (File.Exists(filePath))
				{
					using (FileStream BinFile = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						using (BinaryReader ReaderBin = new BinaryReader(BinFile))
						{
							uint size = 0u;
							size = ((BinFile.Length < 2 * (StartAddr + Wordsize)) ? ((uint)(int)BinFile.Length - StartAddr) : (2 * Wordsize));
							BinFile.Seek(2 * StartAddr, SeekOrigin.Begin);
							byte[] Data8 = ReaderBin.ReadBytes((int)size);
							fixed (byte* pData8 = Data8)
							{
								for (int i = 0; i < size / 2; i++)
								{
									ushort value = (ushort)(pData8[i * 2] | ((pData8 + i * 2)[1] << 8));
									RetList16.Add(value);
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return RetList16;
		}

		public unsafe void ParseFSParamCommLooseningToTCPDataBase(ref ParamCommStucVer1 CommArr, ref ParamLoosStucVer1 LooseningArr, ref ushort[,] Data16, int GP)
		{
			for (int i = 0; i < 20; i++)
			{
				CommArr.TitleChar[i] = Data16[GP, i];
			}
			CommArr.MinTighteningAngle_21 = Data16[GP, 46];
			CommArr.ThePrevailTorqueToBeLinked_23 = Data16[GP, 61];
			CommArr.MaxTighteningTime_24 = Data16[GP, 42];
			CommArr.MaxLooseningTime_25 = Data16[GP, 52];
			CommArr.MaxTighteningAngle_26 = Data16[GP, 43];
			CommArr.MaxLooseningAngle_27 = 32767;
			CommArr.DelayBeforeTighteningStarts_28 = Data16[GP, 45];
			CommArr.DelayBeforeLooseningStarts_29 = Data16[GP, 55];
			CommArr.TorqueUnit_30 = Data16[GP, 22];
			CommArr.AngleintervalForTorqueRateCalc_31 = Data16[GP, 35];
			if ((Data16[GP, 29] & 4) > 0)
			{
				CommArr.AdjustmentAngleForSnugPointSwitch_32 = 32767;
			}
			else
			{
				CommArr.AdjustmentAngleForSnugPointSwitch_32 = Data16[GP, 36];
			}
			CommArr.FinalCurrentSwitch_33 = (ushort)((Data16[GP, 24] <= 0) ? 1 : 0);
			CommArr.DelayBeforeToFeeder_34 = Data16[GP, 34];
			CommArr.ToolAccuracyCompensation_35 = (short)Data16[GP, 58];
			CommArr.TorqueRateDelayDetection_36 = Data16[GP, 37];
			CommArr.StartTorqueForSwitchCurveSample_DW_37 = (uint)(Data16[GP, 65] * 65536 + Data16[GP, 64]);
			CommArr.StartTorqueRateForSnugAngleCalc_DW_39 = (uint)(Data16[GP, 67] * 65536 + Data16[GP, 66]);
			if ((Data16[GP, 29] & 2) > 0)
			{
				CommArr.LostTorqueOfBitSlip_DW_41 = (uint)(Data16[GP, 54] * 65536 + Data16[GP, 53]);
				CommArr.LostAngleOfBitSlip_43 = Data16[GP, 41];
				CommArr.TheNumberOfTimesBitSlip_44 = (ushort)(Data16[GP, 40] & 0x3F);
			}
			else
			{
				CommArr.LostTorqueOfBitSlip_DW_41 = 0u;
				CommArr.LostAngleOfBitSlip_43 = 0;
				CommArr.TheNumberOfTimesBitSlip_44 = 0;
			}
			if ((Data16[GP, 29] & 0x4000) > 0)
			{
				CommArr.GyroAllowError_45 = Data16[GP, 114];
				CommArr.GyroOffset_46 = Data16[GP, 115];
				CommArr.GyroAdvance_47 = Data16[GP, 118];
			}
			else
			{
				CommArr.GyroAllowError_45 = 0;
				CommArr.GyroOffset_46 = 0;
				CommArr.GyroAdvance_47 = 0;
			}
			CommArr.StartTorqueForTighteningAngleCalc_DW_48 = (uint)(Data16[GP, 165] * 65536 + Data16[GP, 164]);
			if ((Data16[GP, 29] & 0x1000) > 0)
			{
				CommArr.MultiAdvance_49 |= 1;
			}
			else
			{
				CommArr.MultiAdvance_49 &= 65534;
			}
			LooseningArr.FirstStageLooseningAngle_1 = Data16[GP, 574];
			LooseningArr.FirstStageLooseningSpeed_2 = Data16[GP, 573];
			LooseningArr.LooseningDirection_5 = (ushort)((Data16[GP, 570] & 0x40) >> 6);
			LooseningArr.FirstStageAccTime_9 = Data16[GP, 576];
			int MonitorTorqSW = Data16[GP, 616] & 1;
			if (MonitorTorqSW == 1)
			{
				LooseningArr.DetectLooseningTorque_DW_6 = (uint)(Data16[GP, 601] * 65536 + Data16[GP, 600]);
			}
			else
			{
				LooseningArr.DetectLooseningTorque_DW_6 = 0u;
			}
			LooseningArr.DetectLooseningTorqueSW_8 = (ushort)MonitorTorqSW;
			LooseningArr.SecondStageLooseningAngle_3 = Data16[GP, 624];
			LooseningArr.SecondStageLooseningSpeed_4 = Data16[GP, 623];
			LooseningArr.SecondStageAccTime_10 = Data16[GP, 626];
			LooseningArr.HomeMode_11 = (((Data16[GP, 371] & 8) >> 3 == 1) ? ((ushort)1) : ((ushort)0));
		}

		public void ParseFSParamItemToTCPDataBase(ref ParamCommStucVer1 CommArr, ref ParamItemStucVer1 ItemArr, ref ushort[,] Data16, int GP, int Stage_i)
		{
			int StageTarget = (Data16[GP, 70 + 50 * Stage_i] & 0x30) >> 4;
			bool ClampMode = (Data16[GP, 70 + 50 * Stage_i] & 0x800) >> 11 == 1;
			int IsAngOrTorq = Data16[GP, 70 + 50 * Stage_i] & 3;
			if (IsAngOrTorq == 2 && StageTarget != 2)
			{
				ItemArr.ControlMode_1 = 6;
			}
			else if (StageTarget == 0 && !ClampMode)
			{
				ItemArr.ControlMode_1 = 0;
			}
			else if (StageTarget == 1 && !ClampMode)
			{
				ItemArr.ControlMode_1 = 1;
			}
			else if (StageTarget == 2 && !ClampMode)
			{
				ItemArr.ControlMode_1 = 2;
			}
			else if (StageTarget == 1 && ClampMode)
			{
				ItemArr.ControlMode_1 = 3;
			}
			else if (StageTarget == 0 && ClampMode)
			{
				ItemArr.ControlMode_1 = 4;
			}
			else if (StageTarget == 3)
			{
				ItemArr.ControlMode_1 = 5;
			}
			ItemArr.TighteningDirection_2 = (ushort)((Data16[GP, 70 + 50 * Stage_i] & 0x40) >> 6);
			ItemArr.RotationSpeed_3 = Data16[GP, 70 + 50 * Stage_i + 3];
			ItemArr.TargetTorque_DW_4 = (uint)(Data16[GP, 70 + 50 * Stage_i + 12 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 12]);
			ItemArr.TargetAngle_6 = Data16[GP, 70 + 50 * Stage_i + 4];
			ItemArr.TargetTorqueRate_DW_7 = (uint)(Data16[GP, 70 + 50 * Stage_i + 16 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 16]);
			ItemArr.AccelerationTime_9 = Data16[GP, 70 + 50 * Stage_i + 6];
			ItemArr.DecelerationTime_32 = Data16[GP, 70 + 50 * Stage_i + 7];
			if (ItemArr.RotationSpeed_3 > 0)
			{
				CommArr.HoldTimeSwitchOfFinalStage_22 = (ushort)((Data16[GP, 70 + 50 * Stage_i] & 0x80) >> 7);
			}
			if ((Data16[GP, 70 + 50 * Stage_i + 46] & 4) >> 2 == 1)
			{
				ItemArr.MaxAngle_10 = Data16[GP, 70 + 50 * Stage_i + 26];
				ItemArr.MinAngle_11 = Data16[GP, 70 + 50 * Stage_i + 27];
			}
			else
			{
				ItemArr.MaxAngle_10 = 0;
				ItemArr.MinAngle_11 = 0;
			}
			if ((Data16[GP, 70 + 50 * Stage_i + 47] & 1) == 1)
			{
				ItemArr.MaxTorque_DW_12 = (uint)(Data16[GP, 70 + 50 * Stage_i + 28 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 28]);
				ItemArr.MinTorque_DW_14 = (uint)(Data16[GP, 70 + 50 * Stage_i + 30 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 30]);
			}
			else
			{
				ItemArr.MaxTorque_DW_12 = 0u;
				ItemArr.MinTorque_DW_14 = 0u;
			}
			if ((Data16[GP, 70 + 50 * Stage_i + 47] & 0x8000) >> 15 == 1)
			{
				ItemArr.MaxOperationTime_16 = Data16[GP, 70 + 50 * Stage_i + 39];
				ItemArr.MinOperationTime_17 = Data16[GP, 70 + 50 * Stage_i + 40];
			}
			else
			{
				ItemArr.MaxOperationTime_16 = 0;
				ItemArr.MinOperationTime_17 = 0;
			}
			ItemArr.PrevailTorqueOnOff_18 = (ushort)((Data16[GP, 70 + 50 * Stage_i] & 0x200) >> 9);
			ItemArr.AngleRangeForPrevailTorqueCalc_19 = Data16[GP, 70 + 50 * Stage_i + 43];
			ItemArr.PauseTime_20 = Data16[GP, 70 + 50 * Stage_i + 5];
			if ((Data16[GP, 70 + 50 * Stage_i + 46] & 8) >> 3 == 1)
			{
				ItemArr.MaxClampTorque_DW_21 = (uint)(Data16[GP, 70 + 50 * Stage_i + 22 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 22]);
				ItemArr.MinClampTorque_DW_23 = (uint)(Data16[GP, 70 + 50 * Stage_i + 24 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 24]);
			}
			else
			{
				ItemArr.MaxClampTorque_DW_21 = 0u;
				ItemArr.MinClampTorque_DW_23 = 0u;
			}
			if ((Data16[GP, 70 + 50 * Stage_i + 46] & 0x20) >> 5 == 1)
			{
				ItemArr.MaxClampAngle_25 = Data16[GP, 70 + 50 * Stage_i + 41];
				ItemArr.MinClampAngle_26 = Data16[GP, 70 + 50 * Stage_i + 42];
			}
			else
			{
				ItemArr.MaxClampAngle_25 = 0;
				ItemArr.MinClampAngle_26 = 0;
			}
			if ((Data16[GP, 70 + 50 * Stage_i] & 0x2000) >> 13 == 1)
			{
				ItemArr.TargetTorque_1st_DW_27 = (uint)(Data16[GP, 70 + 50 * Stage_i + 18 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 18]);
				ItemArr.PauseTime_1st_29 = Data16[GP, 70 + 50 * Stage_i + 10];
				ItemArr.FinalAccelerationTime_30 = Data16[GP, 70 + 50 * Stage_i + 8];
				ItemArr.FinalRotationSpeed_31 = Data16[GP, 70 + 50 * Stage_i + 2];
			}
			else
			{
				ItemArr.TargetTorque_1st_DW_27 = 0u;
				ItemArr.PauseTime_1st_29 = 0;
				ItemArr.FinalAccelerationTime_30 = 0;
				ItemArr.FinalRotationSpeed_31 = 0;
			}
			ItemArr.AdvancedSetting_L_33 = (ushort)(Data16[GP, 70 + 50 * Stage_i + 1] & 7);
			ItemArr.AdvancedSetting_L_33 |= (ushort)((Data16[GP, 70 + 50 * Stage_i + 1] & 0x30) >> 1);
			ItemArr.AdvancedSetting_L_33 |= (ushort)((Data16[GP, 70 + 50 * Stage_i + 1] & 0x100) >> 3);
			ItemArr.AdvancedSetting_H_34 = 0;
			if ((Data16[GP, 70 + 50 * Stage_i + 46] & 0x40) >> 6 == 1)
			{
				ItemArr.MaxSwitchTorque_DW_35 = (uint)(Data16[GP, 70 + 50 * Stage_i + 32 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 32]);
				ItemArr.MinSwitchTorque_DW_37 = (uint)(Data16[GP, 70 + 50 * Stage_i + 34 + 1] * 65536 + Data16[GP, 70 + 50 * Stage_i + 34]);
			}
			else
			{
				ItemArr.MaxSwitchTorque_DW_35 = 0u;
				ItemArr.MinSwitchTorque_DW_37 = 0u;
			}
			if (ItemArr.ControlMode_1 == 5)
			{
				ItemArr.TargetYield_39 = Data16[GP, 8];
				ItemArr.StartTorqueOfYieldDetection_DW_40 = (uint)(Data16[GP, 29] * 65536 + Data16[GP, 28]);
			}
			else
			{
				ItemArr.TargetYield_39 = 0;
				ItemArr.StartTorqueOfYieldDetection_DW_40 = 0u;
			}
		}

		public void ChangeDefaultTorqUnit(ushort Type)
		{
			FSCtrlTorqUnit.Mode = Type;
			FSSrcAll.FSSrcManualX[0].TorqueUnit = FSCtrlTorqUnit.Mode;
			FSSrcAll.FSSrcManualY[0].TorqueUnit = FSCtrlTorqUnit.Mode;
			for (int i = 0; i < 255; i++)
			{
				FSSrcAll.FSSrcBitsX[i].TorqueUnit = FSCtrlTorqUnit.Mode;
				FSSrcAll.FSSrcBitsY[i].TorqueUnit = FSCtrlTorqUnit.Mode;
			}
			for (int j = 0; j < 500; j++)
			{
				FSSrcAll.FSSrcScannerX[j].TorqueUnit = FSCtrlTorqUnit.Mode;
				FSSrcAll.FSSrcScannerY[j].TorqueUnit = FSCtrlTorqUnit.Mode;
			}
			FSSrcAll.FSSrcManual_DualMix[0].TorqueUnit = FSCtrlTorqUnit.Mode;
			for (int k = 0; k < 255; k++)
			{
				FSSrcAll.FSSrcBits_DualMix[k].TorqueUnit = FSCtrlTorqUnit.Mode;
			}
			for (int l = 0; l < 500; l++)
			{
				FSSrcAll.FSSrcScanner_DualMix[l].TorqueUnit = FSCtrlTorqUnit.Mode;
			}
			FSSrcAll.FSSrcManual_DualSync[0].TorqueUnit = FSCtrlTorqUnit.Mode;
			for (int m = 0; m < 255; m++)
			{
				FSSrcAll.FSSrcBits_DualSync[m].TorqueUnit = FSCtrlTorqUnit.Mode;
			}
			for (int n = 0; n < 500; n++)
			{
				FSSrcAll.FSSrcScanner_DualSync[n].TorqueUnit = FSCtrlTorqUnit.Mode;
			}
			BackGroundRunningInfo();
		}

		public double TorqUnitcoef(int Mode)
		{
			int TorqUnit;
			switch (Mode)
			{
			case 0:
				TorqUnit = FSCtrlTorqUnit.Mode;
				break;
			case 1:
				TorqUnit = FSCtrlTorqUnit.Mode;
				break;
			case 2:
				TorqUnit = UISys.RunningSrcX.TorqueUnit;
				break;
			case 3:
				TorqUnit = UISys.RunningSrcY.TorqueUnit;
				break;
			case 1000:
				TorqUnit = 99;
				break;
			case 1001:
				TorqUnit = 1;
				break;
			case 1002:
				TorqUnit = 2;
				break;
			case 1003:
				TorqUnit = 3;
				break;
			case 1004:
				TorqUnit = 4;
				break;
			case 1005:
				TorqUnit = 5;
				break;
			case 1006:
				TorqUnit = 6;
				break;
			case 1050:
				TorqUnit = 50;
				break;
			default:
				TorqUnit = 99;
				break;
			}
			switch (TorqUnit)
			{
			case 1:
				return 10.197;
			case 2:
				return 0.737;
			case 3:
				return 8.849;
			case 4:
				return 11.801;
			case 5:
				return 141.612;
			case 6:
				return 100.0;
			case 50:
				return 50.0;
			default:
				return 1.0;
			}
		}

		public double Round(double InputVal, ushort Mode)
		{
			return InputVal;
		}

		public void ReadResultFile(ref ExReportStuc ReportStuc, uint ReportID)
		{
		}

		public void DefTightening2ndStageMode(ref ParamCommStucVer1 CurrComm, ref ParamItemStucVer1 CurrItem, bool SW)
		{
			if (SW)
			{
				CurrItem.TargetTorque_1st_DW_27 = ((CurrComm.HoldTimeSwitchOfFinalStage_22 == 1) ? ((uint)((double)CurrItem.TargetTorque_DW_4 * 0.25)) : ((uint)((double)CurrItem.TargetTorque_DW_4 * 0.4)));
				CurrItem.PauseTime_1st_29 = 0;
				CurrItem.FinalAccelerationTime_30 = 1000;
				CurrItem.FinalRotationSpeed_31 = CurrItem.RotationSpeed_3;
				ChangeAccDcc(ref CurrComm, ref CurrItem, true);
			}
			else
			{
				CurrItem.TargetTorque_1st_DW_27 = 0u;
				CurrItem.PauseTime_1st_29 = 0;
				CurrItem.FinalAccelerationTime_30 = 0;
				CurrItem.FinalRotationSpeed_31 = 0;
			}
		}

		public void ChangeClampTorqueULLL(ref ParamItemStucVer1 CurrItem)
		{
			double ClampTorqUL = (double)CurrItem.TargetTorque_DW_4 * 1.2;
			double ClampToolTorqUL = (double)(int)UISys.RunningToolMaxULTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint);
			if (ClampTorqUL >= ClampToolTorqUL)
			{
				CurrItem.MaxClampTorque_DW_21 = (uint)ClampToolTorqUL;
			}
			else
			{
				CurrItem.MaxClampTorque_DW_21 = (uint)ClampTorqUL;
			}
			CurrItem.MinClampTorque_DW_23 = 0u;
		}

		public void ChangeTorqueULLL(ref ParamItemStucVer1 CurrItem, bool Mode)
		{
			double TorqUL = 0.0;
			double ToolTorqUL = 0.0;
			if (!Mode)
			{
				TorqUL = (double)CurrItem.TargetTorque_DW_4 * 1.2;
				ToolTorqUL = (double)(int)UISys.RunningToolMaxULTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint);
			}
			else
			{
				TorqUL = (double)CurrItem.TargetTorque_DW_4 * 2.0;
				ToolTorqUL = (double)(int)UISys.RunningToolMaxTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint);
			}
			if (TorqUL >= ToolTorqUL)
			{
				CurrItem.MaxTorque_DW_12 = (uint)ToolTorqUL;
			}
			else
			{
				CurrItem.MaxTorque_DW_12 = (uint)TorqUL;
			}
			if (!Mode)
			{
				CurrItem.MinTorque_DW_14 = (uint)((double)CurrItem.TargetTorque_DW_4 * 0.8);
			}
			else
			{
				CurrItem.MinTorque_DW_14 = 0u;
			}
		}

		public void DefTightening2ndStageSpeed(ref ParamCommStucVer1 CurrComm, ref ParamItemStucVer1 CurrItem, bool FinalTorqMode)
		{
			if (FinalTorqMode && CurrItem.TargetTorque_1st_DW_27 != 0)
			{
				CurrItem.FinalRotationSpeed_31 = CurrItem.RotationSpeed_3;
				ChangeAccDcc(ref CurrComm, ref CurrItem, true);
			}
			else
			{
				CurrItem.FinalRotationSpeed_31 = 0;
			}
		}

		public void ChangeAccDcc(ref ParamCommStucVer1 CurrComm, ref ParamItemStucVer1 CurrItem, bool SW)
		{
			double Vel = (int)CurrItem.RotationSpeed_3;
			ushort CalTGACC = (ushort)(1000.0 / Vel * 100.0);
			if (!SW)
			{
				if (CalTGACC >= 1000)
				{
					CurrItem.AccelerationTime_9 = 1000;
				}
				else
				{
					CurrItem.AccelerationTime_9 = CalTGACC;
				}
			}
			else if (CurrComm.HoldTimeSwitchOfFinalStage_22 != 1)
			{
				if (CalTGACC >= 1000)
				{
					CurrItem.FinalAccelerationTime_30 = 1000;
				}
				else
				{
					CurrItem.FinalAccelerationTime_30 = CalTGACC;
				}
			}
		}

		public unsafe void ResultLedStatusFunc(int Axis, uint ScrewNo, ushort Status)
		{
			uint TotalProcessNum = ScrewNo - 1;
			if (TotalProcessNum >= 1000)
			{
				return;
			}
			switch (Status)
			{
			case 1:
				if (Axis == 0)
				{
					FSResultLedStatusX.Data16[TotalProcessNum] = 2;
				}
				else
				{
					FSResultLedStatusY.Data16[TotalProcessNum] = 2;
				}
				break;
			default:
				if (Status != 5)
				{
					break;
				}
				goto case 2;
			case 2:
				if (Axis == 0)
				{
					FSResultLedStatusX.Data16[TotalProcessNum] = 8;
				}
				else
				{
					FSResultLedStatusY.Data16[TotalProcessNum] = 8;
				}
				break;
			}
		}

		public unsafe void ResultLedStatusFunc(int Axis)
		{
			uint TotalProcessNum = UISys.List_Info.ScrewNo - 1;
			if (TotalProcessNum >= 1000)
			{
				return;
			}
			if (UISys.List_Info.Status == 1)
			{
				if (Axis == 0)
				{
					FSResultLedStatusX.Data16[TotalProcessNum] = 2;
				}
				else
				{
					FSResultLedStatusY.Data16[TotalProcessNum] = 2;
				}
			}
			else if (UISys.List_Info.Status == 2 || UISys.List_Info.Status == 5)
			{
				if (Axis == 0)
				{
					FSResultLedStatusX.Data16[TotalProcessNum] = 8;
				}
				else
				{
					FSResultLedStatusY.Data16[TotalProcessNum] = 8;
				}
			}
		}

		public bool CheckSrcOverRange(int SwitchingMethod, int GP)
		{
			bool OverRange = false;
			switch (SwitchingMethod)
			{
			case 0:
				OverRange = ((GP != 1) ? true : false);
				break;
			case 1:
				OverRange = ((GP < 1 || GP > 255) ? true : false);
				break;
			case 2:
				OverRange = ((GP < 1 || GP > 500) ? true : false);
				break;
			}
			return OverRange;
		}

		public uint FirstDetectPageAxis(ref DetectPageAxis Stuc)
		{
			Stuc.Page_Axis = 0u;
			if (FSModelTypeInfo.MesModelType == 0)
			{
				if (UISys.CtrlDualTool == 1)
				{
					if (FSToolXActive.ActiveEnable == 1)
					{
						Stuc.Page_Axis = 0u;
					}
					else if (FSToolYActive.ActiveEnable == 1)
					{
						Stuc.Page_Axis = 1u;
					}
					if (FSToolXActive.ActiveEnable == 1 && FSToolYActive.ActiveEnable == 0)
					{
						Stuc.Tool1Visable = false;
						Stuc.Tool2Visable = false;
					}
					else if (FSToolXActive.ActiveEnable == 0 && FSToolYActive.ActiveEnable == 1)
					{
						Stuc.Tool1Visable = false;
						Stuc.Tool2Visable = false;
					}
					else
					{
						Stuc.Tool1Visable = true;
						Stuc.Tool2Visable = true;
					}
				}
				else
				{
					Stuc.Page_Axis = 0u;
					Stuc.Tool1Visable = false;
					Stuc.Tool2Visable = false;
				}
			}
			else if (FSModelTypeInfo.MesModelType == 1)
			{
				Stuc.Page_Axis = 0u;
				Stuc.Tool1Visable = false;
				Stuc.Tool2Visable = false;
			}
			else
			{
				Stuc.Page_Axis = 1u;
				Stuc.Tool1Visable = false;
				Stuc.Tool2Visable = false;
			}
			return Stuc.Page_Axis;
		}

		public void SetModelNameType(int CtrlVer)
		{
			if (CtrlVer == 3 || CtrlVer == 1)
			{
				FSModelTypeInfo.MesModelType = 1;
			}
			else
			{
				FSModelTypeInfo.MesModelType = 0;
			}
		}

		public int ParamCheckSettingsRange(ref UIParamStrc UI)
		{
			int ErrCode = 0;
			UIMarvelClear();
			int Axis = UISys.ParamPageAxis;
			uint SetTGRunTime = UI.CurrComm.MaxTighteningTime_24;
			uint SetLOOSRunTime = UI.CurrComm.MaxLooseningTime_25;
			uint SetTGRunAngle = UI.CurrComm.MaxTighteningAngle_26;
			uint SetLOOSRunAngle = UI.CurrComm.MaxLooseningAngle_27;
			uint SetLinkPrevailTorq = UI.CurrComm.ThePrevailTorqueToBeLinked_23;
			double coef = TorqUnitcoef(2 + UISys.ParamPageAxis);
			uint ToolTorqueParamCoef;
			uint ToolMaxULTorqueParamCoef;
			ushort ToolMaxSpeed;
			if (Axis == 0)
			{
				ToolTorqueParamCoef = (uint)((double)(int)UISys.ToolMaxTorqueFW_X / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) * TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				ToolMaxULTorqueParamCoef = (uint)((double)(int)UISys.ToolMaxULTorqueFW_X / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) * TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				ToolMaxSpeed = UISys.ToolMaxSpeed_X;
				ushort ToolMinSpeed = UISys.ToolMinSpeed_X;
			}
			else
			{
				ToolTorqueParamCoef = (uint)((double)(int)UISys.ToolMaxTorqueFW_Y / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) * TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				ToolMaxULTorqueParamCoef = (uint)((double)(int)UISys.ToolMaxULTorqueFW_Y / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) * TorqUnitcoef(1000 + UI.CurrComm.TorqueUnit_30));
				ToolMaxSpeed = UISys.ToolMaxSpeed_Y;
				ushort ToolMinSpeed = UISys.ToolMinSpeed_Y;
			}
			uint PreTGMaxTorqParamCoef = ToolMaxULTorqueParamCoef;
			uint TGMaxTorqParamCoef = ToolMaxULTorqueParamCoef;
			uint RunDMMaxSpd = ToolMaxSpeed;
			uint PreTGMaxSpd = ToolMaxSpeed;
			if (SetTGRunTime > 32767)
			{
				UIMarvelSetBit(0, 6);
				ErrCode = 3132;
			}
			if (SetLOOSRunTime > 32767)
			{
				UIMarvelSetBit(1, 6);
				ErrCode = 3132;
			}
			if (SetTGRunAngle > 32767)
			{
				UIMarvelSetBit(0, 10);
				ErrCode = 3133;
			}
			for (int i = 0; i < 5; i++)
			{
				if (UI.CurrWAItem[i].RotationSpeed_3 == 0 && UI.CurrWAItem[i + 1].RotationSpeed_3 != 0)
				{
					UIMarvelSetBit(2 + i, 3);
					ErrCode = 3109;
				}
			}
			uint Useflag = 0u;
			uint Target = 0u;
			uint LastTarget = 0u;
			uint LastTargetYield = 0u;
			uint LastClamp = 0u;
			for (int j = 0; j <= 5; j++)
			{
				Target = UI.CurrWAItem[j].ControlMode_1;
				if (UI.CurrWAItem[j].RotationSpeed_3 > 0)
				{
					Useflag = 1u;
					if (Target == 2)
					{
						if (LastTarget == 2)
						{
							UIMarvelSetBit(2 + j, 3);
							ErrCode = 3140;
						}
						LastTarget = Target;
					}
					if (UISys.SpecCtrl == 1 && Target == 2)
					{
						ErrCode = 3140;
					}
					if (Target == 5)
					{
						if (LastTargetYield == 5)
						{
							UIMarvelSetBit(2 + j, 3);
							ErrCode = 3160;
						}
						LastTargetYield = Target;
					}
					if (UISys.SpecCtrl == 1 && Target == 5)
					{
						ErrCode = 3160;
					}
					if (Target == 3 || Target == 4)
					{
						if (LastClamp == 1)
						{
							UIMarvelSetBit(2 + j, 3);
							ErrCode = 3142;
						}
						LastClamp = 1u;
					}
					if (UI.CurrWAItem[j].RotationSpeed_3 > ToolMaxSpeed)
					{
						UIMarvelSetBit(2 + j, 4);
						ErrCode = 3120;
					}
				}
				if (UI.CurrWAItem[j].TargetAngle_6 > 32767 || UI.CurrWAItem[j].MaxAngle_10 > 32767 || UI.CurrWAItem[j].MinAngle_11 > 32767)
				{
					ErrCode = 3117;
				}
				if ((Target == 1 || Target == 3 || Target == 6) && UI.CurrWAItem[j].TargetTorque_DW_4 > ToolTorqueParamCoef)
				{
					UIMarvelSetBit(2 + j, 1);
					ErrCode = 3118;
				}
				if (UI.CurrWAItem[j].MaxTorque_DW_12 != 0)
				{
					if ((Target == 1 || Target == 3 || Target == 6) && UI.CurrWAItem[j].MinTorque_DW_14 > UI.CurrWAItem[j].TargetTorque_DW_4)
					{
						UIMarvelSetBit(2 + j, 1);
						UIMarvelSetBit(2 + j, 9);
						if (UI.CurrStrategy == 3)
						{
							ErrCode = 3161;
						}
						else if ((UI.CurrStrategy == 0 || UI.CurrStrategy == 2) && j == 1)
						{
							ErrCode = 3111;
						}
						else if (UI.CurrStrategy == 0 && j == 3)
						{
							ErrCode = 3115;
						}
						else if (UI.CurrStrategy == 1 && j == 0)
						{
							ErrCode = 3115;
						}
					}
					if ((Target == 1 || Target == 3 || Target == 6) && UI.CurrWAItem[j].TargetTorque_DW_4 > ToolTorqueParamCoef)
					{
						UIMarvelSetBit(2 + j, 1);
						UIMarvelSetBit(2 + j, 8);
						if (UI.CurrStrategy == 3)
						{
							ErrCode = 3162;
						}
						else if ((UI.CurrStrategy == 0 || UI.CurrStrategy == 2) && j == 1)
						{
							ErrCode = 3112;
						}
						else if (UI.CurrStrategy == 0 && j == 3)
						{
							ErrCode = 3116;
						}
						else if (UI.CurrStrategy == 1 && j == 0)
						{
							ErrCode = 3116;
						}
					}
					if (UI.CurrWAItem[j].MinTorque_DW_14 > ToolTorqueParamCoef)
					{
						UIMarvelSetBit(2 + j, 9);
						ErrCode = 3118;
					}
					if (UI.CurrWAItem[j].MinTorque_DW_14 > UI.CurrWAItem[j].MaxTorque_DW_12)
					{
						UIMarvelSetBit(2 + j, 8);
						UIMarvelSetBit(2 + j, 9);
						if (UI.CurrStrategy == 3)
						{
							ErrCode = 3163;
						}
						else if ((UI.CurrStrategy == 0 || UI.CurrStrategy == 2) && j == 0)
						{
							ErrCode = 3103;
						}
						else if ((UI.CurrStrategy == 0 || UI.CurrStrategy == 2) && j == 1)
						{
							ErrCode = 3103;
						}
						else if (UI.CurrStrategy == 0 && j == 3)
						{
							ErrCode = 3107;
						}
						else if (UI.CurrStrategy == 1 && j == 0)
						{
							ErrCode = 3107;
						}
					}
					if (UI.CurrWAItem[j].MaxTorque_DW_12 > ToolMaxULTorqueParamCoef)
					{
						UIMarvelSetBit(2 + j, 8);
						ErrCode = 3119;
					}
				}
				if ((UI.CurrWAItem[j].MaxAngle_10 > 0 || UI.CurrWAItem[j].MinAngle_11 > 0) && UI.CurrWAItem[j].MinAngle_11 > UI.CurrWAItem[j].MaxAngle_10)
				{
					UIMarvelSetBit(2 + j, 10);
					UIMarvelSetBit(2 + j, 11);
					if (UI.CurrStrategy == 3)
					{
						ErrCode = 3164;
					}
					else if ((UI.CurrStrategy == 0 || UI.CurrStrategy == 2) && j == 1)
					{
						ErrCode = 3104;
					}
					else if (UI.CurrStrategy == 0 && j == 2)
					{
						ErrCode = 3106;
					}
					else if (UI.CurrStrategy == 0 && j == 3)
					{
						ErrCode = 3108;
					}
					else if (UI.CurrStrategy == 1 && j == 0)
					{
						ErrCode = 3108;
					}
				}
				if (UI.CurrWAItem[j].MaxOperationTime_16 > 0 || UI.CurrWAItem[j].MinOperationTime_17 > 0)
				{
					if (UI.CurrWAItem[j].MinOperationTime_17 > UI.CurrWAItem[j].MaxOperationTime_16)
					{
						UIMarvelSetBit(2 + j, 6);
						UIMarvelSetBit(2 + j, 7);
						if (UI.CurrStrategy == 3)
						{
							ErrCode = 3165;
						}
						else if ((UI.CurrStrategy == 0 || UI.CurrStrategy == 2) && j == 0)
						{
							ErrCode = 3134;
						}
						else if ((UI.CurrStrategy == 0 || UI.CurrStrategy == 2) && j == 1)
						{
							ErrCode = 3135;
						}
						else if (UI.CurrStrategy == 0 && j == 2)
						{
							ErrCode = 3136;
						}
						else if (UI.CurrStrategy == 0 && j == 3)
						{
							ErrCode = 3137;
						}
						else if (UI.CurrStrategy == 1 && j == 0)
						{
							ErrCode = 3137;
						}
					}
					if (UI.CurrWAItem[j].MaxOperationTime_16 > UI.CurrComm.MaxTighteningTime_24 * 10)
					{
						UIMarvelSetBit(0, 6);
						UIMarvelSetBit(2 + j, 6);
						ErrCode = 3138;
					}
					if (UI.CurrWAItem[j].MinOperationTime_17 > UI.CurrComm.MaxTighteningTime_24 * 10)
					{
						UIMarvelSetBit(0, 6);
						UIMarvelSetBit(2 + j, 7);
						ErrCode = 3138;
					}
				}
				if (UI.CurrWAItem[j].MaxClampTorque_DW_21 != 0 || UI.CurrWAItem[j].MinClampTorque_DW_23 != 0)
				{
					if (Target == 3 && UI.CurrWAItem[j].TargetTorque_DW_4 > UI.CurrWAItem[j].MaxClampTorque_DW_21)
					{
						UIMarvelSetBit(2 + j, 1);
						UIMarvelSetBit(2 + j, 12);
						ErrCode = 3127;
					}
					if (UI.CurrWAItem[j].MinClampTorque_DW_23 > UI.CurrWAItem[j].MaxClampTorque_DW_21)
					{
						UIMarvelSetBit(2 + j, 12);
						UIMarvelSetBit(2 + j, 13);
						ErrCode = 3128;
					}
					if (UI.CurrWAItem[j].MaxClampTorque_DW_21 > ToolMaxULTorqueParamCoef)
					{
						UIMarvelSetBit(2 + j, 12);
						ErrCode = 3128;
					}
				}
				if ((UI.CurrWAItem[j].MaxClampAngle_25 > 0 || UI.CurrWAItem[j].MinClampAngle_26 > 0) && UI.CurrWAItem[j].MinClampAngle_26 > UI.CurrWAItem[j].MaxClampAngle_25)
				{
					UIMarvelSetBit(2 + j, 14);
					UIMarvelSetBit(2 + j, 15);
					ErrCode = 3141;
				}
				if (UI.CurrWAItem[j].MaxSwitchTorque_DW_35 != 0 || UI.CurrWAItem[j].MinSwitchTorque_DW_37 != 0)
				{
					if (UI.CurrWAItem[j].MinSwitchTorque_DW_37 > UI.CurrWAItem[j].MaxSwitchTorque_DW_35)
					{
						UIMarvelSetBit(2 + j, 16);
						ErrCode = 3143;
					}
					else if (UI.CurrWAItem[j].MaxSwitchTorque_DW_35 > ToolMaxULTorqueParamCoef)
					{
						UIMarvelSetBit(2 + j, 16);
						ErrCode = 3144;
					}
				}
				if ((Target == 1 || Target == 3 || Target == 6) && UI.CurrWAItem[j].TargetTorque_DW_4 == 0)
				{
					UIMarvelSetBit(2 + j, 1);
					ErrCode = 3121;
				}
			}
			if (UI.CurrLoos.FirstStageLooseningSpeed_2 > ToolMaxSpeed)
			{
			}
			if (UI.CurrLoos.SecondStageLooseningSpeed_4 > ToolMaxSpeed)
			{
			}
			if (Useflag == 0)
			{
				UIMarvelSetBit(2, 3);
				ErrCode = 3110;
			}
			if (UI.CurrStrategy == 0)
			{
				if (UI.CurrWAItem[0].RotationSpeed_3 > UI.CurrWAItem[1].RotationSpeed_3)
				{
					UIMarvelSetBit(2, 4);
					UIMarvelSetBit(3, 4);
					ErrCode = 3148;
				}
				if (UI.CurrWAItem[2].RotationSpeed_3 > UI.CurrWAItem[1].RotationSpeed_3)
				{
					UIMarvelSetBit(3, 4);
					UIMarvelSetBit(4, 4);
					ErrCode = 3149;
				}
				if (UI.CurrWAItem[3].RotationSpeed_3 > UI.CurrWAItem[1].RotationSpeed_3)
				{
					UIMarvelSetBit(3, 4);
					UIMarvelSetBit(5, 4);
					ErrCode = 3150;
				}
				if (UI.CurrWAItem[3].RotationSpeed_3 > UI.CurrWAItem[2].RotationSpeed_3)
				{
					UIMarvelSetBit(4, 4);
					UIMarvelSetBit(5, 4);
					ErrCode = 3147;
				}
			}
			else if (UI.CurrStrategy == 2 && UI.CurrWAItem[0].RotationSpeed_3 > UI.CurrWAItem[1].RotationSpeed_3)
			{
				UIMarvelSetBit(2, 4);
				UIMarvelSetBit(3, 4);
				ErrCode = 3148;
			}
			if (UI.CurrStrategy == 0)
			{
				if (UI.CurrWAItem[1].ControlMode_1 == 1 && UI.CurrWAItem[2].ControlMode_1 == 1 && UI.CurrWAItem[1].TargetTorque_DW_4 > UI.CurrWAItem[2].TargetTorque_DW_4)
				{
					UIMarvelSetBit(3, 1);
					ErrCode = 3124;
				}
				if (UI.CurrWAItem[1].ControlMode_1 == 1 && UI.CurrWAItem[3].ControlMode_1 == 1 && UI.CurrWAItem[1].TargetTorque_DW_4 > UI.CurrWAItem[3].TargetTorque_DW_4)
				{
					UIMarvelSetBit(3, 1);
					ErrCode = 3125;
				}
				if (UI.CurrWAItem[2].ControlMode_1 == 1 && UI.CurrWAItem[3].ControlMode_1 == 1 && UI.CurrWAItem[2].TargetTorque_DW_4 > UI.CurrWAItem[3].TargetTorque_DW_4)
				{
					UIMarvelSetBit(4, 1);
					ErrCode = 3126;
				}
			}
			if (UI.CurrStrategy == 0)
			{
				if (UI.CurrWAItem[3].ControlMode_1 == 1 && UI.CurrWAItem[0].MaxTorque_DW_12 != 0 && UI.CurrWAItem[0].MaxTorque_DW_12 > UI.CurrWAItem[3].TargetTorque_DW_4)
				{
					UIMarvelSetBit(2, 8);
					ErrCode = 3151;
				}
				if (UI.CurrWAItem[3].ControlMode_1 == 1 && UI.CurrWAItem[0].MaxTorque_DW_12 != 0 && UI.CurrWAItem[0].MinTorque_DW_14 > UI.CurrWAItem[3].TargetTorque_DW_4)
				{
					UIMarvelSetBit(2, 9);
					ErrCode = 3152;
				}
				if (UI.CurrWAItem[3].ControlMode_1 == 1 && UI.CurrWAItem[1].MaxTorque_DW_12 != 0 && UI.CurrWAItem[1].MaxTorque_DW_12 > UI.CurrWAItem[3].TargetTorque_DW_4)
				{
					UIMarvelSetBit(3, 8);
					ErrCode = 3153;
				}
				if (UI.CurrWAItem[3].ControlMode_1 == 1 && UI.CurrWAItem[1].MaxTorque_DW_12 != 0 && UI.CurrWAItem[1].MinTorque_DW_14 > UI.CurrWAItem[3].TargetTorque_DW_4)
				{
					UIMarvelSetBit(3, 9);
					ErrCode = 3154;
				}
			}
			UIMarveHeader(ref UI);
			return ErrCode;
		}

		public unsafe int SeqCheckSettingsRange(ref UISeqStrc UI, int CellMaxCount)
		{
			int ErrCode = 0;
			uint TotalScrew = 0u;
			if (CellMaxCount == 0)
			{
				ErrCode = 3181;
			}
			else
			{
				for (int i = 0; i < CellMaxCount; i++)
				{
					TotalScrew += UI.CurrSeq.ScrewQuantityforSet[i];
					if (UI.CurrSeq.ScrewQuantityforSet[i] == 0)
					{
						ErrCode = 3182;
						break;
					}
					if (TotalScrew >= 999999)
					{
						ErrCode = 3183;
						break;
					}
				}
			}
			return ErrCode;
		}

		public unsafe void ExParamCalu(uint Axis, uint GP, ushort Strategy, ushort ToolSpec, ushort CtrlVer, uint RotationSpeed_3)
		{
			DateTime StartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			DateTime NowDate = DateTime.UtcNow;
			TimeSpan timeDifference = NowDate - StartDate;
			uint days = (uint)(timeDifference.TotalSeconds / 86400.0);
			uint seconds = (uint)timeDifference.TotalSeconds - days * 24 * 60 * 60;
			if (Axis == 0)
			{
				ExFSParamX.Strategy[GP] = Strategy;
				ExFSParamX.ToolSpec[GP] = ToolSpec;
				ExFSParamX.EnableGP[GP] = (ushort)((RotationSpeed_3 != 0) ? 1 : 0);
				ExFSParamX.CtrlVer[GP] = CtrlVer;
				ExFSParamX.YYMMDD[GP] = (ushort)days;
				ExFSParamX.HHMMSS[GP] = seconds;
			}
			else
			{
				ExFSParamY.Strategy[GP] = Strategy;
				ExFSParamY.ToolSpec[GP] = ToolSpec;
				ExFSParamY.EnableGP[GP] = (ushort)((RotationSpeed_3 != 0) ? 1 : 0);
				ExFSParamY.CtrlVer[GP] = CtrlVer;
				ExFSParamY.YYMMDD[GP] = (ushort)days;
				ExFSParamY.HHMMSS[GP] = seconds;
			}
		}

		public unsafe void ExSeqCalu(int GP)
		{
			uint ParamQty = 0u;
			for (int i = 0; i < 100; i++)
			{
				if (FSSeqGB[GP].ParameterIDForSet[i] != 0)
				{
					ParamQty++;
				}
			}
			ExFSSeq.ParamQty[GP] = (ushort)ParamQty;
			uint TotalCounter = 0u;
			for (int j = 0; j < 100; j++)
			{
				TotalCounter += FSSeqGB[GP].ScrewQuantityforSet[j];
			}
			ExFSSeq.TotalCounter[GP] = TotalCounter;
			int EnableMode = 0;
			for (int k = 0; k < 100; k++)
			{
				if (FSSeqGB[GP].ParameterIDForSet[k] != 0)
				{
					if (EnableMode == 0)
					{
						EnableMode = ((FSSeqGB[GP].ToolIDForSet[k] == 0) ? 1 : 2);
					}
					else if (EnableMode != FSSeqGB[GP].ToolIDForSet[k] + 1)
					{
						EnableMode = 3;
					}
				}
			}
			int BatchNum = 0;
			for (int l = 0; l < 100; l++)
			{
				if (ExFSSeq.ParamQty[GP] > 0)
				{
					BatchNum |= FSSeqGB[GP].BitIDForSet[l];
				}
			}
			if (ExFSSeq.ParamQty[GP] > 0 && BatchNum == FSSeqGB[GP].BitIDForSet[0])
			{
				EnableMode += 10;
			}
			ExFSSeq.EnableMode[GP] = (ushort)EnableMode;
		}

		public void BackGroundRunningInfo()
		{
			UISys.RunningSrcMode = FSSrcMode;
			if (UISys.RunningSrcMode.ActionMode == 0)
			{
				if (UISys.RunningSrcMode.SwitchingMethodX == 0)
				{
					UISys.RunningSrcX = FSSrcAll.FSSrcManualX[0];
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				else if (UISys.RunningSrcMode.SwitchingMethodX == 1)
				{
					if (TcpStatus.Detail.T1StA.TighteningIDset_00 > 0 && TcpStatus.Detail.T1StA.TighteningIDset_00 <= 255)
					{
						UISys.RunningSrcX = FSSrcAll.FSSrcBitsX[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
					}
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				else if (UISys.RunningSrcMode.SwitchingMethodX == 2)
				{
					if (TcpStatus.Detail.T1StA.TighteningIDset_00 > 0 && TcpStatus.Detail.T1StA.TighteningIDset_00 <= 500)
					{
						UISys.RunningSrcX = FSSrcAll.FSSrcScannerX[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
					}
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				if (UISys.RunningSrcMode.SwitchingMethodY == 0)
				{
					UISys.RunningSrcY = FSSrcAll.FSSrcManualY[0];
					if (TcpStatus.Detail.T2StA.ParamID_03 == 0)
					{
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				else if (UISys.RunningSrcMode.SwitchingMethodY == 1)
				{
					if (TcpStatus.Detail.T2StA.TighteningIDset_00 > 0 && TcpStatus.Detail.T2StA.TighteningIDset_00 <= 255)
					{
						UISys.RunningSrcY = FSSrcAll.FSSrcBitsY[TcpStatus.Detail.T2StA.TighteningIDset_00 - 1];
					}
					if (TcpStatus.Detail.T2StA.ParamID_03 == 0)
					{
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				else if (UISys.RunningSrcMode.SwitchingMethodY == 2)
				{
					if (TcpStatus.Detail.T2StA.TighteningIDset_00 > 0 && TcpStatus.Detail.T2StA.TighteningIDset_00 <= 500)
					{
						UISys.RunningSrcY = FSSrcAll.FSSrcScannerY[TcpStatus.Detail.T2StA.TighteningIDset_00 - 1];
					}
					if (TcpStatus.Detail.T2StA.ParamID_03 == 0)
					{
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
			}
			else if (UISys.RunningSrcMode.ActionMode == 1)
			{
				if (UISys.RunningSrcMode.SwitchingMethodX == 0)
				{
					UISys.RunningSrcX = FSSrcAll.FSSrcManual_DualMix[0];
					UISys.RunningSrcY = FSSrcAll.FSSrcManual_DualMix[0];
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				else if (UISys.RunningSrcMode.SwitchingMethodX == 2)
				{
					if (TcpStatus.Detail.T1StA.TighteningIDset_00 > 0 && TcpStatus.Detail.T1StA.TighteningIDset_00 <= 500)
					{
						UISys.RunningSrcX = FSSrcAll.FSSrcScanner_DualMix[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
						UISys.RunningSrcY = FSSrcAll.FSSrcScanner_DualMix[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
					}
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
			}
			else if (UISys.RunningSrcMode.ActionMode == 2)
			{
				if (UISys.RunningSrcMode.SwitchingMethodX == 0)
				{
					UISys.RunningSrcX = FSSrcAll.FSSrcManual_DualSync[0];
					UISys.RunningSrcY = FSSrcAll.FSSrcManual_DualSync[0];
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				else if (UISys.RunningSrcMode.SwitchingMethodX == 1)
				{
					if (TcpStatus.Detail.T1StA.TighteningIDset_00 > 0 && TcpStatus.Detail.T1StA.TighteningIDset_00 <= 255)
					{
						UISys.RunningSrcX = FSSrcAll.FSSrcBits_DualSync[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
						UISys.RunningSrcY = FSSrcAll.FSSrcBits_DualSync[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
					}
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
				else if (UISys.RunningSrcMode.SwitchingMethodX == 2)
				{
					if (TcpStatus.Detail.T1StA.TighteningIDset_00 > 0 && TcpStatus.Detail.T1StA.TighteningIDset_00 <= 500)
					{
						UISys.RunningSrcX = FSSrcAll.FSSrcScanner_DualSync[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
						UISys.RunningSrcY = FSSrcAll.FSSrcScanner_DualSync[TcpStatus.Detail.T1StA.TighteningIDset_00 - 1];
					}
					if (TcpStatus.Detail.T1StA.ParamID_03 == 0)
					{
						UISys.RunningSrcX.TorqueUnit = FSCtrlTorqUnit.Mode;
						UISys.RunningSrcY.TorqueUnit = FSCtrlTorqUnit.Mode;
					}
				}
			}
			if (FSSrcMode.ActionMode == 0)
			{
				if (TcpStatus.Detail.T1StA.ParamSeqSet_01 == 0)
				{
					if (TcpStatus.Detail.T1StA.ParamID_03 > 0)
					{
						UISys.RunningParamX = FSParamX[TcpStatus.Detail.T1StA.ParamID_03 - 1];
					}
					else
					{
						UISys.RunningParamX = default(ParamStucVer1);
					}
					UISys.RunningSeqX = default(SeqBaseStuc);
				}
				else
				{
					if (TcpStatus.Detail.T1StA.ParamID_03 > 0)
					{
						UISys.RunningParamX = FSParamX[TcpStatus.Detail.T1StA.ParamID_03 - 1];
					}
					else
					{
						UISys.RunningParamX = default(ParamStucVer1);
					}
					if (TcpStatus.Detail.T1StA.SeqID_02 > 0)
					{
						UISys.RunningSeqX = FSSeqGB[TcpStatus.Detail.T1StA.SeqID_02 - 1];
					}
					else
					{
						UISys.RunningSeqX = default(SeqBaseStuc);
					}
				}
				if (TcpStatus.Detail.T2StA.ParamSeqSet_01 == 0)
				{
					if (TcpStatus.Detail.T2StA.ParamID_03 > 0)
					{
						UISys.RunningParamY = FSParamY[TcpStatus.Detail.T2StA.ParamID_03 - 1];
					}
					else
					{
						UISys.RunningParamY = default(ParamStucVer1);
					}
					UISys.RunningSeqY = default(SeqBaseStuc);
				}
				else
				{
					if (TcpStatus.Detail.T2StA.ParamID_03 > 0)
					{
						UISys.RunningParamY = FSParamY[TcpStatus.Detail.T2StA.ParamID_03 - 1];
					}
					else
					{
						UISys.RunningParamY = default(ParamStucVer1);
					}
					if (TcpStatus.Detail.T2StA.SeqID_02 > 0)
					{
						UISys.RunningSeqY = FSSeqGB[TcpStatus.Detail.T2StA.SeqID_02 - 1];
					}
					else
					{
						UISys.RunningSeqY = default(SeqBaseStuc);
					}
				}
			}
			else
			{
				if (TcpStatus.Detail.Comm.TheRunningToolNumberInDualTool_25 == 0)
				{
					if (TcpStatus.Detail.T1StA.ParamID_03 > 0)
					{
						UISys.RunningParamX = FSParamX[TcpStatus.Detail.T1StA.ParamID_03 - 1];
						UISys.RunningParamY = FSParamX[TcpStatus.Detail.T1StA.ParamID_03 - 1];
					}
					else
					{
						UISys.RunningParamX = default(ParamStucVer1);
						UISys.RunningParamY = default(ParamStucVer1);
					}
				}
				else if (TcpStatus.Detail.T2StA.ParamID_03 > 0)
				{
					UISys.RunningParamX = FSParamY[TcpStatus.Detail.T2StA.ParamID_03 - 1];
					UISys.RunningParamY = FSParamY[TcpStatus.Detail.T2StA.ParamID_03 - 1];
				}
				else
				{
					UISys.RunningParamX = default(ParamStucVer1);
					UISys.RunningParamY = default(ParamStucVer1);
				}
				if (TcpStatus.Detail.T1StA.SeqID_02 > 0)
				{
					UISys.RunningSeqX = FSSeqGB[TcpStatus.Detail.T1StA.SeqID_02 - 1];
					UISys.RunningSeqY = FSSeqGB[TcpStatus.Detail.T1StA.SeqID_02 - 1];
				}
				else
				{
					UISys.RunningSeqX = default(SeqBaseStuc);
					UISys.RunningSeqY = default(SeqBaseStuc);
				}
			}
			if (UISys.PCSoftSupport)
			{
				DefToolType(0, 0);
				DefToolType(1, 0);
			}
		}

		public Image LoadPicture(string Path)
		{
			Image Img = null;
			try
			{
				using (FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					using (Image image = Image.FromStream(fs))
					{
						Img = new Bitmap(image);
					}
				}
			}
			catch (Exception)
			{
			}
			return Img;
		}

		public Bitmap DrawNumber(string IDstr, Bitmap bitmap)
		{
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.DrawString(IDstr, new Font("Arial", 80f), new SolidBrush(Color.Black), new PointF(60f, 60f));
			}
			return bitmap;
		}

		public string ReadLine(string FilePath, int LineNumber)
		{
			string result = "";
			try
			{
				if (File.Exists(FilePath))
				{
					using (StreamReader _StreamReader = new StreamReader(FilePath))
					{
						for (int a = 0; a < LineNumber; a++)
						{
							result = _StreamReader.ReadLine();
						}
					}
				}
			}
			catch
			{
				Console.WriteLine("CSV Error");
			}
			return result;
		}

		public int DefCtrlTable(bool Offline, int Type)
		{
			int RetVal = 0;
			if (Offline)
			{
				switch (Type)
				{
				case 0:
					UISys.PM101 = 1;
					UISys.CtrlDualTool = 0;
					break;
				case 1:
					UISys.PM101 = 3;
					UISys.CtrlDualTool = 0;
					break;
				case 2:
					UISys.PM101 = 0;
					UISys.CtrlDualTool = 0;
					break;
				case 3:
					UISys.PM101 = 0;
					UISys.CtrlDualTool = 1;
					break;
				case 4:
					UISys.PM101 = 4;
					UISys.CtrlDualTool = 0;
					break;
				}
				SetModelNameType(UISys.PM101);
			}
			else if (UISys.PM101 == 1)
			{
				RetVal = 0;
			}
			else if (UISys.PM101 == 3)
			{
				RetVal = 1;
			}
			else if (UISys.PM101 == 4)
			{
				RetVal = 4;
			}
			else if (FSModelTypeInfo.MesModelType == 0 && UISys.CtrlDualTool == 0)
			{
				RetVal = 2;
			}
			else if (FSModelTypeInfo.MesModelType == 0 && UISys.CtrlDualTool == 1)
			{
				RetVal = 3;
			}
			return RetVal;
		}

		public int DefToolTable(bool Offline, int Axis, int Type)
		{
			int RetVal = 0;
			if (Offline)
			{
				int MaxTorqueNm = 0;
				if (UISys.PM101 == 0 || UISys.PM101 == 2 || UISys.PM101 == 3)
				{
					switch (Type)
					{
					case 4:
						MaxTorqueNm = 7500;
						break;
					case 3:
						MaxTorqueNm = 5000;
						break;
					case 2:
						MaxTorqueNm = 3000;
						break;
					case 1:
						MaxTorqueNm = 1200;
						break;
					default:
						MaxTorqueNm = 9999;
						break;
					}
				}
				else if (UISys.PM101 == 1)
				{
					switch (Type)
					{
					case 4:
						MaxTorqueNm = 350;
						break;
					case 3:
						MaxTorqueNm = 200;
						break;
					case 2:
						MaxTorqueNm = 130;
						break;
					case 1:
						MaxTorqueNm = 100;
						break;
					default:
						MaxTorqueNm = 9999;
						break;
					}
				}
				else if (UISys.PM101 == 4)
				{
					switch (Type)
					{
					case 3:
						MaxTorqueNm = 25000;
						break;
					case 2:
						MaxTorqueNm = 17000;
						break;
					case 1:
						MaxTorqueNm = 12000;
						break;
					default:
						MaxTorqueNm = 9999;
						break;
					}
				}
				DefToolType(Axis, MaxTorqueNm);
			}
			else
			{
				uint MaxTorqueNm2 = 0u;
				MaxTorqueNm2 = ((Axis != 0) ? UISys.ToolTorqueSpec_Y : UISys.ToolTorqueSpec_X);
				if (UISys.PM101 == 0 || UISys.PM101 == 2 || UISys.PM101 == 3)
				{
					switch (MaxTorqueNm2)
					{
					case 7500u:
						RetVal = 4;
						break;
					case 5000u:
						RetVal = 3;
						break;
					case 3000u:
						RetVal = 2;
						break;
					case 1200u:
						RetVal = 1;
						break;
					default:
						RetVal = 0;
						break;
					}
				}
				else if (UISys.PM101 == 1)
				{
					switch (MaxTorqueNm2)
					{
					case 350u:
						RetVal = 4;
						break;
					case 200u:
						RetVal = 3;
						break;
					case 130u:
						RetVal = 2;
						break;
					case 100u:
						RetVal = 1;
						break;
					default:
						RetVal = 0;
						break;
					}
				}
				else if (UISys.PM101 == 4)
				{
					switch (MaxTorqueNm2)
					{
					case 25000u:
						RetVal = 3;
						break;
					case 17000u:
						RetVal = 2;
						break;
					case 12000u:
						RetVal = 1;
						break;
					default:
						RetVal = 0;
						break;
					}
				}
			}
			return RetVal;
		}

		public void DefToolType(int Axis, int Mode)
		{
			ushort MaxSpeed = 0;
			ushort MinSpeed = 0;
			ushort ToolMaxULTorqueFW = 0;
			ushort ToolMaxTorqueFW = 0;
			ushort ToolSetTorqueFW = 0;
			ushort ToolMinTorqueFW = 0;
			ushort ToolTorqueSpecVal = 0;
			switch (Mode)
			{
			case 25000:
				MaxSpeed = 400;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(30000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(25000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(5000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(5000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 25000;
				break;
			case 17000:
				MaxSpeed = 800;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(20400.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(17000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(3400.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(3400.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 17000;
				break;
			case 12000:
				MaxSpeed = 1000;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(14400.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(12000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(2400.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(2400.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 12000;
				break;
			case 7500:
				MaxSpeed = 500;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(8700.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(7500.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(1500.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(1500.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 7500;
				break;
			case 5000:
				MaxSpeed = 700;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(6000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(5000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(1000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(1000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 5000;
				break;
			case 3000:
				MaxSpeed = 1100;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(3500.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(3000.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(600.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(600.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 3000;
				break;
			case 1200:
				MaxSpeed = 2000;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(1400.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(1200.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(240.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(240.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 1200;
				break;
			case 350:
				MaxSpeed = 1000;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(420.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(350.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(70.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(70.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 350;
				break;
			case 200:
				MaxSpeed = 1000;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(240.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(200.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(40.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(40.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 200;
				break;
			case 130:
				MaxSpeed = 1000;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(156.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(130.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(26.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(26.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 100;
				break;
			case 100:
				MaxSpeed = 1000;
				MinSpeed = 10;
				ToolMaxULTorqueFW = (ushort)(120.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMaxTorqueFW = (ushort)(100.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolSetTorqueFW = (ushort)(20.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolMinTorqueFW = (ushort)(20.0 * TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint));
				ToolTorqueSpecVal = 100;
				break;
			case 0:
				if (Axis == 0)
				{
					MaxSpeed = FSToolXModelInfo.MaxVel;
					MinSpeed = FSToolXModelInfo.MinVel;
					ToolMaxULTorqueFW = FSToolXModelInfo.MaxULTorque;
					ToolMaxTorqueFW = FSToolXModelInfo.MaxTorque;
					ToolSetTorqueFW = FSToolXModelInfo.MinTorqueDef;
					ToolMinTorqueFW = FSToolXModelInfo.MinTorque;
					ToolTorqueSpecVal = FSToolXModelInfo.ToolTorque_Nm;
				}
				else
				{
					MaxSpeed = FSToolYModelInfo.MaxVel;
					MinSpeed = FSToolYModelInfo.MinVel;
					ToolMaxULTorqueFW = FSToolYModelInfo.MaxULTorque;
					ToolMaxTorqueFW = FSToolYModelInfo.MaxTorque;
					ToolSetTorqueFW = FSToolYModelInfo.MinTorqueDef;
					ToolMinTorqueFW = FSToolYModelInfo.MinTorque;
					ToolTorqueSpecVal = FSToolYModelInfo.ToolTorque_Nm;
				}
				break;
			default:
				MaxSpeed = 0;
				MinSpeed = 0;
				ToolMaxULTorqueFW = 0;
				ToolMaxTorqueFW = 0;
				ToolSetTorqueFW = 0;
				ToolMinTorqueFW = 0;
				ToolTorqueSpecVal = 0;
				break;
			}
			if (Axis == 0)
			{
				UISys.ToolMaxSpeed_X = MaxSpeed;
				UISys.ToolMinSpeed_X = MinSpeed;
				UISys.ToolMaxULTorqueFW_X = ToolMaxULTorqueFW;
				UISys.ToolMaxTorqueFW_X = ToolMaxTorqueFW;
				UISys.ToolSetTorqueFW_X = ToolSetTorqueFW;
				UISys.ToolMinTorqueFW_X = ToolMinTorqueFW;
				UISys.ToolTorqueSpec_X = ToolTorqueSpecVal;
			}
			else
			{
				UISys.ToolMaxSpeed_Y = MaxSpeed;
				UISys.ToolMinSpeed_Y = MinSpeed;
				UISys.ToolMaxULTorqueFW_Y = ToolMaxULTorqueFW;
				UISys.ToolMaxTorqueFW_Y = ToolMaxTorqueFW;
				UISys.ToolSetTorqueFW_Y = ToolSetTorqueFW;
				UISys.ToolMinTorqueFW_Y = ToolMinTorqueFW;
				UISys.ToolTorqueSpec_Y = ToolTorqueSpecVal;
			}
			DetectCtrlMode();
		}

		public void DetectCtrlMode()
		{
			if (UISys.ToolTorqueSpec_X < 1200)
			{
				UISys.ToolX_ModelType = 0;
			}
			else if (UISys.ToolTorqueSpec_X > 7500)
			{
				UISys.ToolX_ModelType = 2;
			}
			else
			{
				UISys.ToolX_ModelType = 1;
			}
			if (UISys.ToolTorqueSpec_Y < 1200)
			{
				UISys.ToolY_ModelType = 0;
			}
			else if (UISys.ToolTorqueSpec_Y > 7500)
			{
				UISys.ToolY_ModelType = 2;
			}
			else
			{
				UISys.ToolY_ModelType = 1;
			}
			UISys.NonPushStartTypeX = (ushort)((UISys.ToolX_ModelType == 0 || (UISys.ToolX_ModelType == 1 && FSModelTypeInfo.ToolModel1Type == 2) || (UISys.ToolX_ModelType == 2 && (FSModelTypeInfo.ToolModel1Type == 2 || FSModelTypeInfo.ToolModel1Type == 3))) ? 1 : 0);
			UISys.NonPushStartTypeY = (ushort)((UISys.ToolY_ModelType == 0 || (UISys.ToolY_ModelType == 1 && FSModelTypeInfo.ToolModel2Type == 2) || (UISys.ToolY_ModelType == 2 && (FSModelTypeInfo.ToolModel2Type == 2 || FSModelTypeInfo.ToolModel2Type == 3))) ? 1 : 0);
			UISys.NonLightBrightX = (ushort)((UISys.ToolX_ModelType == 0 || UISys.ToolX_ModelType == 2) ? 1 : 0);
			UISys.NonLightBrightY = (ushort)((UISys.ToolY_ModelType == 0 || UISys.ToolY_ModelType == 2) ? 1 : 0);
			FSCtrlStartCondition.Mode = (ushort)((UISys.NonPushStartTypeX == 1 || UISys.NonPushStartTypeY == 1) ? 2 : 3);
		}

		public bool IsDetectFinalStage(ref UIParamStrc UI)
		{
			if (UI.CurrStageID < 5)
			{
				return UI.CurrWAItem[UI.CurrStageID + 1].RotationSpeed_3 == 0;
			}
			return true;
		}

		public unsafe int ResultDectectSeqStatus(int Axis, uint ScrewNumNo, uint ScrewCounterSize)
		{
			int ResultLEDStatus = 0;
			for (uint n = ScrewNumNo; n < ScrewNumNo + ScrewCounterSize && n < 1000; n++)
			{
				if (Axis == 0)
				{
					if (FSResultLedStatusX.Data16[n] == 8)
					{
						ResultLEDStatus = 1;
						break;
					}
				}
				else if (FSResultLedStatusY.Data16[n] == 8)
				{
					ResultLEDStatus = 1;
					break;
				}
			}
			if (ResultLEDStatus == 1)
			{
				return 8;
			}
			return 2;
		}

		public unsafe int ResultLedST(int Axis, int n)
		{
			if (n >= 1000)
			{
				return 2;
			}
			return (Axis == 0) ? FSResultLedStatusX.Data16[n] : FSResultLedStatusY.Data16[n];
		}

		public string ALWNNumberStr(uint ErrCode)
		{
			string ALStr = "";
			if (ErrCode < 12288)
			{
				ALStr = "AL ";
			}
			else if (ErrCode < 20480)
			{
				ALStr = "NG ";
			}
			else if (ErrCode < 28672)
			{
				ALStr = "WN ";
			}
			return ALStr + ErrCode.ToString("X4");
		}

		public string ALWNTitleStr(uint ErrCode)
		{
			string ALStr = "";
			ushort CodeFFFF = (ushort)ErrCode;
			ushort CodeF000 = (ushort)(ErrCode & 0xF000);
			ushort Code0F00 = (ushort)(ErrCode & 0xF00);
			ushort Code00FF = (ushort)(ErrCode & 0xFF);
			ushort Code0FFF = (ushort)(ErrCode & 0xFFF);
			if ((CodeFFFF >= 12544 && CodeFFFF < 16384) || (CodeFFFF >= 16640 && CodeFFFF < 20480))
			{
				string ALHearStr = "";
				return string.Concat(str1: (Code00FF != 17) ? MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_AL" + (Code00FF + 12544).ToString("X4")) : ProtectChangeName(CodeFFFF), str0: MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_Stage" + (Code0F00 >> 8).ToString("X1")), str2: "    ");
			}
			if ((CodeFFFF >= 12288 && CodeFFFF < 12544) || (CodeFFFF >= 16384 && CodeFFFF < 16640))
			{
				return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_AL" + (Code00FF + 12288).ToString("X4")) + "    ";
			}
			if (CodeFFFF >= 20480 && CodeFFFF < 28672)
			{
				return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_AL5" + Code0FFF.ToString("X3")) + "    ";
			}
			return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_AL1" + Code0FFF.ToString("X3")) + "    ";
		}

		public unsafe string ProtectChangeName(ushort ALNum)
		{
			string Str = "";
			ushort ST = 3;
			ushort ID = 1;
			ushort Stage = (ushort)((ALNum & 0xF00) >> 8);
			if (FSModelTypeInfo.MesModelType == 1)
			{
				ID = (ushort)((TcpStatus.Detail.T1StA.ParamID_03 > 500) ? 1 : TcpStatus.Detail.T1StA.ParamID_03);
				ST = ExFSParamY.Strategy[(int)ID];
			}
			else
			{
				if ((ALNum & 0xF000) == 12288)
				{
					ID = (ushort)((TcpStatus.Detail.T1StA.ParamID_03 > 500) ? 1 : TcpStatus.Detail.T1StA.ParamID_03);
					ST = ExFSParamX.Strategy[(int)ID];
				}
				if ((ALNum & 0xF000) == 16384)
				{
					ID = (ushort)((TcpStatus.Detail.T2StA.ParamID_03 > 500) ? 1 : TcpStatus.Detail.T2StA.ParamID_03);
					ST = ExFSParamY.Strategy[(int)ID];
				}
			}
			switch (ST)
			{
			case 0:
				switch (Stage)
				{
				case 4:
					return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ChageAL2");
				default:
					if (Stage != 2)
					{
						if (Stage == 3)
						{
							return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ChageAL4");
						}
						return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ChageAL1");
					}
					goto case 1;
				case 1:
					return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ChageAL3");
				}
			case 1:
				if (Stage == 1)
				{
					return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ChageAL2");
				}
				return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ChageAL1");
			default:
				return MultiLanguage.GetStr("Form999_ErrorWarningMsg", "tp_ChageAL1");
			}
		}

		public void RangeTitle(object sender, KeyPressEventArgs e)
		{
			if (((TextBox)sender).Text.Length >= 40)
			{
				e.Handled = true;
			}
			if (e.KeyChar > ' ')
			{
				try
				{
					double.Parse(((TextBox)sender).Text + e.KeyChar);
				}
				catch
				{
					e.KeyChar = '\0';
				}
			}
		}

		public void RangeASCIIInput(object sender, KeyPressEventArgs e)
		{
			if ((e.KeyChar < ' ' || e.KeyChar > '\u007f') && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
		}

		public void RangeUnsigned65_535(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "65.535", "0.000");
		}

		public void RangeUnsigned6553_5(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "6553.5", "0.0");
		}

		public void RangeUnsigned32_767(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "32.767", "0.000");
		}

		public void RangeUnsigned327_67(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "327.67", "0.00");
		}

		public void RangeUnsigned3276_7(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "3276.7", "0.0");
		}

		public void RangeUnsigned65_00(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "65.00", "0.00");
		}

		public void RangeUnsigned5000_100(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "5000", "100");
		}

		public void RangeUnsigned200(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "200", "1");
		}

		public void RangeUnsigned6000(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "6000", "1");
		}

		public void RangeUnsigned20000(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "20000", "1");
		}

		public void RangeUnsigned4294967295(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "4294967295", "0");
		}

		public void RangeUnsigned2147483647(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "2147483647", "-2147483648");
		}

		public void RangeUnsigned999998(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "999998", "0");
		}

		public void RangeUnsigned999999(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "999999", "0");
		}

		public void RangeUnsigned9999999(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "9999999", "0");
		}

		public void RangeUnsigned65535(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "65535", "0");
		}

		public void RangeUnsigned32767(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "32767", "0");
		}

		public void RangeUnsigned255(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "255", "0");
		}

		public void RangeUnsigned3000(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "3000", "0");
		}

		public void RangeSigned360(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "360", "-360");
		}

		public void RangeSigned327_67(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "327.67", "-327.68");
		}

		public void RangeUnsigned91_020(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "91.019", "0");
		}

		public void RangeSigned10_0(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "10.0", "-10.0");
		}

		public void RangeSigned50_0(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "50.0", "-50.0");
		}

		public void RangeUnsigned6_0000(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "6.0000", "0.0000");
		}

		public void RangeUnsigned600_0(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "600.0", "0.0");
		}

		public void RangeUnsigned300_0(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "300.0", "0.1");
		}

		public void RangeUnsigned1000(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "1000", "0");
		}

		public void RangeUnsigned100(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "100", "0");
		}

		public void RangeUnsigned180(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "180", "0");
		}

		public void RangeUnsigned360(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "360", "0");
		}

		public void RangeUnsigned50(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "50", "0");
		}

		public void RangeUnsigned500(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "500", "0");
		}

		public void RangeUnsigned5000(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "5000", "0");
		}

		public void RangeUnsigned200to1(object sender, KeyPressEventArgs e)
		{
			RangeSignedValue(sender, e, "200", "1");
		}

		public double ToolMaxTorqueWatchUnit()
		{
			return (double)(int)UISys.RunningToolMaxULTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) / 1000.0;
		}

		public double ToolTorqueWatchUnit()
		{
			return (double)(int)UISys.RunningToolMaxTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) / 1000.0;
		}

		public void RangeToolTorque_000(object sender, KeyPressEventArgs e)
		{
			string ToolTorqStr = ((double)(int)UISys.RunningToolMaxTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) / 1000.0).ToString("F3");
			RangeSignedValue(sender, e, ToolTorqStr, "0.000");
		}

		public void RangeToolTorque_0000(object sender, KeyPressEventArgs e)
		{
			string ToolTorqStr = ((double)(int)UISys.RunningToolMaxTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) / 10000.0).ToString("F4");
			RangeSignedValue(sender, e, ToolTorqStr, "0.0000");
		}

		public void RangeMaxToolTorque_000(object sender, KeyPressEventArgs e)
		{
			string ToolTorqStr = ((double)(int)UISys.RunningToolMaxULTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) / 1000.0).ToString("F3");
			RangeSignedValue(sender, e, ToolTorqStr, "0.000");
		}

		public void RangeMaxToolTorque_0000(object sender, KeyPressEventArgs e)
		{
			string ToolTorqStr = ((double)(int)UISys.RunningToolMaxULTorqueFW * TorqUnitcoef(2 + UISys.ParamPageAxis) / TorqUnitcoef(1000 + FSModelTypeInfo.MesRawDataTorqUint) / 10000.0).ToString("F4");
			RangeSignedValue(sender, e, ToolTorqStr, "0.0000");
		}

		public void RangeToolLimitRPM(object sender, KeyPressEventArgs e)
		{
			string RPMstr = 100.ToString();
			RangeSignedValue(sender, e, RPMstr, "0");
			TextBox TB = (TextBox)sender;
			if (int.TryParse(TB.Text, out var result))
			{
				if (result < UISys.RunningToolMinSpeed || result > UISys.RunningToolMaxSpeed)
				{
					TB.BackColor = Color.LightCoral;
				}
				else
				{
					TB.BackColor = Color.White;
				}
			}
			else
			{
				TB.BackColor = Color.LightCoral;
			}
		}

		public void RangeToolRPM(object sender, KeyPressEventArgs e)
		{
			string RPMstr = UISys.RunningToolMaxSpeed.ToString();
			RangeSignedValue(sender, e, RPMstr, "0");
			TextBox TB = (TextBox)sender;
			if (int.TryParse(TB.Text, out var result))
			{
				if (result < UISys.RunningToolMinSpeed || result > UISys.RunningToolMaxSpeed)
				{
					TB.BackColor = Color.LightCoral;
				}
				else
				{
					TB.BackColor = Color.White;
				}
			}
			else
			{
				TB.BackColor = Color.LightCoral;
			}
		}

		public void RangeUnsigned300_10000(object sender, KeyPressEventArgs e)
		{
			int MaxVal = 10000;
			int MinVal = 300;
			RangeSignedValue(sender, e, MaxVal.ToString(), "0");
			TextBox TB = (TextBox)sender;
			if (int.TryParse(TB.Text, out var result))
			{
				if (result < MinVal || result > MaxVal)
				{
					TB.BackColor = Color.LightCoral;
				}
				else
				{
					TB.BackColor = Color.White;
				}
			}
			else
			{
				TB.BackColor = Color.LightCoral;
			}
		}

		public void ModfiySpeedTB(Control parent)
		{
			foreach (Control control in parent.Controls)
			{
				if (control is GroupBox groupBox)
				{
					foreach (Control groupBoxControl in groupBox.Controls)
					{
						if (groupBoxControl is TextBox SearTB && SearTB.BackColor == Color.LightCoral)
						{
							SearTB.Text = RPMRangeDetect(SearTB.Text, UISys.RunningToolMaxSpeed, UISys.RunningToolMinSpeed);
							SearTB.BackColor = Color.White;
						}
					}
				}
				if (control.HasChildren)
				{
					ModfiySpeedTB(control);
				}
			}
		}

		public string RPMRangeDetect(string str, int MaxVal, int MinVal)
		{
			string Str = str;
			if (int.TryParse(str, out var result))
			{
				if (result < MinVal)
				{
					Str = MinVal.ToString("F0");
				}
				if (result > MaxVal)
				{
					Str = MaxVal.ToString("F0");
				}
			}
			else
			{
				Str = MinVal.ToString("F0");
			}
			return Str;
		}

		private void RangeSignedValue(object sender, KeyPressEventArgs e, string MaxValStr, string MinValStr)
		{
			e.Handled = false;
			int MaxValLen = 0;
			int FloatPoint = -1;
			bool Signal = false;
			double KeyInVal = 0.0;
			if (MinValStr.IndexOf('-') != -1)
			{
				Signal = true;
			}
			if (MaxValStr.IndexOf('.') != -1)
			{
				FloatPoint = MaxValStr.Length - (MaxValStr.IndexOf('.') + 1);
			}
			MaxValLen = MaxValStr.Length;
			if (Signal)
			{
				if (((TextBox)sender).Text.Length >= MaxValLen + 1 && e.KeyChar != '\b')
				{
					e.Handled = true;
				}
			}
			else if (((TextBox)sender).Text.Length >= MaxValLen && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
			if (Signal && FloatPoint > 0)
			{
				if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b')
				{
					e.Handled = true;
				}
			}
			else if (!Signal && FloatPoint > 0)
			{
				if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '.' && e.KeyChar != '\b')
				{
					e.Handled = true;
				}
			}
			else if (Signal && FloatPoint <= 0)
			{
				if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '-' && e.KeyChar != '\b')
				{
					e.Handled = true;
				}
			}
			else if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
			if (Signal)
			{
				if (((TextBox)sender).Text != "" && e.KeyChar == '-')
				{
					e.Handled = true;
				}
				if (e.KeyChar == '-' && ((TextBox)sender).Text.IndexOf('-') != -1)
				{
					e.Handled = true;
				}
			}
			TextBox FloatBox = (TextBox)sender;
			if (FloatPoint > 0)
			{
				if (e.KeyChar == '.' && ((TextBox)sender).Text.IndexOf('.') != -1)
				{
					e.Handled = true;
				}
				if (((TextBox)sender).Text == "" && e.KeyChar == '.')
				{
					FloatBox.Text = "0.";
					FloatBox.SelectionStart = FloatBox.Text.Length;
					e.Handled = true;
				}
				if (e.KeyChar != '.' && ((TextBox)sender).Text == "0" && e.KeyChar != '\b')
				{
					e.Handled = true;
				}
				if (((TextBox)sender).Text.IndexOf('.') != -1 && e.KeyChar != '\b' && ((TextBox)sender).Text.Length - ((TextBox)sender).Text.IndexOf('.') - 1 == FloatPoint)
				{
					e.Handled = true;
				}
			}
			TextBox textBox = (TextBox)sender;
			if (textBox.SelectionLength > 0)
			{
				textBox.Text = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength);
			}
			if (Signal && FloatPoint > 0)
			{
				if (((TextBox)sender).Text == "." && e.KeyChar == '-')
				{
					e.Handled = true;
				}
				if (((TextBox)sender).Text == "-" && e.KeyChar == '.')
				{
					e.Handled = true;
				}
				if (e.KeyChar != '.' && ((TextBox)sender).Text == "-0" && e.KeyChar != '\b')
				{
					e.Handled = true;
				}
			}
			TextBox OrgText = (TextBox)sender;
			int cursorPosition = OrgText.SelectionStart;
			string currentText = OrgText.Text;
			string newText = currentText.Substring(0, cursorPosition) + e.KeyChar + currentText.Substring(cursorPosition);
			double.TryParse(newText, out KeyInVal);
			if (e.KeyChar >= '0' && e.KeyChar <= '9' && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b')
			{
				if (KeyInVal > double.Parse(MaxValStr))
				{
					e.Handled = true;
					((TextBox)sender).Text = MaxValStr;
				}
				else if (KeyInVal < double.Parse(MinValStr))
				{
					e.Handled = true;
					((TextBox)sender).Text = MinValStr;
				}
			}
			Console.WriteLine(newText);
			if (e.KeyChar == '\r' || e.KeyChar == ' ')
			{
				switch (FloatPoint)
				{
				case 1:
					LostFocus_C1(sender, e);
					break;
				case 2:
					LostFocus_C2(sender, e);
					break;
				case 3:
					LostFocus_C3(sender, e);
					break;
				case 4:
					LostFocus_C4(sender, e);
					break;
				default:
					LostFocus_C0(sender, e);
					break;
				}
			}
		}

		public void RangePasswordValue(object sender, KeyPressEventArgs e)
		{
			e.Handled = false;
			if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
			{
				e.Handled = true;
			}
			if (e.KeyChar == '\r' || e.KeyChar == ' ')
			{
				LostFocus_C0(sender, e);
			}
		}

		public void PushSaveSomething()
		{
			if (this.CreateSaveSomething != null)
			{
				this.CreateSaveSomething();
			}
		}

		public void PushOnlyUpdateMervel()
		{
			if (this.OnlyUpdateScreenUI110 != null)
			{
				this.OnlyUpdateScreenUI110();
			}
			if (this.OnlyUpdateScreenUI111 != null)
			{
				this.OnlyUpdateScreenUI111();
			}
			if (this.OnlyUpdateScreenUI112 != null)
			{
				this.OnlyUpdateScreenUI112();
			}
			if (this.OnlyUpdateScreenUI113 != null)
			{
				this.OnlyUpdateScreenUI113();
			}
			if (this.OnlyUpdateScreenUI140 != null)
			{
				this.OnlyUpdateScreenUI140();
			}
			if (this.OnlyUpdateScreenUI141 != null)
			{
				this.OnlyUpdateScreenUI141();
			}
			if (this.OnlyUpdateScreenUI142 != null)
			{
				this.OnlyUpdateScreenUI142();
			}
			if (this.OnlyUpdateScreenUI143 != null)
			{
				this.OnlyUpdateScreenUI143();
			}
			if (this.OnlyUpdateScreenUI144 != null)
			{
				this.OnlyUpdateScreenUI144();
			}
			if (this.OnlyUpdateScreenUI145 != null)
			{
				this.OnlyUpdateScreenUI145();
			}
			if (this.OnlyUpdateScreenUI146 != null)
			{
				this.OnlyUpdateScreenUI146();
			}
			if (this.OnlyUpdateScreenUI100 != null)
			{
				this.OnlyUpdateScreenUI100();
			}
		}

		public void PushUpdateMervel()
		{
			if (this.CreateUI110 != null)
			{
				this.CreateUI110(true);
			}
			if (this.CreateUI111 != null)
			{
				this.CreateUI111(true);
			}
			if (this.CreateUI112 != null)
			{
				this.CreateUI112(true);
			}
			if (this.CreateUI113 != null)
			{
				this.CreateUI113(true);
			}
			if (this.CreateUI140 != null)
			{
				this.CreateUI140(true);
			}
			if (this.CreateUI141 != null)
			{
				this.CreateUI141(true);
			}
			if (this.CreateUI142 != null)
			{
				this.CreateUI142(true);
			}
			if (this.CreateUI143 != null)
			{
				this.CreateUI143(true);
			}
			if (this.CreateUI144 != null)
			{
				this.CreateUI144(true);
			}
			if (this.CreateUI145 != null)
			{
				this.CreateUI145(true);
			}
			if (this.CreateUI146 != null)
			{
				this.CreateUI146(false);
			}
			if (this.CreateUI100 != null)
			{
				this.CreateUI100(true);
			}
		}

		public void LostFocus_C0(object sender, EventArgs e)
		{
			if (((TextBox)sender).Text == "")
			{
				((TextBox)sender).Text = "0";
			}
			PushUpdateMervel();
		}

		public void LostFocus_C1(object sender, EventArgs e)
		{
			double Value = 0.0;
			if (((TextBox)sender).Text == "")
			{
				Value = 0.0;
			}
			else
			{
				double.TryParse(((TextBox)sender).Text, out Value);
			}
			((TextBox)sender).Text = Value.ToString("F1");
			PushUpdateMervel();
		}

		public void LostFocus_C2(object sender, EventArgs e)
		{
			double Value = 0.0;
			if (((TextBox)sender).Text == "")
			{
				Value = 0.0;
			}
			else
			{
				double.TryParse(((TextBox)sender).Text, out Value);
			}
			((TextBox)sender).Text = Value.ToString("F2");
			PushUpdateMervel();
		}

		public void LostFocus_C3(object sender, EventArgs e)
		{
			double Value = 0.0;
			if (((TextBox)sender).Text == "")
			{
				Value = 0.0;
			}
			else
			{
				double.TryParse(((TextBox)sender).Text, out Value);
			}
			((TextBox)sender).Text = Value.ToString("F3");
			PushUpdateMervel();
		}

		public void LostFocus_C4(object sender, EventArgs e)
		{
			double Value = 0.0;
			if (((TextBox)sender).Text == "")
			{
				Value = 0.0;
			}
			else
			{
				double.TryParse(((TextBox)sender).Text, out Value);
			}
			((TextBox)sender).Text = Value.ToString("F4");
			PushUpdateMervel();
		}

		public int ParmIsUseSWTorqEn(ref ParamItemStucVer1[] ParamItem)
		{
			int SWTorqFinalStage = 0;
			for (int i = 0; i < 6; i++)
			{
				if (ParamItem[i].MaxSwitchTorque_DW_35 != 0 && ParamItem[i].RotationSpeed_3 != 0)
				{
					SWTorqFinalStage = i + 1;
				}
			}
			return SWTorqFinalStage;
		}

		public unsafe void ClearReportDeleteCh()
		{
			for (int i = 0; i < 200000; i++)
			{
				ExFSReport.Delete[i] = false;
			}
		}

		public void ClearReportList(int Type)
		{
			switch (Type)
			{
			case 0:
			{
				for (int l = 0; l < 200000; l++)
				{
					ExFSReport.Info[l].ScrewNo = 0u;
					ExFSReport.Info[l].ParmID = 0;
					ExFSReport.Info[l].Status = 0;
				}
				break;
			}
			case 1:
			{
				for (int j = 0; j < 6000; j++)
				{
					ExFSReport.AlarmInfo[j].Code = 0u;
					ExFSReport.AlarmInfoOnlyAL[j].Code = 0u;
					ExFSReport.AlarmInfoOnlyNG[j].Code = 0u;
				}
				break;
			}
			case 2:
			{
				for (int k = 0; k < 6000; k++)
				{
					ExFSReport.WarningInfo[k].Code = 0u;
				}
				break;
			}
			case 3:
			{
				for (int i = 0; i < 6000; i++)
				{
					ExFSReport.ButtonInfo[i].ID = 0u;
				}
				break;
			}
			}
		}

		public string AddStageSnugRow(int CurveMode, int Mode, int Axis)
		{
			int WidthAng = 8;
			int Width = 15;
			string Str = "";
			double Status;
			double SetTorqueRate;
			double MaxTorqueRate;
			double ClampAngle;
			double ClampTorque;
			double SnugTorque;
			if (Mode < 1000)
			{
				ReportInfoStuc RunningInfo = ((Axis == 0) ? UISys.RunningInfoX : UISys.RunningInfoY);
				ReportScaleStuc RunningScale = ((Axis == 0) ? UISys.RunningScaleX : UISys.RunningScaleY);
				Status = (int)RunningInfo.Status;
				double FW2Reportcoef = TorqUnitcoef(1000 + RunningInfo.TorqueUnit) / TorqUnitcoef(1000 + RunningInfo.FWSystemCoef);
				SetTorqueRate = Math.Round(Math.Floor((double)(int)RunningInfo.TargetTorqueRate * FW2Reportcoef) / 10000.0 * 1000.0) / 1000.0;
				MaxTorqueRate = Math.Floor((double)RunningScale.CurveMaxTorqueRate * FW2Reportcoef) / 10000.0;
				ClampAngle = (int)RunningInfo.ClampAngle;
				ClampTorque = Math.Floor((double)(int)RunningInfo.ClampTorque * FW2Reportcoef) / 1000.0;
				SnugTorque = Math.Floor(((double)(int)RunningInfo.AppliedTorque - (double)(int)RunningInfo.ClampTorque) * FW2Reportcoef) / 1000.0;
			}
			else
			{
				Status = (int)ExFSReport.Info[Axis].Status;
				SetTorqueRate = (double)ExFSReport.Info[Axis].TargetTorqueRate_DW / 10000.0;
				MaxTorqueRate = (double)ExFSReport.Scale[Axis].CurveMaxTorqueRate_DW / 10000.0;
				ClampAngle = (int)ExFSReport.Info[Axis].ClampAngle;
				ClampTorque = (double)ExFSReport.Info[Axis].ClampTorque_DW / 1000.0;
				SnugTorque = (double)(int)(ExFSReport.Info[Axis].AppliedTorque_DW - ExFSReport.Info[Axis].ClampTorque_DW) / 1000.0;
			}
			if (Mode == 0 || Mode == 1000)
			{
				if (CurveMode > 0)
				{
					int Len1 = ((Width - SetTorqueRate.ToString("F4").Length * 2 >= 0) ? (Width - SetTorqueRate.ToString("F4").Length * 2) : 0);
					int Len2 = ((Width - MaxTorqueRate.ToString("F4").Length * 2 >= 0) ? (Width - MaxTorqueRate.ToString("F4").Length * 2) : 0);
					Str = string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_SetTorqueRateText"), string.Concat(Enumerable.Repeat(" ", Len1)), SetTorqueRate.ToString("F4"), " ");
					Str = string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_MaxTorqueRateText"), string.Concat(Enumerable.Repeat(" ", Len2)), MaxTorqueRate.ToString("F4"), " ");
				}
			}
			else if ((Mode == 1 || Mode == 1001) && SetTorqueRate > 0.0 && Status != 0.0)
			{
				int Len3 = 0;
				Len3 = ((FSCtrlAngleUnit.Mode != 0) ? ((WidthAng - (ClampAngle / 360.0).ToString("F3").Length * 2 >= 0) ? (WidthAng - (ClampAngle / 360.0).ToString("F3").Length * 2) : 0) : ((WidthAng - ClampAngle.ToString("F0").Length * 2 >= 0) ? (WidthAng - ClampAngle.ToString("F0").Length * 2) : 0));
				int Len4 = ((Width - ClampTorque.ToString("F3").Length * 2 >= 0) ? (Width - ClampTorque.ToString("F3").Length * 2) : 0);
				int Len5 = ((Width - SnugTorque.ToString("F3").Length * 2 >= 0) ? (Width - SnugTorque.ToString("F3").Length * 2) : 0);
				Str = ((FSCtrlAngleUnit.Mode != 0) ? string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageClampAngText"), string.Concat(Enumerable.Repeat(" ", Len3)), (ClampAngle / 360.0).ToString("F3"), " ") : string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageClampAngText"), string.Concat(Enumerable.Repeat(" ", Len3)), ClampAngle.ToString("F0"), " "));
				Str = string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageClampTorqText"), string.Concat(Enumerable.Repeat(" ", Len4)), ClampTorque.ToString("F3"), " ");
				Str = string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageSnugTorqText"), string.Concat(Enumerable.Repeat(" ", Len5)), SnugTorque.ToString("F3"), " ");
			}
			return Str;
		}

		public string AddStageRow(ushort Stage, short StageAngle, int StageTorque, ushort StageTime, int StageSWTorque, bool SWTorqEn, float coef)
		{
			int WidthAng = (int)(12f * FormControlZoom.ScreenWidthZoom);
			int Width = (int)(15f * FormControlZoom.ScreenWidthZoom);
			string Str = "";
			string Title = "";
			double Angle = StageAngle;
			double Torque = Math.Floor((float)StageTorque * coef) / 1000.0;
			double Time = (double)(int)StageTime / 1000.0;
			double SWTorque = Math.Floor((float)StageSWTorque * coef) / 1000.0;
			Title = ((Stage != 7 && Stage != 8) ? (MultiLanguage.GetStr("Form400_Results", "tp_StageText") + Stage) : (MultiLanguage.GetStr("Form400_Results", "tp_StageLoosText") + (Stage - 6)));
			int Len1 = 0;
			Len1 = ((FSCtrlAngleUnit.Mode != 0) ? ((WidthAng - (Angle / 360.0).ToString("F3").Length * 2 >= 0) ? (WidthAng - (Angle / 360.0).ToString("F3").Length * 2) : 0) : ((WidthAng - Angle.ToString("F0").Length * 2 >= 0) ? (WidthAng - Angle.ToString("F0").Length * 2) : 0));
			int Len2 = ((Width - Torque.ToString("F3").Length * 2 >= 0) ? (Width - Torque.ToString("F3").Length * 2) : 0);
			int Len3 = ((Width - Time.ToString("F3").Length * 2 >= 0) ? (Width - Time.ToString("F3").Length * 2) : 0);
			int Len4 = ((Width - SWTorque.ToString("F3").Length * 2 >= 0) ? (Width - Torque.ToString("F3").Length * 2) : 0);
			Str = ((FSCtrlAngleUnit.Mode != 0) ? string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageAngText"), " ", (Angle / 360.0).ToString("F3"), string.Concat(Enumerable.Repeat(" ", Len1))) : string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageAngText"), " ", Angle.ToString("F0"), string.Concat(Enumerable.Repeat(" ", Len1))));
			Str = string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageTorqText"), " ", Torque.ToString("F3"), string.Concat(Enumerable.Repeat(" ", Len2)));
			Str = string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageTimeText"), " ", Time.ToString("F3"), string.Concat(Enumerable.Repeat(" ", Len3)));
			if (SWTorqEn)
			{
				Str = string.Concat(Str, MultiLanguage.GetStr("Form400_Results", "tp_StageSWTorqText"), " ", SWTorque.ToString("F3"), string.Concat(Enumerable.Repeat(" ", Len4)));
			}
			return Title + "   " + Str;
		}

		public unsafe int ParamCreateNewRow(int Axis)
		{
			int CurrNum = 0;
			for (int i = 0; i < 500; i++)
			{
				if (((Axis == 0) ? ExFSParamX.EnableGP[i] : ExFSParamY.EnableGP[i]) == 0)
				{
					CurrNum = i + 1;
					break;
				}
			}
			return CurrNum;
		}

		public unsafe int SeqCreateNewRow()
		{
			int CurrNum = 0;
			for (int i = 0; i < 500; i++)
			{
				if (ExFSSeq.EnableMode[i] == 0)
				{
					CurrNum = i + 1;
					break;
				}
			}
			return CurrNum;
		}

		public bool CheckHMIVer(int HMIVer, int SubVer)
		{
			bool Rst = false;
			if ((FSModelTypeInfo.VerHMI == HMIVer && FSModelTypeInfo.VerHMISub >= SubVer) || FSModelTypeInfo.VerHMI > HMIVer)
			{
				return true;
			}
			return false;
		}

		public bool CheckMotionFWVer(int MotionVer)
		{
			bool Rst = false;
			if (FSModelTypeInfo.VerMotionFW >= MotionVer)
			{
				return true;
			}
			return false;
		}

		public void UseFTPGetFile(string OpenPath, string SavePath)
		{
			string url = "ftp://" + UISys.IPstr + "/" + OpenPath;
			WebClient request = new WebClient();
			request.Credentials = new NetworkCredential("admin", "1234");
			if (!CheckFileExists(url, request))
			{
				return;
			}
			try
			{
				byte[] RawBin = request.DownloadData(new Uri(url));
				using (BinaryWriter PicW = new BinaryWriter(File.Open(SavePath, FileMode.Create)))
				{
					for (int i = 0; i < RawBin.Length; i++)
					{
						PicW.Write(RawBin[i]);
					}
				}
			}
			catch (WebException)
			{
			}
		}

		public List<ushort> UseFTPGetFile(string Namefile)
		{
			List<ushort> ListRawBin = new List<ushort>();
			string url = "ftp://" + UISys.IPstr + "/" + Namefile;
			WebClient request = new WebClient();
			request.Credentials = new NetworkCredential("admin", "1234");
			if (CheckFileExists(url, request))
			{
				try
				{
					byte[] RawBin = request.DownloadData(new Uri(url));
					ushort[] Data16 = new ushort[RawBin.Length / 2];
					for (int i = 0; i < RawBin.Length / 2; i++)
					{
						Data16[i] = (ushort)(RawBin[2 * i + 1] * 256 + RawBin[2 * i]);
					}
					ListRawBin = Data16.ToList();
				}
				catch (WebException)
				{
				}
			}
			return ListRawBin;
		}

		public List<byte> ReadFSBinFile(int Num)
		{
			if (UISys.PCFTPMaster)
			{
				UseFTPGetFile("ScrewInfo/FS/FS" + Num + ".Bin", UISys.FTPSavePath + "\\FS" + Num + ".Bin");
			}
			string filePath = UISys.FTPSavePath + "\\FS" + Num + ".Bin";
			List<byte> RetList8 = new List<byte>();
			try
			{
				if (File.Exists(filePath))
				{
					using (FileStream BinFile = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						using (BinaryReader ReaderBin = new BinaryReader(BinFile))
						{
							BinFile.Seek(0L, SeekOrigin.Begin);
							byte[] Data8 = ReaderBin.ReadBytes((int)BinFile.Length);
							RetList8.AddRange(Data8);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
			return RetList8;
		}

		private bool CheckFileExists(string url, WebClient request)
		{
			try
			{
				request.DownloadData(url);
				return true;
			}
			catch (WebException ex)
			{
				if (ex.Response is FtpWebResponse response && response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
				{
					return false;
				}
				return false;
			}
		}

		public long GetSystemFreeSpace()
		{
			string exePath = Application.StartupPath;
			string rootPath = Path.GetPathRoot(exePath);
			DriveInfo drive = new DriveInfo(rootPath);
			return drive.AvailableFreeSpace / 1048576;
		}

		public void IsProhibitOperation_Param(Control parent)
		{
			if ((UserIDDetect() & 1) <= 0)
			{
				return;
			}
			foreach (Control control in parent.Controls)
			{
				if (!(control.Name == "AxisX_Bn") && !(control.Name == "AxisY_Bn") && !(control.Name == "Stage1Bn") && !(control.Name == "Stage2Bn") && !(control.Name == "Stage3Bn") && !(control.Name == "Stage4Bn") && !(control.Name == "Stage5Bn") && !(control.Name == "Stage6Bn"))
				{
					if (control is TextBox)
					{
						control.Enabled = false;
					}
					else if (control is Button)
					{
						control.Enabled = false;
					}
					else if (control is CheckBox)
					{
						control.Enabled = false;
					}
					else if (control is RadioButton)
					{
						control.Enabled = false;
					}
					else if (control is ComboBox)
					{
						control.Enabled = false;
					}
				}
				if (control.HasChildren)
				{
					IsProhibitOperation_Param(control);
				}
			}
		}

		public void IsProhibitOperation_Seq(Control parent)
		{
			if ((UserIDDetect() & 2) <= 0)
			{
				return;
			}
			foreach (Control control in parent.Controls)
			{
				if (!(control.Name == "dataGridView_Seq"))
				{
					if (control is TextBox)
					{
						control.Enabled = false;
					}
					else if (control is Button)
					{
						control.Enabled = false;
					}
					else if (control is CheckBox)
					{
						control.Enabled = false;
					}
					else if (control is DataGridView DGV)
					{
						DGV.Enabled = false;
					}
				}
				if (control.HasChildren)
				{
					IsProhibitOperation_Seq(control);
				}
			}
		}

		public void IsProhibitOperation_Src(Control parent)
		{
			if ((UserIDDetect() & 4) <= 0)
			{
				return;
			}
			foreach (Control control in parent.Controls)
			{
				if (!(control.Name == "AxisX_Bn") && !(control.Name == "AxisY_Bn"))
				{
					if (control is TextBox)
					{
						control.Enabled = false;
					}
					else if (control is Button)
					{
						control.Enabled = false;
					}
					else if (control is CheckBox)
					{
						control.Enabled = false;
					}
					else if (control is DataGridView DGV)
					{
						DGV.Enabled = false;
					}
					else if (control is ComboBox)
					{
						control.Enabled = false;
					}
				}
				if (control.HasChildren)
				{
					IsProhibitOperation_Src(control);
				}
			}
		}
	}
}
