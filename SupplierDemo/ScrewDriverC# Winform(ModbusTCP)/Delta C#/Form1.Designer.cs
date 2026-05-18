namespace ScrewDriver
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.IPTB = new System.Windows.Forms.TextBox();
            this.IPlab = new System.Windows.Forms.Label();
            this.ConnectBn = new System.Windows.Forms.Button();
            this.StatusTB = new System.Windows.Forms.TextBox();
            this.MsgTB = new System.Windows.Forms.TextBox();
            this.StartBn = new System.Windows.Forms.Button();
            this.StopBn = new System.Windows.Forms.Button();
            this.AutoRunningCB = new System.Windows.Forms.CheckBox();
            this.RstBn = new System.Windows.Forms.Button();
            this.S5Bn = new System.Windows.Forms.Button();
            this.S4Bn = new System.Windows.Forms.Button();
            this.S3Bn = new System.Windows.Forms.Button();
            this.S2Bn = new System.Windows.Forms.Button();
            this.ConnHelpBn = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.S1Bn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // IPTB
            // 
            this.IPTB.Location = new System.Drawing.Point(69, 18);
            this.IPTB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.IPTB.Name = "IPTB";
            this.IPTB.Size = new System.Drawing.Size(173, 29);
            this.IPTB.TabIndex = 0;
            this.IPTB.Text = "192.168.1.11";
            // 
            // IPlab
            // 
            this.IPlab.AutoSize = true;
            this.IPlab.Location = new System.Drawing.Point(21, 23);
            this.IPlab.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.IPlab.Name = "IPlab";
            this.IPlab.Size = new System.Drawing.Size(23, 18);
            this.IPlab.TabIndex = 1;
            this.IPlab.Text = "IP";
            // 
            // ConnectBn
            // 
            this.ConnectBn.Location = new System.Drawing.Point(250, 15);
            this.ConnectBn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ConnectBn.Name = "ConnectBn";
            this.ConnectBn.Size = new System.Drawing.Size(112, 35);
            this.ConnectBn.TabIndex = 2;
            this.ConnectBn.Text = "Connect";
            this.ConnectBn.UseVisualStyleBackColor = true;
            this.ConnectBn.Click += new System.EventHandler(this.Connect_Click);
            // 
            // StatusTB
            // 
            this.StatusTB.Location = new System.Drawing.Point(472, 24);
            this.StatusTB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StatusTB.Name = "StatusTB";
            this.StatusTB.ReadOnly = true;
            this.StatusTB.Size = new System.Drawing.Size(450, 29);
            this.StatusTB.TabIndex = 0;
            // 
            // MsgTB
            // 
            this.MsgTB.Location = new System.Drawing.Point(13, 397);
            this.MsgTB.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MsgTB.Multiline = true;
            this.MsgTB.Name = "MsgTB";
            this.MsgTB.ReadOnly = true;
            this.MsgTB.Size = new System.Drawing.Size(578, 528);
            this.MsgTB.TabIndex = 0;
            // 
            // StartBn
            // 
            this.StartBn.Location = new System.Drawing.Point(83, 352);
            this.StartBn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StartBn.Name = "StartBn";
            this.StartBn.Size = new System.Drawing.Size(112, 35);
            this.StartBn.TabIndex = 2;
            this.StartBn.Text = "Start";
            this.StartBn.UseVisualStyleBackColor = true;
            this.StartBn.Click += new System.EventHandler(this.StartBn_Click);
            // 
            // StopBn
            // 
            this.StopBn.Location = new System.Drawing.Point(203, 352);
            this.StopBn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StopBn.Name = "StopBn";
            this.StopBn.Size = new System.Drawing.Size(112, 35);
            this.StopBn.TabIndex = 2;
            this.StopBn.Text = "Stop";
            this.StopBn.UseVisualStyleBackColor = true;
            this.StopBn.Click += new System.EventHandler(this.StopBn_Click);
            // 
            // AutoRunningCB
            // 
            this.AutoRunningCB.AutoSize = true;
            this.AutoRunningCB.Location = new System.Drawing.Point(83, 321);
            this.AutoRunningCB.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.AutoRunningCB.Name = "AutoRunningCB";
            this.AutoRunningCB.Size = new System.Drawing.Size(128, 22);
            this.AutoRunningCB.TabIndex = 3;
            this.AutoRunningCB.Text = "Auto Running";
            this.AutoRunningCB.UseVisualStyleBackColor = true;
            this.AutoRunningCB.CheckedChanged += new System.EventHandler(this.AutoRunningCB_CheckedChanged);
            // 
            // RstBn
            // 
            this.RstBn.Location = new System.Drawing.Point(932, 21);
            this.RstBn.Margin = new System.Windows.Forms.Padding(4);
            this.RstBn.Name = "RstBn";
            this.RstBn.Size = new System.Drawing.Size(112, 29);
            this.RstBn.TabIndex = 4;
            this.RstBn.Text = "Reset Setting";
            this.RstBn.UseVisualStyleBackColor = true;
            this.RstBn.Click += new System.EventHandler(this.RstBn_Click);
            // 
            // S5Bn
            // 
            this.S5Bn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("S5Bn.BackgroundImage")));
            this.S5Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.S5Bn.FlatAppearance.BorderSize = 0;
            this.S5Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.S5Bn.Location = new System.Drawing.Point(389, 265);
            this.S5Bn.Name = "S5Bn";
            this.S5Bn.Size = new System.Drawing.Size(35, 37);
            this.S5Bn.TabIndex = 16;
            this.S5Bn.UseVisualStyleBackColor = true;
            this.S5Bn.Click += new System.EventHandler(this.S5Bn_Click);
            // 
            // S4Bn
            // 
            this.S4Bn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("S4Bn.BackgroundImage")));
            this.S4Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.S4Bn.FlatAppearance.BorderSize = 0;
            this.S4Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.S4Bn.Location = new System.Drawing.Point(389, 222);
            this.S4Bn.Name = "S4Bn";
            this.S4Bn.Size = new System.Drawing.Size(35, 37);
            this.S4Bn.TabIndex = 17;
            this.S4Bn.UseVisualStyleBackColor = true;
            this.S4Bn.Click += new System.EventHandler(this.S4Bn_Click);
            // 
            // S3Bn
            // 
            this.S3Bn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("S3Bn.BackgroundImage")));
            this.S3Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.S3Bn.FlatAppearance.BorderSize = 0;
            this.S3Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.S3Bn.Location = new System.Drawing.Point(389, 177);
            this.S3Bn.Name = "S3Bn";
            this.S3Bn.Size = new System.Drawing.Size(35, 37);
            this.S3Bn.TabIndex = 18;
            this.S3Bn.UseVisualStyleBackColor = true;
            this.S3Bn.Click += new System.EventHandler(this.S3Bn_Click);
            // 
            // S2Bn
            // 
            this.S2Bn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("S2Bn.BackgroundImage")));
            this.S2Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.S2Bn.FlatAppearance.BorderSize = 0;
            this.S2Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.S2Bn.Location = new System.Drawing.Point(389, 135);
            this.S2Bn.Name = "S2Bn";
            this.S2Bn.Size = new System.Drawing.Size(35, 37);
            this.S2Bn.TabIndex = 19;
            this.S2Bn.UseVisualStyleBackColor = true;
            this.S2Bn.Click += new System.EventHandler(this.S2Bn_Click);
            // 
            // ConnHelpBn
            // 
            this.ConnHelpBn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ConnHelpBn.BackgroundImage")));
            this.ConnHelpBn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ConnHelpBn.FlatAppearance.BorderSize = 0;
            this.ConnHelpBn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ConnHelpBn.Location = new System.Drawing.Point(388, 17);
            this.ConnHelpBn.Name = "ConnHelpBn";
            this.ConnHelpBn.Size = new System.Drawing.Size(35, 37);
            this.ConnHelpBn.TabIndex = 20;
            this.ConnHelpBn.UseVisualStyleBackColor = true;
            this.ConnHelpBn.Click += new System.EventHandler(this.ConnHelpBn_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(608, 104);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(431, 819);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 321);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 18);
            this.label6.TabIndex = 8;
            this.label6.Text = "Step6 :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 279);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(196, 18);
            this.label5.TabIndex = 9;
            this.label5.Text = "Step5 : Set DI/DO function";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 231);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(301, 18);
            this.label4.TabIndex = 10;
            this.label4.Text = "Step4 : Start condition \"DI or Lever Start\"";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(277, 18);
            this.label3.TabIndex = 11;
            this.label3.Text = "Step3 : The source uses this parameter";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(268, 18);
            this.label2.TabIndex = 12;
            this.label2.Text = "Step2 : Create a tightening parameter";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(100, 111);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(263, 18);
            this.label7.TabIndex = 13;
            this.label7.Text = "Use version 1.00.00.170.xxx or above";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 18);
            this.label1.TabIndex = 14;
            this.label1.Text = "Step1 : Check Controller Firmware";
            // 
            // S1Bn
            // 
            this.S1Bn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("S1Bn.BackgroundImage")));
            this.S1Bn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.S1Bn.FlatAppearance.BorderSize = 0;
            this.S1Bn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.S1Bn.Location = new System.Drawing.Point(389, 82);
            this.S1Bn.Name = "S1Bn";
            this.S1Bn.Size = new System.Drawing.Size(35, 37);
            this.S1Bn.TabIndex = 20;
            this.S1Bn.UseVisualStyleBackColor = true;
            this.S1Bn.Click += new System.EventHandler(this.S1Bn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 1026);
            this.Controls.Add(this.S5Bn);
            this.Controls.Add(this.S4Bn);
            this.Controls.Add(this.S3Bn);
            this.Controls.Add(this.S2Bn);
            this.Controls.Add(this.S1Bn);
            this.Controls.Add(this.ConnHelpBn);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RstBn);
            this.Controls.Add(this.AutoRunningCB);
            this.Controls.Add(this.StopBn);
            this.Controls.Add(this.StartBn);
            this.Controls.Add(this.ConnectBn);
            this.Controls.Add(this.IPlab);
            this.Controls.Add(this.MsgTB);
            this.Controls.Add(this.StatusTB);
            this.Controls.Add(this.IPTB);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Example_ModbusTCP";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IPTB;
        private System.Windows.Forms.Label IPlab;
        private System.Windows.Forms.Button ConnectBn;
        private System.Windows.Forms.TextBox StatusTB;
        private System.Windows.Forms.TextBox MsgTB;
        private System.Windows.Forms.Button StartBn;
        private System.Windows.Forms.Button StopBn;
        private System.Windows.Forms.CheckBox AutoRunningCB;
        private System.Windows.Forms.Button RstBn;
        private System.Windows.Forms.Button S5Bn;
        private System.Windows.Forms.Button S4Bn;
        private System.Windows.Forms.Button S3Bn;
        private System.Windows.Forms.Button S2Bn;
        private System.Windows.Forms.Button ConnHelpBn;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button S1Bn;
    }
}

