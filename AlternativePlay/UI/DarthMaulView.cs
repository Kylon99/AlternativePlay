using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class DarthMaulView : BSMLAutomaticViewController
    {
        private PlayModeSettings settings;

        public void SetPlayModeSettings(PlayModeSettings Settings)
        {
            this.settings = Settings;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            this.UpdateAllValues();
            this.SetTrackerText();
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

        private void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.ControllerChoice));
            this.NotifyPropertyChanged(nameof(this.UseLeftController));
            this.NotifyPropertyChanged(nameof(this.ReverseDarthMaul));
            this.NotifyPropertyChanged(nameof(this.UseTriggerToSeparate));
            this.NotifyPropertyChanged(nameof(this.SeparationAmount));

            this.SetTrackerText();
        }

        #region Tracker Selection Members

        // Text Displays for the Main View
        private string leftTrackerSerial;
        [UIValue(nameof(LeftTrackerSerial))]
        public string LeftTrackerSerial { get => this.leftTrackerSerial; set { this.leftTrackerSerial = value; this.NotifyPropertyChanged(); } }

        private string leftTrackerHoverHint;
        [UIValue(nameof(LeftTrackerHoverHint))]
        public string LeftTrackerHoverHint { get => this.leftTrackerHoverHint; set { this.leftTrackerHoverHint = value; this.NotifyPropertyChanged(); } }

        private string rightTrackerSerial;
        [UIValue(nameof(RightTrackerSerial))]
        public string RightTrackerSerial { get => this.rightTrackerSerial; set { this.rightTrackerSerial = value; this.NotifyPropertyChanged(); } }

        private string rightTrackerHoverHint;
        [UIValue(nameof(RightTrackerHoverHint))]
        public string RightTrackerHoverHint { get => this.rightTrackerHoverHint; set { this.rightTrackerHoverHint = value; this.NotifyPropertyChanged(); } }

        // Text Display for the Current Tracker in the Tracker Select Modal
        private string currentTrackerText;

        [UIValue(nameof(CurrentTrackerText))]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(); } }

        // Events

        [UIAction(nameof(OnShowSelectLeftTracker))]
        private void OnShowSelectLeftTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(this.settings.LeftTracker);
        }

        [UIAction(nameof(OnShowSelectRightTracker))]
        private void OnShowSelectRightTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(this.settings.RightTracker);
        }

        [UIAction(nameof(OnClearLeftTracker))]
        private void OnClearLeftTracker()
        {
            this.settings.LeftTracker = new TrackerConfigData();
            this.configuration.SaveConfiguration();
            this.LeftTrackerSerial = TrackerConfigData.NoTrackerText;
            this.LeftTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        [UIAction(nameof(OnClearRightTracker))]
        private void OnClearRightTracker()
        {
            this.settings.RightTracker = new TrackerConfigData();
            this.configuration.SaveConfiguration();
            this.RightTrackerSerial = TrackerConfigData.NoTrackerText;
            this.RightTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the tracker text buttons
        /// </summary>
        private void SetTrackerText()
        {
            bool isLeftEmpty = String.IsNullOrWhiteSpace(this.settings.LeftTracker.Serial);
            bool isRightEmpty = String.IsNullOrWhiteSpace(this.settings.RightTracker.Serial);

            this.LeftTrackerSerial = isLeftEmpty ? TrackerConfigData.NoTrackerText : this.settings.LeftTracker.Serial;
            this.LeftTrackerHoverHint = isLeftEmpty ? TrackerConfigData.NoTrackerHoverHint : this.settings.LeftTracker.FullName;

            this.RightTrackerSerial = isRightEmpty ? TrackerConfigData.NoTrackerText : this.settings.RightTracker.Serial;
            this.RightTrackerHoverHint = isRightEmpty ? TrackerConfigData.NoTrackerHoverHint : this.settings.RightTracker.FullName;
        }

        #endregion

#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
        [Inject]
        private AlternativePlayMainFlowCoordinator mainFlowCoordinator;
#pragma warning restore CS0649
    }
}
