using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using System;

namespace AlternativePlay.UI
{
    [HotReload]
    public class NunchakuView : BSMLAutomaticViewController
    {
        private ModMainFlowCoordinator mainFlowCoordinator;

        [UIParams]
#pragma warning disable CS0649 // Field 'parserParams' is never assigned to, and will always have its default value null
        private BSMLParserParams parserParams;
#pragma warning restore CS0649 // Field 'parserParams' is never assigned to, and will always have its default value null

        public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            SetTrackerText();
        }

        [UIValue("ReverseNunchaku")]
        private bool reverseNunchaku = Configuration.instance.ConfigurationData.ReverseNunchaku;
        [UIAction("ReverseNunchakuChanged")]
        private void OnNoArrowsRandomChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseNunchaku = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("NunchakuLength")]
        private int nunchakuLength = Configuration.instance.ConfigurationData.NunchakuLength;
        [UIAction("OnNunchakuLengthChanged")]
        private void OnNunchakuLengthChanged(int value)
        {
            Configuration.instance.ConfigurationData.NunchakuLength = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("NunchakuGravity")]
        private float nunchakuGravity = Configuration.instance.ConfigurationData.NunchakuGravity;
        [UIAction("OnNunchakuGravityChanged")]
        private void OnNunchakuGravityChanged(float value)
        {
            Configuration.instance.ConfigurationData.NunchakuGravity = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIAction("OnResetGravity")]
        private void OnResetGravity()
        {
            nunchakuGravity = 3.5f;
            Configuration.instance.ConfigurationData.NunchakuGravity = 3.5f;
            Configuration.instance.SaveConfiguration();
            this.parserParams.EmitEvent("RefreshNunchakuGravity");
        }

        [UIAction("LengthFormatter")]
        private string LengthFormatter(int value)
        {
            return $"{value} cm";
        }

        #region SelectTracker Modal Members

        // Text Displays for the Main View
        private string leftNunchakuTrackerSerial;
        [UIValue("LeftNunchakuTrackerSerial")]
        public string LeftNunchakuTrackerSerial { get => this.leftNunchakuTrackerSerial; set { this.leftNunchakuTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftNunchakuTrackerSerial)); } }

        private string leftNunchakuTrackerHoverHint;
        [UIValue("LeftNunchakuTrackerHoverHint")]
        public string LeftNunchakuTrackerHoverHint { get => this.leftNunchakuTrackerHoverHint; set { this.leftNunchakuTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftNunchakuTrackerHoverHint)); } }

        private string rightNunchakuTrackerSerial;
        [UIValue("RightNunchakuTrackerSerial")]
        public string RightNunchakuTrackerSerial { get => this.rightNunchakuTrackerSerial; set { this.rightNunchakuTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightNunchakuTrackerSerial)); } }

        private string rightNunchakuTrackerHoverHint;
        [UIValue("RightNunchakuTrackerHoverHint")]
        public string RightNunchakuTrackerHoverHint { get => this.rightNunchakuTrackerHoverHint; set { this.rightNunchakuTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightNunchakuTrackerHoverHint)); } }

        // Text Display for the Current Tracker in the Tracker Select Modal
        private string currentTrackerText;
        [UIValue("CurrentTrackerText")]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(nameof(this.CurrentTrackerText)); } }

        // Events
        [UIAction("OnShowSelectLeftTracker")]
        private void OnShowSelectLeftTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.LeftNunchakuTracker);
        }

        [UIAction("OnShowSelectRightTracker")]
        private void OnShowSelectRightTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.RightNunchakuTracker);
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftNunchakuTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.LeftNunchakuTrackerSerial = TrackerConfigData.NoTrackerText;
            this.LeftNunchakuTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightNunchakuTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.RightNunchakuTrackerSerial = TrackerConfigData.NoTrackerText;
            this.RightNunchakuTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftNunchakuTracker.Serial))
            {
                this.LeftNunchakuTrackerSerial = TrackerConfigData.NoTrackerText;
                this.LeftNunchakuTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.LeftNunchakuTrackerSerial = config.LeftNunchakuTracker.Serial;
                this.LeftNunchakuTrackerHoverHint = config.LeftNunchakuTracker.FullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightNunchakuTracker.Serial))
            {
                this.RightNunchakuTrackerSerial = TrackerConfigData.NoTrackerText;
                this.RightNunchakuTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.RightNunchakuTrackerSerial = config.RightNunchakuTracker.Serial;
                this.RightNunchakuTrackerHoverHint = config.RightNunchakuTracker.FullName;
            }
        }

        #endregion

    }
}