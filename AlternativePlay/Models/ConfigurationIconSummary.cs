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
        public List<string> PlayModeHints { get; private set; }
        public List<string> TrackerIcons { get; private set; }
        public List<string> GameModifierIcons { get; private set; }
        public List<string> GameModifierHints { get; private set; }

        public ConfigurationIconSummary(PlayModeSettings settings)
        { 
            this.PlayModeIcons = new List<string>();
            this.PlayModeHints = new List<string>();
            this.TrackerIcons = new List<string>();
            this.GameModifierIcons = new List<string>();
            this.GameModifierHints = new List<string>();

            // Play Mode
            switch (settings.PlayMode)
            {
                default:
                case PlayMode.BeatSaber:
                    this.AddBeatSaberIconsAndHints(settings);
                    break;

                case PlayMode.DarthMaul:
                    this.AddDarthMaulIconsAndHints(settings);
                    break;

                case PlayMode.BeatSpear:
                    this.AddBeatSpearIconsAndHints(settings);
                    break;

                case PlayMode.BeatFlail:
                    this.AddBeatFlailIconsAndHints(settings);
                    break;

                case PlayMode.Nunchaku:
                    this.AddNunchakuIconsAndHints(settings);
                    break;
            };

            this.AddTrackerIcons(settings);
            this.AddGameModeIconsAndHints(settings);
        }

        private void AddBeatSaberIconsAndHints(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.BeatSaber);
            this.PlayModeHints.Add(PlayModeSettings.PlayModeDescription(settings.PlayMode));

            if (settings.ReverseLeftSaber && settings.ReverseRightSaber)
            {
                this.PlayModeIcons.Add(IconNames.ReverseBoth);
                this.PlayModeHints.Add("Reverse Both");
            }
            else
            {
                if (settings.ReverseLeftSaber) { this.PlayModeIcons.Add(IconNames.ReverseLeft); this.PlayModeHints.Add("Reverse Left"); }
                if (settings.ReverseRightSaber) { this.PlayModeIcons.Add(IconNames.ReverseRight); this.PlayModeHints.Add("Reverse Right"); }
            }
            if (settings.OneColor) this.PlayModeIcons.Add(settings.UseLeft ? IconNames.LeftSaber : IconNames.RightSaber);
            if (settings.OneColor) this.PlayModeHints.Add(settings.UseLeft ? "One Color Left Saber" : "One Color Right Saber");
        }

        private void AddDarthMaulIconsAndHints(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.DarthMaul);
            this.PlayModeHints.Add(PlayModeSettings.PlayModeDescription(settings.PlayMode));

            if (settings.ControllerCount == ControllerCountEnum.One) { this.PlayModeIcons.Add(IconNames.OneController); this.PlayModeHints.Add("One Controller"); }
            if (settings.ControllerCount == ControllerCountEnum.Two) { this.PlayModeIcons.Add(IconNames.TwoController); this.PlayModeHints.Add("Two Controller"); }
            this.PlayModeIcons.Add(settings.UseLeft ? IconNames.LeftController : IconNames.RightController);
            this.PlayModeHints.Add(settings.UseLeft ? "Left Controller" : "Right Controller");
            if (settings.ReverseMaulDirection) { this.PlayModeIcons.Add(IconNames.ReverseMaulDirection); this.PlayModeHints.Add("Reverse Maul Direction"); }
        }

        private void AddBeatSpearIconsAndHints(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.BeatSpear);
            this.PlayModeHints.Add(PlayModeSettings.PlayModeDescription(settings.PlayMode));

            if (settings.ControllerCount == ControllerCountEnum.One) { this.PlayModeIcons.Add(IconNames.OneController); this.PlayModeHints.Add("One Controller"); }
            if (settings.ControllerCount == ControllerCountEnum.Two) { this.PlayModeIcons.Add(IconNames.TwoController); this.PlayModeHints.Add("Two Controller"); }
            this.PlayModeIcons.Add(settings.UseLeft ? IconNames.LeftController : IconNames.RightController);
            this.PlayModeHints.Add(settings.UseLeft ? "Left Controller" : "Right Controller");
            if (settings.ReverseSpearDirection) { this.PlayModeIcons.Add(IconNames.ReverseSpearDirection); this.PlayModeHints.Add("Reverse Spear Direction"); }
        }

        private void AddBeatFlailIconsAndHints(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.BeatFlail);
            this.PlayModeHints.Add(PlayModeSettings.PlayModeDescription(settings.PlayMode));

            if (settings.LeftFlailMode == BeatFlailMode.Flail) { this.PlayModeIcons.Add(IconNames.LeftFlail); this.PlayModeHints.Add("Left Flail"); }
            if (settings.LeftFlailMode == BeatFlailMode.Sword) { this.PlayModeIcons.Add(IconNames.LeftSaber); this.PlayModeHints.Add("Left Saber"); }
            if (settings.RightFlailMode == BeatFlailMode.Flail) { this.PlayModeIcons.Add(IconNames.RightFlail); this.PlayModeHints.Add("Right Flail"); }
            if (settings.RightFlailMode == BeatFlailMode.Sword) { this.PlayModeIcons.Add(IconNames.RightSaber); this.PlayModeHints.Add("Right Saber"); }
        }

        private void AddNunchakuIconsAndHints(PlayModeSettings settings)
        {
            this.PlayModeIcons.Add(IconNames.Nunchaku);
            this.PlayModeHints.Add(PlayModeSettings.PlayModeDescription(settings.PlayMode));

            if (settings.ReverseNunchaku) { this.PlayModeIcons.Add(IconNames.ReverseNunchaku); this.PlayModeHints.Add("Reverse Nunchaku"); }
            }

        private void AddTrackerIcons(PlayModeSettings settings)
        {
            if (!String.IsNullOrWhiteSpace(settings.LeftTracker.Serial)) this.TrackerIcons.Add(IconNames.LeftTracker);
            if (!String.IsNullOrWhiteSpace(settings.RightTracker.Serial)) this.TrackerIcons.Add(IconNames.RightTracker);
        }       
        
        private void AddGameModeIconsAndHints(PlayModeSettings settings)
        {
            this.GameModifierIcons.Add(settings.NoArrows ? IconNames.NoArrows : IconNames.Empty);
            this.GameModifierIcons.Add(settings.OneColor ? IconNames.OneColor : IconNames.Empty);
            this.GameModifierIcons.Add(settings.NoSliders ? IconNames.NoSliders : IconNames.Empty);
            this.GameModifierIcons.Add(settings.NoArrowsRandom ? IconNames.NoArrowsRandom : IconNames.Empty);
            this.GameModifierIcons.Add(settings.TouchNotes ? IconNames.TouchNotes : IconNames.Empty);

            this.GameModifierHints.Add(settings.NoArrows ? "No Arrows" : "");
            this.GameModifierHints.Add(settings.OneColor ? "One Color" : "");
            this.GameModifierHints.Add(settings.NoSliders ? "No Sliders" : "");
            this.GameModifierHints.Add(settings.NoArrowsRandom ? "No Arrows Random" : "");
            this.GameModifierHints.Add(settings.TouchNotes ? "Touch Notes" : "");
        }
    }
}