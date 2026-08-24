using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using SD3Soft.Properties;

namespace SD3_Graph
{
	internal class MultiLanguage
	{
		public static string DefaultLanguage = "English";

		private static string GetDefLanguage = "";

		private static string DefaultEthernetIP = "192.168.1.11";

		private static string GetDefEthernetIP = "";

		private static string DefaultSeqGuidePicFromCtrl = "0";

		private static string GetDefSeqGuidePicFromCtrl = "";

		private static string DefaultIsReadSupportFTP = "1";

		private static string GetDefIsReadSupportFTP = "";

		private static string DefaultIsAutoSize = "0";

		private static string GetDefIsAutoSize = "";

		private static Dictionary<string, Tuple<string, float>> hashForm500Text;

		public static void CheckAndCreateXml()
		{
			string folderPath = Path.Combine(Application.StartupPath, "Languages");
			string filePath = Path.Combine(folderPath, "DefaultLanguage.xml");
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			if (!File.Exists(filePath))
			{
				CreateDefaultXml(filePath);
				return;
			}
			XmlDocument doc = new XmlDocument();
			bool isModified = false;
			try
			{
				doc.Load(filePath);
			}
			catch (XmlException)
			{
				CreateDefaultXml(filePath);
				return;
			}
			XmlNode root = doc.SelectSingleNode("/Softimite");
			if (root == null)
			{
				CreateDefaultXml(filePath);
				return;
			}
			try
			{
				if (root is XmlElement rootElem && string.IsNullOrEmpty(rootElem.GetAttribute("Language")))
				{
					rootElem.SetAttribute("Language", "默認語言");
					isModified = true;
				}
			}
			catch (XmlException)
			{
				CreateDefaultXml(filePath);
				return;
			}
			KeyValuePair<string, string>[] requiredNodes = new KeyValuePair<string, string>[5]
			{
				new KeyValuePair<string, string>("DefaultLanguage", "English"),
				new KeyValuePair<string, string>("EthIP", "192.168.1.11"),
				new KeyValuePair<string, string>("SeqGuidePicFromCtrl", "1"),
				new KeyValuePair<string, string>("IsReadUseFTP", "1"),
				new KeyValuePair<string, string>("IsAutoSize", "0")
			};
			KeyValuePair<string, string>[] array = requiredNodes;
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<string, string> nodePair = array[i];
				string nodeName = nodePair.Key;
				string defaultValue = nodePair.Value;
				XmlNode node = root.SelectSingleNode(nodeName);
				if (node == null)
				{
					XmlElement newNode = doc.CreateElement(nodeName);
					newNode.InnerText = defaultValue;
					root.AppendChild(newNode);
					isModified = true;
				}
			}
			if (isModified)
			{
				doc.Save(filePath);
			}
		}

		private static void CreateDefaultXml(string filePath)
		{
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Indent = true,
				Encoding = Encoding.UTF8
			};
			using (XmlWriter writer = XmlWriter.Create(filePath, settings))
			{
				writer.WriteProcessingInstruction("xml", "version=\"1.0\" standalone=\"yes\"");
				writer.WriteStartElement("Softimite");
				writer.WriteAttributeString("Language", "默認語言");
				writer.WriteElementString("DefaultLanguage", "English");
				writer.WriteElementString("EthIP", "192.168.1.11");
				writer.WriteElementString("SeqGuidePicFromCtrl", "1");
				writer.WriteElementString("IsReadUseFTP", "1");
				writer.WriteElementString("IsAutoSize", "0");
				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}

		public static string GetDefaultLanguage()
		{
			if (GetDefLanguage == "")
			{
				string defaultLanguage = "English";
				try
				{
					XmlReader reader = new XmlTextReader("Languages/DefaultLanguage.xml");
					XmlDocument doc = new XmlDocument();
					doc.Load(reader);
					XmlNode root = doc.DocumentElement;
					XmlNode node = root.SelectSingleNode("DefaultLanguage");
					if (node != null)
					{
						defaultLanguage = node.InnerText;
					}
					reader.Close();
				}
				catch
				{
				}
				GetDefLanguage = defaultLanguage;
			}
			return GetDefLanguage;
		}

		public static string GetDefaultEthernetIP()
		{
			if (GetDefEthernetIP == "")
			{
				string DefIP = "192.168.1.11";
				try
				{
					XmlReader reader = new XmlTextReader("Languages/DefaultLanguage.xml");
					XmlDocument doc = new XmlDocument();
					doc.Load(reader);
					XmlNode root = doc.DocumentElement;
					XmlNode node1 = root.SelectSingleNode("EthIP");
					if (node1 != null)
					{
						DefIP = node1.InnerText;
					}
					reader.Close();
				}
				catch
				{
				}
				GetDefEthernetIP = DefIP;
			}
			return GetDefEthernetIP;
		}

		public static string GetDefaultSeqGuidePicFromCtrl()
		{
			if (GetDefSeqGuidePicFromCtrl == "")
			{
				string defGuidePicFromCtrl = "0";
				try
				{
					XmlReader reader = new XmlTextReader("Languages/DefaultLanguage.xml");
					XmlDocument doc = new XmlDocument();
					doc.Load(reader);
					XmlNode root = doc.DocumentElement;
					XmlNode node = root.SelectSingleNode("SeqGuidePicFromCtrl");
					if (node != null)
					{
						defGuidePicFromCtrl = node.InnerText;
					}
					reader.Close();
				}
				catch
				{
				}
				GetDefSeqGuidePicFromCtrl = defGuidePicFromCtrl;
			}
			return GetDefSeqGuidePicFromCtrl;
		}

		public static string GetDefaultIsReadUseFTP()
		{
			if (GetDefIsReadSupportFTP == "")
			{
				string defIsReadSupportFTP = "1";
				try
				{
					XmlReader reader = new XmlTextReader("Languages/DefaultLanguage.xml");
					XmlDocument doc = new XmlDocument();
					doc.Load(reader);
					XmlNode root = doc.DocumentElement;
					XmlNode node = root.SelectSingleNode("IsReadUseFTP");
					if (node != null)
					{
						defIsReadSupportFTP = node.InnerText;
					}
					reader.Close();
				}
				catch
				{
				}
				GetDefIsReadSupportFTP = defIsReadSupportFTP;
			}
			return GetDefIsReadSupportFTP;
		}

		public static string GetDefaultIsAutoSize()
		{
			if (GetDefIsAutoSize == "")
			{
				string defIsAutoSize = "0";
				try
				{
					XmlReader reader = new XmlTextReader("Languages/DefaultLanguage.xml");
					XmlDocument doc = new XmlDocument();
					doc.Load(reader);
					XmlNode root = doc.DocumentElement;
					XmlNode node = root.SelectSingleNode("IsAutoSize");
					if (node != null)
					{
						defIsAutoSize = node.InnerText;
					}
					reader.Close();
				}
				catch
				{
				}
				GetDefIsAutoSize = defIsAutoSize;
			}
			return GetDefIsAutoSize;
		}

		public static void SetDefaultLanguage(string lang)
		{
			hashForm500Text = null;
			DataSet ds = new DataSet();
			ds.ReadXml("Languages/DefaultLanguage.xml");
			DataTable dt = ds.Tables["Softimite"];
			if (dt.Columns.Contains("DefaultLanguage"))
			{
				dt.Rows[0]["DefaultLanguage"] = lang;
				ds.AcceptChanges();
				ds.WriteXml("Languages/DefaultLanguage.xml");
			}
			DefaultLanguage = lang;
			GetDefLanguage = lang;
		}

		public static void SetDefaultEthernetIP(string IP)
		{
			DataSet ds = new DataSet();
			ds.ReadXml("Languages/DefaultLanguage.xml");
			DataTable dt = ds.Tables["Softimite"];
			if (dt.Columns.Contains("EthIP"))
			{
				dt.Rows[0]["EthIP"] = IP;
				ds.AcceptChanges();
				ds.WriteXml("Languages/DefaultLanguage.xml");
			}
			DefaultEthernetIP = IP;
			GetDefEthernetIP = IP;
		}

		public static void SetDefaultSeqGuidePicFromCtrl(string val)
		{
			DataSet ds = new DataSet();
			ds.ReadXml("Languages/DefaultLanguage.xml");
			DataTable dt = ds.Tables["Softimite"];
			if (dt.Columns.Contains("SeqGuidePicFromCtrl"))
			{
				dt.Rows[0]["SeqGuidePicFromCtrl"] = val;
				ds.AcceptChanges();
				ds.WriteXml("Languages/DefaultLanguage.xml");
			}
			DefaultSeqGuidePicFromCtrl = val;
			GetDefSeqGuidePicFromCtrl = val;
		}

		public static void SetDefaultIsReadUseFTP(string val)
		{
			DataSet ds = new DataSet();
			ds.ReadXml("Languages/DefaultLanguage.xml");
			DataTable dt = ds.Tables["Softimite"];
			if (dt.Columns.Contains("IsReadUseFTP"))
			{
				dt.Rows[0]["IsReadUseFTP"] = val;
				ds.AcceptChanges();
				ds.WriteXml("Languages/DefaultLanguage.xml");
			}
			DefaultIsReadSupportFTP = val;
			GetDefIsReadSupportFTP = val;
		}

		public static void SetDefaultIsAutoSize(string val)
		{
			DataSet ds = new DataSet();
			ds.ReadXml("Languages/DefaultLanguage.xml");
			DataTable dt = ds.Tables["Softimite"];
			if (dt.Columns.Contains("IsAutoSize"))
			{
				dt.Rows[0]["IsAutoSize"] = val;
				ds.AcceptChanges();
				ds.WriteXml("Languages/DefaultLanguage.xml");
			}
			DefaultIsAutoSize = val;
			GetDefIsAutoSize = val;
		}

		private static Dictionary<string, Tuple<string, float>> ReadXMLText(string frmName, string lang)
		{
			if (frmName == "Form500_Controller" && hashForm500Text != null)
			{
				return hashForm500Text;
			}
			try
			{
				Dictionary<string, Tuple<string, float>> hashText = new Dictionary<string, Tuple<string, float>>();
				XmlReader reader = null;
				switch (lang)
				{
				case "Chinese":
					reader = new XmlTextReader(new StringReader(Resources.Lang_Chinese));
					break;
				case "English":
					reader = new XmlTextReader(new StringReader(Resources.Lang_English));
					break;
				case "Sample":
					reader = new XmlTextReader(new StringReader(Resources.Lang_Sample));
					break;
				case "Japan":
					reader = new XmlTextReader(new StringReader(Resources.Lang_Japan));
					break;
				default:
					return null;
				}
				XmlDocument doc = new XmlDocument();
				doc.Load(reader);
				XmlNode root = doc.DocumentElement;
				XmlNodeList nodeList = root.SelectNodes("Form[Name='" + frmName + "']/Controls/Control");
				foreach (XmlNode node in nodeList)
				{
					try
					{
						string name = node.Attributes["n"]?.Value;
						string text = node.Attributes["t"]?.Value;
						string sizeStr = node.Attributes["s"]?.Value;
						if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(text))
						{
							float size = 0f;
							float.TryParse(sizeStr, out size);
							hashText[name.ToLower()] = Tuple.Create(text, size);
						}
					}
					catch
					{
					}
				}
				reader.Close();
				if (frmName == "Form500_Controller")
				{
					hashForm500Text = hashText;
				}
				return hashText;
			}
			catch
			{
				return null;
			}
		}

		public static void LoadLanguage(Form form)
		{
			string language = GetDefaultLanguage();
			Dictionary<string, Tuple<string, float>> hashText = ReadXMLText(form.Name, language);
			if (hashText == null)
			{
				return;
			}
			Control.ControlCollection sonControls = form.Controls;
			try
			{
				foreach (Control control in sonControls)
				{
					if (control.GetType() == typeof(Panel))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(GroupBox))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(TabControl))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(TabPage))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					string NameCtrl = control.Name.ToLower();
					if (hashText.ContainsKey(NameCtrl))
					{
						Tuple<string, float> tuple = hashText[NameCtrl];
						control.Text = tuple.Item1;
						if (tuple.Item2 != 0f)
						{
							control.Font = new Font(control.Font.FontFamily, tuple.Item2, control.Font.Style);
						}
					}
				}
				string ctrlName = form.Name.ToLower();
				if (hashText.ContainsKey(ctrlName))
				{
					Tuple<string, float> tuple2 = hashText[ctrlName];
					form.Text = tuple2.Item1;
					if (tuple2.Item2 != 0f)
					{
						form.Font = new Font(form.Font.FontFamily, tuple2.Item2, form.Font.Style);
					}
				}
			}
			catch
			{
			}
		}

		public static void LoadLanguage(Form form, string Name)
		{
			string language = GetDefaultLanguage();
			Dictionary<string, Tuple<string, float>> hashText = ReadXMLText(Name, language);
			if (hashText == null)
			{
				return;
			}
			Control.ControlCollection sonControls = form.Controls;
			try
			{
				foreach (Control control in sonControls)
				{
					if (control.GetType() == typeof(Panel))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(GroupBox))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(TabControl))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(TabPage))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					string NameCtrl = control.Name.ToLower();
					if (hashText.ContainsKey(NameCtrl))
					{
						Tuple<string, float> tuple = hashText[NameCtrl];
						control.Text = tuple.Item1;
						if (tuple.Item2 != 0f)
						{
							control.Font = new Font(control.Font.FontFamily, tuple.Item2, control.Font.Style);
						}
					}
				}
				string ctrlName = form.Name.ToLower();
				if (hashText.ContainsKey(ctrlName))
				{
					Tuple<string, float> tuple2 = hashText[ctrlName];
					form.Text = tuple2.Item1;
					if (tuple2.Item2 != 0f)
					{
						form.Font = new Font(form.Font.FontFamily, tuple2.Item2, form.Font.Style);
					}
				}
			}
			catch
			{
			}
		}

		public static string GetStr(Form form, string SearchStr)
		{
			return GetStr(form.Name, SearchStr);
		}

		public static string GetStr(string Name, string SearchStr)
		{
			string language = GetDefaultLanguage();
			string FindStr = "";
			Dictionary<string, Tuple<string, float>> hashText = ReadXMLText(Name, language);
			if (hashText == null)
			{
				return FindStr;
			}
			try
			{
				string ctrlName = SearchStr.ToLower();
				if (hashText.ContainsKey(ctrlName))
				{
					Tuple<string, float> tuple = hashText[ctrlName];
					FindStr = tuple.Item1;
				}
			}
			catch
			{
			}
			return FindStr;
		}

		private static void GetSetSubControls(Control.ControlCollection controls, Dictionary<string, Tuple<string, float>> hashText)
		{
			try
			{
				foreach (Control control in controls)
				{
					if (control.GetType() == typeof(Panel))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(GroupBox))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(TabControl))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					else if (control.GetType() == typeof(TabPage))
					{
						GetSetSubControls(control.Controls, hashText);
					}
					string NameCtrl = control.Name.ToLower();
					if (hashText.ContainsKey(NameCtrl))
					{
						Tuple<string, float> tuple = hashText[NameCtrl];
						control.Text = tuple.Item1;
						if (tuple.Item2 != 0f)
						{
							control.Font = new Font(control.Font.FontFamily, tuple.Item2, control.Font.Style);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}
	}
}
