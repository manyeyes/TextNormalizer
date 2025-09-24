using Xunit;
using TextNormalizer;

namespace TextNormalizer.Tests;

public class NumberNormalizationTests
{
    // ע���ı���һ���������в��Թ���ʵ����
    private readonly EnglishTextNormalizer _textNormalizer = new();

    #region ��������ת��
    [Fact]
    public void NumberNormalization_TextNumberWithSpace_ConvertsToDigit()
    {
        // ���룺���ո����������
        var input = "five twenty five";
        // Ԥ�ڣ�ת��Ϊ��������
        var expected = "525";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NumberNormalization_OrdinalText_ConvertsToOrdinalDigit()
    {
        // ���룺�����ʣ��硰����ʮһ�족��
        var input = "thirty first";
        var expected = "31st";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region ����ʽͳһ
    [Fact]
    public void NumberNormalization_CurrencyWithText_ConvertsToStandardFormat()
    {
        // ���룺���������Ļ���
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
        // ���룺�����ŵĽ���$�����
        var input = "$20 million";
        var expected = "$20000000";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region ���ڸ�ʽͳһ
    [Fact]
    public void NumberNormalization_TextDate_ConvertsToStandardDate()
    {
        // ���룺��������������
        var input = "august twenty sixth twenty twenty one";
        var expected = "august 26th 2021";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }
    #endregion

    #region �����ʽ����ŷ�����
    [Fact]
    public void NumberNormalization_TextWithPunctuation_IgnoresInterference()
    {
        // ���룺�����š����ŵĸ����ı�
        var input = "($14.1 million yuan ), three years ago";
        var expected = ", 3 years ago";

        var result = _textNormalizer.GetEnglishTextNormalizer(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NumberNormalization_SpecialNumberFormat_ConvertsCorrectly()
    {
        // ���룺�����ʽ���硰double zero seven����
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