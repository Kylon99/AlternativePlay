using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay
{
    public class BeatSpearBehavior : MonoBehaviour
    {
        private const KeyCode leftTrigger = KeyCode.JoystickButton14;
        private const KeyCode rightTrigger = KeyCode.JoystickButton15;

        private PlayerController playerController;
        private MainSettingsModelSO mainSettingsModel;
        private XRNode previousForwardHand;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Spear
            if (ConfigOptions.instance.PlayMode != PlayMode.BeatSpear) { return; }

            if (ConfigOptions.instance.SpearControllerCount == ControllerCountEnum.Two)
            {
                this.previousForwardHand = ConfigOptions.instance.UseLeftSpear ? XRNode.RightHand : XRNode.LeftHand;
            }

            SharedCoroutineStarter.instance.StartCoroutine(HideOffColorSaber());

        }
        private IEnumerator HideOffColorSaber()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            // Always hide the off color saber
            Saber saberToHide = ConfigOptions.instance.UseLeftSpear ? this.playerController.rightSaber : this.playerController.leftSaber;
            saberToHide.gameObject.SetActive(false);
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
            this.mainSettingsModel = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
            this.previousForwardHand = ConfigOptions.instance.UseLeftSpear ? XRNode.RightHand : XRNode.LeftHand;

            var pauseAnimationController = Object.FindObjectOfType<PauseAnimationController>();
            if (pauseAnimationController != null) pauseAnimationController.resumeFromPauseAnimationDidFinishEvent += this.ResumeFromPauseAnimationDidFinishEvent;
        }

        private void Update()
        {
            const float handleLength = 0.75f;
            const float handleLengthSquared = 0.5625f;

            if (ConfigOptions.instance.PlayMode != PlayMode.BeatSpear || playerController == null)
            {
                // Do nothing if we aren't playing Beat Spear or if we aren't using two controllers
                return;
            }

            switch (ConfigOptions.instance.SpearControllerCount)
            {
                case ControllerCountEnum.One:
                    if (ConfigOptions.instance.ReverseSpearDirection)
                    {
                        playerController.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                        playerController.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                    }
                    // Nothing else needs to be done for one controller/tracker
                    break;
                case ControllerCountEnum.Two:
                    // Determine the forward hand
                    if (Input.GetKeyDown(leftTrigger)) { this.previousForwardHand = XRNode.LeftHand; }
                    if (Input.GetKeyDown(rightTrigger)) { this.previousForwardHand = XRNode.RightHand; }

                    XRNode forwardHandNode = this.previousForwardHand;
                    XRNode rearHandNode = forwardHandNode == XRNode.RightHand ? XRNode.LeftHand : XRNode.RightHand;

                    // Get positions and rotations of hands
                    (Vector3 position, Quaternion rotation) forwardHand = this.GetXRNodePosRos(forwardHandNode);
                    (Vector3 position, Quaternion rotation) rearHand = this.GetXRNodePosRos(rearHandNode);
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

                    if (ConfigOptions.instance.ReverseSpearDirection)
                    {
                        forward = -forward;
                    }

                    // Apply transforms to saber
                    Saber saberToTransform = ConfigOptions.instance.UseLeftSpear ? this.playerController.leftSaber : this.playerController.rightSaber;
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
            if (ConfigOptions.instance.PlayMode == PlayMode.BeatSpear)
            {
                Saber saberToHide = ConfigOptions.instance.UseLeftSpear ? this.playerController.rightSaber : this.playerController.leftSaber;
                saberToHide.gameObject.SetActive(false);
            }
        }

        private (Vector3, Quaternion) GetXRNodePosRos(XRNode node)
        {
            var pos = InputTracking.GetLocalPosition(node);
            var rot = InputTracking.GetLocalRotation(node);

            var roomCenter = this.mainSettingsModel.roomCenter;
            var roomRotation = Quaternion.Euler(0, this.mainSettingsModel.roomRotation, 0);
            pos = roomRotation * pos;
            pos += roomCenter;
            rot = roomRotation * rot;
            return (pos, rot);
        }


    }
}
