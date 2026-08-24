using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SD3Soft.Properties;

namespace SD3_Graph
{
	public class Form590_MappingIO : Form
	{
		private GlobalVar GB = null;

		private TCPclient TCP = null;

		private TransferCSV TrCSV = null;

		public DataTable dt_Bit = new DataTable();

		private Image[] SWImg = new Image[2];

		private int Mode = 0;

		private int Page_Axis = 0;

		private IContainer components = null;

		private Label lab_Title;

		private Label CloseBn;

		private DataGridView dataGridView_Bit;

		public Form590_MappingIO(GlobalVar GB, TCPclient TCP, TransferCSV TrCSV, int Axis, int Mode)
		{
			InitializeComponent();
			MultiLanguage.LoadLanguage(this);
			this.GB = GB;
			this.TCP = TCP;
			this.TrCSV = TrCSV;
			this.Mode = Mode;
			Page_Axis = Axis;
			dataGridView_Bit.MouseClick += dataGridView_Bit_MouseClick;
			dataGridView_Bit.MouseDoubleClick += dataGridView_Bit_MouseClick;
			SWImg[0] = Resources.BIT0;
			SWImg[1] = Resources.BIT1;
			switch (Mode)
			{
			case 0:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_Title0");
				break;
			case 1:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_Title1");
				break;
			case 2:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_Title2");
				break;
			case 4:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_Title4");
				break;
			case 6:
				lab_Title.Text = MultiLanguage.GetStr(this, "tp_Title6");
				break;
			}
			dt_Bit.Columns.Add("Num", typeof(int));
			dt_Bit.Columns.Add("Bit7", typeof(Image));
			dt_Bit.Columns.Add("Bit6", typeof(Image));
			dt_Bit.Columns.Add("Bit5", typeof(Image));
			dt_Bit.Columns.Add("Bit4", typeof(Image));
			dt_Bit.Columns.Add("Bit3", typeof(Image));
			dt_Bit.Columns.Add("Bit2", typeof(Image));
			dt_Bit.Columns.Add("Bit1", typeof(Image));
			dt_Bit.Columns.Add("Bit0", typeof(Image));
			TrCSV.CtrlTableAllDataReadFromCtrl(Page_Axis, Mode);
			UpdateUI();
			loadGrid1(dataGridView_Bit);
		}

		public void loadGrid1(DataGridView dataGridView1)
		{
			dataGridView1.ScrollBars = ScrollBars.Vertical;
			dataGridView1.AllowUserToAddRows = false;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			dataGridView1.RowHeadersVisible = false;
			dataGridView1.Columns[0].HeaderText = "No.";
			for (int i = 1; i <= 8; i++)
			{
				dataGridView1.Columns[i].HeaderText = "Bit" + (8 - i);
				((DataGridViewImageColumn)dataGridView1.Columns[i]).ImageLayout = DataGridViewImageCellLayout.Zoom;
			}
		}

		private unsafe void UpdateUI()
		{
			dt_Bit.Rows.Clear();
			for (int j = 0; j < 256; j++)
			{
				DataRow BitRow = dt_Bit.NewRow();
				BitRow[0] = j;
				for (int i = 0; i < 8; i++)
				{
					if (Page_Axis == 0)
					{
						if (Mode == 0)
						{
							BitRow[8 - i] = ((((GB.FSCtrlDOBitsTable_X.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
						}
						else if (Mode == 1)
						{
							BitRow[8 - i] = ((((GB.FSCtrlDIBitsTable_X.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
						}
						else if (Mode == 2)
						{
							BitRow[8 - i] = ((((GB.FSCtrlDOParamTable_X.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
						}
						else if (Mode == 4)
						{
							BitRow[8 - i] = ((((GB.FSCtrlDOScrewTable_X.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
						}
						else if (Mode == 6)
						{
							BitRow[8 - i] = ((((GB.FSCtrlDOSeqTable_X.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
						}
					}
					else if (Mode == 0)
					{
						BitRow[8 - i] = ((((GB.FSCtrlDOBitsTable_Y.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
					}
					else if (Mode == 1)
					{
						BitRow[8 - i] = ((((GB.FSCtrlDIBitsTable_Y.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
					}
					else if (Mode == 2)
					{
						BitRow[8 - i] = ((((GB.FSCtrlDOParamTable_Y.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
					}
					else if (Mode == 4)
					{
						BitRow[8 - i] = ((((GB.FSCtrlDOScrewTable_Y.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
					}
					else if (Mode == 6)
					{
						BitRow[8 - i] = ((((GB.FSCtrlDOSeqTable_Y.IOTableFunction[j] >> i) & 1) > 0) ? SWImg[1] : SWImg[0]);
					}
				}
				dt_Bit.Rows.Add(BitRow);
			}
			dataGridView_Bit.DataSource = dt_Bit;
		}

		private unsafe void dataGridView_Bit_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left)
			{
				return;
			}
			int currentMouseOverRow = dataGridView_Bit.HitTest(e.X, e.Y).RowIndex;
			int currentMouseOverCol = dataGridView_Bit.HitTest(e.X, e.Y).ColumnIndex;
			if (currentMouseOverRow < 0 || currentMouseOverCol < 0)
			{
				return;
			}
			DataRow dr = dt_Bit.Rows[currentMouseOverRow];
			if (currentMouseOverCol < 1 || currentMouseOverCol > 8)
			{
				return;
			}
			string title = "Bit" + (8 - currentMouseOverCol);
			dr[title] = ((dr[title] == SWImg[1]) ? SWImg[0] : SWImg[1]);
			if (Page_Axis == 0)
			{
				if (Mode == 0)
				{
					ref ushort reference = ref GB.FSCtrlDOBitsTable_X.IOTableFunction[currentMouseOverRow];
					reference ^= (ushort)(1 << 8 - currentMouseOverCol);
				}
				else if (Mode == 1)
				{
					ref ushort reference2 = ref GB.FSCtrlDIBitsTable_X.IOTableFunction[currentMouseOverRow];
					reference2 ^= (ushort)(1 << 8 - currentMouseOverCol);
				}
				else if (Mode == 2)
				{
					ref ushort reference3 = ref GB.FSCtrlDOParamTable_X.IOTableFunction[currentMouseOverRow];
					reference3 ^= (ushort)(1 << 8 - currentMouseOverCol);
				}
				else if (Mode == 4)
				{
					ref ushort reference4 = ref GB.FSCtrlDOScrewTable_X.IOTableFunction[currentMouseOverRow];
					reference4 ^= (ushort)(1 << 8 - currentMouseOverCol);
				}
				else if (Mode == 6)
				{
					ref ushort reference5 = ref GB.FSCtrlDOSeqTable_X.IOTableFunction[currentMouseOverRow];
					reference5 ^= (ushort)(1 << 8 - currentMouseOverCol);
				}
			}
			else if (Mode == 0)
			{
				ref ushort reference6 = ref GB.FSCtrlDOBitsTable_Y.IOTableFunction[currentMouseOverRow];
				reference6 ^= (ushort)(1 << 8 - currentMouseOverCol);
			}
			else if (Mode == 1)
			{
				ref ushort reference7 = ref GB.FSCtrlDIBitsTable_Y.IOTableFunction[currentMouseOverRow];
				reference7 ^= (ushort)(1 << 8 - currentMouseOverCol);
			}
			else if (Mode == 2)
			{
				ref ushort reference8 = ref GB.FSCtrlDOParamTable_Y.IOTableFunction[currentMouseOverRow];
				reference8 ^= (ushort)(1 << 8 - currentMouseOverCol);
			}
			else if (Mode == 4)
			{
				ref ushort reference9 = ref GB.FSCtrlDOScrewTable_Y.IOTableFunction[currentMouseOverRow];
				reference9 ^= (ushort)(1 << 8 - currentMouseOverCol);
			}
			else if (Mode == 6)
			{
				ref ushort reference10 = ref GB.FSCtrlDOSeqTable_Y.IOTableFunction[currentMouseOverRow];
				reference10 ^= (ushort)(1 << 8 - currentMouseOverCol);
			}
			TCP.FSIDWrite_ByTCP(508, 0, (ushort)Page_Axis, (ushort)Mode, 0, 0);
		}

		private void CloseBn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Form590_MappingIO_Paint(object sender, PaintEventArgs e)
		{
			Pen pen1 = new Pen(Color.DodgerBlue, 8f);
			e.Graphics.DrawRectangle(pen1, 0, 0, base.Width - 1, base.Height - 1);
		}

		private void Form590_MappingIO_Load(object sender, EventArgs e)
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
			this.dataGridView_Bit = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Bit).BeginInit();
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
			this.CloseBn.Location = new System.Drawing.Point(468, 2);
			this.CloseBn.Name = "CloseBn";
			this.CloseBn.Size = new System.Drawing.Size(36, 40);
			this.CloseBn.TabIndex = 127;
			this.CloseBn.Text = "X";
			this.CloseBn.Click += new System.EventHandler(CloseBn_Click);
			this.dataGridView_Bit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView_Bit.Location = new System.Drawing.Point(50, 57);
			this.dataGridView_Bit.Name = "dataGridView_Bit";
			this.dataGridView_Bit.RowHeadersWidth = 51;
			this.dataGridView_Bit.RowTemplate.Height = 24;
			this.dataGridView_Bit.Size = new System.Drawing.Size(402, 427);
			this.dataGridView_Bit.TabIndex = 132;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.ClientSize = new System.Drawing.Size(500, 522);
			base.Controls.Add(this.dataGridView_Bit);
			base.Controls.Add(this.CloseBn);
			base.Controls.Add(this.lab_Title);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Name = "Form590_MappingIO";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.Load += new System.EventHandler(Form590_MappingIO_Load);
			base.Paint += new System.Windows.Forms.PaintEventHandler(Form590_MappingIO_Paint);
			((System.ComponentModel.ISupportInitialize)this.dataGridView_Bit).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
