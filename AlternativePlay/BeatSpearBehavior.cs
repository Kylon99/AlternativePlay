using AlternativePlay.Models;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay
{
    public class BeatSpearBehavior : MonoBehaviour
    {
        private PlayerController playerController;
        private InputDevice leftController;
        private InputDevice rightController;

        private MainSettingsModelSO mainSettingsModel;
        private InputDevice previousForwardHand;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Spear
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSpear) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();
            this.GetHandDevices();

            if (Configuration.instance.ConfigurationData.SpearControllerCount == ControllerCountEnum.Two)
            {
                this.previousForwardHand = Configuration.instance.ConfigurationData.UseLeftSpear ? this.rightController : this.leftController;
            }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftSpearTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightSpearTracker);
            StartCoroutine(HideOffColorSaber());
        }


        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
            this.mainSettingsModel = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
            this.previousForwardHand = Configuration.instance.ConfigurationData.UseLeftSpear ? this.rightController : this.leftController;

            var pauseAnimationController = UnityEngine.Object.FindObjectOfType<PauseAnimationController>();
            if (pauseAnimationController != null) pauseAnimationController.resumeFromPauseAnimationDidFinishEvent += this.ResumeFromPauseAnimationDidFinishEvent;
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSpear || playerController == null)
            {
                // Do nothing if we aren't playing Beat Spear or if we can't find the player controller
                return;
            }

            switch (Configuration.instance.ConfigurationData.SpearControllerCount)
            {
                case ControllerCountEnum.One:
                    TransformForOneControllerSpear();
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
        /// Moves the spear based on one controller
        /// </summary>
        private void TransformForOneControllerSpear()
        {
            var config = Configuration.instance.ConfigurationData;

            // Check for left tracker
            if (!String.IsNullOrWhiteSpace(config.LeftSpearTracker?.Serial))
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.LeftSpearTracker.Serial);
                if (trackerPose != null)
                {
                    Utilities.TransformSaberFromTrackerData(playerController.leftSaber.transform, config.LeftSpearTracker,
                        trackerPose.Value.rotation, trackerPose.Value.position);
                }
            }

            // Check for right tracker
            if (!String.IsNullOrWhiteSpace(config.RightSpearTracker?.Serial))
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(config.RightSpearTracker.Serial);
                if (trackerPose != null)
                {
                    Utilities.TransformSaberFromTrackerData(playerController.rightSaber.transform, config.RightSpearTracker,
                        trackerPose.Value.rotation, trackerPose.Value.position);
                }
            }

            // Handle reversed spear directions
            if (config.ReverseSpearDirection)
            {
                playerController.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                playerController.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }
        }

        /// <summary>
        /// Moves the spear based on two controllers
        /// </summary>
        private void TransformForTwoControllerSpear()
        {
            const float handleLength = 0.75f;
            const float handleLengthSquared = 0.5625f;

            // Determine the forward hand
            if (Configuration.instance.ConfigurationData.UseTriggerToSwitchHands)
            {
                if (BehaviorCatalog.instance.InputManager.GetLeftTriggerClicked()) { this.previousForwardHand = this.leftController; }
                if (BehaviorCatalog.instance.InputManager.GetRightTriggerClicked()) { this.previousForwardHand = this.rightController; }
            }

            InputDevice forwardHandDevice = this.previousForwardHand;
            InputDevice rearHandDevice = (forwardHandDevice == this.rightController ? this.leftController : this.rightController);

            // Get positions and rotations of hands
            (Vector3 position, Quaternion rotation) forwardHand = this.GetXRNodePosRos(forwardHandDevice);
            (Vector3 position, Quaternion rotation) rearHand = this.GetXRNodePosRos(rearHandDevice);
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
            Saber saberToTransform = Configuration.instance.ConfigurationData.UseLeftSpear ? this.playerController.leftSaber : this.playerController.rightSaber;
            saberToTransform.transform.position = saberPosition;
            saberToTransform.transform.rotation = Quaternion.LookRotation(forward, up);
        }

        /// <summary>
        /// Determines which Input Devices to use as the left and right hands
        /// </summary>
        private void GetHandDevices()
        {
            var config = Configuration.instance.ConfigurationData;

            // Determine the left hand controller
            if (String.IsNullOrWhiteSpace(config.LeftSpearTracker?.Serial))
            {
                this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            }
            else
            {
                var device = TrackedDeviceManager.instance.GetInputDeviceFromSerial(config.LeftSpearTracker.Serial);
                if (device == null)
                {
                    this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                }

                this.leftController = device;
            }

            // Determine the right hand controller
            if (String.IsNullOrWhiteSpace(config.RightSpearTracker?.Serial))
            {
                this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            }
            else
            {
                var device = TrackedDeviceManager.instance.GetInputDeviceFromSerial(config.RightSpearTracker.Serial);
                if (device == null)
                {
                    this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                }

                this.rightController = device;
            }
        }

        private IEnumerator HideOffColorSaber()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            // Always hide the off color saber
            Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.playerController.rightSaber : this.playerController.leftSaber;
            saberToHide.gameObject.SetActive(false);
        }

        private void ResumeFromPauseAnimationDidFinishEvent()
        {
            if (Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatSpear)
            {
                Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.playerController.rightSaber : this.playerController.leftSaber;
                saberToHide.gameObject.SetActive(false);
            }
        }

        private (Vector3, Quaternion) GetXRNodePosRos(InputDevice hand)
        {
            hand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
            hand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot);

            var roomCenter = this.mainSettingsModel.roomCenter;
            var roomRotation = Quaternion.Euler(0, this.mainSettingsModel.roomRotation, 0);
            pos = roomRotation * pos;
            pos += roomCenter;
            rot = roomRotation * rot;
            return (pos, rot);
        }


    }
}
