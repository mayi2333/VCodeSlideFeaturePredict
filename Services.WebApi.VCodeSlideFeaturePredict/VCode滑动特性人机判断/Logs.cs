using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VCode滑动特性人机判断
{
    public class Logs
    {
        /// <summary>
        /// 输出指定信息到文本文件
        /// </summary>
        /// <param name="path">文本文件路径</param>
        /// <param name="msg">输出信息</param>
        public static void WriteMessage(string msg, string outpath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Logs).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string path = Path.Combine(assemblyFolderPath, outpath);
            if (!Directory.Exists(path))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, $"{DateTime.Now.ToString("yyyyMMdd")}.txt");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(msg);
                    sw.Flush();
                    //Console.WriteLine(msg);
                }
            }
        }
    }
}
