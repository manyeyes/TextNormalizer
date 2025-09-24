using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextNormalizer
{
    public class EnglishTextNormalizer
    {
        private string _ignorePatterns = @"\b(hmm|mm|mhm|mmm|uh|um)\b";
        private Dictionary<string, string> _replacers = new Dictionary<string, string>() {
            // common contractions
            {@"\bwon't\b","will not" },
            {@"\bcan't\b","can not" },
            {@"\blet's\b","let us" },
            {@"\bain't\b","aint" },
            {@"\by'all\b","you all" },
            {@"\bwanna\b","want to" },
            {@"\bgotta\b","got to" },
            {@"\bgonna\b","going to" },
            {@"\bi'ma\b","i am going to" },
            {@"\bimma\b","i am going to" },
            {@"\bwoulda\b","would have" },
            {@"\bcoulda\b","could have" },
            {@"\bshoulda\b","should have" },
            {@"\bma'am\b","madam" },
            // contractions in titles/prefixes
            {@"\bmr\b","mister " },
            {@"\bmrs\b","missus " },
            {@"\bst\b","saint " },
            {@"\bdr\b","doctor " },
            {@"\bprof\b","professor " },
            {@"\bcapt\b","captain " },
            {@"\bgov\b","governor " },
            {@"\bald\b","alderman " },
            {@"\bgen\b","general " },
            {@"\bsen\b","senator " },
            {@"\brep\b","representative " },
            {@"\bpres\b","president " },
            {@"\brev\b","reverend " },
            {@"\bhon\b","honorable " },
            {@"\basst\b","assistant " },
            {@"\bassoc\b","associate " },
            {@"\blt\b","lieutenant " },
            {@"\bcol\b","colonel " },
            {@"\bjr\b","junior " },
            {@"\bsr\b","senior " },
            {@"\besq\b","esquire " },
            // prefect tenses, ideally it should be any past participles, but it's harder..
            {@"'d been\b"," had been" },
            {@"'s been\b"," has been" },
            {@"'d gone\b"," had gone" },
            {@"'s gone\b"," has gone" },
            {@"'d done\b"," had done" },  // "'s done" is ambiguous
            {@"'s got\b"," has got" },
            // general contractions
            {@"n't\b"," not" },
            {@"'re\b"," are" },
            {@"'s\b"," is" },
            {@"'d\b"," would" },
            {@"'ll\b"," will" },
            {@"'t\b"," not" },
            {@"'ve\b"," have" },
            {@"'m\b"," am" },
        };
        private BasicTextNormalizer _basicTextNormalizer = new BasicTextNormalizer();
        private EnglishNumberNormalizer _standardizeNumbers = new EnglishNumberNormalizer();
        private EnglishSpellingNormalizer _standardizeSpellings = new EnglishSpellingNormalizer();

        public EnglishTextNormalizer()
        {

        }

        public string GetEnglishTextNormalizer_original(string s)
        {
            s = s.ToLower();

            s = Regex.Replace(s, @"[<\[][^>\]]*[>\]]", "");  // remove words between brackets
            s = Regex.Replace(s, @"\(([^)]+?)\)", "");  // remove words between parenthesis
            s = Regex.Replace(s, _ignorePatterns, "");
            s = Regex.Replace(s, @"\s+'", "'");  // when there's a space before an apostrophe

            foreach (var item in _replacers)
            {
                string pattern = item.Key;
                string replacement = item.Value;
                s = Regex.Replace(s, pattern, replacement);
            }

            s = Regex.Replace(s, @"(\d),(\d)", @"$1$2");  // remove commas between digits
            s = Regex.Replace(s, @"\.([^0-9]|$)", @" $1");  // remove periods not followed by numbers
            s = _basicTextNormalizer.RemoveSymbolsAndDiacritics(s, keep: ".%$¢€£");  // keep numeric symbols // TODO : BasicTextNormalizer

            s = _standardizeNumbers.GetEnglishNumberNormalizer(s);
            s = _standardizeSpellings.GetEnglishSpellingNormalizer(s);

            // now remove prefix/suffix symbols that are not preceded/followed by numbers
            s = Regex.Replace(s, @"[.$¢€£]([^0-9])", @" $1");
            s = Regex.Replace(s, @"([^0-9])%", @"$1 ");

            s = Regex.Replace(s, @"\s+", " ");  // replace any successive whitespaces with a space

            return s;
        }

        /// <summary>
        /// solve issue of punctuation, e.g. (. , ())
        /// 仅英文句号.替换为▁
        /// replace:(半角()[]为全角 （ ） [ ] )
        /// </summary>
        /// <param name="s"></param>
        /// <param name="isRemoveBetween">remove words between ()[]<></param>
        /// <returns></returns>
        public string GetEnglishTextNormalizer(string s, bool isRemoveBetween = true)
        {
            s = s.ToLower();
            if (isRemoveBetween)
            {
                s = Regex.Replace(s, @"[<\[][^>\]]*[>\]]", "");  // remove words between brackets
                s = Regex.Replace(s, @"\(([^)]+?)\)", "");  // remove words between parenthesis
            }
            else
            {
                s = Regex.Replace(s, @"\s*([\<|\>|\[|\]\(|\)])\s*", @" $1 ");
            }
            s = Regex.Replace(s, _ignorePatterns, "");
            s = Regex.Replace(s, @"\s+'", "'");  // when there's a space before an apostrophe

            foreach (var item in _replacers)
            {
                string pattern = item.Key;
                string replacement = item.Value;
                s = Regex.Replace(s, pattern, replacement);
            }

            s = Regex.Replace(s, @"(\d),(\d)", @"$1$2");  // remove commas between digits
            s = Regex.Replace(s, @"\.([^0-9]|$)", @"▁");  // remove periods not followed by numbers

            s = _basicTextNormalizer.RemoveSymbolsAndDiacritics(s, keep: ".%$¢€£");  // keep numeric symbols // TODO : BasicTextNormalizer

            s = s.Replace(" ▁", " ");//表示缩写的"."被替换为" ▁",所以这里将" ▁"替换为" "
            s = s.Replace("▁", " ▁ ");//句号被替换为"▁",所以这里将"▁"替换为" ▁ "
            s = s.Replace(",", " ▁▁ ");//将所有逗号","替换为" ▁▁ "

            s = _standardizeNumbers.GetEnglishNumberNormalizer(s);
            s = _standardizeSpellings.GetEnglishSpellingNormalizer(s);

            // now remove prefix/suffix symbols that are not preceded/followed by numbers
            s = Regex.Replace(s, @"[.$¢€£]([^0-9])", @" $1");
            s = Regex.Replace(s, @"([^0-9])%", @"$1 ");

            s = Regex.Replace(s, @"\s+", " ");  // replace any successive whitespaces with a space

            // restore comma period
            s = Regex.Replace(s, @"\s*▁▁\s*", @", ");
            s = Regex.Replace(s, @"\s*▁\s*", @". ");
            s = Regex.Replace(s, @"\s*([\<|\>|\[|\]\(|\)])\s*", @"$1");//remove multiple spaces before and after punctuation
            s = Regex.Replace(s, @"(\s*([\,|，| ])\s*)+", @"$1");//replace multiple spaces before and after punctuation with one space
            return s;
        }
    }
}
