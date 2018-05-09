using NaiveBayes.Helpers;
using NaiveBayes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes
{
    public static class NaiveBayes
    {
        //static List<Document> _trainCorpus = new List<Document>
        //{
        //    new Document("spam", "предоставляю услуги бухгалтера"),
        //    new Document("spam", "спешите купить виагру"),
        //    new Document("ham", "надо купить молоко")
        //};
        static List<Document> _trainCorpus = new List<Document>();

        static string test = "надо купить сигареты";

        public static string Analyze(string message)
        {
            LoadTweetCorpus();
            var c = new Classifier(_trainCorpus);
            var res = c.IsInClassProbability("1", message);

            return res.ToString();
        }

        private static void LoadTweetCorpus()
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/SentimentAnalysisDataset.csv");
                if (File.Exists(path))
                {
                    var lines = File.ReadAllLines(path);
                    for (int i = 0; i < 5000; i++)//5146
                    {
                        var lineArr = lines[i].Split(new string[] { ",Sentiment140,", ",Kaggle," }, StringSplitOptions.None);
                        var t = lineArr[0].Split(',')[1];
                        var text = lineArr[1].Trim();
                        _trainCorpus.Add(new Document(t, text));
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error");
            }
        }
    }
}
