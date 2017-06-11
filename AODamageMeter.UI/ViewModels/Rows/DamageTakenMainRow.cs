﻿using AODamageMeter.UI.Helpers;
using AODamageMeter.UI.Properties;

namespace AODamageMeter.UI.ViewModels.Rows
{
    public sealed class DamageTakenMainRow : MainRowBase
    {
        public DamageTakenMainRow(FightCharacter fightCharacter)
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

{FightCharacter.TotalDamageTaken.ToString("N0")} total dmg

{FightCharacter.WeaponDamageTakenPM.Format()} ({FightCharacter.WeaponPercentOfTotalDamageTaken.FormatPercent()}) weapon dmg / min
{FightCharacter.NanoDamageTakenPM.Format()} ({FightCharacter.NanoPercentOfTotalDamageTaken.FormatPercent()}) nano dmg / min
{FightCharacter.IndirectDamageTakenPM.Format()} ({FightCharacter.IndirectPercentOfTotalDamageTaken.FormatPercent()}) indirect dmg / min
{FightCharacter.TotalDamageTakenPM.Format()} total dmg / min

{FightCharacter.WeaponHitTakenChance.FormatPercent()} weapon hit chance
  {FightCharacter.CritTakenChance.FormatPercent()} crit chance
  {FightCharacter.GlanceTakenChance.FormatPercent()} glance chance

{FightCharacter.WeaponHitAttemptsTakenPM.Format()} weapon hit attempts / min
  {FightCharacter.WeaponHitsTakenPM.Format()} weapon hits / min
  {FightCharacter.CritsTakenPM.Format()} crits / min
  {FightCharacter.GlancesTakenPM.Format()} glances / min
{FightCharacter.NanoHitsTakenPM.Format()} nano hits / min
{FightCharacter.IndirectHitsTakenPM.Format()} indirect hits / min
{FightCharacter.TotalHitsTakenPM.Format()} total hits / min

{FightCharacter.AverageWeaponDamageTaken.Format()} weapon dmg / hit
  {FightCharacter.AverageCritDamageTaken.Format()} crit dmg / hit
  {FightCharacter.AverageGlanceDamageTaken.Format()} glance dmg / hit
{FightCharacter.AverageNanoDamageTaken.Format()} nano dmg / hit
{FightCharacter.AverageIndirectDamageTaken.Format()} indirect dmg / hit"
+ (!FightCharacter.HasSpecialsTaken ? null : $@"

{FightCharacter.GetSpecialsTakenInfo()}")
+ (FightCharacter.HitsAbsorbed == 0 ? null : $@"

{FightCharacter.DamageAbsorbed.ToString("N0")} dmg absorbed
{FightCharacter.HitsAbsorbedPM.Format()} hits absorbed / min
{FightCharacter.DamageAbsorbedPM.Format()} dmg absorbed / min
{FightCharacter.AverageDamageAbsorbed.Format()} dmg absorbed / hit");
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            bool showNPCs = Settings.Default.ShowTopLevelNPCRows;

            PercentWidth = (showNPCs
                ? FightCharacter.PercentOfFightsMaxDamageTaken
                : FightCharacter.PercentOfFightsMaxPlayerOrPetDamageTaken) ?? 0;
            double? percentTaken = Settings.Default.ShowPercentOfTotal
                ? (showNPCs
                    ? FightCharacter.PercentOfFightsTotalDamageTaken
                    : FightCharacter.PercentOfFightsTotalPlayerOrPetDamageTaken)
                : (showNPCs
                    ? FightCharacter.PercentOfFightsMaxDamageTaken
                    : FightCharacter.PercentOfFightsMaxPlayerOrPetDamageTaken);
            RightText = $"{FightCharacter.TotalDamageTaken.Format()} ({FightCharacter.TotalDamageTakenPM.Format()}, {percentTaken.FormatPercent()})";

            base.Update(displayIndex);
        }
    }
}
