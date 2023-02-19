using System;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits;

public sealed class HyperbolicOrbit : Orbit
{
    public HyperbolicOrbit(
        CelestialBody parent,
        Distance semiMajorAxis,
        Eccentricity eccentricity,
        Angle inclination,
        Angle argumentOfPeriapsis,
        Angle longitudeOfAscendingNode,
        Angle meanAnomalyAtEpoch)
        : base( parent, semiMajorAxis, eccentricity, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, meanAnomalyAtEpoch )
    {
        if ( ! eccentricity.IsHyperbolic )
            throw new ArgumentException( $"Eccentricity '{eccentricity}' is not hyperbolic.", nameof( eccentricity ) );
    }

    public override Distance SemiMinorAxis => SemiMajorAxis * Math.Sqrt( Math.Pow( Eccentricity.Value, 2 ) - 1 );
    public override OrbitalEnergy Energy => Parent.GravitationalParameter / (SemiMajorAxis * 2);
    public Velocity ExcessSpeed => (Parent.GravitationalParameter / SemiMajorAxis).Sqrt();

    public static HyperbolicOrbit FromRadii(
        CelestialBody parent,
        Distance periapsis,
        Distance apoapsis,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Angle meanAnomalyAtEpoch = default)
    {
        var semiMajorAxis = (-apoapsis - periapsis) / 2;
        var eccentricity = Eccentricity.Create( periapsis / semiMajorAxis + 1 );
        return new HyperbolicOrbit(
            parent,
            semiMajorAxis,
            eccentricity,
            inclination,
            argumentOfPeriapsis,
            longitudeOfAscendingNode,
            meanAnomalyAtEpoch );
    }

    public static HyperbolicOrbit FromExcessSpeed(
        CelestialBody parent,
        Distance periapsis,
        Velocity excessSpeed,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Angle meanAnomalyAtEpoch = default)
    {
        var eccentricity = Eccentricity.Create( 1 + excessSpeed * excessSpeed * periapsis / parent.GravitationalParameter );
        var semiMajorAxis = periapsis / (eccentricity.Value - 1);
        return new HyperbolicOrbit(
            parent,
            semiMajorAxis,
            eccentricity,
            inclination,
            argumentOfPeriapsis,
            longitudeOfAscendingNode,
            meanAnomalyAtEpoch );
    }
}
