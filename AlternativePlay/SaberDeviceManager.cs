using AlternativePlay.HarmonyPatches;
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
            this.calibrated = false;
        }

        /// <summary>
        /// Gets the pose for the given tracker config data or else it falls back to the
        /// left controller pose. This method accounts for Room Adjust and Noodle Extensions
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
                if (!this.calibrated || !this.leftController.isValid) return new Pose();

                // Return adjusted position from the saber
                Pose controllerPose = TrackedDeviceManager.GetDevicePose(this.leftController) ?? new Pose();
                Pose adjustedControllerPose = this.AdjustForPlayerOrigin(controllerPose);
                return TrackedDeviceManager.GetTrackedObjectPose(this.savedLeftSaber, this.savedLeftController, adjustedControllerPose);
            }
        }

        /// <summary>
        /// Gets the pose for the given tracker config data or else it falls back to the
        /// right controller pose. This method accounts for Room Adjust and Noodle Extensions
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
                if (!this.calibrated || !this.rightController.isValid) return new Pose();

                // Return adjusted position from the saber
                Pose controllerPose = TrackedDeviceManager.GetDevicePose(this.rightController) ?? new Pose();
                Pose adjustedControllerPose = this.AdjustForPlayerOrigin(controllerPose);
                return TrackedDeviceManager.GetTrackedObjectPose(this.savedRightSaber, this.savedRightController, adjustedControllerPose);
            }
        }

        /// <summary>
        /// Moves the left saber to the given <see cref="Pose"/>
        /// </summary>
        public void SetLeftSaberPose(Pose pose)
        {
            if (this.saberManager == null) return;

            this.saberManager.leftSaber.transform.position = pose.position;
            this.saberManager.leftSaber.transform.rotation = pose.rotation;

            if (MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager)
                MultiplayerSyncStateManagerPatch.SetMultiplayerLeftSaberPose(pose);
        }

        /// <summary>
        /// Moves the right saber to the given <see cref="Pose"/>
        /// </summary>
        public void SetRightSaberPose(Pose pose)
        {
            if (this.saberManager == null) return;

            this.saberManager.rightSaber.transform.position = pose.position;
            this.saberManager.rightSaber.transform.rotation = pose.rotation;

            if (MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager)
                MultiplayerSyncStateManagerPatch.SetMultiplayerRightSaberPose(pose);
        }

        /// <summary>
        /// Gets the pose for the given tracker config data or else it falls back to the
        /// left controller and then moves the left saber
        /// </summary>
        public void SetLeftSaber(TrackerConfigData trackerData)
        {
            Pose pose = this.GetLeftSaberPose(trackerData);
            this.SetLeftSaberPose(pose);
        }

        /// <summary>
        /// Gets the pose for the given tracker config data or else it falls back to the
        /// right controller and then moves the right saber
        /// </summary>
        public void SetRightSaber(TrackerConfigData trackerData)
        {
            Pose pose = this.GetRightSaberPose(trackerData);
            this.SetRightSaberPose(pose);
        }

        /// <summary>
        /// Disables all the renders in the left saber.  You may have to await or 
        /// yield return for 0.1f seconds before this method will work properly
        /// </summary>
        public void DisableLeftSaberMesh()
        {
            var saberRenderers = this.saberManager.leftSaber.gameObject.GetComponentsInChildren<Renderer>();
            foreach (var r in saberRenderers) { r.enabled = false; }
        }

        /// <summary>
        /// Disables all the renders in the right saber.  You may have to await or 
        /// yield return for 0.1f seconds before this method will work properly
        /// </summary>
        public void DisableRightSaberMesh()
        {
            var saberRenderers = this.saberManager.rightSaber.gameObject.GetComponentsInChildren<Renderer>();
            foreach (var r in saberRenderers) { r.enabled = false; }
        }

        private void Awake()
        {
            this.saberManager = MultiplayerLocalActivePlayerGameplayManagerPatch.multiplayerSaberManager ?? FindObjectOfType<SaberManager>();
            this.playerOrigin = GameObject.Find("LocalPlayerGameCore/Origin");
            this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }

        private void Update()
        {
            if (!this.calibrated) this.CalibrateSaberPositions();
        }

        /// <summary>
        /// Save the initial saber positions relative to the VRControllers 
        /// </summary>
        private void CalibrateSaberPositions()
        {
            this.calibrated = true;

            // Save current controller position
            this.savedLeftController = TrackedDeviceManager.GetDevicePose(this.leftController) ?? new Pose();
            this.savedLeftController = this.AdjustForPlayerOrigin(this.savedLeftController);

            this.savedRightController = TrackedDeviceManager.GetDevicePose(this.rightController) ?? new Pose();
            this.savedRightController = this.AdjustForPlayerOrigin(this.savedRightController);

            // Save current game saber positions
            this.savedLeftSaber.position = this.saberManager.leftSaber.transform.position;
            this.savedLeftSaber.rotation = this.saberManager.leftSaber.transform.rotation;

            this.savedRightSaber.position = this.saberManager.rightSaber.transform.position;
            this.savedRightSaber.rotation = this.saberManager.rightSaber.transform.rotation;
        }

        /// <summary>
        /// Adjusts any pose according to the player's position in Beat Saber into 
        /// the return pose. This accounts for Beat Saber's Room Adjust and Noodle Extensions
        /// viewpoint change functionality.
        /// </summary>
        private Pose AdjustForPlayerOrigin(Pose pose)
        {
            Pose newDevicePose = pose;
            if (this.playerOrigin != null)
            {
                // Adjust for room rotation and noodle extensions player movement as well
                newDevicePose.position = this.playerOrigin.transform.rotation * pose.position;
                newDevicePose.position += this.playerOrigin.transform.position;
                newDevicePose.position.x *= this.playerOrigin.transform.localScale.x;
                newDevicePose.position.y *= this.playerOrigin.transform.localScale.y;
                newDevicePose.position.z *= this.playerOrigin.transform.localScale.z;
                newDevicePose.rotation = this.playerOrigin.transform.rotation * pose.rotation;
            }

            return newDevicePose;
        }
    }
}
