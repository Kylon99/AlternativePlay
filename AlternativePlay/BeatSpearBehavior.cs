using AlternativePlay.Models;
using System;
using System.Collections;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatSpearBehavior : MonoBehaviour
    {
        private SaberManager saberManager;
        private bool useLeftHandForward;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Spear
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSpear) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            if (Configuration.instance.ConfigurationData.SpearControllerCount == ControllerCountEnum.Two)
            {
                this.useLeftHandForward = !Configuration.instance.ConfigurationData.UseLeftSpear;
            }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftSpearTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightSpearTracker);
            StartCoroutine(HideOffColorSaber());
        }


        private void Awake()
        {
            this.saberManager = FindObjectOfType<SaberManager>();
            this.useLeftHandForward = !Configuration.instance.ConfigurationData.UseLeftSpear;

            var pauseAnimationController = FindObjectOfType<PauseAnimationController>();
            if (pauseAnimationController != null) pauseAnimationController.resumeFromPauseAnimationDidFinishEvent += this.ResumeFromPauseAnimationDidFinishEvent;
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSpear || saberManager == null)
            {
                // Do nothing if we aren't playing Beat Spear or if we can't find the player controller
                return;
            }

            switch (Configuration.instance.ConfigurationData.SpearControllerCount)
            {
                case ControllerCountEnum.One:
                    TransformForOneControllerSpearLeft();
                    TransformForOneControllerSpearRight();
                    break;
                case ControllerCountEnum.Two:
                    TransformForTwoControllerSpear();
                    break;

                default:
                    // Do nothing
                    break;
            }
        }

        /// <summary>
        /// Transform the left spear based on the controller or the tracker
        /// </summary>
        private void TransformForOneControllerSpearLeft()
        {
            var config = Configuration.instance.ConfigurationData;
            if (!config.UseLeftController && config.RemoveOtherSaber) { return; }

            // Check for left tracker
            Pose? trackerPose;
            if (!String.IsNullOrWhiteSpace(config.LeftSpearTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftSpearTracker.Serial)) != null)
            {
                Utilities.TransformSaberFromTrackerData(saberManager.leftSaber.transform, config.LeftSpearTracker, trackerPose.Value);
                if (config.ReverseSpearDirection)
                {
                    saberManager.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }
            else
            {
                if (config.ReverseSpearDirection)
                {
                    // Transform the saber only if we need to also reverse its direction since we need to set the saber position first
                    Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetLeftSaberPose();
                    saberManager.leftSaber.transform.position = saberPose.position;
                    saberManager.leftSaber.transform.rotation = saberPose.rotation;
                    saberManager.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }
        }

        /// <summary>
        /// Transform the right spear based on the controller or the tracker
        /// </summary>
        private void TransformForOneControllerSpearRight()
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.UseLeftController && config.RemoveOtherSaber) { return; }

            // Check for right tracker
            Pose? trackerPose;
            if (!String.IsNullOrWhiteSpace(config.RightSpearTracker?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightSpearTracker.Serial)) != null)
            {
                Utilities.TransformSaberFromTrackerData(saberManager.rightSaber.transform, config.RightSpearTracker, trackerPose.Value);
                if (config.ReverseSpearDirection)
                {
                    saberManager.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }
            else
            {
                if (config.ReverseSpearDirection)
                {
                    // Transform the saber only if we need to also reverse its direction since we need to set the saber position first
                    Pose saberPose = BehaviorCatalog.instance.ControllerManager.GetRightSaberPose();
                    saberManager.rightSaber.transform.position = saberPose.position;
                    saberManager.rightSaber.transform.rotation = saberPose.rotation;
                    saberManager.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                }
            }
        }

        /// <summary>
        /// Transform the spear based on two controllers
        /// </summary>
        private void TransformForTwoControllerSpear()
        {
            const float handleLength = 0.75f;
            const float handleLengthSquared = 0.5625f;

            // Determine the forward hand
            if (Configuration.instance.ConfigurationData.UseTriggerToSwitchHands)
            {
                if (BehaviorCatalog.instance.InputManager.GetLeftTriggerClicked()) { this.useLeftHandForward = true; }
                if (BehaviorCatalog.instance.InputManager.GetRightTriggerClicked()) { this.useLeftHandForward = false; }
            }

            // Get positions and rotations of hands
            Pose forwardHand = this.useLeftHandForward ? this.GetLeftPosition() : this.GetRightPosition();
            Pose rearHand = this.useLeftHandForward ? this.GetRightPosition() : this.GetLeftPosition();
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

            if (Configuration.instance.ConfigurationData.ReverseSpearDirection)
            {
                forward = -forward;
            }

            // Apply transforms to saber
            Saber saberToTransform = Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.leftSaber : this.saberManager.rightSaber;
            saberToTransform.transform.position = saberPosition;
            saberToTransform.transform.rotation = Quaternion.LookRotation(forward, up);
        }

        /// <summary>
        /// Gets the position of the left tracker or controller
        /// </summary>
        private Pose GetLeftPosition()
        {
            var config = Configuration.instance.ConfigurationData;

            // Determine the left hand controller
            Pose? leftTracker;
            if (String.IsNullOrWhiteSpace(config.LeftSpearTracker?.Serial) ||
                (leftTracker = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftSpearTracker.Serial)) == null)
            {
                return BehaviorCatalog.instance.ControllerManager.GetLeftSaberPose();
            }
            else
            {
                return Utilities.CalculatePoseFromTrackerData(config.LeftSpearTracker, leftTracker.Value);
            }
        }

        /// <summary>
        /// Gets the position of the right tracker or controller
        /// </summary>
        private Pose GetRightPosition()
        {
            var config = Configuration.instance.ConfigurationData;

            // Determine the left hand controller
            Pose? rightTracker;
            if (String.IsNullOrWhiteSpace(config.RightSpearTracker?.Serial) ||
                (rightTracker = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightSpearTracker.Serial)) == null)
            {
                return BehaviorCatalog.instance.ControllerManager.GetRightSaberPose();
            }
            else
            {
                return Utilities.CalculatePoseFromTrackerData(config.RightSpearTracker, rightTracker.Value);
            }
        }

        private IEnumerator HideOffColorSaber()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            // Always hide the off color saber
            Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.rightSaber : this.saberManager.leftSaber;
            saberToHide.gameObject.SetActive(false);
        }

        private void ResumeFromPauseAnimationDidFinishEvent()
        {
            if (Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatSpear)
            {
                Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.rightSaber : this.saberManager.leftSaber;
                saberToHide.gameObject.SetActive(false);
            }
        }
    }
}
