using AlternativePlay.Models;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay
{
    public class BeatSpearBehavior : MonoBehaviour
    {
        private InputManager inputManager;
        private PlayerController playerController;
        private InputDevice leftController;
        private InputDevice rightController;

        private MainSettingsModelSO mainSettingsModel;
        private InputDevice previousForwardHand;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene(InputManager inputManager)
        {
            this.inputManager = inputManager;

            // Do nothing if we aren't playing Beat Spear
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSpear) { return; }

            this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            if (Configuration.instance.ConfigurationData.SpearControllerCount == ControllerCountEnum.Two)
            {
                this.previousForwardHand = Configuration.instance.ConfigurationData.UseLeftSpear ? this.rightController : this.leftController;
            }

            SharedCoroutineStarter.instance.StartCoroutine(HideOffColorSaber());

        }
        private IEnumerator HideOffColorSaber()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            // Always hide the off color saber
            Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.playerController.rightSaber : this.playerController.leftSaber;
            saberToHide.gameObject.SetActive(false);
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
            this.mainSettingsModel = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
            this.previousForwardHand = Configuration.instance.ConfigurationData.UseLeftSpear ? this.rightController : this.leftController;

            var pauseAnimationController = Object.FindObjectOfType<PauseAnimationController>();
            if (pauseAnimationController != null) pauseAnimationController.resumeFromPauseAnimationDidFinishEvent += this.ResumeFromPauseAnimationDidFinishEvent;
        }

        private void Update()
        {
            const float handleLength = 0.75f;
            const float handleLengthSquared = 0.5625f;

            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatSpear || playerController == null)
            {
                // Do nothing if we aren't playing Beat Spear or if we aren't using two controllers
                return;
            }

            switch (Configuration.instance.ConfigurationData.SpearControllerCount)
            {
                case ControllerCountEnum.One:
                    if (Configuration.instance.ConfigurationData.ReverseSpearDirection)
                    {
                        playerController.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                        playerController.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                    }
                    // Nothing else needs to be done for one controller/tracker
                    break;
                case ControllerCountEnum.Two:
                    // Determine the forward hand
                    if (this.inputManager.GetLeftTriggerClicked()) { this.previousForwardHand = this.leftController; }
                    if (this.inputManager.GetRightTriggerClicked()) { this.previousForwardHand = this.rightController; }

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
                    break;

                default:
                    // Do nothing
                    break;
            }
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
