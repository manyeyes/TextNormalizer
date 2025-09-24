using System.Numerics;
using System.Text.RegularExpressions;
using Rationals;

namespace TextNormalizer
{
    public class Fraction
    {
        public BigInteger Numerator { get; private set; }
        public BigInteger Denominator { get; private set; }

        // 构造函数  
        public Fraction()
        {
        }
        // 构造函数，从字符串中解析分数  
        public Fraction(string fractionStr)
        {
            // 使用正则表达式来匹配分数格式  
            Match match = Regex.Match(fractionStr, @"^(\d+)/(\d+)$");
            if (!match.Success)
            {
                throw new ArgumentException("Invalid fraction format. Expected 'numerator/denominator'.");
            }

            Numerator = int.Parse(match.Groups[1].Value);
            Denominator = int.Parse(match.Groups[2].Value);

            // 简化分数  
            Simplify();
        }

        // 构造函数，允许直接通过分子和分母创建分数（可选）  
        public Fraction(BigInteger numerator, BigInteger denominator)//int denominator = 1
        {
            if (denominator == 0)
                throw new ArgumentException("Denominator cannot be zero.");

            Numerator = numerator;
            Denominator = denominator;

            // 简化分数  
            Simplify();
        }

        /// <summary>
        /// e.g. var p3 = Rational.ParseDecimal("1.4"); // 7/5
        /// </summary>
        /// <param name="str"></param>
        public void ParseDecimal(string str)
        {
            var rational = Rational.ParseDecimal(str);
            Numerator = rational.Numerator;
            Denominator = rational.Denominator;
        }

        /// <summary>
        /// e.g. var p1 = Rational.Parse("7/5");        // 7/5
        /// e.g. var p2 = Rational.Parse("1 2/5");      // 7/5
        /// </summary>
        /// <param name="str"></param>
        public void Parse(string str)
        {
            var rational = Rational.Parse(str);
            Numerator = rational.Numerator;
            Denominator = rational.Denominator;
        }

        private void Simplify()
        {
            BigInteger gcd = GreatestCommonDivisor(BigInteger.Abs(Numerator), BigInteger.Abs(Denominator));
            Numerator /= gcd;
            Denominator /= gcd;

            // 确保分母为正  
            if (Denominator < 0)
            {
                Numerator = -Numerator;
                Denominator = -Denominator;
            }
        }


        // 辗转相除法求最大公约数  
        private BigInteger GreatestCommonDivisor(BigInteger a, BigInteger b)
        {
            while (b != 0)
            {
                BigInteger temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        // 其他方法：加法、减法、乘法、除法等（此处省略）  

        // 重写ToString方法以便于打印  
        public override string ToString()
        {
            if (Denominator == 1)
                return Numerator.ToString();
            return $"{Numerator}/{Denominator}";
        }
    }
}
