using System;
using System.Diagnostics;
using System.IO;

namespace SD3_Graph
{
	public static class FormPublicFunction
	{
		public static void SaveErrLog(string MessageStr)
		{
			string Path1 = ".\\Log\\";
			string Path2 = DateTime.Now.ToString("yyyyMM") + "\\";
			if (!Directory.Exists(Path1))
			{
				Directory.CreateDirectory(Path1);
			}
			if (!Directory.Exists(Path1 + Path2))
			{
				Directory.CreateDirectory(Path1 + Path2);
			}
			using (StreamWriter writer = new StreamWriter(Path1 + Path2 + DateTime.Now.ToString("yyyyMMdd") + ".txt", true))
			{
				writer.WriteLine("===========  " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + Process.GetCurrentProcess().ProcessName + "  ===========");
				writer.WriteLine(MessageStr);
			}
		}
	}
}
