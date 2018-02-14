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
$@"{DisplayIndex}. {Title}

{DamageTakenInfo?.TotalDamage.ToString("N0") ?? EmDash} ({PercentOfMastersOrOwnTotalPlusPets.FormatPercent()}) total dmg
{PercentOfTotal.FormatPercent()} of {Target.UncoloredName}'s total dmg
{PercentOfMax.FormatPercent()} of {Target.UncoloredName}'s max dmg

{DamageTakenInfo?.WeaponPercentOfTotalDamage.FormatPercent() ?? EmDashPercent} weapon dmg
{DamageTakenInfo?.NanoPercentOfTotalDamage.FormatPercent() ?? EmDashPercent} nano dmg
{DamageTakenInfo?.IndirectPercentOfTotalDamage.FormatPercent() ?? EmDashPercent} indirect dmg
{(!DamageTakenInfo?.HasCompleteAbsorbedDamageStats ?? false ? "≥ " : "")}{DamageTakenInfo?.AbsorbedPercentOfTotalDamage.FormatPercent() ?? EmDashPercent} absorbed dmg

{(!DamageTakenInfo?.HasCompleteMissStats ?? false ? "≤ " : "")}{DamageTakenInfo?.WeaponHitChance.FormatPercent() ?? EmDashPercent} weapon hit chance
  {DamageTakenInfo?.CritChance.FormatPercent() ?? EmDashPercent} crit chance
  {DamageTakenInfo?.GlanceChance.FormatPercent() ?? EmDashPercent} glance chance

{DamageTakenInfo?.AverageWeaponDamage.Format() ?? EmDash} weapon dmg / hit
  {DamageTakenInfo?.AverageRegularDamage.Format() ?? EmDash} regular dmg / hit
    {DamageTakenInfo?.AverageNormalDamage.Format() ?? EmDash} normal dmg / hit
    {DamageTakenInfo?.AverageCritDamage.Format() ?? EmDash} crit dmg / hit
    {DamageTakenInfo?.AverageGlanceDamage.Format() ?? EmDash} glance dmg / hit
  {DamageTakenInfo?.AverageSpecialDamage.Format() ?? EmDash} special dmg / hit
{DamageTakenInfo?.AverageNanoDamage.Format() ?? EmDash} nano dmg / hit
{DamageTakenInfo?.AverageIndirectDamage.Format() ?? EmDash} indirect dmg / hit
{DamageTakenInfo?.AverageAbsorbedDamage.Format() ?? EmDash} absorbed dmg / hit"
+ (!DamageTakenInfo?.HasSpecials ?? true ? null : $@"

{DamageTakenInfo.GetSpecialsInfo()}")
+ ((DamageTakenInfo?.TotalDamage ?? 0) == 0 ? null : $@"

{DamageTakenInfo.GetDamageTypesInfo()}");
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            DamageTakenInfo = DamageTakenInfo ?? Target.DamageTakenInfosBySource.GetValueOrFallback(Source);
            PercentOfTotal = DamageTakenInfo?.PercentOfTargetsTotalDamageTaken;
            PercentOfMax = DamageTakenInfo?.PercentOfTargetsMaxDamagePlusPetsTaken;
            PercentOfMastersOrOwnTotalPlusPets = DamageTakenInfo?.PercentOfMastersOrOwnTotalDamagePlusPets;
            RightText = $"{DamageTakenInfo?.TotalDamage.Format() ?? EmDash} ({PercentOfMastersOrOwnTotalPlusPets.FormatPercent() ?? EmDashPercent}, {DisplayedPercent.FormatPercent()})";

            base.Update(displayIndex);
        }
    }
}
