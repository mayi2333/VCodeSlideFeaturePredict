using Microsoft.ML;
using SSB.AI.VerificationCode.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSB.AI.VerificationCode.Training
{
    /// <summary>
    /// 训练模型
    /// </summary>
    public class Training
    {
        private MLContext mlContext;
        private IDataView originalFullData;
        private IDataView trainData;
        private IDataView testData;
        private ITransformer model;
        private int numberOfLeaves = 20;
        private int numberOfTrees = 100;
        private int minimumExampleCountPerLeaf = 10;
        private double learningRate = 0.2;

        public Training()
        {
            mlContext = new MLContext(seed: 1);
        }
        public Training(int NumberOfLeaves, int NumberOfTrees, int MinimumExampleCountPerLeaf, double LearningRate)
        {
            numberOfLeaves = NumberOfLeaves;
            numberOfTrees = NumberOfTrees;
            minimumExampleCountPerLeaf = MinimumExampleCountPerLeaf;
            learningRate = LearningRate;
            mlContext = new MLContext(seed: 1);
        }
        public void Star()
        {
            //string AssetsRelativePath = @"../../../assets";
            string assetsPath = GetAbsolutePath("");
            //string zipDataSet = Path.Combine(assetsPath, "input", "creditcardfraud-dataset.zip");
            //string fullDataSetFilePath = Path.Combine(assetsPath, "样本数据.csv");
            string fullDataSetFilePath = @"C:\样本数据1.csv";
            string modelFilePath = Path.Combine(assetsPath, "output", "model" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip");

            //准备数据
            PrepDatasets(fullDataSetFilePath);
            //训练模型
            TrainModel();
            //评估模型
            EvaluateModel();
            //保存模型
            SaveModel(modelFilePath);
        }
        /// <summary>
        /// 准备数据
        /// </summary>
        /// <param name="mlContext">ML环境</param>
        /// <param name="fullDataSetFilePath">完整数据集</param>
        public void PrepDatasets(string fullDataSetFilePath)
        {
            //加载原始单个数据集
            Console.WriteLine("加载数据集...");
            originalFullData = mlContext.Data.LoadFromTextFile<Observation>(fullDataSetFilePath, separatorChar: ',', hasHeader: true);

            // 将数据按照80:20比例分解为训练集和测试集，进行训练和测试。
            Console.WriteLine("将数据集分解为训练集和测试集...");
            var trainTestData = mlContext.Data.TrainTestSplit(originalFullData, testFraction: 0.2, seed: 1);
            trainData = trainTestData.TrainSet;
            testData = trainTestData.TestSet;

            //检查测试数据视图，以确保测试集中存在正确和错误的观察结果。
            InspectData(4);

            // save train split
            //using (var fileStream = File.Create(trainDataSetFilePath))
            //{
            //    mlContext.Data.SaveAsText(trainData, fileStream, separatorChar: ',', headerRow: true, schema: true);
            //}

            //// save test split 
            //using (var fileStream = File.Create(testDataSetFilePath))
            //{
            //    mlContext.Data.SaveAsText(testData, fileStream, separatorChar: ',', headerRow: true, schema: true);
            //}
        }
        /// <summary>
        /// 训练模型
        /// </summary>
        /// <returns></returns>
        public void TrainModel()
        {
            //Get all the feature column names (All except the Label and the IdPreservationColumn)
            string[] featureColumnNames = trainData.Schema.AsQueryable()
                .Select(column => column.Name)                               // 获取所有列名
                .Where(name => name != nameof(Observation.label)) // 不包括标签列
                //.Where(name => name != "IdPreservationColumn")               // Do not include the IdPreservationColumn/StratificationColumn
                .Where(name => name != "SamplingKeyColumn")                               // Do not include the Time column. Not needed as feature column
                .ToArray();

            // 创建数据处理管道
            Console.WriteLine("创建数据处理管道...");
            IEstimator<ITransformer> dataProcessPipeline = mlContext.Transforms.Concatenate("Features", featureColumnNames)
                                            //.Append(mlContext.Transforms.DropColumns(new string[] { "total" }))
                                            .Append(mlContext.Transforms.NormalizeMeanVariance(inputColumnName: "Features",
                                                                                 outputColumnName: "FeaturesNormalizedByMeanVar"));

            // (OPTIONAL) Peek data (such as 2 records) in training DataView after applying the ProcessPipeline's transformations into "Features" 
            //PeekDataViewInConsole(trainData, dataProcessPipeline, 2);
            //PeekVectorColumnDataInConsole(mlContext, "Features", trainDataView, dataProcessPipeline, 1);

            // 设置训练算法
            Console.WriteLine("设置训练算法...");
            var trainer = mlContext.BinaryClassification.Trainers.FastTree(labelColumnName: nameof(Observation.label),
                                                                                                featureColumnName: "FeaturesNormalizedByMeanVar",
                                                                                                numberOfLeaves: numberOfLeaves,
                                                                                                numberOfTrees: numberOfTrees,
                                                                                                minimumExampleCountPerLeaf: minimumExampleCountPerLeaf,
                                                                                                learningRate: learningRate);

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            //开始训练模型
            Console.WriteLine("开始训练模型...");
            var m = trainingPipeline.Fit(trainData);

            // Append feature contribution calculator in the pipeline. This will be used
            // at prediction time for explainability. 
            var fccPipeline = m.Append(mlContext.Transforms
                .CalculateFeatureContribution(m.LastTransformer)
                .Fit(dataProcessPipeline.Fit(trainData).Transform(trainData)));
            model = m;
        }
        /// <summary>
        /// 评估模型质量
        /// </summary>
        public void EvaluateModel()
        {
            // Evaluate the model and show accuracy stats
            Console.WriteLine("测试模型质量...");
            var predictions = model.Transform(testData);

            var metrics = mlContext.BinaryClassification.Evaluate(data: predictions,
                                                                  labelColumnName: nameof(Observation.label),
                                                                  scoreColumnName: "Score");
            WriteMessage($"************************************************************");
            WriteMessage($"*       Accuracy: {metrics.Accuracy:P2}");
            WriteMessage($"*       AUC:      {metrics.AreaUnderRocCurve:P2}");
            WriteMessage($"*       F1Score:  {metrics.F1Score:P2}");
            WriteMessage($"*       Entropy:  {metrics.Entropy:P2}");
            WriteMessage($"*    numberOfLeaves:" + numberOfLeaves + ", numberOfTrees:" + numberOfTrees + " ");
            WriteMessage($"*    minimumExampleCountPerLeaf:" + minimumExampleCountPerLeaf + ", learningRate:" + learningRate + " ");
            WriteMessage($"************************************************************");
            //PrintBinaryClassificationMetrics(metrics);
        }
        /// <summary>
        /// 输出指定信息到文本文件
        /// </summary>
        /// <param name="path">文本文件路径</param>
        /// <param name="msg">输出信息</param>
        public void WriteMessage(string msg)
        {
            var path = Path.Combine(GetAbsolutePath(""), "output", "modellog.txt");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(msg);
                    sw.Flush();
                    Console.WriteLine(msg);
                }
            }
        }
        /// <summary>
        /// 保存模型
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model">训练好的模型</param>
        /// <param name="modelFilePath">保存路径</param>
        public void SaveModel(string modelFilePath)
        {
            mlContext.Model.Save(model, trainData.Schema, modelFilePath);
            WriteMessage("保存模型到 " + modelFilePath);
        }
        /// <summary>
        /// 显示几条数据
        /// </summary>
        /// <param name="records"></param>
        public void InspectData(int records)
        {
            //We want to make sure we have True and False observations

            Console.WriteLine("显示四个正常数据(true)");
            ShowObservationsFilteredByLabel(trainData, label: true, count: records);

            Console.WriteLine("显示四个非正常数据(false)");
            ShowObservationsFilteredByLabel(trainData, label: false, count: records);
        }
        public void ShowObservationsFilteredByLabel(IDataView dataView, bool label = true, int count = 2)
        {
            // Convert to an enumerable of user-defined type. 
            var data = mlContext.Data.CreateEnumerable<Observation>(dataView, reuseRowObject: false)
                                            .Where(x => x.label == label)
                                            // Take a couple values as an array.
                                            .Take(count)
                                            .ToList();

            // print to console
            data.ForEach(row => { Console.WriteLine("...meanv:" + row.meanv + "...meana:" + row.meana + "... ,label:" + row.label); });
        }
        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Training).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }
    }
}
