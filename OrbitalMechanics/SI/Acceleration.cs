using System;

namespace OrbitalMechanics.SI;

public readonly struct Acceleration : IEquatable<Acceleration>, IComparable<Acceleration>
{
    public static readonly Acceleration Zero = new Acceleration( 0 );

    private readonly double _metersPerSqSecond;

    private Acceleration(double metersPerSqSecond)
    {
        _metersPerSqSecond = metersPerSqSecond;
    }

    public Velocity PerSecond => GetVelocity( Universe.OneSecond );
    public Velocity PerHour => GetVelocity( Universe.OneHour );

    public static Acceleration CreatePerSecond(Velocity velocity)
    {
        return new Acceleration( velocity.PerSecond.Meters );
    }

    public static Acceleration CreatePerHour(Velocity velocity)
    {
        return new Acceleration( velocity.PerSecond.Meters / Universe.OneHour.TotalSeconds );
    }

    public static Acceleration CreatePerSquareSecond(Distance distance)
    {
        return CreatePerSecond( Velocity.CreatePerSecond( distance ) );
    }

    public static Acceleration CreatePerSquareHour(Distance distance)
    {
        return CreatePerHour( Velocity.CreatePerHour( distance ) );
    }

    public Distance GetDistance(TimeSpan dt)
    {
        return Distance.FromMeters( 0.5 * _metersPerSqSecond * Math.Pow( dt.TotalSeconds, 2 ) );
    }

    public Velocity GetVelocity(TimeSpan dt)
    {
        return Velocity.CreatePerSecond( Distance.FromMeters( _metersPerSqSecond * dt.TotalSeconds ) );
    }

    public OrbitalEnergy GetEnergy(Distance distance)
    {
        return OrbitalEnergy.CreatePerSquareSecond( Area.FromSquareMeters( _metersPerSqSecond * distance.Meters ) );
    }

    public Force GetForce(Mass mass)
    {
        return Force.FromNewtons( _metersPerSqSecond * mass.Kilograms );
    }

    public GravitationalParameter GetGravitationalParameter(Area area)
    {
        return GravitationalParameter.Create( _metersPerSqSecond * area.SquareMeters );
    }

    public Acceleration Abs()
    {
        return new Acceleration( Math.Abs( _metersPerSqSecond ) );
    }

    public override string ToString()
    {
        var absMetersPerSqSec = Math.Abs( _metersPerSqSecond );

        if ( absMetersPerSqSec >= 10000 )
            return $"{PerSecond.PerSecond.Kilometers:N3} km/s^2";

        if ( absMetersPerSqSec >= 0.01 )
            return $"{_metersPerSqSecond:N3} m/s^2";

        if ( absMetersPerSqSec == 0 )
            return "0 m/s^2";

        return $"{_metersPerSqSecond:E6} m/s^2";
    }

    public override int GetHashCode()
    {
        return _metersPerSqSecond.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Acceleration a && Equals( a );
    }

    public int CompareTo(object? obj)
    {
        return obj is Acceleration a ? CompareTo( a ) : 1;
    }

    public bool Equals(Acceleration other)
    {
        return _metersPerSqSecond.Equals( other._metersPerSqSecond );
    }

    public int CompareTo(Acceleration other)
    {
        return _metersPerSqSecond.CompareTo( other._metersPerSqSecond );
    }

    public static Acceleration operator -(Acceleration a)
    {
        return new Acceleration( -a._metersPerSqSecond );
    }

    public static Acceleration operator +(Acceleration a, Acceleration b)
    {
        return new Acceleration( a._metersPerSqSecond + b._metersPerSqSecond );
    }

    public static Acceleration operator -(Acceleration a, Acceleration b)
    {
        return new Acceleration( a._metersPerSqSecond - b._metersPerSqSecond );
    }

    public static Acceleration operator *(Acceleration a, double b)
    {
        return new Acceleration( a._metersPerSqSecond * b );
    }

    public static Velocity operator *(Acceleration a, TimeSpan b)
    {
        return a.GetVelocity( b );
    }

    public static Velocity operator *(TimeSpan a, Acceleration b)
    {
        return b.GetVelocity( a );
    }

    public static OrbitalEnergy operator *(Acceleration a, Distance b)
    {
        return a.GetEnergy( b );
    }

    public static Force operator *(Acceleration a, Mass b)
    {
        return a.GetForce( b );
    }

    public static GravitationalParameter operator *(Acceleration a, Area b)
    {
        return a.GetGravitationalParameter( b );
    }

    public static Acceleration operator /(Acceleration a, double b)
    {
        return new Acceleration( a._metersPerSqSecond / b );
    }

    public static Ratio operator /(Acceleration a, Acceleration b)
    {
        return Ratio.Create( a._metersPerSqSecond / b._metersPerSqSecond );
    }

    public static bool operator ==(Acceleration a, Acceleration b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Acceleration a, Acceleration b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Acceleration a, Acceleration b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Acceleration a, Acceleration b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Acceleration a, Acceleration b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Acceleration a, Acceleration b)
    {
        return a.CompareTo( b ) < 0;
    }
}
