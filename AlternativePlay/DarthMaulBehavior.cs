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
            Pose? trackerPose;
            if (!String.IsNullOrWhiteSpace(config.LeftMaulTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftMaulTracker.Serial)) != null)
            {
                Utilities.TransformSaberFromTrackerData(saberManager.leftSaber.transform, config.LeftMaulTracker, trackerPose.Value);
            }

            // Check for right tracker
            if (!String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightMaulTracker.Serial)) != null)
            {
                Utilities.TransformSaberFromTrackerData(saberManager.rightSaber.transform, config.RightMaulTracker, trackerPose.Value);
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
                Utilities.TransformSaberFromTrackerData(baseSaber.transform, trackerConfigData, trackerPose.Value);
                Utilities.TransformSaberFromTrackerData(otherSaber.transform, trackerConfigData, trackerPose.Value);
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
            Pose? trackerPose;
            Pose leftHand;
            if (String.IsNullOrWhiteSpace(config.LeftMaulTracker?.Serial) ||
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftMaulTracker.Serial)) == null)
            {
                leftHand.position = saberManager.leftSaber.transform.position;
                leftHand.rotation = saberManager.leftSaber.transform.rotation;
            }
            else
            {
                Pose transformedPose = Utilities.CalculatePoseFromTrackerData(config.RightMaulTracker, trackerPose.Value);
                leftHand.position = transformedPose.position;
                leftHand.rotation = transformedPose.rotation;
            }

            // Determine Right Hand position
            Pose rightHand;
            if (String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial) ||
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightMaulTracker.Serial)) == null)
            {
                rightHand.position = saberManager.rightSaber.transform.position;
                rightHand.rotation = saberManager.rightSaber.transform.rotation;
            }
            else
            {
                Pose transformedPose = Utilities.CalculatePoseFromTrackerData(config.RightMaulTracker, trackerPose.Value);
                rightHand.position = transformedPose.position;
                rightHand.rotation = transformedPose.rotation;
            }

            // Determine final saber positions and rotations
            Vector3 middlePos = (rightHand.position + leftHand.position) * 0.5f;
            Vector3 forward = (rightHand.position - leftHand.position).normalized;
            Vector3 rightHandUp = rightHand.rotation * Vector3.up;

            Saber forwardSaber = config.ReverseMaulDirection ? saberManager.leftSaber : saberManager.rightSaber;
            Saber backwardSaber = config.ReverseMaulDirection ? saberManager.rightSaber : saberManager.leftSaber;

            forwardSaber.transform.position = middlePos + (forward * sep);
            backwardSaber.transform.position = middlePos + (-forward * sep);
            forwardSaber.transform.rotation = Quaternion.LookRotation(forward, rightHandUp);
            backwardSaber.transform.rotation = Quaternion.LookRotation(-forward, -rightHandUp);
        }
    }
}
