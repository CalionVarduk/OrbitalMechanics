using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics;

public static class Universe
{
    // KSP seems to use gravitational constant value from 2014
    public const double GravitationalConstant = 6.67408e-11;
    public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds( 1 );
    public static readonly TimeSpan OneHour = TimeSpan.FromHours( 1 );
    public static readonly Acceleration StandardGravity = Acceleration.CreatePerSquareSecond( Distance.FromMeters( 9.80665 ) );
}
