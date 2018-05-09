using NaiveBayes.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes.Models
{
    class Classifier
    {
        List<ClassInfo> _classes;
        int _countOfDocs;
        int _uniqWordsCount;
        public Classifier(List<Document> train)
        {
            _classes = train.GroupBy(x => x.Type).Select(g => new ClassInfo(g.Key, g.Select(x => x.Text).ToList())).ToList();
            _countOfDocs = train.Count;
            _uniqWordsCount = train.SelectMany(x => x.Text.Split(' ')).GroupBy(x => x).Count();
        }

        public double IsInClassProbability(string className, string text)
        {
            var words = text.StringExtention();
            var classResults = _classes
                .Select(x => new
                {
                    Result = Math.Pow(Math.E, Calc(x.NumberOfDocs, _countOfDocs, words, x.WordsCount, x, _uniqWordsCount)),
                    ClassName = x.Name
                });


            return classResults.Single(x => x.ClassName == className).Result / classResults.Sum(x => x.Result);
        }

        private static double Calc(double dc, double d, List<String> q, double lc, ClassInfo @class, double v)
        {
            return Math.Log(dc / d) + q.Sum(x => Math.Log((@class.NumberOfOccurencesInTrainDocs(x) + 1) / (v + lc)));
        }
    }
}
