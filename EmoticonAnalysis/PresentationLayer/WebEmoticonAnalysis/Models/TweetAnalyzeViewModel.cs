using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebEmoticonAnalysis.Models
{
    public class TweetAnalyzeViewModel
    {
        public string SearchTweet { get; set; }
        public string InputTweet { get; set; }
        public string TextTweet { get; set; }

        public bool IsSmile { get; set; } = false;
        public bool IsDictionary { get; set; } = false;
        public bool IsSvm { get; set; } = false;
        public bool IsBayes { get; set; } = false;
    }
}