using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace SD3_Graph
{
	internal static class Program
	{
		private static Mutex mutex = new Mutex(true, "MyAppMutex");

		[STAThread]
		private static void Main()
		{
			try
			{
				MultiLanguage.CheckAndCreateXml();
				string processName = Process.GetCurrentProcess().ProcessName;
				Process[] processes = Process.GetProcessesByName(processName);
				Process[] array = processes;
				foreach (Process process in array)
				{
					if (process.Id != Process.GetCurrentProcess().Id)
					{
						process.Kill();
					}
				}
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
				LoadResoureDll.RegistDLL();
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Form_001Main());
			}
			catch (Exception ex)
			{
				if (ex is DllNotFoundException || ex is TypeLoadException)
				{
					MessageBox.Show("You are missing some packages or other related resources, please install related packages or fix errors first.", "Unable to execute", MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				string errorMessage = ex.Message + " Err No." + ex.StackTrace;
				FormPublicFunction.SaveErrLog(errorMessage);
			}
		}
	}
}
