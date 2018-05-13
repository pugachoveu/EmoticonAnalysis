using Dictionary.Models;
using HtmlAgilityPack;
using NReadability;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary
{
    public class DictionaryMethod
    {
        private static DictionaryMethod instance;
        private static Classifier classifier;
        private static Vocabulary positiveReviews;
        private static Vocabulary negativeReviews;

        private DictionaryMethod()
        {
            positiveReviews = new Vocabulary("Positive", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/Positive.Vocabulary.csv"));
            negativeReviews = new Vocabulary("Negative", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/Negative.Vocabulary.csv"));

            classifier = new Classifier(positiveReviews, negativeReviews);
        }

        public static DictionaryMethod Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DictionaryMethod();
                }
                return instance;
            }
        }

        public string Analyze( string message)
        {
             var scores = classifier.Classify(message, Dictionary.Helpers.VocabularyHelper.ExcludeList);

            return scores["Positive"] > scores["Negative"] ? "positive" : "negative";
        }
        //private String GetWebpageContents(String url)
        //{
        //    var nreadabilityTranscoder = new NReadabilityTranscoder();
        //    using (var wc = new WebClient())
        //    {
        //        var rawHtml = wc.DownloadString(url);
        //        var transcodingInput = new TranscodingInput(rawHtml);
        //        var extractedHtml = nreadabilityTranscoder.Transcode(transcodingInput).ExtractedContent;
        //        var pageHtml = new HtmlDocument();
        //        pageHtml.LoadHtml(extractedHtml);
        //        return pageHtml.DocumentNode.SelectSingleNode("//body").InnerText;
        //    }
        //}
    }
}
