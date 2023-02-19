using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public sealed class CaptureManeuver : IOrbitalManeuver
{
    public CaptureManeuver(
        Distance sourceRadius,
        Orbit parkingOrbit,
        OrbitalManeuverPoint targetBodyManeuverPoint = OrbitalManeuverPoint.Unspecified)
    {
        var targetBody = parkingOrbit.Parent;
        if ( targetBody.Orbit is null )
            throw new ArgumentException( "Parking orbit's parent body must have an orbit.", nameof( parkingOrbit ) );

        ManeuverPoint = OrbitalManeuverPoint.Periapsis;
        Next = parkingOrbit;
        TargetBodyManeuverPoint = targetBodyManeuverPoint;

        var targetRadius = targetBody.Orbit.GetRadius( TargetBodyManeuverPoint );
        Initial = EllipticOrbit.FromRadii(
            parent: targetBody.Orbit.Parent,
            periapsis: sourceRadius < targetRadius ? sourceRadius : targetRadius,
            apoapsis: sourceRadius > targetRadius ? sourceRadius : targetRadius,
            inclination: targetBody.Orbit.Inclination );

        var velocityAtTarget = sourceRadius > targetRadius ? Initial.Periapsis.Velocity : Initial.Apoapsis.Velocity;
        var excessSpeed = (targetBody.Orbit.GetVelocity( TargetBodyManeuverPoint ) - velocityAtTarget).Abs();

        Capture = HyperbolicOrbit.FromExcessSpeed(
            parent: Next.Parent,
            periapsis: Next.Periapsis.Radius,
            excessSpeed: excessSpeed,
            inclination: Next.Inclination );

        DeltaV = (Next.Periapsis.Velocity - Capture.Periapsis.Velocity).Abs();
    }

    public Orbit Initial { get; }
    public Orbit Next { get; }
    public Orbit Capture { get; }
    public Velocity DeltaV { get; }
    public OrbitalManeuverPoint ManeuverPoint { get; }
    public OrbitalManeuverPoint TargetBodyManeuverPoint { get; }

    public override string ToString()
    {
        var lines = new[]
        {
            $"dV: {DeltaV} at {ManeuverPoint} point (capture)",
            $"Initial[{Initial}]",
            $"Next[{Next}]",
            $"Capture[{Capture}] at {TargetBodyManeuverPoint} target point"
        };

        return string.Join( Environment.NewLine, lines );
    }
}
