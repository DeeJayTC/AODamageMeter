﻿using System;
using System.Text.RegularExpressions;

namespace AODamageMeter.FightEvents.Heal
{
    public class YouGaveNano : HealEvent
    {
        public const string EventName = "You gave nano";

        public static readonly Regex
            Normal = CreateRegex($"You increased nano on {TARGET} for {AMOUNT} points.");

        public YouGaveNano(Fight fight, DateTime timestamp, string description)
            : base(fight, timestamp, description)
        { }

        public override string Name => EventName;

        public static YouGaveNano Create(Fight fight, DateTime timestamp, string description)
        {
            var healEvent = new YouGaveNano(fight, timestamp, description);
            healEvent.SetSourceToOwner();
            healEvent.HealType = HealType.Nano;

            if (healEvent.TryMatch(Normal, out Match match))
            {
                healEvent.SetTarget(match, 1);
                healEvent.SetAmount(match, 2);
            }
            else healEvent.IsUnmatched = true;

            return healEvent;
        }
    }
}
