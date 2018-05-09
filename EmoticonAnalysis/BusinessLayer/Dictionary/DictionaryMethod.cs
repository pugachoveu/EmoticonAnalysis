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
    public static class DictionaryMethod
    {
        public static string Analyze( string message)
        {
            const string url = "http://www.engadget.com/2014/02/19/nokia-lumia-icon-review/";
            var testData = GetWebpageContents(url);

            //These evidences are used as training data for the Dragon Classigfier
  //          var positiveReviews = new Vocabulary("Positive", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Positive.Vocabulary.csv"));
//            var negativeReviews = new Vocabulary("Negative", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Negative.Vocabulary.csv"));

            var positiveReviews = new Vocabulary("Positive", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/Positive.Vocabulary.csv"));
            var negativeReviews = new Vocabulary("Negative", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/Negative.Vocabulary.csv"));

            var classifier = new Classifier(positiveReviews, negativeReviews);

            var scores = classifier.Classify(message, Dictionary.Helpers.VocabularyHelper.ExcludeList);
            //var scores = classifier.Classify(testData, Dictionary.Helpers.VocabularyHelper.ExcludeList);

            Console.WriteLine("Positive Score for " + url + " - " + scores["Positive"]);

            
            return scores["Positive"] > scores["Negative"] ? "positive" : "negative";
        }
        private static String GetWebpageContents(String url)
        {
            var nreadabilityTranscoder = new NReadabilityTranscoder();
            using (var wc = new WebClient())
            {
                var rawHtml = wc.DownloadString(url);
                var transcodingInput = new TranscodingInput(rawHtml);
                var extractedHtml = nreadabilityTranscoder.Transcode(transcodingInput).ExtractedContent;
                var pageHtml = new HtmlDocument();
                pageHtml.LoadHtml(extractedHtml);
                return pageHtml.DocumentNode.SelectSingleNode("//body").InnerText;
            }
        }
    }
}
