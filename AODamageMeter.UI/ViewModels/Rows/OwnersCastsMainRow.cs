﻿using AODamageMeter.UI.Helpers;
using AODamageMeter.UI.Properties;
using System.Windows.Media;

namespace AODamageMeter.UI.ViewModels.Rows
{
    public class OwnersCastsMainRow : MainRowBase
    {
        public OwnersCastsMainRow(CastInfo castInfo)
        {
            CastInfo = castInfo;
            IconPath = "/Icons/Cast.png";
            Color = Color.FromRgb(54, 111, 238);
        }

        public CastInfo CastInfo { get; }
        public FightCharacter Source => CastInfo.Source;

        public override string LeftText => CastInfo.NanoProgram;

        public override string LeftTextToolTip
            => $"{DisplayIndex}. {CastInfo.NanoProgram}";

        public override string RightTextToolTip
        {
            get
            {
                lock (Source.DamageMeter)
                {
                    return
$@"{CastInfo.CastSuccesses.ToString("N0")} ({CastInfo.CastSuccessChance.FormatPercent()}) succeeded
{CastInfo.CastCountereds.ToString("N0")} ({CastInfo.CastCounteredChance.FormatPercent()}) countered
{CastInfo.CastAborteds.ToString("N0")} ({CastInfo.CastAbortedChance.FormatPercent()}) aborted
{CastInfo.CastAttempts.ToString("N0")} attempted

{CastInfo.CastSuccessesPM.Format()} succeeded / min
{CastInfo.CastCounteredsPM.Format()} countered / min
{CastInfo.CastAbortedsPM.Format()} aborted / min
{CastInfo.CastAttemptsPM.Format()} attempted / min";
                }
            }
        }

        public override void Update(int? displayIndex = null)
        {
            PercentWidth = CastInfo.PercentOfSourcesMaxCastAttempts ?? 0;
            double? percentDone = Settings.Default.ShowPercentOfTotal
                ? CastInfo.PercentOfSourcesCastAttempts : CastInfo.PercentOfSourcesMaxCastAttempts;
            RightText = $"{CastInfo.CastSuccesses.ToString("N0")} / {CastInfo.CastAttempts.ToString("N0")} ({CastInfo.CastSuccessChance.FormatPercent()}, {percentDone.FormatPercent()})";

            DisplayIndex = displayIndex ?? DisplayIndex;
            RaisePropertyChanged(nameof(LeftTextToolTip));
            RaisePropertyChanged(nameof(RightTextToolTip));
        }
    }
}
