using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace HTMLExtractor
{
    class Extensions
    {
        public static string GetStringBetweenTags(string value, string startTag, string endTag)
        {
            if (value.Contains(startTag) && value.Contains(endTag))
            {
                int index = value.IndexOf(startTag) + startTag.Length;
                return value.Substring(index, value.IndexOf(endTag) - index);
            }
            else
                return null;
        }

        public static List<Match> MultipleRegexMatches(string pattern, string sentence)
        {
            List<Match> Returnable=new List<Match>();

            //string pattern = @"\b\w+es\b";
            Regex rgx = new Regex(pattern);
            //string sentence = "Who writes these notes?";
            foreach (Match match in rgx.Matches(sentence))
            {
                //MessageBox.Show(match.Value, match.Index.ToString());
                Returnable.Add(match);
            }
            return Returnable;
        }

        public static void ReplacePatterns(string Pattern1, string Pattern2, ref string allText)
        {
            Regex regexOne = new Regex(Pattern1);
            Regex regexTwo = new Regex(Pattern2);
            string replacement = "";
            allText = regexOne.Replace(allText, replacement);
            allText = regexTwo.Replace(allText, replacement);
            ;
        }
    }
}
