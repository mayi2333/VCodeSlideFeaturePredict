using Microsoft.ML;
using SSB.AI.VerificationCode.Prediction;
using SSB.AI.VerificationCode.Training;
using System;
using System.IO;

namespace VerificationCode滑动特性AI模型
{
    class Program
    {
        static void Main(string[] args)
        {
            new Prediction(@"C:\model.zip", @"C:\testdata.txt").RunMultiplePredictions(100);
        }
        /// <summary>
        /// 开始训练模型
        /// </summary>
        static void TrainingStar()
        {
            Console.WriteLine("按回车键开始训练模型");
            ConsoleKeyInfo input = Console.ReadKey(true);
            while (input.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("请输入参数");
                var s = Console.ReadLine().Trim();
                if (string.IsNullOrEmpty(s))
                {
                    new Training().Star();
                }
                else
                {
                    var parameter = s.Split(",");
                    int NumberOfLeaves = Int32.Parse(parameter[0]);
                    int NumberOfTrees = Int32.Parse(parameter[1]);
                    int MinimumExampleCountPerLeaf = Int32.Parse(parameter[2]);
                    double LearningRate = Double.Parse(parameter[3]);
                    new Training(
                        NumberOfLeaves: NumberOfLeaves,
                        NumberOfTrees: NumberOfTrees,
                        MinimumExampleCountPerLeaf: MinimumExampleCountPerLeaf,
                        LearningRate: LearningRate
                        ).Star();
                }
                Console.WriteLine("模型训练完毕...");
                Console.WriteLine("按回车键重新训练模型");
                input = Console.ReadKey(true);
            }
        }
    }
}
