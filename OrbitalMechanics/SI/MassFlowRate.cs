using System;

namespace OrbitalMechanics.SI;

public readonly struct MassFlowRate : IEquatable<MassFlowRate>, IComparable<MassFlowRate>
{
    public static readonly MassFlowRate Zero = new MassFlowRate( 0 );

    private readonly double _kilogramsPerSecond;

    private MassFlowRate(double kilogramsPerSecond)
    {
        _kilogramsPerSecond = kilogramsPerSecond;
    }

    public Mass PerSecond => GetMass( Universe.OneSecond );
    public Mass PerHour => GetMass( Universe.OneHour );

    public static MassFlowRate CreatePerSecond(Mass mass)
    {
        return new MassFlowRate( mass.Kilograms );
    }

    public static MassFlowRate CreatePerHour(Mass mass)
    {
        return CreatePerSecond( mass / Universe.OneHour.TotalSeconds );
    }

    public Force GetForce(Velocity velocity)
    {
        return Force.FromNewtons( _kilogramsPerSecond * velocity.PerSecond.Meters );
    }

    public Mass GetMass(TimeSpan dt)
    {
        return Mass.FromKilograms( _kilogramsPerSecond * dt.TotalSeconds );
    }

    public MassFlowRate Abs()
    {
        return new MassFlowRate( Math.Abs( _kilogramsPerSecond ) );
    }

    public override string ToString()
    {
        var absKgPerSec = Math.Abs( _kilogramsPerSecond );

        if ( absKgPerSec >= 0.01 )
            return $"{_kilogramsPerSecond:N3} kg/s";

        if ( absKgPerSec == 0 )
            return "0 kg/s";

        return $"{_kilogramsPerSecond:E6} kg/s";
    }

    public override int GetHashCode()
    {
        return _kilogramsPerSecond.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is MassFlowRate r && Equals( r );
    }

    public int CompareTo(object? obj)
    {
        return obj is MassFlowRate r ? CompareTo( r ) : 1;
    }

    public bool Equals(MassFlowRate other)
    {
        return _kilogramsPerSecond.Equals( other._kilogramsPerSecond );
    }

    public int CompareTo(MassFlowRate other)
    {
        return _kilogramsPerSecond.CompareTo( other._kilogramsPerSecond );
    }

    public static MassFlowRate operator -(MassFlowRate a)
    {
        return new MassFlowRate( -a._kilogramsPerSecond );
    }

    public static MassFlowRate operator +(MassFlowRate a, MassFlowRate b)
    {
        return new MassFlowRate( a._kilogramsPerSecond + b._kilogramsPerSecond );
    }

    public static MassFlowRate operator -(MassFlowRate a, MassFlowRate b)
    {
        return new MassFlowRate( a._kilogramsPerSecond - b._kilogramsPerSecond );
    }

    public static MassFlowRate operator *(MassFlowRate a, double b)
    {
        return new MassFlowRate( a._kilogramsPerSecond * b );
    }

    public static Force operator *(MassFlowRate a, Velocity b)
    {
        return a.GetForce( b );
    }

    public static Mass operator *(MassFlowRate a, TimeSpan b)
    {
        return a.GetMass( b );
    }

    public static Mass operator *(TimeSpan a, MassFlowRate b)
    {
        return b.GetMass( a );
    }

    public static MassFlowRate operator /(MassFlowRate a, double b)
    {
        return new MassFlowRate( a._kilogramsPerSecond / b );
    }

    public static Ratio operator /(MassFlowRate a, MassFlowRate b)
    {
        return Ratio.Create( a._kilogramsPerSecond / b._kilogramsPerSecond );
    }

    public static bool operator ==(MassFlowRate a, MassFlowRate b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(MassFlowRate a, MassFlowRate b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(MassFlowRate a, MassFlowRate b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(MassFlowRate a, MassFlowRate b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(MassFlowRate a, MassFlowRate b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(MassFlowRate a, MassFlowRate b)
    {
        return a.CompareTo( b ) < 0;
    }
}
