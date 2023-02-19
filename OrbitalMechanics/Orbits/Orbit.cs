using System;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.Orbits.Maneuvers;
using OrbitalMechanics.SI;

namespace OrbitalMechanics.Orbits;

public abstract class Orbit
{
    protected Orbit(
        CelestialBody parent,
        Distance semiMajorAxis,
        Eccentricity eccentricity,
        Angle inclination,
        Angle argumentOfPeriapsis,
        Angle longitudeOfAscendingNode,
        Angle meanAnomalyAtEpoch)
    {
        if ( semiMajorAxis <= Distance.Zero )
            throw new ArgumentException( "Semi-major axis must be greater than 0 km.", nameof( semiMajorAxis ) );

        Parent = parent;
        SemiMajorAxis = semiMajorAxis;
        Eccentricity = eccentricity;

        Inclination = inclination.NormalizePositive();
        if ( Inclination > Angle.Half )
            Inclination -= Angle.Half;

        ArgumentOfPeriapsis = argumentOfPeriapsis.NormalizePositive();
        LongitudeOfAscendingNode = longitudeOfAscendingNode.NormalizePositive();

        var periapsis = eccentricity.IsElliptic ? SemiMajorAxis * (1 - Eccentricity.Value) : SemiMajorAxis * (Eccentricity.Value - 1);
        var apoapsis = eccentricity.IsElliptic ? SemiMajorAxis * (1 + Eccentricity.Value) : -SemiMajorAxis * (1 + Eccentricity.Value);

        AngularMomentum = AngularMomentum.CreatePerSecond(
            Area.FromSquareMeters(
                Math.Sqrt( 2 * Parent.GravitationalParameter.Value * (apoapsis * (periapsis / (periapsis + apoapsis))).Meters ) ) );

        Period = TimeSpan.FromSeconds(
            Math.Tau / Math.Sqrt( Parent.GravitationalParameter.Value ) * Math.Pow( SemiMajorAxis.Meters, 1.5 ) );

        Periapsis = OrbitPoint.CreatePeriapsis( Parent, periapsis, AngularMomentum, LongitudeOfAscendingNode, ArgumentOfPeriapsis );
        Apoapsis = OrbitPoint.CreateApoapsis( Parent, apoapsis, AngularMomentum, Period, LongitudeOfAscendingNode, ArgumentOfPeriapsis );
        PointAtEpoch = OrbitPoint.CreateFromMeanAnomaly( this, meanAnomalyAtEpoch );
    }

    public CelestialBody Parent { get; }
    public Distance SemiMajorAxis { get; }
    public Eccentricity Eccentricity { get; }
    public Angle Inclination { get; }
    public Angle ArgumentOfPeriapsis { get; }
    public Angle LongitudeOfAscendingNode { get; }
    public AngularMomentum AngularMomentum { get; }
    public TimeSpan Period { get; }
    public OrbitPoint Periapsis { get; }
    public OrbitPoint Apoapsis { get; }
    public OrbitPoint PointAtEpoch { get; }

    public abstract Distance SemiMinorAxis { get; }
    public abstract OrbitalEnergy Energy { get; }

    public Distance ParentCenterOffset => SemiMajorAxis * Eccentricity.Value;
    public Velocity SemiMajorAxisVelocity => AngularMomentum / SemiMajorAxis;
    public AngularMotion MeanMotion => Angle.Full / Period;
    public Distance SemiLatusRectum => SemiMajorAxis * (1 - Math.Pow( Eccentricity.Value, 2 ));

    public Angle GetAngularAlignment(Orbit other)
    {
        if ( Parent != other.Parent )
            return Angle.FromRadians( double.NaN );

        var result = Angle.Half * (1 - 1 / (2 * Math.Sqrt( 2 )) * Math.Sqrt( Math.Pow( SemiMajorAxis / other.SemiMajorAxis + 1, 3 ) ));
        return result.Normalize();
    }

    public TimeSpan GetSynodicPeriod(Orbit other)
    {
        var result = TimeSpan.FromSeconds( 1 / Math.Abs( 1 / Period.TotalSeconds - 1 / other.Period.TotalSeconds ) );
        return result;
    }

    public TimeSpan GetTimeToNextTransferWindow(Orbit other, TimeSpan timeSinceEpoch)
    {
        if ( ! Eccentricity.IsElliptic ||
            ! other.Eccentricity.IsElliptic ||
            Parent != other.Parent ||
            SemiMajorAxis == other.SemiMajorAxis )
            return TimeSpan.MaxValue;

        Orbit inner;
        Orbit outer;
        if ( SemiMajorAxis > other.SemiMajorAxis )
        {
            inner = other;
            outer = this;
        }
        else
        {
            inner = this;
            outer = other;
        }

        var expectedAlignment = inner.GetAngularAlignment( outer );
        var epsilon = TimeSpan.FromMilliseconds( 1 );
        var lo = TimeSpan.Zero;
        var hi = inner.Period;

        while ( hi - lo > epsilon )
        {
            var mid = (lo + hi) * 0.5;
            var innerPoint = inner.GetPointByElapsedTime( timeSinceEpoch + mid );
            var outerPoint = outer.GetPointByElapsedTime( timeSinceEpoch + mid );

            var alignment = outerPoint.AngularPosition - innerPoint.AngularPosition;
            if ( alignment < Angle.Zero )
                alignment = Angle.Full + alignment;

            var alignmentDelta = alignment - expectedAlignment;
            if ( alignmentDelta > Angle.Zero )
                lo = mid;
            else
                hi = mid;
        }

        return (lo + hi) * 0.5;
    }

    public Distance GetRadius(OrbitalManeuverPoint point)
    {
        return point switch
        {
            OrbitalManeuverPoint.Unspecified => SemiMajorAxis,
            OrbitalManeuverPoint.Periapsis => Periapsis.Radius,
            OrbitalManeuverPoint.Apoapsis => Apoapsis.Radius,
            _ => throw new ArgumentException( $"'{point}' is not a valid orbital maneuver point.", nameof( point ) )
        };
    }

    public Velocity GetVelocity(OrbitalManeuverPoint point)
    {
        return point switch
        {
            OrbitalManeuverPoint.Unspecified => SemiMajorAxisVelocity,
            OrbitalManeuverPoint.Periapsis => Periapsis.Velocity,
            OrbitalManeuverPoint.Apoapsis => Apoapsis.Velocity,
            _ => throw new ArgumentException( $"'{point}' is not a valid orbital maneuver point.", nameof( point ) )
        };
    }

    public EllipticOrbit Circularize()
    {
        if ( Eccentricity.IsCircular && this is EllipticOrbit e )
            return e;

        return new EllipticOrbit(
            Parent,
            SemiMajorAxis,
            Eccentricity.Circular,
            Inclination,
            ArgumentOfPeriapsis,
            LongitudeOfAscendingNode,
            PointAtEpoch.MeanAnomaly );
    }

    public OrbitPoint GetPointByTrueAnomaly(Angle trueAnomaly)
    {
        return OrbitPoint.CreateFromTrueAnomaly( this, trueAnomaly );
    }

    public OrbitPoint GetPointByMeanAnomaly(Angle meanAnomaly)
    {
        return OrbitPoint.CreateFromMeanAnomaly( this, meanAnomaly );
    }

    public OrbitPoint GetPointByElapsedTime(TimeSpan dt)
    {
        return GetPointByMeanAnomaly( PointAtEpoch.MeanAnomaly + MeanMotion * dt );
    }

    public OrbitPoint GetPointByPeriodRatio(Ratio ratio)
    {
        return GetPointByMeanAnomaly( MeanMotion * Period * ratio );
    }

    public static Orbit Create(OrbitInfo info)
    {
        if ( info.Eccentricity.IsElliptic )
            return new EllipticOrbit(
                info.Parent,
                info.SemiMajorAxis,
                info.Eccentricity,
                info.Inclination,
                info.ArgumentOfPeriapsis,
                info.LongitudeOfAscendingNode,
                info.MeanAnomalyAtEpoch );

        if ( info.Eccentricity.IsHyperbolic )
            return new HyperbolicOrbit(
                info.Parent,
                info.SemiMajorAxis,
                info.Eccentricity,
                info.Inclination,
                info.ArgumentOfPeriapsis,
                info.LongitudeOfAscendingNode,
                info.MeanAnomalyAtEpoch );

        throw new ArgumentException( "Parabolic orbits are unsupported.", nameof( info ) );
    }

    public sealed override string ToString()
    {
        var parentText = $"Parent: {Parent.Name}";
        var radiiText = GetRadiiText();
        var eccentricityText = $"Eccentricity: {Eccentricity}";
        var inclinationText = $"Inclination: {Inclination}";
        var velocityText = GetVelocityText();
        return $"{parentText}, {radiiText}, {eccentricityText}, {inclinationText}, {velocityText}";
    }

    private string GetRadiiText()
    {
        if ( Eccentricity.IsCircular )
            return $"Radius: {SemiMajorAxis}";

        if ( Eccentricity.IsElliptic )
            return $"Semi-major axis: {SemiMajorAxis} (Pe: {Periapsis.Radius}, Ap: {Apoapsis.Radius})";

        return $"Semi-major axis: {SemiMajorAxis} (Pe: {Periapsis.Radius})";
    }

    private string GetVelocityText()
    {
        if ( Eccentricity.IsCircular )
            return $"Velocity: {Periapsis.Velocity}";

        if ( Eccentricity.IsElliptic )
            return $"Velocity: {SemiMajorAxisVelocity} (Ap: {Apoapsis.Velocity}, Pe: {Periapsis.Velocity})";

        return $"Velocity: {SemiMajorAxisVelocity} (Pe: {Periapsis.Velocity})";
    }
}
