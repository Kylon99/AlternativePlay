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
            if (Configuration.Current.PlayMode != PlayMode.DarthMaul) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.LeftTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.RightTracker);
        }

        private void Update()
        {
            if (Configuration.Current.PlayMode != PlayMode.DarthMaul)
            {
                // Do nothing if we aren't playing Darth Maul
                return;
            }

            if (Configuration.Current.UseTriggerToSeparate)
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

            switch (Configuration.Current.ControllerCount)
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
            BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaber(Configuration.Current.LeftTracker);
            BehaviorCatalog.instance.SaberDeviceManager.SetRightSaber(Configuration.Current.RightTracker);
        }

        /// <summary>
        /// Moves the maul sabers based on a one controller scheme
        /// </summary>
        private void TransformOneControllerMaul()
        {
            float sep = 1.0f * Configuration.Current.MaulDistance / 100.0f;
            var saberDevice = BehaviorCatalog.instance.SaberDeviceManager;

            // Get the Pose of the base saber and calculate the rotated pose from it
            Pose basePose = Configuration.Current.UseLeft
                ? saberDevice.GetLeftSaberPose(Configuration.Current.LeftTracker)
                : saberDevice.GetRightSaberPose(Configuration.Current.RightTracker);

            Pose rotatedPose = basePose.Reverse();
            Vector3 separation = new Vector3(0.0f, 0.0f, sep * 2.0f);
            rotatedPose.position += (rotatedPose.rotation * separation);

            // Determine which pose belongs to which saber and set them
            Pose leftSaberPose = Configuration.Current.ReverseMaulDirection ? basePose : rotatedPose;
            Pose rightSaberPose = Configuration.Current.ReverseMaulDirection ? rotatedPose : basePose;
            if (Configuration.Current.UseLeft)
            {
                leftSaberPose = Configuration.Current.ReverseMaulDirection ? rotatedPose : basePose;
                rightSaberPose = Configuration.Current.ReverseMaulDirection ? basePose : rotatedPose;
            }
            saberDevice.SetLeftSaberPose(leftSaberPose);
            saberDevice.SetRightSaberPose(rightSaberPose);
        }

        /// <summary>
        /// Moves the maul sabers based on a two controller scheme
        /// </summary>
        private void TransformTwoControllerMaul()
        {
            float sep = 1.0f * Configuration.Current.MaulDistance / 100.0f;

            var saberDevice = BehaviorCatalog.instance.SaberDeviceManager;

            // Determine Hand positions
            Pose leftHand = saberDevice.GetLeftSaberPose(Configuration.Current.LeftTracker);
            Pose rightHand = saberDevice.GetRightSaberPose(Configuration.Current.RightTracker);

            // Determine final saber positions and rotations
            Vector3 middlePos = (rightHand.position + leftHand.position) * 0.5f;
            Vector3 forward = (rightHand.position - leftHand.position).normalized;
            Vector3 rightHandUp = rightHand.rotation * Vector3.up;

            Pose forwardSaberPose = new Pose(middlePos + (forward * sep), Quaternion.LookRotation(forward, rightHandUp));
            Pose backwardSaberPose = new Pose(middlePos + (-forward * sep), Quaternion.LookRotation(-forward, -rightHandUp));

            Pose leftSaberPose = Configuration.Current.ReverseMaulDirection ? forwardSaberPose : backwardSaberPose;
            Pose rightSaberPose = Configuration.Current.ReverseMaulDirection ? backwardSaberPose : forwardSaberPose;

            saberDevice.SetLeftSaberPose(leftSaberPose);
            saberDevice.SetRightSaberPose(rightSaberPose);
        }
    }

}
