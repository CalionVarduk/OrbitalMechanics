using System;

namespace OrbitalMechanics.SI;

public readonly struct Area : IEquatable<Area>, IComparable<Area>
{
    public static readonly Area Zero = new Area( 0 );

    private Area(double squareKilometers)
    {
        SquareKilometers = squareKilometers;
    }

    public double SquareKilometers { get; }
    public double SquareMeters => SquareKilometers * 1000000;

    public static Area FromSquareKilometers(double value)
    {
        return new Area( value );
    }

    public static Area FromSquareMeters(double value)
    {
        return FromSquareKilometers( value / 1000000 );
    }

    public GravitationalParameter GetGravitationalParameter(Acceleration acceleration)
    {
        return GravitationalParameter.Create( SquareMeters * acceleration.PerSecond.PerSecond.Meters );
    }

    public TimeSpan GetTime(AngularMomentum momentum)
    {
        return TimeSpan.FromSeconds( SquareMeters / momentum.PerSecond.SquareMeters );
    }

    public Distance GetDistance(Distance distance)
    {
        return Distance.FromKilometers( SquareKilometers / distance.Kilometers );
    }

    public AngularMomentum GetMomentum(TimeSpan dt)
    {
        return AngularMomentum.CreatePerSecond( this ) / dt.TotalSeconds;
    }

    public Area Abs()
    {
        return new Area( Math.Abs( SquareKilometers ) );
    }

    public override string ToString()
    {
        var absKm = Math.Abs( SquareKilometers );

        if ( absKm >= 1000000 )
            return $"{SquareKilometers:E6} km^2";

        if ( absKm >= 1 )
            return $"{SquareKilometers:N3} km^2";

        if ( absKm >= 0.0000001 )
            return $"{SquareMeters:N3} m^2";

        if ( absKm == 0 )
            return "0 km^2";

        return $"{SquareKilometers:E6} km^2";
    }

    public override int GetHashCode()
    {
        return SquareKilometers.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Area a && Equals( a );
    }

    public int CompareTo(object? obj)
    {
        return obj is Area a ? CompareTo( a ) : 1;
    }

    public bool Equals(Area other)
    {
        return SquareKilometers.Equals( other.SquareKilometers );
    }

    public int CompareTo(Area other)
    {
        return SquareKilometers.CompareTo( other.SquareKilometers );
    }

    public static Area operator -(Area a)
    {
        return new Area( -a.SquareKilometers );
    }

    public static Area operator +(Area a, Area b)
    {
        return new Area( a.SquareKilometers + b.SquareKilometers );
    }

    public static Area operator -(Area a, Area b)
    {
        return new Area( a.SquareKilometers - b.SquareKilometers );
    }

    public static Area operator *(Area a, double b)
    {
        return new Area( a.SquareKilometers * b );
    }

    public static GravitationalParameter operator *(Area a, Acceleration b)
    {
        return a.GetGravitationalParameter( b );
    }

    public static Area operator /(Area a, double b)
    {
        return new Area( a.SquareKilometers / b );
    }

    public static Ratio operator /(Area a, Area b)
    {
        return Ratio.Create( a.SquareKilometers / b.SquareKilometers );
    }

    public static TimeSpan operator /(Area a, AngularMomentum b)
    {
        return a.GetTime( b );
    }

    public static Distance operator /(Area a, Distance b)
    {
        return a.GetDistance( b );
    }

    public static AngularMomentum operator /(Area a, TimeSpan b)
    {
        return a.GetMomentum( b );
    }

    public static bool operator ==(Area a, Area b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Area a, Area b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Area a, Area b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Area a, Area b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Area a, Area b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Area a, Area b)
    {
        return a.CompareTo( b ) < 0;
    }
}
