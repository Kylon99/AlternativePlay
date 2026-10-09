using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class DarthMaulView : BaseModeSelectView
    {
        protected override void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.ControllerChoice));
            this.NotifyPropertyChanged(nameof(this.UseLeftController));
            this.NotifyPropertyChanged(nameof(this.ReverseDarthMaul));
            this.NotifyPropertyChanged(nameof(this.UseTriggerToSeparate));
            this.NotifyPropertyChanged(nameof(this.SeparationAmount));
        }

        [UIValue(nameof(DarthMaulIcon))]
        public string DarthMaulIcon => IconNames.DarthMaul;

        [UIValue(nameof(ControllerChoiceIcon))]
        public string ControllerChoiceIcon => this.settings.ControllerCount == ControllerCountEnum.One ? IconNames.OneController: IconNames.TwoController;

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

        [UIValue(nameof(UseLeftControllerIcon))]
        public string UseLeftControllerIcon => this.settings.UseLeft ? IconNames.LeftController : IconNames.RightController;

        [UIValue(nameof(UseLeftController))]
        private bool UseLeftController
        {
            get => this.settings.UseLeft;
            set
            {
                this.settings.UseLeft = value;
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.UseLeftControllerIcon));
            }
        }

        [UIValue(nameof(ReverseDarthMaulIcon))]
        public string ReverseDarthMaulIcon => IconNames.ReverseMaulDirection;

        [UIValue(nameof(ReverseDarthMaul))]
        private bool ReverseDarthMaul
        {
            get => this.settings.ReverseMaulDirection;
            set
            {
                this.settings.ReverseMaulDirection = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(UseTriggerToSeparate))]
        private bool UseTriggerToSeparate
        {
            get => this.settings.UseTriggerToSeparate;
            set
            {
                this.settings.UseTriggerToSeparate = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(SeparationAmount))]
        private int SeparationAmount
        {
            get => this.settings.MaulDistance;
            set
            {
                this.settings.MaulDistance = value;
                this.configuration.SaveConfiguration();
            }
        }
    }
}
