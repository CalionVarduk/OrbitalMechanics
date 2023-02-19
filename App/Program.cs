using System;
using KSP;

var moho = Kerbol.System.Bodies[Kerbol.Names.Moho];
var eve = Kerbol.System.Bodies[Kerbol.Names.Eve];
var kerbin = Kerbol.System.Bodies[Kerbol.Names.Kerbin];
var duna = Kerbol.System.Bodies[Kerbol.Names.Duna];
var dres = Kerbol.System.Bodies[Kerbol.Names.Dres];
var jool = Kerbol.System.Bodies[Kerbol.Names.Jool];

var date = new Date( 67, 138, new TimeSpan( hours: 2, minutes: 35, seconds: 0 ) );
var timeSinceEpoch = date.ToTimeSinceEpoch().ToTimeSpan();

var mohoPoint = moho.Orbit!.GetPointByElapsedTime( timeSinceEpoch );
var evePoint = eve.Orbit!.GetPointByElapsedTime( timeSinceEpoch );
var kerbinPoint = kerbin.Orbit!.GetPointByElapsedTime( timeSinceEpoch );
var dunaPoint = duna.Orbit!.GetPointByElapsedTime( timeSinceEpoch );
var dresPoint = dres.Orbit!.GetPointByElapsedTime( timeSinceEpoch );
var joolPoint = jool.Orbit!.GetPointByElapsedTime( timeSinceEpoch );

var timeToMoho = new Time( kerbin.Orbit!.GetTimeToNextTransferWindow( moho.Orbit!, timeSinceEpoch ) );
var timeToEve = new Time( kerbin.Orbit!.GetTimeToNextTransferWindow( eve.Orbit!, timeSinceEpoch ) );
var timeToDuna = new Time( kerbin.Orbit!.GetTimeToNextTransferWindow( duna.Orbit!, timeSinceEpoch ) );
var timeToDres = new Time( kerbin.Orbit!.GetTimeToNextTransferWindow( dres.Orbit!, timeSinceEpoch ) );
var timeToJool = new Time( kerbin.Orbit!.GetTimeToNextTransferWindow( jool.Orbit!, timeSinceEpoch ) );
