using Xunit;
using TextNormalizer;

namespace TextNormalizer.Tests;

public class TextNormalizationTests
{
    private readonly EnglishTextNormalizer _textNormalizer = new();

    #region 头衔与缩写展开
    [Fact]
    public void TextNormalization_TitleAbbreviation_ExpandsToFullText()
    {
        // 输入：头衔缩写（Mr.、Assoc. Prof.、Jr.）
        var input = "Mr. Park visited Assoc. Prof. Kim Jr.";
        var expected = "mister park visited associate professor kim junior";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region 所有格与Contractions解析
    [Fact]
    public void TextNormalization_Contraction_ExpandsToFullForm()
    {
        // 输入：所有格（Chagee's）、Contractions（He's、Let's）
        var input1 = "Chagee's founder said";
        var expected1 = "chagee is founder said";

        var input2 = "He's like 'Let's go'";
        var expected2 = "he is like'let us go'";

        var result1 = _textNormalizer.GetEnglishTextNormalizer(input1);
        var result2 = _textNormalizer.GetEnglishTextNormalizer(input2);

        Assert.Equal(expected1, result1);
        Assert.Equal(expected2, result2);
    }
    #endregion

    #region 标点与冗余字符清理
    [Fact]
    public void TextNormalization_RedundantPunctuation_CleansUp()
    {
        // 输入：多逗号、多余括号干扰
        var input = "100 million yuan,,,, [$14.1 million yuan],,,";
        var expected = "100000000 yuan, ";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region 大小写统一
    [Fact]
    public void TextNormalization_UpperCaseText_ConvertsToLowerCase()
    {
        // 输入：首字母大写文本
        var input = "By the end of the year, Among its most successful innovations";
        var expected = "by the end of the year, among its most successful innovations";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion
}