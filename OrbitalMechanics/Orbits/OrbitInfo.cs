using OrbitalMechanics.Bodies;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits;

public record struct OrbitInfo(
    CelestialBody Parent,
    Distance SemiMajorAxis,
    Eccentricity Eccentricity,
    Angle Inclination = default,
    Angle ArgumentOfPeriapsis = default,
    Angle LongitudeOfAscendingNode = default,
    Angle? MeanAnomalyAtEpoch = null);
