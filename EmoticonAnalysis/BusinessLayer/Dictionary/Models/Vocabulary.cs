using Dictionary.Interfaces;
using Dictionary.NLPToolkit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.Models
{
    public class Vocabulary : IVocabulary
    {
        private readonly Dictionary<string, double> _vocabularies;
        private readonly string _vocabularyRepository;
        private readonly string _vocabularyFileName;
        private readonly string _vocabularyFilePath;
        private const char VOCABULARY_SEPARATOR = ',';
        private double _totalWords;//total count words value
        private readonly string _className;
        private readonly bool _saveVocabulary;

        public Vocabulary(string className, string vocabularyFileName, bool loadVocabulary = true, bool saveVocabulary = false)
        {
            if (string.IsNullOrWhiteSpace(className)) throw new Exception("Class name was not defined");

            _saveVocabulary = saveVocabulary;
            _className = className;
            _vocabularies = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
            _vocabularyFileName = vocabularyFileName;

            if (loadVocabulary) LoadVocabularyFromCache(vocabularyFileName);
        }

        ~Vocabulary()
        {
            if (_saveVocabulary) PersistVocabulary();
        }

        public void LoadVocabularyFromCache(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return;


                using (var file = new StreamReader(filePath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        var keyValue = line.Split(VOCABULARY_SEPARATOR);
                        if (!_vocabularies.ContainsKey(keyValue[0]))
                        {
                            var value = int.Parse(keyValue[1]);
                            _vocabularies.Add(keyValue[0], value);
                            _totalWords += value;
                        }
                        else
                        {
                            throw new Exception(
                                string.Format("Duplicate entries of {0} found while loading vocabulary from {1}",
                                              keyValue[0], filePath));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool AddVocabularyData(string trainingData, HashSet<string> wordsToIgnore)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(trainingData)) return false;

                var tokens = Tokenizer.TokenizeNow(trainingData).ToList();

                foreach (
                    var token in
                        tokens.Where(token => !string.IsNullOrWhiteSpace(token) && !wordsToIgnore.Contains(token)))
                {
                    if (!_vocabularies.ContainsKey(token))
                        _vocabularies[token] = 0;

                    _vocabularies[token]++;
                    _totalWords++;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public bool AddVocabularyData(IEnumerable<string> trainingData, HashSet<string> wordsToIgnore)
        {
            foreach (var data in trainingData)
            {
                AddVocabularyData(data, wordsToIgnore);
            }

            return true;
        }

        public IDictionary<string, double> GetVocabulary()
        {
            return _vocabularies;
        }

        public bool PersistVocabulary(bool backupExisting = true)
        {
            try
            {
                if (_vocabularies == null || _vocabularies.Count <= 0)
                    return false;

                if (backupExisting)

                    using (var file = new StreamWriter(_vocabularyFilePath))
                    {
                        foreach (
                            var line in
                                _vocabularies.Select(
                                    evidence =>
                                    evidence.Key.Trim() + "," + evidence.Value.ToString(CultureInfo.InvariantCulture).Trim())
                            )
                        {
                            file.WriteLine(line);
                        }
                    }
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

        public string Name()
        {
            return _className;
        }

        public double TotalWords()
        {
            return _totalWords;
        }
    }
}
