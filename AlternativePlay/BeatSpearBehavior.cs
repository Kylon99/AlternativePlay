using AlternativePlay.Models;
using System;
using System.Collections;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatSpearBehavior : MonoBehaviour
    {
        private bool useLeftHandForward;
        private readonly Pose hiddenPose = new Pose(new Vector3(0.0f, -1000.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Spear
            if (Configuration.Current.PlayMode != PlayMode.BeatSpear) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            if (Configuration.Current.ControllerCount == ControllerCountEnum.Two)
            {
                this.useLeftHandForward = !Configuration.Current.UseLeft;
            }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.LeftTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.RightTracker);

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Awake()
        {
            this.useLeftHandForward = !Configuration.Current.UseLeft;
        }

        private void Update()
        {
            if (Configuration.Current.PlayMode != PlayMode.BeatSpear)
            {
                // Do nothing if we aren't playing Beat Spear or if we can't find the player controller
                return;
            }

            switch (Configuration.Current.ControllerCount)
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
            SaberDeviceManager saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;
            if (Configuration.Current.UseLeft)
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
            SaberDeviceManager saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;
            Action<Pose> setSpearPose = this.GetSpearPoseAction();
            Pose spearPose = Configuration.Current.UseLeft
                ? saberDeviceManager.GetLeftSaberPose(Configuration.Current.LeftTracker)
                : saberDeviceManager.GetRightSaberPose(Configuration.Current.RightTracker);

            // Check for reversing saber direction
            if (Configuration.Current.ReverseSpearDirection)
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
            if (Configuration.Current.UseTriggerToSwitchHands)
            {
                if (BehaviorCatalog.instance.InputManager.GetLeftTriggerClicked()) { this.useLeftHandForward = true; }
                if (BehaviorCatalog.instance.InputManager.GetRightTriggerClicked()) { this.useLeftHandForward = false; }
            }

            // Get positions and rotations of hands
            Pose leftPosition = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(Configuration.Current.LeftTracker);
            Pose rightPosition = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(Configuration.Current.RightTracker);

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

            if (Configuration.Current.ReverseSpearDirection)
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
            if (Configuration.Current.UseLeft)
                result = BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaberPose;
            else
                result = BehaviorCatalog.instance.SaberDeviceManager.SetRightSaberPose;

            return result;
        }

        /// <summary>
        /// Disables the rendering of the other saber
        /// </summary>
        private IEnumerator DisableOtherSaberMesh()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (Configuration.Current.UseLeft)
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableRightSaberMesh();
            }
            else
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableLeftSaberMesh();
            }
        }
    }
}
