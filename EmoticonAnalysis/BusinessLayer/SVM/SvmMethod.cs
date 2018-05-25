using libsvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess;
using System.IO;

namespace SVM
{
    public class SvmMethod
    {
        private Dictionary<int, string> _predictionDictionary;
        private static SvmMethod instance;
        private static C_SVC model;
        private static List<string> vocabulary;

        private SvmMethod()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/SentimentAnalysisDataset.csv");
            List<string> x = new List<string>();

            List<double> y = new List<double>();
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                for (int i = 0; i < 500; i++)//5146
                {
                    var lineArr = lines[i].Split(new string[] { ",Sentiment140,", ",Kaggle," }, StringSplitOptions.None);
                    y.Add(double.Parse(lineArr[0].Split(',')[1]));
                    x.Add(lineArr[1].Trim());
                }

            }

            //var dataTable = DataTable.New.ReadCsv(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/spamdata.csv"));
            //List<string> x = dataTable.Rows.Select(row => row["Text"]).ToList();
            //double[] y = dataTable.Rows.Select(row => double.Parse(row["IsSpam"])).ToArray();

            vocabulary = x.SelectMany(GetWords).Distinct().OrderBy(word => word).ToList();

            var problemBuilder = new TextClassificationProblemBuilder();
            var problem = problemBuilder.CreateProblem(x, y.ToArray(), vocabulary.ToList());

            const int C = 1;
            model = new C_SVC(problem, KernelHelper.LinearKernel(), C);

            _predictionDictionary = new Dictionary<int, string> {{ 0, "negative" }, { 1, "positive" } };
        }

        public static SvmMethod Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SvmMethod();
                }
                return instance;
            }
        }

        public int Analyze(string message)
        {

            //var accuracy = model.GetCrossValidationAccuracy(10);
            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("Files/model_{0}_accuracy.model", accuracy));
            //model.Export(path);

            var newX = TextClassificationProblemBuilder.CreateNode(message, vocabulary);

            var predictedY = model.Predict(newX);
            var predictedProb = model.PredictProbabilities(newX);

            //return _predictionDictionary[(int)predictedY];
            return (int)predictedY>0 ? 1 : -1 ;
        }
        
        private IEnumerable<string> GetWords(string x)
        {
            return x.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
