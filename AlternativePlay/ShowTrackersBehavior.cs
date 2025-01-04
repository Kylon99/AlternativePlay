using AlternativePlay.Models;
using BeatSaber.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using Zenject;

namespace AlternativePlay
{
    public class ShowTrackersBehavior : MonoBehaviour
    {
#pragma warning disable CS0649
        [Inject]
        private TrackedDeviceManager trackedDeviceManager;
        [Inject]
        private AssetLoaderBehavior assetLoaderBehavior;
        [Inject]
        private SettingsManager settingsManager;
#pragma warning restore CS0649

        private bool showTrackers;
        private TrackerConfigData selectedTracker;
        private List<TrackerInstance> trackerInstances;
        private GameObject saberInstance;

        /// <summary>
        /// Begins showing the tracked devices
        /// </summary>
        public void EnableShowTrackers()
        {
            this.RemoveAllInstances();

            this.trackerInstances = this.trackedDeviceManager.TrackedDevices.Select((t) => new TrackerInstance
            {
                Instance = GameObject.Instantiate(this.assetLoaderBehavior.TrackerPrefab),
                InputDevice = t,
                Serial = t.serialNumber,
            }).ToList();

            this.trackerInstances.ForEach(t => t.Instance.SetActive(true));
            this.saberInstance = GameObject.Instantiate(this.assetLoaderBehavior.SaberPrefab);
            this.showTrackers = true;
            this.enabled = true;
        }

        /// <summary>
        /// Hides all trackers and prevents rendering
        /// </summary>
        public void DisableShowTrackers()
        {
            this.showTrackers = false;
            this.RemoveAllInstances();

            this.enabled = false;
        }

        /// <summary>
        /// Sets the given serial of the tracker as the currently selected tracker.
        /// The tracker will be drawn with a saber.  Send null or a non-existant
        /// serial to set nothing to be selected.
        /// </summary>
        /// <param name="serial">The serial of the tracker to set as selected</param>
        public void SetSelectedSerial(TrackerConfigData tracker)
        {
            this.selectedTracker = tracker;
        }

        private void Update()
        {
            if (!this.showTrackers || this.trackerInstances == null || this.trackerInstances.Count == 0) return;

            foreach (var tracker in this.trackerInstances)
            {
                // Update all the tracker poses
                Pose trackerPose = TrackedDeviceManager.GetDevicePose(tracker.InputDevice) ?? new Pose();
                trackerPose = this.AdjustForRoomRotation(trackerPose);

                tracker.Instance.transform.position = trackerPose.position;
                tracker.Instance.transform.rotation = trackerPose.rotation;
            }

            var selectedTrackerInstance = this.trackerInstances.Find(t => t.Serial == this.selectedTracker.Serial);
            if (this.selectedTracker == null || String.IsNullOrWhiteSpace(this.selectedTracker.Serial))
            {
                // No selected tracker so disable the saber
                this.saberInstance.SetActive(false);
                return;
            }

            // Transform the Saber according to the Tracker Config Data
            Pose selectedTrackerPose = new Pose(
                selectedTrackerInstance.Instance.transform.position,
                selectedTrackerInstance.Instance.transform.rotation);

            Pose pose = Utilities.CalculatePoseFromTrackerData(this.selectedTracker, selectedTrackerPose);
            this.saberInstance.transform.position = pose.position;
            this.saberInstance.transform.rotation = pose.rotation;

            this.saberInstance.SetActive(true);
        }

        /// <summary>
        /// Given any pose this method returns a new pose that is adjusted for the Beat Saber
        /// Room Rotation.
        /// </summary>
        private Pose AdjustForRoomRotation(Pose pose)
        {
            var roomCenter = this.settingsManager.settings.room.center;
            var roomRotation = Quaternion.Euler(0, this.settingsManager.settings.room.rotation, 0);

            Pose result = pose;
            result.position = roomRotation * pose.position;
            result.position += new Vector3(roomCenter.x, roomCenter.y, roomCenter.z);
            result.rotation = roomRotation * pose.rotation;
            return result;
        }

        // Deletes all the instances and all the locally stored tracker instances data
        private void RemoveAllInstances()
        {
            if (this.trackerInstances != null) this.trackerInstances.ForEach(t => GameObject.Destroy(t.Instance));
            this.trackerInstances = null;

            if (this.saberInstance != null) GameObject.Destroy(this.saberInstance);
            this.saberInstance = null;
        }


        private class TrackerInstance
        {
            public GameObject Instance { get; set; }
            public bool Selected { get; set; }
            public string Serial { get; set; }
            public InputDevice InputDevice { get; internal set; }
        }
    }
}
