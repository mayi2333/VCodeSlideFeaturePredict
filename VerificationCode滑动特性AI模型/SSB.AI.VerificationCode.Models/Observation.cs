using Microsoft.ML.Data;
using System;

namespace SSB.AI.VerificationCode.Models
{
    public class Observation
    {
        [LoadColumn(0)]
        public float sumtime;
        [LoadColumn(1)]
        public float abscissa;
        [LoadColumn(2)]
        public float total;
        [LoadColumn(3)]
        public float meanv;
        [LoadColumn(4)]
        public float meanv1;
        [LoadColumn(5)]
        public float meanv2;
        [LoadColumn(6)]
        public float meanv3;
        [LoadColumn(7)]
        public float meana;
        [LoadColumn(8)]
        public float meana1;
        [LoadColumn(9)]
        public float meana2;
        [LoadColumn(10)]
        public float meana3;
        [LoadColumn(11)]
        public float standardv;
        [LoadColumn(12)]
        public float standarda;
        [LoadColumn(13)]
        public bool label;
    }
}
