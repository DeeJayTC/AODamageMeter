﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AODamageMeter.FightEvents.Heal
{
    public class MeGotNano : HealEvent
    {
        public const string EventName = "Me got nano";

        public static readonly Regex
            Normal = CreateRegex($"You got nano from {SOURCE} for {AMOUNT} points.");

        protected MeGotNano(Fight fight, DateTime timestamp, string description)
            : base(fight, timestamp, description)
        { }

        public override string Name => EventName;

        public static async Task<MeGotNano> Create(Fight fight, DateTime timestamp, string description)
        {
            var healEvent = new MeGotNano(fight, timestamp, description);
            healEvent.SetTargetToOwner();
            healEvent.HealType = HealType.Nano;

            if (healEvent.TryMatch(Normal, out Match match))
            {
                // TODO: is the source always a player character?
                await healEvent.SetSource(match, 1);
                healEvent.SetAmount(match, 2);
            }
            else healEvent.IsUnmatched = true;

            return healEvent;
        }
    }
}
