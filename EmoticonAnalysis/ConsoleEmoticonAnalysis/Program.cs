using Dictionary;
using Smile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleEmoticonAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AFINN-111.txt");

            var fileLines= File.ReadAllLines(path);

            List<string> positive = new List<string>();
            List<string> negative = new List<string>();

            
            foreach (var line in fileLines)
            {
                var lineArr = line.Split( new char[] {'\t'}, StringSplitOptions.RemoveEmptyEntries);
                if (Convert.ToInt32(lineArr.Last()) >= 0)
                {
                    var temp = $"{lineArr.First()},{lineArr.Last()}";
                    positive.Add(temp);
                }
                else
                {
                    var temp = $"{lineArr.First()},{Convert.ToInt32(lineArr.Last())*-1}";
                    negative.Add(temp);
                }
            }
           

            //string posCsv = "";
           // string negativeCsv = "";

            File.WriteAllLines("textPos.csv", positive.Select(x => string.Join("\n", x)));
            File.WriteAllLines("textNeg.csv", negative.Select(x => string.Join("\n", x)));
            Console.ReadLine();
        }
    }
}
