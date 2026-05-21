using ICSharpCode.SharpZipLib.Zip;
using System.Collections;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class ZipHelper
    {
        /// <summary>
        /// 压缩文件中的文件，可设置密码
        /// </summary>
        /// <param name="inputFolderPath">输入的文件夹</param>
        /// <param name="outputPathAndFile">输出的压缩文件全名</param>
        /// <param name="password">压缩密码</param>
        public static void ZipFiles(string inputFolderPath, string outputPathAndFile, string password)
        {
            ArrayList ar = GenerateFileList(inputFolderPath);
            int TrimLength = Directory.GetParent(inputFolderPath).ToString().Length;
            // find number of chars to remove   // from orginal file path
            TrimLength += 1; //remove '\'
            FileStream ostream;
            byte[] obuffer;
            string outPath = inputFolderPath + @"\" + outputPathAndFile;
            ZipOutputStream oZipStream = new ZipOutputStream(File.Create(outPath));
            if (!string.IsNullOrEmpty(password))
            {
                oZipStream.Password = password;
            }
            oZipStream.SetLevel(9); // 设置最大压缩率

            ZipEntry oZipEntry;
            foreach (string Fil in ar)
            {
                oZipEntry = new ZipEntry(Fil.Remove(0, TrimLength));
                oZipStream.PutNextEntry(oZipEntry);

                if (!Fil.EndsWith(@"/")) // 如果文件以 '/' 结束，则是目录
                {
                    ostream = File.OpenRead(Fil);
                    obuffer = new byte[ostream.Length];
                    ostream.Read(obuffer, 0, obuffer.Length);
                    oZipStream.Write(obuffer, 0, obuffer.Length);
                }
            }
            oZipStream.Finish();
            oZipStream.Close();
        }

        /// <summary>
        /// 根据文件夹生成文件列表
        /// </summary>
        /// <param name="Dir"></param>
        /// <returns></returns>
        private static ArrayList GenerateFileList(string Dir)
        {
            ArrayList fils = new ArrayList();
            bool Empty = true;
            foreach (string file in Directory.GetFiles(Dir))
            {
                fils.Add(file);
                Empty = false;
            }

            if (Empty)
            {
                //加入完全为空的目录
                if (Directory.GetDirectories(Dir).Length == 0)
                {
                    fils.Add(Dir + @"/");
                }
            }

            foreach (string dirs in Directory.GetDirectories(Dir)) // 递归目录
            {
                foreach (object obj in GenerateFileList(dirs))
                {
                    fils.Add(obj);
                }
            }
            return fils;
        }

        /// <summary>
        /// 解压文件到指定的目录，可设置密码、删除原文件等
        /// </summary>
        /// <param name="zipPathAndFile">压缩文件全名</param>
        /// <param name="outputFolder">解压输出文件目录</param>
        /// <param name="password">解压密码</param>
        /// <param name="deleteZipFile">是否删除原文件（压缩文件）</param>
        public static void UnZipFiles(string zipPathAndFile, string outputFolder, string password, bool deleteZipFile)
        {
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipPathAndFile)))
            {
                if (password != null && password != string.Empty)
                {
                    s.Password = password;
                }

                ZipEntry theEntry;
                string tmpEntry = string.Empty;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    #region 遍历每个Entry对象进行解压处理

                    string directoryName = outputFolder;
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName != "")
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != string.Empty)
                    {
                        if (theEntry.Name.IndexOf(".ini") < 0)
                        {
                            string fullPath = directoryName + "\\" + theEntry.Name;
                            fullPath = fullPath.Replace("\\ ", "\\");
                            string fullDirPath = Path.GetDirectoryName(fullPath);
                            if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
                            using (FileStream streamWriter = File.Create(fullPath))
                            {
                                #region 写入文件流

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                #endregion 写入文件流
                            }
                        }
                    }

                    #endregion 遍历每个Entry对象进行解压处理
                }
            }

            if (deleteZipFile)
            {
                File.Delete(zipPathAndFile);
            }
        }

        /// <summary>
        /// 压缩一个文件夹到同名的ZIP文件
        /// </summary>
        public static async Task<string> ZipDirectory(string rootPath, DirectoryInfo folderDir)
        {
            var result = await Task.Run(() =>
            {
                string zipPath = Path.Combine(rootPath, $"{folderDir.Name}.zip");
                ZipFile.Create(zipPath);
                return zipPath;
            });

            return result;
        }
    }
}