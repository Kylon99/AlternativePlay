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
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul || saberManager == null) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftMaulTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightMaulTracker);
        }

        private void Awake()
        {
            this.saberManager = FindObjectOfType<SaberManager>();
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

            // Move the left saber
            Pose? trackerPose;
            if (String.IsNullOrWhiteSpace(config.LeftMaulTracker?.Serial) ||
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftMaulTracker.Serial)) == null)
            {
                // Transform using the left controller
                Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetLeftSaberPose();
                saberManager.leftSaber.transform.position = saberPose.position;
                saberManager.leftSaber.transform.rotation = saberPose.rotation;
            }
            else
            {
                // Transform using the left tracker
                Utilities.TransformSaberFromTrackerData(saberManager.leftSaber.transform, config.LeftMaulTracker, trackerPose.Value);
            }

            // Move the right saber
            if (String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial) ||
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightMaulTracker.Serial)) == null)
            {
                // Transform using the right controller
                Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetRightSaberPose();
                saberManager.rightSaber.transform.position = saberPose.position;
                saberManager.rightSaber.transform.rotation = saberPose.rotation;
            }
            else
            {
                // Transform using the right tracker
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

            // Determine which is the held normally 'base' saber and which is the 'other' saber that must be moved
            Func<Pose> getBaseController;
            Saber baseSaber;
            Saber otherSaber;

            if (config.UseLeftController)
            {
                getBaseController = BehaviorCatalog.instance.ControllerManager.GetLeftSaberPose;
                baseSaber = saberManager.leftSaber;
                otherSaber = saberManager.rightSaber;
            }
            else
            {
                getBaseController = BehaviorCatalog.instance.ControllerManager.GetRightSaberPose;
                baseSaber = saberManager.rightSaber;
                otherSaber = saberManager.leftSaber;
            }
            var rotateSaber = config.ReverseMaulDirection ? baseSaber : otherSaber;

            // Move the other saber to the base saber
            string trackerSerial = config.UseLeftController ? config.LeftMaulTracker?.Serial : config.RightMaulTracker?.Serial;
            Pose? trackerPose;
            if (String.IsNullOrWhiteSpace(trackerSerial) ||
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(trackerSerial)) == null)
            {
                // Use controller positions
                Pose saberPose = getBaseController();
                baseSaber.transform.position = saberPose.position;
                baseSaber.transform.rotation = saberPose.rotation;
                otherSaber.transform.position = saberPose.position;
                otherSaber.transform.rotation = saberPose.rotation;
            }
            else
            {
                // Move both sabers to tracker pose
                TrackerConfigData trackerConfigData = config.UseLeftController ? config.LeftMaulTracker : config.RightMaulTracker;
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
                // Track saber to the left controller
                Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetLeftSaberPose();
                leftHand.position = saberPose.position;
                leftHand.rotation = saberPose.rotation;
            }
            else
            {
                // Track saber to the left tracker
                Pose transformedPose = Utilities.CalculatePoseFromTrackerData(config.RightMaulTracker, trackerPose.Value);
                leftHand.position = transformedPose.position;
                leftHand.rotation = transformedPose.rotation;
            }

            // Determine Right Hand position
            Pose rightHand;
            if (String.IsNullOrWhiteSpace(config.RightMaulTracker?.Serial) ||
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightMaulTracker.Serial)) == null)
            {
                // Track saber to the right controller
                Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetRightSaberPose();
                rightHand.position = saberPose.position;
                rightHand.rotation = saberPose.rotation;
            }
            else
            {
                // Track saber to the right tracker
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
