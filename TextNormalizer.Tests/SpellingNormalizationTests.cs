using Xunit;
using TextNormalizer;

namespace TextNormalizer.Tests;

public class SpellingNormalizationTests
{
    private readonly EnglishSpellingNormalizer _spellingNormalizer = new();

    [Fact]
    public void SpellingNormalization_BritishToAmerican_UnifiesSpelling()
    {
        // 输入：英式拼写，预期：美式拼写
        var testCases = new Dictionary<string, string>
        {
            { "mobilisation", "mobilization" },
            { "cancelation", "cancellation" },
            { "favourite", "favorite" },
            { "colour", "color" },
            { "organise", "organize" }
        };

        foreach (var (input, expected) in testCases)
        {
            var result = _spellingNormalizer.GetEnglishSpellingNormalizer(input);
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void SpellingNormalization_AmericanSpelling_KeepsUnchanged()
    {
        // 输入：美式拼写，预期：保持不变
        var input = "mobilization, cancellation, favorite";
        var expected = "mobilization, cancellation, favorite";

        var result = _spellingNormalizer.GetEnglishSpellingNormalizer(input);
        Assert.Equal(expected, result);
    }
}