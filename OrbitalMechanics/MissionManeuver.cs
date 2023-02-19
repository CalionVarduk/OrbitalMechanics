using System;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.Orbits.Maneuvers;
using OrbitalMechanics.SI;

namespace OrbitalMechanics;

public sealed class MissionManeuver
{
    public readonly struct VesselManeuverPair
    {
        public VesselManeuverPair(VesselManeuver firstHalf, VesselManeuver secondHalf)
        {
            FirstHalf = firstHalf;
            SecondHalf = secondHalf;
        }

        public VesselManeuver FirstHalf { get; }
        public VesselManeuver SecondHalf { get; }
    }

    private MissionManeuver(
        Vessel initialVessel,
        Vessel nextVessel,
        Velocity deltaV,
        TimeSpan elapsedTime,
        IOrbitalManeuver? orbitalManeuver,
        VesselManeuverPair? vesselManeuver,
        string description)
    {
        InitialVessel = initialVessel;
        NextVessel = nextVessel;
        DeltaV = deltaV;
        ElapsedTime = elapsedTime;
        OrbitalManeuver = orbitalManeuver;
        VesselManeuver = vesselManeuver;
        Description = description;
    }

    public Vessel InitialVessel { get; }
    public Vessel NextVessel { get; }
    public Velocity DeltaV { get; }
    public TimeSpan ElapsedTime { get; }
    public IOrbitalManeuver? OrbitalManeuver { get; }
    public VesselManeuverPair? VesselManeuver { get; }
    public string Description { get; }

    public override string ToString()
    {
        var description = Description.Length > 0 ? $"{Description}, " : string.Empty;
        var delta = OrbitalManeuver is not null ? $"dV: {DeltaV}, dt: {ElapsedTime}, " : string.Empty;
        var vessel = $"Vessel[{NextVessel}]";
        return $"{description}{delta}{vessel}";
    }

    public static MissionManeuver FromOrbital(Vessel vessel, IOrbitalManeuver orbitalManeuver, string description = "")
    {
        var firstVesselManeuver = vessel.GetManeuver( orbitalManeuver.DeltaV * 0.5 );
        var secondVesselManeuver = firstVesselManeuver.FinalVessel.GetManeuver( orbitalManeuver.DeltaV * 0.5 );

        return new MissionManeuver(
            vessel,
            secondVesselManeuver.FinalVessel,
            orbitalManeuver.DeltaV,
            secondVesselManeuver.ElapsedTime == TimeSpan.MaxValue
                ? secondVesselManeuver.ElapsedTime
                : firstVesselManeuver.ElapsedTime + secondVesselManeuver.ElapsedTime,
            orbitalManeuver,
            new VesselManeuverPair( firstVesselManeuver, secondVesselManeuver ),
            description );
    }

    public static MissionManeuver FromPayloadDrop(Vessel vessel, Mass dryMass, string description = "")
    {
        return new MissionManeuver( vessel, vessel.RemoveDryMass( dryMass ), Velocity.Zero, TimeSpan.Zero, null, null, description );
    }
}
