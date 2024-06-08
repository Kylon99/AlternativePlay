using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class TrackerPoseView : BSMLAutomaticViewController
    {
        private const float positionScaling = 1000.0f;
        private const float rotationScaling = 10.0f;

        private TrackerConfigData trackerConfigData;
        private Vector3 originalPosition;
        private Vector3 originalEuler;

        private PlayModeSettings settings;

        public void SetPlayModeSettings(PlayModeSettings settings)
        {
            this.settings = settings;
        }

        public void SetSelectingTracker(TrackerConfigData trackerConfigData)
        {
            this.trackerConfigData = trackerConfigData;
            this.originalPosition = this.trackerConfigData.Position;
            this.originalEuler = this.trackerConfigData.EulerAngles;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            this.UpdateAllValues();
        }

        [UIValue(nameof(PositionIncrementChoice))]
        private string PositionIncrementChoice
        {
            get => this.settings.PositionIncrement;
            set
            {
                this.settings.PositionIncrement = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(PositionIncrementList))]
        private List<object> PositionIncrementList => PlayModeSettings.PositionIncrementList.Cast<object>().ToList();

        [UIValue(nameof(RotationIncrementChoice))]
        private string RotationIncrementChoice
        {
            get => this.settings.RotationIncrement;
            set
            {
                this.settings.RotationIncrement = value;
                this.configuration.SaveConfiguration();
            }
        }

        [UIValue(nameof(RotationIncrementList))]
        private List<object> RotationIncrementList => PlayModeSettings.RotationIncrementList.Cast<object>().ToList();

        [UIValue(nameof(PositionX))]
        private int PositionX
        {
            get => Convert.ToInt32(this.trackerConfigData.Position.x * positionScaling);
            set
            {
                int incrementedValue = this.PositionIncrement(Convert.ToInt32(this.trackerConfigData.Position.x * positionScaling), value);
                this.trackerConfigData.Position = new Vector3(incrementedValue / positionScaling, this.trackerConfigData.Position.y, this.trackerConfigData.Position.z);

                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.PositionX));
            }
        }

        [UIValue(nameof(PositionY))]
        private int PositionY
        {
            get => Convert.ToInt32(this.trackerConfigData.Position.y * positionScaling);
            set
            {
                int incrementedValue = this.PositionIncrement(Convert.ToInt32(this.trackerConfigData.Position.y * positionScaling), value);
                this.trackerConfigData.Position = new Vector3(this.trackerConfigData.Position.x, incrementedValue / positionScaling, this.trackerConfigData.Position.z);

                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.PositionY));
            }
        }

        [UIValue(nameof(PositionZ))]
        private int PositionZ
        {
            get => Convert.ToInt32(this.trackerConfigData.Position.z * positionScaling);
            set
            {
                int incrementedValue = this.PositionIncrement(Convert.ToInt32(this.trackerConfigData.Position.z * positionScaling), value);
                this.trackerConfigData.Position = new Vector3(this.trackerConfigData.Position.x, this.trackerConfigData.Position.y, incrementedValue / positionScaling);

                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.PositionZ));
            }
        }

        [UIValue(nameof(RotationX))]
        private int RotationX
        {
            get => Convert.ToInt32(this.trackerConfigData.EulerAngles.x * rotationScaling);
            set
            {
                int incrementedValue = this.RotationIncrement(Convert.ToInt32(this.trackerConfigData.EulerAngles.x * rotationScaling), value);
                this.trackerConfigData.EulerAngles = new Vector3(incrementedValue / rotationScaling, this.trackerConfigData.EulerAngles.y, this.trackerConfigData.EulerAngles.z);

                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.RotationX));
            }
        }

        [UIValue(nameof(RotationY))]
        private int RotationY
        {
            get => Convert.ToInt32(this.trackerConfigData.EulerAngles.y * rotationScaling);
            set
            {
                int incrementedValue = this.RotationIncrement(Convert.ToInt32(this.trackerConfigData.EulerAngles.y * rotationScaling), value);
                this.trackerConfigData.EulerAngles = new Vector3(this.trackerConfigData.EulerAngles.x, incrementedValue / rotationScaling, this.trackerConfigData.EulerAngles.z);

                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.RotationY));
            }
        }

        [UIValue(nameof(RotationZ))]
        private int RotationZ
        {
            get => Convert.ToInt32(this.trackerConfigData.EulerAngles.z * rotationScaling);
            set
            {
                int incrementedValue = this.RotationIncrement(Convert.ToInt32(this.trackerConfigData.EulerAngles.z * rotationScaling), value);
                this.trackerConfigData.EulerAngles = new Vector3(this.trackerConfigData.EulerAngles.x, this.trackerConfigData.EulerAngles.y, incrementedValue / rotationScaling);

                this.configuration.SaveConfiguration();
                this.NotifyPropertyChanged(nameof(this.RotationZ));
            }
        }

        [UIAction(nameof(PositionFormatter))]
        private string PositionFormatter(float value)
        {
            return String.Format("{0:0.0} cm", value / 10.0f);
        }

        [UIAction(nameof(RotationFormatter))]
        private string RotationFormatter(float value)
        {
            return string.Format("{0:0.0} deg", value / rotationScaling);
        }


        [UIAction(nameof(OnReset))]
        private void OnReset()
        {
            this.trackerConfigData.Position = Vector3.zero;
            this.trackerConfigData.EulerAngles = Vector3.zero;
            this.configuration.SaveConfiguration();
            this.UpdateAllValues();
        }


        [UIAction(nameof(OnRevert))]
        private void OnRevert()
        {
            this.trackerConfigData.Position = this.originalPosition;
            this.trackerConfigData.EulerAngles = this.originalEuler;
            this.configuration.SaveConfiguration();
            this.UpdateAllValues();
        }

        private void UpdateAllValues()
        {
            this.NotifyPropertyChanged(nameof(this.PositionIncrementChoice));
            this.NotifyPropertyChanged(nameof(this.RotationIncrementChoice));
            this.NotifyPropertyChanged(nameof(this.PositionX));
            this.NotifyPropertyChanged(nameof(this.PositionY));
            this.NotifyPropertyChanged(nameof(this.PositionZ));
            this.NotifyPropertyChanged(nameof(this.RotationX));
            this.NotifyPropertyChanged(nameof(this.RotationY));
            this.NotifyPropertyChanged(nameof(this.RotationZ));
        }

        private int PositionIncrement(int currentValue, int value)
        {
            float positionIncrement = PlayModeSettings.GetIncrement(this.settings.PositionIncrement);

            int result = currentValue;
            result = currentValue < value
                ? result + Convert.ToInt32(positionIncrement * 10.0f)
                : result - Convert.ToInt32(positionIncrement * 10.0f);

            result = Math.Min(result, Convert.ToInt32(PlayModeSettings.PositionMax * 10.0f));  // Clamps to the MAX
            result = Math.Max(result, Convert.ToInt32(PlayModeSettings.PositionMax) * -10);  // Clamps to the MIN
            return result;
        }

        private int RotationIncrement(int currentValue, int value)
        {
            float rotationIncrement = PlayModeSettings.GetIncrement(this.settings.RotationIncrement);

            int result = currentValue;
            result = currentValue < value
                ? result + Convert.ToInt32(rotationIncrement * rotationScaling)
                : result - Convert.ToInt32(rotationIncrement * rotationScaling);

            // Go over the 0 / 360 degree point
            if (result >= Convert.ToInt32(PlayModeSettings.RotationMax * rotationScaling)) result = result - 3600;
            if (result < 0) result = result + 3600;

            return result;
        }

#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
#pragma warning restore CS0649
    }
}
