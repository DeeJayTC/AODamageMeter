﻿using AODamageMeter.Helpers;
using AODamageMeter.UI.Helpers;
using AODamageMeter.UI.Properties;

namespace AODamageMeter.UI.ViewModels.Rows
{
    public class OwnersHealingTakenDetailRow : DetailRowBase
    {
        public OwnersHealingTakenDetailRow(FightCharacter target, FightCharacter source)
            : base(source)
        {
            Target = target;
            Source = source;
        }

        public HealingInfo HealingTakenInfo { get; private set; }
        public FightCharacter Target { get; }
        public FightCharacter Source { get; }
        public bool IsOwnerTheSource => Source.IsDamageMeterOwner;

        public override string RightTextToolTip
        {
            get
            {
                lock (Source.DamageMeter)
                {
                    return
$@"{DisplayIndex}. {Target.UncoloredName} <- {Source.UncoloredName}

{(IsOwnerTheSource ? "≥ " : "")}{HealingTakenInfo?.PotentialHealing.Format() ?? EmDash} potential healing
{HealingTakenInfo?.RealizedHealing.Format() ?? EmDash} realized healing
{(IsOwnerTheSource ? "≥ " : "")}{HealingTakenInfo?.Overhealing.Format() ?? EmDash} overhealing
{(IsOwnerTheSource ? "≥ " : "")}{HealingTakenInfo?.NanoHealing.Format() ?? EmDash} nano healing

{(IsOwnerTheSource ? "≥ " : "")}{HealingTakenInfo?.PercentOfOverhealing.FormatPercent() ?? EmDash} overhealing";
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            HealingTakenInfo = HealingTakenInfo ?? Target.HealingTakenInfosBySource.GetValueOrFallback(Source);

            PercentWidth = HealingTakenInfo?.PercentOfTargetsMaxPotentialHealingPlusPetsTaken ?? 0;
            double? percentDone = Settings.Default.ShowPercentOfTotal
                ? HealingTakenInfo?.PercentOfTargetsPotentialHealingTaken : HealingTakenInfo?.PercentOfTargetsMaxPotentialHealingPlusPetsTaken;
            RightText = $"{HealingTakenInfo?.PotentialHealing.Format() ?? EmDash} ({HealingTakenInfo?.PercentOfOwnersOrOwnPotentialHealingPlusPets.FormatPercent() ?? EmDash}, {percentDone.FormatPercent()})";

            base.Update(displayIndex);
        }
    }
}
