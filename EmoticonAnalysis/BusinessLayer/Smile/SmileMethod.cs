using Newtonsoft.Json;
using Smile.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smile
{
    public static class SmileMethod
    {
        public static string Analyze(string message)
        {
            var emoticonList = LoadEmoticons();
            int meaningValue = 0;
            var messArr = message.Split(' ');

            foreach (var e in messArr)
            {
                var t = emoticonList.Where(c => c.Emoji.Contains(e));
                if (t.Any())
                {
                    meaningValue += t.First().Polarity;
                }
            }
            return meaningValue > 0 ? "positive" : "negative";
        }

        private static List<EmoticonModel> LoadEmoticons()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files/emoticon_list.txt");
            if (!File.Exists(path))
                return null;

            var fileJsonData = File.ReadAllText(path);

            var emoticonList = JsonConvert.DeserializeObject<List<EmoticonModel>>(fileJsonData);

            return emoticonList;
        }
    }
}
