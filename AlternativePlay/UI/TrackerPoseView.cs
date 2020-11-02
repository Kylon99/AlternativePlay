using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using UnityEngine;

namespace AlternativePlay.UI
{
    [HotReload]
    public class TrackerPoseView : BSMLAutomaticViewController
    {
        private const float positionScaling = 1000.0f;
        private const float rotationScaling = 10.0f;

        [UIParams]
#pragma warning disable CS0649 // Field 'TrackerPoseView.parserParams' is never assigned to, and will always have its default value null
        private BSMLParserParams parserParams;
#pragma warning restore CS0649 // Field 'TrackerPoseView.parserParams' is never assigned to, and will always have its default value null

        private TrackerConfigData trackerConfigData;
        private Vector3 originalPosition;
        private Vector3 originalEuler;

        public void SetSelectingTracker(TrackerConfigData trackerConfigData)
        {
            this.trackerConfigData = trackerConfigData;
            this.originalPosition = this.trackerConfigData.Position;
            this.originalEuler = this.trackerConfigData.EulerAngles;
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            RefreshAllValues();
        }

        [UIValue("PositionX")]
        private int PositionX
        {
            get => Convert.ToInt32(this.trackerConfigData.Position.x * positionScaling);
            set
            {
                int incrementedValue = PositionIncrement(Convert.ToInt32(this.trackerConfigData.Position.x * positionScaling), value);
                this.trackerConfigData.Position = new Vector3(incrementedValue / positionScaling, this.trackerConfigData.Position.y, this.trackerConfigData.Position.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshPositionXEvent");
            }
        }

        [UIValue("PositionY")]
        private int PositionY
        {
            get => Convert.ToInt32(this.trackerConfigData.Position.y * positionScaling);
            set
            {
                int incrementedValue = PositionIncrement(Convert.ToInt32(this.trackerConfigData.Position.y * positionScaling), value);
                this.trackerConfigData.Position = new Vector3(this.trackerConfigData.Position.x, incrementedValue / positionScaling, this.trackerConfigData.Position.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshPositionYEvent");
            }
        }

        [UIValue("PositionZ")]
        private int PositionZ
        {
            get => Convert.ToInt32(this.trackerConfigData.Position.z * positionScaling);
            set
            {
                int incrementedValue = PositionIncrement(Convert.ToInt32(this.trackerConfigData.Position.z * positionScaling), value);
                this.trackerConfigData.Position = new Vector3(this.trackerConfigData.Position.x, this.trackerConfigData.Position.y, incrementedValue / positionScaling);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshPositionZEvent");
            }
        }

        [UIValue("RotationX")]
        private int RotationX
        {
            get => Convert.ToInt32(this.trackerConfigData.EulerAngles.x * rotationScaling);
            set
            {
                int incrementedValue = RotationIncrement(Convert.ToInt32(this.trackerConfigData.EulerAngles.x * rotationScaling), value);
                this.trackerConfigData.EulerAngles = new Vector3(incrementedValue / rotationScaling, this.trackerConfigData.EulerAngles.y, this.trackerConfigData.EulerAngles.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshRotationXEvent");
            }
        }

        [UIValue("RotationY")]
        private int RotationY
        {
            get => Convert.ToInt32(this.trackerConfigData.EulerAngles.y * rotationScaling);
            set
            {
                int incrementedValue = RotationIncrement(Convert.ToInt32(this.trackerConfigData.EulerAngles.y * rotationScaling), value);
                this.trackerConfigData.EulerAngles = new Vector3(this.trackerConfigData.EulerAngles.x, incrementedValue / rotationScaling, this.trackerConfigData.EulerAngles.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshRotationYEvent");
            }
        }

        [UIValue("RotationZ")]
        private int RotationZ
        {
            get => Convert.ToInt32(this.trackerConfigData.EulerAngles.z * rotationScaling);
            set
            {
                int incrementedValue = RotationIncrement(Convert.ToInt32(this.trackerConfigData.EulerAngles.z * rotationScaling), value);
                this.trackerConfigData.EulerAngles = new Vector3(this.trackerConfigData.EulerAngles.x, this.trackerConfigData.EulerAngles.y, incrementedValue / rotationScaling);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshRotationZEvent");
            }
        }

        [UIAction("PositionFormatter")]
        private string PositionFormatter(float value)
        {
            return String.Format("{0:0.0} cm", value / 10.0f);
        }

        [UIAction("RotationFormatter")]
        private string RotationFormatter(float value)
        {
            return string.Format("{0:0.0} deg", value / rotationScaling);
        }


        [UIAction("OnReset")]
        private void OnSelected()
        {
            this.trackerConfigData.Position = Vector3.zero;
            this.trackerConfigData.EulerAngles = Vector3.zero;
            Configuration.instance.SaveConfiguration();
            RefreshAllValues();
        }


        [UIAction("OnRevert")]
        private void OnRevert()
        {
            this.trackerConfigData.Position = this.originalPosition;
            this.trackerConfigData.EulerAngles = this.originalEuler;
            Configuration.instance.SaveConfiguration();
            RefreshAllValues();
        }

        private void RefreshAllValues()
        {
            this.parserParams.EmitEvent("RefreshPositionXEvent");
            this.parserParams.EmitEvent("RefreshPositionYEvent");
            this.parserParams.EmitEvent("RefreshPositionZEvent");
            this.parserParams.EmitEvent("RefreshRotationXEvent");
            this.parserParams.EmitEvent("RefreshRotationYEvent");
            this.parserParams.EmitEvent("RefreshRotationZEvent");
        }

        private int PositionIncrement(int currentValue, int value)
        {
            float positionIncrement = ConfigurationData.GetIncrement(Configuration.instance.ConfigurationData.PositionIncrement);

            int result = currentValue;
            result = currentValue < value
                ? result + Convert.ToInt32(positionIncrement * 10.0f)
                : result - Convert.ToInt32(positionIncrement * 10.0f);

            result = Math.Min(result, Convert.ToInt32(ConfigurationData.PositionMax * 10.0f));  // Clamps to the MAX
            result = Math.Max(result, Convert.ToInt32(ConfigurationData.PositionMax) * -10);  // Clamps to the MIN
            return result;
        }

        private int RotationIncrement(int currentValue, int value)
        {
            float rotationIncrement = ConfigurationData.GetIncrement(Configuration.instance.ConfigurationData.RotationIncrement);

            int result = currentValue;
            result = currentValue < value
                ? result + Convert.ToInt32(rotationIncrement * rotationScaling)
                : result - Convert.ToInt32(rotationIncrement * rotationScaling);

            // Go over the 0 / 360 degree point
            if (result >= Convert.ToInt32(ConfigurationData.RotationMax * rotationScaling)) result = result - 3600;
            if (result < 0) result = result + 3600;

            return result;
        }
    }
}
