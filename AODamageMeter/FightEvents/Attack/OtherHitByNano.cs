﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AODamageMeter.FightEvents.Attack
{
    public class OtherHitByNano : AttackEvent
    {
        public const string EventKey = "04";
        public const string EventName = "Other hit by nano";

        public static readonly Regex
            Sourced =   CreateRegex(@"(.+?) was attacked with nanobots from (.+?) for (\d+) points of (.+?) damage.", rightToLeft: true),
            Unsourced = CreateRegex(@"(.+?) was attacked with nanobots for (\d+) points of (.+?) damage.", rightToLeft: true);

        protected OtherHitByNano(DamageMeter damageMeter, Fight fight, DateTime timestamp, string description)
            : base(damageMeter, fight, timestamp, description)
        { }

        public override string Key => EventKey;
        public override string Name => EventName;

        public static async Task<OtherHitByNano> Create(DamageMeter damageMeter, Fight fight, DateTime timestamp, string description)
        {
            var attackEvent = new OtherHitByNano(damageMeter, fight, timestamp, description);

            if (attackEvent.TryMatch(Sourced, out Match match))
            {
                await attackEvent.SetSourceAndTarget(match, 2, 1);
                attackEvent.AttackResult = AttackResult.DirectHit;
                attackEvent.SetAmount(match, 3);
                attackEvent.SetDamageType(match, 4);
            }
            else if (attackEvent.TryMatch(Unsourced, out match))
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
