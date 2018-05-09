using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NaiveBayes.Helpers
{
    public static class Helpers
    {
        public static List<String> StringExtention(this String text)
        {
            return Regex.Replace(text, "\\p{P}+", "").Split(' ').ToList();
        }
    }
}
