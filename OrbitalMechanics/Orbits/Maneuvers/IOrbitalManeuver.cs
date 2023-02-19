using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits.Maneuvers;

public interface IOrbitalManeuver
{
    public Orbit Initial { get; }
    public Orbit Next { get; }
    public Velocity DeltaV { get; }
    public OrbitalManeuverPoint ManeuverPoint { get; }
}
