using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public sealed class EscapeManeuver : IOrbitalManeuver
{
    public EscapeManeuver(
        Orbit parkingOrbit,
        Distance targetRadius,
        OrbitalManeuverPoint maneuverPoint = OrbitalManeuverPoint.Periapsis,
        OrbitalManeuverPoint sourceBodyManeuverPoint = OrbitalManeuverPoint.Unspecified)
    {
        var sourceBody = parkingOrbit.Parent;
        if ( sourceBody.Orbit is null )
            throw new ArgumentException( "Parking orbit's parent body must have an orbit.", nameof( parkingOrbit ) );

        Initial = parkingOrbit;
        ManeuverPoint = maneuverPoint;
        SourceBodyManeuverPoint = sourceBodyManeuverPoint;

        var sourceRadius = sourceBody.Orbit.GetRadius( SourceBodyManeuverPoint );
        Next = EllipticOrbit.FromRadii(
            parent: sourceBody.Orbit.Parent,
            periapsis: sourceRadius < targetRadius ? sourceRadius : targetRadius,
            apoapsis: sourceRadius > targetRadius ? sourceRadius : targetRadius,
            inclination: sourceBody.Orbit.Inclination );

        var velocityAtSource = sourceRadius < targetRadius ? Next.Periapsis.Velocity : Next.Apoapsis.Velocity;
        var excessSpeed = (sourceBody.Orbit.GetVelocity( SourceBodyManeuverPoint ) - velocityAtSource).Abs();

        Escape = HyperbolicOrbit.FromExcessSpeed(
            parent: Initial.Parent,
            periapsis: Initial.GetRadius( ManeuverPoint ),
            excessSpeed: excessSpeed,
            inclination: Initial.Inclination );

        DeltaV = (Escape.Periapsis.Velocity - Initial.GetVelocity( ManeuverPoint )).Abs();
    }

    public Orbit Initial { get; }
    public Orbit Next { get; }
    public Velocity DeltaV { get; }
    public OrbitalManeuverPoint ManeuverPoint { get; }
    public Orbit Escape { get; }
    public OrbitalManeuverPoint SourceBodyManeuverPoint { get; }
    public Angle ImpulseAngle => Angle.FromRadians( Math.Acos( -1 / Escape.Eccentricity.Value ) );

    public override string ToString()
    {
        var lines = new[]
        {
            $"dV: {DeltaV} at {ManeuverPoint} point (escape)",
            $"Impulse angle: {ImpulseAngle}",
            $"Initial[{Initial}]",
            $"Next[{Next}]",
            $"Escape[{Escape}] at {SourceBodyManeuverPoint} source point"
        };

        return string.Join( Environment.NewLine, lines );
    }
}
