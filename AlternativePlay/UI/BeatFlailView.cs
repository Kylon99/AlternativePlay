using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatFlailView : BSMLAutomaticViewController
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

        [UIValue(nameof(FlailIcon))]
        public string FlailIcon => IconNames.BeatFlail;

        [UIValue(nameof(LeftFlailModeIcon))]
        public string LeftFlailModeIcon 
        {
            get
            {
                switch (this.settings.LeftFlailMode)
                {
                    default:
                    case BeatFlailMode.Flail:
                        return IconNames.LeftFlail;
                    case BeatFlailMode.Sword:
                        return IconNames.LeftSaber;
                    case BeatFlailMode.None:
                        return IconNames.Empty;
                }
            }
        }

        [UIValue(nameof(LeftFlailMode))]
        private string LeftFlailMode
        {
            get => this.settings.LeftFlailMode.ToString();
            set
            {
                this.settings.LeftFlailMode = (BeatFlailMode)Enum.Parse(typeof(BeatFlailMode), value);
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.LeftFlailModeIcon));
            }
        }

        [UIValue(nameof(LeftFlailModeList))]
        private List<object> LeftFlailModeList = new List<object> { BeatFlailMode.Flail.ToString(), BeatFlailMode.Sword.ToString(), BeatFlailMode.None.ToString() };

        [UIValue(nameof(RightFlailModeIcon))]
        public string RightFlailModeIcon
        {
            get
            {
                switch (this.settings.RightFlailMode)
                {
                    default:
                    case BeatFlailMode.Flail:
                        return IconNames.RightFlail;
                    case BeatFlailMode.Sword:
                        return IconNames.RightSaber;
                    case BeatFlailMode.None:
                        return IconNames.Empty;
                }
            }
        }

        [UIValue(nameof(RightFlailMode))]
        private string RightFlailMode
        {
            get => this.settings.RightFlailMode.ToString();
            set
            {
                this.settings.RightFlailMode = (BeatFlailMode)Enum.Parse(typeof(BeatFlailMode), value);
                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.RightFlailModeIcon));
            }
        }

        [UIValue(nameof(RightFlailModeList))]
        private List<object> RightFlailModeList = new List<object> { BeatFlailMode.Flail.ToString(), BeatFlailMode.Sword.ToString(), BeatFlailMode.None.ToString() };

        [UIValue(nameof(LeftFlailLength))]
        private int LeftFlailLength
        {
            get => this.settings.LeftFlailLength;
            set
            {
                this.settings.LeftFlailLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(RightFlailLength))]
        private int RightFlailLength
        {
            get => this.settings.RightFlailLength;
            set
            {
                this.settings.RightFlailLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(LeftHandleLength))]
        private int LeftHandleLength
        {
            get => this.settings.LeftHandleLength;
            set
            {
                this.settings.LeftHandleLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(RightHandleLength))]
        private int RightHandleLength
        {
            get => this.settings.RightHandleLength;
            set
            {
                this.settings.RightHandleLength = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(Gravity))]
        private float Gravity
        {
            get => this.settings.Gravity;
            set
            {
                this.settings.Gravity = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(MoveNotesBack))]
        private int MoveNotesBack
        {
            get => this.settings.MoveNotesBack;
            set
            {
                this.settings.MoveNotesBack = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIAction(nameof(OnResetGravity))]
        private void OnResetGravity()
        {
            this.settings.Gravity = 3.5f;
            this.configuration.SaveConfiguration();
            this.NotifyPropertyChanged(nameof(this.Gravity));
        }

        [UIAction(nameof(LengthFormatter))]
        private string LengthFormatter(int value)
        {
            return $"{value} cm";
        }

        private void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.LeftFlailMode));
            this.NotifyPropertyChanged(nameof(this.RightFlailMode));
            this.NotifyPropertyChanged(nameof(this.LeftFlailLength));
            this.NotifyPropertyChanged(nameof(this.RightFlailLength));
            this.NotifyPropertyChanged(nameof(this.Gravity));
            this.NotifyPropertyChanged(nameof(this.MoveNotesBack));
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