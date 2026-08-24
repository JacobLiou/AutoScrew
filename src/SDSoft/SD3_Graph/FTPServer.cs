using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SD3_Graph
{
	public class FTPServer
	{
		private readonly string _username = "BinQS";

		private readonly string _password = "2308";

		private TcpListener _dataListener;

		private IPEndPoint _dataEndPoint;

		private GlobalVar GB = null;

		public FTPServer(GlobalVar GB)
		{
			this.GB = GB;
			if (!Directory.Exists(GB.UISys.FTPSavePath))
			{
				Directory.CreateDirectory(GB.UISys.FTPSavePath);
			}
			GB.FTPServerListener = new TcpListener(IPAddress.Any, GB.UISys.passivePort);
		}

		public void Start()
		{
			try
			{
				GB.FTPServerListener.Start();
				Console.WriteLine("FTP Server 已啟動，等待連線中...");
				while (GB.FTPServerFlag)
				{
					TcpClient client = GB.FTPServerListener.AcceptTcpClient();
					client.Client.SendBufferSize = 131072;
					client.Client.ReceiveBufferSize = 131072;
					Task.Run(() => HandleClient(client));
				}
			}
			catch
			{
			}
		}

		public void Stop()
		{
			GB.FTPServerFlag = false;
			GB.FTPServerListener.Stop();
		}

		private async Task HandleClient(TcpClient client)
		{
			using (NetworkStream networkStream = client.GetStream())
			{
				using (StreamReader reader = new StreamReader(networkStream, Encoding.ASCII))
				{
					using (StreamWriter writer = new StreamWriter(networkStream, Encoding.ASCII)
					{
						AutoFlush = true
					})
					{
						await writer.WriteLineAsync("220 ScrewDriver FTP Server").ConfigureAwait(false);
						bool isAuthenticated = false;
						while (client.Connected)
						{
							try
							{
								string command = await reader.ReadLineAsync().ConfigureAwait(false);
								if (command == null)
								{
									break;
								}
								string[] commandParts = command.Split(' ');
								string cmd = commandParts[0].ToUpperInvariant();
								string argument = ((commandParts.Length > 1) ? commandParts[1] : null);
								switch (cmd)
								{
								case "USER":
									if (argument == _username)
									{
										await writer.WriteLineAsync("331 Username ok, need password.").ConfigureAwait(false);
									}
									else
									{
										await writer.WriteLineAsync("530 Invalid username.").ConfigureAwait(false);
									}
									break;
								case "PASS":
									if (argument == _password)
									{
										isAuthenticated = true;
										await writer.WriteLineAsync("230 Login successful.").ConfigureAwait(false);
									}
									else
									{
										await writer.WriteLineAsync("530 Invalid password.").ConfigureAwait(false);
									}
									break;
								case "PWD":
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
									}
									else
									{
										await writer.WriteLineAsync("257 \"" + GB.UISys.FTPSavePath + "\" is the current directory.").ConfigureAwait(false);
									}
									break;
								case "TYPE":
									if (argument == "I")
									{
										await writer.WriteLineAsync("200 Type set to I (binary).").ConfigureAwait(false);
									}
									else if (argument == "A")
									{
										await writer.WriteLineAsync("200 Type set to A (ASCII).").ConfigureAwait(false);
									}
									else
									{
										await writer.WriteLineAsync("501 Syntax error in parameters.").ConfigureAwait(false);
									}
									break;
								case "PASV":
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
									}
									else
									{
										await EnterPassiveMode(writer);
									}
									break;
								case "STOR":
								{
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
										break;
									}
									if (string.IsNullOrEmpty(argument))
									{
										await writer.WriteLineAsync("501 Missing filename.").ConfigureAwait(false);
										break;
									}
									string filePath = Path.Combine(GB.UISys.FTPSavePath, argument);
									await writer.WriteLineAsync("150 Opening data connection.").ConfigureAwait(false);
									await SaveFileAsync(networkStream, filePath);
									await writer.WriteLineAsync("226 Transfer complete.").ConfigureAwait(false);
									break;
								}
								case "NLST":
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
									}
									else
									{
										await ListFileNames(writer);
									}
									break;
								case "LIST":
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
									}
									else
									{
										await ListFiles(writer);
									}
									break;
								case "AUTH":
									if (argument?.ToUpperInvariant() == "TLS")
									{
										await writer.WriteLineAsync("502 Command not implemented.").ConfigureAwait(false);
									}
									else
									{
										await writer.WriteLineAsync("502 Command not implemented.").ConfigureAwait(false);
									}
									break;
								case "SIZE":
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
									}
									else
									{
										await GetFileSize(writer, argument);
									}
									break;
								case "MDTM":
								{
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
										break;
									}
									if (string.IsNullOrEmpty(argument))
									{
										await writer.WriteLineAsync("501 Missing filename.").ConfigureAwait(false);
										break;
									}
									string mdtmFilePath = Path.Combine(GB.UISys.FTPSavePath, argument);
									if (File.Exists(mdtmFilePath))
									{
										string formattedDate = File.GetLastWriteTime(mdtmFilePath).ToString("yyyyMMddHHmmss");
										await writer.WriteLineAsync("213 " + formattedDate).ConfigureAwait(false);
									}
									else
									{
										await writer.WriteLineAsync("550 File not found.").ConfigureAwait(false);
									}
									break;
								}
								case "ALLO":
								{
									long size;
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
									}
									else if (string.IsNullOrEmpty(argument) || !long.TryParse(argument, out size))
									{
										await writer.WriteLineAsync("501 Invalid parameter.").ConfigureAwait(false);
									}
									else
									{
										await writer.WriteLineAsync($"202 Allocated {size} bytes.").ConfigureAwait(false);
									}
									break;
								}
								case "DELE":
								{
									if (!isAuthenticated)
									{
										await writer.WriteLineAsync("530 Not logged in.").ConfigureAwait(false);
										break;
									}
									if (string.IsNullOrEmpty(argument))
									{
										await writer.WriteLineAsync("501 Missing filename.").ConfigureAwait(false);
										break;
									}
									string fileToDelete = Path.Combine(GB.UISys.FTPSavePath, argument);
									if (File.Exists(fileToDelete))
									{
										try
										{
											File.Delete(fileToDelete);
											await writer.WriteLineAsync("250 File deleted successfully.").ConfigureAwait(false);
										}
										catch (Exception ex)
										{
											await writer.WriteLineAsync("550 File could not be deleted: " + ex.Message).ConfigureAwait(false);
										}
									}
									else
									{
										await writer.WriteLineAsync("550 File not found.").ConfigureAwait(false);
									}
									break;
								}
								case "SITE":
									if (argument != null && argument.ToUpperInvariant() == "UTIME")
									{
										string serverTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
										await writer.WriteLineAsync("213 " + serverTime);
									}
									else
									{
										await writer.WriteLineAsync("502 Command not implemented.").ConfigureAwait(false);
									}
									break;
								case "QUIT":
									await writer.WriteLineAsync("221 Goodbye.").ConfigureAwait(false);
									client.Close();
									return;
								case "FEAT":
									await writer.WriteLineAsync("211-Extensions supported:").ConfigureAwait(false);
									await writer.WriteLineAsync(" USER").ConfigureAwait(false);
									await writer.WriteLineAsync(" PASS").ConfigureAwait(false);
									await writer.WriteLineAsync(" PWD").ConfigureAwait(false);
									await writer.WriteLineAsync(" TYPE").ConfigureAwait(false);
									await writer.WriteLineAsync(" PASV").ConfigureAwait(false);
									await writer.WriteLineAsync(" STOR").ConfigureAwait(false);
									await writer.WriteLineAsync(" NLST").ConfigureAwait(false);
									await writer.WriteLineAsync(" LIST").ConfigureAwait(false);
									await writer.WriteLineAsync(" AUTH TLS").ConfigureAwait(false);
									await writer.WriteLineAsync(" SIZE").ConfigureAwait(false);
									await writer.WriteLineAsync(" MDTM").ConfigureAwait(false);
									await writer.WriteLineAsync(" ALLO").ConfigureAwait(false);
									await writer.WriteLineAsync(" DELE").ConfigureAwait(false);
									await writer.WriteLineAsync(" SITE").ConfigureAwait(false);
									await writer.WriteLineAsync(" QUIT").ConfigureAwait(false);
									await writer.WriteLineAsync("211 End").ConfigureAwait(false);
									break;
								default:
									await writer.WriteLineAsync("502 Command not implemented.").ConfigureAwait(false);
									break;
								}
								continue;
							}
							catch (IOException ex2)
							{
								Console.WriteLine("網路錯誤：" + ex2.Message);
								if (!(await TryReconnect(client)))
								{
									Console.WriteLine("重連失敗，關閉連線");
									break;
								}
								Console.WriteLine("重新連接成功，繼續操作");
								continue;
							}
							catch (ObjectDisposedException ex3)
							{
								Console.WriteLine("網路錯誤：" + ex3.Message);
								if (!(await TryReconnect(client)))
								{
									Console.WriteLine("重連失敗，關閉連線");
									break;
								}
								Console.WriteLine("重新連接成功，繼續操作");
								continue;
							}
						}
					}
				}
			}
		}

		private async Task<bool> TryReconnect(TcpClient tcpClient)
		{
			int retries = 50;
			for (int attempt = 0; attempt < retries; attempt++)
			{
				try
				{
					tcpClient.Close();
					tcpClient.Connect("127.0.0.1", GB.UISys.passivePort);
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"重連失敗 (嘗試 {attempt + 1}/{retries}): {ex.Message}");
					await Task.Delay(1000);
				}
			}
			return false;
		}

		private async Task GetFileSize(StreamWriter writer, string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				await writer.WriteLineAsync("501 Syntax error in parameters or arguments.").ConfigureAwait(false);
				return;
			}
			string filePath = Path.Combine(GB.UISys.FTPSavePath, fileName);
			if (File.Exists(filePath))
			{
				FileInfo fileInfo = new FileInfo(filePath);
				await writer.WriteLineAsync($"213 {fileInfo.Length}").ConfigureAwait(false);
			}
			else
			{
				await writer.WriteLineAsync("550 File not found.").ConfigureAwait(false);
			}
		}

		private async Task ListFileNames(StreamWriter writer)
		{
			await writer.WriteLineAsync("150 Here comes the name listing.").ConfigureAwait(false);
			using (TcpClient dataClient = _dataListener.AcceptTcpClient())
			{
				using (NetworkStream dataStream = dataClient.GetStream())
				{
					using (StreamWriter dataWriter = new StreamWriter(dataStream, Encoding.ASCII))
					{
						string[] fileSystemEntries = Directory.GetFileSystemEntries(GB.UISys.FTPSavePath);
						foreach (string entry in fileSystemEntries)
						{
							string fileName = Path.GetFileName(entry);
							dataWriter.WriteLine(fileName);
						}
						dataWriter.Flush();
					}
				}
			}
			await writer.WriteLineAsync("226 Name listing send okay.").ConfigureAwait(false);
		}

		private async Task ListFiles(StreamWriter writer)
		{
			await writer.WriteLineAsync("150 Here comes the directory listing.").ConfigureAwait(false);
			using (TcpClient dataClient = _dataListener.AcceptTcpClient())
			{
				using (NetworkStream dataStream = dataClient.GetStream())
				{
					using (StreamWriter dataWriter = new StreamWriter(dataStream, Encoding.ASCII))
					{
						string[] fileSystemEntries = Directory.GetFileSystemEntries(GB.UISys.FTPSavePath);
						foreach (string entry in fileSystemEntries)
						{
							FileAttributes attributes = File.GetAttributes(entry);
							string type = (attributes.HasFlag(FileAttributes.Directory) ? "dir" : "file");
							string fileName = Path.GetFileName(entry);
							dataWriter.WriteLine(type + " " + fileName);
						}
						dataWriter.Flush();
					}
				}
			}
			await writer.WriteLineAsync("226 Directory send okay.").ConfigureAwait(false);
		}

		private async Task EnterPassiveMode(StreamWriter writer)
		{
			_dataListener = new TcpListener(IPAddress.Any, 0);
			_dataListener.Start();
			_dataEndPoint = (IPEndPoint)_dataListener.LocalEndpoint;
			IPAddress ipAddress = ((IPEndPoint)GB.FTPServerListener.LocalEndpoint).Address;
			int port = _dataEndPoint.Port;
			string[] ipParts = ipAddress.ToString().Split('.');
			int portHigh = port / 256;
			await writer.WriteLineAsync(string.Format(arg2: port % 256, format: "227 Entering Passive Mode ({0},{1},{2}).", arg0: string.Join(",", ipParts), arg1: portHigh)).ConfigureAwait(false);
		}

		private async Task SaveFileAsync(NetworkStream networkStream, string filePath)
		{
			try
			{
				using (TcpClient dataClient = await _dataListener.AcceptTcpClientAsync())
				{
					using (NetworkStream dataStream = dataClient.GetStream())
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							await dataStream.CopyToAsync(memoryStream);
							File.WriteAllBytes(filePath, memoryStream.ToArray());
						}
					}
				}
			}
			catch (Exception ex)
			{
				Exception ex2 = ex;
				Console.WriteLine("Error while saving the file: " + ex2.Message);
				throw;
			}
		}
	}
}
