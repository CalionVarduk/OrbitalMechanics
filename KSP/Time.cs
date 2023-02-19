using System;

namespace KSP;

public readonly struct Time : IEquatable<Time>, IComparable<Time>
{
    public const long TicksPerDay = TimeSpan.TicksPerHour * 6;
    public const long TicksPerYear = TicksPerDay * 426;
    public static readonly Time Zero = new Time( TimeSpan.Zero );

    private readonly TimeSpan _span;

    public Time(TimeSpan span)
    {
        _span = span;
    }

    public Time(int years, int days, TimeSpan hours)
    {
        _span = TimeSpan.FromTicks( years * TicksPerYear + days * TicksPerDay + hours.Ticks );
    }

    public int Years => (int)(_span.Ticks / TicksPerYear);
    public int Days => (int)((_span.Ticks - Years * TicksPerYear - Hours.Ticks) / TicksPerDay);
    public TimeSpan Hours => TimeSpan.FromTicks( _span.Ticks % TicksPerDay );

    public TimeSpan ToTimeSpan()
    {
        return _span;
    }

    public override string ToString()
    {
        return $"Years: {Years}, Days: {Days}, Hours: {Hours:c}";
    }

    public override int GetHashCode()
    {
        return _span.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Time t && Equals( t );
    }

    public int CompareTo(object? obj)
    {
        return obj is Time t ? CompareTo( t ) : 1;
    }

    public bool Equals(Time other)
    {
        return _span == other._span;
    }

    public int CompareTo(Time other)
    {
        return _span.CompareTo( other._span );
    }

    public static Time operator -(Time a)
    {
        return new Time( -a._span );
    }

    public static Time operator -(Time a, Time b)
    {
        return new Time( a._span - b._span );
    }

    public static Time operator +(Time a, Time b)
    {
        return new Time( a._span + b._span );
    }

    public static bool operator ==(Time a, Time b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Time a, Time b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Time a, Time b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Time a, Time b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Time a, Time b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Time a, Time b)
    {
        return a.CompareTo( b ) < 0;
    }
}
