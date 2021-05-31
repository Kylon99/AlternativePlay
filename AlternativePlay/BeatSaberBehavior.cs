using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
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
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSaber) return;

            var config = Configuration.instance.ConfigurationData;
            var saberDevice = BehaviorCatalog.instance.SaberDeviceManager;

            // Move the left saber if we are reversing it or it was assigned a tracker
            if (config.ReverseLeftSaber || !String.IsNullOrWhiteSpace(config.LeftSaberTracker.Serial))
            {
                Pose leftSaberPose = saberDevice.GetLeftSaberPose(config.LeftSaberTracker);
                saberDevice.SetLeftSaberPose(config.ReverseLeftSaber ? leftSaberPose.Reverse() : leftSaberPose);
            }

            // Move the right saber if we are reversing it or it was assigned a tracker
            if (config.ReverseRightSaber || !String.IsNullOrWhiteSpace(config.RightSaberTracker.Serial))
            {
                Pose rightSaberPose = saberDevice.GetRightSaberPose(config.RightSaberTracker);
                saberDevice.SetRightSaberPose(config.ReverseRightSaber ? rightSaberPose.Reverse() : rightSaberPose);
            }
        }
    }
}
