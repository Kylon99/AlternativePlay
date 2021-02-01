using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
        private SaberManager saberManager;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Saber
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSaber) { return; }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftSaberTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightSaberTracker);
        }

        private void Awake()
        {
            this.saberManager = FindObjectOfType<SaberManager>();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSaber || saberManager == null)
            {
                // Do nothing if we aren't playing Beat Saber
                return;
            }

            var config = Configuration.instance.ConfigurationData;

            // Transform the left saber
            Pose? trackerPose;
            if (!String.IsNullOrWhiteSpace(config.LeftSaberTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftSaberTracker.Serial)) != null)
            {
                // Transform according to the assigned tracker
                Utilities.TransformSaberFromTrackerData(saberManager.leftSaber.transform, config.LeftSaberTracker, trackerPose.Value);
                if (config.ReverseLeftSaber)
                {
                    saberManager.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }
            else
            {
                if (config.ReverseLeftSaber)
                {
                    // Transform the saber only if we need to also reverse its direction since we need to set the saber position first
                    Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetLeftSaberPose();
                    saberManager.leftSaber.transform.position = saberPose.position;
                    saberManager.leftSaber.transform.rotation = saberPose.rotation;
                    saberManager.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }

            // Transform the right saber
            if (!String.IsNullOrWhiteSpace(config.RightSaberTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightSaberTracker.Serial)) != null)
            {
                // Transform according to the assigned tracker
                Utilities.TransformSaberFromTrackerData(saberManager.rightSaber.transform, config.RightSaberTracker, trackerPose.Value);
                if (config.ReverseRightSaber)
                {
                    saberManager.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }
            else
            {
                if (config.ReverseRightSaber)
                {
                    // Transform the saber only if we need to also reverse its direction since we need to set the saber position first
                    Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetRightSaberPose();
                    saberManager.rightSaber.transform.position = saberPose.position;
                    saberManager.rightSaber.transform.rotation = saberPose.rotation;
                    saberManager.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }
        }
    }
}
