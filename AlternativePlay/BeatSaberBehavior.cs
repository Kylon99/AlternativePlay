using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
        private PlayerController playerController;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Currently nothing needs to be done on GameCore start
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSaber || playerController == null)
            {
                // Do nothing if we aren't playing Beat Saber
                return;
            }

            var config = Configuration.instance.ConfigurationData;

            // Check for left tracker
            if (!String.IsNullOrWhiteSpace(config.LeftSaberTracker))
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftSaberTracker);
                if (trackerPose != null)
                {
                    // Set the left saber based on the tracker
                    playerController.leftSaber.transform.position = trackerPose.Value.position;
                    playerController.leftSaber.transform.rotation = trackerPose.Value.rotation;
                }
            }

            // Check for right tracker
            if (!String.IsNullOrWhiteSpace(config.RightSaberTracker))
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightSaberTracker);
                if (trackerPose != null)
                {
                    // Set the left saber based on the tracker
                    playerController.rightSaber.transform.position = trackerPose.Value.position;
                    playerController.rightSaber.transform.rotation = trackerPose.Value.rotation;
                }
            }

            if (Configuration.instance.ConfigurationData.ReverseLeftSaber)
            {
                playerController.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }

            if (Configuration.instance.ConfigurationData.ReverseRightSaber)
            {
                playerController.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }
        }
    }
}
