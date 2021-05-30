using AlternativePlay.Models;
using UnityEngine;

namespace AlternativePlay
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        public bool Split { get; private set; }
        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Darth Maul
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftMaulTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightMaulTracker);

            // Take control of both sabers
            BehaviorCatalog.instance.SaberDeviceManager.DisableLeftVRControl();
            BehaviorCatalog.instance.SaberDeviceManager.DisableRightVRControl();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul)
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
                    this.Split = !this.Split;
                }
            }

            this.TransformSabers();
        }

        /// <summary>
        /// Move the sabers that have been disconnected from the VRControllers ourselves
        /// </summary>
        private void TransformSabers()
        {
            if (this.Split)
            {
                this.TransformForSplitDarthMaul();
                return;
            }

            switch (Configuration.instance.ConfigurationData.DarthMaulControllerCount)
            {
                case ControllerCountEnum.One:
                    this.TransformOneControllerMaul();
                    break;

                case ControllerCountEnum.Two:
                    this.TransformTwoControllerMaul();
                    break;

                default:
                    // Do nothing
                    break;
            }
        }

        /// <summary>
        /// Tracks the sabers for when Darth Maul mode is split into two swords
        /// </summary>
        private void TransformForSplitDarthMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaber(config.LeftMaulTracker);
            BehaviorCatalog.instance.SaberDeviceManager.SetRightSaber(config.RightMaulTracker);
        }

        /// <summary>
        /// Moves the maul sabers based on a one controller scheme
        /// </summary>
        private void TransformOneControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;
            var saberDevice = BehaviorCatalog.instance.SaberDeviceManager;

            // Get the Pose of the base saber and calculate the rotated pose from it
            Pose basePose = config.UseLeftController
                ? saberDevice.GetLeftSaberPose(config.LeftMaulTracker)
                : saberDevice.GetRightSaberPose(config.RightMaulTracker);

            Pose rotatedPose = basePose.Reverse();
            Vector3 separation = new Vector3(0.0f, 0.0f, sep * 2.0f);
            rotatedPose.position += (rotatedPose.rotation * separation);

            // Determine which pose belongs to which saber and set them
            Pose leftSaberPose = config.ReverseMaulDirection ? basePose : rotatedPose;
            Pose rightSaberPose = config.ReverseMaulDirection ? rotatedPose : basePose;

            saberDevice.SetLeftSaberPose(leftSaberPose);
            saberDevice.SetRightSaberPose(rightSaberPose);
        }

        /// <summary>
        /// Moves the maul sabers based on a two controller scheme
        /// </summary>
        private void TransformTwoControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;

            var saberDevice = BehaviorCatalog.instance.SaberDeviceManager;

            // Determine Hand positions
            Pose leftHand = saberDevice.GetLeftSaberPose(config.LeftMaulTracker);
            Pose rightHand = saberDevice.GetRightSaberPose(config.RightMaulTracker);

            // Determine final saber positions and rotations
            Vector3 middlePos = (rightHand.position + leftHand.position) * 0.5f;
            Vector3 forward = (rightHand.position - leftHand.position).normalized;
            Vector3 rightHandUp = rightHand.rotation * Vector3.up;

            Pose forwardSaberPose = new Pose(middlePos + (forward * sep), Quaternion.LookRotation(forward, rightHandUp));
            Pose backwardSaberPose = new Pose(middlePos + (-forward * sep), Quaternion.LookRotation(-forward, -rightHandUp));

            Pose leftSaberPose = config.ReverseMaulDirection ? forwardSaberPose : backwardSaberPose;
            Pose rightSaberPose = config.ReverseMaulDirection ? backwardSaberPose : forwardSaberPose;

            saberDevice.SetLeftSaberPose(leftSaberPose);
            saberDevice.SetRightSaberPose(rightSaberPose);
        }
    }

}
