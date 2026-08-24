using System.Drawing;
using System.Windows.Forms;

namespace SD3_Graph
{
	public static class FormControlZoom
	{
		public static int ScreenWidth;

		public static int ScreenHeight;

		public static float ScreenWidthZoom;

		public static float ScreenHeightZoom;

		public static float ScreenFontZoom;

		public static void SetControlsMain(Control cons)
		{
			float NewX = ScreenWidthZoom;
			float NewY = ScreenHeightZoom;
			float FontZoom = ScreenFontZoom;
			foreach (Control con in cons.Controls)
			{
				try
				{
					if (!(con is Panel) && !(con is Form))
					{
						con.Width = (int)((float)con.Width * NewX);
						con.Height = (int)((float)con.Height * NewY);
						con.Left = (int)((float)con.Left * NewX);
						con.Top = (int)((float)con.Top * NewY);
						con.Font = new Font(con.Font.Name, con.Font.Size * FontZoom, con.Font.Style, con.Font.Unit);
						if (con.Controls.Count > 0)
						{
							SetControls(con);
						}
					}
				}
				catch
				{
				}
			}
		}

		public static void SetControls(Control cons)
		{
			float NewX = ScreenWidthZoom;
			float NewY = ScreenHeightZoom;
			float FontZoom = ScreenFontZoom;
			foreach (Control con in cons.Controls)
			{
				try
				{
					con.Width = (int)((float)con.Width * NewX);
					con.Height = (int)((float)con.Height * NewY);
					con.Left = (int)((float)con.Left * NewX);
					con.Top = (int)((float)con.Top * NewY);
					con.Font = new Font(con.Font.Name, con.Font.Size * FontZoom, con.Font.Style, con.Font.Unit);
					if (con.Controls.Count > 0)
					{
						SetControls(con);
					}
				}
				catch
				{
				}
			}
		}

		public static void ScaleForm(Form form)
		{
			float NewX = ScreenWidthZoom;
			float NewY = ScreenHeightZoom;
			form.Width = (int)((float)form.Width * NewX);
			form.Height = (int)((float)form.Height * NewY);
		}
	}
}
