using System;
using System.IO;

namespace 训练数据生成
{
    class Program
    {
        static void Main(string[] args)
        {
            匀加速();
            匀速数据();
        }
        public static void 匀加速()
        {
            int _MinRangeX = 30;
            int _MaxRangeX = 240;
            Random rd = new Random();
            for (int n = 0; n < 50; n++)
            {
                Console.WriteLine("正在生成第 " + (n + 1) + "  行...");
                System.Threading.Thread.Sleep(10);
                int _PositionX = rd.Next(_MinRangeX, _MaxRangeX);
                long time = ConvertDateTimeInt(DateTime.Now);
                int sum = rd.Next(1, 4);
                string datalist = string.Empty;
                int sumflag = 0;
                for (int i = rd.Next(1, 5); i <= _PositionX; i++)
                {
                    datalist += i + "," + time + "|";
                    i += sum;
                    if (sumflag > 2)
                    {
                        sumflag = 0;
                        sum++;
                    }
                    sumflag++;
                    time += rd.Next(45, 95);
                }
                time += rd.Next(25, 65);
                datalist += _PositionX + "," + time;
                SlideFeature(datalist, _PositionX, "匀加速");
            }
        }
        public static void 匀速数据()
        {
            int _MinRangeX = 30;
            int _MaxRangeX = 240;
            Random rd = new Random();
            for (int n = 0; n < 50; n++)
            {
                Console.WriteLine("正在生成第 " + (n + 1) + "  行...");
                System.Threading.Thread.Sleep(10);
                int _PositionX = rd.Next(_MinRangeX, _MaxRangeX);
                long time = ConvertDateTimeInt(DateTime.Now);
                int sum = rd.Next(1, 4);
                string datalist = string.Empty;
                for (int i = rd.Next(1, 5); i <= _PositionX; i++)
                {
                    datalist += i + "," + time + "|";
                    i += sum;
                    time += rd.Next(45, 95);
                }
                time += rd.Next(25, 65);
                datalist += _PositionX + "," + time;
                SlideFeature(datalist, _PositionX, "匀速");
            }
        }
        /// <summary>
        /// 滑动特性
        /// </summary>
        public static void SlideFeature(string as_data, int _PositionX, string name)
        {
            if (string.IsNullOrEmpty(as_data))
                return;
            string[] _datalist = as_data.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (_datalist.Length < 10)
                return;
            //__array二维数组共两列 第一列为x轴坐标值 第二列为时间
            long[,] __array = new long[_datalist.Length, 2];
            #region 获取__array
            int row = 0;
            foreach (string str in _datalist)
            {
                string[] strlist = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (strlist.Length != 2)
                    return;
                long __coor = 0, __date = 0;
                try { __coor = long.Parse(strlist[0]); __date = long.Parse(strlist[1]); }
                catch { return; }
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        __array[row, i] = __coor;
                    if (i == 1)
                        __array[row, i] = __date;
                }
                row++;
            }
            #endregion
            #region 计算速度 加速度 以及他们的标准差
            //速度 像素/每秒
            double[] __v = new double[_datalist.Length - 1];
            //加速度 像素/每2次方秒
            double[] __a = new double[_datalist.Length - 1];
            //总时间
            int __totaldate = 0;
            for (int i = 0; i < __v.Length; i++)
            {
                //时间差
                int __timeSpan = 0;
                if (__array[i + 1, 1] - __array[i, 1] == 0)
                    __timeSpan = 1;
                else
                    __timeSpan = (GetTime(__array[i + 1, 1].ToString()) - GetTime(__array[i, 1].ToString())).Milliseconds;
                __v[i] = (double)1000 * Math.Abs(__array[i + 1, 0] - __array[i, 0]) / __timeSpan;//有可能移过再一回来 这里只取正值
                __a[i] = (double)1000 * __v[i] / __timeSpan;
                __totaldate += __timeSpan;
            }
            //速度的计算公式：v=S/t
            //加速度计算公式：a=Δv/Δt
            //分析速度与加速度

            //平均速度
            double __mv = 0;
            double __sumv = 0;
            double __s2v = 0;//速度方差
            double __o2v = 0;//速度标准差
            foreach (double a in __v)
            {
                __sumv += a;
            }
            __mv = __sumv / __v.Length;
            __sumv = 0;
            for (int i = 0; i < __v.Length; i++)
            {
                __sumv += Math.Pow(__v[i] - __mv, 2);
            }
            __s2v = __sumv / __v.Length;
            __o2v = Math.Sqrt(__s2v);

            //平均加速度
            double __ma = 0;
            double __suma = 0;
            double __s2a = 0;//加速度方差
            double __o2a = 0;//加速度标准差
            foreach (double a in __a)
            {
                __suma += a;
            }
            __ma = __suma / __a.Length;
            __suma = 0;
            for (int i = 0; i < __a.Length; i++)
            {
                __suma += Math.Pow(__a[i] - __ma, 2);
            }
            __s2a = __suma / __v.Length;
            __o2a = Math.Sqrt(__s2a);

            double threeEqual = __a.Length / 3;
            //将加速度数组分成三等分 求每一份的加速度
            double __ma1 = 0, __ma2 = 0, __ma3 = 0;
            for (int i = 0; i < __a.Length; i++)
            {
                if (i > threeEqual * 2)
                    __ma3 += __a[i];
                else if (i > threeEqual && i < threeEqual * 2)
                    __ma2 += __a[i];
                else
                    __ma1 += __a[i];
            }
            __ma1 = __ma1 / threeEqual;
            __ma2 = __ma2 / threeEqual;
            __ma3 = __ma3 / threeEqual;
            //将速度数组分成三等分 求每一份的速度
            threeEqual = __v.Length / 3;
            double __mv1 = 0, __mv2 = 0, __mv3 = 0;
            for (int i = 0; i < __v.Length; i++)
            {
                if (i > threeEqual * 2)
                    __mv3 += __v[i];
                else if (i > threeEqual && i < threeEqual * 2)
                    __mv2 += __v[i];
                else
                    __mv1 += __v[i];
            }
            __mv1 = __mv1 / threeEqual;
            __mv2 = __mv2 / threeEqual;
            __mv3 = __mv3 / threeEqual;
            #endregion
            WriteMessage(@"c:\" + name + "生成的datalist.txt", as_data + "&" + _PositionX);
            string m_Feature = __totaldate + "," + _PositionX + "," + _datalist.Length + "," +
                __mv.ToString("0.0000000000") + "," + __mv1.ToString("0.0000000000") + "," + __mv2.ToString("0.0000000000") + "," + __mv3.ToString("0.0000000000") + "," +
                __ma.ToString("0.0000000000") + "," + __ma1.ToString("0.0000000000") + "," + __ma2.ToString("0.0000000000") + "," + __ma3.ToString("0.0000000000") + "," +
                __o2v.ToString("0.0000000000") + "," + __o2a.ToString("0.0000000000") + ",0";
            WriteMessage(@"c:\" + name + "生成的的数据.txt", m_Feature);
        }
        /// <summary>
        /// 输出指定信息到文本文件
        /// </summary>
        /// <param name="path">文本文件路径</param>
        /// <param name="msg">输出信息</param>
        public static void WriteMessage(string path, string msg)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(msg);
                    sw.Flush();
                }
            }
        }
        /// <summary>
        /// 将c# DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>long</returns>
        public static long ConvertDateTimeInt(System.DateTime time)
        {
            //double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //intResult = (time- startTime).TotalMilliseconds;
            long t = (time.Ticks - startTime.Ticks) / 10000;            //除10000调整为13位
            return t;
        }
        /// <summary>
        /// 时间戳转为C#格式时间
        /// </summary>
        /// <param name="timeStamp">Unix时间戳格式</param>
        /// <returns>C#格式时间</returns>
        public static DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime now = dtStart.Add(toNow);
            return now;
        }
    }
}
