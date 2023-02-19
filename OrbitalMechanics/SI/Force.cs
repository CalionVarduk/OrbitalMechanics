using System;

namespace OrbitalMechanics.SI;

public readonly struct Force : IEquatable<Force>, IComparable<Force>
{
    public static readonly Force Zero = new Force( 0 );

    private Force(double newtons)
    {
        Newtons = newtons;
    }

    public double Newtons { get; }
    public double Kilonewtons => Newtons / 1000;

    public static Force FromNewtons(double value)
    {
        return new Force( value );
    }

    public static Force FromKilonewtons(double value)
    {
        return FromNewtons( value * 1000 );
    }

    public Mass GetMass(Acceleration acceleration)
    {
        return Mass.FromKilograms( Newtons / acceleration.PerSecond.PerSecond.Meters );
    }

    public Acceleration GetAcceleration(Mass mass)
    {
        return Acceleration.CreatePerSquareSecond( Distance.FromMeters( Newtons / mass.Kilograms ) );
    }

    public Velocity GetVelocity(MassFlowRate flowRate)
    {
        return Velocity.CreatePerSecond( Distance.FromMeters( Newtons / flowRate.PerSecond.Kilograms ) );
    }

    public MassFlowRate GetFlowRate(Velocity velocity)
    {
        return MassFlowRate.CreatePerSecond( Mass.FromKilograms( Newtons / velocity.PerSecond.Meters ) );
    }

    public Force Abs()
    {
        return new Force( Math.Abs( Newtons ) );
    }

    public override string ToString()
    {
        var absN = Math.Abs( Newtons );

        if ( absN >= 1000000000 )
            return $"{Kilonewtons:E6} kN";

        if ( absN >= 1000 )
            return $"{Kilonewtons:N3} kN";

        if ( absN >= 0.01 )
            return $"{Newtons:N3} N";

        if ( absN == 0 )
            return "0 N";

        return $"{Newtons:E6} N";
    }

    public override int GetHashCode()
    {
        return Newtons.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Force f && Equals( f );
    }

    public int CompareTo(object? obj)
    {
        return obj is Force f ? CompareTo( f ) : 1;
    }

    public bool Equals(Force other)
    {
        return Newtons.Equals( other.Newtons );
    }

    public int CompareTo(Force other)
    {
        return Newtons.CompareTo( other.Newtons );
    }

    public static Force operator -(Force a)
    {
        return new Force( -a.Newtons );
    }

    public static Force operator +(Force a, Force b)
    {
        return new Force( a.Newtons + b.Newtons );
    }

    public static Force operator -(Force a, Force b)
    {
        return new Force( a.Newtons - b.Newtons );
    }

    public static Force operator *(Force a, double b)
    {
        return new Force( a.Newtons * b );
    }

    public static Force operator /(Force a, double b)
    {
        return new Force( a.Newtons / b );
    }

    public static Ratio operator /(Force a, Force b)
    {
        return Ratio.Create( a.Newtons / b.Newtons );
    }

    public static Mass operator /(Force a, Acceleration b)
    {
        return a.GetMass( b );
    }

    public static Acceleration operator /(Force a, Mass b)
    {
        return a.GetAcceleration( b );
    }

    public static Velocity operator /(Force a, MassFlowRate b)
    {
        return a.GetVelocity( b );
    }

    public static MassFlowRate operator /(Force a, Velocity b)
    {
        return a.GetFlowRate( b );
    }

    public static bool operator ==(Force a, Force b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Force a, Force b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Force a, Force b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Force a, Force b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Force a, Force b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Force a, Force b)
    {
        return a.CompareTo( b ) < 0;
    }
}
