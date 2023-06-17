using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay.Models
{
    /// <summary>
    /// Manages a list of tracked devices and provides convenient functions to interact with them.
    /// </summary>
    public class TrackedDeviceManager : PersistentSingleton<TrackedDeviceManager>
    {
        public List<InputDevice> TrackedDevices { get; private set; } = new List<InputDevice>();

        /// <summary>
        /// Updates the internal list of all tracked devices
        /// </summary>
        public void LoadTrackedDevices()
        {
            var desiredCharacteristics = InputDeviceCharacteristics.TrackedDevice;
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, this.TrackedDevices);
        }

        /// <summary>
        /// Allows you to get one device based on the XRNode, usually the XRNode.LeftHand or the
        /// XRNode.RightHand and will return the Pose of the device.  It will return null if the
        /// device could not be found or the tracked position and rotation cannot be obtained.
        /// </summary>
        /// <param name="node">The XRNode of the device to find</param>
        /// <returns>The position and rotation in a Pose of the device.  Null if it cannot be found
        /// or tracked.</returns>
        public Pose? GetDevicePose(XRNode node)
        {
            var device = InputDevices.GetDeviceAtXRNode(node);
            if (!device.isValid) return null;

            bool positionSuccess = device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
            bool rotationSuccess = device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
            if (positionSuccess && rotationSuccess)
            {
                return new Pose { position = position, rotation = rotation };
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="InputDevice"/> from the saved list of devices given the serial
        /// </summary>
        /// <param name="serial">The serial number of the tracked device</param>
        /// <returns>A <see cref="InputDevice"/> if found or otherwise null</returns>
        public InputDevice GetInputDeviceFromSerial(string serial)
        {
            return this.TrackedDevices.FirstOrDefault(i => i.serialNumber == serial);
        }

        /// <summary>
        /// Gets the position in a <see cref="Vector3"/> of a tracked device given the serial string
        /// </summary>
        /// <param name="serial">The serial number of the tracked device</param>
        /// <returns>A <see cref="Vector3"/> containing the position the tracked device</returns>
        public Vector3? GetPositionFromSerial(string serial)
        {
            var device = this.TrackedDevices.FirstOrDefault(i => i.serialNumber == serial);
            if (device == null) { return null; }

            bool positionSuccess = device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);

            if (positionSuccess)
            {
                return position;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the Pose of a tracked device given the serial string
        /// </summary>
        /// <param name="serial">The serial number of the tracked device</param>
        /// <returns>A pose containing the position and rotation of the tracked device</returns>
        public Pose? GetPoseFromSerial(string serial)
        {
            var device = this.TrackedDevices.FirstOrDefault(i => i.serialNumber == serial);
            if (device == null) { return null; }

            bool positionSuccess = device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
            bool rotationSuccess = device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

            if (positionSuccess && rotationSuccess)
            {
                return new Pose { position = position, rotation = rotation };
            }
            else
            {
                return null;
            }
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
            var currentDevicePose = instance.GetPoseFromSerial(serial);
            if (currentDevicePose == null) { return null; }

            return GetTrackedObjectPose(objectPose, calibratedPose, currentDevicePose.Value);
        }


        /// <summary>
        /// A convenience function to automatically poll the devices position and rotation into a Pose.
        /// If either or both cannot be polled, null will be returned.
        /// </summary>
        /// <param name="device">The device to poll</param>
        /// <returns>Returns a Pose containing the position and rotation.  Returns null if either or both
        /// cannot be read.</returns>
        public static Pose? GetDevicePose(InputDevice device)
        {
            bool getPositionSuccess = device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePosition);
            bool getRotationSuccess = device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion deviceRotation);

            if (!getPositionSuccess || !getRotationSuccess) return null;

            return new Pose(devicePosition, deviceRotation);
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
