using AlternativePlay.Models;
using System;
using System.Collections;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
        private readonly Pose hiddenPose = new Pose(new Vector3(0.0f, -1000.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Saber
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.BeatSaber) { return; }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftSaberTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightSaberTracker);

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Update()
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.BeatSaber) return;

            this.UpdateLeftSaber();
            this.UpdateRightSaber();
        }

        private void UpdateLeftSaber()
        {
            var config = Configuration.instance.ConfigurationData;
            var saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;

            if (config.OneColor && config.RemoveOtherSaber && !config.UseLeftSaber)
            {
                // Move the other saber away since there's a bug in the base game which makes it
                // able to cut bombs still
                saberDeviceManager.SetLeftSaberPose(hiddenPose);
                return;
            }

            // Move the left saber if we are reversing it or it was assigned a tracker
            if (config.ReverseLeftSaber || !String.IsNullOrWhiteSpace(config.LeftSaberTracker.Serial))
            {
                Pose leftSaberPose = saberDeviceManager.GetLeftSaberPose(config.LeftSaberTracker);
                saberDeviceManager.SetLeftSaberPose(config.ReverseLeftSaber ? leftSaberPose.Reverse() : leftSaberPose);
            }
        }

        private void UpdateRightSaber()
        {
            var config = Configuration.instance.ConfigurationData;
            var saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;

            if (config.OneColor && config.RemoveOtherSaber && config.UseLeftSaber)
            {
                // Move the other saber away since there's a bug in the base game which makes it
                // able to cut bombs still
                saberDeviceManager.SetRightSaberPose(hiddenPose);
                return;
            }

            // Move the right saber if we are reversing it or it was assigned a tracker
            if (config.ReverseRightSaber || !String.IsNullOrWhiteSpace(config.RightSaberTracker.Serial))
            {
                Pose rightSaberPose = saberDeviceManager.GetRightSaberPose(config.RightSaberTracker);
                saberDeviceManager.SetRightSaberPose(config.ReverseRightSaber ? rightSaberPose.Reverse() : rightSaberPose);
            }
        }

        /// <summary>
        /// Disables the rendering of the other saber
        /// </summary>
        private IEnumerator DisableOtherSaberMesh()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            var config = Configuration.instance.ConfigurationData;
            if (!(config.OneColor && config.RemoveOtherSaber)) { yield break; }

            if (config.UseLeftSaber)
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableRightSaberMesh();
            }
            else
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableLeftSaberMesh();
            }
        }
    }
}
