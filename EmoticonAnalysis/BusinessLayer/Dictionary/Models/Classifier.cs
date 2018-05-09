using Dictionary.Helpers;
using Dictionary.Interfaces;
using Dictionary.NLPToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.Models
{
    public class Classifier
    {
        private readonly IVocabulary _positiveClass;
        private readonly IVocabulary _negativeClass;
        private const int WORDS_IN_CHUNK = 50;

        public Classifier(IVocabulary first, IVocabulary second)
        {
            _positiveClass = first;
            _negativeClass = second;
        }

        /// <summary>
        /// Classifies the sentiment of the input text corpus
        /// </summary>
        /// <param name="contents">Text contents that needs to be classified. Works best for 1000+ words</param>
        /// <param name="wordsToIgnore"></param>
        /// <returns></returns>
        public IDictionary<string, double> Classify(string contents, HashSet<string> wordsToIgnore)
        {
            if (_positiveClass == null || _negativeClass == null)
                throw new Exception("One of the evidences were not properly defined");
            if (string.IsNullOrWhiteSpace(_positiveClass.Name()))
                throw new Exception("Evidence name not defined on the first one");
            if (string.IsNullOrWhiteSpace(_negativeClass.Name()))
                throw new Exception("Evidence name not defined on the second one");

            var words = Tokenizer.TokenizeNow(contents).ToList();
            var chunkSize = Math.Ceiling(words.Count() / (double)WORDS_IN_CHUNK);
            var index = 0;

            #region Classify in chunks

            var scores = new List<Dictionary<string, decimal>>();
            for (var i = 0; i < chunkSize; i++)
            {
                var score = new Dictionary<string, decimal>
                    {
                        {_positiveClass.Name(), (decimal) 0.0},
                        {_negativeClass.Name(), (decimal) 0.0}
                    };
                scores.Add(score);
            }

            foreach (var wordsChunk in words.Chunk(WORDS_IN_CHUNK))
            {
                foreach (
                    var word in
                        wordsChunk.Where(word => !string.IsNullOrWhiteSpace(word) && !wordsToIgnore.Contains(word)))
                {
                    //First Class
                    var classAEvidence = _positiveClass.GetVocabulary();
                    double wordCountInClassA;
                    wordCountInClassA = classAEvidence.TryGetValue(word, out wordCountInClassA)
                                            ? wordCountInClassA
                                            : 1;    //ToDo - Make this 1 or 0.01

                    var scoreClassA = (decimal)Math.Log(wordCountInClassA / _positiveClass.TotalWords());
                    scores[index][_positiveClass.Name()] += scoreClassA;

                    //Second Class
                    var classBEvidence = _negativeClass.GetVocabulary();
                    double wordCountInClassB;
                    wordCountInClassB = classBEvidence.TryGetValue(word, out wordCountInClassB)
                                            ? wordCountInClassB
                                            : 1;    //ToDo - Make this 1 or 0.01
                    var scoreClassB = (decimal)Math.Log(wordCountInClassB / _negativeClass.TotalWords());
                    scores[index][_negativeClass.Name()] += scoreClassB;

                    //Logger.DebugFormat(",[TAG_A],{0}, {1}, {2}, {3}, {4},,{5}, {6}, {7}, {8}", word, _positiveClass.Name(),
                    //wordCountInClassA, _positiveClass.TotalWords(), scoreClassA, _negativeClass.Name(),
                    //wordCountInClassB, _negativeClass.TotalWords(), scoreClassB);
                }

                var totalWordsAllCategories = _positiveClass.TotalWords() + _negativeClass.TotalWords();
                scores[index][_positiveClass.Name()] += (decimal)Math.Log(_positiveClass.TotalWords() / totalWordsAllCategories);
                scores[index][_negativeClass.Name()] += (decimal)Math.Log(_negativeClass.TotalWords() / totalWordsAllCategories);

                var scoreA = Math.Exp((double)scores[index][_positiveClass.Name()]);
                var scoreB = Math.Exp((double)scores[index][_negativeClass.Name()]);
                var totalScore = scoreA + scoreB;

                try
                {
                    scores[index][_positiveClass.Name()] = (decimal)(100 * scoreA / totalScore);
                    scores[index][_negativeClass.Name()] = (decimal)(100 * scoreB / totalScore);
                }
                catch (OverflowException overflow)
                {

                    throw;
                }

                //Logger.DebugFormat("Chunk_{0} score for {1} : {2}", index, _positiveClass.Name(), scores[index][_positiveClass.Name()]);
                //Logger.DebugFormat("Chunk_{0} score for {1} : {2}", index, _negativeClass.Name(), scores[index][_negativeClass.Name()]);

                index++;
            }

            //Coumpute the average
            var results = new Dictionary<string, double>
                                {
                                    {_positiveClass.Name(), 0.0},
                                    {_negativeClass.Name(), 0.0}
                                };

            foreach (var score in scores)
            {
                results[_positiveClass.Name()] += (double)score[_positiveClass.Name()];
                results[_negativeClass.Name()] += (double)score[_negativeClass.Name()];
            }

            results[_positiveClass.Name()] = results[_positiveClass.Name()] / scores.Count;
            results[_negativeClass.Name()] = results[_negativeClass.Name()] / scores.Count;

            //Logger.DebugFormat("Total score for {0} : {1}, {2} : {3} ", _positiveClass.Name(), results[_positiveClass.Name()],
            //                   _negativeClass.Name(), results[_negativeClass.Name()]);

            return results;

            #endregion Classify in chunks
        }
    }
}
