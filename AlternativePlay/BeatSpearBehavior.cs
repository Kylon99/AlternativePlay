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
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.BeatSpear) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            if (Configuration.instance.ConfigurationData.SpearControllerCount == ControllerCountEnum.Two)
            {
                this.useLeftHandForward = !Configuration.instance.ConfigurationData.UseLeftSpear;
            }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftSpearTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightSpearTracker);

            // Take control of both sabers
            SaberDeviceManager saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;
            saberDeviceManager.DisableLeftVRControl();
            saberDeviceManager.DisableRightVRControl();

            // Move the other saber away since there's a bug in the base game which makes it
            // able to cut bombs still
            if (config.UseLeftSpear)
                saberDeviceManager.SetRightSaberPose(hiddenPose);
            else
                saberDeviceManager.SetLeftSaberPose(hiddenPose);

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Awake()
        {
            this.useLeftHandForward = !Configuration.instance.ConfigurationData.UseLeftSpear;
        }

        private void Update()
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.BeatSpear)
            {
                // Do nothing if we aren't playing Beat Spear or if we can't find the player controller
                return;
            }

            switch (config.SpearControllerCount)
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
        }

        /// <summary>
        /// Transform the left spear based on the controller or the tracker
        /// </summary>
        private void TransformForOneControllerSpear()
        {
            var config = Configuration.instance.ConfigurationData;

            // Get the spear pose and correct spear method
            SaberDeviceManager saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;
            Action<Pose> setSpearPose = this.GetSpearPoseAction();
            Pose spearPose = config.UseLeftSpear
                ? saberDeviceManager.GetLeftSaberPose(config.LeftSpearTracker)
                : saberDeviceManager.GetRightSaberPose(config.RightSpearTracker);

            // Check for reversing saber direction
            if (config.ReverseSpearDirection)
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
            var config = Configuration.instance.ConfigurationData;

            // Determine the forward hand
            if (Configuration.instance.ConfigurationData.UseTriggerToSwitchHands)
            {
                if (BehaviorCatalog.instance.InputManager.GetLeftTriggerClicked()) { this.useLeftHandForward = true; }
                if (BehaviorCatalog.instance.InputManager.GetRightTriggerClicked()) { this.useLeftHandForward = false; }
            }

            // Get positions and rotations of hands
            Pose leftPosition = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftSpearTracker);
            Pose rightPosition = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightSpearTracker);

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

            if (config.ReverseSpearDirection)
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
            var config = Configuration.instance.ConfigurationData;

            Action<Pose> result;
            if (config.UseLeftSpear)
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

            if (Configuration.instance.ConfigurationData.UseLeftSpear)
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
