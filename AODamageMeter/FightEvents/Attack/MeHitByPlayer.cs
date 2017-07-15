﻿using System;
using System.Text.RegularExpressions;

namespace AODamageMeter.FightEvents.Attack
{
    public class MeHitByPlayer : AttackEvent
    {
        public const string EventName = "Me hit by player";
        public override string Name => EventName;

        public static readonly Regex
            Basic =  CreateRegex($"(?:Player )?{SOURCE} hit you for {AMOUNT} points of {DAMAGETYPE} damage.", rightToLeft: true),
            Crit =   CreateRegex($"(?:Player )?{SOURCE} hit you for {AMOUNT} points of {DAMAGETYPE} damage. Critical hit!", rightToLeft: true),
            Glance = CreateRegex($"(?:Player )?{SOURCE} hit you for {AMOUNT} points of {DAMAGETYPE} damage. Glancing hit.", rightToLeft: true);

        public MeHitByPlayer(Fight fight, DateTime timestamp, string description)
            : base(fight, timestamp, description)
        {
            SetTargetToOwner();
            AttackResult = AttackResult.WeaponHit;

            bool crit = false, glance = false;
            if (TryMatch(Basic, out Match match, out bool basic)
                || TryMatch(Crit, out match, out crit)
                || TryMatch(Glance, out match, out glance))
            {
                SetSource(match, 1);
                Source.Character.IsPlayer = true;
                SetAmount(match, 2);
                SetDamageType(match, 3);
                AttackModifier = crit ? AODamageMeter.AttackModifier.Crit
                    : glance ? AODamageMeter.AttackModifier.Glance
                    : (AttackModifier?)null;
            }
            else IsUnmatched = true;
        }
    }
}
