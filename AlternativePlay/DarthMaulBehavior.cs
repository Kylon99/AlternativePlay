using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        private PlayerController playerController;

        public static bool Split { get; set; }


        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Darth Maul
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul) { return; }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftMaulTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightMaulTracker);
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
            TrackedDeviceManager.instance.LoadTrackedDevices();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul || playerController == null)
            {
                // Do nothing if we aren't playing Darth Maul
                return;
            }

            if (Configuration.instance.ConfigurationData.UseTriggerToSeparate)
            {
                // Check to see if the trigger has been pressed
                bool leftTriggerPressed = BehaviorCatalog.instance.InputManager.GetLeftTriggerClicked();
                bool rightTriggerPressed = BehaviorCatalog.instance.InputManager.GetRightTriggerClicked();

                if (leftTriggerPressed || rightTriggerPressed)
                {
                    Split = !Split;
                }

                if (Split)
                {
                    TransformForSplitDarthMaul();
                    return;
                }
            }

            TransformForMaul();
        }

        /// <summary>
        /// Tracks the sabers for when Darth Maul mode is split into two swords
        /// </summary>
        private void TransformForSplitDarthMaul()
        {
            var config = Configuration.instance.ConfigurationData;

            // Check for left tracker
            Pose? trackerPose;
            if (!String.IsNullOrWhiteSpace(config.LeftMaulTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftMaulTracker.Serial)) != null)
            {
                // Set the left saber based on the tracker
                Utilities.TransformSaberFromTrackerData(playerController.leftSaber.transform, config.LeftMaulTracker,
                    trackerPose.Value.rotation, trackerPose.Value.position);
            }

            // Check for right tracker
            if (!String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightMaulTracker.Serial)) != null)
            {
                // Set the left saber based on the tracker
                Utilities.TransformSaberFromTrackerData(playerController.rightSaber.transform, config.RightMaulTracker,
                    trackerPose.Value.rotation, trackerPose.Value.position);
            }
        }

        /// <summary>
        /// Track the Maul sabers using internal Beat Saber locations or the assigned trackers
        /// </summary>
        private void TransformForMaul()
        {
            switch (Configuration.instance.ConfigurationData.DarthMaulControllerCount)
            {
                case ControllerCountEnum.One:
                    TransformOneControllerMaul();
                    break;

                case ControllerCountEnum.Two:
                    TransformTwoControllerMaul();
                    break;

                default:
                    // Do nothing
                    break;
            }
        }

        /// <summary>
        /// Moves the maul sabers based on a one controller scheme
        /// </summary>
        private void TransformOneControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;

            var baseSaber = config.UseLeftController ? playerController.leftSaber : playerController.rightSaber;
            var otherSaber = config.UseLeftController ? playerController.rightSaber : playerController.leftSaber;
            var rotateSaber = config.ReverseMaulDirection ? baseSaber : otherSaber;

            string trackerSerial = config.UseLeftController ? config.LeftMaulTracker?.Serial : config.RightMaulTracker?.Serial;
            TrackerConfigData trackerConfigData = config.UseLeftController ? config.LeftMaulTracker : config.RightMaulTracker;

            Pose? trackerPose;
            if (String.IsNullOrWhiteSpace(trackerSerial) ||
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(trackerSerial)) == null)
            {
                // Move saber using Beat Saber saber positions
                otherSaber.transform.localPosition = baseSaber.transform.localPosition;
                otherSaber.transform.localRotation = baseSaber.transform.localRotation;
            }
            else
            {
                // Move both sabers to tracker pose
                Utilities.TransformSaberFromTrackerData(baseSaber.transform, trackerConfigData,
                    trackerPose.Value.rotation, trackerPose.Value.position);
                Utilities.TransformSaberFromTrackerData(otherSaber.transform, trackerConfigData,
                    trackerPose.Value.rotation, trackerPose.Value.position);
            }

            rotateSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            rotateSaber.transform.Translate(0.0f, 0.0f, sep * 2.0f, Space.Self);
        }

        /// <summary>
        /// Moves the maul sabers based on a two controller scheme
        /// </summary>
        private void TransformTwoControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;

            // Determine Left Hand position
            Vector3 leftHandPos;
            Vector3? trackerPosition;
            if (String.IsNullOrWhiteSpace(config.LeftMaulTracker?.Serial) ||
                (trackerPosition = TrackedDeviceManager.instance.GetPositionFromSerial(config.LeftMaulTracker.Serial)) == null)
            {
                leftHandPos = playerController.leftSaber.transform.position;
            }
            else
            {
                leftHandPos = trackerPosition.Value;
            }

            // Determine Right Hand position
            Vector3 rightHandPos;
            if (String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial) ||
                (trackerPosition = TrackedDeviceManager.instance.GetPositionFromSerial(config.RightMaulTracker.Serial)) == null)
            {
                rightHandPos = playerController.rightSaber.transform.position;
            }
            else
            {
                rightHandPos = trackerPosition.Value;
            }

            // Determine final saber positions
            Vector3 middlePos = (rightHandPos + leftHandPos) * 0.5f;
            Vector3 forward = (rightHandPos - leftHandPos).normalized;

            var forwardSaber = config.ReverseMaulDirection ? playerController.leftSaber : playerController.rightSaber;
            var forwardExtraPosition = config.ReverseMaulDirection ? config.LeftMaulTracker.Position : config.RightMaulTracker.Position;
            var backwardSaber = config.ReverseMaulDirection ? playerController.rightSaber : playerController.leftSaber;
            var backwardExtraPosition = config.ReverseMaulDirection ? config.RightMaulTracker.Position : config.LeftMaulTracker.Position;

            forwardSaber.transform.position = middlePos + (forward * sep) + forwardExtraPosition;
            backwardSaber.transform.position = middlePos + (-forward * sep) + backwardExtraPosition;
            forwardSaber.transform.rotation = Quaternion.LookRotation(forward, backwardSaber.transform.up);
            backwardSaber.transform.rotation = Quaternion.LookRotation(-forward, -backwardSaber.transform.up);
        }
    }
}
