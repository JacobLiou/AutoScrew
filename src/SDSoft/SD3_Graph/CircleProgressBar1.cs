using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SD3_Graph
{
	public class CircleProgressBar1 : Control
	{
		private Pen penBottom = null;

		private Pen penTop = null;

		private int maxValue = 999999;

		private int progress = 999999;

		private Color bottomColor = Color.FromArgb(224, 224, 224);

		private Color topColor = Color.FromArgb(78, 134, 239);

		private Color finishedColor = Color.FromArgb(78, 134, 239);

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 33554432;
				return cp;
			}
		}

		public int MaxValue
		{
			get
			{
				return maxValue;
			}
			set
			{
				if (value >= progress)
				{
					maxValue = value;
					Invalidate();
				}
			}
		}

		public int Progress
		{
			get
			{
				return progress;
			}
			set
			{
				if (value <= maxValue)
				{
					progress = value;
					Invalidate();
				}
			}
		}

		public Color BottomColor
		{
			get
			{
				return bottomColor;
			}
			set
			{
				bottomColor = value;
				penBottom.Color = value;
				Invalidate();
			}
		}

		public Color TopColor
		{
			get
			{
				return topColor;
			}
			set
			{
				topColor = value;
				penTop.Color = value;
				Invalidate();
			}
		}

		public Color FinishedColor
		{
			get
			{
				return finishedColor;
			}
			set
			{
				finishedColor = value;
				Invalidate();
			}
		}

		public CircleProgressBar1()
		{
			base.Width = 350;
			base.Height = 350;
			BackColor = Color.White;
			penBottom = new Pen(bottomColor, 20f);
			penTop = new Pen(topColor, 20f);
			base.SizeChanged += delegate
			{
				Invalidate();
			};
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawShape(e.Graphics);
		}

		private void DrawShape(Graphics g)
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.CompositingQuality = CompositingQuality.HighQuality;
			int size = Math.Min(base.Width, base.Height);
			int sizeWOffs = (int)(20f * FormControlZoom.ScreenWidthZoom);
			int sizeHOffs = (int)(20f * FormControlZoom.ScreenHeightZoom);
			Rectangle rectangle = new Rectangle(base.Width / 2 - size / 2 + sizeWOffs, base.Height / 2 - size / 2 + sizeHOffs, size - sizeWOffs * 2, size - sizeHOffs * 2);
			g.DrawArc(penBottom, rectangle, 0f, 360f);
			double topAngle = (double)progress / (double)maxValue * 360.0;
			g.DrawArc(penTop, rectangle, 270f, (int)topAngle);
			SizeF proValSize = g.MeasureString(progress.ToString(), Font);
			g.DrawString(progress.ToString(), Font, new SolidBrush(ForeColor), (float)(rectangle.X + rectangle.Width / 2) - proValSize.Width / 2f, (float)(rectangle.Y + rectangle.Height / 2) - proValSize.Height / 2f - (float)sizeHOffs);
			SizeF maxValSize = g.MeasureString(maxValue.ToString(), Font);
			g.DrawString(maxValue.ToString(), Font, new SolidBrush(ForeColor), (float)(rectangle.X + rectangle.Width / 2) - maxValSize.Width / 2f, (float)(rectangle.Y + rectangle.Height / 2) - maxValSize.Height / 2f + (float)sizeHOffs);
			Pen myPen = new Pen(Color.Black, 2f);
			g.DrawLine(myPen, rectangle.X + rectangle.Width / 2 - base.Width / 5, rectangle.Y + rectangle.Height / 2, rectangle.X + rectangle.Width / 2 + base.Width / 5, rectangle.Y + rectangle.Height / 2);
		}
	}
}
