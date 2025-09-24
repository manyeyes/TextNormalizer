using Xunit;
using TextNormalizer;

namespace TextNormalizer.Tests;

public class NumberNormalizationTests
{
    // 注入文本归一化器（所有测试共用实例）
    private readonly EnglishTextNormalizer _textNormalizer = new();

    #region 基础数字转换
    [Fact]
    public void NumberNormalization_TextNumberWithSpace_ConvertsToDigit()
    {
        // 输入：带空格的文字数字
        var input = "five twenty five";
        // 预期：转换为连续数字
        var expected = "525";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NumberNormalization_OrdinalText_ConvertsToOrdinalDigit()
    {
        // 输入：序数词（如“第三十一天”）
        var input = "thirty first";
        var expected = "31st";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region 金额格式统一
    [Fact]
    public void NumberNormalization_CurrencyWithText_ConvertsToStandardFormat()
    {
        // 输入：文字描述的货币
        var input1 = "three euros and sixty five cents";
        var expected1 = "�3.65";

        var input2 = "14.1 million yuan";
        var expected2 = "14100000 yuan";

        var result1 = _textNormalizer.GetEnglishTextNormalizer(input1);
        var result2 = _textNormalizer.GetEnglishTextNormalizer(input2);

        Assert.Equal(expected1, result1);
        Assert.Equal(expected2, result2);
    }

    [Fact]
    public void NumberNormalization_CurrencyWithSymbol_KeepsStandardSymbol()
    {
        // 输入：带符号的金额（如$、�）
        var input = "$20 million";
        var expected = "$20000000";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region 日期格式统一
    [Fact]
    public void NumberNormalization_TextDate_ConvertsToStandardDate()
    {
        // 输入：文字描述的日期
        var input = "august twenty sixth twenty twenty one";
        var expected = "august 26th 2021";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region 特殊格式与干扰符处理
    [Fact]
    public void NumberNormalization_TextWithPunctuation_IgnoresInterference()
    {
        // 输入：带括号、逗号的干扰文本
        var input = "($14.1 million yuan ), three years ago";
        var expected = ", 3 years ago";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NumberNormalization_SpecialNumberFormat_ConvertsCorrectly()
    {
        // 输入：特殊格式（如“double zero seven”）
        var input1 = "double zero seven";
        var expected1 = "007";

        var input2 = "nine one one";
        var expected2 = "911";

        var result1 = _textNormalizer.GetEnglishTextNormalizer(input1);
        var result2 = _textNormalizer.GetEnglishTextNormalizer(input2);

        Assert.Equal(expected1, result1);
        Assert.Equal(expected2, result2);
    }
    #endregion
}