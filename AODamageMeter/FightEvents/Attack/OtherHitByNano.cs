﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AODamageMeter.FightEvents.Attack
{
    public class OtherHitByNano : AttackEvent
    {
        public const string EventName = "Other hit by nano";

        public static readonly Regex
            Sourced =   CreateRegex($"{TARGET} was attacked with nanobots from {SOURCE} for {AMOUNT} points of {DAMAGETYPE} damage.", rightToLeft: true),
            Unsourced = CreateRegex($"{TARGET} was attacked with nanobots for {AMOUNT} points of {DAMAGETYPE} damage.", rightToLeft: true);

        protected OtherHitByNano(Fight fight, DateTime timestamp, string description)
            : base(fight, timestamp, description)
        { }

        public override string Name => EventName;

        public static OtherHitByNano Create(Fight fight, DateTime timestamp, string description)
        {
            var attackEvent = new OtherHitByNano(fight, timestamp, description);
            attackEvent.AttackResult = AttackResult.Hit;

            if (attackEvent.TryMatch(Sourced, out Match match))
            {
                attackEvent.SetSourceAndTarget(match, 2, 1);
                attackEvent.SetAmount(match, 3);
                attackEvent.SetDamageType(match, 4);
            }
            else if (attackEvent.TryMatch(Unsourced, out match))
            {
                attackEvent.SetTarget(match, 1);
                attackEvent.SetAmount(match, 2);
                attackEvent.SetDamageType(match, 3);
            }
            else attackEvent.IsUnmatched = true;

            return attackEvent;
        }
    }
}
