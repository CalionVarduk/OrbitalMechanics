using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public sealed class ChangePeriapsisManeuver : IOrbitalManeuver
{
    public ChangePeriapsisManeuver(Orbit orbit, Distance nextPeriapsis)
    {
        if ( ! orbit.Eccentricity.IsElliptic )
            throw new ArgumentException( "Orbit must be elliptic in order to change its periapsis.", nameof( orbit ) );

        Initial = orbit;
        Next = Initial;
        ManeuverPoint = OrbitalManeuverPoint.Apoapsis;

        var apoapsis = Initial.Apoapsis.Radius;

        if ( nextPeriapsis <= apoapsis )
        {
            Next = EllipticOrbit.FromRadii(
                parent: Initial.Parent,
                periapsis: nextPeriapsis,
                apoapsis: apoapsis,
                inclination: Initial.Inclination,
                argumentOfPeriapsis: Initial.ArgumentOfPeriapsis,
                longitudeOfAscendingNode: Initial.LongitudeOfAscendingNode );
        }
        else
        {
            Next = EllipticOrbit.FromRadii(
                parent: Initial.Parent,
                periapsis: apoapsis,
                apoapsis: nextPeriapsis,
                inclination: Initial.Inclination,
                argumentOfPeriapsis: Initial.ArgumentOfPeriapsis,
                longitudeOfAscendingNode: Initial.LongitudeOfAscendingNode );
        }

        DeltaV = (Initial.Apoapsis.Velocity - Next.Apoapsis.Velocity).Abs();
    }

    public Orbit Initial { get; }
    public Orbit Next { get; }
    public Velocity DeltaV { get; }
    public OrbitalManeuverPoint ManeuverPoint { get; }

    public override string ToString()
    {
        var lines = new[]
        {
            $"dV: {DeltaV} at {ManeuverPoint} point (periapsis change)",
            $"Initial[{Initial}]",
            $"Next[{Next}]"
        };

        return string.Join( Environment.NewLine, lines );
    }
}
