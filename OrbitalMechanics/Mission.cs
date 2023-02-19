using System.Collections.Generic;
using System.Linq;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.Orbits;
using OrbitalMechanics.Orbits.Maneuvers;
using OrbitalMechanics.SI;

namespace OrbitalMechanics;

public sealed class Mission
{
    private readonly List<MissionManeuver> _maneuvers;

    public Mission(string description, Vessel vessel, Orbit startingOrbit)
    {
        Description = description;
        _maneuvers = new List<MissionManeuver> { MissionManeuver.FromOrbital( vessel, new EmptyManeuver( startingOrbit ), "Initial" ) };
    }

    public string Description { get; }
    public IReadOnlyList<MissionManeuver> Maneuvers => _maneuvers;
    public Vessel Vessel => _maneuvers[^1].NextVessel;
    public Orbit Orbit => _maneuvers.Last( m => m.OrbitalManeuver is not null ).OrbitalManeuver!.Next;

    public Mission AddOrbitalManeuver(IOrbitalManeuver maneuver, string description = "")
    {
        var result = MissionManeuver.FromOrbital( Vessel, maneuver, description );
        _maneuvers.Add( result );
        return this;
    }

    public Mission AddPayloadDrop(Mass mass, string description = "")
    {
        var result = MissionManeuver.FromPayloadDrop( Vessel, mass, description );
        _maneuvers.Add( result );
        return this;
    }
}
