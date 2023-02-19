using System;

namespace OrbitalMechanics.SI;

public readonly struct AngularMomentum : IEquatable<AngularMomentum>, IComparable<AngularMomentum>
{
    public static readonly AngularMomentum Zero = new AngularMomentum( 0 );

    private readonly double _squareMetersPerSecond;

    private AngularMomentum(double squareMetersPerSecond)
    {
        _squareMetersPerSecond = squareMetersPerSecond;
    }

    public Area PerSecond => GetArea( Universe.OneSecond );
    public Area PerHour => GetArea( Universe.OneHour );

    public static AngularMomentum CreatePerSecond(Area area)
    {
        return new AngularMomentum( area.SquareMeters );
    }

    public static AngularMomentum CreatePerHour(Area area)
    {
        return new AngularMomentum( area.SquareMeters / Universe.OneHour.TotalSeconds );
    }

    public GravitationalParameter GetGravitationalParameter(Velocity velocity)
    {
        return GravitationalParameter.Create( _squareMetersPerSecond * velocity.PerSecond.Meters );
    }

    public Area GetArea(TimeSpan dt)
    {
        return Area.FromSquareMeters( _squareMetersPerSecond * dt.TotalSeconds );
    }

    public Velocity GetVelocity(Distance distance)
    {
        return Velocity.CreatePerSecond( Distance.FromMeters( _squareMetersPerSecond / distance.Meters ) );
    }

    public TimeSpan GetTime(OrbitalEnergy energy)
    {
        return TimeSpan.FromSeconds( _squareMetersPerSecond / energy.PerSecond.PerSecond.SquareMeters );
    }

    public Distance GetDistance(Velocity velocity)
    {
        return Distance.FromMeters( _squareMetersPerSecond / velocity.PerSecond.Meters );
    }

    public OrbitalEnergy GetEnergy(TimeSpan dt)
    {
        return OrbitalEnergy.CreatePerSquareSecond( Area.FromSquareMeters( _squareMetersPerSecond / dt.TotalSeconds ) );
    }

    public AngularMomentum Abs()
    {
        return new AngularMomentum( Math.Abs( _squareMetersPerSecond ) );
    }

    public override string ToString()
    {
        var absAreaPerSec = Math.Abs( _squareMetersPerSecond );

        if ( absAreaPerSec >= 1000000 )
            return $"{_squareMetersPerSecond:E6} m^2/s";

        if ( absAreaPerSec >= 0.01 )
            return $"{_squareMetersPerSecond:N3} m^2/s";

        if ( absAreaPerSec == 0 )
            return "0 m^2/s";

        return $"{_squareMetersPerSecond:E6} m^2/s";
    }

    public override int GetHashCode()
    {
        return _squareMetersPerSecond.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is AngularMomentum m && Equals( m );
    }

    public int CompareTo(object? obj)
    {
        return obj is AngularMomentum m ? CompareTo( m ) : 1;
    }

    public bool Equals(AngularMomentum other)
    {
        return _squareMetersPerSecond.Equals( other._squareMetersPerSecond );
    }

    public int CompareTo(AngularMomentum other)
    {
        return _squareMetersPerSecond.CompareTo( other._squareMetersPerSecond );
    }

    public static AngularMomentum operator -(AngularMomentum a)
    {
        return new AngularMomentum( -a._squareMetersPerSecond );
    }

    public static AngularMomentum operator +(AngularMomentum a, AngularMomentum b)
    {
        return new AngularMomentum( a._squareMetersPerSecond + b._squareMetersPerSecond );
    }

    public static AngularMomentum operator -(AngularMomentum a, AngularMomentum b)
    {
        return new AngularMomentum( a._squareMetersPerSecond - b._squareMetersPerSecond );
    }

    public static AngularMomentum operator *(AngularMomentum a, double b)
    {
        return new AngularMomentum( a._squareMetersPerSecond * b );
    }

    public static GravitationalParameter operator *(AngularMomentum a, Velocity b)
    {
        return a.GetGravitationalParameter( b );
    }

    public static Area operator *(AngularMomentum a, TimeSpan b)
    {
        return a.GetArea( b );
    }

    public static Area operator *(TimeSpan a, AngularMomentum b)
    {
        return b.GetArea( a );
    }

    public static AngularMomentum operator /(AngularMomentum a, double b)
    {
        return new AngularMomentum( a._squareMetersPerSecond / b );
    }

    public static Ratio operator /(AngularMomentum a, AngularMomentum b)
    {
        return Ratio.Create( a._squareMetersPerSecond / b._squareMetersPerSecond );
    }

    public static Velocity operator /(AngularMomentum a, Distance b)
    {
        return a.GetVelocity( b );
    }

    public static TimeSpan operator /(AngularMomentum a, OrbitalEnergy b)
    {
        return a.GetTime( b );
    }

    public static Distance operator /(AngularMomentum a, Velocity b)
    {
        return a.GetDistance( b );
    }

    public static OrbitalEnergy operator /(AngularMomentum a, TimeSpan b)
    {
        return a.GetEnergy( b );
    }

    public static bool operator ==(AngularMomentum a, AngularMomentum b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(AngularMomentum a, AngularMomentum b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(AngularMomentum a, AngularMomentum b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(AngularMomentum a, AngularMomentum b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(AngularMomentum a, AngularMomentum b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(AngularMomentum a, AngularMomentum b)
    {
        return a.CompareTo( b ) < 0;
    }
}
