using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatSaberView : BaseModeSelectView
    {
        protected override void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.ReverseLeftSaber));
            this.NotifyPropertyChanged(nameof(this.ReverseRightSaber));
            this.NotifyPropertyChanged(nameof(this.RemoveOtherSaber));
            this.NotifyPropertyChanged(nameof(this.UseLeftSaber));
        }

        [UIValue(nameof(BeatSaberIcon))]
        public string BeatSaberIcon => IconNames.BeatSaber;

        [UIValue(nameof(ReverseLeftSaberIcon))]
        public string ReverseLeftSaberIcon => this.settings.ReverseLeftSaber && this.settings.ReverseRightSaber ? IconNames.ReverseBoth : IconNames.ReverseLeft;

        [UIValue(nameof(ReverseLeftSaber))]
        private bool ReverseLeftSaber
        {
            get => this.settings.ReverseLeftSaber;
            set
            {
                this.settings.ReverseLeftSaber = value;
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.ReverseLeftSaberIcon));
                this.NotifyPropertyChanged(nameof(this.ReverseRightSaberIcon));
            }
        }

        [UIValue(nameof(ReverseRightSaberIcon))]
        public string ReverseRightSaberIcon => this.settings.ReverseLeftSaber && this.settings.ReverseRightSaber ? IconNames.ReverseBoth : IconNames.ReverseRight;

        [UIValue(nameof(ReverseRightSaber))]
        private bool ReverseRightSaber
        {
            get => this.settings.ReverseRightSaber;
            set
            {
                this.settings.ReverseRightSaber = value;
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.ReverseLeftSaberIcon));
                this.NotifyPropertyChanged(nameof(this.ReverseRightSaberIcon));
            }
        }

        [UIValue(nameof(RemoveOtherSaber))]
        private bool RemoveOtherSaber
        {
            get => this.settings.RemoveOtherSaber;
            set
            {
                this.settings.RemoveOtherSaber = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(UseLeftSaberIcon))]
        public string UseLeftSaberIcon => this.settings.UseLeft ? IconNames.LeftSaber : IconNames.RightSaber;

        [UIValue(nameof(UseLeftSaber))]
        private bool UseLeftSaber
        {
            get => this.settings.UseLeft;
            set
            {
                this.settings.UseLeft = value;
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.UseLeftSaberIcon));
            }
        }
    }
}
