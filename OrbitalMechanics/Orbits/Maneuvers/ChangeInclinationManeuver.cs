using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public sealed class ChangeInclinationManeuver : IOrbitalManeuver
{
    public ChangeInclinationManeuver(
        Orbit orbit,
        Angle targetInclination,
        OrbitalManeuverPoint maneuverPoint = OrbitalManeuverPoint.Unspecified)
    {
        if ( ! orbit.Eccentricity.IsElliptic && maneuverPoint == OrbitalManeuverPoint.Apoapsis )
            throw new InvalidOperationException( "Orbit must be elliptic in order to change its inclination at apoapsis." );

        Initial = orbit;
        ManeuverPoint = maneuverPoint;

        targetInclination = targetInclination.NormalizePositive();
        if ( targetInclination > Angle.Half )
            targetInclination -= Angle.Half;

        Next = Orbit.Create(
            new OrbitInfo
            {
                Parent = Initial.Parent,
                SemiMajorAxis = Initial.SemiMajorAxis,
                Eccentricity = Initial.Eccentricity,
                Inclination = targetInclination,
                ArgumentOfPeriapsis = Initial.ArgumentOfPeriapsis,
                LongitudeOfAscendingNode = Initial.LongitudeOfAscendingNode,
                MeanAnomalyAtEpoch = Initial.PointAtEpoch.MeanAnomaly
            } );

        var inclinationDelta = (Initial.Inclination - Next.Inclination).Abs();
        DeltaV = Initial.GetVelocity( ManeuverPoint ) * 2 * (inclinationDelta * 0.5).Sin();
    }

    public Orbit Initial { get; }
    public Orbit Next { get; }
    public Velocity DeltaV { get; }
    public OrbitalManeuverPoint ManeuverPoint { get; }

    public override string ToString()
    {
        var lines = new[]
        {
            $"dV: {DeltaV} at {ManeuverPoint} point (inclination change)",
            $"Initial[{Initial}]",
            $"Next[{Next}]"
        };

        return string.Join( Environment.NewLine, lines );
    }
}
