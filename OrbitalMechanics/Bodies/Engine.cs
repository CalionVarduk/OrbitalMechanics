using System;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Bodies;

public sealed class Engine
{
    public Engine(TimeSpan specificImpulse, Force thrust)
    {
        if ( specificImpulse <= TimeSpan.Zero )
            throw new ArgumentException( "Specific impulse must be greater than 0 s.", nameof( specificImpulse ) );

        if ( thrust <= Force.Zero )
            throw new ArgumentException( "Thrust must be greater than 0 N.", nameof( thrust ) );

        SpecificImpulse = specificImpulse;
        Thrust = thrust;
    }

    public TimeSpan SpecificImpulse { get; }
    public Force Thrust { get; }
    public Velocity NaturalDeltaV => Universe.StandardGravity * SpecificImpulse;
    public MassFlowRate PropellantFlowRate => Thrust / NaturalDeltaV;

    public override string ToString()
    {
        return $"Isp: {SpecificImpulse.TotalSeconds:N3} s, Thrust: {Thrust}, Flow rate: {PropellantFlowRate}";
    }

    public Velocity GetAvailableDeltaV(Mass dryMass, Mass propellantMass)
    {
        return NaturalDeltaV * Math.Log( (dryMass + propellantMass) / dryMass );
    }

    public Mass GetPropellantMass(Mass dryMass, Velocity availableDeltaV)
    {
        return dryMass * (Math.Pow( Math.E, availableDeltaV / NaturalDeltaV ) - 1);
    }
}
