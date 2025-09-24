using Rationals;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

namespace TextNormalizer
{
    public class EnglishNumberNormalizer
    {
        private Dictionary<string, int> _zeros = new Dictionary<string, int>() {
            { "o" ,0},{ "oh",0 },{ "zero",0 }
        };
        private string[] _one = new string[] {
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "ten",
            "eleven",
            "twelve",
            "thirteen",
            "fourteen",
            "fifteen",
            "sixteen",
            "seventeen",
            "eighteen",
            "nineteen",
        };
        private Dictionary<string, int> _ones = new Dictionary<string, int>();
        private Dictionary<string, Tuple<int, string>> _onesPlural = new Dictionary<string, Tuple<int, string>>();
        private Dictionary<string, Tuple<int, string>> _onesOrdinal = new Dictionary<string, Tuple<int, string>>()
        {
            {"zeroth", new Tuple<int,string>(0,"th") },
            {"first",new Tuple<int,string>(1,"st") },
            {"second", new Tuple<int,string>(2,"nd")},
            {"third", new Tuple<int,string>(3,"rd")},
            {"fifth", new Tuple<int,string>(5,"th")},
            {"twelfth",new Tuple<int,string>(12,"th")},
        };
        private Dictionary<string, Tuple<int, string>> _onesSuffixed = new Dictionary<string, Tuple<int, string>>();
        private Dictionary<string, int> _tens = new Dictionary<string, int>() {
            { "twenty", 20 },
            { "thirty", 30 },
            { "forty", 40 },
            { "fifty", 50 },
            { "sixty", 60 },
            { "seventy", 70 },
            { "eighty", 80 },
            { "ninety", 90 },
        };
        private Dictionary<string, Tuple<int, string>> _tensPlural = new Dictionary<string, Tuple<int, string>>();
        private Dictionary<string, Tuple<int, string>> _tensOrdinal = new Dictionary<string, Tuple<int, string>>();
        private Dictionary<string, Tuple<int, string>> _tensSuffixed = new Dictionary<string, Tuple<int, string>>();
        private Dictionary<string, BigInteger> _multipliers = new Dictionary<string, BigInteger>()
        {
            { "hundred", 100 },
            { "thousand", 1_000 },
            { "million", 1_000_000 },
            { "billion", 1_000_000_000 },
            { "trillion", 1_000_000_000_000 },
            { "quadrillion", 1_000_000_000_000_000 },
            { "quintillion", 1_000_000_000_000_000_000 },
            { "sextillion",  BigInteger.Pow(10,21)},//1_000_000_000_000_000_000_000
            { "septillion", BigInteger.Pow(10,24) },//1_000_000_000_000_000_000_000_000
            { "octillion", BigInteger.Pow(10,27) },//1_000_000_000_000_000_000_000_000_000
            { "nonillion", BigInteger.Pow(10,30) },//1_000_000_000_000_000_000_000_000_000_000
            { "decillion", BigInteger.Pow(10,33) },//1_000_000_000_000_000_000_000_000_000_000_000
        };
        private Dictionary<string, Tuple<BigInteger, string>> _multipliersPlural = new Dictionary<string, Tuple<BigInteger, string>>();
        private Dictionary<string, Tuple<BigInteger, string>> _multipliersOrdinal = new Dictionary<string, Tuple<BigInteger, string>>();
        private Dictionary<string, Tuple<BigInteger, string>> _multipliersSuffixed = new Dictionary<string, Tuple<BigInteger, string>>();
        private Dictionary<string, int> _decimals = new Dictionary<string, int>();
        private Dictionary<string, string> _precedingPrefixers = new Dictionary<string, string>()
        {
            { "minus", "-" },
            { "negative", "-"},
            { "plus", "+"},
            { "positive", "+"},
        };
        private Dictionary<string, string> _followingPrefixers = new Dictionary<string, string>() {
            { "pound", "£"},
            { "pounds", "£"},
            { "euro", "€"},
            { "euros", "€"},
            { "dollar", "$"},
            { "dollars", "$"},
            { "cent", "¢"},
            { "cents", "¢"},
        };
        private List<string> _prefixes = new List<string>();
        private Dictionary<string, string[]> _suffixers = new Dictionary<string, string[]>() {
            { "per",new string[] {"cent", "%"} },
            { "percent",new string[] { "%"} },
        };
        private string[] _specials = new string[] { "and", "double", "triple", "point" };
        private List<string> _words = new List<string>();
        private string[] _literalWords = new string[] { "one", "ones" };

        public EnglishNumberNormalizer()
        {
            BigInteger decillion = BigInteger.Pow(10, 66);
            for (int i = 0; i < _one.Length; i++)
            {
                _ones.Add(_one[i], i + 1);
            }
            foreach (var item in _ones)
            {
                _onesPlural.Add(item.Key == "six" ? "sixes" : item.Key + "s", new Tuple<int, string>(item.Value, "s"));
            }
            foreach (var item in _ones)
            {
                if (item.Value > 3 && item.Value != 5 && item.Value != 12)
                {
                    _onesOrdinal.Add(item.Key.EndsWith("t") ? item.Key + "h" : item.Key + "th", new Tuple<int, string>(item.Value, "th"));
                }
            }
            _onesSuffixed = _onesPlural.Union(_onesOrdinal).ToDictionary(x => x.Key, x => x.Value);
            foreach (var item in _tens)
            {
                _tensPlural.Add(item.Key.Replace("y", "ies"), new Tuple<int, string>(item.Value, "s"));
            }
            foreach (var item in _tens)
            {
                _tensOrdinal.Add(item.Key.Replace("y", "ieth"), new Tuple<int, string>(item.Value, "th"));
            }
            _tensSuffixed = _tensPlural.Union(_tensOrdinal).ToDictionary(x => x.Key, x => x.Value);
            foreach (var item in _multipliers)
            {
                _multipliersPlural.Add(item.Key + "s", new Tuple<BigInteger, string>(item.Value, "s"));
            }
            foreach (var item in _multipliers)
            {
                _multipliersOrdinal.Add(item.Key + "th", new Tuple<BigInteger, string>(item.Value, "th"));
            }
            _multipliersSuffixed = _multipliersPlural.Union(_multipliersOrdinal).ToDictionary(x => x.Key, x => x.Value);
            _decimals = _ones.Union(_tens).Union(_zeros).ToDictionary(
                item => item.Key,
                item => item.Value
                );
            _prefixes = _precedingPrefixers.Values.ToList();
            _prefixes.AddRange(_followingPrefixers.Values.ToList());
            _words = _zeros.Keys.ToList();
            _words.AddRange(_zeros.Keys.ToList());
            _words.AddRange(_ones.Keys.ToList());
            _words.AddRange(_onesSuffixed.Keys.ToList());
            _words.AddRange(_tens.Keys.ToList());
            _words.AddRange(_tensSuffixed.Keys.ToList());
            _words.AddRange(_multipliers.Keys.ToList());
            _words.AddRange(_multipliersSuffixed.Keys.ToList());
            _words.AddRange(_precedingPrefixers.Keys.ToList());
            _words.AddRange(_followingPrefixers.Keys.ToList());
            _words.AddRange(_suffixers.Keys.ToList());
            _words.AddRange(_specials.ToList());
        }

        public string GetEnglishNumberNormalizer(string s)
        {
            s = PreProcess(s);
            IEnumerable<string> strs = ProcessWords(s.Split().Where(x => !string.IsNullOrEmpty(x)).ToList());
            List<string> xxx = new List<string>();
            foreach (string str in strs)
            {
                xxx.Add(str);
            }
            s = string.Join(" ", xxx);
            s = PostProcess(s);
            return s;
        }

        private IEnumerable<IEnumerable<string>> Windowed(IEnumerable<string> source, int windowSize)
        {
            var buffer = new Queue<string>();
            foreach (var item in source)
            {
                buffer.Enqueue(item);
                if (buffer.Count == windowSize)
                {
                    yield return buffer.ToList();
                    buffer.Dequeue();
                }
            }
        }
        public IEnumerable<string> ProcessWords(List<string> words)
        {
            string? prefix = null;
            string? value = null;
            bool skip = false;
            if (words.Count == 0)
            {
                yield return null;
            }
            // 创建窗口大小为3，且用0填充的枚举器
            words.Insert(0, null);
            words.Add(null);
            var windows = Windowed(words, 3);
            foreach (var item in windows)
            {
                if (skip)
                {
                    skip = false;
                    continue;
                }
                var window = item.ToArray();
                string? prev = window[0];
                string? current = window[1];
                string? next = window[2];
                bool nextIsNumeric = !string.IsNullOrEmpty(next) && Regex.IsMatch(next, @"^\d+(\.\d+)?$");
                bool hasPrefix = !string.IsNullOrEmpty(current) && _prefixes.Contains(current[0].ToString());
                string? currentWithoutPrefix = hasPrefix ? current.Substring(1) : current;
                if (currentWithoutPrefix != null && Regex.IsMatch(currentWithoutPrefix, @"^\d+(\.\d+)?$"))
                {
                    // arabic numbers (potentially with signs and fractions)
                    Fraction f = ToFraction(currentWithoutPrefix);
                    Debug.Assert(f != null);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value.EndsWith("."))
                        {
                            value = value + current;
                            continue;
                        }
                        else
                        {
                            yield return OutPut(result: value, prefix: ref prefix, value: ref value);
                        }
                    }
                    prefix = hasPrefix ? current[0].ToString() : prefix;
                    if (f != null && f.Denominator == 1)
                    {
                        value = f.Numerator.ToString();
                    }
                    else
                    {
                        value = currentWithoutPrefix;
                    }

                }
                else if (!words.Contains(current))
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        yield return OutPut(result: value, prefix: ref prefix, value: ref value);
                    }
                    yield return OutPut(result: current, prefix: ref prefix, value: ref value);
                }
                else if (_zeros.Keys.Contains(current))
                {
                    value = string.IsNullOrEmpty(value) ? "0" : value + "0";
                }
                else if (_ones.ContainsKey(current))
                {
                    int ones = _ones[current];
                    if (string.IsNullOrEmpty(value))
                    {
                        value = ones.ToString();
                    }
                    else if (!Regex.IsMatch(value, @"^\d+$") || _ones.ContainsKey(prev))
                    {
                        value = value.TrimEnd('▁');
                        if (_tens.ContainsKey(prev) && ones < 10)
                        {
                            Debug.Assert(value.Last().ToString() == "0");
                            value = value.Substring(0, value.Length - 2) + ones.ToString();
                        }
                        else
                        {
                            value = value.ToString() + ones.ToString();
                        }
                    }
                    else if (ones < 10)
                    {
                        if (int.Parse(value) % 10 == 0)
                        {
                            value = (int.Parse(value) + ones).ToString();
                        }
                        else
                        {
                            value = value.ToString() + ones.ToString();
                        }
                    }
                    else
                    {
                        if (int.Parse(value) % 100 == 0)
                        {
                            value = (int.Parse(value) + ones).ToString();
                        }
                        else
                        {
                            value = value.ToString() + ones.ToString();
                        }
                    }
                }
                else if (_onesSuffixed.ContainsKey(current))
                {
                    //ordinal or cardinal; yield the number right away
                    Tuple<int, string> onesSuffix = _onesSuffixed[current];
                    int ones = onesSuffix.Item1;
                    string suffix = onesSuffix.Item2;
                    if (string.IsNullOrEmpty(value))
                    {
                        yield return OutPut(ones.ToString() + suffix, ref prefix, ref value);
                    }
                    else if (!Regex.IsMatch(value, @"^\d+$") || _ones.ContainsKey(prev))
                    {
                        value = value.TrimEnd('▁');
                        if (_tens.ContainsKey(prev) && ones < 10)
                        {
                            Debug.Assert(value.Last().ToString() == "0");
                            yield return OutPut(value.Substring(0, value.Length - 2) + ones.ToString() + suffix, ref prefix, ref value);
                        }
                        else
                        {
                            yield return OutPut(value + ones.ToString() + suffix, ref prefix, ref value);
                        }
                    }
                    else if (ones < 10)
                    {
                        if (int.Parse(value) % 10 == 0)
                        {
                            yield return OutPut((int.Parse(value) + ones).ToString() + suffix, ref prefix, ref value);
                        }
                        else
                        {
                            yield return OutPut(value + ones.ToString() + suffix, ref prefix, ref value);
                        }
                    }
                    else // eleven to nineteen
                    {
                        if (int.Parse(value) % 100 == 0)
                        {
                            yield return OutPut((int.Parse(value) + ones).ToString() + suffix, ref prefix, ref value);
                        }
                        else
                        {
                            yield return OutPut(value + ones.ToString() + suffix, ref prefix, ref value);
                        }
                    }
                    value = null;
                }
                else if (_tens.ContainsKey(current))
                {
                    int tens = _tens[current];
                    if (value == null)
                    {
                        value = tens.ToString();
                    }
                    else if (!Regex.IsMatch(value, @"^\d+$"))
                    {
                        value = value.TrimEnd('▁');
                        value = value + tens.ToString();
                    }
                    else
                    {
                        if (int.Parse(value) % 100 == 0)
                        {
                            value = (int.Parse(value) + tens).ToString();
                        }
                        else
                        {
                            value = value + tens.ToString();
                        }
                    }
                }
                else if (_tensSuffixed.ContainsKey(current))
                {
                    //ordinal or cardinal; yield the number right away
                    Tuple<int, string> tensSuffix = _tensSuffixed[current];
                    int tens = tensSuffix.Item1;
                    string suffix = tensSuffix.Item2;
                    if (string.IsNullOrEmpty(value))
                    {
                        yield return OutPut(tens.ToString() + suffix, ref prefix, ref value);
                    }
                    else if (!Regex.IsMatch(value, @"^\d+$"))
                    {
                        value = value.TrimEnd('▁');
                        yield return OutPut(value + tens.ToString() + suffix, ref prefix, ref value);
                    }
                    else
                    {
                        if (int.Parse(value) % 100 == 0)
                        {
                            yield return OutPut((int.Parse(value) + tens).ToString() + suffix, ref prefix, ref value);
                        }
                        else
                        {
                            yield return OutPut(value + tens.ToString() + suffix, ref prefix, ref value);
                        }
                    }
                }
                else if (_multipliers.ContainsKey(current))
                {
                    BigInteger multiplier = _multipliers[current];
                    if (string.IsNullOrEmpty(value))
                    {
                        value = multiplier.ToString();
                    }
                    else if (!Regex.IsMatch(value, @"^\d+$") || int.Parse(value) == 0)
                    {
                        value = value.TrimEnd('▁');
                        Fraction f = ToFraction(value);
                        var p = f != null ? new Fraction(f.Numerator * multiplier, f.Denominator) : null;
                        if (p != null && p.Denominator == 1)
                        {
                            value = p.Numerator.ToString();
                        }
                        else
                        {
                            yield return OutPut(value, ref prefix, ref value);
                            value = multiplier.ToString();
                        }
                    }
                    else
                    {
                        int before = int.Parse(value) / 1000 * 1000;
                        int residual = int.Parse(value) % 1000;
                        value = (before + residual * multiplier).ToString();
                    }
                }
                else if (_multipliersSuffixed.ContainsKey(current))
                {
                    Tuple<BigInteger, string> tuple = _multipliersSuffixed[current];
                    BigInteger multiplier = tuple.Item1;
                    string suffix = tuple.Item2;
                    if (string.IsNullOrEmpty(value))
                    {
                        yield return OutPut(multiplier.ToString() + suffix, ref prefix, ref value);
                    }
                    else if (!Regex.IsMatch(value, @"^\d+$"))
                    {
                        value = value.TrimEnd('▁');
                        Fraction f = ToFraction(value);
                        var p = f != null ? new Fraction(f.Numerator * multiplier, f.Denominator) : null;
                        if (p != null && p.Denominator == 1)
                        {
                            //value = p.Numerator.ToString();
                            yield return OutPut(p.Numerator.ToString() + suffix, ref prefix, ref value);
                        }
                        else
                        {
                            yield return OutPut(value, ref prefix, ref value);
                            //value = multiplier.ToString();
                            yield return OutPut(multiplier.ToString() + suffix, ref prefix, ref value);
                        }
                    }
                    else //int
                    {
                        int before = int.Parse(value) / 1000 * 1000;
                        int residual = int.Parse(value) % 1000;
                        value = (before + residual * multiplier).ToString();
                        yield return OutPut(value + suffix, ref prefix, ref value);
                    }
                    value = null;
                }
                else if (_precedingPrefixers.ContainsKey(current))
                {
                    //apply prefix (positive, minus, etc.) if it precedes a number
                    if (!string.IsNullOrEmpty(value))
                    {
                        yield return OutPut(value, ref prefix, ref value);
                    }
                    if (_words.Contains(next) || nextIsNumeric)
                    {
                        prefix = _precedingPrefixers[current];
                    }
                    else
                    {
                        yield return OutPut(current, ref prefix, ref value);
                    }
                }
                else if (_followingPrefixers.ContainsKey(current))
                {
                    //apply prefix (dollars, cents, etc.) only after a number
                    if (!string.IsNullOrEmpty(value))
                    {
                        prefix = _followingPrefixers[current];
                        yield return OutPut(value, ref prefix, ref value);
                    }
                    else
                    {
                        yield return OutPut(current, ref prefix, ref value);
                    }
                }
                else if (_suffixers.ContainsKey(current))
                {
                    //apply suffix symbols (percent -> '%')
                    if (!string.IsNullOrEmpty(value))
                    {
                        string[] suffix = _suffixers[current];
                        if (suffix.Length == 2)
                        {
                            if (next == suffix[0])
                            {
                                yield return OutPut(value + suffix[1], ref prefix, ref value);
                                skip = true;
                            }
                            else
                            {
                                yield return OutPut(value, ref prefix, ref value);
                                yield return OutPut(current, ref prefix, ref value);
                            }
                        }
                        else
                        {
                            yield return OutPut(value + suffix[0], ref prefix, ref value);
                        }
                    }
                    else
                    {
                        yield return OutPut(current, ref prefix, ref value);
                    }
                }
                else if (_specials.Contains(current))
                {
                    if (!_words.Contains(next) && !nextIsNumeric)
                    {
                        //apply special handling only if the next word can be numeric
                        if (!string.IsNullOrEmpty(value))
                        {
                            yield return OutPut(value, ref prefix, ref value);
                        }
                        yield return OutPut(current, ref prefix, ref value);
                    }
                    else if (current == "and")
                    {
                        //ignore "and" after hundreds, thousands, etc.
                        if (!_multipliers.ContainsKey(prev))
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                yield return OutPut(value, ref prev, ref value);
                            }
                            yield return OutPut(current, ref prefix, ref value);
                        }
                    }
                    else if (current == "double" || current == "triple")
                    {
                        if (_ones.ContainsKey(next) || _zeros.ContainsKey(next))
                        {
                            int repeats = current == "double" ? 2 : 3;
                            int ones = 0;
                            _ones.TryGetValue(next, out ones);
                            value = (string.IsNullOrEmpty(value) ? "" : value) + string.Concat(Enumerable.Repeat(ones.ToString(), repeats)) + "▁";
                            skip = true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(value))
                            {
                                yield return OutPut(value, ref prefix, ref value);
                            }
                            yield return OutPut(current, ref prefix, ref value);
                        }
                    }
                    else if (current == "point")
                    {
                        if (_decimals.ContainsKey(next) || nextIsNumeric)
                        {
                            value = (string.IsNullOrEmpty(value) ? "" : value) + ".";
                        }
                    }
                    else
                    {
                        yield return OutPut(current, ref prefix, ref value);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!Regex.IsMatch(value, @"^\d+$"))
                        {
                            value = value.TrimEnd('▁');
                        }
                        yield return OutPut(result: value, prefix: ref prefix, value: ref value);
                    }
                    yield return OutPut(current, ref prefix, ref value);
                }
            }
            if (!string.IsNullOrEmpty(value))
            {
                if (!Regex.IsMatch(value, @"^\d+$"))
                {
                    value = value.TrimEnd('▁');
                }
                yield return OutPut(value, ref prefix, ref value);
            }
        }
        private Fraction ToFraction(string s)
        {
            try
            {
                Fraction f1 = new Fraction();
                f1.ParseDecimal(s);
                return f1;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string OutPut(string result, ref string? prefix, ref string? value)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                result = prefix + result;
            }
            value = null;
            prefix = null;
            return result;
        }
        private string PreProcess(string s)
        {
            // replace "<number> and a half" with "<number> point five"
            List<string> results = new List<string>();
            string[] segments = Regex.Split(s, @"\band\s+a\s+half\b");
            int i = 0;
            foreach (string segment in segments)
            {
                if (segment.Trim().Length == 0)
                {
                    continue;
                }
                if (i == segments.Length - 1)
                {
                    results.Add(segment);
                }
                else
                {
                    results.Add(segment);
                    string[] splitResult = segment.Split().Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    string last_word = splitResult.Length >= 2 ? splitResult[splitResult.Length - 1] : splitResult[0];
                    if (_decimals.Keys.Contains(last_word) || _multipliers.Keys.Contains(last_word))
                    {
                        results.Add("point five");
                    }
                    else
                    {
                        results.Add("and a half");
                    }
                }
                i++;
            }
            s = string.Join(" ", results);
            // put a space at number/letter boundary
            s = Regex.Replace(s, @"([a-z])([0-9])", "$1 $2");
            s = Regex.Replace(s, @"([0-9])([a-z])", "$1 $2");

            // but remove spaces which could be a suffix
            s = Regex.Replace(s, @"([0-9])\s+(st|nd|rd|th|s)\b", "$1$2");
            return s;
        }
        private static string CombineCents(Match match)
        {
            try
            {
                string currency = match.Groups[1].Value;
                string integer = match.Groups[2].Value;
                int cents = 0;
                int.TryParse(match.Groups[3].Value, out cents);
                return string.Format("{0}{1}.{2}", currency, integer, cents.ToString("D2"));
            }
            catch (Exception ex)
            {
                return match.ToString();
            }
        }

        private string ExtractCents(Match match)
        {
            try
            {
                int cents = 0;
                int.TryParse(match.Groups[1].Value, out cents);
                return string.Format("¢{0}", cents);
            }
            catch (Exception ex)
            {
                return match.ToString();
            }

        }
        private string PostProcess(string s)
        {
            s = new Regex(@"([€£$])([0-9]+) (?:and )?¢([0-9]{1,2})\b").Replace(s, new MatchEvaluator(CombineCents));
            s = new Regex(@"[€£$]0.([0-9]{1,2})\b").Replace(s, ExtractCents);
            // write "one(s)" instead of "1(s)", just for the readability
            s = Regex.Replace(s, @"\b1(s?)\b", "one$1");
            return s;
        }

    }
}
