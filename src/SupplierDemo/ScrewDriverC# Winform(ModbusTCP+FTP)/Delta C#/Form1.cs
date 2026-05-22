using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ScrewDriver
{
    public partial class Form1 : Form
    {
        ModbusClient ModB = new ModbusClient();
        private System.Windows.Forms.Timer TimerEvent;
        private int SM = 0;
        private uint LastReportRow = 0;
        private uint NowReportRow = 0;
        private static DateTime LastTickTime = DateTime.Now;
        private const int TimeOut = 3000;
        private string MsgCurveStr = "";
        public List<float> List_Time = new List<float>();
        public List<float> List_Angle = new List<float>();
        public List<float> List_Torq = new List<float>();
        public List<float> List_TorqRate = new List<float>();
        public List<float> List_Speed = new List<float>();
        public List<float> List_Stage = new List<float>();
        public ReportInfo List_Info = new ReportInfo();
        public ReportScale List_Scale = new ReportScale();
        public ParamCommItemVer1 List_Param = new ParamCommItemVer1();
        public OtherInfo List_OtherInfo = new OtherInfo();
        public int CurveVer = 0;
        public struct ReportInfo
        {
            public uint Year;
            public uint Month;
            public uint Day;
            public uint Hour;
            public uint Min;
            public uint Sec;
            public string SN;
            public uint Tool;
            public uint ScrewNo;
            public uint SeqID;
            public uint ParmID;
            public float TargetTorque;
            public int TargetAngle;
            public float TargetTorqueRate;
            public float FinalTorque;
            public int TighteningAngle;
            public int TotalAngle;
            public uint Status;
            public float CT_Time;
            public uint ErrorCode;
            public int MaxTighteningAngle;
            public int MinTighteningAngle;
            public Single MaxTorque;
            public Single MinTorque;
            public uint TorqueUnit;
            public float ToolMaxTorque;
            public float ToolProtectTorque;
            public float PreTighteningTorque;
            public float SetMaxTime;
            public uint SetMaxAngle;
            public float FinalStage_SetMaxTorque;
            public float FinalStage_SetMinTorque;
            public int FinalStage_SetMaxAngle;
            public int FinalStage_SetMinAngle;
            public float FinalStage_SetMaxTime;
            public float FinalStage_SetMinTime;
            public float PrevailTorque;
            public float AppliedTorque;
            public float FinalCurrent;
            public float ClampTorque;
            public float SetMaxClampTorque;
            public float SetMinClampTorque;
            public int ClampAngle;
            public int SetMaxClampAngle;
            public int SetMinClampAngle;
            public int SetMinAngle;
            public int UserID;
            public int RawDataUnit;
            public int TargetYield;
        }

        public struct ReportScale
        {
            public int Stage1Angle;
            public int Stage2Angle;
            public int Stage3Angle;
            public int Stage4Angle;
            public int Stage5Angle;
            public int Stage6Angle;
            public int Loosening1Angle;
            public int Loosening2Angle;
            public float Stage1Torque;
            public float Stage2Torque;
            public float Stage3Torque;
            public float Stage4Torque;
            public float Stage5Torque;
            public float Stage6Torque;
            public float Loosening1Torque;
            public float Loosening2Torque;
            public ushort Stage1Time;
            public ushort Stage2Time;
            public ushort Stage3Time;
            public ushort Stage4Time;
            public ushort Stage5Time;
            public ushort Stage6Time;
            public ushort Loosening1Time;
            public ushort Loosening2Time;
            public float Curve_MaxTime;
            public int Curve_MaxAngle;
            public float Curve_MaxTorque;
            public float Curve_MaxTorqueRate;
            public uint Curve_TotalPoint;
            public float SetMaxTorque;
            public float SetMinTorque;
            public float SetMaxTorqRate;
            public int SetMaxAngle;
            public int SetMinAngle;
            public uint CurveVer;
            public uint CurveFreqModeVer;
            public float CurveMaxTorqueRate;
            public float Curve_MinTime;
            public int Curve_MinAngle;
            public float Curve_MinTorque;
            public float Curve_MinTorqueRate;
            public float Stage1SwitchTorq;
            public float Stage2SwitchTorq;
            public float Stage3SwitchTorq;
            public float Stage4SwitchTorq;
            public float Stage5SwitchTorq;
            public float Stage6SwitchTorq;
        }
        public class OtherInfo
        {
            public string ParameterTitle;
            public string SequenceTitle;
            public string ToolModelName;
            public string ToolSerialNumber;
            public uint ToolLifeCounter;
            public uint RepairToolLifeCounter;
            public uint RepairYear;
            public uint RepairMonth;
            public uint RepairDay;
            public uint RepairHour;
            public uint RepairMin;
            public uint RepairSec;
            public uint ToolMaxSpeed;
            public int TargetArmX;
            public int TargetArmY;
            public int TargetArmZ;
            public int StartTighteningArmX;
            public int StartTighteningArmY;
            public int StartTighteningArmZ;
            public string ControllerName;
            public int HMIMainVersion;
            public int FWVersion;
            public int HMISubVersion;
            public uint ReportID;
        }


        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        unsafe public struct ParamCommItemVer1
        {
            [FieldOffset(0)]
            public fixed ushort Data16[1200 / 2];
            [FieldOffset(0)]
            public ParamCommVer1 Comm;
            [FieldOffset(100)]
            public ParamItemVer1 Item1;
            [FieldOffset(200)]
            public ParamItemVer1 Item2;
            [FieldOffset(300)]
            public ParamItemVer1 Item3;
            [FieldOffset(400)]
            public ParamItemVer1 Item4;
            [FieldOffset(500)]
            public ParamItemVer1 Item5;
            [FieldOffset(600)]
            public ParamItemVer1 Item6;
            [FieldOffset(700)]
            public ParamItemVer1 Loos1;
            [FieldOffset(800)]
            public ParamItemVer1 Loos2;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        unsafe public struct ParamCommVer1
        {
            [FieldOffset(0)]
            public fixed ushort Data16[100 / 2];
            [FieldOffset(0)]
            public ushort PC1_ID_00;
            [FieldOffset(2)]
            public ushort PC1_VER_01;
            [FieldOffset(4)]
            public ushort PC1_TUNIT_02;
            [FieldOffset(6)]
            public ushort PC1_TYPE_03;
            [FieldOffset(8)]
            public ushort PC1_CURR_04;
            [FieldOffset(10)]
            public ushort PC1_UNIT_05;
            [FieldOffset(12)]
            public ushort PC1_LOOS_ANG1_06;
            [FieldOffset(14)]
            public ushort PC1_LOOS_ANG2_07;
            [FieldOffset(16)]
            public ushort PC1_YIELDPERCENT_08;
            [FieldOffset(18)]
            public ushort PC1_HMI_09;
            [FieldOffset(20)]
            public ushort PC1_TOOL_10;
            [FieldOffset(22)]
            public ushort PC1_MONSW1_11;
            [FieldOffset(24)]
            public ushort PC1_LIMITSW2_12;
            [FieldOffset(26)]
            public ushort PC1_CWCCW_13;
            [FieldOffset(28)]
            public ushort PC1_TGENDDELAY_14;
            [FieldOffset(30)]
            public ushort PC1_ANGLEDIFF_15;
            [FieldOffset(32)]
            public ushort PC1_SUNGBACKANG_16;
            [FieldOffset(34)]
            public ushort PC1_SUNGDELAYANG_17;
            [FieldOffset(36)]
            public ushort PC1_CTRLVER_18;
            [FieldOffset(38)]
            public ushort PC1_TOOLSPEC_19;
            [FieldOffset(40)]
            public ushort PC1_BITSLIPCNT_BOMPARM_20;
            [FieldOffset(42)]
            public ushort PC1_BITSLIPANG_21;
            [FieldOffset(44)]
            public ushort PC1_TGTIUU_22;
            [FieldOffset(46)]
            public ushort PC1_TGAGUU_23;
            [FieldOffset(48)]
            public ushort PC1_RESEVER_24;
            [FieldOffset(50)]
            public ushort PC1_TGDELAY_25;
            [FieldOffset(52)]
            public ushort PC1_TGAGUL_26;
            [FieldOffset(54)]
            public ushort PC1_RESEVER_27;
            [FieldOffset(56)]
            public uint PC1_YIELDTORQ_28;//DW
            [FieldOffset(60)]
            public ushort PC1_LOOS_CW_30;
            [FieldOffset(62)]
            public ushort PC1_YYMMDD_31;
            [FieldOffset(64)]
            public ushort PC1_LOTIUU_32;
            [FieldOffset(66)]
            public uint PC1_BITSLIPTORQ_33;//DW
            [FieldOffset(70)]
            public ushort PC1_LODELAY_35;
            [FieldOffset(72)]
            public uint PC1_HHMMSS_36;//DW
            [FieldOffset(76)]
            public ushort PC1_TOOLSENCOMP_38;
            [FieldOffset(78)]
            public uint PC1_LOOS_MON_T_39;//DW
            [FieldOffset(82)]
            public ushort PC1_COMPVAL_41;
            [FieldOffset(84)]
            public ushort PC1_COMPLINK_42;
            [FieldOffset(86)]
            public ushort PC1_RESEVER_43;
            [FieldOffset(88)]
            public uint PC1_TGANGT_44;//DW
            [FieldOffset(92)]
            public ushort PC1_SUANGT_46;
            [FieldOffset(96)]
            public ushort PC1_LOOS_SPD1_48;
            [FieldOffset(98)]
            public ushort PC1_LOOS_SPD2_49;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        unsafe public struct ParamItemVer1
        {
            [FieldOffset(0)]
            public fixed ushort Data16[100 / 2];
            [FieldOffset(0)]
            public ushort PB1_CTRLSW_00;
            [FieldOffset(2)]
            public ushort PB1_CTRLSW_01;
            [FieldOffset(4)]
            public ushort PB1_FINVELCMD_02;
            [FieldOffset(6)]
            public ushort PB1_VELCMD_03;
            [FieldOffset(8)]
            public ushort PB1_ANGCMD_04;
            [FieldOffset(10)]
            public ushort PB1_DELAY_05;
            [FieldOffset(12)]
            public ushort PB1_TACC_06;
            [FieldOffset(14)]
            public ushort PB1_DACC_07;
            [FieldOffset(16)]
            public ushort PB1_FINTACC_08;
            [FieldOffset(18)]
            public ushort PB1_DIFFANG_09;
            [FieldOffset(20)]
            public ushort PB1_PREDELAY_10;
            [FieldOffset(22)]
            public ushort PB1_CALCOMP_11;
            [FieldOffset(24)]
            public uint PB1_TORQCMD_12;//DW
            [FieldOffset(28)]
            public uint PB1_SUNGT_14;//DW
            [FieldOffset(32)]
            public uint PB1_TGRATE_16;//DW
            [FieldOffset(36)]
            public uint PB1_PRETORQCMD_18;//DW
            [FieldOffset(40)]
            public ushort PB1_RESEVER_20;
            [FieldOffset(42)]
            public ushort PB1_RESEVER_21;
            [FieldOffset(44)]
            public ushort PB1_CLAMPTUU_22;
            [FieldOffset(48)]
            public uint PB1_CLAMPTUL_24;//DW
            [FieldOffset(52)]
            public ushort PB1_ANGUU_26;
            [FieldOffset(54)]
            public ushort PB1_ANGUL_27;
            [FieldOffset(56)]
            public uint PB1_TORQUU_28;//DW
            [FieldOffset(60)]
            public uint PB1_TORQUL_30;//DW
            [FieldOffset(64)]
            public uint PB1_SWTORQUU_32;//DW
            [FieldOffset(68)]
            public uint PB1_SWTORQUL_34;//DW
            [FieldOffset(72)]
            public ushort PB1_RESEVER_36;
            [FieldOffset(74)]
            public ushort PB1_RESEVER_37;
            [FieldOffset(76)]
            public ushort PB1_RESEVER_38;
            [FieldOffset(78)]
            public ushort PB1_TIMEUU_39;
            [FieldOffset(80)]
            public ushort PB1_TIMEUL_40;
            [FieldOffset(82)]
            public ushort PB1_CLAMPAUU_41;
            [FieldOffset(84)]
            public ushort PB1_CLAMPAUL_42;
            [FieldOffset(86)]
            public ushort PB1_COMPPRE_43;
            [FieldOffset(88)]
            public ushort PB1_COMMUSE_44;
            [FieldOffset(90)]
            public ushort PB1_COMMUSE_45;
            [FieldOffset(92)]
            public ushort PB1_MONSW_46;
            [FieldOffset(94)]
            public ushort PB1_LIMITSW_47;
            [FieldOffset(96)]
            public ushort PB1_COMMUSE_48;
            [FieldOffset(98)]
            public ushort PB1_TYPE_49;
        }
        public static class StatusMachine
        {
            public const int Stop = 0;
            public const int Init_GetReportID = 10;
            public const int Init_SetClearDI = 11;
            public const int Init_SendUnAutoLock = 12;
            public const int Init_CheckUnAutoLock = 13;
            public const int Init_SetAutoExportBinFile = 14;
            public const int Init_CheckAutoExportBinFile = 15;
            public const int Init_SendCurveVer = 16;
            public const int Init_CheckCurveVer = 17;
            public const int CheckReady = 100;
            public const int SendDICmd = 101;
            public const int CheckDISend = 102;
            public const int CheckReportID = 103;
            public const int ClearCmd = 104;
            public const int CheckDIClear = 105;
            public const int GetFTPFile = 106;
            public const int GetCaluNextReportID = 999;
            public const int Rst_SendUnAutoLock = 3000;
            public const int Rst_CheckUnAutoLock = 3001;
            public const int Rst_SetAutoExportBinFile = 3002;
            public const int Rst_CheckAutoExportBinFile = 3003;
            public const int Rst_ClearDICmd = 3004;
        }
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Example_Modbus+FTP_v0.0.0.3";
            RstBn.Enabled = AutoRunningCB.Enabled = StartBn.Enabled = StopBn.Enabled = false;
            TimerEvent = new System.Windows.Forms.Timer();
            TimerEvent.Interval = 100;
            TimerEvent.Tick += Timer_Tick;
            pictureBox1.Image = Delta_C_.Properties.Resources.LeverStart;
        }
        

        private void Connect_Click(object sender, EventArgs e)
        {
            if (ConnectBn.Text == "Connect")
            {
                ModB.IPAddress = IPTB.Text;
                ModB.Port = 502;
                try {
                    ModB.Connect(); 
                    StatusTB.Text = "Connect Success";
                    StatusTB.ForeColor = Color.Blue;
                    ConnectBn.Text = "Disconnect";
                    RstBn.Enabled = AutoRunningCB.Enabled = StartBn.Enabled = StopBn.Enabled = true;
                }
                catch (Exception ex) 
                { 
                    StatusTB.Text = "Connect Fail";
                    StatusTB.ForeColor = Color.Red;
                    RstBn.Enabled = AutoRunningCB.Enabled = StartBn.Enabled = StopBn.Enabled = false;
                }
            }
            else if (ConnectBn.Text == "Disconnect")
            {
                ConnectBn.Text = "Connect";
                ModB.Disconnect();
                RstBn.Enabled = AutoRunningCB.Enabled = StartBn.Enabled = StopBn.Enabled = false;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                switch (SM)
                {
                    case StatusMachine.Init_GetReportID:
                        int[] Row = Modbus_Read(0x6B, 2);
                        LastReportRow = (uint)Row[1] * 65536 + (uint)Row[0];
                        SM = StatusMachine.Init_SetClearDI;
                        break;
                    case StatusMachine.Init_SetClearDI:
                        Modbus_Write(0x68, 0);//Clear DI Bit0
                        SM = StatusMachine.Init_SendUnAutoLock;
                        break;
                    case StatusMachine.Init_SendUnAutoLock:
                        int[] Send533 = new int[10] { 533, 0, 0, 0, 0, 0, 1, 0, 0, 0 };// #533 Cmd + Clear Status
                        Modbus_Write(0xC8, Send533);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Init_CheckUnAutoLock;
                        break;
                    case StatusMachine.Init_CheckUnAutoLock:
                        int[] Read533ST = Modbus_Read(0xCF, 3);
                        if ((Read533ST[0] == 533) && (Read533ST[1] == 1) && (Read533ST[2] == 0))
                        {
                            SM = StatusMachine.Init_SetAutoExportBinFile;//Success 
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Init_SendUnAutoLock;//Retry
                        }
                        break;
                    case StatusMachine.Init_SetAutoExportBinFile:
                        int[] Send517 = new int[10] { 517, 0, 3, 0, 0, 0, 1, 0, 0, 0 };// #517 Cmd + Clear Status
                        Modbus_Write(0xC8, Send517);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Init_CheckAutoExportBinFile;
                        break;
                    case StatusMachine.Init_CheckAutoExportBinFile:
                        int[] Read517 = Modbus_Read(0xCF, 3);
                        if ((Read517[0] == 517) && (Read517[1] == 1) && (Read517[2] == 0))
                        {
                            SM = StatusMachine.Init_SendCurveVer;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                            {
                                SM = StatusMachine.Init_SetAutoExportBinFile;//Retry
                            }
                        }
                        break;
                    case StatusMachine.Init_SendCurveVer:
                        int[] Send562 = new int[10] { 562, 0, 0, 0, 0, 0, 1, 0, 0, 0 };// #562 Cmd + Clear Status
                        Modbus_Write(0xC8, Send562);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Init_CheckCurveVer;
                        break;
                    case StatusMachine.Init_CheckCurveVer:
                        int[] Read562ST = Modbus_Read(0xCF, 3);
                        if ((Read562ST[0] == 562) && (Read562ST[1] == 1) && (Read562ST[2] == 0))
                        {
                            CurveVer = Modbus_Read(0xC8);
                            if (AutoRunningCB.Checked == true)
                                SM = StatusMachine.CheckReady;//Success
                            else
                                SM = StatusMachine.CheckReportID;
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Init_SendCurveVer;//Retry
                        }
                        break;
                    case StatusMachine.CheckReady:
                        if (Modbus_Read(0x1F52) == 1)
                        {
                            SM = StatusMachine.SendDICmd;
                        }
                        break;
                    case StatusMachine.SendDICmd:
                        Modbus_Write(0x68, 1);//Send DI Bit0
                        SM = StatusMachine.CheckDISend;
                        break;
                    case StatusMachine.CheckDISend:
                        if ((Modbus_Read(0x67) & 0x1) == 1) //Check DI Bit0
                        {
                            SM = StatusMachine.CheckReportID;
                        }
                        break;
                    case StatusMachine.CheckReportID:
                        int[] NowReportID = Modbus_Read(0x6B, 2);
                        NowReportRow = ((uint)NowReportID[1] * 65536 + (uint)NowReportID[0]);
                        if (LastReportRow != NowReportRow)
                        {
                            if ((CurveVer == 0) || (CurveVer == 1))
                            {
                                if (LastReportRow < 200000)
                                    LastReportRow = LastReportRow + 1;
                                else
                                    LastReportRow = 1;//Restart
                            }
                            else if ((CurveVer == 2) || (CurveVer == 3))
                            {
                                if (LastReportRow < 100000)
                                    LastReportRow = LastReportRow + 1;
                                else
                                    LastReportRow = 1;//Restart
                            }
                            else
                            {
                                if (LastReportRow < 50000)
                                    LastReportRow = LastReportRow + 1;
                                else
                                    LastReportRow = 1;//Restart
                            }
                            SM = StatusMachine.ClearCmd;
                        }
                        break;
                    case StatusMachine.ClearCmd:
                        Modbus_Write(0x68, 0);//Clear DI Bit0
                        SM = StatusMachine.CheckDIClear;
                        break;
                    case StatusMachine.CheckDIClear:
                        if ((Modbus_Read(0x67) & 0x1) == 0) //Check DI Bit0
                        {
                            SM = StatusMachine.GetFTPFile;
                        }
                        break;
                    case StatusMachine.GetFTPFile:
                        bool Rst = UseFTPGetFile((LastReportRow - 1) % 100);

                        MsgCurveStr = "ReportID : " + LastReportRow.ToString() + "\r\n";
                        if (Rst == true)
                        {
                            MsgCurveStr = MsgCurveStr + "Report Info \r\n"
                                + "0x13C(Tool): " + List_Info.Tool.ToString() + "\r\n"
                                + "0x145(Tightening Angle):" + List_Info.TighteningAngle.ToString() + "\r\n"
                                + "0x146(Total Angle):" + List_Info.TotalAngle.ToString() + "\r\n"
                                + "0x147(Status):" + List_Info.Status.ToString() + "\r\n"
                                + "0x148(CT):" + List_Info.CT_Time.ToString() + "\r\n"
                                + "0x17D,0x17E(Final + Prevail Torque):" + List_Info.AppliedTorque.ToString("F3") + "\n";

                            MsgCurveStr = MsgCurveStr + "\r\n"
                            + "Scale \r\n"
                            + "TotalPoint: " + List_Scale.Curve_TotalPoint.ToString() + "\r\n";

                            MsgCurveStr = MsgCurveStr + "\r\n"
                            + "Param \r\n"
                            + "ID: " + List_Param.Comm.PC1_ID_00.ToString() + "\r\n"
                            + "Stage1 Speed: " + List_Param.Item1.PB1_VELCMD_03.ToString() + "\r\n";

                            MsgCurveStr = MsgCurveStr + "(Angle,Torque): \r\n";
                            float[] AnglePoint = List_Angle.ToArray();
                            float[] TorquePoint = List_Torq.ToArray();
                            for (int p = 0; p < List_Scale.Curve_TotalPoint; p++)
                            {
                                MsgCurveStr = MsgCurveStr + "(" + AnglePoint[p].ToString() + "," + TorquePoint[p].ToString("F3") + "),";
                            }
                        }
                        else
                        {
                            MsgCurveStr = MsgCurveStr + "Bin File does not exist";
                        }

                        MsgTB.Text = MsgCurveStr;
                        SM = StatusMachine.GetCaluNextReportID;
                        break;
                    case StatusMachine.GetCaluNextReportID:
                        if (LastReportRow != NowReportRow)
                        {
                            SM = StatusMachine.CheckReportID;
                        }
                        else
                        {
                            if (AutoRunningCB.Checked == true)
                                SM = StatusMachine.CheckReady;
                            else
                                SM = StatusMachine.CheckReportID;
                        }
                        break;
                    case StatusMachine.Rst_SendUnAutoLock:
                        int[] Send533_R = new int[10] { 533, 0, 0, 0, 0, 0, 1, 0, 0, 0 };// #533 Cmd + Clear Status
                        Modbus_Write(0xC8, Send533_R);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Rst_CheckUnAutoLock;
                        break;
                    case StatusMachine.Rst_CheckUnAutoLock:
                        int[] Read533_R = Modbus_Read(0xCF, 3);
                        if ((Read533_R[0] == 533) && (Read533_R[1] == 1) && (Read533_R[2] == 0))
                        {
                            SM = StatusMachine.Rst_SetAutoExportBinFile;//Success 
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Rst_SendUnAutoLock;//Retry
                        }
                        break;
                    case StatusMachine.Rst_SetAutoExportBinFile:
                        int[] Send517_R = new int[10] { 517, 0, 0, 0, 0, 0, 1, 0, 0, 0 };// #517 Cmd + Clear Status
                        Modbus_Write(0xC8, Send517_R);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Rst_CheckAutoExportBinFile;
                        break;
                    case StatusMachine.Rst_CheckAutoExportBinFile:
                        int[] Read517_R = Modbus_Read(0xCF, 3);
                        if ((Read517_R[0] == 517) && (Read517_R[1] == 1) && (Read517_R[2] == 0))
                        {
                            SM = StatusMachine.Rst_ClearDICmd;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                            {
                                SM = StatusMachine.Rst_SetAutoExportBinFile;//Retry
                            }
                        }
                        break;
                    case StatusMachine.Rst_ClearDICmd:
                        Modbus_Write(0x68, 0);//Clear DI Bit0
                        SM = StatusMachine.Stop;
                        break;
                    default:
                        break;
                }
                StatusTB.Text = "SM: " + SM.ToString();
            }
            catch
            {
                ModB.Connect();
            }
        }
        static bool IsTimeout(int TimeOut)
        {
            // 計算自 LastTickTime 以來的時間差
            TimeSpan elapsed = DateTime.Now - LastTickTime;

            // 檢查是否超過了設置的超時時間
            return elapsed.TotalMilliseconds >= TimeOut;
        }
        public bool UseFTPGetFile(uint BinID)
        {
            bool Rst = false;
            string url = "ftp://" + IPTB.Text + "/ScrewInfo/BIN/ID" + BinID.ToString() + ".bin";
            WebClient request = new WebClient();
            request.Credentials = new NetworkCredential("admin", "1234");

            if (CheckFileExists(url, request))
            {
                try
                {
                    if (!Directory.Exists(@".\ScrewInfo\BIN"))
                    {
                        Directory.CreateDirectory(@".\ScrewInfo\BIN");
                    }
                    byte[] RawBin = request.DownloadData(new Uri(url));
                    // Save Bin
                    using (BinaryWriter PicW = new BinaryWriter(File.Open(@".\ScrewInfo\BIN\ID" + BinID.ToString() + ".bin", FileMode.Create)))
                    {
                        for (int i = 0; i < RawBin.Length; i++)
                        {
                            PicW.Write(RawBin[i]);
                        }
                    }
                    // Parse 
                    ParseFileBin(RawBin);
                    Rst = true;
                }
                catch (WebException e)
                {
                }
            }
            return Rst;
        }

        private bool CheckFileExists(string url, WebClient request)
        {
            try
            {
                request.DownloadData(url); // 嘗試下載檔案大小
                return true; // 檔案存在
            }
            catch (WebException ex)
            {
                if (ex.Response is FtpWebResponse response)
                {
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false; // 檔案不存在
                    }
                }
                return false; // 檔案不存在
                //throw; // 重新拋出其他異常
            }
        }
        unsafe public void ParseFileBin(Byte[] Data8)
        {
            Int32 Size = System.Convert.ToInt32(Data8.Length);
            UInt16[] Data16 = new UInt16[Size / 2];
            for (int i = 0; i < Size / 2; i++)
            {
                Data16[i] = BitConverter.ToUInt16(Data8, i * 2);
            }
            double ChangeCoef = 1;
            double ParamConvertNmThenConvertUserUnitCoef = TorqUnitcoef(1000 + (int)Data16[121]) / TorqUnitcoef(1000 + (int)Data16[144]);
            ushort CurveType = Data16[8150 + 34];
            //Scale
            List_Scale.Stage1Angle = (int)(short)Data16[8150 + 0];
            List_Scale.Stage2Angle = (int)(short)Data16[8150 + 1];
            List_Scale.Stage3Angle = (int)(short)Data16[8150 + 2];
            List_Scale.Stage4Angle = (int)(short)Data16[8150 + 3];
            List_Scale.Stage5Angle = (int)(short)Data16[8150 + 4];
            List_Scale.Stage6Angle = (int)(short)Data16[8150 + 5];
            List_Scale.Loosening1Angle = (int)(short)Data16[8150 + 6];
            List_Scale.Loosening2Angle = (int)(short)Data16[8150 + 7];
            List_Scale.Stage1Torque = (float)Data16[8150 + 8] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage2Torque = (float)Data16[8150 + 9] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage3Torque = (float)Data16[8150 + 10] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage4Torque = (float)Data16[8150 + 11] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage5Torque = (float)Data16[8150 + 12] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage6Torque = (float)Data16[8150 + 13] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Loosening1Torque = (float)Data16[8150 + 14] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Loosening2Torque = (float)Data16[8150 + 15] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage1Time = (ushort)Data16[8150 + 16];
            List_Scale.Stage2Time = (ushort)Data16[8150 + 17];
            List_Scale.Stage3Time = (ushort)Data16[8150 + 18];
            List_Scale.Stage4Time = (ushort)Data16[8150 + 19];
            List_Scale.Stage5Time = (ushort)Data16[8150 + 20];
            List_Scale.Stage6Time = (ushort)Data16[8150 + 21];
            List_Scale.Loosening1Time = (ushort)Data16[8150 + 22];
            List_Scale.Loosening2Time = (ushort)Data16[8150 + 23];
            List_Scale.Curve_MaxTime = (float)(short)Data16[8150 + 24] / 1000;
            List_Scale.Curve_MaxAngle = (int)(short)Data16[8150 + 25];
            List_Scale.Curve_MaxTorque = (float)(short)Data16[8150 + 26] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Curve_MaxTorqueRate = (float)Data16[8150 + 27] * (float)ParamConvertNmThenConvertUserUnitCoef / 10000;
            List_Scale.Curve_TotalPoint = Data16[8150 + 28];
            List_Scale.SetMaxTorque = (float)Data16[8150 + 29] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.SetMinTorque = (float)Data16[8150 + 30] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            if (CurveType == 2)
                List_Scale.SetMaxTorqRate = (float)Data16[8150 + 31];//Speed
            else
                List_Scale.SetMaxTorqRate = (float)Data16[8150 + 31] * (float)ParamConvertNmThenConvertUserUnitCoef / 10000;

            List_Scale.SetMaxAngle = (int)Data16[8150 + 32];
            List_Scale.SetMinAngle = (int)Data16[8150 + 33];
            List_Scale.CurveVer = Data16[8150 + 34];
            List_Scale.CurveFreqModeVer = Data16[8150 + 35];
            if (CurveType == 2)
                List_Scale.CurveMaxTorqueRate = (float)Data16[8150 + 36];//Speed
            else
                List_Scale.CurveMaxTorqueRate = (float)Data16[8150 + 36] * (float)ParamConvertNmThenConvertUserUnitCoef / 10000;

            List_Scale.Curve_MinTime = (float)(short)Data16[8150 + 37] / 1000;
            List_Scale.Curve_MinAngle = (int)(short)Data16[8150 + 38];
            List_Scale.Curve_MinTorque = (float)(short)Data16[8150 + 39] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;

            if (CurveType == 2)
                List_Scale.Curve_MinTorqueRate = (float)(short)Data16[8150 + 40];//Speed
            else
                List_Scale.Curve_MinTorqueRate = (float)(short)Data16[8150 + 40] * (float)ParamConvertNmThenConvertUserUnitCoef / 10000;

            List_Scale.Stage1SwitchTorq = (float)Data16[8150 + 41] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage2SwitchTorq = (float)Data16[8150 + 42] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage3SwitchTorq = (float)Data16[8150 + 43] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage4SwitchTorq = (float)Data16[8150 + 44] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage5SwitchTorq = (float)Data16[8150 + 45] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Scale.Stage6SwitchTorq = (float)Data16[8150 + 46] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;

            if (CurveType == 2)
                ChangeCoef = 1;
            else
                ChangeCoef = ParamConvertNmThenConvertUserUnitCoef / 1000;

            for (int j = 0; j < List_Scale.Curve_TotalPoint; j++)
            {
                if (j < 2000)
                {
                    List_Time.Add((short)Data16[150 + j]);
                    List_Angle.Add((short)Data16[2150 + j]);
                    List_Torq.Add((float)(short)Data16[4150 + j] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000);
                    List_TorqRate.Add((float)(short)Data16[6150 + j] * (float)ChangeCoef);
                }
                else if (j < 4000)
                {
                    List_Time.Add((short)Data16[8750 + j - 2000]);
                    List_Angle.Add((short)Data16[8750 + 2000 + j - 2000]);
                    List_Torq.Add((float)(short)Data16[8750 + 4000 + j - 2000] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000);
                    List_TorqRate.Add((float)(short)Data16[8750 + 6000 + j - 2000] * (float)ChangeCoef);
                }
                else if (j < 6000)
                {
                    List_Time.Add((short)Data16[16750 + j - 4000]);
                    List_Angle.Add((short)Data16[16750 + 2000 + j - 4000]);
                    List_Torq.Add((float)(short)Data16[16750 + 4000 + j - 4000] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000);
                    List_TorqRate.Add((float)(short)Data16[16750 + 6000 + j - 4000] * (float)ChangeCoef);
                }
                else if (j < 8000)
                {
                    List_Time.Add((short)Data16[24750 + j - 6000]);
                    List_Angle.Add((short)Data16[24750 + 2000 + j - 6000]);
                    List_Torq.Add((float)(short)Data16[24750 + 4000 + j - 6000] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000);
                    List_TorqRate.Add((float)(short)Data16[24750 + 6000 + j - 6000] * (float)ChangeCoef);
                }
            }

            //Date
            DateTime OpTime = ((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddDays(Data16[100])).AddSeconds(Data16[102] * 65536 + Data16[101]);
            List_Info.Year = (uint)OpTime.Year;
            List_Info.Month = (uint)OpTime.Month;
            List_Info.Day = (uint)OpTime.Day;
            List_Info.Hour = (uint)OpTime.Hour;
            List_Info.Min = (uint)OpTime.Minute;
            List_Info.Sec = (uint)OpTime.Second;
            //Serial number
            List_Info.SN = Byte2AsciiStr(Data8, 200);
            //Report
            List_Info.Tool = Data16[103];
            List_Info.ScrewNo = (uint)(Data16[105] * 65536 + Data16[104]);
            List_Info.SeqID = Data16[106];
            List_Info.ParmID = Data16[107];
            List_Info.TargetTorque = (float)Data16[108] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.TargetAngle = Data16[109];
            List_Info.TargetTorqueRate = (float)Data16[110] * (float)ParamConvertNmThenConvertUserUnitCoef / 10000;
            List_Info.FinalTorque = (float)Data16[111] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.TighteningAngle = (int)Data16[112];
            List_Info.TotalAngle = (int)(short)Data16[113];
            List_Info.Status = Data16[114];
            List_Info.CT_Time = (float)Data16[115] / 1000;
            List_Info.ErrorCode = Data16[116];
            List_Info.MaxTighteningAngle = (int)Data16[117];
            List_Info.MinTighteningAngle = (int)Data16[118];
            List_Info.MaxTorque = (float)Data16[119] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.MinTorque = (float)Data16[120] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.TorqueUnit = Data16[121];
            List_Info.ToolMaxTorque = (float)Data16[122] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.ToolProtectTorque = (float)Data16[123] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.PreTighteningTorque = (float)Data16[124] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.SetMaxTime = (float)Data16[125] / 10;
            List_Info.SetMaxAngle = Data16[126];
            List_Info.FinalStage_SetMaxTorque = (float)Data16[127] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.FinalStage_SetMinTorque = (float)Data16[128] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.FinalStage_SetMaxAngle = Data16[129];
            List_Info.FinalStage_SetMinAngle = Data16[130];
            List_Info.FinalStage_SetMaxTime = (float)Data16[131] / 100;
            List_Info.FinalStage_SetMinTime = (float)Data16[132] / 100;
            List_Info.PrevailTorque = (float)Data16[133] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.AppliedTorque = (float)Data16[134] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.FinalCurrent = (float)Data16[135] / 100;
            List_Info.ClampTorque = (float)Data16[136] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.SetMaxClampTorque = (float)Data16[137] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.SetMinClampTorque = (float)Data16[138] * (float)ParamConvertNmThenConvertUserUnitCoef / 1000;
            List_Info.ClampAngle = Data16[139];
            List_Info.SetMaxClampAngle = Data16[140];
            List_Info.SetMinClampAngle = Data16[141];
            List_Info.SetMinAngle = Data16[142];
            List_Info.UserID = Data16[143];
            List_Info.RawDataUnit = Data16[144];
            List_Info.TargetYield = Data16[145];
            for (int i = 0; i < 550; i++)
            {
                List_Param.Data16[i] = Data16[8200 + i];
            }

            if (Data16.Length >= 32950)//有支援OtherInfo
            {
                Byte[] Data8_Arr = new Byte[40];
                Array.Copy(Data8, (32750 + 0) * 2, Data8_Arr, 0, 40);
                List_OtherInfo.ParameterTitle = Byte2AsciiStr(Data8_Arr, 40);
                Array.Copy(Data8, (32750 + 20) * 2, Data8_Arr, 0, 40);
                List_OtherInfo.SequenceTitle = Byte2AsciiStr(Data8_Arr, 40);
                Array.Copy(Data8, (32750 + 40) * 2, Data8_Arr, 0, 20);
                List_OtherInfo.ToolModelName = Byte2AsciiStr(Data8_Arr, 20);
                Array.Copy(Data8, (32750 + 50) * 2, Data8_Arr, 0, 20);
                List_OtherInfo.ToolSerialNumber = Byte2AsciiStr(Data8_Arr, 20);
                List_OtherInfo.ToolLifeCounter = (uint)(Data16[32750 + 61] * 65536 + Data16[32750 + 60]);
                List_OtherInfo.RepairToolLifeCounter = (uint)(Data16[32750 + 63] * 65536 + Data16[32750 + 62]);
                DateTime RepairTime = ((new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddDays(Data16[32750 + 64])).AddSeconds(Data16[32750 + 66] * 65536 + Data16[32750 + 65]);
                List_OtherInfo.RepairYear = (uint)RepairTime.Year;
                List_OtherInfo.RepairMonth = (uint)RepairTime.Month;
                List_OtherInfo.RepairDay = (uint)RepairTime.Day;
                List_OtherInfo.RepairHour = (uint)RepairTime.Hour;
                List_OtherInfo.RepairMin = (uint)RepairTime.Minute;
                List_OtherInfo.RepairSec = (uint)RepairTime.Second;
                List_OtherInfo.ToolMaxSpeed = Data16[32750 + 67];
                List_OtherInfo.TargetArmX = (int)(Data16[32750 + 69] * 65536 + Data16[32750 + 68]);
                List_OtherInfo.TargetArmY = (int)(Data16[32750 + 71] * 65536 + Data16[32750 + 70]);
                List_OtherInfo.TargetArmZ = (int)(Data16[32750 + 73] * 65536 + Data16[32750 + 72]);
                List_OtherInfo.StartTighteningArmX = (int)(Data16[32750 + 75] * 65536 + Data16[32750 + 74]);
                List_OtherInfo.StartTighteningArmY = (int)(Data16[32750 + 77] * 65536 + Data16[32750 + 76]);
                List_OtherInfo.StartTighteningArmZ = (int)(Data16[32750 + 79] * 65536 + Data16[32750 + 78]);
                Array.Copy(Data8, (32750 + 80) * 2, Data8_Arr, 0, 40);
                List_OtherInfo.ControllerName = Byte2AsciiStr(Data8_Arr, 40);
                List_OtherInfo.HMIMainVersion = Data16[32750 + 100];
                List_OtherInfo.FWVersion = Data16[32750 + 101];
                List_OtherInfo.HMISubVersion = Data16[32750 + 102];
                List_OtherInfo.ReportID = (uint)(Data16[32750 + 104] * 65536 + Data16[32750 + 103]);
            }
        }

        public string Byte2AsciiStr(byte[] Src, uint size)
        {
            byte[] Det = new byte[size];
            Array.Copy(Src, Det, size);
            //return System.Text.UnicodeEncoding.ASCII.GetString(Det);
            return System.Text.Encoding.ASCII.GetString(Det).Trim('*', '/');
        }
        public double TorqUnitcoef(int Mode)
        {
            int TorqUnit;
            double coef;
            if (Mode == 0)
                TorqUnit = 1;//system
            else if (Mode == 1)
                TorqUnit = 1; //系統觀看單位
            else if (Mode == 2)
                TorqUnit = 1; //運行結果觀看單位X
            else if (Mode == 3)
                TorqUnit = 1; //運行結果觀看單位Y
            else if (Mode == 1000)
                TorqUnit = 99;
            else if (Mode == 1001)
                TorqUnit = 1;
            else if (Mode == 1002)
                TorqUnit = 2;
            else if (Mode == 1003)
                TorqUnit = 3;
            else if (Mode == 1004)
                TorqUnit = 4;
            else if (Mode == 1005)
                TorqUnit = 5;
            else if (Mode == 1006)
                TorqUnit = 6;
            else if (Mode == 1050)
                TorqUnit = 50;
            else
                TorqUnit = 99;

            if (TorqUnit == 1) //Kgf.cm
                coef = 10.197;
            else if (TorqUnit == 2) //lbf.ft
                coef = 0.737;
            else if (TorqUnit == 3)//lbf.in
                coef = 8.849;
            else if (TorqUnit == 4)//ozf.ft
                coef = 11.801;
            else if (TorqUnit == 5)//lbf.in
                coef = 141.612;
            else if (TorqUnit == 6)//cNm
                coef = 100.0;
            else if (TorqUnit == 50)//Mini
                coef = 50.0;
            else //Nm
                coef = 1;

            return coef;
        }
        private int Modbus_Read(int ardesss)
        {
            int[] data = new int[1];
            try
            {
                data = ModB.ReadHoldingRegisters(ardesss, 1);
            }
            catch
            {

            }
            return data[0];
        } // 
        private int[] Modbus_Read(int addresss, int size)
        {
            int WindowSize = 120;
            int[] RstData = new int[size];
            for (int offs = 0; offs < size; offs = offs + WindowSize)
            {
                int CmdSize = (offs + WindowSize >= size) ? size - offs : WindowSize;
                int[] data = new int[CmdSize];
                try 
                { 
                    data = ModB.ReadHoldingRegisters(addresss + offs, CmdSize);
                }
                catch
                {

                }
                Array.Copy(data, 0, RstData, offs, CmdSize);
            }
            return RstData;
        } // 

        private void Modbus_Write(int ardress, int data)
        {
            try
            {
                ModB.WriteSingleRegister(ardress, data);
            }
            catch
            {

            }
        }
        private void Modbus_Write(int address, int[] data)
        {
            int WindowSize = 120;
            int Size = data.Length;
            for (int offs = 0; offs < Size; offs = offs + WindowSize)
            {
                int CmdSize = (offs + WindowSize >= Size) ? Size - offs : WindowSize;
                int[] Rstdata = new int[CmdSize];
                Array.Copy(data, offs, Rstdata, 0, CmdSize);
                try
                {
                    ModB.WriteMultipleRegisters(address, Rstdata);
                }
                catch
                {

                }
            }
        }

        private void StartBn_Click(object sender, EventArgs e)
        {
            if (TimerEvent != null)
                TimerEvent.Start();

            RstBn.Enabled = AutoRunningCB.Enabled = false;
            SM = StatusMachine.Init_GetReportID;
        }

        private void StopBn_Click(object sender, EventArgs e)
        {
            RstBn.Enabled = AutoRunningCB.Enabled = true;
            Modbus_Write(0x68, 0);//Send DI Bit0
            SM = StatusMachine.Stop;
            StatusTB.Text = "";
            if (TimerEvent != null)
                TimerEvent.Stop();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Modbus_Write(0x68, 0);
            if (TimerEvent != null)
                TimerEvent.Stop();
        }

        private void RstBn_Click(object sender, EventArgs e)
        {
            if (TimerEvent != null)
                TimerEvent.Start();

            SM = StatusMachine.Rst_SendUnAutoLock;
        }

        private void S1Bn_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Delta_C_.Properties.Resources.Step1Pic;
        }

        private void S2Bn_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Delta_C_.Properties.Resources.Step2Pic;
        }

        private void S3Bn_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Delta_C_.Properties.Resources.Step3Pic;
        }

        private void S4Bn_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Delta_C_.Properties.Resources.Step4Pic;
        }

        private void S5Bn_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Delta_C_.Properties.Resources.Step5Pic;
        }


        private void AutoRunningCB_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = (AutoRunningCB.Checked == true) ? Delta_C_.Properties.Resources.DIStart : Delta_C_.Properties.Resources.LeverStart;
        }

        private void ConnHelpBn_Click(object sender, EventArgs e)
        {
            Process.Start("ncpa.cpl");
        }
    }
}
