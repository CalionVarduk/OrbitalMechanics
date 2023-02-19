using System;
using System.Collections.Generic;

namespace OrbitalMechanics.Bodies;

public sealed class PlanetarySystem
{
    private readonly Dictionary<string, CelestialBody> _bodies;

    public PlanetarySystem()
    {
        _bodies = new Dictionary<string, CelestialBody>();
    }

    public IReadOnlyDictionary<string, CelestialBody> Bodies => _bodies;

    public PlanetarySystem AddBody(CelestialBody body)
    {
        if ( body.Orbit is not null && ! _bodies.ContainsKey( body.Orbit.Parent.Name ) )
            throw new ArgumentException( $"Body's parent '{body.Orbit.Parent.Name}' does not exist in this system.", nameof( body ) );

        AddBodyToDictionary( body );
        return this;
    }

    private void AddBodyToDictionary(CelestialBody body)
    {
        _bodies.TryAdd( body.Name, body );
        foreach ( var child in body.Children.Values )
            AddBodyToDictionary( child );
    }
}
