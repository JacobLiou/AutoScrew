using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form210_SeqPicture : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private PictureBox GuidePicPB = new PictureBox();

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private Panel SeqPicChoosePL;

		private Button OpenGuidePicBn;

		private Button SaveGuidePicBn;

		private Button DelGuidePicBn;

		private Label lab_RemainSpaceSize;

		public event CreateForm210_ChooseHandler CreateCloseEvent;

		public Form210_SeqPicture(GlobalVar GB, TCPclient TCP, ref Image Img)
		{
			InitializeComponent();
			this.GB = GB;
			this.TCP = TCP;
			MultiLanguage.LoadLanguage(this);
			if (Img != null)
			{
				GuidePicPB.Image = Img;
				GuidePicPB.Dock = DockStyle.Fill;
				GuidePicPB.SizeMode = PictureBoxSizeMode.StretchImage;
				SeqPicChoosePL.Controls.Add(GuidePicPB);
			}
			if (GB.CheckHMIVer(169, 14) && GB.UISys.IsGuidePicFromCtrl)
			{
				TCP.FSIDRead_ByTCP(262, 0, 0, 0, 0, 0);
				lab_RemainSpaceSize.Text = "Remaining: " + GB.SeqRemainSpaceSize + "MB";
				lab_RemainSpaceSize.Visible = true;
			}
			else
			{
				lab_RemainSpaceSize.Visible = false;
			}
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form210_SeqPicture_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form210_SeqPicture_Load(object sender, EventArgs e)
		{
			SeqPicChoosePL.Paint += SeqPicChoosePL_Paint;
			FormControlZoom.SetControls(this);
			FormControlZoom.ScaleForm(this);
		}

		private void SeqPicChoosePL_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.FromArgb(0, 255, 255), 5f);
			Control control = sender as Control;
			e.Graphics.DrawRectangle(pen1, 0, 0, control.Width - 4, control.Height - 4);
		}

		private void OpenGuidePicBn_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Choose Picture";
			openFileDialog.Filter = "PNG Files|*.png";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = openFileDialog.FileName;
				GuidePicPB.Image = (File.Exists(filePath) ? GB.LoadPicture(filePath) : null);
				GuidePicPB.Dock = DockStyle.Fill;
				GuidePicPB.SizeMode = PictureBoxSizeMode.StretchImage;
				SeqPicChoosePL.Controls.Add(GuidePicPB);
			}
		}

		private void SaveGuidePicBn_Click(object sender, EventArgs e)
		{
			if (GB.SeqRemainSpaceSize != 0 || !GB.UISys.IsGuidePicFromCtrl || !GB.UISys.PCSoftSupport)
			{
				if (this.CreateCloseEvent != null)
				{
					this.CreateCloseEvent(GuidePicPB.Image);
				}
				Close();
			}
			else
			{
				Form995_RemindOKNG Form995 = new Form995_RemindOKNG(GB, 3190, "");
				Form995.Show(this);
			}
		}

		private void DelGuidePicBn_Click(object sender, EventArgs e)
		{
			if (this.CreateCloseEvent != null)
			{
				this.CreateCloseEvent(null);
			}
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
			this.lab_Title = new System.Windows.Forms.Label();
			this.CloseBn = new System.Windows.Forms.Label();
			this.SeqPicChoosePL = new System.Windows.Forms.Panel();
			this.OpenGuidePicBn = new System.Windows.Forms.Button();
			this.SaveGuidePicBn = new System.Windows.Forms.Button();
			this.DelGuidePicBn = new System.Windows.Forms.Button();
			this.lab_RemainSpaceSize = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.lab_Title.BackColor = System.Drawing.Color.DodgerBlue;
			this.lab_Title.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_Title.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
			this.lab_Title.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lab_Title.Location = new System.Drawing.Point(0, 0);
			this.lab_Title.Name = "lab_Title";
			this.lab_Title.Size = new System.Drawing.Size(818, 35);
			this.lab_Title.TabIndex = 63;
			this.lab_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.CloseBn.AutoSize = true;
			this.CloseBn.BackColor = System.Drawing.Color.DodgerBlue;
			this.CloseBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.CloseBn.Font = new System.Drawing.Font("Arial Narrow", 20.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.CloseBn.ForeColor = System.Drawing.Color.White;
			this.CloseBn.Location = new System.Drawing.Point(781, -3);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.SeqPicChoosePL.BackColor = System.Drawing.Color.White;
			this.SeqPicChoosePL.Location = new System.Drawing.Point(37, 103);
			this.SeqPicChoosePL.Name = "SeqPicChoosePL";
			this.SeqPicChoosePL.Size = new System.Drawing.Size(736, 460);
			this.SeqPicChoosePL.TabIndex = 170;
			this.OpenGuidePicBn.BackgroundImage = SD3Soft.Properties.Resources.開啟舊檔_灰;
			this.OpenGuidePicBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.OpenGuidePicBn.FlatAppearance.BorderSize = 0;
			this.OpenGuidePicBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.OpenGuidePicBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.OpenGuidePicBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.OpenGuidePicBn.Location = new System.Drawing.Point(37, 46);
			this.OpenGuidePicBn.Margin = new System.Windows.Forms.Padding(4);
			this.OpenGuidePicBn.Name = "OpenGuidePicBn";
			this.OpenGuidePicBn.Size = new System.Drawing.Size(53, 50);
			this.OpenGuidePicBn.TabIndex = 176;
			this.OpenGuidePicBn.UseVisualStyleBackColor = true;
			this.OpenGuidePicBn.Click += new System.EventHandler(OpenGuidePicBn_Click);
			this.SaveGuidePicBn.BackgroundImage = SD3Soft.Properties.Resources.存檔_50x50__灰1;
			this.SaveGuidePicBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.SaveGuidePicBn.FlatAppearance.BorderSize = 0;
			this.SaveGuidePicBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.SaveGuidePicBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.SaveGuidePicBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.SaveGuidePicBn.Location = new System.Drawing.Point(98, 46);
			this.SaveGuidePicBn.Margin = new System.Windows.Forms.Padding(4);
			this.SaveGuidePicBn.Name = "SaveGuidePicBn";
			this.SaveGuidePicBn.Size = new System.Drawing.Size(53, 50);
			this.SaveGuidePicBn.TabIndex = 175;
			this.SaveGuidePicBn.UseVisualStyleBackColor = true;
			this.SaveGuidePicBn.Click += new System.EventHandler(SaveGuidePicBn_Click);
			this.DelGuidePicBn.BackgroundImage = SD3Soft.Properties.Resources.B_Del_ICON_01;
			this.DelGuidePicBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.DelGuidePicBn.FlatAppearance.BorderSize = 0;
			this.DelGuidePicBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.DelGuidePicBn.Font = new System.Drawing.Font("新細明體", 12f);
			this.DelGuidePicBn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.DelGuidePicBn.Location = new System.Drawing.Point(720, 46);
			this.DelGuidePicBn.Margin = new System.Windows.Forms.Padding(4);
			this.DelGuidePicBn.Name = "DelGuidePicBn";
			this.DelGuidePicBn.Size = new System.Drawing.Size(53, 50);
			this.DelGuidePicBn.TabIndex = 174;
			this.DelGuidePicBn.UseVisualStyleBackColor = true;
			this.DelGuidePicBn.Click += new System.EventHandler(DelGuidePicBn_Click);
			this.lab_RemainSpaceSize.AutoSize = true;
			this.lab_RemainSpaceSize.Font = new System.Drawing.Font("新細明體", 12f);
			this.lab_RemainSpaceSize.Location = new System.Drawing.Point(158, 76);
			this.lab_RemainSpaceSize.Name = "lab_RemainSpaceSize";
			this.lab_RemainSpaceSize.Size = new System.Drawing.Size(185, 20);
			this.lab_RemainSpaceSize.TabIndex = 177;
			this.lab_RemainSpaceSize.Text = "Remaining:            MB";
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(818, 610);
			base.Controls.Add(this.lab_RemainSpaceSize);
			base.Controls.Add(this.OpenGuidePicBn);
			base.Controls.Add(this.SaveGuidePicBn);
			base.Controls.Add(this.DelGuidePicBn);
			base.Controls.Add(this.SeqPicChoosePL);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form210_SeqPicture";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form210_SeqPicture_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form210_SeqPicture_Paint);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
