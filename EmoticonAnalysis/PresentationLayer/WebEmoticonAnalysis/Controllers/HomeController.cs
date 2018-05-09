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

        public async System.Threading.Tasks.Task<ActionResult> AnalyzeTweetAsync(string inputTweet, bool isSmile = false,
            bool isDictionary = false, bool isSvm = false, bool isBayes = false)
        {
            if (String.IsNullOrEmpty(inputTweet))
            {
                ViewBag.Error = "Input is empty";
                return View("~/Views/Home/Index.cshtml");
            }
            if (!isSmile && !isDictionary && !isSvm && !isBayes)
            {
                ViewBag.Error = "Select any checkbox";
                return View("~/Views/Home/Index.cshtml");
            }

            ulong tweetId = 0;
            var tweetUrlArray = inputTweet.Split('/');

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
            List<Status> status =
               await
               (from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Show &&
                      tweet.ID == tweetId &&
                      tweet.TweetMode == TweetMode.Extended &&
                      tweet.IncludeAltText == true
                select tweet)
               .ToListAsync();

            if (status != null)
            {
                currTweet = status.First();
                var message = currTweet.FullText;

                var smileResult = isSmile ? SmileMethod.Analyze(message) : String.Empty;
                var dictionaryResult = isDictionary ? DictionaryMethod.Analyze(message) : String.Empty;
                var svmResult = isSvm ? SvmMethod.Analyze(message) : String.Empty;
                var bayesResult = isBayes ? NaiveBayes.NaiveBayes.Analyze(message) : String.Empty;

                return View(new AnalyzeTweetResultViewModel
                {
                    TweetAuthor = currTweet.User.ScreenNameResponse,
                    TweetId = currTweet.StatusID,
                    TweetText = currTweet.FullText,
                    SmileResult = smileResult.ToString(),
                    DictionaryResult = dictionaryResult.ToString(),
                    SvmResult = svmResult,
                    BayesResult = bayesResult
                });
            }
            return View(new AnalyzeTweetResultViewModel());
        }
    }
}