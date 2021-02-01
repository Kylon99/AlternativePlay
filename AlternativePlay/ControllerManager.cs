using AlternativePlay.Models;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay
{
    /// <summary>
    /// Responsible for maintaining and tracking the sabers to the controller positions
    /// which is necessary after Beat Saber 1.13.2 where the SaberManager no longer
    /// resets the saber position on each Update() frame.
    /// </summary>
    public class ControllerManager : MonoBehaviour
    {
        private SaberManager saberManager;
        private MainSettingsModelSO mainSettingsModel;
        private InputDevice leftController;
        private InputDevice rightController;

        private Pose? savedLeftController;
        private Pose savedLeftSaber;
        private Pose? savedRightController;
        private Pose savedRightSaber;
        private bool calibrated;

        public void BeginGameCoreScene()
        {
            this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            calibrated = false;
        }

        public Pose GetLeftSaberPose()
        {
            if (!calibrated || !this.leftController.isValid) return new Pose();

            Pose? leftControllerPose = this.GetBeatSaberDevicePosition(this.leftController);
            if (leftControllerPose == null) return new Pose();

            return TrackedDeviceManager.GetTrackedObjectPose(this.savedLeftSaber, this.savedLeftController.Value, leftControllerPose.Value);
        }

        public Pose GetRightSaberPose()
        {
            if (!calibrated || !this.rightController.isValid) return new Pose();

            Pose? rightControllerPose = this.GetBeatSaberDevicePosition(this.rightController);
            if (rightControllerPose == null) return new Pose();

            return TrackedDeviceManager.GetTrackedObjectPose(this.savedRightSaber, this.savedRightController.Value, rightControllerPose.Value);
        }

        private void Awake()
        {
            this.saberManager = FindObjectOfType<SaberManager>();
            this.mainSettingsModel = Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
        }

        private void Update()
        {
            if (!calibrated)
            {
                calibrated = true;

                this.savedLeftController = this.GetBeatSaberDevicePosition(this.leftController);
                this.savedLeftSaber.position = saberManager.leftSaber.transform.position;
                this.savedLeftSaber.rotation = saberManager.leftSaber.transform.rotation;

                this.savedRightController = this.GetBeatSaberDevicePosition(this.rightController);
                this.savedRightSaber.position = saberManager.rightSaber.transform.position;
                this.savedRightSaber.rotation = saberManager.rightSaber.transform.rotation;
            }
        }

        /// <summary>
        /// Gets the position of the device but factors in the room rotation of Beat Saber into the return.
        /// It requires the <see cref="MainSettingsModelSO" /> which can be found by calling
        /// Resources.FindObjectsOfTypeAll<MainSettingsModelSO>().FirstOrDefault();
        /// </summary>
        private Pose? GetBeatSaberDevicePosition(InputDevice device)
        {
            Pose? devicePose = TrackedDeviceManager.GetDevicePose(device);
            if (devicePose == null) return null;

            var roomCenter = this.mainSettingsModel.roomCenter;
            var roomRotation = Quaternion.Euler(0, this.mainSettingsModel.roomRotation, 0);

            Pose newDevicePose = devicePose.Value;
            newDevicePose.position = roomRotation * devicePose.Value.position;
            newDevicePose.position += roomCenter;
            newDevicePose.rotation = roomRotation * devicePose.Value.rotation;
            return newDevicePose;
        }
    }
}
