using NaiveBayes.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes.Models
{
    class ClassInfo
    {
        public ClassInfo(string name, List<String> trainDocs)
        {
            Name = name;
            var features = trainDocs.SelectMany(x => x.StringExtention());
            WordsCount = features.Count();
            WordCount =
                features.GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());
            NumberOfDocs = trainDocs.Count;
        }
        public string Name { get; set; }
        public int WordsCount { get; set; }
        public Dictionary<string, int> WordCount { get; set; }
        public int NumberOfDocs { get; set; }
        public int NumberOfOccurencesInTrainDocs(String word)
        {
            if (WordCount.Keys.Contains(word)) return WordCount[word];
            return 0;
        }
    }
}
