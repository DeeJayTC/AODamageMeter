﻿using AODamageMeter.Helpers;
using AODamageMeter.UI.Helpers;

namespace AODamageMeter.UI.ViewModels.Rows
{
    public sealed class DamageTakenInfoDetailRow : FightCharacterDetailRowBase
    {
        public DamageTakenInfoDetailRow(FightViewModel fightViewModel, FightCharacter target, FightCharacter source)
            : base(fightViewModel, source)
        {
            Target = target;
            Source = source;
        }

        public DamageInfo DamageTakenInfo { get; private set; }
        public FightCharacter Target { get; }
        public FightCharacter Source { get; }

        public override string Title => $"{Target.UncoloredName}'s Damage Taken from {Source.UncoloredName} (Detail)";

        public override string RightTextToolTip
        {
            get
            {
                lock (Fight)
                {
                    return
$@"{DisplayIndex}. {Target.UncoloredName} <- {Source.UncoloredName}

{DamageTakenInfo?.TotalDamage.ToString("N0") ?? EmDash} total dmg

{((DamageTakenInfo?.HasIncompleteMissStats ?? false) ? "≤ " : "")}{DamageTakenInfo?.WeaponHitChance.FormatPercent() ?? EmDash} weapon hit chance
  {DamageTakenInfo?.CritChance.FormatPercent() ?? EmDash} crit chance
  {DamageTakenInfo?.GlanceChance.FormatPercent() ?? EmDash} glance chance

{DamageTakenInfo?.AverageWeaponDamage.Format() ?? EmDash} weapon dmg / hit
  {DamageTakenInfo?.AverageCritDamage.Format() ?? EmDash} crit dmg / hit
  {DamageTakenInfo?.AverageGlanceDamage.Format() ?? EmDash} glance dmg / hit
{DamageTakenInfo?.AverageNanoDamage.Format() ?? EmDash} nano dmg / hit
{DamageTakenInfo?.AverageIndirectDamage.Format() ?? EmDash} indirect dmg / hit"
+ (!(DamageTakenInfo?.HasSpecials ?? false) ? null : $@"

{DamageTakenInfo.GetSpecialsInfo()}");
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            DamageTakenInfo = DamageTakenInfo ?? Target.DamageTakenInfosBySource.GetValueOrFallback(Source);
            PercentOfTotal = DamageTakenInfo?.PercentOfTargetsTotalDamageTaken;
            PercentOfMax = DamageTakenInfo?.PercentOfTargetsMaxDamagePlusPetsTaken;
            RightText = $"{DamageTakenInfo?.TotalDamage.Format() ?? EmDash} ({DamageTakenInfo?.PercentOfOwnersOrOwnTotalDamagePlusPets.FormatPercent() ?? EmDash}, {DisplayedPercent.FormatPercent()})";

            base.Update(displayIndex);
        }
    }
}
