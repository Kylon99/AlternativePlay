using AlternativePlay.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatFlailBehavior : MonoBehaviour
    {
        private const float BallMass = 3.0f;
        private const float LinkMass = 1.0f;
        private const float HandleMass = 2.0f;
        private const float AngularDrag = 2.0f;
        private const int LinkCount = 3;

        private bool removeLeftFlail;
        private bool removeRightFlail;

        private List<GameObject> leftPhysicsFlail;
        private List<GameObject> rightPhysicsFlail;

        private List<GameObject> leftLinkMeshes;
        private List<GameObject> rightLinkMeshes;
        private GameObject leftHandleMesh;
        private GameObject rightHandleMesh;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatFlail) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            var config = Configuration.instance.ConfigurationData;
            this.removeLeftFlail = config.OneColor && !config.UseLeftFlail && config.RemoveOtherSaber;
            this.removeRightFlail = config.OneColor && config.UseLeftFlail && config.RemoveOtherSaber;

            if (!this.removeLeftFlail)
            {
                Utilities.CheckAndDisableForTrackerTransforms(config.LeftFlailTracker);
            }

            if (!this.removeRightFlail)
            {
                Utilities.CheckAndDisableForTrackerTransforms(config.RightFlailTracker);
            }
        }

        private void Awake()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatFlail) { return; }

            // Create the GameObjects used for physics calculations
            var config = Configuration.instance.ConfigurationData;
            this.leftPhysicsFlail = this.CreatePhysicsChain("Left", config.LeftFlailLength / 100.0f);
            this.rightPhysicsFlail = this.CreatePhysicsChain("Right", config.RightFlailLength / 100.0f);
            this.leftLinkMeshes = Utilities.CreateLinkMeshes(this.leftPhysicsFlail.Count, config.LeftFlailLength / 100.0f);
            this.leftHandleMesh = GameObject.Instantiate(BehaviorCatalog.instance.AssetLoaderBehavior.FlailHandlePrefab);
            this.rightLinkMeshes = Utilities.CreateLinkMeshes(this.rightPhysicsFlail.Count, config.RightFlailLength / 100.0f);
            this.rightHandleMesh = GameObject.Instantiate(BehaviorCatalog.instance.AssetLoaderBehavior.FlailHandlePrefab);
        }

        private void FixedUpdate()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatFlail) { return; }

            var config = Configuration.instance.ConfigurationData;
            float gravity = config.FlailGravity * -9.81f;

            if (!this.removeLeftFlail)
            {
                // Apply gravity to the left handle
                foreach (var link in this.leftPhysicsFlail.Skip(1))
                {
                    var rigidBody = link.GetComponent<Rigidbody>();
                    rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                }
                // Apply motion force from the left controller
                var leftFirstLink = this.leftPhysicsFlail.First();
                Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftFlailTracker);
                leftFirstLink.transform.position = leftSaberPose.position * 10.0f;
                leftFirstLink.transform.rotation = leftSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }

            if (!this.removeRightFlail)
            {
                // Apply gravity to the right handle
                foreach (var link in this.rightPhysicsFlail.Skip(1))
                {
                    var rigidBody = link.GetComponent<Rigidbody>();
                    rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                }

                // Apply motion force from the right controller
                var rightFirstLink = this.rightPhysicsFlail.First();
                Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightFlailTracker);
                rightFirstLink.transform.position = rightSaberPose.position * 10.0f;
                rightFirstLink.transform.rotation = rightSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
        }

        private void Update()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.BeatFlail) { return; }

            var config = Configuration.instance.ConfigurationData;
            if (!this.removeLeftFlail)
            {
                var lastLeftLink = this.leftPhysicsFlail.Last();
                Pose leftLastLinkPose = new Pose(lastLeftLink.transform.position / 10.0f, lastLeftLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaberPose(leftLastLinkPose);
                Utilities.MoveLinkMeshes(this.leftLinkMeshes, this.leftPhysicsFlail, (float)config.LeftFlailLength / 100f);

                Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftFlailTracker);
                float oneChainDistance = config.LeftFlailLength / 100.0f / (leftPhysicsFlail.Count - 1);
                Vector3 moveHandleUp = leftSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                this.leftHandleMesh.transform.position = leftSaberPose.position + moveHandleUp;
                this.leftHandleMesh.transform.rotation = leftSaberPose.rotation;
            }

            if (!this.removeRightFlail)
            {
                var lastRightLink = this.rightPhysicsFlail.Last();
                Pose rightLastLinkPose = new Pose(lastRightLink.transform.position / 10.0f, lastRightLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                BehaviorCatalog.instance.SaberDeviceManager.SetRightSaberPose(rightLastLinkPose);
                Utilities.MoveLinkMeshes(this.rightLinkMeshes, this.rightPhysicsFlail, config.RightFlailLength / 100.0f);

                Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightFlailTracker);
                float oneChainDistance = config.RightFlailLength / 100.0f / (rightPhysicsFlail.Count - 1);
                Vector3 moveHandleUp = rightSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                this.rightHandleMesh.transform.position = rightSaberPose.position + moveHandleUp;
                this.rightHandleMesh.transform.rotation = rightSaberPose.rotation;
            }
        }

        private void OnDestroy()
        {
            // Destroy all flail game objects
            if (this.leftPhysicsFlail != null) this.leftPhysicsFlail.ForEach(o => GameObject.Destroy(o));
            this.leftPhysicsFlail = null;

            if (this.rightPhysicsFlail != null) this.rightPhysicsFlail.ForEach(o => GameObject.Destroy(o));
            this.rightPhysicsFlail = null;

            if (this.leftLinkMeshes != null) this.leftLinkMeshes.ForEach(o => GameObject.Destroy(o));
            this.leftLinkMeshes = null;

            if (this.rightLinkMeshes != null) this.rightLinkMeshes.ForEach(o => GameObject.Destroy(o));
            this.rightLinkMeshes = null;

            if (this.leftHandleMesh != null) GameObject.Destroy(this.leftHandleMesh);
            this.leftHandleMesh = null;

            if (this.rightHandleMesh != null) GameObject.Destroy(this.rightHandleMesh);
            this.rightHandleMesh = null;
        }

        /// <summary>
        /// Creates a chain of GameObjects used for physics and connects them
        /// </summary>
        private List<GameObject> CreatePhysicsChain(string prefix, float length)
        {
            var chain = new List<GameObject>();
            var handle = Utilities.CreateLink(prefix + "FlailHandle", HandleMass, AngularDrag, true);
            chain.Add(handle);

            for (int i = 0; i < LinkCount; i++)
            {
                var link = Utilities.CreateLink(prefix + "FlailLink" + i.ToString(), LinkMass, AngularDrag);
                chain.Add(link);
            }

            var ball = Utilities.CreateLink(prefix + "FlailBall", BallMass, AngularDrag);
            chain.Add(ball);

            Utilities.ConnectChain(chain, length);
            return chain;
        }
    }
}