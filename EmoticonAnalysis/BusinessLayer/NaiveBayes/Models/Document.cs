using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaiveBayes.Models
{
    class Document
    {
        public Document(string type, string text)
        {
            Type = type;
            Text = text;
        }
        public string Type { get; set; }
        public string Text { get; set; }
    }
}
