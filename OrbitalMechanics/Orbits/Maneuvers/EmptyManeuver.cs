using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public sealed class EmptyManeuver : IOrbitalManeuver
{
    public EmptyManeuver(Orbit orbit)
    {
        Initial = orbit;
    }

    public Orbit Initial { get; }
    public Orbit Next => Initial;
    public OrbitalManeuverPoint ManeuverPoint => OrbitalManeuverPoint.Unspecified;
    public Velocity DeltaV => Velocity.Zero;

    public override string ToString()
    {
        return $"Orbit[{Initial}]";
    }
}
