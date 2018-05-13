using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebEmoticonAnalysis.Models
{
    public class ResultModel
    {
        public AnalyzeTextResultViewModel Text { get; set; }
        public AnalyzeTweetResultViewModel Tweet { get; set; }
        public ICollection<AnalyzeSearchResultViewModel> Tweets { get; set; }
    }
}