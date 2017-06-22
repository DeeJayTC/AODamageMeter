﻿using AODamageMeter.Helpers;
using AODamageMeter.UI.Helpers;
using System.Linq;

namespace AODamageMeter.UI.ViewModels.Rows
{
    public sealed class DamageDoneInfoMainRow : FightCharacterMainRowBase
    {
        public DamageDoneInfoMainRow(FightViewModel fightViewModel, DamageInfo damageDoneInfo)
            : base(fightViewModel, damageDoneInfo.Target)
             => DamageDoneInfo = damageDoneInfo;

        public DamageInfo DamageDoneInfo { get; }
        public FightCharacter Source => DamageDoneInfo.Source;
        public FightCharacter Target => DamageDoneInfo.Target;

        public override string Title => $"{Source.UncoloredName}'s Damage Done to {Target.UncoloredName}";

        public override string RightTextToolTip
        {
            get
            {
                lock (Fight)
                {
                    return
$@"{DisplayIndex}. {Source.UncoloredName} -> {Target.UncoloredName}

{DamageDoneInfo.TotalDamagePlusPets:N0} total dmg

{(DamageDoneInfo.HasIncompleteMissStatsPlusPets ? "≤ " : "")}{DamageDoneInfo.WeaponHitChancePlusPets.FormatPercent()} weapon hit chance
  {DamageDoneInfo.CritChancePlusPets.FormatPercent()} crit chance
  {DamageDoneInfo.GlanceChancePlusPets.FormatPercent()} glance chance

{DamageDoneInfo.AverageWeaponDamagePlusPets.Format()} weapon dmg / hit
  {DamageDoneInfo.AverageCritDamagePlusPets.Format()} crit dmg / hit
  {DamageDoneInfo.AverageGlanceDamagePlusPets.Format()} glance dmg / hit
{DamageDoneInfo.AverageNanoDamagePlusPets.Format()} nano dmg / hit
{DamageDoneInfo.AverageIndirectDamagePlusPets.Format()} indirect dmg / hit"
+ (!DamageDoneInfo.HasSpecials ? null : $@"

{DamageDoneInfo.GetSpecialsInfo()}");
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            PercentOfTotal = DamageDoneInfo.PercentPlusPetsOfSourcesTotalDamageDonePlusPets;
            PercentOfMax = DamageDoneInfo.PercentPlusPetsOfSourcesMaxDamageDonePlusPets;
            RightText = $"{DamageDoneInfo.TotalDamagePlusPets.Format()} ({DisplayedPercent.FormatPercent()})";

            if (Source.IsFightPetOwner)
            {
                int detailRowDisplayIndex = 1;
                foreach (var fightCharacter in new[] { Source }.Concat(Source.FightPets)
                    .OrderByDescending(c => c.DamageDoneInfosByTarget.GetValueOrFallback(Target)?.TotalDamage ?? 0)
                    .ThenBy(c => c.UncoloredName))
                {
                    if (!_detailRowMap.TryGetValue(fightCharacter, out DetailRowBase detailRow))
                    {
                        _detailRowMap.Add(fightCharacter, detailRow = new DamageDoneInfoDetailRow(FightViewModel, fightCharacter, Target));
                        DetailRows.Add(detailRow);
                    }
                    detailRow.Update(detailRowDisplayIndex++);
                }
            }

            base.Update(displayIndex);
        }
    }
}
