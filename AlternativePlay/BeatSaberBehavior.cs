using AlternativePlay.Models;
using System;
using UnityEngine;
using AlternativePlay.HarmonyPatches;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
        private SaberManager saberManager;
        private bool needTransform;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Saber
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSaber) { return; }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftSaberTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightSaberTracker);

            this.needTransform = true;
        }

        private void Awake()
        {
            if (MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager)
                this.saberManager = MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager;
            else
                this.saberManager = FindObjectOfType<SaberManager>();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSaber || !this.needTransform)
            {
                return;
            }

            var config = Configuration.instance.ConfigurationData;

            if (config.ReverseLeftSaber || !String.IsNullOrWhiteSpace(config.LeftSaberTracker.Serial))
            {
                // Transform the left saber
                Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftSaberTracker);
                this.saberManager.leftSaber.transform.position = leftSaberPose.position;
                this.saberManager.leftSaber.transform.rotation = leftSaberPose.rotation;
                this.saberManager.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }

            if (config.ReverseRightSaber || !String.IsNullOrWhiteSpace(config.RightSaberTracker.Serial))
            {
                // Transform the right saber
                Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightSaberTracker);
                this.saberManager.rightSaber.transform.position = rightSaberPose.position;
                this.saberManager.rightSaber.transform.rotation = rightSaberPose.rotation;
                this.saberManager.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }

            this.needTransform = false;
        }
    }
}
