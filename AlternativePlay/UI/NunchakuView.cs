using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class NunchakuView : BaseModeSelectView
    {
        protected override void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.ReverseNunchaku));
            this.NotifyPropertyChanged(nameof(this.NunchakuLength));
            this.NotifyPropertyChanged(nameof(this.Gravity));
        }

        [UIValue(nameof(NunchakuIcon))]
        public string NunchakuIcon => IconNames.Nunchaku;

        [UIValue(nameof(ReverseNunchakuIcon))]
        public string ReverseNunchakuIcon => IconNames.ReverseNunchaku;

        [UIValue(nameof(ReverseNunchaku))]
        private bool ReverseNunchaku
        {
            get => this.settings.ReverseNunchaku;
            set
            {
                this.settings.ReverseNunchaku = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(NunchakuLength))]
        private int NunchakuLength
        {
            get => this.settings.NunchakuLength;
            set
            {
                this.settings.NunchakuLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(Gravity))]
        private float Gravity
        {
            get => this.settings.Gravity;
            set
            {
                this.settings.Gravity = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIAction(nameof(OnResetGravity))]
        private void OnResetGravity()
        {
            this.settings.Gravity = 3.5f;
            this.configuration.SaveConfiguration();
            this.NotifyPropertyChanged(nameof(this.Gravity));
        }

        [UIAction(nameof(LengthFormatter))]
        private string LengthFormatter(int value)
        {
            return $"{value} cm";
        }
    }
}