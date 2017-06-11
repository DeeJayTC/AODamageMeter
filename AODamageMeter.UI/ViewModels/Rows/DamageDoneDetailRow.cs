﻿using AODamageMeter.UI.Helpers;
using AODamageMeter.UI.Properties;

namespace AODamageMeter.UI.ViewModels.Rows
{
    public sealed class DamageDoneDetailRow : DetailRowBase
    {
        public DamageDoneDetailRow(FightCharacter fightCharacter)
            : base(fightCharacter)
        { }

        public override string RightTextToolTip
        {
            get
            {
                lock (FightCharacter.DamageMeter)
                {
                    return
$@"{DisplayIndex}. {FightCharacterName}

{FightCharacter.TotalDamageDone.ToString("N0")} total dmg

{FightCharacter.WeaponDamageDonePM.Format()} ({FightCharacter.WeaponPercentOfTotalDamageDone.FormatPercent()}) weapon dmg / min
{FightCharacter.NanoDamageDonePM.Format()} ({FightCharacter.NanoPercentOfTotalDamageDone.FormatPercent()}) nano dmg / min
{FightCharacter.IndirectDamageDonePM.Format()} ({FightCharacter.IndirectPercentOfTotalDamageDone.FormatPercent()}) indirect dmg / min
{FightCharacter.TotalDamageDonePM.Format()} total dmg / min

{FightCharacter.WeaponHitDoneChance.FormatPercent()} weapon hit chance
  {FightCharacter.CritDoneChance.FormatPercent()} crit chance
  {FightCharacter.GlanceDoneChance.FormatPercent()} glance chance

{FightCharacter.WeaponHitAttemptsDonePM.Format()} weapon hit attempts / min
  {FightCharacter.WeaponHitsDonePM.Format()} weapon hits / min
  {FightCharacter.CritsDonePM.Format()} crits / min
  {FightCharacter.GlancesDonePM.Format()} glances / min
{FightCharacter.NanoHitsDonePM.Format()} nano hits / min
{FightCharacter.IndirectHitsDonePM.Format()} indirect hits / min
{FightCharacter.TotalHitsDonePM.Format()} total hits / min

{FightCharacter.AverageWeaponDamageDone.Format()} weapon dmg / hit
  {FightCharacter.AverageCritDamageDone.Format()} crit dmg / hit
  {FightCharacter.AverageGlanceDamageDone.Format()} glance dmg / hit
{FightCharacter.AverageNanoDamageDone.Format()} nano dmg / hit
{FightCharacter.AverageIndirectDamageDone.Format()} indirect dmg / hit"
+ (!FightCharacter.HasSpecialsDone ? null : $@"

{FightCharacter.GetSpecialsDoneInfo()}");
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            bool showNPCs = Settings.Default.ShowTopLevelNPCRows;

            PercentWidth = (showNPCs
                ? FightCharacter.PercentOfFightsMaxDamageDonePlusPets
                : FightCharacter.PercentOfFightsMaxPlayerDamageDonePlusPets) ?? 0;
            double? percentDone = Settings.Default.ShowPercentOfTotal
                ? (showNPCs
                    ? FightCharacter.PercentOfFightsTotalDamageDone
                    : FightCharacter.PercentOfFightsTotalPlayerDamageDonePlusPets)
                : (showNPCs
                    ? FightCharacter.PercentOfFightsMaxDamageDonePlusPets
                    : FightCharacter.PercentOfFightsMaxPlayerDamageDonePlusPets);
            RightText = $"{FightCharacter.TotalDamageDone.Format()} ({FightCharacter.TotalDamageDonePM.Format()}, {FightCharacter.PercentOfOwnersOrOwnTotalDamageDonePlusPets.FormatPercent()}, {percentDone.FormatPercent()})";

            base.Update(displayIndex);
        }
    }
}
