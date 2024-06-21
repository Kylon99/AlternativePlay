using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatSpearView : BSMLAutomaticViewController
    {
        private Configuration configuration;
        private AlternativePlayMainFlowCoordinator mainFlowCoordinator;

        private PlayModeSettings settings;

        public void Initialize(Configuration config, AlternativePlayMainFlowCoordinator flowCoordinator)
        {
            this.configuration = config;
            this.mainFlowCoordinator = flowCoordinator;
        }

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

        private void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.ControllerChoice));
            this.NotifyPropertyChanged(nameof(this.UseLeftSpear));
            this.NotifyPropertyChanged(nameof(this.UseTriggerToSwitchHands));
            this.NotifyPropertyChanged(nameof(this.ReverseSpearDirection));
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
    }
}
