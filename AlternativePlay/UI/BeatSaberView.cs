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

        public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            SetTrackerText();
        }

        [UIValue("ReverseLeftSaber")]
        private bool reverseLeftSaber = Configuration.instance.ConfigurationData.ReverseLeftSaber;
        [UIAction("OnReverseLeftSaberChanged")]
        private void OnReverseLeftSaberChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseLeftSaber = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("ReverseRightSaber")]
        private bool reverseRightSaber = Configuration.instance.ConfigurationData.ReverseRightSaber;
        [UIAction("OnReverseRightSaberChanged")]
        private void OnReverseRightSaberChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseRightSaber = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("RemoveOtherSaber")]
        private bool removeOtherSaber = Configuration.instance.ConfigurationData.RemoveOtherSaber;
        [UIAction("OnRemoveOtherSaberChanged")]
        private void OnRemoveOtherSaberChanged(bool value)
        {
            Configuration.instance.ConfigurationData.RemoveOtherSaber = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseLeftSaber")]
        private bool useLeftSaber = Configuration.instance.ConfigurationData.UseLeftSaber;
        [UIAction("OnUseLeftSaberChanged")]
        private void OnUseLeftSaberChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseLeftSaber = value;
            Configuration.instance.SaveConfiguration();
        }

        #region SelectTracker Modal Members

        // Text Displays for the Main View
        private string leftSaberTrackerSerial;
        [UIValue("LeftSaberTrackerSerial")]
        public string LeftSaberTrackerSerial { get => this.leftSaberTrackerSerial; set { this.leftSaberTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftSaberTrackerSerial)); } }

        private string leftSaberTrackerHoverHint;
        [UIValue("LeftSaberTrackerHoverHint")]
        public string LeftSaberTrackerHoverHint { get => this.leftSaberTrackerHoverHint; set { this.leftSaberTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftSaberTrackerHoverHint)); } }

        private string rightSaberTrackerSerial;
        [UIValue("RightSaberTrackerSerial")]
        public string RightSaberTrackerSerial { get => this.rightSaberTrackerSerial; set { this.rightSaberTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightSaberTrackerSerial)); } }

        private string rightSaberTrackerHoverHint;
        [UIValue("RightSaberTrackerHoverHint")]
        public string RightSaberTrackerHoverHint { get => this.rightSaberTrackerHoverHint; set { this.rightSaberTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightSaberTrackerHoverHint)); } }

        // Text Display for the Current Tracker in the Tracker Select Modal
        private string currentTrackerText;
        [UIValue("CurrentTrackerText")]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(nameof(this.CurrentTrackerText)); } }

        // Events

        [UIAction("OnShowSelectLeftTracker")]
        private void OnShowSelectLeftTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.LeftSaberTracker);
        }

        [UIAction("OnShowSelectRightTracker")]
        private void OnShowSelectRightTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.RightSaberTracker);
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftSaberTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.LeftSaberTrackerSerial = TrackerConfigData.NoTrackerText;
            this.LeftSaberTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightSaberTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.RightSaberTrackerSerial = TrackerConfigData.NoTrackerText;
            this.RightSaberTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftSaberTracker.Serial))
            {
                this.LeftSaberTrackerSerial = TrackerConfigData.NoTrackerText;
                this.LeftSaberTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.LeftSaberTrackerSerial = config.LeftSaberTracker.Serial;
                this.LeftSaberTrackerHoverHint = config.LeftSaberTracker.FullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightSaberTracker.Serial))
            {
                this.RightSaberTrackerSerial = TrackerConfigData.NoTrackerText;
                this.RightSaberTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.RightSaberTrackerSerial = config.RightSaberTracker.Serial;
                this.RightSaberTrackerHoverHint = config.RightSaberTracker.FullName;
            }
        }

        #endregion

    }
}
