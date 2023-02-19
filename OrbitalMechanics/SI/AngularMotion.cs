using System;

namespace OrbitalMechanics.SI;

public readonly struct AngularMotion : IEquatable<AngularMotion>, IComparable<AngularMotion>
{
    public static readonly AngularMotion Zero = new AngularMotion( 0 );
    private readonly double _radiansPerSecond;

    private AngularMotion(double radiansPerSecond)
    {
        _radiansPerSecond = radiansPerSecond;
    }

    public Angle PerSecond => GetAngle( Universe.OneSecond );
    public Angle PerHour => GetAngle( Universe.OneHour );

    public static AngularMotion CreatePerSecond(Angle angle)
    {
        return new AngularMotion( angle.Radians );
    }

    public static AngularMotion CreatePerHour(Angle angle)
    {
        return new AngularMotion( angle.Radians / Universe.OneHour.TotalSeconds );
    }

    public Angle GetAngle(TimeSpan dt)
    {
        return Angle.FromRadians( _radiansPerSecond * dt.TotalSeconds );
    }

    public AngularMotion Abs()
    {
        return new AngularMotion( Math.Abs( _radiansPerSecond ) );
    }

    public override string ToString()
    {
        var degreesPerSec = PerSecond.Degrees;
        var absDegreesPerSec = Math.Abs( degreesPerSec );

        if ( absDegreesPerSec >= 1000000 )
            return $"{degreesPerSec:E6} °/s";

        if ( absDegreesPerSec >= 0.01 )
            return $"{degreesPerSec:N3} °/s";

        if ( absDegreesPerSec == 0 )
            return "0 °/s";

        return $"{degreesPerSec:E6} °/s";
    }

    public override int GetHashCode()
    {
        return _radiansPerSecond.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is AngularMotion m && Equals( m );
    }

    public int CompareTo(object? obj)
    {
        return obj is AngularMotion m ? CompareTo( m ) : 1;
    }

    public bool Equals(AngularMotion other)
    {
        return _radiansPerSecond.Equals( other._radiansPerSecond );
    }

    public int CompareTo(AngularMotion other)
    {
        return _radiansPerSecond.CompareTo( other._radiansPerSecond );
    }

    public static AngularMotion operator -(AngularMotion a)
    {
        return new AngularMotion( -a._radiansPerSecond );
    }

    public static AngularMotion operator +(AngularMotion a, AngularMotion b)
    {
        return new AngularMotion( a._radiansPerSecond + b._radiansPerSecond );
    }

    public static AngularMotion operator -(AngularMotion a, AngularMotion b)
    {
        return new AngularMotion( a._radiansPerSecond - b._radiansPerSecond );
    }

    public static AngularMotion operator *(AngularMotion a, double b)
    {
        return new AngularMotion( a._radiansPerSecond * b );
    }

    public static Angle operator *(AngularMotion a, TimeSpan b)
    {
        return a.GetAngle( b );
    }

    public static Angle operator *(TimeSpan a, AngularMotion b)
    {
        return b.GetAngle( a );
    }

    public static AngularMotion operator /(AngularMotion a, double b)
    {
        return new AngularMotion( a._radiansPerSecond / b );
    }

    public static Ratio operator /(AngularMotion a, AngularMotion b)
    {
        return Ratio.Create( a._radiansPerSecond / b._radiansPerSecond );
    }

    public static bool operator ==(AngularMotion a, AngularMotion b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(AngularMotion a, AngularMotion b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(AngularMotion a, AngularMotion b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(AngularMotion a, AngularMotion b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(AngularMotion a, AngularMotion b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(AngularMotion a, AngularMotion b)
    {
        return a.CompareTo( b ) < 0;
    }
}
