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
        private const float scaleDisplayScaling = 10.0f;

        [UIParams]
#pragma warning disable CS0649 // Field 'TrackerPoseView.parserParams' is never assigned to, and will always have its default value null
        private BSMLParserParams parserParams;
#pragma warning restore CS0649 // Field 'TrackerPoseView.parserParams' is never assigned to, and will always have its default value null

        private TrackerConfigData trackerData;
        private Vector3 originalPosition;
        private Vector3 originalEuler;
        private float originalScale;

        public void SetSelectingTracker(TrackerConfigData trackerData)
        {
            this.trackerData = trackerData;
            this.originalPosition = this.trackerData.Position;
            this.originalEuler = this.trackerData.EulerAngles;
            this.originalScale = this.trackerData.Scale;
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            RefreshAllValues();
        }

        [UIValue("PositionX")]
        private int PositionX
        {
            get => Convert.ToInt32(this.trackerData.Position.x * positionScaling);
            set
            {
                int incrementedValue = PositionIncrement(Convert.ToInt32(this.trackerData.Position.x * positionScaling), value);
                this.trackerData.Position = new Vector3(incrementedValue / positionScaling, this.trackerData.Position.y, this.trackerData.Position.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshPositionXEvent");
            }
        }

        [UIValue("PositionY")]
        private int PositionY
        {
            get => Convert.ToInt32(this.trackerData.Position.y * positionScaling);
            set
            {
                int incrementedValue = PositionIncrement(Convert.ToInt32(this.trackerData.Position.y * positionScaling), value);
                this.trackerData.Position = new Vector3(this.trackerData.Position.x, incrementedValue / positionScaling, this.trackerData.Position.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshPositionYEvent");
            }
        }

        [UIValue("PositionZ")]
        private int PositionZ
        {
            get => Convert.ToInt32(this.trackerData.Position.z * positionScaling);
            set
            {
                int incrementedValue = PositionIncrement(Convert.ToInt32(this.trackerData.Position.z * positionScaling), value);
                this.trackerData.Position = new Vector3(this.trackerData.Position.x, this.trackerData.Position.y, incrementedValue / positionScaling);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshPositionZEvent");
            }
        }

        [UIValue("RotationX")]
        private int RotationX
        {
            get => Convert.ToInt32(this.trackerData.EulerAngles.x * rotationScaling);
            set
            {
                int incrementedValue = RotationIncrement(Convert.ToInt32(this.trackerData.EulerAngles.x * rotationScaling), value);
                this.trackerData.EulerAngles = new Vector3(incrementedValue / rotationScaling, this.trackerData.EulerAngles.y, this.trackerData.EulerAngles.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshRotationXEvent");
            }
        }

        [UIValue("RotationY")]
        private int RotationY
        {
            get => Convert.ToInt32(this.trackerData.EulerAngles.y * rotationScaling);
            set
            {
                int incrementedValue = RotationIncrement(Convert.ToInt32(this.trackerData.EulerAngles.y * rotationScaling), value);
                this.trackerData.EulerAngles = new Vector3(this.trackerData.EulerAngles.x, incrementedValue / rotationScaling, this.trackerData.EulerAngles.z);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshRotationYEvent");
            }
        }

        [UIValue("RotationZ")]
        private int RotationZ
        {
            get => Convert.ToInt32(this.trackerData.EulerAngles.z * rotationScaling);
            set
            {
                int incrementedValue = RotationIncrement(Convert.ToInt32(this.trackerData.EulerAngles.z * rotationScaling), value);
                this.trackerData.EulerAngles = new Vector3(this.trackerData.EulerAngles.x, this.trackerData.EulerAngles.y, incrementedValue / rotationScaling);

                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshRotationZEvent");
            }
        }

        [UIValue("Scale")]
        private float Scale
        {
            get => Convert.ToInt32(this.trackerData.Scale * scaleDisplayScaling);
            set
            {
                this.trackerData.Scale = value / scaleDisplayScaling;
                Configuration.instance.SaveConfiguration();
                this.parserParams.EmitEvent("RefreshScaleEvent");
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

        [UIAction("ScaleFormatter")]
        private string ScaleFormatter(float value)
        {
            return string.Format("{0:0.0} x", value / scaleDisplayScaling);
        }


        [UIAction("OnReset")]
        private void OnSelected()
        {
            this.trackerData.Position = Vector3.zero;
            this.trackerData.EulerAngles = Vector3.zero;
            this.trackerData.Scale = 1.0f;
            Configuration.instance.SaveConfiguration();
            RefreshAllValues();
        }


        [UIAction("OnRevert")]
        private void OnRevert()
        {
            this.trackerData.Position = this.originalPosition;
            this.trackerData.EulerAngles = this.originalEuler;
            this.trackerData.Scale = this.originalScale;
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
            this.parserParams.EmitEvent("RefreshScaleEvent");
        }

        private int PositionIncrement(int currentValue, int value)
        {
            int result = currentValue;

            result = currentValue < value
                ? result + Convert.ToInt32(Configuration.instance.ConfigurationData.PositionIncrement * 10.0f)
                : result - Convert.ToInt32(Configuration.instance.ConfigurationData.PositionIncrement * 10.0f);

            result = Math.Min(result, Convert.ToInt32(ConfigurationData.PositionMax * 10.0f));  // Clamps to the MAX
            result = Math.Max(result, Convert.ToInt32(ConfigurationData.PositionMax) * -10);  // Clamps to the MIN
            return result;
        }

        private int RotationIncrement(int currentValue, int value)
        {
            int result = currentValue;
            result = currentValue < value
                ? result + Convert.ToInt32(Configuration.instance.ConfigurationData.RotationIncrement * rotationScaling)
                : result - Convert.ToInt32(Configuration.instance.ConfigurationData.RotationIncrement * rotationScaling);

            // Go over the 0 / 360 degree point
            if (result >= Convert.ToInt32(ConfigurationData.RotationMax * rotationScaling)) result = result - 3600;
            if (result < 0) result = result + 3600;

            return result;
        }
    }
}
