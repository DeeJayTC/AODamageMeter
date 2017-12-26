﻿using AODamageMeter.UI.Helpers;
using AODamageMeter.UI.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace AODamageMeter.UI.ViewModels.Rows
{
    public sealed class DamageDoneViewingModeMainRow : ViewingModeMainRowBase
    {
        private readonly Dictionary<FightCharacter, DetailRowBase> _detailRowMap = new Dictionary<FightCharacter, DetailRowBase>();

        public DamageDoneViewingModeMainRow(FightViewModel fightViewModel)
            : base(ViewingMode.DamageDone, "Damage Done", 1, "/Icons/DamageDone.png", Color.FromRgb(91, 84, 183), fightViewModel)
        { }

        public override string LeftTextToolTip
        {
            get
            {
                lock (Fight)
                {
                    var counts = Fight.GetFightCharacterCounts(
                        includeNPCs: Settings.Default.IncludeTopLevelNPCRows,
                        includeZeroDamageDones: Settings.Default.IncludeTopLevelZeroDamageRows);

                    return counts.GetFightCharacterCountsTooltip(Title);
                }
            }
        }

        public override string RightTextToolTip
        {
            get
            {
                lock (Fight)
                {
                    var stats = Fight.GetDamageDoneStats(
                        includeNPCs: Settings.Default.IncludeTopLevelNPCRows,
                        includeZeroDamageDones: Settings.Default.IncludeTopLevelZeroDamageRows);

                    return
$@"{Title}

{stats.FightCharacterCount} {(stats.FightCharacterCount == 1 ? "character": "characters")}

{stats.TotalDamage:N0} total dmg

{stats.WeaponDamagePM.Format()} ({stats.WeaponPercentOfTotalDamage.FormatPercent()}) weapon dmg / min
{stats.NanoDamagePM.Format()} ({stats.NanoPercentOfTotalDamage.FormatPercent()}) nano dmg / min
{stats.IndirectDamagePM.Format()} ({stats.IndirectPercentOfTotalDamage.FormatPercent()}) indirect dmg / min
{stats.AbsorbedDamagePM.Format()} ({stats.AbsorbedPercentOfTotalDamage.FormatPercent()}) absorbed dmg / min
{stats.TotalDamagePM.Format()} total dmg / min

≤ {stats.WeaponHitChance.FormatPercent()} weapon hit chance
  {stats.CritChance.FormatPercent()} crit chance
  {stats.GlanceChance.FormatPercent()} glance chance

≥ {stats.WeaponHitAttemptsPM.Format()} weapon hit attempts / min
{stats.WeaponHitsPM.Format()} weapon hits / min
  {stats.RegularsPM.Format()} regulars / min
    {stats.NormalsPM.Format()} normals / min
    {stats.CritsPM.Format()} crits / min
    {stats.GlancesPM.Format()} glances / min
  {stats.SpecialsPM.Format()} specials / min
{stats.NanoHitsPM.Format()} nano hits / min
{stats.IndirectHitsPM.Format()} indirect hits / min
{stats.AbsorbedHitsPM.Format()} absorbed hits / min
{stats.TotalHitsPM.Format()} total hits / min

{stats.AverageWeaponDamage.Format()} weapon dmg / hit
  {stats.AverageRegularDamage.Format()} regular dmg / hit
    {stats.AverageNormalDamage.Format()} normal dmg / hit
    {stats.AverageCritDamage.Format()} crit dmg / hit
    {stats.AverageGlanceDamage.Format()} glance dmg / hit
  {stats.AverageSpecialDamage.Format()} special dmg / hit
{stats.AverageNanoDamage.Format()} nano dmg / hit
{stats.AverageIndirectDamage.Format()} indirect dmg / hit
{stats.AverageAbsorbedDamage.Format()} absorbed dmg / hit"
+ (!stats.HasSpecials ? null : $@"

{stats.GetSpecialsDoneInfo()}")
+ (stats.TotalDamage == 0 ? null : $@"

{stats.GetDamageTypesDoneInfo()}");
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            RightText = Settings.Default.IncludeTopLevelNPCRows
                ? $"{Fight.TotalDamageDone.Format()} ({Fight.TotalDamageDonePM.Format()})"
                : $"{Fight.TotalPlayerDamageDonePlusPets.Format()} ({Fight.TotalPlayerDamageDonePMPlusPets.Format()})";

            var topFightCharacters = Fight.FightCharacters
                .Where(c => (Settings.Default.IncludeTopLevelNPCRows || c.IsPlayerOrFightPet)
                    && (Settings.Default.IncludeTopLevelZeroDamageRows || c.TotalDamageDonePlusPets != 0))
                .Where(c => !c.IsFightPet)
                .OrderByDescending(c => c.TotalDamageDonePlusPets)
                .ThenBy(c => c.UncoloredName)
                .Take(Settings.Default.MaxNumberOfDetailRows).ToArray();

            foreach (var noLongerTopFightCharacter in _detailRowMap.Keys
                .Except(topFightCharacters).ToArray())
            {
                DetailRows.Remove(_detailRowMap[noLongerTopFightCharacter]);
                _detailRowMap.Remove(noLongerTopFightCharacter);
            }

            int detailRowDisplayIndex = 1;
            foreach (var fightCharacter in topFightCharacters)
            {
                if (!_detailRowMap.TryGetValue(fightCharacter, out DetailRowBase detailRow))
                {
                    _detailRowMap.Add(fightCharacter, detailRow = new DamageDoneViewingModeDetailRow(FightViewModel, fightCharacter));
                    DetailRows.Add(detailRow);
                }
                detailRow.Update(detailRowDisplayIndex++);
            }

            base.Update();
        }

        public override bool TryCopyAndScriptProgressedRowsInfo()
            => CopyAndScriptProgressedRowsInfo(FightViewModel.GetUpdatedDamageDoneRows());
    }
}
