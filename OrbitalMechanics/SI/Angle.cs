using System;

namespace OrbitalMechanics.SI;

public readonly struct Angle : IEquatable<Angle>, IComparable<Angle>
{
    public static readonly Angle Zero = new Angle( 0 );
    public static readonly Angle Half = new Angle( Math.PI );
    public static readonly Angle Full = new Angle( Math.Tau );

    private Angle(double radians)
    {
        Radians = radians;
    }

    public double Radians { get; }
    public double Degrees => 180 / Math.PI * Radians;

    public static Angle FromRadians(double value)
    {
        return new Angle( value );
    }

    public static Angle FromDegrees(double value)
    {
        return new Angle( Math.PI / 180 * value );
    }

    public AngularMotion GetMotion(TimeSpan dt)
    {
        return AngularMotion.CreatePerSecond( this ) / dt.TotalSeconds;
    }

    public TimeSpan GetTime(AngularMotion motion)
    {
        return TimeSpan.FromSeconds( Radians / motion.PerSecond.Radians );
    }

    public Angle Normalize()
    {
        return new Angle( Radians % Full.Radians );
    }

    public Angle NormalizePositive()
    {
        var result = Normalize();
        if ( result < Zero )
            result += Full;

        return result;
    }

    public Angle Abs()
    {
        return new Angle( Math.Abs( Radians ) );
    }

    public double Sin()
    {
        return Math.Sin( Radians );
    }

    public double Cos()
    {
        return Math.Cos( Radians );
    }

    public double Sinh()
    {
        return Math.Sinh( Radians );
    }

    public double Cosh()
    {
        return Math.Cosh( Radians );
    }

    public override string ToString()
    {
        var degrees = Degrees;
        var absDegrees = Math.Abs( degrees );

        if ( absDegrees >= 1000000 )
            return $"{degrees:E6} °";

        if ( absDegrees >= 0.01 )
            return $"{degrees:N3} °";

        if ( absDegrees == 0 )
            return "0 °";

        return $"{degrees:E6} °";
    }

    public override int GetHashCode()
    {
        return Radians.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Angle a && Equals( a );
    }

    public int CompareTo(object? obj)
    {
        return obj is Angle a ? CompareTo( a ) : 1;
    }

    public bool Equals(Angle other)
    {
        return Radians.Equals( other.Radians );
    }

    public int CompareTo(Angle other)
    {
        return Radians.CompareTo( other.Radians );
    }

    public static Angle operator -(Angle a)
    {
        return new Angle( -a.Radians );
    }

    public static Angle operator +(Angle a, Angle b)
    {
        return new Angle( a.Radians + b.Radians );
    }

    public static Angle operator -(Angle a, Angle b)
    {
        return new Angle( a.Radians - b.Radians );
    }

    public static Angle operator *(Angle a, double b)
    {
        return new Angle( a.Radians * b );
    }

    public static Angle operator /(Angle a, double b)
    {
        return new Angle( a.Radians / b );
    }

    public static Ratio operator /(Angle a, Angle b)
    {
        return Ratio.Create( a.Radians / b.Radians );
    }

    public static AngularMotion operator /(Angle a, TimeSpan b)
    {
        return a.GetMotion( b );
    }

    public static TimeSpan operator /(Angle a, AngularMotion b)
    {
        return a.GetTime( b );
    }

    public static Angle operator %(Angle a, Angle b)
    {
        return new Angle( a.Radians % b.Radians );
    }

    public static bool operator ==(Angle a, Angle b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Angle a, Angle b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Angle a, Angle b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Angle a, Angle b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Angle a, Angle b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Angle a, Angle b)
    {
        return a.CompareTo( b ) < 0;
    }
}
