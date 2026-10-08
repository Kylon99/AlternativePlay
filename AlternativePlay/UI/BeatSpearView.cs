using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatSpearView : BaseModeSelectView
    {
        protected override void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.ControllerChoice));
            this.NotifyPropertyChanged(nameof(this.UseLeftSpear));
            this.NotifyPropertyChanged(nameof(this.UseTriggerToSwitchHands));
            this.NotifyPropertyChanged(nameof(this.ReverseSpearDirection));
        }

        [UIValue(nameof(BeatSpearIcon))]
        public string BeatSpearIcon => IconNames.BeatSpear;

        [UIValue(nameof(ControllerChoiceIcon))]
        public string ControllerChoiceIcon => this.settings.ControllerCount == ControllerCountEnum.One ? IconNames.OneController : IconNames.TwoController;

        [UIValue(nameof(ControllerChoice))]
        private string ControllerChoice
        { 
            get => this.settings.ControllerCount.ToString();
            set
            {
                this.settings.ControllerCount = (ControllerCountEnum)Enum.Parse(typeof(ControllerCountEnum), value);
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.ControllerChoiceIcon));
            }
        }

        [UIValue(nameof(ControllerChoiceList))]
        private List<object> ControllerChoiceList => new List<object> { ControllerCountEnum.One.ToString(), ControllerCountEnum.Two.ToString() };

        [UIValue(nameof(UseLeftSpearIcon))]
        public string UseLeftSpearIcon => this.settings.UseLeft ? IconNames.LeftSaber : IconNames.RightSaber;

        [UIValue(nameof(UseLeftSpear))]
        private bool UseLeftSpear
        {
            get => this.settings.UseLeft;
            set
            {
                this.settings.UseLeft = value;
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.UseLeftSpearIcon));
            }
        }

        [UIValue(nameof(UseTriggerToSwitchHands))]
        private bool UseTriggerToSwitchHands
        {
            get => this.settings.UseTriggerToSwitchHands;
            set
            {
                this.settings.UseTriggerToSwitchHands = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(ReverseSpearDirectionIcon))]
        public string ReverseSpearDirectionIcon => IconNames.ReverseSpearDirection;

        [UIValue(nameof(ReverseSpearDirection))]
        private bool ReverseSpearDirection
        {
            get => this.settings.ReverseSpearDirection;
            set
            {
                this.settings.ReverseSpearDirection = value;
                this.configuration.SaveConfiguration();
            }
        }
    }
}
