﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AODamageMeter.FightEvents.Attack
{
    public class YouHitOtherWithNano : AttackEvent
    {
        public const string EventKey = "05";
        public const string EventName = "You hit other with nano";

        public static readonly Regex
            Normal = CreateRegex($"You hit {TARGET} with nanobots for {AMOUNT} points of {DAMAGETYPE} damage.");

        protected YouHitOtherWithNano(DamageMeter damageMeter, Fight fight, DateTime timestamp, string description)
            : base(damageMeter, fight, timestamp, description)
        { }

        public override string Key => EventKey;
        public override string Name => EventName;

        public static async Task<YouHitOtherWithNano> Create(DamageMeter damageMeter, Fight fight, DateTime timestamp, string description)
        {
            var attackEvent = new YouHitOtherWithNano(damageMeter, fight, timestamp, description);
            attackEvent.SetSourceToOwner();

            if (attackEvent.TryMatch(Normal, out Match match))
            {
                await attackEvent.SetTarget(match, 1);
                attackEvent.AttackResult = AttackResult.DirectHit;
                attackEvent.SetAmount(match, 2);
                attackEvent.SetDamageType(match, 3);
            }
            else throw new NotSupportedException($"{EventName}: {description}");

            return attackEvent;
        }
    }
}
