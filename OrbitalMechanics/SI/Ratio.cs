using System;

namespace OrbitalMechanics.SI;

public readonly struct Ratio : IEquatable<Ratio>, IComparable<Ratio>
{
    public static readonly Ratio Zero = new Ratio( 0 );
    public static readonly Ratio One = new Ratio( 1 );

    private Ratio(double value)
    {
        Value = value;
    }

    public double Value { get; }

    public static Ratio Create(double value)
    {
        return new Ratio( value );
    }

    public static Ratio Normalize(double value)
    {
        return Create( value * 0.01 );
    }

    public Ratio Clamp(Ratio min, Ratio max)
    {
        return new Ratio( Math.Min( Math.Max( Value, min ), max ) );
    }

    public Ratio Abs()
    {
        return new Ratio( Math.Abs( Value ) );
    }

    public override string ToString()
    {
        return $"{Value:P6}";
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Ratio r && Equals( r );
    }

    public int CompareTo(object? obj)
    {
        return obj is Ratio r ? CompareTo( r ) : 1;
    }

    public bool Equals(Ratio other)
    {
        return Value.Equals( other.Value );
    }

    public int CompareTo(Ratio other)
    {
        return Value.CompareTo( other.Value );
    }

    public static implicit operator double(Ratio a)
    {
        return a.Value;
    }

    public static Ratio operator -(Ratio a)
    {
        return new Ratio( -a.Value );
    }

    public static Ratio operator +(Ratio a, Ratio b)
    {
        return new Ratio( a.Value + b.Value );
    }

    public static Ratio operator -(Ratio a, Ratio b)
    {
        return new Ratio( a.Value - b.Value );
    }

    public static Ratio operator *(Ratio a, double b)
    {
        return new Ratio( a.Value * b );
    }

    public static Ratio operator /(Ratio a, double b)
    {
        return new Ratio( a.Value / b );
    }

    public static bool operator ==(Ratio a, Ratio b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Ratio a, Ratio b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Ratio a, Ratio b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Ratio a, Ratio b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Ratio a, Ratio b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Ratio a, Ratio b)
    {
        return a.CompareTo( b ) < 0;
    }
}
