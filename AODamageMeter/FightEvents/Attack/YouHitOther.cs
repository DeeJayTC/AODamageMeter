﻿using System;
using System.Text.RegularExpressions;

namespace AODamageMeter.FightEvents.Attack
{
    public class YouHitOther : AttackEvent
    {
        public const string EventName = "You hit other";
        public override string Name => EventName;

        public static readonly Regex
            Normal =  CreateRegex($"You hit {TARGET} for {AMOUNT} points of {DAMAGETYPE} damage."),
            Crit =    CreateRegex($"You hit {TARGET} for {AMOUNT} points of {DAMAGETYPE} damage. Critical hit!", rightToLeft: true),
            Glance =  CreateRegex($"You hit {TARGET} for {AMOUNT} points of {DAMAGETYPE} damage. Glancing hit.", rightToLeft: true),
            Reflect = CreateRegex($"Your reflect shield hit {TARGET} for {AMOUNT} points of damage."),
            Shield =  CreateRegex($"Your damage shield hit {TARGET} for {AMOUNT} points of damage.");

        public YouHitOther(Fight fight, DateTime timestamp, string description)
            : base(fight, timestamp, description)
        {
            SetSourceToOwner();

            bool crit = false, glance = false, reflect = false, shield = false;
            if (TryMatch(Normal, out Match match, out bool normal)
                || TryMatch(Crit, out match, out crit)
                || TryMatch(Glance, out match, out glance))
            {
                SetTarget(match, 1);
                AttackResult = AttackResult.WeaponHit;
                SetAmount(match, 2);
                SetDamageType(match, 3);
                AttackModifier = crit ? AODamageMeter.AttackModifier.Crit
                    : glance ? AODamageMeter.AttackModifier.Glance
                    : (AttackModifier?)null;
            }
            else if (TryMatch(Reflect, out match, out reflect)
                || TryMatch(Shield, out match, out shield))
            {
                SetTarget(match, 1);
                AttackResult = AttackResult.IndirectHit;
                SetAmount(match, 2);
                DamageType = reflect ? AODamageMeter.DamageType.Reflect : AODamageMeter.DamageType.Shield;
            }
            else IsUnmatched = true;
        }
    }
}
