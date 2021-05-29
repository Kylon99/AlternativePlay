using AlternativePlay.Models;
using System.Collections;
using UnityEngine;
using AlternativePlay.HarmonyPatches;

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

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Awake()
        {
            if (MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager)
                this.saberManager = MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager;
            else
                this.saberManager = FindObjectOfType<SaberManager>();

            this.useLeftHandForward = !Configuration.instance.ConfigurationData.UseLeftSpear;

            var pauseAnimationController = FindObjectOfType<PauseAnimationController>();
            if (pauseAnimationController != null) pauseAnimationController.resumeFromPauseAnimationDidFinishEvent += this.ResumeFromPauseAnimationDidFinishEvent;
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSpear)
            {
                // Do nothing if we aren't playing Beat Spear or if we can't find the player controller
                return;
            }

            switch (Configuration.instance.ConfigurationData.SpearControllerCount)
            {
                case ControllerCountEnum.One:
                    this.TransformForOneControllerSpearLeft();
                    this.TransformForOneControllerSpearRight();
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
            Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.rightSaber : this.saberManager.leftSaber;
            saberToHide.gameObject.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            saberToHide.gameObject.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Transform the left spear based on the controller or the tracker
        /// </summary>
        private void TransformForOneControllerSpearLeft()
        {
            var config = Configuration.instance.ConfigurationData;
            if (!config.UseLeftController && config.RemoveOtherSaber) { return; }

            Pose saberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftSpearTracker);
            this.saberManager.leftSaber.transform.position = saberPose.position;
            this.saberManager.leftSaber.transform.rotation = saberPose.rotation;
            if (config.ReverseSpearDirection)
            {
                this.saberManager.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }

            if (MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager)
                MultiplayerSyncStateManagerPatch.SetMultiplayerSaberPositionAndRotate(this.saberManager.leftSaber,this.saberManager.rightSaber);

        }

        /// <summary>
        /// Transform the right spear based on the controller or the tracker
        /// </summary>
        private void TransformForOneControllerSpearRight()
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.UseLeftController && config.RemoveOtherSaber) { return; }

            Pose saberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightSpearTracker);
            this.saberManager.rightSaber.transform.position = saberPose.position;
            this.saberManager.rightSaber.transform.rotation = saberPose.rotation;
            if (config.ReverseSpearDirection)
            {
                this.saberManager.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }
            if (MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager)
                MultiplayerSyncStateManagerPatch.SetMultiplayerSaberPositionAndRotate(this.saberManager.leftSaber, this.saberManager.rightSaber);
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

            if (Configuration.instance.ConfigurationData.ReverseSpearDirection)
            {
                forward = -forward;
            }

            // Apply transforms to saber
            Saber saberToTransform = Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.leftSaber : this.saberManager.rightSaber;
            saberToTransform.transform.position = saberPosition;
            saberToTransform.transform.rotation = Quaternion.LookRotation(forward, up);

            if (MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager)
                MultiplayerSyncStateManagerPatch.SetMultiplayerSaberPositionAndRotate(Configuration.instance.ConfigurationData.UseLeftSpear ? saberToTransform : this.saberManager.rightSaber, Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.leftSaber : saberToTransform);
        }

        /// <summary>
        /// Disables the rendering of the other saber
        /// </summary>
        private IEnumerator DisableOtherSaberMesh()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.rightSaber : this.saberManager.leftSaber;
            var saberRenderers = saberToHide.gameObject.GetComponentsInChildren<Renderer>();
            foreach (var r in saberRenderers) { r.enabled = false; }
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
