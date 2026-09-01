// Fraction.cs
// Fraction object that stores a numerator and denominator
// 31 August 2026
// will b. gaming

using UnityEngine;

public class Fraction
{
    public static int GCD(int x, int y)
    {
        int greater = Mathf.Abs(x);
        int lesser = Mathf.Abs(y);
        if (greater < lesser)
        {
            lesser = Mathf.Abs(x);
            greater = Mathf.Abs(y);
        }
        if (greater == 0)
        {
            return 1;
        }
        if (lesser == 0)
        {
            return greater;
        }
        return GCD(lesser, greater % lesser);
    }
    public static int LCM(int x, int y)
    {
        return Mathf.Abs(x * y / GCD(x, y));
    }

    public static Fraction operator +(Fraction fraction1, Fraction fraction2)
    {
        int denominator = LCM(fraction1.denominator, fraction2.denominator);
        int numerator = (denominator * fraction1.numerator / fraction1.denominator) + (denominator * fraction2.numerator / fraction2.denominator);
        return new Fraction(numerator, denominator);
    }
    public static Fraction operator -(Fraction fraction1, Fraction fraction2)
    {
        return fraction1 + -fraction2;
    }

    int numerator;
    int denominator;
}
