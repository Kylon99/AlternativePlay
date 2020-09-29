using AlternativePlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay
{
    public class ShowTrackersBehavior : MonoBehaviour
    {
        private const float trackerSize = 0.15f;
        private readonly Color selectedColor = new Color(1.0f, 0.0f, 0.0f, 0.0f);
        private readonly Color trackerColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);

        private bool showTrackers;
        private Material selectedMaterial;
        private Material trackerMaterial;
        private GameObject trackerSphere;
        private List<TrackerInstance> trackerInstances;

        /// <summary>
        /// Sets the given serial of the tracker as the currently selected tracker.
        /// The tracker will be drawn in a different color.  Send null or a non-existant
        /// serial to set nothing to be selected.
        /// </summary>
        /// <param name="serial">The serial of the tracker to set as selected</param>
        public void SetSelectedSerial(string serial)
        {
            if (trackerInstances == null) return;

            foreach (var tracker in trackerInstances)
            {
                Material setMaterial = tracker.Serial == serial ? this.selectedMaterial : this.trackerMaterial;
                tracker.Instance.GetComponentsInChildren<Renderer>().ToList().ForEach(c => c.material = setMaterial);
            }
        }

        /// <summary>
        /// Hides all trackers and prevents rendering
        /// </summary>
        public void HideTrackers()
        {
            showTrackers = false;
            RemoveAllInstances();
        }

        /// <summary>
        /// Begins showing the tracked devices
        /// </summary>
        public void ShowTrackers()
        {
            RemoveAllInstances();

            trackerInstances = TrackedDeviceManager.instance.TrackedDevices.Select((t) => new TrackerInstance
            {
                Instance = GameObject.Instantiate(this.trackerSphere),
                InputDevice = t,
                Serial = t.serialNumber,
            }).ToList();

            trackerInstances.ForEach(t => t.Instance.SetActive(true));
            showTrackers = true;
        }

        private void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("AlternativePlay.Resources.alternativeplaymodels"));
            AlternativePlay.Logger.Info($"Asset Bundle Objects {String.Join(", ", assetBundle.GetAllAssetNames())}");
            this.trackerSphere = assetBundle.LoadAsset<GameObject>("APTracker");
            this.trackerMaterial = assetBundle.LoadAsset<Material>("APTrackerNotSelected");
            this.selectedMaterial = assetBundle.LoadAsset<Material>("APTrackerSelected");

            // Create the materials to be used later
            //this.trackerMaterial = new Material(Shader.Find("Standard"));
            //this.trackerMaterial.SetColor("_Color", this.trackerColor);

            //this.selectedMaterial = new Material(Shader.Find("Standard"));
            //this.selectedMaterial.SetColor("_Color", this.selectedColor);

            //// Create the sphere and set to the tracker material
            //this.trackerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //this.trackerSphere.transform.localScale = new Vector3(trackerSize, trackerSize, trackerSize);
            //this.trackerSphere.GetComponentsInChildren<Renderer>().ToList().ForEach(c => c.material = this.trackerMaterial);
            //this.trackerSphere.SetActive(false);
        }

        private void Update()
        {
            if (!showTrackers || trackerInstances == null || trackerInstances.Count == 0) return;

            foreach (var tracker in trackerInstances)
            {
                // Update all the tracker poses
                bool positionSuccess = tracker.InputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
                if (positionSuccess) tracker.Instance.transform.position = position;

                bool rotationSuccess = tracker.InputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
                if (rotationSuccess) tracker.Instance.transform.rotation = rotation;

            }
        }

        // Deletes all the instances and all the locally stored tracker instances data
        private void RemoveAllInstances()
        {
            if (trackerInstances == null) return;

            trackerInstances.ForEach(t => GameObject.Destroy(t.Instance));
            trackerInstances = null;
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
