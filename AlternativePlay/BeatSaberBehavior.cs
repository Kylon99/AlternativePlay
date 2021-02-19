using AlternativePlay.Models;
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
            Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftSaberTracker);
            saberManager.leftSaber.transform.position = leftSaberPose.position;
            saberManager.leftSaber.transform.rotation = leftSaberPose.rotation;
            if (config.ReverseLeftSaber)
            {
                saberManager.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }

            // Transform the right saber
            Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightSaberTracker);
            saberManager.rightSaber.transform.position = rightSaberPose.position;
            saberManager.rightSaber.transform.rotation = rightSaberPose.rotation;
            if (config.ReverseRightSaber)
            {
                saberManager.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }
        }
    }
}
