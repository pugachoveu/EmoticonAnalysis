using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary.NLPToolkit
{
    public static class Tokenizer
    {
        static Tokenizer()
        {
        }

        /// <summary>
        /// Split the input content to individual words
        /// </summary>
        /// <param name="contents">Content to split into words</param>
        /// <returns></returns>
        public static IEnumerable<string> TokenizeNow(string contents)
        {
            //ToDo: Make preprocessing function a functional pointer
            var processedContents = PreProcessing(contents);

            string[] separatingChars = { " ", ".", "," };
            string[] words = processedContents.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        /// <summary>
        /// ToDo: Test scenario where numbers are replaced with *
        /// </summary>
        /// <param name="inp"></param>
        /// <returns></returns>
        private static string PreProcessing(string inp)
        {
            var sb = new StringBuilder();

            //Retain only characters
            foreach (var c in inp)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }
    }
}
