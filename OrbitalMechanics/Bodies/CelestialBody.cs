using System;
using System.Collections.Generic;
using System.Linq;
using OrbitalMechanics.Orbits;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Bodies;

public sealed class CelestialBody
{
    private readonly Dictionary<string, CelestialBody> _children = new Dictionary<string, CelestialBody>();

    public CelestialBody(string name, Mass mass, Distance radius, Distance atmosphereHeight = default, OrbitInfo? orbit = null)
    {
        if ( mass <= Mass.Zero )
            throw new ArgumentException( "Mass must be greater than 0 kg.", nameof( mass ) );

        if ( radius <= Distance.Zero )
            throw new ArgumentException( "Radius must be greater than 0 km.", nameof( radius ) );

        if ( atmosphereHeight < Distance.Zero )
            throw new ArgumentException( "Atmosphere height cannot be less than 0 km.", nameof( atmosphereHeight ) );

        Name = name;
        Mass = mass;
        Radius = radius;
        AtmosphereHeight = atmosphereHeight;

        if ( orbit is null )
        {
            Orbit = null;
            return;
        }

        Orbit = Orbit.Create( orbit.Value );
        Orbit.Parent._children.Add( Name, this );
    }

    public string Name { get; }
    public Mass Mass { get; }
    public Distance Radius { get; }
    public Distance AtmosphereHeight { get; }
    public Orbit? Orbit { get; }

    public Distance StableApoapsisRadius =>
        _children.Count > 0
            ? _children.Values.Select( c => c.Orbit!.Periapsis.Radius - c.SphereOfInfluenceRadius ).Min()
            : SphereOfInfluenceRadius;

    public Distance SphereOfInfluenceRadius =>
        Orbit is null
            ? Distance.FromKilometers( double.PositiveInfinity )
            : Orbit.SemiMajorAxis * Math.Pow( Mass / Orbit.Parent.Mass, 0.4 );

    public GravitationalParameter GravitationalParameter => GravitationalParameter.Create( Mass );
    public Acceleration SurfaceGravity => GravitationalParameter / Radius / Radius;
    public Velocity EscapeVelocity => (SurfaceGravity * Radius * 2).Sqrt();
    public Distance EquatorialCircumference => Radius * Math.Tau;
    public Area SurfaceArea => Radius * Radius * (Math.Tau * 2);

    public IReadOnlyDictionary<string, CelestialBody> Children => _children;

    public Acceleration GetGravitationalAcceleration(Distance altitude)
    {
        var scale = Radius / GetRadius( altitude );
        return SurfaceGravity * (scale * scale);
    }

    public Distance GetAltitude(Distance radius)
    {
        return radius - Radius;
    }

    public Distance GetRadius(Distance altitude)
    {
        return altitude + Radius;
    }

    public CelestialBody AddChild(
        string name,
        Mass mass,
        Distance radius,
        Distance semiMajorAxis,
        Eccentricity eccentricity,
        Angle inclination = default,
        Angle argumentOfPeriapsis = default,
        Angle longitudeOfAscendingNode = default,
        Distance atmosphereHeight = default,
        Angle meanAnomalyAtEpoch = default)
    {
        var child = new CelestialBody(
            name,
            mass,
            radius,
            atmosphereHeight,
            new OrbitInfo
            {
                Parent = this,
                SemiMajorAxis = semiMajorAxis,
                Eccentricity = eccentricity,
                Inclination = inclination,
                ArgumentOfPeriapsis = argumentOfPeriapsis,
                LongitudeOfAscendingNode = longitudeOfAscendingNode,
                MeanAnomalyAtEpoch = meanAnomalyAtEpoch
            } );

        return child;
    }

    public override string ToString()
    {
        return Name;
    }
}
