using AlternativePlay.Models;
using System;
using System.Collections.Generic;

namespace AlternativePlay.UI
{
    /// <summary>
    /// Converts a <see cref="PlayModeSettings"/> into a list of strings indicating
    /// which icon to use.  This helps to represent the <see cref="PlayModeSettings"/>
    /// visually as a set of icons.
    /// </summary>
    public class ConfigurationIconSummary
    {
        public List<string> PlayModeIcons { get; private set; }
        public List<string> TrackerIcons { get; private set; }
        public List<string> GameModifierIcons { get; private set; }

        public ConfigurationIconSummary(PlayModeSettings settings)
        { 
            this.PlayModeIcons = new List<string>();
            this.TrackerIcons = new List<string>();
            this.GameModifierIcons = new List<string>();

            // Play Mode
            switch (settings.PlayMode)
            {
                default:
                case PlayMode.BeatSaber:
                    this.AddBeatSaberIcons(settings);
                    break;

                case PlayMode.DarthMaul:
                    this.AddDarthMaulIcons(settings);
                    break;

                case PlayMode.BeatSpear:
                    this.AddBeatSpearIcons(settings);
                    break;

                case PlayMode.BeatFlail:
                    this.AddBeatFlailIcons(settings);
                    break;

                case PlayMode.Nunchaku:
                    this.AddNunchakuIcons(settings);
                    break;
            };

            this.AddTrackerIcons(settings);
            this.AddGameModeIcons(settings);
        }

        private void AddBeatSaberIcons(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.BeatSaber);

            if (settings.ReverseLeftSaber && settings.ReverseRightSaber)
            {
                this.PlayModeIcons.Add(IconNames.ReverseBoth);
            }
            else
            {
                if (settings.ReverseLeftSaber) this.PlayModeIcons.Add(IconNames.ReverseLeft);
                if (settings.ReverseRightSaber) this.PlayModeIcons.Add(IconNames.ReverseRight);
            }
            if (settings.OneColor) this.PlayModeIcons.Add(settings.UseLeft ? IconNames.LeftSaber : IconNames.RightSaber);
        }

        private void AddDarthMaulIcons(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.DarthMaul);

            if (settings.ControllerCount == ControllerCountEnum.One) { this.PlayModeIcons.Add(IconNames.OneController); }
            if (settings.ControllerCount == ControllerCountEnum.Two) { this.PlayModeIcons.Add(IconNames.TwoController); }
            this.PlayModeIcons.Add(settings.UseLeft ? IconNames.LeftController : IconNames.RightController);
            if (settings.ReverseMaulDirection) { this.PlayModeIcons.Add(IconNames.ReverseMaulDirection); }
        }

        private void AddBeatSpearIcons(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.BeatSpear);

            if (settings.ControllerCount == ControllerCountEnum.One) { this.PlayModeIcons.Add(IconNames.OneController); }
            if (settings.ControllerCount == ControllerCountEnum.Two) { this.PlayModeIcons.Add(IconNames.TwoController); }
            this.PlayModeIcons.Add(settings.UseLeft ? IconNames.LeftController : IconNames.RightController);
            if (settings.ReverseSpearDirection) { this.PlayModeIcons.Add(IconNames.ReverseSpearDirection); }
        }

        private void AddBeatFlailIcons(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.BeatFlail);

            if (settings.LeftFlailMode == BeatFlailMode.Flail) { this.PlayModeIcons.Add(IconNames.LeftFlail); }
            if (settings.LeftFlailMode == BeatFlailMode.Sword) { this.PlayModeIcons.Add(IconNames.LeftSaber); }
            if (settings.RightFlailMode == BeatFlailMode.Flail) { this.PlayModeIcons.Add(IconNames.RightFlail); }
            if (settings.RightFlailMode == BeatFlailMode.Sword) { this.PlayModeIcons.Add(IconNames.RightSaber); }
        }

        private void AddNunchakuIcons(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.Nunchaku);

            if (settings.ReverseNunchaku) this.PlayModeIcons.Add(IconNames.ReverseNunchaku);
        }

        private void AddTrackerIcons(PlayModeSettings settings)
        {
            if (!String.IsNullOrWhiteSpace(settings.LeftTracker.Serial)) this.TrackerIcons.Add(IconNames.LeftTracker);
            if (!String.IsNullOrWhiteSpace(settings.RightTracker.Serial)) this.TrackerIcons.Add(IconNames.RightTracker);
        }       
        
        private void AddGameModeIcons(PlayModeSettings settings)
        {
            this.GameModifierIcons.Add(settings.NoArrows ? IconNames.NoArrows : IconNames.Empty);
            this.GameModifierIcons.Add(settings.OneColor ? IconNames.OneColor : IconNames.Empty);
            this.GameModifierIcons.Add(settings.NoSliders ? IconNames.NoSliders : IconNames.Empty);
            this.GameModifierIcons.Add(settings.NoArrowsRandom ? IconNames.NoArrowsRandom : IconNames.Empty);
            this.GameModifierIcons.Add(settings.TouchNotes ? IconNames.TouchNotes : IconNames.Empty);
        }
    }
}