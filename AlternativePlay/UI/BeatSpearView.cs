using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatSpearView : BSMLAutomaticViewController
    {
        private ModMainFlowCoordinator mainFlowCoordinator;

        public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            SetTrackerText();
        }

        [UIValue("ControllerChoice")]
        private string controllerChoice = Configuration.instance.ConfigurationData.SpearControllerCount.ToString();
        [UIValue("ControllerChoiceList")]
        private List<object> controllerChoiceList = new List<object> { "One", "Two" };
        [UIAction("OnControllersChanged")]
        private void OnControllersChanged(string value)
        {
            Configuration.instance.ConfigurationData.SpearControllerCount = (ControllerCountEnum)Enum.Parse(typeof(ControllerCountEnum), value);
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseLeftSpear")]
        private bool useLeftSpear = Configuration.instance.ConfigurationData.UseLeftSpear;
        [UIAction("OnUseLeftSpearChanged")]
        private void OnUseLeftSpearChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseLeftSpear = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseTriggerToSwitchHands")]
        private bool useTriggerToSwitchHands = Configuration.instance.ConfigurationData.UseTriggerToSwitchHands;
        [UIAction("OnUseTriggerToSwitchHands")]
        private void OnUseTriggerToSwitchHands(bool value)
        {
            Configuration.instance.ConfigurationData.UseTriggerToSwitchHands = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("ReverseSpearDirection")]
        private bool reverseSpearDirection = Configuration.instance.ConfigurationData.ReverseSpearDirection;
        [UIAction("OnReverseSpearDirectionChanged")]
        private void OnReverseSpearDirectionChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseSpearDirection = value;
            Configuration.instance.SaveConfiguration();
        }

        #region SelectTracker Modal Members

        // Text Displays for the Main View
        private string leftSpearTrackerSerial;
        [UIValue("LeftSpearTrackerSerial")]
        public string LeftSpearTrackerSerial { get => this.leftSpearTrackerSerial; set { this.leftSpearTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftSpearTrackerSerial)); } }

        private string leftSpearTrackerHoverHint;
        [UIValue("LeftSpearTrackerHoverHint")]
        public string LeftSpearTrackerHoverHint { get => this.leftSpearTrackerHoverHint; set { this.leftSpearTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftSpearTrackerHoverHint)); } }

        private string rightSpearTrackerSerial;
        [UIValue("RightSpearTrackerSerial")]
        public string RightSpearTrackerSerial { get => this.rightSpearTrackerSerial; set { this.rightSpearTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightSpearTrackerSerial)); } }

        private string rightSpearTrackerHoverHint;
        [UIValue("RightSpearTrackerHoverHint")]
        public string RightSpearTrackerHoverHint { get => this.rightSpearTrackerHoverHint; set { this.rightSpearTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightSpearTrackerHoverHint)); } }

        // Text Display for the Current Tracker in the Tracker Select Modal
        private string currentTrackerText;
        [UIValue("CurrentTrackerText")]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(nameof(this.CurrentTrackerText)); } }

        // Events
        [UIAction("OnShowSelectLeftTracker")]
        private void OnShowSelectLeftTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.LeftSpearTracker);
        }

        [UIAction("OnShowSelectRightTracker")]
        private void OnShowSelectRightTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.RightSpearTracker);
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftSpearTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.LeftSpearTrackerSerial = TrackerConfigData.NoTrackerText;
            this.LeftSpearTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightSpearTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.RightSpearTrackerSerial = TrackerConfigData.NoTrackerText;
            this.RightSpearTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftSpearTracker.Serial))
            {
                this.LeftSpearTrackerSerial = TrackerConfigData.NoTrackerText;
                this.LeftSpearTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.LeftSpearTrackerSerial = config.LeftSpearTracker.Serial;
                this.LeftSpearTrackerHoverHint = config.LeftSpearTracker.FullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightSpearTracker.Serial))
            {
                this.RightSpearTrackerSerial = TrackerConfigData.NoTrackerText;
                this.RightSpearTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.RightSpearTrackerSerial = config.RightSpearTracker.Serial;
                this.RightSpearTrackerHoverHint = config.RightSpearTracker.FullName;
            }
        }

        #endregion

    }
}
