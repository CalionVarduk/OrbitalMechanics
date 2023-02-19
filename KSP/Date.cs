using System;

namespace KSP;

public readonly struct Date : IEquatable<Date>, IComparable<Date>
{
    public static readonly Date Epoch = new Date( Time.Zero );

    private readonly Time _timeSinceEpoch;

    public Date(Time timeSinceEpoch)
    {
        if ( timeSinceEpoch < Time.Zero )
            throw new ArgumentException( "Time since epoch cannot be negative.", nameof( timeSinceEpoch ) );

        _timeSinceEpoch = timeSinceEpoch;
    }

    public Date(int year, int day, TimeSpan timeOfDay)
    {
        if ( year < 1 )
            throw new ArgumentException( "Year cannot be less than 1.", nameof( year ) );

        if ( day < 1 || day > 426 )
            throw new ArgumentException( "Day must be in [1, 426] range.", nameof( day ) );

        if ( timeOfDay < TimeSpan.Zero || timeOfDay.Ticks >= Time.TicksPerDay )
            throw new ArgumentException( "Time of day must be in [0h, 6h) range.", nameof( timeOfDay ) );

        _timeSinceEpoch = new Time( year - 1, day - 1, timeOfDay );
    }

    public int Year => _timeSinceEpoch.Years + 1;
    public int Day => _timeSinceEpoch.Days + 1;
    public TimeSpan TimeOfDay => _timeSinceEpoch.Hours;

    public Time ToTimeSinceEpoch()
    {
        return _timeSinceEpoch;
    }

    public override string ToString()
    {
        return $"Year: {Year}, Day: {Day}, Time: {TimeOfDay:c}";
    }

    public override int GetHashCode()
    {
        return _timeSinceEpoch.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Date d && Equals( d );
    }

    public int CompareTo(object? obj)
    {
        return obj is Date d ? CompareTo( d ) : 1;
    }

    public bool Equals(Date other)
    {
        return _timeSinceEpoch.Equals( other._timeSinceEpoch );
    }

    public int CompareTo(Date other)
    {
        return _timeSinceEpoch.CompareTo( other._timeSinceEpoch );
    }

    public static Time operator -(Date a, Date b)
    {
        return a._timeSinceEpoch - b._timeSinceEpoch;
    }

    public static Date operator -(Date a, Time b)
    {
        return new Date( a._timeSinceEpoch - b );
    }

    public static Date operator +(Date a, Time b)
    {
        return new Date( a._timeSinceEpoch + b );
    }

    public static bool operator ==(Date a, Date b)
    {
        return a.Equals( b );
    }

    public static bool operator !=(Date a, Date b)
    {
        return ! a.Equals( b );
    }

    public static bool operator >=(Date a, Date b)
    {
        return a.CompareTo( b ) >= 0;
    }

    public static bool operator <=(Date a, Date b)
    {
        return a.CompareTo( b ) <= 0;
    }

    public static bool operator >(Date a, Date b)
    {
        return a.CompareTo( b ) > 0;
    }

    public static bool operator <(Date a, Date b)
    {
        return a.CompareTo( b ) < 0;
    }
}
