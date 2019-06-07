using SSB.AI.VerificationCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VCode滑动特性人机判断
{
    public class ResultBase
    {
        public int code;
        public string info;
    }
    public class VCodePredictModel:ResultBase
    {
        public bool predic;
    }
    public class VCodePredictFromModel
    {
        public float sumtime;
        public float abscissa;
        public float total;
        public float meanv;
        public float meanv1;
        public float meanv2;
        public float meanv3;
        public float meana;
        public float meana1;
        public float meana2;
        public float meana3;
        public float standardv;
        public float standarda;
        public Observation GetObservationModel()
        {
            return new Observation()
            {
                sumtime = sumtime,
                abscissa = abscissa,
                total = total,
                meanv = meanv,
                meanv1 = meanv1,
                meanv2 = meanv2,
                meanv3 = meanv3,
                meana = meana,
                meana1 = meana1,
                meana2 = meana2,
                meana3 = meana3,
                standardv = standardv,
                standarda = standarda,
            };
        }
        /// <summary>
        /// 获取用于测试的数据
        /// </summary>
        /// <returns></returns>
        public string GetTestData(bool label)
        {
            string testdatastr = $"{sumtime},{abscissa},{total},{meanv},{meanv1},{meanv2},{meanv3},{meana},{meana1},{meana2},{meana3},{standardv},{standarda},{label}";
            return testdatastr;
        }
    }
}
