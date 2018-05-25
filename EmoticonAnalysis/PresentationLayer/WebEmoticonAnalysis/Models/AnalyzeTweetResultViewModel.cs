using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebEmoticonAnalysis.Models
{
    public class AnalyzeTweetResultViewModel
    {
        public ulong TweetId { get; set; }
        public string TweetAuthor { get; set; }
        public string TweetText { get; set; }
        public string SmileResult { get; set; }
        public string DictionaryResult { get; set; }
        public string SvmResult { get; set; }
        public string BayesResult { get; set; }
        public string TotalResult { get; set; }
    }
}