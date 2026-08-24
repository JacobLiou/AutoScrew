using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form106_ToolSensitivity : Form
	{
		private GlobalVar GB = null;

		private int Axis = 0;

		private double ToolHMITorque = 0.0;

		private double Tool3rdPartyTorque = 0.0;

		private IContainer components = null;

		private Button btn_OK;

		private Label lab_InputTitle;

		private Label CloseBn;

		private TextBox ToolTorqueTB;

		private Label lab_ToolTorq;

		private Label lab_TorqUnit2;

		private Label lab_TorqUnit1;

		private Label lab_Title;

		private TextBox DifferenceTB;

		private TextBox TorqueMeassureTB;

		private Label lab_Diff;

		private Label lab_MeasureTorq;

		private Label lab_Precent;

		public event CreateForm106_Handler CreateID;

		public Form106_ToolSensitivity(GlobalVar GB, int Axis)
		{
			InitializeComponent();
			this.GB = GB;
			ToolTorqueTB.KeyPress += Enter3rdPartyTorque_KeyPress;
			ToolTorqueTB.LostFocus += Enter3rdPartyTorque_Leave;
			TorqueMeassureTB.KeyPress += Enter3rdPartyTorque_KeyPress;
			TorqueMeassureTB.LostFocus += Enter3rdPartyTorque_Leave;
			TorqueMeassureTB.Text = 0f.ToString("F3");
			GetHMIFinalTorque();
			MultiLanguage.LoadLanguage(this);
		}

		private void btn_OK_Click(object sender, EventArgs e)
		{
			if (this.CreateID != null)
			{
				this.CreateID(float.Parse(DifferenceTB.Text));
			}
			Close();
		}

		private void Enter3rdPartyTorque_KeyPress(object sender, KeyPressEventArgs e)
		{
			GB.RangeMaxToolTorque_000(sender, e);
			if (e.KeyChar == '\r')
			{
				CalcUIToMessage();
			}
		}

		private void Enter3rdPartyTorque_Leave(object sender, EventArgs e)
		{
			GB.LostFocus_C3(sender, e);
			CalcUIToMessage();
		}

		public void GetHMIFinalTorque()
		{
			uint TorqueDW = 0u;
			uint WatchTorq = 0u;
			if (Axis == 0)
			{
				WatchTorq = GB.UISys.RunningSrcX.TorqueUnit;
				TorqueDW = (uint)(GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T1StB.FinalAndPrevailTorque_L_06);
			}
			else
			{
				WatchTorq = GB.UISys.RunningSrcY.TorqueUnit;
				TorqueDW = (uint)(GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_H_07 * 65536 + GB.TcpStatus.Detail.T2StB.FinalAndPrevailTorque_L_06);
			}
			ToolTorqueTB.Text = ((float)TorqueDW / 1000f).ToString("F3");
			string TorqStr = MultiLanguage.GetStr("Form500_Controller", "tp_TorqUnit" + WatchTorq);
			lab_TorqUnit1.Text = TorqStr;
			lab_TorqUnit2.Text = TorqStr;
		}

		public void CalcUIToMessage()
		{
			if (Axis == 0)
			{
				ToolHMITorque = (double)float.Parse(ToolTorqueTB.Text) / GB.TorqUnitcoef(1000 + GB.UISys.RunningSrcX.TorqueUnit);
				Tool3rdPartyTorque = (double)float.Parse(TorqueMeassureTB.Text) / GB.TorqUnitcoef(1000 + GB.UISys.RunningSrcX.TorqueUnit);
			}
			else
			{
				ToolHMITorque = (double)float.Parse(ToolTorqueTB.Text) / GB.TorqUnitcoef(1000 + GB.UISys.RunningSrcY.TorqueUnit);
				Tool3rdPartyTorque = (double)float.Parse(TorqueMeassureTB.Text) / GB.TorqUnitcoef(1000 + GB.UISys.RunningSrcY.TorqueUnit);
			}
			if (Axis == 0)
			{
				if (ToolHMITorque == 0.0)
				{
					GB.FSToolXCalibration.Precision = 1.0;
				}
				else
				{
					GB.FSToolXCalibration.Precision = (float)Tool3rdPartyTorque / (float)ToolHMITorque;
				}
			}
			else if (ToolHMITorque == 0.0)
			{
				GB.FSToolYCalibration.Precision = 1.0;
			}
			else
			{
				GB.FSToolYCalibration.Precision = (float)Tool3rdPartyTorque / (float)ToolHMITorque;
			}
			DifferenceTB.Text = ((Tool3rdPartyTorque - ToolHMITorque) / ToolHMITorque * 100.0).ToString("F2");
		}

		private void Form106_ToolSensitivity_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void Form106_ToolSensitivity_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form106_ToolSensitivity));
			this.lab_InputTitle = new System.Windows.Forms.Label();
			this.btn_OK = new System.Windows.Forms.Button();
			this.CloseBn = new System.Windows.Forms.Label();
			this.ToolTorqueTB = new System.Windows.Forms.TextBox();
			this.lab_ToolTorq = new System.Windows.Forms.Label();
			this.lab_TorqUnit2 = new System.Windows.Forms.Label();
			this.lab_TorqUnit1 = new System.Windows.Forms.Label();
			this.lab_Title = new System.Windows.Forms.Label();
			this.DifferenceTB = new System.Windows.Forms.TextBox();
			this.TorqueMeassureTB = new System.Windows.Forms.TextBox();
			this.lab_Diff = new System.Windows.Forms.Label();
			this.lab_MeasureTorq = new System.Windows.Forms.Label();
			this.lab_Precent = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.lab_InputTitle.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_InputTitle.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_InputTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_InputTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_InputTitle.Location = new System.Drawing.Point(0, 0);
			this.lab_InputTitle.Name = "lab_InputTitle";
			this.lab_InputTitle.Size = new System.Drawing.Size(500, 35);
			this.lab_InputTitle.TabIndex = 55;
			this.lab_InputTitle.Text = "Title";
			this.lab_InputTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btn_OK.BackgroundImage = (System.Drawing.Image)resources.GetObject("btn_OK.BackgroundImage");
			this.btn_OK.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.btn_OK.FlatAppearance.BorderSize = 0;
			this.btn_OK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btn_OK.Font = new System.Drawing.Font("新細明體", 12f);
			this.btn_OK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btn_OK.Location = new System.Drawing.Point(197, 226);
			this.btn_OK.Name = "btn_OK";
			this.btn_OK.Size = new System.Drawing.Size(92, 30);
			this.btn_OK.TabIndex = 60;
			this.btn_OK.Text = "Confirm";
			this.btn_OK.UseVisualStyleBackColor = true;
			this.btn_OK.Click += new System.EventHandler(btn_OK_Click);
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(464, 0);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 130;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.ToolTorqueTB.Location = new System.Drawing.Point(256, 79);
			this.ToolTorqueTB.Name = "ToolTorqueTB";
			this.ToolTorqueTB.Size = new System.Drawing.Size(175, 25);
			this.ToolTorqueTB.TabIndex = 172;
			this.ToolTorqueTB.Text = "0.000";
			this.ToolTorqueTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_ToolTorq.Location = new System.Drawing.Point(8, 79);
			this.lab_ToolTorq.Name = "lab_ToolTorq";
			this.lab_ToolTorq.Size = new System.Drawing.Size(242, 25);
			this.lab_ToolTorq.TabIndex = 171;
			this.lab_ToolTorq.Text = "Tool Torque";
			this.lab_ToolTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_TorqUnit2.AutoSize = true;
			this.lab_TorqUnit2.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit2.Location = new System.Drawing.Point(438, 117);
			this.lab_TorqUnit2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit2.Name = "lab_TorqUnit2";
			this.lab_TorqUnit2.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit2.TabIndex = 176;
			this.lab_TorqUnit2.Text = "N.m";
			this.lab_TorqUnit1.AutoSize = true;
			this.lab_TorqUnit1.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_TorqUnit1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_TorqUnit1.Location = new System.Drawing.Point(438, 79);
			this.lab_TorqUnit1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lab_TorqUnit1.Name = "lab_TorqUnit1";
			this.lab_TorqUnit1.Size = new System.Drawing.Size(43, 20);
			this.lab_TorqUnit1.TabIndex = 177;
			this.lab_TorqUnit1.Text = "N.m";
			this.lab_Title.BackColor = System.Drawing.Color.Black;
			this.lab_Title.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(-217, 158);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(652, 2);
			this.lab_Title.TabIndex = 175;
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.DifferenceTB.Location = new System.Drawing.Point(256, 173);
			this.DifferenceTB.Name = "DifferenceTB";
			this.DifferenceTB.ReadOnly = true;
			this.DifferenceTB.Size = new System.Drawing.Size(175, 25);
			this.DifferenceTB.TabIndex = 173;
			this.DifferenceTB.Text = "0.00";
			this.DifferenceTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.TorqueMeassureTB.Location = new System.Drawing.Point(256, 117);
			this.TorqueMeassureTB.Name = "TorqueMeassureTB";
			this.TorqueMeassureTB.Size = new System.Drawing.Size(175, 25);
			this.TorqueMeassureTB.TabIndex = 174;
			this.TorqueMeassureTB.Text = "0.000";
			this.TorqueMeassureTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Diff.Location = new System.Drawing.Point(8, 173);
			this.lab_Diff.Name = "lab_Diff";
			this.lab_Diff.Size = new System.Drawing.Size(242, 25);
			this.lab_Diff.TabIndex = 169;
			this.lab_Diff.Text = "Difference";
			this.lab_Diff.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MeasureTorq.Location = new System.Drawing.Point(8, 117);
			this.lab_MeasureTorq.Name = "lab_MeasureTorq";
			this.lab_MeasureTorq.Size = new System.Drawing.Size(242, 25);
			this.lab_MeasureTorq.TabIndex = 170;
			this.lab_MeasureTorq.Text = "Torque Measured from External Device";
			this.lab_MeasureTorq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_Precent.AutoSize = true;
			this.lab_Precent.Location = new System.Drawing.Point(439, 173);
			this.lab_Precent.Name = "lab_Precent";
			this.lab_Precent.Size = new System.Drawing.Size(19, 15);
			this.lab_Precent.TabIndex = 178;
			this.lab_Precent.Text = "%";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.Control;
			base.ClientSize = new System.Drawing.Size(500, 280);
			base.Controls.Add(this.ToolTorqueTB);
			base.Controls.Add(this.lab_ToolTorq);
			base.Controls.Add(this.lab_TorqUnit2);
			base.Controls.Add(this.lab_TorqUnit1);
			base.Controls.Add(this.lab_Title);
			base.Controls.Add(this.DifferenceTB);
			base.Controls.Add(this.TorqueMeassureTB);
			base.Controls.Add(this.lab_Diff);
			base.Controls.Add(this.lab_MeasureTorq);
			base.Controls.Add(this.lab_Precent);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.btn_OK);
			base.Controls.Add(this.lab_InputTitle);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form106_ToolSensitivity";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form106_ToolSensitivity_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form106_ToolSensitivity_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
