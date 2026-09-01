// Fraction.cs
// Fraction object that stores a numerator and denominator
// 31 August 2026
// will b. gaming

using System;
using UnityEngine;

public class Fraction
{
    // Greatest common denominator
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
    // Least common multiple
    public static int LCM(int x, int y)
    {
        return Mathf.Abs(x * y / GCD(x, y));
    }

    // Arithmetic operators
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
    public static Fraction operator *(Fraction fraction1, Fraction fraction2)
    {
        return new Fraction(fraction1.numerator * fraction2.numerator, fraction1.denominator * fraction2.denominator);
    }
    public static Fraction operator /(Fraction fraction1, Fraction fraction2)
    {
        return fraction1 * Reciprocal(fraction2);
    }
    public static Fraction operator %(Fraction fraction1, Fraction fraction2)
    {
        int i = 0;
        while (abs(fraction1) >= abs(fraction2 * i))
        {
            i++;
        }
        return fraction1 - fraction2 * (i - 1);
    }
    public static Fraction operator -(Fraction fraction)
    {
        return new Fraction(-fraction.numerator, fraction.denominator);
    }
    public static Fraction Reciprocal(Fraction fraction)
    {
        return new Fraction(fraction.denominator, fraction.numerator);
    }
    public static Fraction abs(Fraction fraction)
    {
        if (fraction < 0)
        {
            return fraction * -1;
        }
        return fraction;
    }

    // Comparison operators
    public static bool operator ==(Fraction fraction1, Fraction fraction2)
    {
        return (!(fraction1 > fraction2) && !(fraction2 > fraction1));
    }
    public static bool operator !=(Fraction fraction1, Fraction fraction2)
    {
        return !(fraction1 == fraction2);
    }
    public static bool operator >(Fraction fraction1, Fraction fraction2)
    {
        int denominator = LCM(fraction1.denominator, fraction2.denominator);
        return (fraction1.numerator * denominator / fraction1.denominator > fraction2.numerator * denominator / fraction2.denominator);
    }
    public static bool operator >=(Fraction fraction1, Fraction fraction2)
    {
        return !(fraction1 < fraction2);
    }
    public static bool operator <(Fraction fraction1, Fraction fraction2)
    {
        return fraction2 > fraction1;
    }
    public static bool operator <=(Fraction fraction1, Fraction fraction2)
    {
        return !(fraction1 > fraction2);
    }

    // Constructor
    public Fraction(int numerator, int denominator=1)
    {
        this.numerator = numerator;
        this.denominator = denominator;
        if (denominator == 0)
            denominator = 1;
        Simplify();
    }
    public Fraction(Fraction fraction)
    {
        numerator = fraction.numerator;
        denominator = fraction.denominator;
    }
    // Implicit Conversion from int to Fraction
    public static implicit operator Fraction(int numerator)
    {
        return new Fraction(numerator);
    }
    // Mutators
    public void SetValue(int n, int d)
    {
        numerator = n;
        denominator = d;
        if (denominator == 0)
        {
            denominator = 1;
        }
        Simplify();
    }
    void Simplify()
    {
        int gcd = GCD(numerator, denominator);
        numerator /= gcd;
        denominator /= gcd;
        // Enforces the rule that if fraction is negative, only the numerator is negative
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }
    }
    // Accessors
    public int GetNumerator()
    {
        return numerator;
    }
    public int GetDenominator()
    {
        return denominator;
    }
    public double GetDouble()
    {
        return (double)numerator/denominator;
    }

    public override bool Equals(object obj)
    {
        return obj is Fraction fraction &&
               numerator == fraction.numerator &&
               denominator == fraction.denominator;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(numerator, denominator);
    }

    public override string ToString()
    {
        if (denominator == 1)
            return numerator.ToString();
        return numerator + "/" + denominator;
    }

    int numerator;
    int denominator;
}
