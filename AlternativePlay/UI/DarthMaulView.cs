using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;

namespace AlternativePlay.UI
{
    [HotReload]
    public class DarthMaulView : BSMLAutomaticViewController
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
        private string controllerChoice = Configuration.instance.ConfigurationData.DarthMaulControllerCount.ToString();
        [UIValue("ControllerChoiceList")]
        private List<object> controllerChoiceList = new List<object> { "One", "Two" };
        [UIAction("OnControllersChanged")]
        private void OnControllersChanged(string value)
        {
            Configuration.instance.ConfigurationData.DarthMaulControllerCount = (ControllerCountEnum)Enum.Parse(typeof(ControllerCountEnum), value);
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseLeftController")]
        private bool useLeftController = Configuration.instance.ConfigurationData.UseLeftController;
        [UIAction("OnUseLeftControllerChanged")]
        private void OnUseLeftControllerChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseLeftController = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("ReverseMaulDirection")]
        private bool reverseSaberDirection = Configuration.instance.ConfigurationData.ReverseMaulDirection;
        [UIAction("OnReverseMaulDirectionChanged")]
        private void OnReverseMaulDirectionChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseMaulDirection = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseTriggerToSeparate")]
        private bool useTriggerToSeparate = Configuration.instance.ConfigurationData.UseTriggerToSeparate;
        [UIAction("OnUseTriggerToSeparateChanged")]
        private void OnUseTriggerToSeparateChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseTriggerToSeparate = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("SeparationAmount")]
        private int separationAmount = Configuration.instance.ConfigurationData.MaulDistance;
        [UIAction("OnSeparationAmountChanged")]
        private void OnSeparationAmountChanged(int value)
        {
            Configuration.instance.ConfigurationData.MaulDistance = value;
            Configuration.instance.SaveConfiguration();
        }

        #region SelectTracker Modal Members

        // Text Displays for the Main View
        private string leftMaulTrackerSerial;
        [UIValue("LeftMaulTrackerSerial")]
        public string LeftMaulTrackerSerial { get => this.leftMaulTrackerSerial; set { this.leftMaulTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftMaulTrackerSerial)); } }

        private string leftMaulTrackerHoverHint;
        [UIValue("LeftMaulTrackerHoverHint")]
        public string LeftMaulTrackerHoverHint { get => this.leftMaulTrackerHoverHint; set { this.leftMaulTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftMaulTrackerHoverHint)); } }

        private string rightMaulTrackerSerial;
        [UIValue("RightMaulTrackerSerial")]
        public string RightMaulTrackerSerial { get => this.rightMaulTrackerSerial; set { this.rightMaulTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightMaulTrackerSerial)); } }

        private string rightMaulTrackerHoverHint;
        [UIValue("RightMaulTrackerHoverHint")]
        public string RightMaulTrackerHoverHint { get => this.rightMaulTrackerHoverHint; set { this.rightMaulTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightMaulTrackerHoverHint)); } }

        // Text Display for the Current Tracker in the Tracker Select Modal
        private string currentTrackerText;
        [UIValue("CurrentTrackerText")]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(nameof(this.CurrentTrackerText)); } }

        // Events
        [UIAction("OnShowSelectLeftTracker")]
        private void OnShowSelectLeftTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.LeftMaulTracker);
        }

        [UIAction("OnShowSelectRightTracker")]
        private void OnShowSelectRightTracker()
        {
            this.mainFlowCoordinator.ShowTrackerSelect(Configuration.instance.ConfigurationData.RightMaulTracker);
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftMaulTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.LeftMaulTrackerSerial = TrackerConfigData.NoTrackerText;
            this.LeftMaulTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightMaulTracker = new TrackerConfigData();
            Configuration.instance.SaveConfiguration();
            this.RightMaulTrackerSerial = TrackerConfigData.NoTrackerText;
            this.RightMaulTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftMaulTracker.Serial))
            {
                this.LeftMaulTrackerSerial = TrackerConfigData.NoTrackerText;
                this.LeftMaulTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.LeftMaulTrackerSerial = config.LeftMaulTracker.Serial;
                this.LeftMaulTrackerHoverHint = config.LeftMaulTracker.FullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightMaulTracker.Serial))
            {
                this.RightMaulTrackerSerial = TrackerConfigData.NoTrackerText;
                this.RightMaulTrackerHoverHint = TrackerConfigData.NoTrackerHoverHint;
            }
            else
            {
                this.RightMaulTrackerSerial = config.RightMaulTracker.Serial;
                this.RightMaulTrackerHoverHint = config.RightMaulTracker.FullName;
            }
        }

        #endregion

    }
}
