using AlternativePlay.Models;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay
{
    /// <summary>
    /// Responsible for maintaining and tracking the sabers to the controller or tracker
    /// positions which is necessary after Beat Saber 1.13.2 where the SaberManager no 
    /// longer resets the saber position on each Update() frame.
    /// </summary>
    public class SaberDeviceManager : MonoBehaviour
    {
        private SaberManager saberManager;
        private GameObject playerOrigin;
        private InputDevice leftController;
        private InputDevice rightController;

        private Pose savedLeftController;
        private Pose savedLeftSaber;
        private Pose savedRightController;
        private Pose savedRightSaber;
        private bool calibrated;

        public void BeginGameCoreScene()
        {
            this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            this.playerOrigin = GameObject.Find("LocalPlayerGameCore/Origin");
            calibrated = false;
        }

        /// <summary>
        /// Gets the pose for the given tracker config data or else it falls back to the
        /// left saber pose. This method accounts for Room Adjust and Noodle Extensions
        /// changing viewpoint functionality.
        /// </summary>
        public Pose GetLeftSaberPose(TrackerConfigData configData)
        {
            Pose? trackerPose;
            if (!String.IsNullOrWhiteSpace(configData?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(configData.Serial)) != null)
            {
                // Return adjusted position from the tracker
                Pose adjustedPose = this.AdjustForPlayerOrigin(trackerPose.Value);
                return Utilities.CalculatePoseFromTrackerData(configData, adjustedPose);
            }
            else
            {
                if (!calibrated || !this.leftController.isValid) return new Pose();

                // Return adjusted position from the saber
                Pose controllerPose = TrackedDeviceManager.GetDevicePose(this.leftController) ?? new Pose();
                Pose adjustedControllerPose = this.AdjustForPlayerOrigin(controllerPose);
                return TrackedDeviceManager.GetTrackedObjectPose(this.savedLeftSaber, this.savedLeftController, adjustedControllerPose);
            }
        }

        /// <summary>
        /// Gets the pose for the given tracker config data or else it falls back to the
        /// right saber pose. This method accounts for Room Adjust and Noodle Extensions
        /// changing viewpoint functionality.
        /// </summary>
        public Pose GetRightSaberPose(TrackerConfigData configData)
        {
            Pose? trackerPose;
            if (!String.IsNullOrWhiteSpace(configData?.Serial) &&
                (trackerPose = TrackedDeviceManager.instance.GetPoseFromSerial(configData.Serial)) != null)
            {
                // Return adjusted position from the tracker
                Pose adjustedPose = this.AdjustForPlayerOrigin(trackerPose.Value);
                return Utilities.CalculatePoseFromTrackerData(configData, adjustedPose);
            }
            else
            {
                if (!calibrated || !this.rightController.isValid) return new Pose();

                // Return adjusted position from the saber
                Pose controllerPose = TrackedDeviceManager.GetDevicePose(this.rightController) ?? new Pose();
                Pose adjustedControllerPose = this.AdjustForPlayerOrigin(controllerPose);
                return TrackedDeviceManager.GetTrackedObjectPose(this.savedRightSaber, this.savedRightController, adjustedControllerPose);
            }
        }

        private void Awake()
        {
            this.saberManager = FindObjectOfType<SaberManager>();
        }

        private void Update()
        {
            if (!calibrated)
            {
                calibrated = true;

                // Save current controller position
                this.savedLeftController = TrackedDeviceManager.GetDevicePose(this.leftController) ?? new Pose();
                this.savedLeftController = this.AdjustForPlayerOrigin(this.savedLeftController);

                this.savedRightController = TrackedDeviceManager.GetDevicePose(this.rightController) ?? new Pose();
                this.savedRightController = this.AdjustForPlayerOrigin(this.savedRightController);

                // Save current game saber positions
                this.savedLeftSaber.position = saberManager.leftSaber.transform.position;
                this.savedLeftSaber.rotation = saberManager.leftSaber.transform.rotation;

                this.savedRightSaber.position = saberManager.rightSaber.transform.position;
                this.savedRightSaber.rotation = saberManager.rightSaber.transform.rotation;
            }
        }

        /// <summary>
        /// Adjusts any pose according to the player's position in Beat Saber into 
        /// the return pose. This accounts for Beat Saber's Room Adjust and Noodle Extensions
        /// viewpoint change functionality.
        /// </summary>
        private Pose AdjustForPlayerOrigin(Pose pose)
        {
            Pose newDevicePose = pose;
            if (playerOrigin != null)
            {
                // Adjust for room rotation and noodle extensions player movement as well
                newDevicePose.position = playerOrigin.transform.rotation * pose.position;
                newDevicePose.position += playerOrigin.transform.position;
                newDevicePose.position.x *= playerOrigin.transform.localScale.x;
                newDevicePose.position.y *= playerOrigin.transform.localScale.y;
                newDevicePose.position.z *= playerOrigin.transform.localScale.z;
                newDevicePose.rotation = playerOrigin.transform.rotation * pose.rotation;
            }

            return newDevicePose;
        }
    }
}
