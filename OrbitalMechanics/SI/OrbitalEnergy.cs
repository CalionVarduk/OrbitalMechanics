using System;

namespace OrbitalMechanics.SI;

public readonly struct OrbitalEnergy : IEquatable<OrbitalEnergy>, IComparable<OrbitalEnergy>
{
    public static readonly OrbitalEnergy Zero = new OrbitalEnergy( 0 );

    private readonly double _squareMetersPerSquareSecond;

    private OrbitalEnergy(double squareMetersPerSquareSecond)
    {
        _squareMetersPerSquareSecond = squareMetersPerSquareSecond;
    }

    public AngularMomentum PerSecond => GetMomentum( Universe.OneSecond );
    public AngularMomentum PerHour => GetMomentum( Universe.OneHour );

    public static OrbitalEnergy CreatePerSecond(AngularMomentum momentum)
    {
        return new OrbitalEnergy( momentum.PerSecond.SquareMeters );
    }

    public static OrbitalEnergy CreatePerHour(AngularMomentum momentum)
    {
        return new OrbitalEnergy( momentum.PerSecond.SquareMeters / Universe.OneHour.TotalSeconds );
    }

    public static OrbitalEnergy CreatePerSquareSecond(Area area)
    {
        return CreatePerSecond( AngularMomentum.CreatePerSecond( area ) );
    }

    public static OrbitalEnergy CreatePerSquareHour(Area area)
    {
        return CreatePerHour( AngularMomentum.CreatePerHour( area ) );
    }

    public Area GetArea(TimeSpan dt)
    {
        return Area.FromSquareMeters( 0.5 * _squareMetersPerSquareSecond * Math.Pow( dt.TotalSeconds, 2 ) );
    }

    public GravitationalParameter GetGravitationalParameter(Distance distance)
    {
        return GravitationalParameter.Create( _squareMetersPerSquareSecond * distance.Meters );
    }

    public AngularMomentum GetMomentum(TimeSpan dt)
    {
        return AngularMomentum.CreatePerSecond( Area.FromSquareMeters( _squareMetersPerSquareSecond * dt.TotalSeconds ) );
    }

    public Distance GetDistance(Acceleration acceleration)
    {
        return Distance.FromMeters( _squareMetersPerSquareSecond / acceleration.PerSecond.PerSecond.Meters );
    }

    public Acceleration GetAcceleration(Distance distance)
    {
        return Acceleration.CreatePerSquareSecond( Distance.FromMeters( _squareMetersPerSquareSecond / distance.Meters ) );
    }

    public Velocity GetVelocity(Velocity velocity)
    {
        return Velocity.CreatePerSecond( Distance.FromMeters( _squareMetersPerSquareSecond / velocity.PerSecond.Meters ) );
    }

    public Velocity Sqrt()
    {
        return Velocity.CreatePerSecond( Distance.FromMeters( Math.Sqrt( _squareMetersPerSquareSecond ) ) );
    }

    public OrbitalEnergy Abs()
    {
        return new OrbitalEnergy( Math.Abs( _squareMetersPerSquareSecond ) );
    }

    public override string ToString()
    {
        var absSqMetersPerSqSec = Math.Abs( _squareMetersPerSquareSecond );

        if ( absSqMetersPerSqSec >= 1000000 )
            return $"{_squareMetersPerSquareSecond:E6} m^2/s^2";

        if ( absSqMetersPerSqSec >= 0.01 )
            return $"{_squareMetersPerSquareSecond:N3} m^2/s^2";

        if ( absSqMetersPerSqSec == 0 )
            return "0 m^2/s^2";

        return $"{_squareMetersPerSquareSecond:E6} m^2/s^2";
    }

    public override int GetHashCode()
    {
        return _squareMetersPerSquareSecond.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is OrbitalEnergy e && Equals( e );
    }

    public int CompareTo(object? obj)
    {
        return obj is OrbitalEnergy e ? CompareTo( e ) : 1;
    }

    public bool Equals(OrbitalEnergy other)
    {
        return _squareMetersPerSquareSecond.Equals( other._squareMetersPerSquareSecond );
    }

    public int CompareTo(OrbitalEnergy other)
    {
        return _squareMetersPerSquareSecond.CompareTo( other._squareMetersPerSquareSecond );
    }

    public static OrbitalEnergy operator -(OrbitalEnergy a)
    {
        return new OrbitalEnergy( -a._squareMetersPerSquareSecond );
    }

    public static OrbitalEnergy operator +(OrbitalEnergy a, OrbitalEnergy b)
    {
        return new OrbitalEnergy( a._squareMetersPerSquareSecond + b._squareMetersPerSquareSecond );
    }

    public static OrbitalEnergy operator -(OrbitalEnergy a, OrbitalEnergy b)
    {
        return new OrbitalEnergy( a._squareMetersPerSquareSecond - b._squareMetersPerSquareSecond );
    }

    public static OrbitalEnergy operator *(OrbitalEnergy a, double b)
    {
        return new OrbitalEnergy( a._squareMetersPerSquareSecond * b );
    }

    public static GravitationalParameter operator *(OrbitalEnergy a, Distance b)
    {
        return a.GetGravitationalParameter( b );
    }

    public static AngularMomentum operator *(OrbitalEnergy a, TimeSpan b)
    {
        return a.GetMomentum( b );
    }

    public static AngularMomentum operator *(TimeSpan a, OrbitalEnergy b)
    {
        return b.GetMomentum( a );
    }

    public static OrbitalEnergy operator /(OrbitalEnergy a, double b)
    {
        return new OrbitalEnergy( a._squareMetersPerSquareSecond / b );
    }

    public static Ratio operator /(OrbitalEnergy a, OrbitalEnergy b)
    {
        return Ratio.Create( a._squareMetersPerSquareSecond / b._squareMetersPerSquareSecond );
    }

    public static Distance operator /(OrbitalEnergy a, Acceleration b)
    {
        return a.GetDistance( b );
    }

    public static Acceleration operator /(OrbitalEnergy a, Distance b)
    {
        return a.GetAcceleration( b );
    }

    public static Velocity operator /(OrbitalEnergy a, Velocity b)
    {
        return a.GetVelocity( b );
    }

    public static bool operator ==(OrbitalEnergy a, OrbitalEnergy b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(OrbitalEnergy a, OrbitalEnergy b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(OrbitalEnergy a, OrbitalEnergy b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(OrbitalEnergy a, OrbitalEnergy b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(OrbitalEnergy a, OrbitalEnergy b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(OrbitalEnergy a, OrbitalEnergy b)
    {
        return a.CompareTo( b ) < 0;
    }
}
