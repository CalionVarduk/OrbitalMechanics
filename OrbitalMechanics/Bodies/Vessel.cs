using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Bodies;

public sealed class Vessel
{
    public Vessel(Engine engine, Mass dryMass, Mass propellantMass = default)
    {
        Engine = engine;
        DryMass = dryMass;
        PropellantMass = propellantMass;
    }

    public Engine Engine { get; }
    public Mass DryMass { get; }
    public Mass PropellantMass { get; }
    public Mass Mass => DryMass + PropellantMass;
    public Velocity AvailableDeltaV => Engine.GetAvailableDeltaV( DryMass, PropellantMass );
    public Acceleration Acceleration => Engine.Thrust / Mass;

    public Force GetWeight(Acceleration gravitationalAcceleration)
    {
        return Mass * gravitationalAcceleration;
    }

    public Ratio GetThrustToWeightRatio(Acceleration gravitationalAcceleration)
    {
        return Engine.Thrust / GetWeight( gravitationalAcceleration );
    }

    public VesselManeuver GetManeuver(Velocity deltaV, Ratio? thrustRatio = null)
    {
        return new VesselManeuver( this, deltaV, thrustRatio ?? Ratio.One );
    }

    public VesselManeuver GetManeuver(Mass propellantMass, Ratio? thrustRatio = null)
    {
        if ( propellantMass < Mass.Zero )
            return GetManeuver( Velocity.Zero, thrustRatio );

        if ( propellantMass > PropellantMass )
            return GetManeuver( Distance.FromKilometers( double.PositiveInfinity ) / Universe.OneSecond, thrustRatio );

        var remainingDeltaV = Engine.GetAvailableDeltaV( DryMass, PropellantMass - propellantMass );
        return GetManeuver( AvailableDeltaV - remainingDeltaV, thrustRatio );
    }

    public VesselManeuver GetManeuver(TimeSpan dt, Ratio? thrustRatio = null)
    {
        var propellantMass = Engine.PropellantFlowRate * dt * (thrustRatio?.Clamp( Ratio.Zero, Ratio.One ) ?? Ratio.One);
        return GetManeuver( propellantMass, thrustRatio );
    }

    public Vessel AddPropellant(Mass mass)
    {
        return new Vessel( Engine, DryMass, PropellantMass + mass );
    }

    public Vessel RemovePropellant(Mass mass)
    {
        return new Vessel( Engine, DryMass, PropellantMass - mass );
    }

    public Vessel AddDryMass(Mass mass)
    {
        return new Vessel( Engine, DryMass + mass, PropellantMass );
    }

    public Vessel RemoveDryMass(Mass mass)
    {
        return new Vessel( Engine, DryMass - mass, PropellantMass );
    }

    public override string ToString()
    {
        return
            $"Mass: {Mass} (Dry: {DryMass}, Propellant: {PropellantMass}), Engine: ({Engine}), dV: {AvailableDeltaV}, Acceleration: {Acceleration}";
    }
}
