using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebEmoticonAnalysis.Models
{
    public class AnalyzeTextResultViewModel
    {
        public string TweetText { get; set; }
        public string SmileResult { get; set; }
        public string DictionaryResult { get; set; }
        public string SvmResult { get; set; }
        public string BayesResult { get; set; }
    }
}