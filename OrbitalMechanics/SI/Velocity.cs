using System;

namespace OrbitalMechanics.SI;

public readonly struct Velocity : IEquatable<Velocity>, IComparable<Velocity>
{
    public static readonly Velocity Zero = new Velocity( 0 );

    private readonly double _metersPerSecond;

    private Velocity(double metersPerSecond)
    {
        _metersPerSecond = metersPerSecond;
    }

    public Distance PerSecond => GetDistance( Universe.OneSecond );
    public Distance PerHour => GetDistance( Universe.OneHour );

    public static Velocity CreatePerSecond(Distance distance)
    {
        return new Velocity( distance.Meters );
    }

    public static Velocity CreatePerHour(Distance distance)
    {
        return CreatePerSecond( distance / Universe.OneHour.TotalSeconds );
    }

    public GravitationalParameter GetGravitationalParameter(AngularMomentum momentum)
    {
        return GravitationalParameter.Create( _metersPerSecond * momentum.PerSecond.SquareMeters );
    }

    public AngularMomentum GetMomentum(Distance distance)
    {
        return AngularMomentum.CreatePerSecond( Area.FromSquareMeters( _metersPerSecond * distance.Meters ) );
    }

    public Force GetForce(MassFlowRate flowRate)
    {
        return Force.FromNewtons( _metersPerSecond * flowRate.PerSecond.Kilograms );
    }

    public OrbitalEnergy GetEnergy(Velocity velocity)
    {
        return OrbitalEnergy.CreatePerSquareSecond( Area.FromSquareMeters( _metersPerSecond * velocity._metersPerSecond ) );
    }

    public Distance GetDistance(TimeSpan dt)
    {
        return Distance.FromMeters( _metersPerSecond * dt.TotalSeconds );
    }

    public TimeSpan GetTime(Acceleration acceleration)
    {
        return TimeSpan.FromSeconds( _metersPerSecond / acceleration.PerSecond.PerSecond.Meters );
    }

    public Acceleration GetAcceleration(TimeSpan dt)
    {
        return Acceleration.CreatePerSquareSecond( Distance.FromMeters( _metersPerSecond / dt.TotalSeconds ) );
    }

    public Velocity Abs()
    {
        return new Velocity( Math.Abs( _metersPerSecond ) );
    }

    public override string ToString()
    {
        var absMetersPerSec = Math.Abs( _metersPerSecond );

        if ( absMetersPerSec >= 100000 )
            return $"{PerSecond.Kilometers:N3} km/s";

        if ( absMetersPerSec >= 0.01 )
            return $"{_metersPerSecond:N3} m/s";

        if ( absMetersPerSec == 0 )
            return "0 m/s";

        return $"{_metersPerSecond:E6} m/s";
    }

    public override int GetHashCode()
    {
        return _metersPerSecond.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Velocity v && Equals( v );
    }

    public int CompareTo(object? obj)
    {
        return obj is Velocity v ? CompareTo( v ) : 1;
    }

    public bool Equals(Velocity other)
    {
        return _metersPerSecond.Equals( other._metersPerSecond );
    }

    public int CompareTo(Velocity other)
    {
        return _metersPerSecond.CompareTo( other._metersPerSecond );
    }

    public static Velocity operator -(Velocity a)
    {
        return new Velocity( -a._metersPerSecond );
    }

    public static Velocity operator +(Velocity a, Velocity b)
    {
        return new Velocity( a._metersPerSecond + b._metersPerSecond );
    }

    public static Velocity operator -(Velocity a, Velocity b)
    {
        return new Velocity( a._metersPerSecond - b._metersPerSecond );
    }

    public static Velocity operator *(Velocity a, double b)
    {
        return new Velocity( a._metersPerSecond * b );
    }

    public static GravitationalParameter operator *(Velocity a, AngularMomentum b)
    {
        return a.GetGravitationalParameter( b );
    }

    public static AngularMomentum operator *(Velocity a, Distance b)
    {
        return a.GetMomentum( b );
    }

    public static Force operator *(Velocity a, MassFlowRate b)
    {
        return a.GetForce( b );
    }

    public static OrbitalEnergy operator *(Velocity a, Velocity b)
    {
        return a.GetEnergy( b );
    }

    public static Distance operator *(Velocity a, TimeSpan b)
    {
        return a.GetDistance( b );
    }

    public static Distance operator *(TimeSpan a, Velocity b)
    {
        return b.GetDistance( a );
    }

    public static Velocity operator /(Velocity a, double b)
    {
        return new Velocity( a._metersPerSecond / b );
    }

    public static Ratio operator /(Velocity a, Velocity b)
    {
        return Ratio.Create( a._metersPerSecond / b._metersPerSecond );
    }

    public static TimeSpan operator /(Velocity a, Acceleration b)
    {
        return a.GetTime( b );
    }

    public static Acceleration operator /(Velocity a, TimeSpan b)
    {
        return a.GetAcceleration( b );
    }

    public static bool operator ==(Velocity a, Velocity b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Velocity a, Velocity b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Velocity a, Velocity b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Velocity a, Velocity b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Velocity a, Velocity b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Velocity a, Velocity b)
    {
        return a.CompareTo( b ) < 0;
    }
}
