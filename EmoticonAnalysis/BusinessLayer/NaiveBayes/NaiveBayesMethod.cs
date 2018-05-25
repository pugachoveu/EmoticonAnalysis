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
    public class NaiveBayesMethod
    {
        
        static List<Document> trainCorpus = new List<Document>();
        private static NaiveBayesMethod instance;
        private static Classifier classifier;

        private NaiveBayesMethod()
        {
            trainCorpus = LoadTweetCorpus();
            classifier = new Classifier(trainCorpus);
        }

        public static NaiveBayesMethod Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NaiveBayesMethod();
                }
                return instance;
            }
        }

        public int Analyze(string message)
        {
        
            var positiveRes = classifier.IsInClassProbability("1", message);//positive
            var negativeRes = classifier.IsInClassProbability("0", message);//negative

            return positiveRes>negativeRes? 1 : -1 ;
        }

        private List<Document> LoadTweetCorpus()
        {
            try
            {
                List<Document> corpus = new List<Document>();
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/SentimentAnalysisDataset.csv");
                if (File.Exists(path))
                {
                    var lines = File.ReadAllLines(path);
                    for (int i = 0; i < 5000; i++)//5146
                    {
                        var lineArr = lines[i].Split(new string[] { ",Sentiment140,", ",Kaggle," }, StringSplitOptions.None);
                        var t = lineArr[0].Split(',')[1];
                        var text = lineArr[1].Trim();
                        corpus.Add(new Document(t, text));
                    }

                }
                return corpus;
            }
            catch (Exception e)
            {
                Console.WriteLine("error");
                return new List<Document>();
            }
        }
    }
}
