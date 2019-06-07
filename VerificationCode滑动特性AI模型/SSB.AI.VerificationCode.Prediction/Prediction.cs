using Microsoft.ML;
using Microsoft.ML.Data;
using SSB.AI.VerificationCode.Models;
using System;
using System.IO;
using System.Linq;

namespace SSB.AI.VerificationCode.Prediction
{
    public class Prediction
    {
        private readonly string _modelfile;
        private readonly string _dasetFile;

        public Prediction(string modelfile, string dasetFile)
        {
            _modelfile = modelfile ?? throw new ArgumentNullException(nameof(modelfile));
            _dasetFile = dasetFile ?? throw new ArgumentNullException(nameof(dasetFile));
        }
        public Prediction()
        {
        }
        public void RunMultiplePredictions(int numberOfPredictions)
        {

            var mlContext = new MLContext();
            Console.WriteLine($"正在加载数据:");
            //加载数据
            IDataView inputDataForPredictions = mlContext.Data.LoadFromTextFile<Observation>(_dasetFile, separatorChar: ',', hasHeader: false);

            Console.WriteLine($"正在加载模型");
            //加载模型
            ITransformer model = mlContext.Model.Load(_modelfile, out var inputSchema);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<Observation, FraudPrediction>(model);
            Console.WriteLine($"准备测试 {numberOfPredictions} 条数据");

            //mlContext.Data.CreateEnumerable<Observation>(inputDataForPredictions, reuseRowObject: false)
            //            .Where(x => x.label == true)
            //            .Take(numberOfPredictions)
            //            .Select(testData => testData)
            //            .ToList()
            //            .ForEach(testData =>
            //            {
            //                Console.WriteLine($"--- Transaction ---");
            //                predictionEngine.Predict(testData).PrintToConsole();
            //                Console.WriteLine($"-------------------");
            //            });
            int num = 0;
            mlContext.Data.CreateEnumerable<Observation>(inputDataForPredictions, reuseRowObject: false)
                       .Take(numberOfPredictions)
                       .ToList()
                       .ForEach(testData =>
                       {
                           Console.WriteLine($"-------------------");
                           var p = predictionEngine.Predict(testData);
                           if (!p.PredictedLabel)
                           {
                               num++;
                           }
                           Console.WriteLine($"PredictedLabel:{p.PredictedLabel}");
                           Console.WriteLine($"-------------------");
                       });
            Console.WriteLine($"成功预测: {num} 条");
        }
        /// <summary>
        /// 滑动特性识别
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回true表示是人工操作</returns>
        public static bool Predict(Observation data)
        {
            var mlContext = new MLContext();
            FileInfo _dataRoot = new FileInfo(typeof(Prediction).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string _modelfile = Path.Combine(assemblyFolderPath, "verificationcode_ai_model.zip");
            //加载模型
            ITransformer model = mlContext.Model.Load(_modelfile, out var inputSchema);
            //创建预测引擎
            var predictionEngine = mlContext.Model.CreatePredictionEngine<Observation, FraudPrediction>(model);

            return predictionEngine.Predict(data).PredictedLabel;
        }
    }
}

