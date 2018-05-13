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
            var dataTable = DataTable.New.ReadCsv(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/spamdata.csv"));
            List<string> x = dataTable.Rows.Select(row => row["Text"]).ToList();

            double[] y = dataTable.Rows.Select(row => double.Parse(row["IsSpam"])).ToArray();

            vocabulary = x.SelectMany(GetWords).Distinct().OrderBy(word => word).ToList();

            var problemBuilder = new TextClassificationProblemBuilder();
            var problem = problemBuilder.CreateProblem(x, y, vocabulary.ToList());

            const int C = 1;
            model = new C_SVC(problem, KernelHelper.LinearKernel(), C);

            _predictionDictionary = new Dictionary<int, string> { { -2, "Angry" }, { -1, "Sad" }, { 0, "Normal" }, { 1, "Happy" }, { 2, "Love" } };
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

        public string Analyze(string message)
        {

            //var accuracy = model.GetCrossValidationAccuracy(10);
            //var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("Files/model_{0}_accuracy.model", accuracy));
            //model.Export(path);

            var newX = TextClassificationProblemBuilder.CreateNode(message, vocabulary);

            var predictedY = model.Predict(newX);
            var predictedProb = model.PredictProbabilities(newX);

            return $"The prediction is {_predictionDictionary[(int)predictedY]} value is {predictedY}";
        }
        
        private IEnumerable<string> GetWords(string x)
        {
            return x.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
