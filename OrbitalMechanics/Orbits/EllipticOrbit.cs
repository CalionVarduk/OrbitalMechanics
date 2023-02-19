using System;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits;

public sealed class EllipticOrbit : Orbit
{
    public EllipticOrbit(
        CelestialBody parent,
        Distance semiMajorAxis,
        Eccentricity eccentricity,
        Angle inclination,
        Angle argumentOfPeriapsis,
        Angle longitudeOfAscendingNode,
        Angle meanAnomalyAtEpoch)
        : base( parent, semiMajorAxis, eccentricity, inclination, argumentOfPeriapsis, longitudeOfAscendingNode, meanAnomalyAtEpoch )
    {
        if ( ! eccentricity.IsElliptic )
            throw new ArgumentException( $"Eccentricity '{eccentricity}' is not elliptic.", nameof( eccentricity ) );
    }

    public override Distance SemiMinorAxis => SemiMajorAxis * Math.Sqrt( 1 - Math.Pow( Eccentricity.Value, 2 ) );
    public override OrbitalEnergy Energy => -Parent.GravitationalParameter / (SemiMajorAxis * 2);

    public static EllipticOrbit CircularFromRadius(
        CelestialBody parent,
        Distance radius,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Angle meanAnomalyAtEpoch = default)
    {
        return new EllipticOrbit(
            parent,
            radius,
            Eccentricity.Circular,
            inclination,
            argumentOfPeriapsis,
            longitudeOfAscendingNode,
            meanAnomalyAtEpoch );
    }

    public static EllipticOrbit CircularFromAltitude(
        CelestialBody parent,
        Distance altitude,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Angle meanAnomalyAtEpoch = default)
    {
        return CircularFromRadius(
            parent,
            parent.GetRadius( altitude ),
            inclination,
            argumentOfPeriapsis,
            longitudeOfAscendingNode,
            meanAnomalyAtEpoch );
    }

    public static EllipticOrbit CircularFromSphereOfInfluenceEdge(
        CelestialBody parent,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Angle meanAnomalyAtEpoch = default)
    {
        return CircularFromRadius(
            parent,
            parent.SphereOfInfluenceRadius,
            inclination,
            argumentOfPeriapsis,
            longitudeOfAscendingNode,
            meanAnomalyAtEpoch );
    }

    public static EllipticOrbit FromRadii(
        CelestialBody parent,
        Distance periapsis,
        Distance apoapsis,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Angle meanAnomalyAtEpoch = default)
    {
        var eccentricity = Eccentricity.Create( (apoapsis - periapsis) / (apoapsis + periapsis) );
        var semiMajorAxis = periapsis / (1 - eccentricity.Value);
        return new EllipticOrbit(
            parent,
            semiMajorAxis,
            eccentricity,
            inclination,
            argumentOfPeriapsis,
            longitudeOfAscendingNode,
            meanAnomalyAtEpoch );
    }

    public static EllipticOrbit FromAltitudes(
        CelestialBody parent,
        Distance periapsis,
        Distance apoapsis,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Angle meanAnomalyAtEpoch = default)
    {
        return FromRadii(
            parent,
            parent.GetRadius( periapsis ),
            parent.GetRadius( apoapsis ),
            inclination,
            argumentOfPeriapsis,
            longitudeOfAscendingNode,
            meanAnomalyAtEpoch );
    }
}
