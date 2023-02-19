using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Bodies;

public sealed class VesselManeuver
{
    public VesselManeuver(Vessel vessel, Velocity deltaV, Ratio thrustRatio)
    {
        Vessel = vessel;
        ThrustRatio = thrustRatio.Clamp( Ratio.Zero, Ratio.One );

        if ( deltaV <= Velocity.Zero )
        {
            DeltaV = Velocity.Zero;
            UsedPropellant = Mass.Zero;
            ElapsedTime = TimeSpan.Zero;
            return;
        }

        DeltaV = deltaV;

        if ( ThrustRatio == 0 )
        {
            UsedPropellant = Mass.Zero;
            ElapsedTime = TimeSpan.MaxValue;
            return;
        }

        var availableDeltaV = Vessel.AvailableDeltaV;

        if ( deltaV > availableDeltaV )
        {
            UsedPropellant = Vessel.PropellantMass;
            ElapsedTime = TimeSpan.MaxValue;
            return;
        }

        UsedPropellant = Vessel.PropellantMass - Vessel.Engine.GetPropellantMass( Vessel.DryMass, availableDeltaV - DeltaV );
        ElapsedTime = UsedPropellant / (Vessel.Engine.PropellantFlowRate * ThrustRatio);
    }

    public Vessel Vessel { get; }
    public Velocity DeltaV { get; }
    public Mass UsedPropellant { get; }
    public TimeSpan ElapsedTime { get; }
    public Ratio ThrustRatio { get; }
    public Force Thrust => Vessel.Engine.Thrust * ThrustRatio;
    public Ratio UsedPropellantRatio => Vessel.PropellantMass != Mass.Zero ? UsedPropellant / Vessel.PropellantMass : Ratio.One;
    public Vessel FinalVessel => Vessel.RemovePropellant( UsedPropellant );

    public override string ToString()
    {
        return $"dV: {DeltaV}, Used propellant mass: {UsedPropellant}, Time: {ElapsedTime.TotalSeconds:N3} s, Thrust: {Thrust}";
    }
}
