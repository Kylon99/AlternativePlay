using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using System;

namespace AlternativePlay.UI
{
    /// <summary>
    /// Represents one selectable option in the list on both <see cref="AlternativePlayView"/> 
    /// and <see cref="PlayModeSelectTab"/>.
    /// </summary>
    public class PlayModeSelectOption
    {
        public int Index { get; private set; }

        public ConfigurationIconSummary IconSummary { get; private set; }

        public Action<int> EditCallback { get; set; }
        public Action<int> DeleteCallback { get; set; }

        private ConfigurationData configurationData;

        public PlayModeSelectOption(ConfigurationData configuration, int index, Action<int> editCallback = null, Action<int> deleteCallback = null)
        {
            this.configurationData = configuration;
            PlayModeSettings settings = configuration.PlayModeSettings[index];
            this.Mode = PlayModeSettings.PlayModeDescription(settings.PlayMode);
            this.Index = index;
            this.IconSummary = new ConfigurationIconSummary(settings);
            this.EditCallback = editCallback;
            this.DeleteCallback = deleteCallback;
        }

        [UIValue(nameof(Mode))]
        public string Mode { get; set; }

        [UIValue(nameof(SelectedColor))]
        public string SelectedColor => this.configurationData.Selected == this.Index ? "#FFFFFF" : "#7F7F7F";

        [UIAction(nameof(OnEditClicked))]
        public void OnEditClicked()
        {
            if (this.EditCallback != null) this.EditCallback(this.Index);
        }

        [UIAction(nameof(OnDeleteClicked))]
        public void OnDeleteClicked()
        {
            if (this.DeleteCallback != null) this.DeleteCallback(this.Index);
        }

        private string GetPlayModeIcon(int index)
        {
            if (this.IconSummary == null || index >= this.IconSummary.PlayModeIcons.Count) return IconNames.Empty;
            return this.IconSummary.PlayModeIcons[index];
        }

        private string GetPlayModeHint(int index)
        {
            if (this.IconSummary == null || index >= this.IconSummary.PlayModeHints.Count) return "";
            return this.IconSummary.PlayModeHints[index];
        }

        private string GetTrackerIcon(int index)
        {
            if (this.IconSummary == null || index >= this.IconSummary.TrackerIcons.Count) return IconNames.Empty;
            return this.IconSummary.TrackerIcons[index];
        }

        private string GetGameModifierIcon(int index)
        {
            if (this.IconSummary == null || index >= this.IconSummary.GameModifierIcons.Count) return IconNames.Empty;
            return this.IconSummary.GameModifierIcons[index];
        }

        private string GetGameModifierHint(int index)
        {
            if (this.IconSummary == null || index >= this.IconSummary.GameModifierHints.Count) return "";
            return this.IconSummary.GameModifierHints[index];
        }


        [UIValue(nameof(PlayModeIcon00))]
        public string PlayModeIcon00 => this.GetPlayModeIcon(0);
        [UIValue(nameof(PlayModeIcon01))]
        public string PlayModeIcon01 => this.GetPlayModeIcon(1);
        [UIValue(nameof(PlayModeIcon02))]
        public string PlayModeIcon02 => this.GetPlayModeIcon(2);
        [UIValue(nameof(PlayModeIcon03))]
        public string PlayModeIcon03 => this.GetPlayModeIcon(3);

        [UIValue(nameof(TrackerIcon00))]
        public string TrackerIcon00 => this.GetTrackerIcon(0);
        [UIValue(nameof(TrackerIcon01))]
        public string TrackerIcon01 => this.GetTrackerIcon(1);

        [UIValue(nameof(GameModifierIcon00))]
        public string GameModifierIcon00 => this.GetGameModifierIcon(0);
        [UIValue(nameof(GameModifierIcon01))]
        public string GameModifierIcon01 => this.GetGameModifierIcon(1);
        [UIValue(nameof(GameModifierIcon02))]
        public string GameModifierIcon02 => this.GetGameModifierIcon(2);
        [UIValue(nameof(GameModifierIcon03))]
        public string GameModifierIcon03 => this.GetGameModifierIcon(3);
        [UIValue(nameof(GameModifierIcon04))]
        public string GameModifierIcon04 => this.GetGameModifierIcon(4);

        [UIValue(nameof(PlayModeHint00))]
        public string PlayModeHint00 => this.GetPlayModeHint(0);
        [UIValue(nameof(PlayModeHint01))]
        public string PlayModeHint01 => this.GetPlayModeHint(1);
        [UIValue(nameof(PlayModeHint02))]
        public string PlayModeHint02 => this.GetPlayModeHint(2);
        [UIValue(nameof(PlayModeHint03))]
        public string PlayModeHint03 => this.GetPlayModeHint(3);

        [UIValue(nameof(GameModifierHint00))]
        public string GameModifierHint00 => this.GetGameModifierHint(0);
        [UIValue(nameof(GameModifierHint01))]
        public string GameModifierHint01 => this.GetGameModifierHint(1);
        [UIValue(nameof(GameModifierHint02))]
        public string GameModifierHint02 => this.GetGameModifierHint(2);
        [UIValue(nameof(GameModifierHint03))]
        public string GameModifierHint03 => this.GetGameModifierHint(3);
        [UIValue(nameof(GameModifierHint04))]
        public string GameModifierHint04 => this.GetGameModifierHint(4);

    }

}