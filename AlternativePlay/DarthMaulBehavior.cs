using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        private SaberManager saberManager;

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
            this.saberManager = FindObjectOfType<SaberManager>();
            TrackedDeviceManager.instance.LoadTrackedDevices();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul || saberManager == null)
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
            if (!String.IsNullOrWhiteSpace(config.LeftMaulTracker?.Serial))
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftMaulTracker.Serial);
                if (trackerPose != null)
                {
                    // Set the left saber based on the tracker
                    saberManager.leftSaber.transform.position = trackerPose.Value.position;
                    saberManager.leftSaber.transform.rotation = trackerPose.Value.rotation;
                }
            }

            // Check for right tracker
            if (!String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial))
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightMaulTracker.Serial);
                if (trackerPose != null)
                {
                    // Set the left saber based on the tracker
                    saberManager.rightSaber.transform.position = trackerPose.Value.position;
                    saberManager.rightSaber.transform.rotation = trackerPose.Value.rotation;
                }
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

            var baseSaber = config.UseLeftController ? saberManager.leftSaber : saberManager.rightSaber;
            var otherSaber = config.UseLeftController ? saberManager.rightSaber : saberManager.leftSaber;
            var rotateSaber = config.ReverseMaulDirection ? baseSaber : otherSaber;

            string trackerSerial = config.UseLeftController ? config.LeftMaulTracker?.Serial : config.RightMaulTracker?.Serial;
            TrackerConfigData trackerData = config.UseLeftController ? config.LeftMaulTracker : config.RightMaulTracker;

            if (String.IsNullOrWhiteSpace(trackerSerial))
            {
                // Move saber using Beat Saber saber positions
                otherSaber.transform.localPosition = baseSaber.transform.localPosition;
                otherSaber.transform.localRotation = baseSaber.transform.localRotation;
            }
            else
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(trackerSerial);
                if (trackerPose == null)
                {
                    // Fall back to move saber using Beat Saber saber positions
                    otherSaber.transform.localPosition = baseSaber.transform.localPosition;
                    otherSaber.transform.localRotation = baseSaber.transform.localRotation;
                }

                // Move both sabers to tracker pose
                Utilities.TransformSaberFromTrackerData(baseSaber.transform, trackerData,
                    trackerPose.Value.rotation, trackerPose.Value.position);
                Utilities.TransformSaberFromTrackerData(otherSaber.transform, trackerData,
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
            if (String.IsNullOrWhiteSpace(config.LeftMaulTracker?.Serial))
            {
                leftHandPos = saberManager.leftSaber.transform.position;
            }
            else
            {
                Vector3? trackerPosition = TrackedDeviceManager.instance.GetPositionFromSerial(config.LeftMaulTracker.Serial);
                leftHandPos = trackerPosition == null
                    ? saberManager.leftSaber.transform.position // Fall back to the Beat Saber saber position
                    : trackerPosition.Value;
            }

            // Determine Right Hand position
            Vector3 rightHandPos;
            if (String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial))
            {
                rightHandPos = saberManager.rightSaber.transform.position;
            }
            else
            {
                Vector3? trackerPosition = TrackedDeviceManager.instance.GetPositionFromSerial(config.RightMaulTracker.Serial);
                rightHandPos = trackerPosition == null
                    ? rightHandPos = saberManager.rightSaber.transform.position // Fall back to the Beat Saber saber position
                    : trackerPosition.Value;
            }

            Vector3 middlePos = (rightHandPos + leftHandPos) * 0.5f;
            Vector3 forward = (rightHandPos - leftHandPos).normalized;

            var forwardSaber = config.ReverseMaulDirection ? saberManager.leftSaber : saberManager.rightSaber;
            var backwardSaber = config.ReverseMaulDirection ? saberManager.rightSaber : saberManager.leftSaber;

            forwardSaber.transform.position = middlePos + forward * sep;
            backwardSaber.transform.position = middlePos + -forward * sep;
            forwardSaber.transform.rotation = Quaternion.LookRotation(forward, backwardSaber.transform.up);
            backwardSaber.transform.rotation = Quaternion.LookRotation(-forward, -backwardSaber.transform.up);
        }
    }
}
