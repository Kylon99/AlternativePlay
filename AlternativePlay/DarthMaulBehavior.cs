using AlternativePlay.Models;
using UnityEngine;
using Zenject;

namespace AlternativePlay
{
    public class DarthMaulBehavior : MonoBehaviour
    {
#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
        [Inject]
        private SaberDeviceManager saberDeviceManager;
        [Inject]
        private InputManager inputManager;
#pragma warning restore CS0649

        public bool Split { get; private set; }

        private void Start()
        {
            // Do nothing if we aren't playing Darth Maul
            if (this.configuration.Current.PlayMode != PlayMode.DarthMaul) { return; }

            Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.LeftTracker);
            Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.RightTracker);
        }

        private void Update()
        {
            if (this.configuration.Current.PlayMode != PlayMode.DarthMaul)
            {
                // Do nothing if we aren't playing Darth Maul
                return;
            }

            if (this.configuration.Current.UseTriggerToSeparate)
            {
                // Check to see if the trigger has been pressed
                bool leftTriggerPressed = this.inputManager.GetLeftTriggerClicked();
                bool rightTriggerPressed = this.inputManager.GetRightTriggerClicked();

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

            switch (this.configuration.Current.ControllerCount)
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
            this.saberDeviceManager.SetLeftSaber(this.configuration.Current.LeftTracker);
            this.saberDeviceManager.SetRightSaber(this.configuration.Current.RightTracker);
        }

        /// <summary>
        /// Moves the maul sabers based on a one controller scheme
        /// </summary>
        private void TransformOneControllerMaul()
        {
            float sep = 1.0f * this.configuration.Current.MaulDistance / 100.0f;

            // Get the Pose of the base saber and calculate the rotated pose from it
            Pose basePose = this.configuration.Current.UseLeft
                ? this.saberDeviceManager.GetLeftSaberPose(this.configuration.Current.LeftTracker)
                : this.saberDeviceManager.GetRightSaberPose(this.configuration.Current.RightTracker);

            Pose rotatedPose = basePose.Reverse();
            Vector3 separation = new Vector3(0.0f, 0.0f, sep * 2.0f);
            rotatedPose.position += (rotatedPose.rotation * separation);

            // Determine which pose belongs to which saber and set them
            Pose leftSaberPose = this.configuration.Current.ReverseMaulDirection ? basePose : rotatedPose;
            Pose rightSaberPose = this.configuration.Current.ReverseMaulDirection ? rotatedPose : basePose;
            if (this.configuration.Current.UseLeft)
            {
                leftSaberPose = this.configuration.Current.ReverseMaulDirection ? rotatedPose : basePose;
                rightSaberPose = this.configuration.Current.ReverseMaulDirection ? basePose : rotatedPose;
            }
            this.saberDeviceManager.SetLeftSaberPose(leftSaberPose);
            this.saberDeviceManager.SetRightSaberPose(rightSaberPose);
        }

        /// <summary>
        /// Moves the maul sabers based on a two controller scheme
        /// </summary>
        private void TransformTwoControllerMaul()
        {
            float sep = 1.0f * this.configuration.Current.MaulDistance / 100.0f;

            // Determine Hand positions
            Pose leftHand = this.saberDeviceManager.GetLeftSaberPose(this.configuration.Current.LeftTracker);
            Pose rightHand = this.saberDeviceManager.GetRightSaberPose(this.configuration.Current.RightTracker);

            // Determine final saber positions and rotations
            Vector3 middlePos = (rightHand.position + leftHand.position) * 0.5f;
            Vector3 forward = (rightHand.position - leftHand.position).normalized;
            Vector3 rightHandUp = rightHand.rotation * Vector3.up;

            Pose forwardSaberPose = new Pose(middlePos + (forward * sep), Quaternion.LookRotation(forward, rightHandUp));
            Pose backwardSaberPose = new Pose(middlePos + (-forward * sep), Quaternion.LookRotation(-forward, -rightHandUp));

            Pose leftSaberPose = this.configuration.Current.ReverseMaulDirection ? forwardSaberPose : backwardSaberPose;
            Pose rightSaberPose = this.configuration.Current.ReverseMaulDirection ? backwardSaberPose : forwardSaberPose;

            this.saberDeviceManager.SetLeftSaberPose(leftSaberPose);
            this.saberDeviceManager.SetRightSaberPose(rightSaberPose);
        }
    }
}
