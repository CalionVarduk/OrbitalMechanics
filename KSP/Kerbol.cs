using OrbitalMechanics.Bodies;
using OrbitalMechanics.SI;

namespace KSP;

public static class Kerbol
{
    public static class Names
    {
        public const string Kerbol = nameof( Kerbol );
        public const string Moho = nameof( Moho );
        public const string Eve = nameof( Eve );
        public const string Gilly = nameof( Gilly );
        public const string Kerbin = nameof( Kerbin );
        public const string Mun = nameof( Mun );
        public const string Minmus = nameof( Minmus );
        public const string Duna = nameof( Duna );
        public const string Ike = nameof( Ike );
        public const string Dres = nameof( Dres );
        public const string Jool = nameof( Jool );
        public const string Laythe = nameof( Laythe );
        public const string Vall = nameof( Vall );
        public const string Tylo = nameof( Tylo );
        public const string Bop = nameof( Bop );
        public const string Pol = nameof( Pol );
        public const string Eeloo = nameof( Eeloo );
    }

    public static readonly PlanetarySystem System = CreateKerbolSystem();

    private static PlanetarySystem CreateKerbolSystem()
    {
        var result = new PlanetarySystem();

        var kerbol = new CelestialBody(
            name: Names.Kerbol,
            mass: Mass.FromKilograms( 1.7565459e28 ),
            radius: Distance.FromKilometers( 261600 ),
            atmosphereHeight: Distance.FromKilometers( 600 ) );

        AddMoho( kerbol );
        AddEve( kerbol );
        AddKerbin( kerbol );
        AddDuna( kerbol );
        AddDres( kerbol );
        AddJool( kerbol );
        AddEeloo( kerbol );

        result.AddBody( kerbol );
        return result;
    }

    private static CelestialBody AddMoho(CelestialBody kerbol)
    {
        return kerbol.AddChild(
            name: Names.Moho,
            mass: Mass.FromKilograms( 2.5263314e21 ),
            radius: Distance.FromKilometers( 250 ),
            semiMajorAxis: Distance.FromMeters( 5263138304 ),
            eccentricity: Eccentricity.Create( 0.2 ),
            inclination: Angle.FromDegrees( 7 ),
            argumentOfPeriapsis: Angle.FromDegrees( 15 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 70 ),
            meanAnomalyAtEpoch: Angle.Half );
    }

    private static CelestialBody AddEve(CelestialBody kerbol)
    {
        var eve = kerbol.AddChild(
            name: Names.Eve,
            mass: Mass.FromKilograms( 1.2243980e23 ),
            radius: Distance.FromKilometers( 700 ),
            atmosphereHeight: Distance.FromKilometers( 90 ),
            semiMajorAxis: Distance.FromMeters( 9832684544 ),
            eccentricity: Eccentricity.Create( 0.01 ),
            inclination: Angle.FromDegrees( 2.1 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 15 ),
            meanAnomalyAtEpoch: Angle.Half );

        eve.AddChild(
            name: Names.Gilly,
            mass: Mass.FromKilograms( 1.2420363e17 ),
            radius: Distance.FromKilometers( 13 ),
            semiMajorAxis: Distance.FromMeters( 31500000 ),
            eccentricity: Eccentricity.Create( 0.55 ),
            inclination: Angle.FromDegrees( 12 ),
            argumentOfPeriapsis: Angle.FromDegrees( 10 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 80 ),
            meanAnomalyAtEpoch: Angle.FromRadians( 0.9 ) );

        return eve;
    }

    private static CelestialBody AddKerbin(CelestialBody kerbol)
    {
        var kerbin = kerbol.AddChild(
            name: Names.Kerbin,
            mass: Mass.FromKilograms( 5.2915158e22 ),
            radius: Distance.FromKilometers( 600 ),
            atmosphereHeight: Distance.FromKilometers( 70 ),
            semiMajorAxis: Distance.FromMeters( 13599840256 ),
            eccentricity: Eccentricity.Circular,
            meanAnomalyAtEpoch: Angle.Half );

        kerbin.AddChild(
            name: Names.Mun,
            mass: Mass.FromKilograms( 9.7599066e20 ),
            radius: Distance.FromKilometers( 200 ),
            semiMajorAxis: Distance.FromMeters( 12000000 ),
            eccentricity: Eccentricity.Circular,
            meanAnomalyAtEpoch: Angle.FromRadians( 1.7 ) );

        kerbin.AddChild(
            name: Names.Minmus,
            mass: Mass.FromKilograms( 2.6457580e19 ),
            radius: Distance.FromKilometers( 60 ),
            semiMajorAxis: Distance.FromMeters( 47000000 ),
            eccentricity: Eccentricity.Circular,
            inclination: Angle.FromDegrees( 6 ),
            argumentOfPeriapsis: Angle.FromDegrees( 38 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 78 ),
            meanAnomalyAtEpoch: Angle.FromRadians( 0.9 ) );

        return kerbin;
    }

    private static CelestialBody AddDuna(CelestialBody kerbol)
    {
        var duna = kerbol.AddChild(
            name: Names.Duna,
            mass: Mass.FromKilograms( 4.5154270e21 ),
            radius: Distance.FromKilometers( 320 ),
            atmosphereHeight: Distance.FromKilometers( 50 ),
            semiMajorAxis: Distance.FromMeters( 20726155264 ),
            eccentricity: Eccentricity.Create( 0.051 ),
            inclination: Angle.FromDegrees( 0.06 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 135.5 ),
            meanAnomalyAtEpoch: Angle.Half );

        duna.AddChild(
            name: Names.Ike,
            mass: Mass.FromKilograms( 2.7821615e20 ),
            radius: Distance.FromKilometers( 130 ),
            semiMajorAxis: Distance.FromMeters( 3200000 ),
            eccentricity: Eccentricity.Create( 0.03 ),
            inclination: Angle.FromDegrees( 0.2 ),
            meanAnomalyAtEpoch: Angle.FromRadians( 1.7 ) );

        return duna;
    }

    private static CelestialBody AddDres(CelestialBody kerbol)
    {
        return kerbol.AddChild(
            name: Names.Dres,
            mass: Mass.FromKilograms( 3.2190937e20 ),
            radius: Distance.FromKilometers( 138 ),
            semiMajorAxis: Distance.FromMeters( 40839348203 ),
            eccentricity: Eccentricity.Create( 0.145 ),
            inclination: Angle.FromDegrees( 5 ),
            argumentOfPeriapsis: Angle.FromDegrees( 90 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 280 ),
            meanAnomalyAtEpoch: Angle.Half );
    }

    private static CelestialBody AddJool(CelestialBody kerbol)
    {
        var jool = kerbol.AddChild(
            name: Names.Jool,
            mass: Mass.FromKilograms( 4.2332127e24 ),
            radius: Distance.FromKilometers( 6000 ),
            atmosphereHeight: Distance.FromKilometers( 200 ),
            semiMajorAxis: Distance.FromMeters( 68773560320 ),
            eccentricity: Eccentricity.Create( 0.05 ),
            inclination: Angle.FromDegrees( 1.304 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 52 ),
            meanAnomalyAtEpoch: Angle.FromRadians( 0.1 ) );

        jool.AddChild(
            name: Names.Laythe,
            mass: Mass.FromKilograms( 2.9397311e22 ),
            radius: Distance.FromKilometers( 500 ),
            atmosphereHeight: Distance.FromKilometers( 50 ),
            semiMajorAxis: Distance.FromMeters( 27184000 ),
            eccentricity: Eccentricity.Circular,
            meanAnomalyAtEpoch: Angle.Half );

        jool.AddChild(
            name: Names.Vall,
            mass: Mass.FromKilograms( 3.1087655e21 ),
            radius: Distance.FromKilometers( 300 ),
            semiMajorAxis: Distance.FromMeters( 43152000 ),
            eccentricity: Eccentricity.Circular,
            meanAnomalyAtEpoch: Angle.FromRadians( 0.9 ) );

        jool.AddChild(
            name: Names.Tylo,
            mass: Mass.FromKilograms( 4.2332127e22 ),
            radius: Distance.FromKilometers( 600 ),
            semiMajorAxis: Distance.FromMeters( 68500000 ),
            eccentricity: Eccentricity.Circular,
            inclination: Angle.FromDegrees( 0.025 ),
            meanAnomalyAtEpoch: Angle.Half );

        jool.AddChild(
            name: Names.Bop,
            mass: Mass.FromKilograms( 3.7261090e19 ),
            radius: Distance.FromKilometers( 65 ),
            semiMajorAxis: Distance.FromMeters( 128500000 ),
            eccentricity: Eccentricity.Create( 0.235 ),
            inclination: Angle.FromDegrees( 15 ),
            argumentOfPeriapsis: Angle.FromDegrees( 25 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 10 ),
            meanAnomalyAtEpoch: Angle.FromRadians( 0.9 ) );

        jool.AddChild(
            name: Names.Pol,
            mass: Mass.FromKilograms( 1.0813507e19 ),
            radius: Distance.FromKilometers( 44 ),
            semiMajorAxis: Distance.FromMeters( 179890000 ),
            eccentricity: Eccentricity.Create( 0.171 ),
            inclination: Angle.FromDegrees( 4.25 ),
            argumentOfPeriapsis: Angle.FromDegrees( 15 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 2 ),
            meanAnomalyAtEpoch: Angle.FromRadians( 0.9 ) );

        return jool;
    }

    private static CelestialBody AddEeloo(CelestialBody kerbol)
    {
        var eeloo = kerbol.AddChild(
            name: Names.Eeloo,
            mass: Mass.FromKilograms( 1.1149224e21 ),
            radius: Distance.FromKilometers( 210 ),
            semiMajorAxis: Distance.FromMeters( 90118820000 ),
            eccentricity: Eccentricity.Create( 0.26 ),
            inclination: Angle.FromDegrees( 6.15 ),
            argumentOfPeriapsis: Angle.FromDegrees( 260 ),
            longitudeOfAscendingNode: Angle.FromDegrees( 50 ),
            meanAnomalyAtEpoch: Angle.Half );

        return eeloo;
    }
}
