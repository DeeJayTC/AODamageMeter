﻿using System;
using System.Text.RegularExpressions;

namespace AODamageMeter.FightEvents.Nano
{
    public class MeCastNano : NanoEvent
    {
        public const string EventName = "Me Cast Nano";

        public static readonly Regex
            Executing = CreateRegex("Executing Nano Program: (.+)."),
            Success =   CreateRegex("Nano program executed successfully."),
            Resisted =  CreateRegex("Target resisted."),
            Countered = CreateRegex("Your target countered the nano program."),
            Aborted =   CreateRegex("Nano program aborted.");

        protected MeCastNano(Fight fight, DateTime timestamp, string description)
            : base(fight, timestamp, description)
        { }

        public override string Name => EventName;

        public static MeCastNano Create(Fight fight, DateTime timestamp, string description)
        {
            var nanoEvent = new MeCastNano(fight, timestamp, description);
            nanoEvent.SetSourceToOwner();

            bool resisted = false, countered = false, aborted = false;
            if (nanoEvent.TryMatch(Executing, out Match match))
            {
                nanoEvent.NanoProgram = match.Groups[1].Value;
                nanoEvent.IsStartOfCast = true;
            }
            else if (nanoEvent.TryMatch(Success, out match, out bool success)
                || nanoEvent.TryMatch(Resisted, out match, out resisted)
                || nanoEvent.TryMatch(Resisted, out match, out countered)
                || nanoEvent.TryMatch(Resisted, out match, out aborted))
            {
                MeCastNano startEvent = null;
                for (int i = fight.NanoEvents.Count - 1; i >= 0; --i)
                {
                    if (fight.NanoEvents[i].IsStartOfCast)
                    {
                        startEvent = fight.NanoEvents[i] as MeCastNano;
                        break;
                    }
                }

                // It's not obvious that these two checks are necessary, but I think they might be.
                if (startEvent != null && startEvent.EndEvent == null)
                {
                    startEvent.CastResult = success ? AODamageMeter.CastResult.Success
                        : resisted ? AODamageMeter.CastResult.Resisted
                        : countered ? AODamageMeter.CastResult.Countered
                        : AODamageMeter.CastResult.Aborted;
                    startEvent.EndEvent = nanoEvent;
                    nanoEvent.NanoProgram = startEvent.NanoProgram;
                    nanoEvent.CastResult = startEvent.CastResult;
                }
            }
            else nanoEvent.Unmatched = true;

            return nanoEvent;
        }
    }
}
