using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SD3_Graph
{
	public static class LoadResoureDll
	{
		private static Dictionary<string, Assembly> LoadedDlls = new Dictionary<string, Assembly>();

		private static Dictionary<string, object> Assemblies = new Dictionary<string, object>();

		private static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
		{
			try
			{
				string assName = new AssemblyName(args.Name).FullName;
				if (LoadedDlls.TryGetValue(assName, out var ass) && ass != null)
				{
					LoadedDlls[assName] = null;
					return ass;
				}
				throw new DllNotFoundException(assName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static void RegistDLL(string pattern = "*.dll")
		{
			Directory.GetFiles("", "");
			Assembly ass = new StackTrace(0).GetFrame(1).GetMethod().Module.Assembly;
			if (Assemblies.ContainsKey(ass.FullName))
			{
				return;
			}
			Assemblies.Add(ass.FullName, null);
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
			string[] res = ass.GetManifestResourceNames();
			Regex regex = new Regex("^" + pattern.Replace(".", "\\.").Replace("*", ".*").Replace("_", ".") + "$", RegexOptions.IgnoreCase);
			string[] array = res;
			foreach (string r in array)
			{
				if (!regex.IsMatch(r))
				{
					continue;
				}
				try
				{
					Stream s = ass.GetManifestResourceStream(r);
					byte[] bts = new byte[s.Length];
					s.Read(bts, 0, (int)s.Length);
					Assembly da = Assembly.Load(bts);
					if (!LoadedDlls.ContainsKey(da.FullName))
					{
						LoadedDlls[da.FullName] = da;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("error:Load dll fail\n address：RegistDLL()！\n Detail:" + ex.Message);
				}
			}
		}
	}
}
