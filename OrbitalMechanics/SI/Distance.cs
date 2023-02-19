using System;

namespace OrbitalMechanics.SI;

public readonly struct Distance : IEquatable<Distance>, IComparable<Distance>
{
    public static readonly Distance Zero = new Distance( 0 );

    private Distance(double kilometers)
    {
        Kilometers = kilometers;
    }

    public double Kilometers { get; }
    public double Meters => Kilometers * 1000;

    public static Distance FromKilometers(double value)
    {
        return new Distance( value );
    }

    public static Distance FromMeters(double value)
    {
        return FromKilometers( value / 1000 );
    }

    public OrbitalEnergy GetEnergy(Acceleration acceleration)
    {
        return OrbitalEnergy.CreatePerSquareSecond( Area.FromSquareMeters( Meters * acceleration.PerSecond.PerSecond.Meters ) );
    }

    public Area GetArea(Distance distance)
    {
        return Area.FromSquareKilometers( Kilometers * distance.Kilometers );
    }

    public GravitationalParameter GetGravitationalParameter(OrbitalEnergy energy)
    {
        return GravitationalParameter.Create( Meters * energy.PerSecond.PerSecond.SquareMeters );
    }

    public AngularMomentum GetMomentum(Velocity velocity)
    {
        return AngularMomentum.CreatePerSecond( Area.FromSquareKilometers( Kilometers * velocity.PerSecond.Kilometers ) );
    }

    public TimeSpan GetTime(Velocity velocity)
    {
        return TimeSpan.FromSeconds( Kilometers / velocity.PerSecond.Kilometers );
    }

    public Velocity GetVelocity(TimeSpan dt)
    {
        return Velocity.CreatePerSecond( this ) / dt.TotalSeconds;
    }

    public Distance Abs()
    {
        return new Distance( Math.Abs( Kilometers ) );
    }

    public override string ToString()
    {
        var absKm = Math.Abs( Kilometers );

        if ( absKm >= 1000000 )
            return $"{Kilometers:E6} km";

        if ( absKm >= 1 )
            return $"{Kilometers:N3} km";

        if ( absKm >= 0.0001 )
            return $"{Meters:N3} m";

        if ( absKm == 0 )
            return "0 km";

        return $"{Kilometers:E6} km";
    }

    public override int GetHashCode()
    {
        return Kilometers.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Distance l && Equals( l );
    }

    public int CompareTo(object? obj)
    {
        return obj is Distance l ? CompareTo( l ) : 1;
    }

    public bool Equals(Distance other)
    {
        return Kilometers.Equals( other.Kilometers );
    }

    public int CompareTo(Distance other)
    {
        return Kilometers.CompareTo( other.Kilometers );
    }

    public static Distance operator -(Distance a)
    {
        return new Distance( -a.Kilometers );
    }

    public static Distance operator +(Distance a, Distance b)
    {
        return new Distance( a.Kilometers + b.Kilometers );
    }

    public static Distance operator -(Distance a, Distance b)
    {
        return new Distance( a.Kilometers - b.Kilometers );
    }

    public static Distance operator *(Distance a, double b)
    {
        return new Distance( a.Kilometers * b );
    }

    public static OrbitalEnergy operator *(Distance a, Acceleration b)
    {
        return a.GetEnergy( b );
    }

    public static Area operator *(Distance a, Distance b)
    {
        return a.GetArea( b );
    }

    public static GravitationalParameter operator *(Distance a, OrbitalEnergy b)
    {
        return a.GetGravitationalParameter( b );
    }

    public static AngularMomentum operator *(Distance a, Velocity b)
    {
        return a.GetMomentum( b );
    }

    public static Distance operator /(Distance a, double b)
    {
        return new Distance( a.Kilometers / b );
    }

    public static Ratio operator /(Distance a, Distance b)
    {
        return Ratio.Create( a.Kilometers / b.Kilometers );
    }

    public static TimeSpan operator /(Distance a, Velocity b)
    {
        return a.GetTime( b );
    }

    public static Velocity operator /(Distance a, TimeSpan b)
    {
        return a.GetVelocity( b );
    }

    public static bool operator ==(Distance a, Distance b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Distance a, Distance b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Distance a, Distance b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Distance a, Distance b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Distance a, Distance b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Distance a, Distance b)
    {
        return a.CompareTo( b ) < 0;
    }
}
