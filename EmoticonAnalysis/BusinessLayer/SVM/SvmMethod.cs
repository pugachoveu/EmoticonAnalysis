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
    public static class SvmMethod
    {
        private static Dictionary<int, string> _predictionDictionary;

        public static string Analyze(string message)
        {

            // STEP 4: Read the data
            string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/spamdata.csv");
            var dataTable = DataTable.New.ReadCsv(dataFilePath);
            List<string> x = dataTable.Rows.Select(row => row["Text"]).ToList();

            double[] y = dataTable.Rows.Select(row => double.Parse(row["IsSpam"])).ToArray();

            var vocabulary = x.SelectMany(GetWords).Distinct().OrderBy(word => word).ToList();

            var problemBuilder = new TextClassificationProblemBuilder();
            var problem = problemBuilder.CreateProblem(x, y, vocabulary.ToList());

            // If you want you can save this problem with : 
            // ProblemHelper.WriteProblem(@"D:\MACHINE_LEARNING\SVM\Tutorial\sunnyData.problem", problem);
            // And then load it again using:
            // var problem = ProblemHelper.ReadProblem(@"D:\MACHINE_LEARNING\SVM\Tutorial\sunnyData.problem");

            const int C = 1;
            var model = new C_SVC(problem, KernelHelper.LinearKernel(), C);

            var accuracy = model.GetCrossValidationAccuracy(10);
            //Console.Clear();
            //Console.WriteLine(new string('=', 50));
            //Console.WriteLine("Accuracy of the model is {0:P}", accuracy);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("Files/model_{0}_accuracy.model", accuracy));
            model.Export(path);

            //Console.WriteLine(new string('=', 50));
            //Console.WriteLine("The model is trained. \r\nEnter a sentence to make a prediction. (ex: love hate dong)");
            //Console.WriteLine(new string('=', 50));

            string userInput;

            //This just takes the predicted value (-1 to 3) and translates to your categorization response

            _predictionDictionary = new Dictionary<int, string> { { -2, "Angry" }, { -1, "Sad" }, { 0, "Normal" }, { 1, "Happy" }, { 2, "Love" } };

            var newX = TextClassificationProblemBuilder.CreateNode(message, vocabulary);

            var predictedY = model.Predict(newX);
            var predictedProb = model.PredictProbabilities(newX);

            return $"The prediction is {_predictionDictionary[(int)predictedY]} value is {predictedY}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetWords(string x)
        {
            return x.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
