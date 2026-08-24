using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class Form504_ScreenSetting : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private ComboBox ErrorSignalBuzzerCB;

		private ComboBox FinishedSignalBuzzerOfASingleScrewCB;

		private ComboBox FinishedSignalBuzzerOfTotalScrewsCB;

		private ComboBox HomeScreenCB;

		private ComboBox DisplayOrientationOfExternalScreenCB;

		private Label lab_ErrorSignalBuzzer;

		private Label lab_FinishedSignalBuzzerOfASingleScrew;

		private Label lab_FinishedSignalBuzzerOfTotalScrews;

		private Label lab_MessageVolume;

		private Label lab_HomeScreen;

		private Label lab_DisplayOrientationOfExternalScreen;

		private TrackBar BuzzerVolumeTrA;

		private Label lab_ButtonVolume;

		private TrackBar BuzzerVolumeTrB;

		private ComboBox KeyboardCursorblinkCB;

		private Label lab_KeyboardCursorblink;

		public Form504_ScreenSetting(GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
			TCP.FSIDRead_ByTCP(569, 0, 0, 0, 0, 0);
			TCP.FSIDRead_ByTCP(570, 0, 0, 0, 0, 0);
			ErrorSignalBuzzerCB.SelectedIndexChanged -= ErrorSignalBuzzerCB_SelectedIndexChanged;
			ErrorSignalBuzzerCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode01"));
			ErrorSignalBuzzerCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode02"));
			ErrorSignalBuzzerCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode03"));
			ErrorSignalBuzzerCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode04"));
			if (GB.FSCtrlBuzzerMode.Error < ErrorSignalBuzzerCB.Items.Count)
			{
				ErrorSignalBuzzerCB.SelectedIndex = GB.FSCtrlBuzzerMode.Error;
			}
			ErrorSignalBuzzerCB.SelectedIndexChanged += ErrorSignalBuzzerCB_SelectedIndexChanged;
			FinishedSignalBuzzerOfASingleScrewCB.SelectedIndexChanged -= FinishedSignalBuzzerOfASingleScrewCB_SelectedIndexChanged;
			FinishedSignalBuzzerOfASingleScrewCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode01"));
			FinishedSignalBuzzerOfASingleScrewCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode02"));
			FinishedSignalBuzzerOfASingleScrewCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode03"));
			FinishedSignalBuzzerOfASingleScrewCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode04"));
			if (GB.FSCtrlBuzzerMode.EachFinish < FinishedSignalBuzzerOfASingleScrewCB.Items.Count)
			{
				FinishedSignalBuzzerOfASingleScrewCB.SelectedIndex = GB.FSCtrlBuzzerMode.EachFinish;
			}
			FinishedSignalBuzzerOfASingleScrewCB.SelectedIndexChanged += FinishedSignalBuzzerOfASingleScrewCB_SelectedIndexChanged;
			FinishedSignalBuzzerOfTotalScrewsCB.SelectedIndexChanged -= FinishedSignalBuzzerOfTotalScrewsCB_SelectedIndexChanged;
			FinishedSignalBuzzerOfTotalScrewsCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode01"));
			FinishedSignalBuzzerOfTotalScrewsCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode02"));
			FinishedSignalBuzzerOfTotalScrewsCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode03"));
			FinishedSignalBuzzerOfTotalScrewsCB.Items.Add(MultiLanguage.GetStr(this, "tp_BuzzerMode04"));
			if (GB.FSCtrlBuzzerMode.AllFinish < FinishedSignalBuzzerOfTotalScrewsCB.Items.Count)
			{
				FinishedSignalBuzzerOfTotalScrewsCB.SelectedIndex = GB.FSCtrlBuzzerMode.AllFinish;
			}
			FinishedSignalBuzzerOfTotalScrewsCB.SelectedIndexChanged += FinishedSignalBuzzerOfTotalScrewsCB_SelectedIndexChanged;
			HomeScreenCB.SelectedIndexChanged -= HomeScreenCB_SelectedIndexChanged;
			HomeScreenCB.Items.Add(MultiLanguage.GetStr(this, "tp_HomePage01"));
			HomeScreenCB.Items.Add(MultiLanguage.GetStr(this, "tp_HomePage02"));
			if (GB.FSCtrlHomeStartPage.Mode < HomeScreenCB.Items.Count)
			{
				HomeScreenCB.SelectedIndex = GB.FSCtrlHomeStartPage.Mode;
			}
			HomeScreenCB.SelectedIndexChanged += HomeScreenCB_SelectedIndexChanged;
			DisplayOrientationOfExternalScreenCB.SelectedIndexChanged -= DisplayOrientationOfExternalScreenCB_SelectedIndexChanged;
			DisplayOrientationOfExternalScreenCB.Items.Add(MultiLanguage.GetStr(this, "tp_HDMI01"));
			DisplayOrientationOfExternalScreenCB.Items.Add(MultiLanguage.GetStr(this, "tp_HDMI02"));
			if (GB.FSCtrlDisplayHDMI.Mode < DisplayOrientationOfExternalScreenCB.Items.Count)
			{
				DisplayOrientationOfExternalScreenCB.SelectedIndex = GB.FSCtrlDisplayHDMI.Mode;
			}
			DisplayOrientationOfExternalScreenCB.SelectedIndexChanged += DisplayOrientationOfExternalScreenCB_SelectedIndexChanged;
			KeyboardCursorblinkCB.SelectedIndexChanged -= KeyboardCursorblinkCB_SelectedIndexChanged;
			KeyboardCursorblinkCB.Items.Add(MultiLanguage.GetStr("Form500_Controller", "tp_DisableEnable1"));
			KeyboardCursorblinkCB.Items.Add(MultiLanguage.GetStr("Form500_Controller", "tp_DisableEnable2"));
			if (GB.FSCtrlKeyboardCursorBlinkingInResults.Enable < KeyboardCursorblinkCB.Items.Count)
			{
				KeyboardCursorblinkCB.SelectedIndex = GB.FSCtrlKeyboardCursorBlinkingInResults.Enable;
			}
			KeyboardCursorblinkCB.SelectedIndexChanged += KeyboardCursorblinkCB_SelectedIndexChanged;
			BuzzerVolumeTrA.Maximum = 100;
			BuzzerVolumeTrA.Minimum = 0;
			BuzzerVolumeTrA.Value = GB.FSCtrlBuzzerVolume.MsgVolume;
			BuzzerVolumeTrB.Maximum = 100;
			BuzzerVolumeTrB.Minimum = 0;
			BuzzerVolumeTrB.Value = GB.FSCtrlBuzzerVolume.KeyBoardVolum;
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form504_ScreenSetting_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void ErrorSignalBuzzerCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlBuzzerMode.Error = (ushort)ErrorSignalBuzzerCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(506, 0, GB.FSCtrlBuzzerMode.Error, GB.FSCtrlBuzzerMode.EachFinish, GB.FSCtrlBuzzerMode.AllFinish, 0);
		}

		private void FinishedSignalBuzzerOfASingleScrewCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlBuzzerMode.EachFinish = (ushort)FinishedSignalBuzzerOfASingleScrewCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(506, 0, GB.FSCtrlBuzzerMode.Error, GB.FSCtrlBuzzerMode.EachFinish, GB.FSCtrlBuzzerMode.AllFinish, 0);
		}

		private void FinishedSignalBuzzerOfTotalScrewsCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlBuzzerMode.AllFinish = (ushort)FinishedSignalBuzzerOfTotalScrewsCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(506, 0, GB.FSCtrlBuzzerMode.Error, GB.FSCtrlBuzzerMode.EachFinish, GB.FSCtrlBuzzerMode.AllFinish, 0);
		}

		private void HomeScreenCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlHomeStartPage.Mode = (ushort)HomeScreenCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(527, 0, GB.FSCtrlHomeStartPage.Mode, 0, 0, 0);
		}

		private void DisplayOrientationOfExternalScreenCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlDisplayHDMI.Mode = (ushort)DisplayOrientationOfExternalScreenCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(526, 0, GB.FSCtrlDisplayHDMI.Mode, 0, 0, 0);
		}

		private void KeyboardCursorblinkCB_SelectedIndexChanged(object sender, EventArgs e)
		{
			GB.FSCtrlKeyboardCursorBlinkingInResults.Enable = (ushort)KeyboardCursorblinkCB.SelectedIndex;
			TCP.FSIDWrite_ByTCP(531, 0, GB.FSCtrlKeyboardCursorBlinkingInResults.Enable, 0, 0, 0);
		}

		private void BuzzerVolumeTrA_MouseUp(object sender, MouseEventArgs e)
		{
			GB.FSCtrlBuzzerVolume.MsgVolume = (ushort)BuzzerVolumeTrA.Value;
			TCP.FSIDWrite_ByTCP(525, 0, GB.FSCtrlBuzzerVolume.MsgVolume, GB.FSCtrlBuzzerVolume.KeyBoardVolum, 0, 0);
		}

		private void BuzzerVolumeTrB_MouseUp(object sender, MouseEventArgs e)
		{
			GB.FSCtrlBuzzerVolume.KeyBoardVolum = (ushort)BuzzerVolumeTrB.Value;
			TCP.FSIDWrite_ByTCP(525, 0, GB.FSCtrlBuzzerVolume.MsgVolume, GB.FSCtrlBuzzerVolume.KeyBoardVolum, 0, 0);
		}

		private void Form504_ScreenSetting_Load(object sender, EventArgs e)
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
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.ErrorSignalBuzzerCB = new System.Windows.Forms.ComboBox();
			this.FinishedSignalBuzzerOfASingleScrewCB = new System.Windows.Forms.ComboBox();
			this.FinishedSignalBuzzerOfTotalScrewsCB = new System.Windows.Forms.ComboBox();
			this.HomeScreenCB = new System.Windows.Forms.ComboBox();
			this.DisplayOrientationOfExternalScreenCB = new System.Windows.Forms.ComboBox();
			this.lab_ErrorSignalBuzzer = new System.Windows.Forms.Label();
			this.lab_FinishedSignalBuzzerOfASingleScrew = new System.Windows.Forms.Label();
			this.lab_FinishedSignalBuzzerOfTotalScrews = new System.Windows.Forms.Label();
			this.lab_MessageVolume = new System.Windows.Forms.Label();
			this.lab_HomeScreen = new System.Windows.Forms.Label();
			this.lab_DisplayOrientationOfExternalScreen = new System.Windows.Forms.Label();
			this.BuzzerVolumeTrA = new System.Windows.Forms.TrackBar();
			this.lab_ButtonVolume = new System.Windows.Forms.Label();
			this.BuzzerVolumeTrB = new System.Windows.Forms.TrackBar();
			this.KeyboardCursorblinkCB = new System.Windows.Forms.ComboBox();
			this.lab_KeyboardCursorblink = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)this.BuzzerVolumeTrA).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.BuzzerVolumeTrB).BeginInit();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, 0);
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
			this.CloseBn.TabIndex = 126;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.ErrorSignalBuzzerCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ErrorSignalBuzzerCB.FormattingEnabled = true;
			this.ErrorSignalBuzzerCB.Location = new System.Drawing.Point(268, 64);
			this.ErrorSignalBuzzerCB.Name = "ErrorSignalBuzzerCB";
			this.ErrorSignalBuzzerCB.Size = new System.Drawing.Size(177, 23);
			this.ErrorSignalBuzzerCB.TabIndex = 127;
			this.FinishedSignalBuzzerOfASingleScrewCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.FinishedSignalBuzzerOfASingleScrewCB.FormattingEnabled = true;
			this.FinishedSignalBuzzerOfASingleScrewCB.Location = new System.Drawing.Point(268, 105);
			this.FinishedSignalBuzzerOfASingleScrewCB.Name = "FinishedSignalBuzzerOfASingleScrewCB";
			this.FinishedSignalBuzzerOfASingleScrewCB.Size = new System.Drawing.Size(177, 23);
			this.FinishedSignalBuzzerOfASingleScrewCB.TabIndex = 127;
			this.FinishedSignalBuzzerOfTotalScrewsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.FinishedSignalBuzzerOfTotalScrewsCB.FormattingEnabled = true;
			this.FinishedSignalBuzzerOfTotalScrewsCB.Location = new System.Drawing.Point(268, 148);
			this.FinishedSignalBuzzerOfTotalScrewsCB.Name = "FinishedSignalBuzzerOfTotalScrewsCB";
			this.FinishedSignalBuzzerOfTotalScrewsCB.Size = new System.Drawing.Size(177, 23);
			this.FinishedSignalBuzzerOfTotalScrewsCB.TabIndex = 127;
			this.HomeScreenCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.HomeScreenCB.FormattingEnabled = true;
			this.HomeScreenCB.Location = new System.Drawing.Point(268, 306);
			this.HomeScreenCB.Name = "HomeScreenCB";
			this.HomeScreenCB.Size = new System.Drawing.Size(177, 23);
			this.HomeScreenCB.TabIndex = 127;
			this.DisplayOrientationOfExternalScreenCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.DisplayOrientationOfExternalScreenCB.FormattingEnabled = true;
			this.DisplayOrientationOfExternalScreenCB.Location = new System.Drawing.Point(268, 347);
			this.DisplayOrientationOfExternalScreenCB.Name = "DisplayOrientationOfExternalScreenCB";
			this.DisplayOrientationOfExternalScreenCB.Size = new System.Drawing.Size(177, 23);
			this.DisplayOrientationOfExternalScreenCB.TabIndex = 127;
			this.lab_ErrorSignalBuzzer.Location = new System.Drawing.Point(21, 64);
			this.lab_ErrorSignalBuzzer.Name = "lab_ErrorSignalBuzzer";
			this.lab_ErrorSignalBuzzer.Size = new System.Drawing.Size(241, 20);
			this.lab_ErrorSignalBuzzer.TabIndex = 128;
			this.lab_ErrorSignalBuzzer.Text = "Error Signal Buzzer";
			this.lab_ErrorSignalBuzzer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinishedSignalBuzzerOfASingleScrew.Location = new System.Drawing.Point(21, 105);
			this.lab_FinishedSignalBuzzerOfASingleScrew.Name = "lab_FinishedSignalBuzzerOfASingleScrew";
			this.lab_FinishedSignalBuzzerOfASingleScrew.Size = new System.Drawing.Size(241, 20);
			this.lab_FinishedSignalBuzzerOfASingleScrew.TabIndex = 128;
			this.lab_FinishedSignalBuzzerOfASingleScrew.Text = "Finished Signal Buzzer of A Single Screw";
			this.lab_FinishedSignalBuzzerOfASingleScrew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_FinishedSignalBuzzerOfTotalScrews.Location = new System.Drawing.Point(21, 148);
			this.lab_FinishedSignalBuzzerOfTotalScrews.Name = "lab_FinishedSignalBuzzerOfTotalScrews";
			this.lab_FinishedSignalBuzzerOfTotalScrews.Size = new System.Drawing.Size(241, 20);
			this.lab_FinishedSignalBuzzerOfTotalScrews.TabIndex = 128;
			this.lab_FinishedSignalBuzzerOfTotalScrews.Text = "Finished Signal Buzzer of Total Screws";
			this.lab_FinishedSignalBuzzerOfTotalScrews.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_MessageVolume.Location = new System.Drawing.Point(21, 211);
			this.lab_MessageVolume.Name = "lab_MessageVolume";
			this.lab_MessageVolume.Size = new System.Drawing.Size(241, 20);
			this.lab_MessageVolume.TabIndex = 128;
			this.lab_MessageVolume.Text = "Error and Finished Signal Buzzer Volume";
			this.lab_MessageVolume.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_HomeScreen.Location = new System.Drawing.Point(21, 306);
			this.lab_HomeScreen.Name = "lab_HomeScreen";
			this.lab_HomeScreen.Size = new System.Drawing.Size(241, 20);
			this.lab_HomeScreen.TabIndex = 128;
			this.lab_HomeScreen.Text = "Home Screen";
			this.lab_HomeScreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_DisplayOrientationOfExternalScreen.Location = new System.Drawing.Point(21, 348);
			this.lab_DisplayOrientationOfExternalScreen.Name = "lab_DisplayOrientationOfExternalScreen";
			this.lab_DisplayOrientationOfExternalScreen.Size = new System.Drawing.Size(241, 20);
			this.lab_DisplayOrientationOfExternalScreen.TabIndex = 128;
			this.lab_DisplayOrientationOfExternalScreen.Text = "Display orientation of External Screen(HDMI)";
			this.lab_DisplayOrientationOfExternalScreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.BuzzerVolumeTrA.Location = new System.Drawing.Point(268, 193);
			this.BuzzerVolumeTrA.Name = "BuzzerVolumeTrA";
			this.BuzzerVolumeTrA.Size = new System.Drawing.Size(177, 56);
			this.BuzzerVolumeTrA.TabIndex = 129;
			this.BuzzerVolumeTrA.MouseUp += new System.Windows.Forms.MouseEventHandler(BuzzerVolumeTrA_MouseUp);
			this.lab_ButtonVolume.Location = new System.Drawing.Point(21, 268);
			this.lab_ButtonVolume.Name = "lab_ButtonVolume";
			this.lab_ButtonVolume.Size = new System.Drawing.Size(241, 20);
			this.lab_ButtonVolume.TabIndex = 128;
			this.lab_ButtonVolume.Text = "Button Volume";
			this.lab_ButtonVolume.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.BuzzerVolumeTrB.Location = new System.Drawing.Point(268, 250);
			this.BuzzerVolumeTrB.Name = "BuzzerVolumeTrB";
			this.BuzzerVolumeTrB.Size = new System.Drawing.Size(177, 56);
			this.BuzzerVolumeTrB.TabIndex = 129;
			this.BuzzerVolumeTrB.MouseUp += new System.Windows.Forms.MouseEventHandler(BuzzerVolumeTrB_MouseUp);
			this.KeyboardCursorblinkCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.KeyboardCursorblinkCB.FormattingEnabled = true;
			this.KeyboardCursorblinkCB.Location = new System.Drawing.Point(268, 394);
			this.KeyboardCursorblinkCB.Name = "KeyboardCursorblinkCB";
			this.KeyboardCursorblinkCB.Size = new System.Drawing.Size(177, 23);
			this.KeyboardCursorblinkCB.TabIndex = 127;
			this.lab_KeyboardCursorblink.Location = new System.Drawing.Point(21, 395);
			this.lab_KeyboardCursorblink.Name = "lab_KeyboardCursorblink";
			this.lab_KeyboardCursorblink.Size = new System.Drawing.Size(241, 20);
			this.lab_KeyboardCursorblink.TabIndex = 128;
			this.lab_KeyboardCursorblink.Text = "Keyboard cursor blink in results page";
			this.lab_KeyboardCursorblink.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 447);
			base.Controls.Add(this.BuzzerVolumeTrB);
			base.Controls.Add(this.BuzzerVolumeTrA);
			base.Controls.Add(this.lab_KeyboardCursorblink);
			base.Controls.Add(this.lab_DisplayOrientationOfExternalScreen);
			base.Controls.Add(this.lab_ButtonVolume);
			base.Controls.Add(this.lab_HomeScreen);
			base.Controls.Add(this.lab_MessageVolume);
			base.Controls.Add(this.lab_FinishedSignalBuzzerOfTotalScrews);
			base.Controls.Add(this.lab_FinishedSignalBuzzerOfASingleScrew);
			base.Controls.Add(this.lab_ErrorSignalBuzzer);
			base.Controls.Add(this.KeyboardCursorblinkCB);
			base.Controls.Add(this.DisplayOrientationOfExternalScreenCB);
			base.Controls.Add(this.HomeScreenCB);
			base.Controls.Add(this.FinishedSignalBuzzerOfTotalScrewsCB);
			base.Controls.Add(this.FinishedSignalBuzzerOfASingleScrewCB);
			base.Controls.Add(this.ErrorSignalBuzzerCB);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form504_ScreenSetting";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form504_ScreenSetting_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form504_ScreenSetting_Paint);
			((System.ComponentModel.ISupportInitialize)this.BuzzerVolumeTrA).EndInit();
			((System.ComponentModel.ISupportInitialize)this.BuzzerVolumeTrB).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
