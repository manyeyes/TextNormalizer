using static System.Net.Mime.MediaTypeNames;

namespace TextNormalizer
{
    public class EnglishSpellingNormalizer
    {
        public static string applicationBase = AppDomain.CurrentDomain.BaseDirectory;
        //Applies British-American spelling mappings as listed in [1].
        //[1] https://www.tysto.com/uk-us-spelling-list.html
        private Dictionary<string, string> mapping = new Dictionary<string, string>(); 
        public EnglishSpellingNormalizer() {
            string mappingPath = applicationBase + "/normalizers/english.txt";
            mapping = new Dictionary<string, string>();
            if(File.Exists(mappingPath))
            {
                using (StreamReader reader = new StreamReader(mappingPath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] keyValue = line.Split('=');
                        if (keyValue.Length == 2)
                        {
                            mapping[keyValue[0]] = keyValue[1];
                        }
                    }
                }
            }
        }

        public string GetEnglishSpellingNormalizer(string text)
        {
            string[] textArr = text.Split();
            string normalizerText = string.Join(" ", textArr.Select(x=> mapping.ContainsKey(x) ? mapping.GetValueOrDefault(x) : x).ToArray());
            return normalizerText;
        }
    }
}
