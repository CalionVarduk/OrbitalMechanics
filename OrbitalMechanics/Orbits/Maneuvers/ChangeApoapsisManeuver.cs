using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public sealed class ChangeApoapsisManeuver : IOrbitalManeuver
{
    public ChangeApoapsisManeuver(Orbit orbit, Distance nextApoapsis)
    {
        Initial = orbit;
        Next = Initial;
        ManeuverPoint = OrbitalManeuverPoint.Periapsis;

        var periapsis = Initial.Periapsis.Radius;

        if ( nextApoapsis >= periapsis )
        {
            Next = EllipticOrbit.FromRadii(
                parent: Initial.Parent,
                periapsis: periapsis,
                apoapsis: nextApoapsis,
                inclination: Initial.Inclination,
                argumentOfPeriapsis: Initial.ArgumentOfPeriapsis,
                longitudeOfAscendingNode: Initial.LongitudeOfAscendingNode );
        }
        else if ( nextApoapsis >= Distance.Zero )
        {
            Next = EllipticOrbit.FromRadii(
                parent: Initial.Parent,
                periapsis: nextApoapsis,
                apoapsis: periapsis,
                inclination: Initial.Inclination,
                argumentOfPeriapsis: Initial.ArgumentOfPeriapsis,
                longitudeOfAscendingNode: Initial.LongitudeOfAscendingNode );
        }
        else
        {
            Next = HyperbolicOrbit.FromRadii(
                parent: Initial.Parent,
                periapsis: periapsis,
                apoapsis: nextApoapsis,
                inclination: Initial.Inclination,
                argumentOfPeriapsis: Initial.ArgumentOfPeriapsis,
                longitudeOfAscendingNode: Initial.LongitudeOfAscendingNode );
        }

        DeltaV = (Initial.Periapsis.Velocity - Next.Periapsis.Velocity).Abs();
    }

    public Orbit Initial { get; }
    public Orbit Next { get; }
    public Velocity DeltaV { get; }
    public OrbitalManeuverPoint ManeuverPoint { get; }

    public override string ToString()
    {
        var lines = new[]
        {
            $"dV: {DeltaV} at {ManeuverPoint} point (apoapsis change)",
            $"Initial[{Initial}]",
            $"Next[{Next}]"
        };

        return string.Join( Environment.NewLine, lines );
    }
}
