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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ScrewDriver
{
    public partial class Form1 : Form
    {
        ModbusClient ModB = new ModbusClient();
        private System.Windows.Forms.Timer TimerEvent;
        private int SM = 0;
        private int LastReportRow = 0;
        private int NowReportRow = 0;
        private static DateTime LastTickTime = DateTime.Now;
        private const int TimeOut = 3000;
        private int CurveVer = 0;
        private int[] CurveAngle = new int[8000];
        private int[] CurveTorque = new int[16000];
        private int[] Curve = new int[2000];
        private string MsgCurveStr = "";
        private int CaluAnglePoint = 0;
        private int CaluTorquePoint = 0;
        private ReportInfoStuc Info = new ReportInfoStuc();
        private ReportScaleStuc Scale = new ReportScaleStuc();
        public ParamCommItemVer1 Param = new ParamCommItemVer1();
        public static class StatusMachine
        {
            public const int Stop = 0;
            public const int Init_GetReportID = 10;
            public const int Init_ClearCmd = 11;
            public const int Init_SendUnAutoLock = 12;
            public const int Init_CheckUnAutoLock = 13;
            public const int Init_SendCurveVer = 14;
            public const int Init_CheckCurveVer = 15;
            public const int CheckReady = 100;
            public const int SendDICmd = 101;
            public const int CheckDISend = 102;
            public const int CheckReportID = 103;
            public const int ClearCmd = 104;
            public const int CheckDIClear = 105;
            public const int Send750Cmd = 200;
            public const int Check750Status = 201;
            public const int Read750Info = 202;
            public const int Send751Cmd_Scale = 300;
            public const int Check751Status_Scale = 301;
            public const int Read751Info_Scale = 302;
            public const int Send751Cmd_AngleInit = 310;
            public const int Send751Cmd_AngleLoop = 311;
            public const int Check751Status_AngleLoop = 312;
            public const int Read751Info_AngleLoop = 313;
            public const int Send751Cmd_TorqueInit = 320;
            public const int Send751Cmd_TorqueLoop = 321;
            public const int Check751Status_TorqueLoop = 322;
            public const int Read751Info_TorqueLoop = 323;
            public const int Send751Cmd_Param = 330;
            public const int Check751Status_Param = 331;
            public const int Read751Info_Param = 332;
            public const int CaluNextReportID = 999;
            public const int Rst_SendUnAutoLock = 3000;
            public const int Rst_CheckUnAutoLock = 3001;
            public const int Rst_ClearDICmd = 3002;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        unsafe public struct ReportInfoStuc
        {
            [FieldOffset(0)]
            public fixed ushort Data16[600 / 2];
            [FieldOffset(0)]
            public fixed ushort SaveStr[100];
            [FieldOffset(200)]
            public ushort Year;
            [FieldOffset(202)]
            public ushort Month;
            [FieldOffset(204)]
            public ushort Day;
            [FieldOffset(206)]
            public ushort Hour;
            [FieldOffset(208)]
            public ushort Min;
            [FieldOffset(210)]
            public ushort Sec;
            [FieldOffset(212)]
            public ushort Tool;
            [FieldOffset(214)]
            public uint ScrewNo; //DW
            [FieldOffset(218)]
            public ushort SeqID;
            [FieldOffset(220)]
            public ushort ParmID;
            [FieldOffset(222)]
            public ushort TargetTorque;
            [FieldOffset(224)]
            public ushort TargetAngle;
            [FieldOffset(226)]
            public ushort TargetTorqueRate;
            [FieldOffset(228)]
            public ushort FinalTorque;
            [FieldOffset(230)]
            public ushort TighteningAngle;
            [FieldOffset(232)]
            public short TotalAngle;
            [FieldOffset(234)]
            public ushort Status;
            [FieldOffset(236)]
            public ushort CT_Time;
            [FieldOffset(238)]
            public ushort ErrorCode;
            [FieldOffset(240)]
            public ushort MaxTighteningAngle;
            [FieldOffset(242)]
            public ushort MinTighteningAngle;
            [FieldOffset(244)]
            public ushort MaxTorque;
            [FieldOffset(246)]
            public ushort MinTorque;
            [FieldOffset(248)]
            public ushort TorqueUnit;
            [FieldOffset(250)]
            public ushort ToolMaxTorque_NM;
            [FieldOffset(252)]
            public ushort ToolProtectTorque;
            [FieldOffset(254)]
            public ushort PreTighteningTorque;
            [FieldOffset(256)]
            public ushort SetMaxTime;
            [FieldOffset(258)]
            public ushort SetMaxAngle;
            [FieldOffset(260)]
            public ushort FinalStage_SetMaxTorque;
            [FieldOffset(262)]
            public ushort FinalStage_SetMinTorque;
            [FieldOffset(264)]
            public ushort FinalStage_SetMaxAngle;
            [FieldOffset(266)]
            public ushort FinalStage_SetMinAngle;
            [FieldOffset(268)]
            public ushort FinalStage_SetMaxTime;
            [FieldOffset(270)]
            public ushort FinalStage_SetMinTime;
            [FieldOffset(272)]
            public ushort PrevailTorque;
            [FieldOffset(274)]
            public ushort AppliedTorque;
            [FieldOffset(276)]
            public ushort FinalCurrent;
            [FieldOffset(278)]
            public ushort ClampTorque;
            [FieldOffset(280)]
            public ushort SetMaxClampTorque;
            [FieldOffset(282)]
            public ushort SetMinClampTorque;
            [FieldOffset(284)]
            public ushort ClampAngle;
            [FieldOffset(286)]
            public ushort SetMaxClampAngle;
            [FieldOffset(288)]
            public ushort SetMinClampAngle;
            [FieldOffset(290)]
            public ushort SetMinAngle;
            [FieldOffset(292)]
            public ushort UserID;
            [FieldOffset(294)]
            public ushort FWSystemCoef;
            [FieldOffset(296)]
            public ushort TargetYield;
            [FieldOffset(298)]
            public fixed ushort Rreserve[4];
            [FieldOffset(306)]
            public uint TargetTorque_DW;
            [FieldOffset(310)]
            public uint TargetTorqueRate_DW;
            [FieldOffset(314)]
            public uint FinalTorque_DW;
            [FieldOffset(318)]
            public uint MaxTorque_DW;
            [FieldOffset(322)]
            public uint MinTorque_DW;
            [FieldOffset(326)]
            public uint PreTighteningTorque_DW;
            [FieldOffset(330)]
            public uint FinalStage_SetMaxTorque_DW;
            [FieldOffset(334)]
            public uint FinalStage_SetMinTorque_DW;
            [FieldOffset(338)]
            public uint PrevailTorque_DW;
            [FieldOffset(342)]
            public uint AppliedTorque_DW;
            [FieldOffset(346)]
            public uint ClampTorque_DW;
            [FieldOffset(350)]
            public uint SetMaxClampTorque_DW;
            [FieldOffset(354)]
            public uint ToolMaxTorque_DW;
            [FieldOffset(358)]
            public uint ToolProtectTorque_DW;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        unsafe public struct ReportScaleStuc
        {
            [FieldOffset(0)]
            public fixed ushort Data16[600 / 2];
            [FieldOffset(0)]
            public short Stage1Angle;
            [FieldOffset(2)]
            public short Stage2Angle;
            [FieldOffset(4)]
            public short Stage3Angle;
            [FieldOffset(6)]
            public short Stage4Angle;
            [FieldOffset(8)]
            public short Stage5Angle;
            [FieldOffset(10)]
            public short Stage6Angle;
            [FieldOffset(12)]
            public short Loosening1Angle;
            [FieldOffset(14)]
            public short Loosening2Angle;
            [FieldOffset(16)]
            public short Stage1Torque;
            [FieldOffset(18)]
            public short Stage2Torque;
            [FieldOffset(20)]
            public short Stage3Torque;
            [FieldOffset(22)]
            public short Stage4Torque;
            [FieldOffset(24)]
            public short Stage5Torque;
            [FieldOffset(26)]
            public short Stage6Torque;
            [FieldOffset(28)]
            public short Loosening1Torque;
            [FieldOffset(30)]
            public short Loosening2Torque;
            [FieldOffset(32)]
            public ushort Stage1Time;
            [FieldOffset(34)]
            public ushort Stage2Time;
            [FieldOffset(36)]
            public ushort Stage3Time;
            [FieldOffset(38)]
            public ushort Stage4Time;
            [FieldOffset(40)]
            public ushort Stage5Time;
            [FieldOffset(42)]
            public ushort Stage6Time;
            [FieldOffset(44)]
            public ushort Loosening1Time;
            [FieldOffset(46)]
            public ushort Loosening2Time;
            [FieldOffset(48)]
            public short Curve_MaxTime;
            [FieldOffset(50)]
            public short Curve_MaxAngle;
            [FieldOffset(52)]
            public short Curve_MaxTorque;
            [FieldOffset(54)]
            public short Curve_MaxTorqueRate;
            [FieldOffset(56)]
            public ushort Curve_TotalPoint;
            [FieldOffset(58)]
            public ushort SetMaxTorque;
            [FieldOffset(60)]
            public ushort SetMinTorque;
            [FieldOffset(62)]
            public ushort SetMaxTorqRate;
            [FieldOffset(64)]
            public ushort SetMaxAngle;
            [FieldOffset(66)]
            public ushort SetMinAngle;
            [FieldOffset(68)]
            public ushort CurveVer;
            [FieldOffset(70)]
            public ushort CurveFreqModeVer;
            [FieldOffset(72)]
            public short CurveMaxTorqueRate;
            [FieldOffset(74)]
            public short Curve_MinTime;
            [FieldOffset(76)]
            public short Curve_MinAngle;
            [FieldOffset(78)]
            public short Curve_MinTorque;
            [FieldOffset(80)]
            public short Curve_MinTorqueRate;
            [FieldOffset(82)]
            public short Stage1SwitchTorq;
            [FieldOffset(84)]
            public short Stage2SwitchTorq;
            [FieldOffset(86)]
            public short Stage3SwitchTorq;
            [FieldOffset(88)]
            public short Stage4SwitchTorq;
            [FieldOffset(90)]
            public short Stage5SwitchTorq;
            [FieldOffset(92)]
            public short Stage6SwitchTorq;
            [FieldOffset(84)]
            public fixed ushort Rreserve[3];
            [FieldOffset(100)]
            public int Stage1Torque_DW;
            [FieldOffset(104)]
            public int Stage2Torque_DW;
            [FieldOffset(108)]
            public int Stage3Torque_DW;
            [FieldOffset(112)]
            public int Stage4Torque_DW;
            [FieldOffset(116)]
            public int Stage5Torque_DW;
            [FieldOffset(120)]
            public int Stage6Torque_DW;
            [FieldOffset(124)]
            public int Loosening1Torque_DW;
            [FieldOffset(128)]
            public int Loosening2Torque_DW;
            [FieldOffset(132)]
            public int Stage7Torque_DW;
            [FieldOffset(136)]
            public int Stage8Torque_DW;
            [FieldOffset(140)]
            public int Stage9Torque_DW;
            [FieldOffset(144)]
            public int StageATorque_DW;
            [FieldOffset(148)]
            public int Curve_MaxTorque_DW;
            [FieldOffset(152)]
            public int Curve_MaxTorqueRate_DW;
            [FieldOffset(156)]
            public int SetMaxTorque_DW;
            [FieldOffset(160)]
            public int SetMinTorque_DW;
            [FieldOffset(164)]
            public int SetMaxTorqRate_DW;
            [FieldOffset(168)]
            public uint CurveMaxTorqueRate_DW;
            [FieldOffset(172)]
            public int Curve_MinTorque_DW;
            [FieldOffset(176)]
            public int Curve_MinTorqueRate_DW;
            [FieldOffset(180)]
            public int Stage1SwitchTorq_DW;
            [FieldOffset(184)]
            public int Stage2SwitchTorq_DW;
            [FieldOffset(188)]
            public int Stage3SwitchTorq_DW;
            [FieldOffset(192)]
            public int Stage4SwitchTorq_DW;
            [FieldOffset(196)]
            public int Stage5SwitchTorq_DW;
            [FieldOffset(200)]
            public int Stage6SwitchTorq_DW;
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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Example_ModbusTCP_V0.0.0.3";
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
        unsafe private void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                switch (SM)
                {
                    case StatusMachine.Init_GetReportID:
                        int[] Row = Modbus_Read(0x6B, 2);
                        LastReportRow = Row[1] * 65536 + Row[0];
                        SM = StatusMachine.Init_ClearCmd;
                        break;
                    case StatusMachine.Init_ClearCmd:
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
                            SM = StatusMachine.Init_SendCurveVer;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Init_SendUnAutoLock;//Retry
                        }
                        break;
                    case StatusMachine.Init_SendCurveVer:
                        int[] Send562 = new int[10] { 562, 0, 0, 0, 0, 0, 1, 0, 0, 0 };// #533 Cmd + Clear Status
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
                            SM = StatusMachine.SendDICmd;

                        break;
                    case StatusMachine.SendDICmd:
                        Modbus_Write(0x68, 1);//Send DI Bit0
                        SM = StatusMachine.CheckDISend;
                        break;
                    case StatusMachine.CheckDISend:
                        if ((Modbus_Read(0x67) & 0x1) == 1) //Check DI Bit0
                            SM = StatusMachine.CheckReportID;

                        break;
                    case StatusMachine.CheckReportID:
                        int[] NowRow = Modbus_Read(0x6B, 2);
                        NowReportRow = NowRow[1] * 65536 + NowRow[0];
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

                            MsgCurveStr = "ReportID:" + LastReportRow.ToString() + "\r\n";
                            MsgTB.Text = MsgCurveStr;
                            SM = StatusMachine.ClearCmd;
                        }
                        break;
                    case StatusMachine.ClearCmd:
                        Modbus_Write(0x68, 0);//Clear DI Bit0
                        SM = StatusMachine.CheckDIClear;
                        break;
                    case StatusMachine.CheckDIClear:
                        if ((Modbus_Read(0x67) & 0x1) == 0) //Check DI Bit0
                            SM = StatusMachine.Send750Cmd;

                        break;
                    case StatusMachine.Send750Cmd:
                        int[] Send750 = new int[10] { 750, 0, (int)(LastReportRow % 0xFFFF), (int)(LastReportRow / 0x10000), 0, 0, 1, 0, 0, 0 };// #750 Cmd + Clear Status
                        Modbus_Write(0xC8, Send750); 
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Check750Status;
                        break;
                    case StatusMachine.Check750Status:
                        int[] Read750ST = Modbus_Read(0xCF, 3);
                        if ((Read750ST[0] == 750) && (Read750ST[1] == 1) && (Read750ST[2] == 0))
                        {
                            SM = StatusMachine.Read750Info;//Success 
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Send750Cmd;//Retry
                        }
                        break;
                    case StatusMachine.Read750Info:
                        int[] ReportInfo = Modbus_Read(0xD2, 253);
                        for (int i=0;i < ReportInfo.Length;i++)
                        {
                            Info.Data16[i] = (ushort)ReportInfo[i];
                        }
                        MsgCurveStr = MsgCurveStr
                            + "#750(Info) \r\n"
                            + "0x13C(Tool): " + Info.Tool.ToString() + "\r\n"
                            + "0x145(Tightening Angle):" + Info.TighteningAngle.ToString() + "\r\n"
                            + "0x146(Total Angle):" + Info.TotalAngle.ToString() + "\r\n"
                            + "0x147(Status):" + Info.Status.ToString() + "\r\n"
                            + "0x148(CT):" + Info.CT_Time.ToString() + "\r\n"
                            + "0x17D,0x17E(Final + Prevail Torque):" + ((double)(Info.AppliedTorque_DW) / 1000).ToString("F3") + "\n";

                        MsgTB.Text = MsgCurveStr;
                        SM = StatusMachine.Send751Cmd_Scale;//Success 
                        break;
                    case StatusMachine.Send751Cmd_Scale:
                        int[] Send751 = new int[10] { 751, 0, (int)(LastReportRow % 0xFFFF), (int)(LastReportRow / 0x10000), 10, 0, 1, 0, 0, 0 };// #751 Cmd + Clear Status
                        Modbus_Write(0xC8, Send751);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Check751Status_Scale;
                        break;
                    case StatusMachine.Check751Status_Scale:
                        int[] Read751ST = Modbus_Read(0xCF, 3);
                        if ((Read751ST[0] == 751) && (Read751ST[1] == 1) && (Read751ST[2] == 0))
                        {
                            SM = StatusMachine.Read751Info_Scale;//Success 
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                            {
                                SM = StatusMachine.Send751Cmd_Scale;//Retry
                            }
                        }
                        break;
                    case StatusMachine.Read751Info_Scale:
                        int[] ScaleInfo = Modbus_Read(0xD2, 150);
                        for (int i = 0; i < ScaleInfo.Length; i++)
                        {
                            Scale.Data16[i] = (ushort)ScaleInfo[i];
                        }
                        MsgCurveStr = MsgCurveStr + "\r\n"
                        + "#751(Scale) \r\n"
                        + "TotalPoint: " + Scale.Curve_TotalPoint.ToString() + "\r\n";
                        MsgTB.Text = MsgCurveStr;

                        SM = StatusMachine.Send751Cmd_AngleInit;//Success 
                        break;
                    case StatusMachine.Send751Cmd_AngleInit:
                        CaluAnglePoint = 0;//Clear
                        SM = StatusMachine.Send751Cmd_AngleLoop;//Success 
                        break;
                    case StatusMachine.Send751Cmd_AngleLoop:
                        int Mode = 0;
                        if (CaluAnglePoint < 2000)
                            Mode = 1;
                        else if (CaluAnglePoint < 4000)
                            Mode = 21;
                        else if (CaluAnglePoint < 6000)
                            Mode = 31;
                        else if (CaluAnglePoint < 8000)
                            Mode = 41;

                        int[] Send751_1 = new int[10] { 751, 0, (int)(LastReportRow % 0xFFFF), (int)(LastReportRow / 0x10000), Mode, 0, 1, 0, 0, 0 };// #751 Cmd + Clear Status
                        Modbus_Write(0xC8, Send751_1);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Check751Status_AngleLoop;
                        break;
                    case StatusMachine.Check751Status_AngleLoop:
                        Read751ST = Modbus_Read(0xCF, 3);
                        if ((Read751ST[0] == 751) && (Read751ST[1] == 1) && (Read751ST[2] == 0))
                        {
                            SM = StatusMachine.Read751Info_AngleLoop;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                            {
                                SM = StatusMachine.Send751Cmd_AngleLoop;//Retry
                            }
                        }
                        break;
                    case StatusMachine.Read751Info_AngleLoop:
                        int EachCycleCurvePoint = 0;
                        if ((Scale.Curve_TotalPoint - CaluAnglePoint) >= 2000) // Each cycle is limited to 2000 words.
                            EachCycleCurvePoint = 2000;
                        else
                            EachCycleCurvePoint = Scale.Curve_TotalPoint - CaluAnglePoint;

                        Curve = Modbus_Read(0xD2, EachCycleCurvePoint);
                        if(CaluAnglePoint < 2000)
                            Array.Copy(Curve, 0, CurveAngle, 0, EachCycleCurvePoint);
                        else if (CaluAnglePoint < 4000)
                            Array.Copy(Curve, 0, CurveAngle, 2000, EachCycleCurvePoint);
                        else if (CaluAnglePoint < 6000)
                            Array.Copy(Curve, 0, CurveAngle, 4000, EachCycleCurvePoint);
                        else if (CaluAnglePoint < 8000)
                            Array.Copy(Curve, 0, CurveAngle, 6000, EachCycleCurvePoint);


                        CaluAnglePoint = CaluAnglePoint + EachCycleCurvePoint;
                        if (CaluAnglePoint < Scale.Curve_TotalPoint)
                            SM = StatusMachine.Send751Cmd_AngleLoop;//Repeat
                        else
                            SM = StatusMachine.Send751Cmd_TorqueInit;//Success
                        break;
                    case StatusMachine.Send751Cmd_TorqueInit:
                        CaluTorquePoint = 0;//Clear
                        SM = StatusMachine.Send751Cmd_TorqueLoop;//Success 
                        break;
                    case StatusMachine.Send751Cmd_TorqueLoop:
                        int Mode_2 = 0;
                        if (CaluAnglePoint < 1000)
                            Mode_2 = 4;
                        else if(CaluAnglePoint < 2000)
                            Mode_2 = 5;
                        else if (CaluAnglePoint < 3000)
                            Mode_2 = 24;
                        else if (CaluAnglePoint < 4000)
                            Mode_2 = 25;
                        else if (CaluAnglePoint < 5000)
                            Mode_2 = 34;
                        else if (CaluAnglePoint < 6000)
                            Mode_2 = 35;
                        else if (CaluAnglePoint < 7000)
                            Mode_2 = 44;
                        else if (CaluAnglePoint < 8000)
                            Mode_2 = 45;

                        int[] Send751_2 = new int[10] { 751, 0, (int)(LastReportRow % 0xFFFF), (int)(LastReportRow / 0x10000), Mode_2, 0, 1, 0, 0, 0 };// #751 Cmd + Clear Status
                        Modbus_Write(0xC8, Send751_2);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Check751Status_TorqueLoop;
                        break;
                    case StatusMachine.Check751Status_TorqueLoop:
                        Read751ST = Modbus_Read(0xCF, 3);
                        if ((Read751ST[0] == 751) && (Read751ST[1] == 1) && (Read751ST[2] == 0))
                        {
                            SM = StatusMachine.Read751Info_TorqueLoop;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                            {
                                SM = StatusMachine.Send751Cmd_TorqueLoop;//Retry
                            }
                        }
                        break;
                    case StatusMachine.Read751Info_TorqueLoop:
                        int EachCycleTorquePoint = 0;
                        if ((Scale.Curve_TotalPoint - CaluTorquePoint) >= 1000) // Each cycle is limited to 1000 words.
                            EachCycleTorquePoint = 1000;
                        else
                            EachCycleTorquePoint = Scale.Curve_TotalPoint - CaluTorquePoint;

                        Curve = Modbus_Read(0xD2, 2 * EachCycleTorquePoint);
                        if (Scale.Curve_TotalPoint < 1000)
                            Array.Copy(Curve, 0, CurveTorque, 0, 2 * EachCycleTorquePoint);
                        else if (Scale.Curve_TotalPoint < 2000)
                            Array.Copy(Curve, 0, CurveTorque, 2000, 2 * EachCycleTorquePoint);
                        else if (Scale.Curve_TotalPoint < 3000)
                            Array.Copy(Curve, 0, CurveTorque, 4000, 2 * EachCycleTorquePoint);
                        else if (Scale.Curve_TotalPoint < 4000)
                            Array.Copy(Curve, 0, CurveTorque, 6000, 2 * EachCycleTorquePoint);
                        else if(Scale.Curve_TotalPoint < 5000)
                            Array.Copy(Curve, 0, CurveTorque, 8000, 2 * EachCycleTorquePoint);
                        else if (Scale.Curve_TotalPoint < 6000)
                            Array.Copy(Curve, 0, CurveTorque, 10000, 2 * EachCycleTorquePoint);
                        else if (Scale.Curve_TotalPoint < 7000)
                            Array.Copy(Curve, 0, CurveTorque, 12000, 2 * EachCycleTorquePoint);
                        else if (Scale.Curve_TotalPoint < 8000)
                            Array.Copy(Curve, 0, CurveTorque, 14000, 2 * EachCycleTorquePoint);


                        CaluTorquePoint = CaluTorquePoint + EachCycleTorquePoint;
                        if (CaluTorquePoint < Scale.Curve_TotalPoint)
                            SM = StatusMachine.Send751Cmd_TorqueLoop;//Repeat
                        else
                            SM = StatusMachine.Send751Cmd_Param;//Success
                        break;
                    case StatusMachine.Send751Cmd_Param:
                        int[] Send751_Param = new int[10] { 751, 0, (int)(LastReportRow % 0xFFFF), (int)(LastReportRow / 0x10000), 11, 0, 1, 0, 0, 0 };// #751 Cmd + Clear Status
                        Modbus_Write(0xC8, Send751_Param);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Check751Status_Param;
                        break;
                    case StatusMachine.Check751Status_Param:
                        int[] Read751_P = Modbus_Read(0xCF, 3);
                        if ((Read751_P[0] == 751) && (Read751_P[1] == 1) && (Read751_P[2] == 0))
                        {
                            SM = StatusMachine.Read751Info_Param;//Success 
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                            {
                                SM = StatusMachine.Send751Cmd_Param;//Retry
                            }
                        }
                        break;
                    case StatusMachine.Read751Info_Param:
                        int[] ParamInfo = Modbus_Read(0xD2, 550);
                        for (int i = 0; i < ParamInfo.Length; i++)
                        {
                            Param.Data16[i] = (ushort)ParamInfo[i];
                        }
                        MsgCurveStr = MsgCurveStr + "\r\n"
                        + "#751(Param) \r\n"
                        + "ID: " + Param.Comm.PC1_ID_00.ToString() + "\r\n"
                        + "Stage1 Speed: " + Param.Item1.PB1_FINVELCMD_02.ToString() + "\r\n";
                        MsgTB.Text = MsgCurveStr;

                        SM = StatusMachine.CaluNextReportID;//Success 
                        break;
                    case StatusMachine.CaluNextReportID:
                        if (LastReportRow != NowReportRow)
                        {
                            SM = StatusMachine.CheckReportID;
                        }
                        else
                        {
                            if(AutoRunningCB.Checked == false)
                                SM = StatusMachine.CheckReportID;
                            else
                                SM = StatusMachine.CheckReady;
                        }
                        //==== Show Message === //
                        MsgCurveStr = MsgCurveStr + "(Angle,Torque): \r\n";
                        for (int p = 0; p < Scale.Curve_TotalPoint; p++)
                        {
                            MsgCurveStr = MsgCurveStr + "(" + ((short)CurveAngle[p]).ToString() +","+ ((double)((ushort)CurveTorque[2 * p + 1] * 65536 + (ushort)CurveTorque[2 * p]) / 1000).ToString("F3") + ")";
                        }
                        MsgTB.Text = MsgCurveStr;
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
                            SM = StatusMachine.Rst_ClearDICmd;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Rst_SendUnAutoLock;//Retry
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
                Array.Copy(data,0, RstData, offs, CmdSize);
            }
            return RstData;
        } // 
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

        private void StartBn_Click(object sender, EventArgs e)
        {
            if (TimerEvent != null)
                TimerEvent.Start();

            SM = StatusMachine.Init_GetReportID;
            RstBn.Enabled = AutoRunningCB.Enabled = false;
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
            Modbus_Write(0x68, 0);//Send DI Bit0
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
