using System;

namespace OrbitalMechanics.SI;

public readonly struct Mass : IEquatable<Mass>, IComparable<Mass>
{
    public static readonly Mass Zero = new Mass( 0 );

    private Mass(double kilograms)
    {
        Kilograms = kilograms;
    }

    public double Kilograms { get; }
    public double Tons => Kilograms / 1000;
    public double Grams => Kilograms * 1000;

    public static Mass FromTons(double value)
    {
        return new Mass( value * 1000 );
    }

    public static Mass FromKilograms(double value)
    {
        return new Mass( value );
    }

    public static Mass FromGrams(double value)
    {
        return new Mass( value / 1000 );
    }

    public Force GetForce(Acceleration acceleration)
    {
        return Force.FromNewtons( Kilograms * acceleration.PerSecond.PerSecond.Meters );
    }

    public MassFlowRate GetFlowRate(TimeSpan dt)
    {
        return MassFlowRate.CreatePerSecond( this ) / dt.TotalSeconds;
    }

    public TimeSpan GetTime(MassFlowRate flowRate)
    {
        return TimeSpan.FromSeconds( Kilograms / flowRate.PerSecond.Kilograms );
    }

    public Mass Abs()
    {
        return new Mass( Math.Abs( Kilograms ) );
    }

    public override string ToString()
    {
        var absKg = Math.Abs( Kilograms );

        if ( absKg >= 1000000000 )
            return $"{Kilograms:E6} kg";

        if ( absKg >= 100000 )
            return $"{Tons:N3} t";

        if ( absKg >= 1 )
            return $"{Kilograms:N3} kg";

        if ( absKg >= 0.0001 )
            return $"{Grams:N3} g";

        if ( absKg == 0 )
            return "0 kg";

        return $"{Kilograms:E6} kg";
    }

    public override int GetHashCode()
    {
        return Kilograms.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Mass m && Equals( m );
    }

    public int CompareTo(object? obj)
    {
        return obj is Mass m ? CompareTo( m ) : 1;
    }

    public bool Equals(Mass other)
    {
        return Kilograms.Equals( other.Kilograms );
    }

    public int CompareTo(Mass other)
    {
        return Kilograms.CompareTo( other.Kilograms );
    }

    public static Mass operator -(Mass a)
    {
        return new Mass( -a.Kilograms );
    }

    public static Mass operator +(Mass a, Mass b)
    {
        return new Mass( a.Kilograms + b.Kilograms );
    }

    public static Mass operator -(Mass a, Mass b)
    {
        return new Mass( a.Kilograms - b.Kilograms );
    }

    public static Mass operator *(Mass a, double b)
    {
        return new Mass( a.Kilograms * b );
    }

    public static Force operator *(Mass a, Acceleration b)
    {
        return a.GetForce( b );
    }

    public static Mass operator /(Mass a, double b)
    {
        return new Mass( a.Kilograms / b );
    }

    public static Ratio operator /(Mass a, Mass b)
    {
        return Ratio.Create( a.Kilograms / b.Kilograms );
    }

    public static MassFlowRate operator /(Mass a, TimeSpan b)
    {
        return a.GetFlowRate( b );
    }

    public static TimeSpan operator /(Mass a, MassFlowRate b)
    {
        return a.GetTime( b );
    }

    public static bool operator ==(Mass a, Mass b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Mass a, Mass b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Mass a, Mass b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Mass a, Mass b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Mass a, Mass b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Mass a, Mass b)
    {
        return a.CompareTo( b ) < 0;
    }
}
