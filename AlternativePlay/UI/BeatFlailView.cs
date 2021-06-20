using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using System;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatFlailView : BSMLAutomaticViewController
    {
        private ModMainFlowCoordinator mainFlowCoordinator;

        [UIParams]
#pragma warning disable CS0649 // Field 'TrackerPoseView.parserParams' is never assigned to, and will always have its default value null
        private BSMLParserParams parserParams;
#pragma warning restore CS0649 // Field 'TrackerPoseView.parserParams' is never assigned to, and will always have its default value null

        public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            SetTrackerText();
        }

        [UIValue("UseLeftFlail")]
        private bool useLeftFlail = Configuration.instance.ConfigurationData.UseLeftFlail;
        [UIAction("OnUseLeftFlailChanged")]
        private void OnUseLeftFlailChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseLeftFlail = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("LeftFlailLength")]
        private int LeftFlailLength = Configuration.instance.ConfigurationData.LeftFlailLength;
        [UIAction("OnLeftFlailLengthChanged")]
        private void OnLeftFlailLengthChanged(int value)
        {
            Configuration.instance.ConfigurationData.LeftFlailLength = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("RightFlailLength")]
        private int RightFlailLength = Configuration.instance.ConfigurationData.RightFlailLength;
        [UIAction("OnRightFlailLengthChanged")]
        private void OnRightFlailLengthChanged(int value)
        {
            Configuration.instance.ConfigurationData.RightFlailLength = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("FlailGravity")]
        private float FlailGravity = Configuration.instance.ConfigurationData.FlailGravity;
        [UIAction("OnFlailGravityChanged")]
        private void OnFlailGravityChanged(float value)
        {
            Configuration.instance.ConfigurationData.FlailGravity = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("MoveNotesBack")]
        private int MoveNotesBack = Configuration.instance.ConfigurationData.MoveNotesBack;
        [UIAction("OnMoveNotesBackChanged")]
        private void OnMoveNotesBackChanged(int value)
        {
            Configuration.instance.ConfigurationData.MoveNotesBack = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIAction("OnResetGravity")]
        private void OnResetGravity()
        {
            Configuration.instance.ConfigurationData.FlailGravity = 3.5f;
            Configuration.instance.SaveConfiguration();
            this.parserParams.EmitEvent("RefreshFlailGravity");
        }

        [UIAction("LengthFormatter")]
        private string LengthFormatter(int value)
        {
            return $"{value} cm";
        }

        #region SelectTracker Modal Members

        // Text Displays for the Main View
        private string leftFlailTrackerSerial;
        [UIValue("LeftFlailTrackerSerial")]
        public string LeftFlailTrackerSerial { get => this.leftFlailTrackerSerial; set { this.leftFlailTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftFlailTrackerSerial)); } }

        private string leftFlailTrackerHoverHint;
        [UIValue("LeftFlailTrackerHoverHint")]
        public string LeftFlailTrackerHoverHint { get => this.leftFlailTrackerHoverHint; set { this.leftFlailTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftFlailTrackerHoverHint)); } }

        private string rightFlailTrackerSerial;
        [UIValue("RightFlailTrackerSerial")]
        public string RightFlailTrackerSerial { get => this.rightFlailTrackerSerial; set { this.rightFlailTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightFlailTrackerSerial)); } }

        private string rightFlailTrackerHoverHint;
        [UIValue("RightFlailTrackerHoverHint")]
        public string RightFlailTrackerHoverHint { get => this.rightFlailTrackerHoverHint; set { this.rightFlailTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightFlailTrackerHoverHint)); } }

        // Text Display for the Current Tracker in the Tracker Select Modal
        private string currentTrackerText;
        [UIValue("CurrentTrackerText")]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(nameof(this.CurrentTrackerText)); } }

        // Events
        [UIAction("OnShowSelectLeftTracker")]
        private void OnShowSelectLeftTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.LeftFlailTracker);
        }

        [UIAction("OnShowSelectRightTracker")]
        private void OnShowSelectRightTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.RightFlailTracker);
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftFlailTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.LeftFlailTrackerSerial = TrackerConfigData.NoTrackerText;
            this.LeftFlailTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightFlailTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.RightFlailTrackerSerial = TrackerConfigData.NoTrackerText;
            this.RightFlailTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftFlailTracker.Serial))
            {
                this.LeftFlailTrackerSerial = TrackerConfigData.NoTrackerText;
                this.LeftFlailTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.LeftFlailTrackerSerial = config.LeftFlailTracker.Serial;
                this.LeftFlailTrackerHoverHint = config.LeftFlailTracker.FullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightFlailTracker.Serial))
            {
                this.RightFlailTrackerSerial = TrackerConfigData.NoTrackerText;
                this.RightFlailTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.RightFlailTrackerSerial = config.RightFlailTracker.Serial;
                this.RightFlailTrackerHoverHint = config.RightFlailTracker.FullName;
            }
        }

        #endregion

    }
}
