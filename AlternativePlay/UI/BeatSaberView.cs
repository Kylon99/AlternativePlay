using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatSaberView : BSMLAutomaticViewController
    {
        private ModMainFlowCoordinator mainFlowCoordinator;
        private PlayModeSettings settings;

        public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
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
                Configuration.instance.SaveConfiguration();
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
                Configuration.instance.SaveConfiguration();
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
                Configuration.instance.SaveConfiguration();
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
                Configuration.instance.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.UseLeftSaberIcon));
            }
        }

        private void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.ReverseLeftSaber));
            this.NotifyPropertyChanged(nameof(this.ReverseRightSaber));
            this.NotifyPropertyChanged(nameof(this.RemoveOtherSaber));
            this.NotifyPropertyChanged(nameof(this.UseLeftSaber));
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
            Configuration.instance.SaveConfiguration();
            this.LeftTrackerSerial = TrackerConfigData.NoTrackerText;
            this.LeftTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        [UIAction(nameof(OnClearRightTracker))]
        private void OnClearRightTracker()
        {
            this.settings.RightTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
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
