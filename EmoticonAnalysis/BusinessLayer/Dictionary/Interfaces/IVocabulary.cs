using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.Interfaces
{
    public interface IVocabulary
    {
        string Name();
        double TotalWords();
        bool AddVocabularyData(string trainingData, HashSet<string> wordsToIgnore);
        bool AddVocabularyData(IEnumerable<string> trainingData, HashSet<string> wordsToIgnore);
        IDictionary<string, double> GetVocabulary();
        bool PersistVocabulary(bool backupExisting);
        void LoadVocabularyFromCache(string filePath);
    }
}
