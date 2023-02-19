using System;
using OrbitalMechanics;
using OrbitalMechanics.Bodies;
using OrbitalMechanics.Orbits;
using OrbitalMechanics.Orbits.Maneuvers;
using OrbitalMechanics.SI;

namespace KSP;

public static class KerbolMissions
{
    public static readonly (Mass DryMass, Mass PropellantMass) ManweFuelTank = (Mass.FromTons( 8.083667 ), Mass.FromTons( 97.004 ));
    public static readonly Mass StrutMass = Mass.FromTons( 0.05 );
    public static readonly Mass DockingPortMass = Mass.FromTons( 1.2 );
    public static readonly Mass ParachuteMass = Mass.FromTons( 0.225 );

    public static readonly Mass EonweFairingMass = Mass.FromTons( 8.779 );
    public static readonly Mass IlmareFairingMass = Mass.FromTons( 5.628 );
    public static readonly Mass OromeFairingMass = Mass.FromTons( 6.228 );

    public static readonly Mass LargeOromeFairingMass = Mass.FromTons( 6.928 );
    public static readonly Mass OromeAtmEntryFairingMass = Mass.FromTons( 10.725 );

    public static readonly Mass NessaFairingMass = Mass.FromTons( 4.878 );

    public static readonly (Mass DryMass, Mass PropellantMass) ManweAuxHalfFuelPart =
        (DockingPortMass + ManweFuelTank.DryMass * 0.5, ManweFuelTank.PropellantMass * 0.5);

    public static readonly (Mass DryMass, Mass PropellantMass) ManweAuxFuelPart =
        (DockingPortMass + ManweFuelTank.DryMass, ManweFuelTank.PropellantMass);

    public static readonly Vessel Eonwe = new Vessel(
        engine: new Engine( specificImpulse: TimeSpan.FromSeconds( 6380 ), thrust: Force.FromKilonewtons( 11.879 ) ),
        dryMass: Mass.FromTons( 9.773 ),
        propellantMass: Mass.FromTons( 1.8 ) );

    public static readonly Vessel Ilmare = new Vessel(
        engine: new Engine( specificImpulse: TimeSpan.FromSeconds( 6380 ), thrust: Force.FromKilonewtons( 2.1 ) ),
        dryMass: Mass.FromTons( 3.186 ),
        propellantMass: Mass.FromTons( 0.36 ) );

    public static readonly Vessel Nahar = new Vessel(
        engine: new Engine( specificImpulse: TimeSpan.FromSeconds( 430 ), thrust: Force.FromKilonewtons( 70.565 ) ),
        dryMass: Mass.FromTons( 3.326 ),
        propellantMass: Mass.FromTons( 3.008 ) );

    public static readonly Vessel Orome = new Vessel(
        engine: new Engine( specificImpulse: TimeSpan.FromSeconds( 2600 ), thrust: Force.FromKilonewtons( 8.362 ) ),
        dryMass: Mass.FromTons( 1.387 ),
        propellantMass: Mass.FromTons( 0.308 ) );

    public static readonly Vessel Osse = new Vessel(
        engine: new Engine( specificImpulse: TimeSpan.FromSeconds( 370 ), thrust: Force.FromKilonewtons( 160 ) ),
        dryMass: Mass.FromTons( 1.758 ),
        propellantMass: Mass.FromTons( 3.335 ) );

    public static readonly Mass NessaMass = Mass.FromTons( 1.463 );

    public static readonly Vessel EmptyManwe = new Vessel(
        engine: new Engine( specificImpulse: TimeSpan.FromSeconds( 458 ), thrust: Force.FromKilonewtons( 2730 ) ),
        dryMass: Mass.FromTons( 70.493 ) + ManweFuelTank.DryMass * 4.5 );

    public static readonly Vessel Manwe = EmptyManwe.AddPropellant( ManweFuelTank.PropellantMass * 4.5 );

    public static readonly Orbit StartingOrbit = EllipticOrbit.CircularFromAltitude(
        Kerbol.System.Bodies[Kerbol.Names.Kerbin],
        Distance.FromKilometers( 90 ) );

    public static Vessel AugmentVessel(Vessel vessel, (Mass DryMass, Mass PropellantMass) auxPart, int count = 2)
    {
        return vessel.AddDryMass( auxPart.DryMass * count ).AddPropellant( auxPart.PropellantMass * count );
    }

    // missions:
    // x vall (eonwe, ilmare, orome)
    // x laythe (eonwe, ilmare)
    // x jool (eonwe-m) + laythe (nessa)
    // x sarnus (eonwe-m) + eeloo (eonwe)
    // x hale (eonwe, orome)
    // x eeloo (ilmare, orome)
    // x tekto (eonwe, ilmare, orome, nessa)
    // x urlum (eonwe-m)
    // x polta (eonwe, ilmare, orome)
    // x neidon (eonwe-m)
    // x thatmo (eonwe, ilmare, orome)

    public static Mission ToVall()
    {
        var jool = Kerbol.System.Bodies[Kerbol.Names.Jool];
        var vall = Kerbol.System.Bodies[Kerbol.Names.Vall];

        var mission = new Mission(
            "Vall (2x Eonwe, 3x Ilmare, 1x Orome)",
            AugmentVessel( Manwe, ManweAuxHalfFuelPart )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( IlmareFairingMass + Ilmare.Mass * 3 )
                .AddDryMass( OromeFairingMass + Nahar.Mass + Orome.Mass ),
            StartingOrbit );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( mission.Orbit, Distance.FromKilometers( 4000 ) );
        mission.AddOrbitalManeuver( maneuver, "Kerbin apoapsis pre-escape lift" );

        mission.AddPayloadDrop( ManweAuxHalfFuelPart.DryMass * 2, "Detach empty aux fuel tanks at collision course with Kerbin" );

        var initialJoolOrbit = EllipticOrbit.FromRadii(
            jool,
            jool.GetRadius( jool.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            vall.Orbit!.Apoapsis.Radius );

        AddCaptureFromKerbin( mission, initialJoolOrbit );

        var parkingOrbit = EllipticOrbit.FromRadii(
            vall,
            vall.GetRadius( Distance.FromKilometers( 20 ) ),
            vall.SphereOfInfluenceRadius * 0.9 );

        maneuver = new CaptureManeuver( initialJoolOrbit.Periapsis.Radius, parkingOrbit );
        mission.AddOrbitalManeuver( maneuver, "Vall capture from Jool" );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe on Vall" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, vall.GetRadius( Distance.FromKilometers( 360 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Vall apoapsis drop to Ilmare altitude" );

        mission.AddPayloadDrop( Ilmare.Mass * 3, "Deploy 3x Ilmare on Vall" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, maneuver.Next.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Vall apoapsis drop to circularize" );

        mission.AddPayloadDrop( Nahar.Mass + Orome.Mass, "Deploy 1x Orome on Vall" );
        AddEmptyFaringDrop( mission, EonweFairingMass + IlmareFairingMass + OromeFairingMass, Distance.FromKilometers( -5 ) );

        maneuver = new EscapeManeuver( maneuver.Next, jool.GetRadius( jool.AtmosphereHeight + Distance.FromKilometers( 20 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Vall escape to Jool" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToLaythe()
    {
        var jool = Kerbol.System.Bodies[Kerbol.Names.Jool];
        var laythe = Kerbol.System.Bodies[Kerbol.Names.Laythe];

        var mission = new Mission(
            "Laythe (2x Eonwe, 3x Ilmare)",
            AugmentVessel( AugmentVessel( Manwe, ManweAuxHalfFuelPart, count: 3 ), ManweAuxFuelPart, count: 1 )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( IlmareFairingMass + Ilmare.Mass * 3 ),
            StartingOrbit );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( mission.Orbit, Distance.FromKilometers( 3000 ) );
        mission.AddOrbitalManeuver( maneuver, "Kerbin apoapsis pre-escape lift" );

        mission.AddPayloadDrop( ManweAuxHalfFuelPart.DryMass * 2, "Detach empty aux fuel tanks at collision course with Kerbin" );

        var initialJoolOrbit = EllipticOrbit.FromRadii(
            jool,
            jool.GetRadius( jool.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            laythe.Orbit!.Apoapsis.Radius );

        AddCaptureFromKerbin( mission, initialJoolOrbit );

        var parkingOrbit = EllipticOrbit.FromRadii(
            laythe,
            laythe.GetRadius( laythe.AtmosphereHeight + Distance.FromKilometers( 10 ) ),
            laythe.SphereOfInfluenceRadius * 0.9 );

        maneuver = new CaptureManeuver( initialJoolOrbit.Periapsis.Radius, parkingOrbit );
        mission.AddOrbitalManeuver( maneuver, "Laythe capture from Jool" );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe on Laythe" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, laythe.GetRadius( Distance.FromKilometers( 360 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Laythe apoapsis drop to Ilmare altitude" );

        mission.AddPayloadDrop( Ilmare.Mass * 3, "Deploy 3x Ilmare on Laythe" );
        AddEmptyFaringDrop(
            mission,
            EonweFairingMass + IlmareFairingMass + ManweAuxHalfFuelPart.DryMass + ManweAuxFuelPart.DryMass,
            Distance.FromKilometers( 20 ) );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, maneuver.Next.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Laythe apoapsis drop to circularize" );

        maneuver = new EscapeManeuver( maneuver.Next, jool.GetRadius( jool.AtmosphereHeight + Distance.FromKilometers( 20 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Laythe escape to Jool" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToLaytheSurface()
    {
        var jool = Kerbol.System.Bodies[Kerbol.Names.Jool];
        var laythe = Kerbol.System.Bodies[Kerbol.Names.Laythe];

        var mission = new Mission(
            "Jool (2x Eonwe-M) & Laythe (1x Nessa)",
            AugmentVessel( AugmentVessel( Manwe, ManweAuxHalfFuelPart, count: 3 ), ManweAuxFuelPart, count: 1 )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( NessaFairingMass + Osse.Mass + NessaMass + ParachuteMass * 2 ),
            StartingOrbit );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( mission.Orbit, Distance.FromKilometers( 3000 ) );
        mission.AddOrbitalManeuver( maneuver, "Kerbin apoapsis pre-escape lift" );

        mission.AddPayloadDrop( ManweAuxHalfFuelPart.DryMass * 2, "Detach empty aux fuel tanks at collision course with Kerbin" );

        var initialJoolOrbit = EllipticOrbit.FromRadii(
            jool,
            jool.GetRadius( jool.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            jool.SphereOfInfluenceRadius );

        AddCaptureFromKerbin( mission, initialJoolOrbit );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe-M on Jool" );

        maneuver = new ChangeApoapsisManeuver( initialJoolOrbit, laythe.Orbit!.Apoapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Jool apoapsis drop to match Laythe" );

        var parkingOrbit = EllipticOrbit.FromRadii(
            laythe,
            laythe.GetRadius( laythe.AtmosphereHeight + Distance.FromKilometers( 10 ) ),
            laythe.GetRadius( laythe.AtmosphereHeight + Distance.FromKilometers( 30 ) ) );

        maneuver = new CaptureManeuver( maneuver.Next.Periapsis.Radius, parkingOrbit );
        mission.AddOrbitalManeuver( maneuver, "Laythe capture from Jool" );

        maneuver = new ChangePeriapsisManeuver( parkingOrbit, laythe.GetRadius( Distance.FromKilometers( 20 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Laythe periapsis drop to atmosphere" );

        mission.AddPayloadDrop( Osse.Mass + NessaMass + ParachuteMass * 2, "Deploy 1x Osse+Nessa on Laythe" );
        mission.AddPayloadDrop(
            EonweFairingMass + NessaFairingMass + ManweAuxHalfFuelPart.DryMass + ManweAuxFuelPart.DryMass,
            "Detach fairings at collision course with Laythe" );

        maneuver = new ChangePeriapsisManeuver( maneuver.Next, parkingOrbit.Apoapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Laythe periapsis lift to circularize" );

        maneuver = new EscapeManeuver( maneuver.Next, jool.GetRadius( jool.AtmosphereHeight + Distance.FromKilometers( 20 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Laythe escape to Jool" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToSarnus()
    {
        var sarnus = Kerbol.System.Bodies[Kerbol.Names.Sarnus];
        var eeloo = Kerbol.System.Bodies[Kerbol.Names.Eeloo];

        var mission = new Mission(
            "Sarnus (2x Eonwe-M) & Eeloo (2x Eonwe)",
            AugmentVessel( AugmentVessel( Manwe, ManweAuxHalfFuelPart, count: 3 ), ManweAuxFuelPart, count: 1 )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 ),
            StartingOrbit );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( mission.Orbit, Distance.FromKilometers( 2600 ) );
        mission.AddOrbitalManeuver( maneuver, "Kerbin apoapsis pre-escape lift" );

        mission.AddPayloadDrop( ManweAuxHalfFuelPart.DryMass * 2, "Detach empty aux fuel tanks at collision course with Kerbin" );

        var initialSarnusOrbit = EllipticOrbit.FromRadii(
            sarnus,
            sarnus.GetRadius( sarnus.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            sarnus.SphereOfInfluenceRadius );

        AddCaptureFromKerbin( mission, initialSarnusOrbit );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe-M on Sarnus" );

        maneuver = new ChangeApoapsisManeuver( initialSarnusOrbit, eeloo.Orbit!.Apoapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Sarnus apoapsis drop to match Eeloo" );

        maneuver = new ChangeInclinationManeuver( maneuver.Next, eeloo.Orbit!.Inclination, OrbitalManeuverPoint.Periapsis );
        mission.AddOrbitalManeuver( maneuver, "Match Eeloo inclination" );

        var parkingOrbit = EllipticOrbit.FromRadii(
            eeloo,
            eeloo.GetRadius( Distance.FromKilometers( 100 ) ),
            eeloo.SphereOfInfluenceRadius );

        maneuver = new CaptureManeuver( maneuver.Next.Periapsis.Radius, parkingOrbit, OrbitalManeuverPoint.Apoapsis );
        mission.AddOrbitalManeuver( maneuver, "Eeloo capture from Sarnus" );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe on Eeloo" );
        AddEmptyFaringDrop(
            mission,
            EonweFairingMass * 2 + ManweAuxHalfFuelPart.DryMass + ManweAuxFuelPart.DryMass,
            Distance.FromKilometers( -5 ) );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, parkingOrbit.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Eeloo apoapsis drop to circularize" );

        maneuver = new EscapeManeuver(
            maneuver.Next,
            sarnus.GetRadius( sarnus.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            OrbitalManeuverPoint.Apoapsis );

        mission.AddOrbitalManeuver( maneuver, "Eeloo escape to Sarnus" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToEeloo()
    {
        var sarnus = Kerbol.System.Bodies[Kerbol.Names.Sarnus];
        var eeloo = Kerbol.System.Bodies[Kerbol.Names.Eeloo];

        var mission = new Mission(
            "Eeloo (3x Ilmare, 1x Orome)",
            AugmentVessel( AugmentVessel( Manwe, ManweAuxHalfFuelPart ), ManweAuxFuelPart, count: 1 )
                .AddDryMass( IlmareFairingMass + Ilmare.Mass * 3 )
                .AddDryMass( OromeFairingMass + Nahar.Mass + Orome.Mass ),
            StartingOrbit );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( mission.Orbit, Distance.FromKilometers( 4200 ) );
        mission.AddOrbitalManeuver( maneuver, "Kerbin apoapsis pre-escape lift" );

        mission.AddPayloadDrop( ManweAuxHalfFuelPart.DryMass * 2, "Detach empty aux fuel tanks at collision course with Kerbin" );

        var initialSarnusOrbit = EllipticOrbit.FromRadii(
            sarnus,
            sarnus.GetRadius( sarnus.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            eeloo.Orbit!.Apoapsis.Radius );

        AddCaptureFromKerbin( mission, initialSarnusOrbit );

        maneuver = new ChangeInclinationManeuver( initialSarnusOrbit, eeloo.Orbit!.Inclination, OrbitalManeuverPoint.Periapsis );
        mission.AddOrbitalManeuver( maneuver, "Match Eeloo inclination" );

        var parkingOrbit = EllipticOrbit.FromAltitudes( eeloo, Distance.FromKilometers( 100 ), Distance.FromKilometers( 360 ) );

        maneuver = new CaptureManeuver( maneuver.Next.Periapsis.Radius, parkingOrbit, OrbitalManeuverPoint.Apoapsis );
        mission.AddOrbitalManeuver( maneuver, "Eeloo capture from Sarnus" );

        mission.AddPayloadDrop( Ilmare.Mass * 3, "Deploy 3x Ilmare on Eeloo" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, parkingOrbit.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Eeloo apoapsis drop to circularize" );

        mission.AddPayloadDrop( Nahar.Mass + Orome.Mass, "Deploy 1x Orome on Eeloo" );
        AddEmptyFaringDrop( mission, IlmareFairingMass + OromeFairingMass + ManweAuxFuelPart.DryMass, Distance.FromKilometers( -5 ) );

        maneuver = new EscapeManeuver(
            maneuver.Next,
            sarnus.GetRadius( sarnus.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            OrbitalManeuverPoint.Apoapsis );

        mission.AddOrbitalManeuver( maneuver, "Eeloo escape to Sarnus" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToHale()
    {
        var sarnus = Kerbol.System.Bodies[Kerbol.Names.Sarnus];
        var hale = Kerbol.System.Bodies[Kerbol.Names.Hale];

        var mission = new Mission(
            "Hale (2x Eonwe, 1x Orome)",
            AugmentVessel( AugmentVessel( Manwe, ManweAuxHalfFuelPart ), ManweAuxFuelPart )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( OromeFairingMass + Nahar.Mass + Orome.Mass ),
            StartingOrbit );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( mission.Orbit, Distance.FromKilometers( 4200 ) );
        mission.AddOrbitalManeuver( maneuver, "Kerbin apoapsis pre-escape lift" );

        mission.AddPayloadDrop( ManweAuxHalfFuelPart.DryMass * 2, "Detach empty aux fuel tanks at collision course with Kerbin" );

        var initialSarnusOrbit = EllipticOrbit.FromRadii(
            sarnus,
            sarnus.GetRadius( sarnus.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            hale.Orbit!.Apoapsis.Radius );

        AddCaptureFromKerbin( mission, initialSarnusOrbit );

        maneuver = new ChangeInclinationManeuver( initialSarnusOrbit, hale.Orbit!.Inclination, OrbitalManeuverPoint.Periapsis );
        mission.AddOrbitalManeuver( maneuver, "Match Hale inclination" );

        var parkingOrbit = EllipticOrbit.CircularFromRadius( hale, Distance.FromKilometers( 5 ) );

        maneuver = new CaptureManeuver( maneuver.Next.Periapsis.Radius, parkingOrbit, OrbitalManeuverPoint.Apoapsis );
        mission.AddOrbitalManeuver( maneuver, "Hale capture from Sarnus" );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe on Hale" );
        mission.AddPayloadDrop( Nahar.Mass + Orome.Mass, "Deploy 1x Orome on Hale" );
        mission.AddPayloadDrop(
            EonweFairingMass + OromeFairingMass + ManweAuxFuelPart.DryMass * 2,
            "Detach fairing at Hale collision course" );

        maneuver = new EscapeManeuver(
            parkingOrbit,
            sarnus.GetRadius( sarnus.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            OrbitalManeuverPoint.Apoapsis );

        mission.AddOrbitalManeuver( maneuver, "Hale escape to Sarnus" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToTekto()
    {
        var sarnus = Kerbol.System.Bodies[Kerbol.Names.Sarnus];
        var tekto = Kerbol.System.Bodies[Kerbol.Names.Tekto];

        var mission = new Mission(
            "Tekto (2x Eonwe, 3x Ilmare, 1x Orome, 1x Nessa) !EXPENDABLE!",
            Manwe
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( IlmareFairingMass + Ilmare.Mass * 3 )
                .AddDryMass( OromeFairingMass + Nahar.Mass + Orome.Mass + ParachuteMass * 2 )
                .AddDryMass( NessaFairingMass + Osse.Mass + NessaMass + ParachuteMass * 2 ),
            StartingOrbit );

        var initialSarnusOrbit = EllipticOrbit.FromRadii(
            sarnus,
            sarnus.GetRadius( sarnus.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            tekto.Orbit!.Apoapsis.Radius );

        AddCaptureFromKerbin( mission, initialSarnusOrbit );

        IOrbitalManeuver maneuver = new ChangeInclinationManeuver(
            initialSarnusOrbit,
            tekto.Orbit!.Inclination,
            OrbitalManeuverPoint.Periapsis );

        mission.AddOrbitalManeuver( maneuver, "Match Tekto inclination" );

        var parkingOrbit = EllipticOrbit.FromRadii(
            tekto,
            tekto.GetRadius( tekto.AtmosphereHeight + Distance.FromKilometers( 10 ) ),
            tekto.SphereOfInfluenceRadius );

        maneuver = new CaptureManeuver( maneuver.Next.Periapsis.Radius, parkingOrbit, OrbitalManeuverPoint.Apoapsis );
        mission.AddOrbitalManeuver( maneuver, "Tekto capture from Sarnus" );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe on Tekto" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, tekto.GetRadius( Distance.FromKilometers( 360 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Tekto apoapsis drop to Ilmare deploy" );

        mission.AddPayloadDrop( Ilmare.Mass * 3, "Deploy 3x Ilmare on Tekto" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, tekto.GetRadius( tekto.AtmosphereHeight + Distance.FromKilometers( 30 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Tekto apoapsis drop to Orome deploy" );

        mission.AddPayloadDrop( Nahar.Mass + Orome.Mass + ParachuteMass * 2, "Deploy 1x Orome on Tekto" );

        maneuver = new ChangePeriapsisManeuver( maneuver.Next, tekto.GetRadius( Distance.FromKilometers( 50 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Tekto periapsis drop to atmosphere" );

        mission.AddPayloadDrop( Osse.Mass + NessaMass + ParachuteMass * 2, "Deploy 1x Nessa on Tekto" );
        mission.AddPayloadDrop(
            EonweFairingMass + IlmareFairingMass + OromeFairingMass + NessaFairingMass,
            "Detach fairing at Tekto collision course" );

        maneuver = new ChangePeriapsisManeuver( maneuver.Next, maneuver.Next.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Tekto periapsis lift away from atmosphere" );

        return mission;
    }

    public static Mission ToUrlum()
    {
        var urlum = Kerbol.System.Bodies[Kerbol.Names.Urlum];
        var polta = Kerbol.System.Bodies[Kerbol.Names.Polta];

        var mission = new Mission(
            "Urlum (2x Eonwe-M) & Polta (2x Eonwe, 3x Ilmare, 1x Orome)",
            AugmentVessel( Manwe, ManweAuxHalfFuelPart )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( IlmareFairingMass + Ilmare.Mass * 3 )
                .AddDryMass( OromeFairingMass + Nahar.Mass + Orome.Mass ),
            StartingOrbit );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( mission.Orbit, Distance.FromKilometers( 4000 ) );
        mission.AddOrbitalManeuver( maneuver, "Kerbin apoapsis pre-escape lift" );

        mission.AddPayloadDrop( ManweAuxHalfFuelPart.DryMass * 2, "Detach empty aux fuel tanks at collision course with Kerbin" );

        var initialUrlumOrbit = EllipticOrbit.FromRadii(
            urlum,
            urlum.GetRadius( urlum.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            urlum.SphereOfInfluenceRadius );

        AddCaptureFromKerbin( mission, initialUrlumOrbit );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe-M on Urlum" );

        maneuver = new ChangeApoapsisManeuver( initialUrlumOrbit, polta.Orbit!.Apoapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Urlum apoapsis drop to match Polta" );

        maneuver = new ChangeInclinationManeuver( maneuver.Next, polta.Orbit!.Inclination, OrbitalManeuverPoint.Periapsis );
        mission.AddOrbitalManeuver( maneuver, "Match Polta inclination" );

        var parkingOrbit = EllipticOrbit.FromRadii(
            polta,
            polta.GetRadius( Distance.FromKilometers( 20 ) ),
            polta.SphereOfInfluenceRadius );

        maneuver = new CaptureManeuver( maneuver.Next.Periapsis.Radius, parkingOrbit, OrbitalManeuverPoint.Apoapsis );
        mission.AddOrbitalManeuver( maneuver, "Polta capture from Urlum" );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe on Polta" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, polta.GetRadius( Distance.FromKilometers( 360 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Polta apoapsis drop to Ilmare deploy" );

        mission.AddPayloadDrop( Ilmare.Mass * 3, "Deploy 3x Ilmare on Polta" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, polta.GetRadius( Distance.FromKilometers( 30 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Polta apoapsis drop to Orome deploy" );

        mission.AddPayloadDrop( Nahar.Mass + Orome.Mass, "Deploy 1x Orome on Polta" );

        AddEmptyFaringDrop(
            mission,
            EonweFairingMass + EonweFairingMass + IlmareFairingMass + OromeFairingMass,
            Distance.FromKilometers( -5 ) );

        maneuver = new EscapeManeuver(
            parkingOrbit,
            urlum.GetRadius( urlum.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            OrbitalManeuverPoint.Apoapsis );

        mission.AddOrbitalManeuver( maneuver, "Polta escape to Urlum" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToNeidon()
    {
        var neidon = Kerbol.System.Bodies[Kerbol.Names.Neidon];

        var mission = new Mission(
            "Neidon (2x Eonwe-M)",
            AugmentVessel( Manwe, ManweAuxFuelPart, count: 1 )
                .AddDryMass( DockingPortMass )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 ),
            StartingOrbit );

        var initialNeidonOrbit = EllipticOrbit.FromRadii(
            neidon,
            neidon.GetRadius( neidon.AtmosphereHeight + Distance.FromKilometers( 300 ) ),
            neidon.SphereOfInfluenceRadius );

        AddCaptureFromKerbin( mission, initialNeidonOrbit );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe-M on Neidon" );

        AddEmptyFaringDrop(
            mission,
            EonweFairingMass + ManweAuxFuelPart.DryMass + DockingPortMass,
            neidon.AtmosphereHeight - Distance.FromKilometers( 20 ) );

        IOrbitalManeuver maneuver = new ChangeApoapsisManeuver( initialNeidonOrbit, initialNeidonOrbit.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Neidon apoapsis drop to circularize" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    public static Mission ToThatmo()
    {
        var neidon = Kerbol.System.Bodies[Kerbol.Names.Neidon];
        var thatmo = Kerbol.System.Bodies[Kerbol.Names.Thatmo];

        var mission = new Mission(
            "Thatmo (2x Eonwe, 3x Ilmare, 1x Orome)",
            AugmentVessel( Manwe, ManweAuxFuelPart, count: 1 )
                .AddDryMass( DockingPortMass )
                .AddDryMass( EonweFairingMass + Eonwe.Mass * 2 )
                .AddDryMass( IlmareFairingMass + Ilmare.Mass * 3 )
                .AddDryMass( OromeFairingMass + Nahar.Mass + Orome.Mass ),
            StartingOrbit );

        var initialNeidonOrbit = EllipticOrbit.FromRadii(
            neidon,
            neidon.GetRadius( neidon.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            thatmo.Orbit!.Apoapsis.Radius,
            inclination: Angle.Half );

        AddCaptureFromKerbin( mission, initialNeidonOrbit );

        IOrbitalManeuver maneuver = new ChangeInclinationManeuver(
            initialNeidonOrbit,
            thatmo.Orbit!.Inclination,
            OrbitalManeuverPoint.Periapsis );

        mission.AddOrbitalManeuver( maneuver, "Match Thatmo inclination" );

        var parkingOrbit = EllipticOrbit.FromRadii(
            thatmo,
            thatmo.GetRadius( thatmo.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            thatmo.SphereOfInfluenceRadius );

        maneuver = new CaptureManeuver( maneuver.Next.Periapsis.Radius, parkingOrbit, OrbitalManeuverPoint.Apoapsis );
        mission.AddOrbitalManeuver( maneuver, "Thatmo capture from Neidon" );

        mission.AddPayloadDrop( Eonwe.Mass * 2, "Deploy 2x Eonwe on Thatmo" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, thatmo.GetRadius( Distance.FromKilometers( 360 ) ) );
        mission.AddOrbitalManeuver( maneuver, "Thatmo apoapsis drop to Ilmare deploy" );

        mission.AddPayloadDrop( Ilmare.Mass * 3, "Deploy 3x Ilmare on Thatmo" );

        maneuver = new ChangeApoapsisManeuver( maneuver.Next, maneuver.Next.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, "Thatmo apoapsis drop to Orome deploy" );

        mission.AddPayloadDrop( Nahar.Mass + Orome.Mass, "Deploy 1x Orome on Thatmo" );
        AddEmptyFaringDrop(
            mission,
            EonweFairingMass + IlmareFairingMass + OromeFairingMass + ManweAuxFuelPart.DryMass + DockingPortMass,
            Distance.FromKilometers( -5 ) );

        // this could be salvageable, needs about 1.2km/s more dV
        // also, 1km/s of dV can be saved if neidon capture happens to have a similar inclination to that of thatmo's orbit
        // inclination change is done with full payload, so dropping that maneuver's required dV close to 0 is actually more than enough
        maneuver = new EscapeManeuver(
            parkingOrbit,
            neidon.GetRadius( neidon.AtmosphereHeight + Distance.FromKilometers( 20 ) ),
            OrbitalManeuverPoint.Apoapsis );

        mission.AddOrbitalManeuver( maneuver, "Thatmo escape to Neidon" );

        AddEscapeToKerbin( mission );
        return mission;
    }

    private static void AddCaptureFromKerbin(Mission mission, Orbit targetOrbit)
    {
        var kerbin = Kerbol.System.Bodies[Kerbol.Names.Kerbin];
        var orbit = mission.Orbit;
        if ( orbit.Parent != kerbin )
            throw new InvalidOperationException( "Mission's current orbit must be on Kerbin." );

        IOrbitalManeuver maneuver = new EscapeManeuver( orbit, targetOrbit.Parent.Orbit!.Apoapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, $"{kerbin.Name} escape to {targetOrbit.Parent.Name}" );

        maneuver = new ChangeInclinationManeuver(
            maneuver.Next,
            targetOrbit.Parent.Orbit!.Inclination,
            OrbitalManeuverPoint.Periapsis );

        mission.AddOrbitalManeuver( maneuver, $"Match {targetOrbit.Parent.Name} inclination" );

        maneuver = new CaptureManeuver( kerbin.Orbit!.SemiMajorAxis, targetOrbit, OrbitalManeuverPoint.Apoapsis );
        mission.AddOrbitalManeuver( maneuver, $"{targetOrbit.Parent.Name} capture from {kerbin.Name}" );
    }

    private static void AddEscapeToKerbin(Mission mission)
    {
        var parkingOrbit = mission.Orbit;
        var kerbin = Kerbol.System.Bodies[Kerbol.Names.Kerbin];
        var minmus = Kerbol.System.Bodies[Kerbol.Names.Minmus];

        IOrbitalManeuver maneuver = new EscapeManeuver(
            parkingOrbit,
            kerbin.Orbit!.SemiMajorAxis,
            sourceBodyManeuverPoint: OrbitalManeuverPoint.Apoapsis );

        mission.AddOrbitalManeuver( maneuver, $"{parkingOrbit.Parent.Name} escape to {kerbin.Name}" );

        maneuver = new ChangeInclinationManeuver( maneuver.Next, kerbin.Orbit!.Inclination, OrbitalManeuverPoint.Periapsis );
        mission.AddOrbitalManeuver( maneuver, $"Match {kerbin.Name} inclination" );

        var finalOrbit = EllipticOrbit.FromRadii( kerbin, kerbin.GetRadius( Distance.FromKilometers( 90 ) ), minmus.Orbit!.SemiMajorAxis );

        maneuver = new CaptureManeuver( maneuver.Next.Apoapsis.Radius, finalOrbit );
        mission.AddOrbitalManeuver( maneuver, $"{kerbin.Name} capture from {parkingOrbit.Parent.Name} to {minmus.Name} altitude" );
    }

    private static void AddEmptyFaringDrop(Mission mission, Mass dryMass, Distance altitude, string fairingName = "fairing")
    {
        var initialOrbit = mission.Orbit;
        var body = initialOrbit.Parent;

        IOrbitalManeuver maneuver = new ChangePeriapsisManeuver( initialOrbit, body.GetRadius( altitude ) );
        mission.AddOrbitalManeuver( maneuver, $"{body.Name} periapsis drop to collision course" );

        mission.AddPayloadDrop( dryMass, $"Detach {fairingName} at {body.Name} collision course" );

        maneuver = new ChangePeriapsisManeuver( maneuver.Next, initialOrbit.Periapsis.Radius );
        mission.AddOrbitalManeuver( maneuver, $"{body.Name} periapsis lift away from collision course" );
    }
}
