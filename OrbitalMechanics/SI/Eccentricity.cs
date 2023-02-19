using System;

namespace OrbitalMechanics.SI;

public readonly struct Eccentricity : IEquatable<Eccentricity>, IComparable<Eccentricity>
{
    public static readonly Eccentricity Circular = new Eccentricity( 0 );
    public static readonly Eccentricity Parabolic = new Eccentricity( 1 );

    private Eccentricity(double value)
    {
        if ( value < 0 )
            throw new ArgumentException( "Eccentricity cannot be less than 0.", nameof( value ) );

        Value = value;
    }

    public double Value { get; }
    public bool IsCircular => Value.Equals( 0.0 );
    public bool IsElliptic => Value.CompareTo( 0.0 ) >= 0 && Value.CompareTo( 1.0 ) < 0;
    public bool IsParabolic => Value.Equals( 1.0 );
    public bool IsHyperbolic => Value.CompareTo( 1.0 ) > 0;

    public static Eccentricity Create(double value)
    {
        return new Eccentricity( value );
    }

    public override string ToString()
    {
        if ( Value >= 0.0001 )
            return $"{Value:N6}";

        if ( Value == 0 )
            return "0";

        return $"{Value:E6}";
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Eccentricity e && Equals( e );
    }

    public int CompareTo(object? obj)
    {
        return obj is Eccentricity e ? CompareTo( e ) : 1;
    }

    public bool Equals(Eccentricity other)
    {
        return Value.Equals( other.Value );
    }

    public int CompareTo(Eccentricity other)
    {
        return Value.CompareTo( other.Value );
    }

    public static bool operator ==(Eccentricity a, Eccentricity b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Eccentricity a, Eccentricity b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Eccentricity a, Eccentricity b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Eccentricity a, Eccentricity b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Eccentricity a, Eccentricity b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Eccentricity a, Eccentricity b)
    {
        return a.CompareTo( b ) < 0;
    }
}
