using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        private PlayerController playerController;
        private InputManager inputManager;

        public static bool Split { get; set; }

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
            TrackedDeviceManager.instance.LoadTrackedDevices();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul || playerController == null)
            {
                // Do nothing if we aren't playing Darth Maul
                return;
            }

            if (Configuration.instance.ConfigurationData.UseTriggerToSeparate)
            {
                // Check to see if the trigger has been pressed
                bool leftTriggerPressed = inputManager.GetLeftTriggerClicked();
                bool rightTriggerPressed = inputManager.GetRightTriggerClicked();

                if (leftTriggerPressed || rightTriggerPressed)
                {
                    Split = !Split;
                }

                if (Split) return;  // When you split Darth Maul it's just regular two sabers so do nothing
            }

            TransformForMaul();
        }

        /// <summary>
        /// Track the Maul sabers using internal Beat Saber locations or the assigned trackers
        /// </summary>
        private void TransformForMaul()
        {
            switch (Configuration.instance.ConfigurationData.DarthMaulControllerCount)
            {
                case ControllerCountEnum.One:
                    TransformOneControllerMaul();
                    break;

                case ControllerCountEnum.Two:
                    TransformTwoControllerMaul();
                    break;

                default:
                    // Do nothing
                    break;
            }
        }

        /// <summary>
        /// Moves the maul sabers based on a one controller scheme
        /// </summary>
        private void TransformOneControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;

            var baseSaber = config.UseLeftController ? playerController.leftSaber : playerController.rightSaber;
            var otherSaber = config.UseLeftController ? playerController.rightSaber : playerController.leftSaber;
            var rotateSaber = config.ReverseMaulDirection ? baseSaber : otherSaber;

            string trackerPosition = config.UseLeftController ? config.LeftMaulTracker : config.RightMaulTracker;
            if (String.IsNullOrWhiteSpace(trackerPosition))
            {
                // Move saber using Beat Saber saber positions
                otherSaber.transform.localPosition = baseSaber.transform.localPosition;
                otherSaber.transform.localRotation = baseSaber.transform.localRotation;
            }
            else
            {
                Pose? trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(trackerPosition);
                if (trackerPose == null)
                {
                    // Fall back to move saber using Beat Saber saber positions
                    otherSaber.transform.localPosition = baseSaber.transform.localPosition;
                    otherSaber.transform.localRotation = baseSaber.transform.localRotation;
                }

                // Move both sabers to tracker pose
                baseSaber.transform.localPosition = trackerPose.Value.position;
                baseSaber.transform.localRotation = trackerPose.Value.rotation;
                otherSaber.transform.localPosition = trackerPose.Value.position;
                otherSaber.transform.localRotation = trackerPose.Value.rotation;
            }

            rotateSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            rotateSaber.transform.Translate(0.0f, 0.0f, sep * 2.0f, Space.Self);
        }

        /// <summary>
        /// Moves the maul sabers based on a two controller scheme
        /// </summary>
        private void TransformTwoControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;

            // Determine Left Hand position
            Vector3 leftHandPos;
            if (String.IsNullOrWhiteSpace(config.LeftMaulTracker))
            {
                leftHandPos = playerController.leftSaber.transform.position;
            }
            else
            {
                Vector3? trackerPosition = TrackedDeviceManager.instance.GetPositionFromSerial(config.LeftMaulTracker);
                if (trackerPosition == null)
                {
                    // Fall back to the Beat Saber saber position
                    leftHandPos = playerController.leftSaber.transform.position;
                }

                leftHandPos = trackerPosition.Value;
            }

            // Determine Right Hand position
            Vector3 rightHandPos;
            if (String.IsNullOrWhiteSpace(config.RightMaulTracker))
            {
                rightHandPos = playerController.rightSaber.transform.position;
            }
            else
            {
                Vector3? trackerPosition = TrackedDeviceManager.instance.GetPositionFromSerial(config.RightMaulTracker);
                if (trackerPosition == null)
                {
                    // Fall back to the Beat Saber saber position
                    rightHandPos = playerController.rightSaber.transform.position;
                }

                rightHandPos = trackerPosition.Value;
            }

            Vector3 middlePos = (rightHandPos + leftHandPos) * 0.5f;
            Vector3 forward = (rightHandPos - leftHandPos).normalized;

            var forwardSaber = config.ReverseMaulDirection ? playerController.leftSaber : playerController.rightSaber;
            var backwardSaber = config.ReverseMaulDirection ? playerController.rightSaber : playerController.leftSaber;

            forwardSaber.transform.position = middlePos + forward * sep;
            backwardSaber.transform.position = middlePos + -forward * sep;
            forwardSaber.transform.rotation = Quaternion.LookRotation(forward, backwardSaber.transform.up);
            backwardSaber.transform.rotation = Quaternion.LookRotation(-forward, -backwardSaber.transform.up);
        }
    }
}
