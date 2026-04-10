using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;
using Zenject;
using static Valve.VR.IVRSystem;

namespace AlternativePlay.Models
{
    public class OpenVRDeviceInfo
    {
        public int Index { get; set; }
        public Pose Pose { get; set; }
        public ETrackedDeviceClass DeviceClass { get; set; }
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Manufacturer { get; set; }
    }

    public class OpenVRDevicePose
    {
        public int Index { get; set; }
        public ETrackedDeviceClass DeviceClass { get; set; }
        public string Serial { get; set; }

    }

    /// <summary>
    /// Manages a list of tracked devices and provides convenient functions to interact with them.
    /// </summary>
    public class TrackedDeviceManager
    {
#pragma warning disable CS0649
        [Inject]
        private OpenVRManager openVRManager;
#pragma warning restore CS0649

        public List<OpenVRDeviceInfo> TrackedDevices { get; private set; } = new List<OpenVRDeviceInfo>();

        /// <summary>
        /// Updates the list of valid tracked devices and their properties only.  Use <see cref="PollTrackedDevices"/> to get the
        /// poses of the devices instead.
        /// </summary>
        public void LoadTrackedDeviceProperties()
        {
            // Get a list of all valid tracked devices up to OpenVR's maximum
            this.TrackedDevices = Enumerable.Range(0, (int)OpenVR.k_unMaxTrackedDeviceCount).Select(i =>
                new OpenVRDeviceInfo
                {
                    Index = i,
                    DeviceClass = this.openVRManager.System.GetTrackedDeviceClass((uint)i),
                })
                .Where(di => di.DeviceClass != ETrackedDeviceClass.Invalid)
                .ToList();

            // Populate the extra data that we need for display purposes
            this.TrackedDevices.ForEach(di =>
            {
                var nameBuilder = new StringBuilder(120);
                var nameError = new ETrackedPropertyError();
                this.openVRManager.System.GetStringTrackedDeviceProperty((uint)di.Index, ETrackedDeviceProperty.Prop_ModelNumber_String, nameBuilder, 120, ref nameError);

                var serialBuilder = new StringBuilder(120);
                var serialError = new ETrackedPropertyError();
                this.openVRManager.System.GetStringTrackedDeviceProperty((uint)di.Index, ETrackedDeviceProperty.Prop_SerialNumber_String, serialBuilder, 120, ref serialError);

                var manufacturerBuilder = new StringBuilder(120);
                var manufacturerError = new ETrackedPropertyError();
                this.openVRManager.System.GetStringTrackedDeviceProperty((uint)di.Index, ETrackedDeviceProperty.Prop_ManufacturerName_String, manufacturerBuilder, 120, ref manufacturerError);

                di.Name = nameBuilder.ToString();
                di.Serial = serialBuilder.ToString();
                di.Manufacturer = manufacturerBuilder.ToString();
            });
        }

        /// <summary>
        /// Poll the current positions of all of the tracked devices. Meant to be called once per frame (Update).
        /// The OpenVR API polls all of them at once so it's more efficient to just call this even if you want one device.
        /// </summary>
        /// <remarks>
        /// Ensure that <see cref="LoadTrackedDeviceProperties"/> has been called recently to ensure the list is up to date!
        /// </remarks>
        public void PollTrackedDevices()
        {
            const float minPredictionSeconds = 0.0f;
            const float maxPredictionSeconds = 0.1f;
            const float predictionBiasSeconds = 0.0225f;

            // Calculate time to predict into the future
            float secondsSinceLastVsync = 0.0f;
            ulong pullFrameCounter = 0;
            this.openVRManager.System.GetTimeSinceLastVsync(ref secondsSinceLastVsync, ref pullFrameCounter);

            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_Success;
            float displayFrequency = this.openVRManager.System.GetFloatTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_DisplayFrequency_Float, ref error);
            float frameDuration = 1.0f / displayFrequency;
            float fVsyncToPhotons = this.openVRManager.System.GetFloatTrackedDeviceProperty(OpenVR.k_unTrackedDeviceIndex_Hmd, ETrackedDeviceProperty.Prop_SecondsFromVsyncToPhotons_Float, ref error);

            float predictedSecondsFromNow = frameDuration - secondsSinceLastVsync + fVsyncToPhotons;
            predictedSecondsFromNow += predictionBiasSeconds;

            if (predictedSecondsFromNow < minPredictionSeconds) { predictedSecondsFromNow = minPredictionSeconds; }
            if (predictedSecondsFromNow > maxPredictionSeconds) { predictedSecondsFromNow = maxPredictionSeconds; }

            // Get all tracked device poses from OpenVR API
            TrackedDevicePose_t[] trackedDevicePoseArray = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            this.openVRManager.System.GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin.TrackingUniverseStanding, predictedSecondsFromNow, trackedDevicePoseArray);

            foreach (var device in this.TrackedDevices)
            {
                var polledDevice = trackedDevicePoseArray[device.Index];
                Vector3 position = polledDevice.mDeviceToAbsoluteTracking.GetPosition();
                Quaternion rotation = polledDevice.mDeviceToAbsoluteTracking.GetRotation();
                device.Pose = new Pose(position, rotation);
            }
        }

        /// <summary>
        /// Gets the <see cref="InputDevice"/> from the saved list of devices given the serial
        /// </summary>
        /// <param name="serial">The serial number of the tracked device</param>
        /// <returns>A <see cref="InputDevice"/> if found or otherwise null</returns>
        public OpenVRDeviceInfo GetInputDeviceFromSerial(string serial)
        {
            return this.TrackedDevices.FirstOrDefault(i => i.Serial == serial);
        }

        /// <summary>
        /// Gets the Pose of a tracked device given the serial string
        /// </summary>
        /// <param name="serial">The serial number of the tracked device</param>
        /// <returns>A pose containing the position and rotation of the tracked device</returns>
        public Pose? GetPoseFromSerial(string serial)
        {
            var device = this.TrackedDevices.FirstOrDefault(i => i.Serial == serial);
            if (device == null) { return null; }

            return device.Pose;
        }

        public Pose? GetPoseFromLeftController()
        {
            if (TryGetOpenXrControllerPose(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left, out Pose openXrPose))
            {
                return openXrPose;
            }

            uint index = this.openVRManager.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
            var device = this.TrackedDevices.ElementAtOrDefault((int)index);

            if (device == null) { return null; }

            return device.Pose;
        }

        public Pose? GetPoseFromRightController()
        {
            if (TryGetOpenXrControllerPose(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right, out Pose openXrPose))
            {
                return openXrPose;
            }

            uint index = this.openVRManager.System.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
            var device = this.TrackedDevices.FirstOrDefault(d => d.Index == (int)index);

            if (device == null) { return null; }

            return device.Pose;
        }

        private static bool TryGetOpenXrControllerPose(InputDeviceCharacteristics characteristics, out Pose pose)
        {
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(characteristics, devices);

            foreach (var device in devices)
            {
                bool hasPosition = device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
                bool hasRotation = device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
                if (hasPosition && hasRotation)
                {
                    pose = new Pose(position, rotation);
                    return true;
                }
            }

            pose = default;
            return false;
        }

        /// <summary>
        /// This method calls on <see cref="GetTrackedObjectPose(Pose, Pose, Pose)"/> to compute the tracked position
        /// given the serial number of the tracker.
        /// </summary>
        /// <param name="serial">The serial of the tracked device to use as the basis</param>
        /// <param name="objectPose">The pose of the object's world location at the tiem of calibration</param>
        /// <param name="calibratedPose">The pose of the tracked device at the time of calibration</param>
        /// <returns>A pose containing the new position and rotation of the tracked object.  Null if the tracked
        /// device could not be found.</returns>

        public Pose? GetTrackedObjectPoseBySerial(string serial, Pose objectPose, Pose calibratedPose)
        {
            // Get the current tracker position
            var currentDevicePose = this.GetPoseFromSerial(serial);
            if (currentDevicePose == null) { return null; }

            return GetTrackedObjectPose(objectPose, calibratedPose, currentDevicePose.Value);
        }

        /// <summary>
        /// This method computes the new <see cref="Pose"/> of a tracked object, given its original Pose, 
        /// the original calibrated Pose of the the tracked device, and the current tracked device's Pose.
        /// In essense this provides the code to track an object using a tracked device
        /// </summary>
        /// <param name="objectPose">The pose of the object's world location at the tiem of calibration</param>
        /// <param name="calibratedPose">The pose of the tracked device at the time of calibration</param>
        /// <param name="devicePose">The current pose of the device that is doing the tracking</param>
        /// <returns>A pose containing the new position and rotation of the tracked object.</returns>
        public static Pose GetTrackedObjectPose(Pose objectPose, Pose calibratedPose, Pose devicePose)
        {
            Pose result;

            // Apply the diff of rotation from the calibrated position and the current position to the original rotation of the object
            Quaternion rotationDiff = devicePose.rotation * Quaternion.Inverse(calibratedPose.rotation); // Unity and everyone uses inverse order rotation multiplication
            result.rotation = rotationDiff * objectPose.rotation;

            // Calculate the diff in position due to tracker rotation
            Vector3 originTrackerDiff = objectPose.position - calibratedPose.position;
            Vector3 rotatedOriginTrackerDiff = rotationDiff * originTrackerDiff;
            Vector3 positionDiffFromRotation = rotatedOriginTrackerDiff - originTrackerDiff; // The change in position due to rotation

            // Apply the diff from the calibrated position and the current to the original object position
            Vector3 totalPositionDiff = calibratedPose.position - devicePose.position;
            result.position = objectPose.position - totalPositionDiff + positionDiffFromRotation;

            return result;
        }
    }
}
