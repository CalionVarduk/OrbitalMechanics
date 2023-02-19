using System;

namespace OrbitalMechanics.SI;

public readonly struct GravitationalParameter : IEquatable<GravitationalParameter>, IComparable<GravitationalParameter>
{
    public static readonly GravitationalParameter Zero = new GravitationalParameter( 0 );

    private GravitationalParameter(double value)
    {
        Value = value;
    }

    public double Value { get; }

    public static GravitationalParameter Create(double value)
    {
        return new GravitationalParameter( value );
    }

    public static GravitationalParameter Create(Mass mass)
    {
        return Create( Universe.GravitationalConstant * mass.Kilograms );
    }

    public Area GetArea(Acceleration acceleration)
    {
        return Area.FromSquareMeters( Value / acceleration.PerSecond.PerSecond.Meters );
    }

    public Velocity GetVelocity(AngularMomentum momentum)
    {
        return Velocity.CreatePerSecond( Distance.FromMeters( Value / momentum.PerSecond.SquareMeters ) );
    }

    public OrbitalEnergy GetEnergy(Distance distance)
    {
        return OrbitalEnergy.CreatePerSquareSecond( Area.FromSquareMeters( Value / distance.Meters ) );
    }

    public Distance GetDistance(OrbitalEnergy energy)
    {
        return Distance.FromMeters( Value / energy.PerSecond.PerSecond.SquareMeters );
    }

    public AngularMomentum GetMomentum(Velocity velocity)
    {
        return AngularMomentum.CreatePerSecond( Area.FromSquareMeters( Value / velocity.PerSecond.Meters ) );
    }

    public Acceleration GetAcceleration(Area area)
    {
        return Acceleration.CreatePerSquareSecond( Distance.FromMeters( Value / area.SquareMeters ) );
    }

    public GravitationalParameter Abs()
    {
        return new GravitationalParameter( Math.Abs( Value ) );
    }

    public override string ToString()
    {
        var absValue = Math.Abs( Value );

        if ( absValue >= 1000000 )
            return $"{Value:E6} m^3/s^2";

        if ( absValue >= 0.01 )
            return $"{Value:N3} m^3/s^2";

        if ( absValue == 0 )
            return "0 m^3/s^2";

        return $"{Value:E6} m^3/s^2";
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is GravitationalParameter g && Equals( g );
    }

    public int CompareTo(object? obj)
    {
        return obj is GravitationalParameter g ? CompareTo( g ) : 1;
    }

    public bool Equals(GravitationalParameter other)
    {
        return Value.Equals( other.Value );
    }

    public int CompareTo(GravitationalParameter other)
    {
        return Value.CompareTo( other.Value );
    }

    public static GravitationalParameter operator -(GravitationalParameter a)
    {
        return new GravitationalParameter( -a.Value );
    }

    public static GravitationalParameter operator +(GravitationalParameter a, GravitationalParameter b)
    {
        return new GravitationalParameter( a.Value + b.Value );
    }

    public static GravitationalParameter operator -(GravitationalParameter a, GravitationalParameter b)
    {
        return new GravitationalParameter( a.Value - b.Value );
    }

    public static GravitationalParameter operator *(GravitationalParameter a, double b)
    {
        return new GravitationalParameter( a.Value * b );
    }

    public static GravitationalParameter operator /(GravitationalParameter a, double b)
    {
        return new GravitationalParameter( a.Value / b );
    }

    public static Ratio operator /(GravitationalParameter a, GravitationalParameter b)
    {
        return Ratio.Create( a.Value / b.Value );
    }

    public static Area operator /(GravitationalParameter a, Acceleration b)
    {
        return a.GetArea( b );
    }

    public static Velocity operator /(GravitationalParameter a, AngularMomentum b)
    {
        return a.GetVelocity( b );
    }

    public static OrbitalEnergy operator /(GravitationalParameter a, Distance b)
    {
        return a.GetEnergy( b );
    }

    public static Distance operator /(GravitationalParameter a, OrbitalEnergy b)
    {
        return a.GetDistance( b );
    }

    public static AngularMomentum operator /(GravitationalParameter a, Velocity b)
    {
        return a.GetMomentum( b );
    }

    public static Acceleration operator /(GravitationalParameter a, Area b)
    {
        return a.GetAcceleration( b );
    }

    public static bool operator ==(GravitationalParameter a, GravitationalParameter b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(GravitationalParameter a, GravitationalParameter b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(GravitationalParameter a, GravitationalParameter b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(GravitationalParameter a, GravitationalParameter b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(GravitationalParameter a, GravitationalParameter b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(GravitationalParameter a, GravitationalParameter b)
    {
        return a.CompareTo( b ) < 0;
    }
}
