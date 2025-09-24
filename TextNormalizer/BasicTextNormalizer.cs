using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unidecode.NET;

namespace TextNormalizer
{
    public static class DictionaryExtensions
    {
        // 重载 1：不指定默认值时，返回 TValue 的默认值（如 null）
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            dictionary.TryGetValue(key, out var value);
            return value;
        }

        // 重载 2：允许指定自定义默认值
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue defaultValue)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));

            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }
    }
    public class BasicTextNormalizer
    {
        private delegate string MyDelegate(string s, string keep = "");
        private event MyDelegate _clean;
        private Dictionary<string, string> _ADDITIONAL_DIACRITICS = new Dictionary<string, string>() {
            { "œ", "oe" },
            { "Œ", "OE" },
            { "ø", "o" },
            { "Ø", "O" },
            { "æ", "ae" },
            { "Æ", "AE" },
            { "ß", "ss" },
            { "ẞ", "SS" },
            { "đ", "d" },
            { "Đ", "D" },
            { "ð", "d" },
            { "Ð", "D" },
            { "þ", "th" },
            { "Þ", "th" },
            { "ł", "l" },
            { "Ł", "L" },
        };
        private bool _splitLetters = false;
        public BasicTextNormalizer(bool removeDiacritics = false, bool splitLetters = false)
        {
            _clean += removeDiacritics ? RemoveSymbolsAndDiacritics : RemoveSymbols;
            _splitLetters = splitLetters;
        }

        public string RemoveSymbolsAndDiacritics(string s, string keep="")
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in NormalizeNFKD(s))
            {
                if (keep.Contains(c))
                {
                    result.Append(c);
                }
                else if (_ADDITIONAL_DIACRITICS.ContainsKey(c.ToString()))
                {
                    result.Append(_ADDITIONAL_DIACRITICS.GetValueOrDefault(c.ToString()));
                }
                else if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                {
                    result.Append("");
                }
                else if (IsMarkerSymbolOrPunctuation(c))
                {
                    result.Append(" ");
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        static string NormalizeNFKD(string input)
        {
            StringBuilder normalized = new StringBuilder();
            foreach (char c in input.Normalize(NormalizationForm.FormKD))
            {
                // Here you can add more complex NFKD normalization logic if needed
                normalized.Append(c);
            }
            return normalized.ToString();
        }

        static bool IsMarkerSymbolOrPunctuation(char c)
        {
            string category = CharUnicodeInfo.GetUnicodeCategory(c).ToString();
            return category.StartsWith("Punctuation") || category.StartsWith("Symbol") || category.StartsWith("Modifier");
        }

        private string RemoveSymbols(string s, string keep = "")
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in NormalizeNFKC(s))
            {
                string category = CharUnicodeInfo.GetUnicodeCategory(c).ToString();
                if (category.StartsWith("Punctuation") || category.StartsWith("Symbol") || category.StartsWith("Modifier"))
                {
                    result.Append(' ');
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        static string NormalizeNFKC(string input)
        {
            StringBuilder normalized = new StringBuilder();
            foreach (char c in input)
            {
                string normalizedChar = CharUnicodeInfo.GetUnicodeCategory(c).ToString();
                if (normalizedChar.StartsWith("Punctuation") || normalizedChar.StartsWith("Symbol") || normalizedChar.StartsWith("Modifier"))
                {
                    normalized.Append(' ');
                }
                else
                {
                    normalized.Append(c);
                }
            }
            return normalized.ToString();
        }

        public string GetBasicTextNormalizer(string s)
        {
            s = s.ToLower();
            s = Regex.Replace(s, @"[<\[][^>\]]*[>\]]", "");  // remove words between brackets
            s = Regex.Replace(s, @"\(([^)]+?)\)", "");  // remove words between parenthesis
            s = _clean(s).ToLower();

            if (_splitLetters)
            {
                s = string.Join(" ", Regex.Matches(s, @"\X", RegexOptions.ECMAScript | RegexOptions.Singleline).Cast<Match>().Select(m => m.Value));
            }

            s = Regex.Replace(s, @"\s+", " ");  // replace any successive whitespace characters with a space

            return s;
        }
    }
}
