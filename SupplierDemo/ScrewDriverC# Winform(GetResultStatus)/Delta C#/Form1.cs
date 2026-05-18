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
        private static DateTime LastTickTime = DateTime.Now;
        private const int TimeOut = 3000;
        private string MsgCurveStr = "";
        private int ParamID = 1;
        public static class StatusMachine
        {
            public const int Stop = 0;
            public const int Init_ClearCmd = 10;
            public const int Init_ClearFinishCmd = 11;
            public const int Init_SendAutoLock = 12;
            public const int Init_CheckAutoLock = 13;
            public const int Init_SendUnlockCmd = 14;
            public const int Init_CheckUnlockCmd = 15;
            public const int CheckReady = 100;
            public const int SendDICmd = 101;
            public const int CheckDISend = 102;
            public const int CheckTigheningStatus = 103;
            public const int ClearCmd = 104;
            public const int CheckDIClear = 105;
            public const int ClearFinishFlag = 107;
            public const int SendUnlockCmd = 108;
            public const int CheckUnlockCmd = 109;
            public const int Rst_SendUnAutoLock = 3000;
            public const int Rst_CheckUnAutoLock = 3001;
            public const int Rst_ClearDICmd = 3002;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Example_GetResultStatus_V0.0.0.0";
            RstBn.Enabled = AutoRunningCB.Enabled = StartBn.Enabled = StopBn.Enabled = false;
            TimerEvent = new System.Windows.Forms.Timer();
            TimerEvent.Interval = 100;
            TimerEvent.Tick += Timer_Tick;
            pictureBox1.Image = Delta_C_.Properties.Resources.DIStart;
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
                    case StatusMachine.Init_ClearCmd:
                        Modbus_Write(0x68, 0);//Clear DI Bit0
                        SM = StatusMachine.Init_ClearFinishCmd;
                        break;
                    case StatusMachine.Init_ClearFinishCmd:
#if NeverClear
                        Modbus_Write(0x26, 0);//Clear Finish Flag
#else
                        Modbus_Write(0x1F5D, 0);//Clear Finish Flag
#endif
                        SM = StatusMachine.Init_SendAutoLock;
                        break;
                    case StatusMachine.Init_SendAutoLock:
                        int[] Send533 = new int[10] { 533, 0, 1, 0, 0, 0, 1, 0, 0, 0 };// #533 Cmd + Clear Status
                        Modbus_Write(0xC8, Send533);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Init_CheckAutoLock;
                        break;
                    case StatusMachine.Init_CheckAutoLock:
                        int[] Read533ST = Modbus_Read(0xCF, 3);
                        if ((Read533ST[0] == 533) && (Read533ST[1] == 1) && (Read533ST[2] == 0))
                        {
                            SM = StatusMachine.Init_SendUnlockCmd;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Init_SendAutoLock;//Retry
                        }
                        break;
                    case StatusMachine.Init_SendUnlockCmd:
                        int[] Send406 = new int[10] { 406, 0, 0, 0, 0, 0, 1, 0, 0, 0 };// #406 Cmd + Clear Status
                        Modbus_Write(0xC8, Send406);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.Init_CheckUnlockCmd;
                        break;
                    case StatusMachine.Init_CheckUnlockCmd:
                        int[] Read406ST = Modbus_Read(0xCF, 3);
                        if ((Read406ST[0] == 406) && (Read406ST[1] == 1) && (Read406ST[2] == 0))
                        {
                            SM = StatusMachine.CheckReady;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.Init_CheckUnlockCmd;//Retry
                        }
                        break;
                        ///////////////////////////////
                    case StatusMachine.CheckReady:
                        if (Modbus_Read(0x1F52) == 1)
                        {
                            if (AutoRunningCB.Checked == true)
                                SM = StatusMachine.SendDICmd;
                            else
                                SM = StatusMachine.CheckTigheningStatus;
                        }
                        break;
                    case StatusMachine.SendDICmd:
                        Modbus_Write(0x68, 1);//Send DI Bit0
                        SM = StatusMachine.CheckDISend;
                        break;
                    case StatusMachine.CheckDISend:
                        if ((Modbus_Read(0x67) & 0x1) == 1) //Check DI Bit0
                        {
                            SM = StatusMachine.CheckTigheningStatus;
                        }
                        break;
                    case StatusMachine.CheckTigheningStatus:
#if NeverClear
                        int TigheningStatus = Modbus_Read(0x26);
#else
                        int TigheningStatus = Modbus_Read(0x1F5D);
#endif
                        if ((TigheningStatus == 1) || (TigheningStatus == 2)) //Check Status (1: OK 2:NG)
                        {
                            int TotalAngle = Modbus_Read(0x24);
                            int[] FinalPrevailTorque =  Modbus_Read(0x1F46, 2);

                            MsgCurveStr = "Param ID: " + ParamID.ToString() + "\r\n"
                                + "0x24(Total Angle): " + TotalAngle.ToString() + "\r\n"
                                + "0x1F46,0x1F47(Final + Prevail Torque):" + ((double)(FinalPrevailTorque[1] * 65536 + FinalPrevailTorque[0]) / 1000).ToString("F3") + "\n";

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
                        {
                            SM = StatusMachine.ClearFinishFlag;
                        }
                        break;
                    case StatusMachine.ClearFinishFlag:
#if NeverClear
                        Modbus_Write(0x26, 0);//Clear Finish Flag
#else
                        Modbus_Write(0x1F5D, 0);//Clear Finish Flag
#endif
                        SM = StatusMachine.SendUnlockCmd;
                        break;
                    case StatusMachine.SendUnlockCmd:
                        // Only allow operation after reading the result value and successfully switching parameters.
                        int[] Send406_2 = new int[10] { 406, 0, 0, 0, 0, 0, 1, 0, 0, 0 };// #406 Cmd + Clear Status
                        Modbus_Write(0xC8, Send406_2);
                        LastTickTime = DateTime.Now;
                        SM = StatusMachine.CheckUnlockCmd;
                        break;
                    case StatusMachine.CheckUnlockCmd:
                        int[] Read406ST_2 = Modbus_Read(0xCF, 3);
                        if ((Read406ST_2[0] == 406) && (Read406ST_2[1] == 1) && (Read406ST_2[2] == 0))
                        {
                            SM = StatusMachine.CheckReady;//Success
                        }
                        else
                        {
                            if (IsTimeout(TimeOut))
                                SM = StatusMachine.CheckUnlockCmd;//Retry
                        }
                        break;
                        ////////////////////////////////////
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

            SM = StatusMachine.Init_ClearCmd;
            RstBn.Enabled = false;
        }

        private void StopBn_Click(object sender, EventArgs e)
        {
            RstBn.Enabled = true;
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


        private void ConnHelpBn_Click(object sender, EventArgs e)
        {
            Process.Start("ncpa.cpl");
        }

        private void AutoRunningCB_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = (AutoRunningCB.Checked == true)? Delta_C_.Properties.Resources.DIStart : Delta_C_.Properties.Resources.LeverStart;
        }
    }
}
