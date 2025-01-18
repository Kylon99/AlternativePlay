using AlternativePlay.Models;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace AlternativePlay
{
    public class BeatSpearBehavior : MonoBehaviour
    {
        private readonly Pose hiddenPose = new Pose(new Vector3(0.0f, -1000.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));

#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
        [Inject]
        private SaberDeviceManager saberDeviceManager;
        [Inject]
        private InputManager inputManager;
#pragma warning restore CS0649

        private bool useLeftHandForward;

        private void Start()
        {
            // Do nothing if we aren't playing Beat Spear
            if (this.configuration.Current.PlayMode != PlayMode.BeatSpear) { return; }

            this.useLeftHandForward = !this.configuration.Current.UseLeft;

            if (this.configuration.Current.ControllerCount == ControllerCountEnum.Two)
            {
                this.useLeftHandForward = !this.configuration.Current.UseLeft;
            }

            Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.LeftTracker);
            Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.RightTracker);

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Update()
        {
            if (this.configuration.Current.PlayMode != PlayMode.BeatSpear)
            {
                // Do nothing if we aren't playing Beat Spear or if we can't find the player controller
                return;
            }

            switch (this.configuration.Current.ControllerCount)
            {
                case ControllerCountEnum.One:
                    this.TransformForOneControllerSpear();
                    break;
                case ControllerCountEnum.Two:
                    this.TransformForTwoControllerSpear();
                    break;

                default:
                    // Do nothing
                    break;
            }

            // Move the other saber away since there's a bug in the base game which makes it
            // able to cut bombs still
            SaberDeviceManager saberDeviceManager = this.saberDeviceManager;
            if (this.configuration.Current.UseLeft)
                saberDeviceManager.SetRightSaberPose(this.hiddenPose);
            else
                saberDeviceManager.SetLeftSaberPose(this.hiddenPose);
        }

        /// <summary>
        /// Transform the left spear based on the controller or the tracker
        /// </summary>
        private void TransformForOneControllerSpear()
        {
            // Get the spear pose and correct spear method
            SaberDeviceManager saberDeviceManager = this.saberDeviceManager;
            Action<Pose> setSpearPose = this.GetSpearPoseAction();
            Pose spearPose = this.configuration.Current.UseLeft
                ? saberDeviceManager.GetLeftSaberPose(this.configuration.Current.LeftTracker)
                : saberDeviceManager.GetRightSaberPose(this.configuration.Current.RightTracker);

            // Check for reversing saber direction
            if (this.configuration.Current.ReverseSpearDirection)
            {
                spearPose = spearPose.Reverse();
            }

            setSpearPose(spearPose);
        }

        /// <summary>
        /// Transform the spear based on two controllers
        /// </summary>
        private void TransformForTwoControllerSpear()
        {
            const float handleLength = 0.75f;
            const float handleLengthSquared = 0.5625f;

            // Determine the forward hand
            if (this.configuration.Current.UseTriggerToSwitchHands)
            {
                if (this.inputManager.GetLeftTriggerClicked()) { this.useLeftHandForward = true; }
                if (this.inputManager.GetRightTriggerClicked()) { this.useLeftHandForward = false; }
            }

            // Get positions and rotations of hands
            Pose leftPosition = this.saberDeviceManager.GetLeftSaberPose(this.configuration.Current.LeftTracker);
            Pose rightPosition = this.saberDeviceManager.GetRightSaberPose(this.configuration.Current.RightTracker);

            Pose forwardHand = this.useLeftHandForward ? leftPosition : rightPosition;
            Pose rearHand = this.useLeftHandForward ? rightPosition : leftPosition;
            Vector3 forward = (forwardHand.position - rearHand.position).normalized;
            Vector3 up = forwardHand.rotation * Vector3.one;

            // Determine final saber position
            Vector3 saberPosition;
            float handSeparationSquared = (forwardHand.position - rearHand.position).sqrMagnitude;
            if (handSeparationSquared > handleLengthSquared)
            {
                // Clamp the saber at the extent of the forward hand
                saberPosition = forwardHand.position;
            }
            else
            {
                // Allow the saber to be pushed forward by the rear hand
                saberPosition = rearHand.position + (forward * handleLength);
            }

            if (this.configuration.Current.ReverseSpearDirection)
            {
                forward = -forward;
            }

            // Apply transforms to saber
            Pose finalSpearPose = new Pose(saberPosition, Quaternion.LookRotation(forward, up));
            Action<Pose> setSpearPose = this.GetSpearPoseAction();
            setSpearPose(finalSpearPose);
        }

        /// <summary>
        /// Gets the method in <see cref="SaberDeviceManager"/> to set the correct saber
        /// based on whether we are using the left or the right as the spear.
        /// </summary>
        private Action<Pose> GetSpearPoseAction()
        {
            Action<Pose> result;
            if (this.configuration.Current.UseLeft)
                result = this.saberDeviceManager.SetLeftSaberPose;
            else
                result = this.saberDeviceManager.SetRightSaberPose;

            return result;
        }

        /// <summary>
        /// Disables the rendering of the other saber
        /// </summary>
        private IEnumerator DisableOtherSaberMesh()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (this.configuration.Current.UseLeft)
            {
                this.saberDeviceManager.DisableRightSaberMesh();
            }
            else
            {
                this.saberDeviceManager.DisableLeftSaberMesh();
            }
        }
    }
}
