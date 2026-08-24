using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form507_LogIn : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private bool EditUserNameSW = false;

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private TabPage tabPage2;

		private Button ModifyBn;

		private TabPage tabPage1;

		private Button LogInBn;

		private Button LogOutBn;

		private TextBox PasswordTB;

		private Label lab_Password;

		private Label lab_User;

		private TabControl tab_LogIn;

		private Label lab_NewPassword;

		private Label lab_OldPassword;

		private Label lab_ModifyUser;

		private TextBox NewPasswordTB;

		private TextBox OldPasswordTB;

		private ComboBox LogUserCB;

		private ComboBox ModifyUserCB;

		private Button HideBn;

		private Button HideBn2;

		private Button EditBn;

		private TextBox EditUserNameTB;

		public event CreateForm507_ChooseHandler CreateCloseEvent;

		public Form507_LogIn(GlobalVar GB, TCPclient TCP)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
			EditUserNameSW = false;
			UpdataUI();
			PasswordTB.MaxLength = 20;
			PasswordTB.PasswordChar = '*';
			PasswordTB.KeyPress += GB.RangePasswordValue;
			OldPasswordTB.MaxLength = 20;
			OldPasswordTB.PasswordChar = '*';
			OldPasswordTB.KeyPress += GB.RangePasswordValue;
			NewPasswordTB.MaxLength = 20;
			NewPasswordTB.PasswordChar = '*';
			NewPasswordTB.KeyPress += GB.RangePasswordValue;
			EditUserNameTB.MaxLength = 20;
			EditUserNameTB.KeyPress += GB.RangeASCIIInput;
			EditUserNameTB.Multiline = false;
			EditUserNameTB.ShortcutsEnabled = false;
			EditUserNameTB.KeyDown += TbEditUserNameTitle_KeyDown;
			EditBn.Visible = (GB.CheckHMIVer(169, 11) ? true : false);
		}

		private void UpdataUI()
		{
			LogUserCB.Items.Clear();
			LogUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 0));
			LogUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 1));
			LogUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 2));
			LogUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 3));
			LogUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 4));
			LogUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 5));
			LogUserCB.SelectedIndex = 1;
			ModifyUserCB.Items.Clear();
			ModifyUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 0));
			ModifyUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 1));
			ModifyUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 2));
			ModifyUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 3));
			ModifyUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 4));
			ModifyUserCB.Items.Add(GB.GetNameTitleStr(FormType.SubCtrlUserName, 5));
			ModifyUserCB.SelectedIndex = 1;
			int CurrID = ((ModifyUserCB.SelectedIndex > 5) ? 5 : ModifyUserCB.SelectedIndex);
			EditUserNameTB.Text = GB.GetNameTitleStr(FormType.SubCtrlUserName, CurrID);
			EditUserNameTB.Visible = false;
		}

		private void TbEditUserNameTitle_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				ushort UserID = (ushort)ModifyUserCB.SelectedIndex;
				if (UserID <= 5)
				{
					GB.SetNameTitleStr(FormType.SubCtrlUserName, UserID, EditUserNameTB.Text);
					TCP.FSIDWrite_ByTCP(1514, 0, UserID, 0, 0, 0);
				}
			}
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form507_LogIn_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void ModifyBn_Click(object sender, EventArgs e)
		{
			GB.SetNameTitleStr(FormType.SubCtrlCurrentPassword, 0, OldPasswordTB.Text);
			GB.SetNameTitleStr(FormType.SubCtrlNewPassword, 0, NewPasswordTB.Text);
			TCP.FSIDWrite_ByTCP(501, 0, (ushort)(ModifyUserCB.SelectedIndex + 1), 0, 0, 0);
			Close();
		}

		private void LogInBn_Click(object sender, EventArgs e)
		{
			GB.SetNameTitleStr(FormType.SubCtrlLogInPassword, 0, PasswordTB.Text);
			TCP.FSIDWrite_ByTCP(500, 0, (ushort)(LogUserCB.SelectedIndex + 1), 0, 0, 0);
			TCP.FSIDRead_ByTCP(1571, 0, 0, 0, 0, 0);
			Close();
			if (this.CreateCloseEvent != null)
			{
				this.CreateCloseEvent((uint)LogUserCB.SelectedIndex);
			}
		}

		private void LogOutBn_Click(object sender, EventArgs e)
		{
			TCP.FSIDWrite_ByTCP(502, 0, 0, 0, 0, 0);
			TCP.FSIDRead_ByTCP(1571, 0, 0, 0, 0, 0);
			Close();
			if (this.CreateCloseEvent != null)
			{
				this.CreateCloseEvent(0u);
			}
		}

		private void HideBn_MouseDown(object sender, MouseEventArgs e)
		{
			PasswordTB.PasswordChar = '\0';
		}

		private void HideBn_MouseUp(object sender, MouseEventArgs e)
		{
			PasswordTB.PasswordChar = '*';
		}

		private void HideBn2_MouseDown(object sender, MouseEventArgs e)
		{
			OldPasswordTB.PasswordChar = '\0';
			NewPasswordTB.PasswordChar = '\0';
		}

		private void HideBn2_MouseUp(object sender, MouseEventArgs e)
		{
			OldPasswordTB.PasswordChar = '*';
			NewPasswordTB.PasswordChar = '*';
		}

		private void Form507_LogIn_Load(object sender, EventArgs e)
		{
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void EditBn_Click(object sender, EventArgs e)
		{
			if (GB.ExFSUser.UserID >= 5 || ModifyUserCB.SelectedIndex == GB.ExFSUser.UserID)
			{
				EditUserNameSW = !EditUserNameSW;
			}
			if (EditUserNameSW)
			{
				EditUserNameTB.Visible = true;
				ModifyUserCB.Visible = false;
			}
			else
			{
				EditUserNameTB.Visible = false;
				ModifyUserCB.Visible = true;
				UpdataUI();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SD3_Graph.Form507_LogIn));
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.HideBn2 = new System.Windows.Forms.Button();
			this.ModifyUserCB = new System.Windows.Forms.ComboBox();
			this.NewPasswordTB = new System.Windows.Forms.TextBox();
			this.EditUserNameTB = new System.Windows.Forms.TextBox();
			this.OldPasswordTB = new System.Windows.Forms.TextBox();
			this.lab_NewPassword = new System.Windows.Forms.Label();
			this.lab_OldPassword = new System.Windows.Forms.Label();
			this.lab_ModifyUser = new System.Windows.Forms.Label();
			this.EditBn = new System.Windows.Forms.Button();
			this.ModifyBn = new System.Windows.Forms.Button();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.HideBn = new System.Windows.Forms.Button();
			this.LogUserCB = new System.Windows.Forms.ComboBox();
			this.LogInBn = new System.Windows.Forms.Button();
			this.LogOutBn = new System.Windows.Forms.Button();
			this.PasswordTB = new System.Windows.Forms.TextBox();
			this.lab_Password = new System.Windows.Forms.Label();
			this.lab_User = new System.Windows.Forms.Label();
			this.tab_LogIn = new System.Windows.Forms.TabControl();
			this.tabPage2.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tab_LogIn.SuspendLayout();
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
			this.CloseBn.Location = new System.Drawing.Point(469, 3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.tabPage2.Controls.Add(this.HideBn2);
			this.tabPage2.Controls.Add(this.ModifyUserCB);
			this.tabPage2.Controls.Add(this.NewPasswordTB);
			this.tabPage2.Controls.Add(this.EditUserNameTB);
			this.tabPage2.Controls.Add(this.OldPasswordTB);
			this.tabPage2.Controls.Add(this.lab_NewPassword);
			this.tabPage2.Controls.Add(this.lab_OldPassword);
			this.tabPage2.Controls.Add(this.lab_ModifyUser);
			this.tabPage2.Controls.Add(this.EditBn);
			this.tabPage2.Controls.Add(this.ModifyBn);
			this.tabPage2.Location = new System.Drawing.Point(4, 30);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(468, 182);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Change Password";
			this.tabPage2.UseVisualStyleBackColor = true;
			this.HideBn2.BackgroundImage = SD3Soft.Properties.Resources.NonHide;
			this.HideBn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.HideBn2.FlatAppearance.BorderSize = 0;
			this.HideBn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HideBn2.Font = new System.Drawing.Font("新細明體", 12f);
			this.HideBn2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.HideBn2.Location = new System.Drawing.Point(7, 7);
			this.HideBn2.Margin = new System.Windows.Forms.Padding(4);
			this.HideBn2.Name = "HideBn2";
			this.HideBn2.Size = new System.Drawing.Size(44, 31);
			this.HideBn2.TabIndex = 147;
			this.HideBn2.UseVisualStyleBackColor = true;
			this.HideBn2.MouseDown += new System.Windows.Forms.MouseEventHandler(HideBn2_MouseDown);
			this.HideBn2.MouseUp += new System.Windows.Forms.MouseEventHandler(HideBn2_MouseUp);
			this.ModifyUserCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ModifyUserCB.FormattingEnabled = true;
			this.ModifyUserCB.Location = new System.Drawing.Point(183, 30);
			this.ModifyUserCB.Name = "ModifyUserCB";
			this.ModifyUserCB.Size = new System.Drawing.Size(233, 28);
			this.ModifyUserCB.TabIndex = 146;
			this.NewPasswordTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.NewPasswordTB.Location = new System.Drawing.Point(183, 104);
			this.NewPasswordTB.Name = "NewPasswordTB";
			this.NewPasswordTB.Size = new System.Drawing.Size(233, 27);
			this.NewPasswordTB.TabIndex = 145;
			this.NewPasswordTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.EditUserNameTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.EditUserNameTB.Location = new System.Drawing.Point(183, 31);
			this.EditUserNameTB.Name = "EditUserNameTB";
			this.EditUserNameTB.Size = new System.Drawing.Size(233, 27);
			this.EditUserNameTB.TabIndex = 145;
			this.EditUserNameTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.OldPasswordTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.OldPasswordTB.Location = new System.Drawing.Point(183, 67);
			this.OldPasswordTB.Name = "OldPasswordTB";
			this.OldPasswordTB.Size = new System.Drawing.Size(233, 27);
			this.OldPasswordTB.TabIndex = 145;
			this.OldPasswordTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_NewPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_NewPassword.Location = new System.Drawing.Point(3, 107);
			this.lab_NewPassword.Name = "lab_NewPassword";
			this.lab_NewPassword.Size = new System.Drawing.Size(180, 25);
			this.lab_NewPassword.TabIndex = 144;
			this.lab_NewPassword.Text = "New Password";
			this.lab_NewPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_OldPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_OldPassword.Location = new System.Drawing.Point(3, 70);
			this.lab_OldPassword.Name = "lab_OldPassword";
			this.lab_OldPassword.Size = new System.Drawing.Size(180, 25);
			this.lab_OldPassword.TabIndex = 144;
			this.lab_OldPassword.Text = "Current Password";
			this.lab_OldPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ModifyUser.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_ModifyUser.Location = new System.Drawing.Point(3, 33);
			this.lab_ModifyUser.Name = "lab_ModifyUser";
			this.lab_ModifyUser.Size = new System.Drawing.Size(180, 25);
			this.lab_ModifyUser.TabIndex = 143;
			this.lab_ModifyUser.Text = "User";
			this.lab_ModifyUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.EditBn.BackgroundImage = SD3Soft.Properties.Resources.編輯_筆跟畫布;
			this.EditBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.EditBn.FlatAppearance.BorderSize = 0;
			this.EditBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.EditBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.EditBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.EditBn.Location = new System.Drawing.Point(422, 30);
			this.EditBn.Name = "EditBn";
			this.EditBn.Size = new System.Drawing.Size(30, 30);
			this.EditBn.TabIndex = 142;
			this.EditBn.UseVisualStyleBackColor = true;
			this.EditBn.Click += new System.EventHandler(EditBn_Click);
			this.ModifyBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("ModifyBn.BackgroundImage");
			this.ModifyBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ModifyBn.FlatAppearance.BorderSize = 0;
			this.ModifyBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.ModifyBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.ModifyBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.ModifyBn.Location = new System.Drawing.Point(178, 141);
			this.ModifyBn.Name = "ModifyBn";
			this.ModifyBn.Size = new System.Drawing.Size(120, 30);
			this.ModifyBn.TabIndex = 142;
			this.ModifyBn.Text = "Change";
			this.ModifyBn.UseVisualStyleBackColor = true;
			this.ModifyBn.Click += new System.EventHandler(ModifyBn_Click);
			this.tabPage1.Controls.Add(this.HideBn);
			this.tabPage1.Controls.Add(this.LogUserCB);
			this.tabPage1.Controls.Add(this.LogInBn);
			this.tabPage1.Controls.Add(this.LogOutBn);
			this.tabPage1.Controls.Add(this.PasswordTB);
			this.tabPage1.Controls.Add(this.lab_Password);
			this.tabPage1.Controls.Add(this.lab_User);
			this.tabPage1.Location = new System.Drawing.Point(4, 30);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(468, 182);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Log In";
			this.tabPage1.UseVisualStyleBackColor = true;
			this.HideBn.BackgroundImage = SD3Soft.Properties.Resources.NonHide;
			this.HideBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.HideBn.FlatAppearance.BorderSize = 0;
			this.HideBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.HideBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.HideBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.HideBn.Location = new System.Drawing.Point(7, 7);
			this.HideBn.Margin = new System.Windows.Forms.Padding(4);
			this.HideBn.Name = "HideBn";
			this.HideBn.Size = new System.Drawing.Size(44, 31);
			this.HideBn.TabIndex = 146;
			this.HideBn.UseVisualStyleBackColor = true;
			this.HideBn.MouseDown += new System.Windows.Forms.MouseEventHandler(HideBn_MouseDown);
			this.HideBn.MouseUp += new System.Windows.Forms.MouseEventHandler(HideBn_MouseUp);
			this.LogUserCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.LogUserCB.FormattingEnabled = true;
			this.LogUserCB.Location = new System.Drawing.Point(157, 37);
			this.LogUserCB.Name = "LogUserCB";
			this.LogUserCB.Size = new System.Drawing.Size(259, 28);
			this.LogUserCB.TabIndex = 145;
			this.LogInBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("LogInBn.BackgroundImage");
			this.LogInBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.LogInBn.FlatAppearance.BorderSize = 0;
			this.LogInBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LogInBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.LogInBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.LogInBn.Location = new System.Drawing.Point(82, 133);
			this.LogInBn.Name = "LogInBn";
			this.LogInBn.Size = new System.Drawing.Size(120, 30);
			this.LogInBn.TabIndex = 144;
			this.LogInBn.Text = "LogIn";
			this.LogInBn.UseVisualStyleBackColor = true;
			this.LogInBn.Click += new System.EventHandler(LogInBn_Click);
			this.LogOutBn.BackgroundImage = (System.Drawing.Image)resources.GetObject("LogOutBn.BackgroundImage");
			this.LogOutBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.LogOutBn.FlatAppearance.BorderSize = 0;
			this.LogOutBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.LogOutBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.LogOutBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.LogOutBn.Location = new System.Drawing.Point(264, 133);
			this.LogOutBn.Name = "LogOutBn";
			this.LogOutBn.Size = new System.Drawing.Size(120, 30);
			this.LogOutBn.TabIndex = 143;
			this.LogOutBn.Text = "LogOut";
			this.LogOutBn.UseVisualStyleBackColor = true;
			this.LogOutBn.Click += new System.EventHandler(LogOutBn_Click);
			this.PasswordTB.Font = new System.Drawing.Font("新細明體", 10f);
			this.PasswordTB.Location = new System.Drawing.Point(157, 77);
			this.PasswordTB.Name = "PasswordTB";
			this.PasswordTB.Size = new System.Drawing.Size(259, 27);
			this.PasswordTB.TabIndex = 139;
			this.PasswordTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.lab_Password.Location = new System.Drawing.Point(31, 79);
			this.lab_Password.Name = "lab_Password";
			this.lab_Password.Size = new System.Drawing.Size(120, 25);
			this.lab_Password.TabIndex = 137;
			this.lab_Password.Text = "Password";
			this.lab_Password.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.lab_User.Location = new System.Drawing.Point(31, 37);
			this.lab_User.Name = "lab_User";
			this.lab_User.Size = new System.Drawing.Size(120, 25);
			this.lab_User.TabIndex = 138;
			this.lab_User.Text = "User";
			this.lab_User.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.tab_LogIn.Controls.Add(this.tabPage1);
			this.tab_LogIn.Controls.Add(this.tabPage2);
			this.tab_LogIn.Font = new System.Drawing.Font("新細明體", 12f);
			this.tab_LogIn.Location = new System.Drawing.Point(12, 50);
			this.tab_LogIn.Name = "tab_LogIn";
			this.tab_LogIn.SelectedIndex = 0;
			this.tab_LogIn.Size = new System.Drawing.Size(476, 216);
			this.tab_LogIn.TabIndex = 128;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 278);
			base.Controls.Add(this.tab_LogIn);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form507_LogIn";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form507_LogIn_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form507_LogIn_Paint);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tab_LogIn.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
