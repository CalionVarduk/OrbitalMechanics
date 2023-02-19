using System;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits;

// TODO: this doesn't work very well for hyperbolic orbits
public sealed class OrbitPoint
{
    private OrbitPoint(
        Angle meanAnomaly,
        Angle eccentricAnomaly,
        Angle trueAnomaly,
        Angle angularPosition,
        Distance radius,
        Distance altitude,
        Velocity velocity,
        TimeSpan timeSincePeriapsis,
        Acceleration gravitationalAcceleration)
    {
        MeanAnomaly = meanAnomaly;
        EccentricAnomaly = eccentricAnomaly;
        TrueAnomaly = trueAnomaly;
        AngularPosition = angularPosition;
        Radius = radius;
        Altitude = altitude;
        Velocity = velocity;
        TimeSincePeriapsis = timeSincePeriapsis;
        GravitationalAcceleration = gravitationalAcceleration;
    }

    public Angle MeanAnomaly { get; }
    public Angle EccentricAnomaly { get; }
    public Angle TrueAnomaly { get; }
    public Angle AngularPosition { get; }
    public Distance Radius { get; }
    public Distance Altitude { get; }
    public Velocity Velocity { get; }
    public TimeSpan TimeSincePeriapsis { get; }
    public Acceleration GravitationalAcceleration { get; }

    public override string ToString()
    {
        return $"Radius: {Radius}, Velocity: {Velocity}, Angular position: {AngularPosition}";
    }

    public static OrbitPoint CreatePeriapsis(
        CelestialBody parent,
        Distance radius,
        AngularMomentum angularMomentum,
        Angle longitudeOfAscendingNode,
        Angle argumentOfPeriapsis)
    {
        var altitude = parent.GetAltitude( radius );

        return new OrbitPoint(
            Angle.Zero,
            Angle.Zero,
            Angle.Zero,
            GetAngularPositionFromTrueAnomaly( Angle.Zero, longitudeOfAscendingNode, argumentOfPeriapsis ),
            radius,
            altitude,
            angularMomentum / radius,
            TimeSpan.Zero,
            parent.GetGravitationalAcceleration( altitude ) );
    }

    public static OrbitPoint CreateApoapsis(
        CelestialBody parent,
        Distance radius,
        AngularMomentum angularMomentum,
        TimeSpan orbitalPeriod,
        Angle longitudeOfAscendingNode,
        Angle argumentOfPeriapsis)
    {
        var altitude = parent.GetAltitude( radius );

        return new OrbitPoint(
            Angle.Half,
            Angle.Half,
            Angle.Half,
            GetAngularPositionFromTrueAnomaly( Angle.Half, longitudeOfAscendingNode, argumentOfPeriapsis ),
            radius,
            altitude,
            angularMomentum / radius,
            orbitalPeriod * 0.5,
            parent.GetGravitationalAcceleration( altitude ) );
    }

    public static OrbitPoint CreateFromTrueAnomaly(Orbit orbit, Angle trueAnomaly)
    {
        trueAnomaly = trueAnomaly.NormalizePositive();
        var eccentricAnomaly = GetEccentricAnomalyByTrueAnomaly( trueAnomaly, orbit.Eccentricity );
        var meanAnomaly = GetMeanAnomalyByEccentricAnomaly( eccentricAnomaly, orbit.Eccentricity );
        var radius = GetRadiusByEccentricAnomaly( eccentricAnomaly, orbit.SemiMajorAxis, orbit.Eccentricity );
        var timeSincePeriapsis = GetTimeSincePeriapsisByMeanAnomaly( meanAnomaly, orbit.Period );
        var altitude = orbit.Parent.GetAltitude( radius );

        return new OrbitPoint(
            meanAnomaly,
            eccentricAnomaly,
            trueAnomaly,
            GetAngularPositionFromTrueAnomaly( trueAnomaly, orbit.LongitudeOfAscendingNode, orbit.ArgumentOfPeriapsis ),
            radius,
            altitude,
            orbit.AngularMomentum / radius,
            timeSincePeriapsis,
            orbit.Parent.GetGravitationalAcceleration( altitude ) );
    }

    public static OrbitPoint CreateFromMeanAnomaly(Orbit orbit, Angle meanAnomaly)
    {
        meanAnomaly = meanAnomaly.NormalizePositive();
        var trueAnomaly = GetTrueAnomalyByMeanAnomaly( meanAnomaly, orbit.Eccentricity );
        var eccentricAnomaly = GetEccentricAnomalyByTrueAnomaly( trueAnomaly, orbit.Eccentricity );
        var radius = GetRadiusByEccentricAnomaly( eccentricAnomaly, orbit.SemiMajorAxis, orbit.Eccentricity );
        var timeSincePeriapsis = GetTimeSincePeriapsisByMeanAnomaly( meanAnomaly, orbit.Period );
        var altitude = orbit.Parent.GetAltitude( radius );

        return new OrbitPoint(
            meanAnomaly,
            eccentricAnomaly,
            trueAnomaly,
            GetAngularPositionFromTrueAnomaly( trueAnomaly, orbit.LongitudeOfAscendingNode, orbit.ArgumentOfPeriapsis ),
            radius,
            altitude,
            orbit.AngularMomentum / radius,
            timeSincePeriapsis,
            orbit.Parent.GetGravitationalAcceleration( altitude ) );
    }

    private static Angle GetEccentricAnomalyByTrueAnomaly(Angle trueAnomaly, Eccentricity eccentricity)
    {
        var trueAnomalyCos = trueAnomaly.Cos();
        var eccentricAnomalyCos = (eccentricity.Value + trueAnomalyCos) / (1 + eccentricity.Value * trueAnomalyCos);

        var result = Angle.FromRadians( eccentricity.IsElliptic ? Math.Acos( eccentricAnomalyCos ) : Math.Acosh( eccentricAnomalyCos ) );
        if ( trueAnomaly > Angle.Half )
            result = Angle.Full - result;

        return result;
    }

    private static Angle GetMeanAnomalyByEccentricAnomaly(Angle eccentricAnomaly, Eccentricity eccentricity)
    {
        var result = eccentricity.IsElliptic
            ? eccentricAnomaly - Angle.FromRadians( eccentricAnomaly.Sin() ) * eccentricity.Value
            : Angle.FromRadians( eccentricAnomaly.Sinh() ) * eccentricity.Value - eccentricAnomaly;

        return result;
    }

    private static Distance GetRadiusByEccentricAnomaly(Angle eccentricAnomaly, Distance semiMajorAxis, Eccentricity eccentricity)
    {
        var result = semiMajorAxis * (1 - eccentricity.Value * eccentricAnomaly.Cos());
        return result;
    }

    private static TimeSpan GetTimeSincePeriapsisByMeanAnomaly(Angle meanAnomaly, TimeSpan orbitalPeriod)
    {
        var result = meanAnomaly / (Angle.Full / orbitalPeriod);
        if ( result < TimeSpan.Zero )
            result += orbitalPeriod;

        return result;
    }

    private static Angle GetTrueAnomalyByMeanAnomaly(Angle meanAnomaly, Eccentricity eccentricity)
    {
        if ( meanAnomaly == Angle.Zero || meanAnomaly == Angle.Half )
            return meanAnomaly;

        var epsilon = Angle.FromRadians( 0.000000000000001 );
        var minTrue = meanAnomaly < Angle.Half ? Angle.Zero : Angle.Half;
        var maxTrue = meanAnomaly < Angle.Half ? Angle.Half : Angle.Full;

        while ( maxTrue - minTrue > epsilon )
        {
            var midTrue = (minTrue + maxTrue) * 0.5;
            var midEcc = GetEccentricAnomalyByTrueAnomaly( midTrue, eccentricity );
            var midMean = GetMeanAnomalyByEccentricAnomaly( midEcc, eccentricity );

            if ( midMean < meanAnomaly )
                minTrue = midTrue;
            else
                maxTrue = midTrue;
        }

        return (minTrue + maxTrue) * 0.5;
    }

    private static Angle GetAngularPositionFromTrueAnomaly(Angle trueAnomaly, Angle longitudeOfAscendingNode, Angle argumentOfPeriapsis)
    {
        return (trueAnomaly + longitudeOfAscendingNode + argumentOfPeriapsis).Normalize();
    }
}
