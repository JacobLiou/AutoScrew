using System.Runtime.InteropServices;

namespace ProtocolSimulationTest.Common.Helpers
{
    public static class FileHelper
    {
        public static bool _windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// 遍历一个文件夹及其所有子文件夹，并过滤出具有特定扩展名的文件
        /// </summary>
        /// <param name="root"></param>
        /// <param name="extension"></param>
        public static void TraverseDirectory(string root, string extension)
        {
            var stack = new Stack<string>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                var files = Directory.GetFiles(current, "*" + extension);

                foreach (var file in files)
                {
                    Console.WriteLine(file);
                }

                var folders = Directory.GetDirectories(current);

                foreach (var folder in folders)
                {
                    stack.Push(folder);
                }
            }
        }

        /// <summary>
        /// 通过迭代器读取txt日志内容
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="seekEnd"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadPageLine(string fullPath, int page, int pageSize, bool seekEnd = false)
        {
            if (page <= 0)
            {
                page = 1;
            }
            fullPath = fullPath.ReplacePath();
            var lines = File.ReadLines(fullPath, Encoding.UTF8);
            if (seekEnd)
            {
                int lineCount = lines.Count();
                int linPageCount = (int)Math.Ceiling(lineCount / (pageSize * 1.00));
                //超过总页数，不处理
                if (page > linPageCount)
                {
                    page = 0;
                    pageSize = 0;
                }
                else if (page == linPageCount)//最后一页，取最后一页剩下所有的行
                {
                    pageSize = lineCount - (page - 1) * pageSize;
                    if (page == 1)
                    {
                        page = 0;
                    }
                    else
                    {
                        page = lines.Count() - page * pageSize;
                    }
                }
                else
                {
                    page = lines.Count() - page * pageSize;
                }
            }
            else
            {
                page = (page - 1) * pageSize;
            }
            lines = lines.Skip(page).Take(pageSize);

            var enumerator = lines.GetEnumerator();
            int count = 1;
            while (enumerator.MoveNext() || count <= pageSize)
            {
                yield return enumerator.Current;
                count++;
            }
            enumerator.Dispose();
        }

        public static string ReadFile(string path)
        {
            path = path.ReplacePath();
            if (!File.Exists(path))
                return "";
            using (StreamReader stream = new StreamReader(path))
            {
                return stream.ReadToEnd(); // 读取文件
            }
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="Path">文件路径</param>
        /// <param name="Strings">文件内容</param>
        public static void WriteFile(string path, string fileName, string content, bool appendToLast = false)
        {
            if (!path.EndsWith("\\"))
            {
                path = path + "\\";
            }
            path = path.ReplacePath();
            if (!Directory.Exists(path))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream stream = File.Open(path + fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] by = Encoding.Default.GetBytes(content);
                if (appendToLast)
                {
                    stream.Position = stream.Length;
                }
                else
                {
                    stream.SetLength(0);
                }
                stream.Write(by, 0, by.Length);
            }
        }

        public static string ReplacePath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            if (_windows)
                return path.Replace("/", "\\");
            return path.Replace("\\", "/");
        }
    }
}