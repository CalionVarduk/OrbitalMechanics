using System;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public sealed class GravityAssistManeuver : IOrbitalManeuver
{
    public GravityAssistManeuver(
        Distance sourceRadius,
        CelestialBody assistBody,
        Distance assistAltitude,
        Distance targetRadius,
        OrbitalManeuverPoint assistBodyManeuverPoint = OrbitalManeuverPoint.Unspecified)
    {
        if ( assistBody.Orbit is null )
            throw new ArgumentException( "Assist body must have an orbit.", nameof( assistBody ) );

        ManeuverPoint = OrbitalManeuverPoint.Periapsis;
        AssistBodyManeuverPoint = assistBodyManeuverPoint;
        var assistRadius = assistBody.Orbit.GetRadius( AssistBodyManeuverPoint );

        Initial = EllipticOrbit.FromRadii(
            parent: assistBody.Orbit.Parent,
            periapsis: sourceRadius < assistRadius ? sourceRadius : assistRadius,
            apoapsis: sourceRadius > assistRadius ? sourceRadius : assistRadius,
            inclination: assistBody.Orbit.Inclination );

        Next = EllipticOrbit.FromRadii(
            parent: assistBody.Orbit.Parent,
            periapsis: targetRadius < assistRadius ? targetRadius : assistRadius,
            apoapsis: targetRadius > assistRadius ? targetRadius : assistRadius,
            inclination: assistBody.Orbit.Inclination );

        var velocityAtAssist = sourceRadius > assistRadius ? Initial.Periapsis.Velocity : Initial.Apoapsis.Velocity;
        var excessSpeed = (assistBody.Orbit.GetVelocity( AssistBodyManeuverPoint ) - velocityAtAssist).Abs();

        Capture = HyperbolicOrbit.FromExcessSpeed(
            parent: assistBody,
            periapsis: assistBody.GetRadius( assistAltitude ),
            excessSpeed: excessSpeed );

        velocityAtAssist = assistRadius < targetRadius ? Next.Periapsis.Velocity : Next.Apoapsis.Velocity;
        excessSpeed = (assistBody.Orbit.GetVelocity( AssistBodyManeuverPoint ) - velocityAtAssist).Abs();

        Escape = HyperbolicOrbit.FromExcessSpeed(
            parent: assistBody,
            periapsis: Capture.Periapsis.Radius,
            excessSpeed: excessSpeed );

        DeltaV = (Capture.Periapsis.Velocity - Escape.Periapsis.Velocity).Abs();
    }

    public Orbit Initial { get; }
    public Orbit Next { get; }
    public Velocity DeltaV { get; }
    public OrbitalManeuverPoint ManeuverPoint { get; }
    public OrbitalManeuverPoint AssistBodyManeuverPoint { get; }
    public Orbit Capture { get; }
    public Orbit Escape { get; }
}
