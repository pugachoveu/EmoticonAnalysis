using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using WebEmoticonAnalysis.Models;
using Smile;
using Dictionary;
using SVM;
using NaiveBayes;
using System.IO;

namespace WebEmoticonAnalysis.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async System.Threading.Tasks.Task<ActionResult> AnalyzeTweetAsync(TweetAnalyzeViewModel model)
        {
            System.Diagnostics.Debug.WriteLine("SomeText");
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            SmileMethod smileAnalyzer = SmileMethod.Instance;
            DictionaryMethod dictionaryAnalyzer = DictionaryMethod.Instance;
            SvmMethod svmAnalyzer = SvmMethod.Instance;
            NaiveBayesMethod naiveBayesAnalyzer = NaiveBayesMethod.Instance;

            if (String.IsNullOrEmpty(model.InputTweet))
            {
                ViewBag.Error = "Input is empty";
                return View("~/Views/Home/Index.cshtml");
            }
            if (!model.IsSmile && !model.IsDictionary && !model.IsSvm && !model.IsBayes)
            {
                ViewBag.Error = "Select any checkbox";
                return View("~/Views/Home/Index.cshtml");
            }

            var result = new ResultModel();
            ulong tweetId = 0;
            var tweetUrlArray = model.InputTweet.Split('/');

            try
            {
                tweetId = Convert.ToUInt64(tweetUrlArray.Last());
            }
            catch (FormatException e)
            {
                ViewBag.Error = "Enter correct tweet path or id";
                return View("~/Views/Home/Index.cshtml");
            }

            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore()
                {
                    ConsumerKey = "7utcTWUvNbtE7fexfkClOnr8N",
                    ConsumerSecret = "n5QBwTBeYsw1Bsj7K9M22St2yeGdErBTRK0ZoVLwXMPkjL5DFk"
                }
            };

            await auth.AuthorizeAsync();

            var twitterCtx = new TwitterContext(auth);
            var currTweet = new Status();
            var status =
               await
               (from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Show &&
                      tweet.ID == tweetId &&
                      tweet.TweetMode == TweetMode.Extended &&
                      tweet.IncludeAltText == true
                select tweet)
               .ToListAsync();

            //Analyze tweet
            if (status != null)
            {
                currTweet = status.First();
                var message = currTweet.FullText;

                var smileResult = model.IsSmile ? smileAnalyzer.Analyze(message) : String.Empty;
                var dictionaryResult = model.IsDictionary ? dictionaryAnalyzer.Analyze(message) : String.Empty;
                var svmResult = model.IsSvm ? svmAnalyzer.Analyze(message) : String.Empty;
                var bayesResult = model.IsBayes ? naiveBayesAnalyzer.Analyze(message) : String.Empty;

                result.Tweet = new AnalyzeTweetResultViewModel
                {
                    TweetAuthor = currTweet.User.ScreenNameResponse,
                    TweetId = currTweet.StatusID,
                    TweetText = currTweet.FullText,
                    SmileResult = smileResult.ToString(),
                    DictionaryResult = dictionaryResult.ToString(),
                    SvmResult = svmResult,
                    BayesResult = bayesResult
                };
            }
            //Analyze text
            if (!String.IsNullOrEmpty(model.TextTweet))
            {
                var message = model.TextTweet;

                var smileResult = model.IsSmile ? smileAnalyzer.Analyze(message) : String.Empty;
                var dictionaryResult = model.IsDictionary ? dictionaryAnalyzer.Analyze(message) : String.Empty;
                var svmResult = model.IsSvm ? svmAnalyzer.Analyze(message) : String.Empty;
                var bayesResult = model.IsBayes ? naiveBayesAnalyzer.Analyze(message) : String.Empty;

                result.Text = new AnalyzeTextResultViewModel
                {
                    TweetText = model.TextTweet,
                    SmileResult = smileResult.ToString(),
                    DictionaryResult = dictionaryResult.ToString(),
                    SvmResult = svmResult,
                    BayesResult = bayesResult
                };
            }
            //Analyze tweet search
            if (!String.IsNullOrEmpty(model.SearchTweet))
            {
                ViewBag.SearchString = model.SearchTweet;
                Search searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                     search.Query == model.SearchTweet &&
                     search.IncludeEntities == true &&
                     search.TweetMode == TweetMode.Extended
                 select search)
                .SingleOrDefaultAsync();

                if (searchResponse?.Statuses != null)
                {
                    result.Tweets = new List<AnalyzeSearchResultViewModel>();
                    foreach (var tweet in searchResponse.Statuses)
                    {
                        var message = model.TextTweet;

                        var smileResult = model.IsSmile ? smileAnalyzer.Analyze(message) : String.Empty;
                        var dictionaryResult = model.IsDictionary ? dictionaryAnalyzer.Analyze(message) : String.Empty;
                        var svmResult = model.IsSvm ? svmAnalyzer.Analyze(message) : String.Empty;
                        var bayesResult = model.IsBayes ? naiveBayesAnalyzer.Analyze(message) : String.Empty;

                        result.Tweets.Add(new AnalyzeSearchResultViewModel
                        {
                            TweetAuthor = tweet.User.ScreenNameResponse,
                            TweetId = tweet.StatusID,
                            TweetText = tweet.FullText,
                            SmileResult = smileResult.ToString(),
                            DictionaryResult = dictionaryResult.ToString(),
                            SvmResult = svmResult,
                            BayesResult = bayesResult
                        });
                    }
                }
            }

            sw.Stop();
            System.Diagnostics.Debug.WriteLine((sw.ElapsedMilliseconds / 100.0).ToString());

            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/logs.txt");
            
            string log = $"{(sw.ElapsedMilliseconds / 100.0).ToString()}" + Environment.NewLine;
            if (!System.IO.File.Exists(logFilePath))
                System.IO.File.WriteAllText(logFilePath, log);
            else
                System.IO.File.AppendAllText(logFilePath, log);

            return View(result);
        }
    }
}