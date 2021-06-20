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

        private List<GameObject> leftFlail;
        private List<GameObject> rightFlail;
        private List<GameObject> leftLinks;
        private List<GameObject> rightLinks;

        private GameObject leftHandle;
        private GameObject rightHandle;

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

            this.leftFlail.ForEach(go => go.SetActive(!this.removeLeftFlail));
            this.leftLinks.ForEach(go => go.SetActive(!this.removeLeftFlail));

            if (!this.removeRightFlail)
            {
                Utilities.CheckAndDisableForTrackerTransforms(config.RightFlailTracker);
            }

            this.rightFlail.ForEach(go => go.SetActive(!this.removeRightFlail));
            this.rightLinks.ForEach(go => go.SetActive(!this.removeRightFlail));
        }

        private void Awake()
        {
            var config = Configuration.instance.ConfigurationData;
            this.leftFlail = this.CreateFlail("Left", config.LeftFlailLength / 100.0f);
            this.rightFlail = this.CreateFlail("Right", config.RightFlailLength / 100.0f);
            this.leftLinks = Utilities.CreateLinkMeshes(this.leftFlail.Count, config.LeftFlailLength / 100.0f);
            this.rightLinks = Utilities.CreateLinkMeshes(this.rightFlail.Count, config.RightFlailLength / 100.0f);
            this.leftHandle = GameObject.Instantiate(BehaviorCatalog.instance.AssetLoaderBehavior.FlailHandlePrefab);
            this.rightHandle = GameObject.Instantiate(BehaviorCatalog.instance.AssetLoaderBehavior.FlailHandlePrefab);
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
                foreach (var link in this.leftFlail.Skip(1))
                {
                    var rigidBody = link.GetComponent<Rigidbody>();
                    rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                }
                // Apply motion force from the left controller
                var leftFirstLink = this.leftFlail.First();
                Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftFlailTracker);
                leftFirstLink.transform.position = leftSaberPose.position * 10.0f;
                leftFirstLink.transform.rotation = leftSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }

            if (!this.removeRightFlail)
            {
                // Apply gravity to the right handle
                foreach (var link in this.rightFlail.Skip(1))
                {
                    var rigidBody = link.GetComponent<Rigidbody>();
                    rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                }

                // Apply motion force from the right controller
                var rightFirstLink = this.rightFlail.First();
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
                var lastLeftLink = this.leftFlail.Last();
                Pose leftLastLinkPose = new Pose(lastLeftLink.transform.position / 10.0f, lastLeftLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaberPose(leftLastLinkPose);
                Utilities.MoveLinkMeshes(this.leftLinks, this.leftFlail, (float)config.LeftFlailLength / 100f);

                Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftFlailTracker);
                float oneChainDistance = config.LeftFlailLength / 100.0f / (leftFlail.Count - 1);
                Vector3 moveHandleUp = leftSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                this.leftHandle.transform.position = leftSaberPose.position + moveHandleUp;
                this.leftHandle.transform.rotation = leftSaberPose.rotation;
            }

            if (!this.removeRightFlail)
            {
                var lastRightLink = this.rightFlail.Last();
                Pose rightLastLinkPose = new Pose(lastRightLink.transform.position / 10.0f, lastRightLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                BehaviorCatalog.instance.SaberDeviceManager.SetRightSaberPose(rightLastLinkPose);
                Utilities.MoveLinkMeshes(this.rightLinks, this.rightFlail, config.RightFlailLength / 100.0f);

                Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightFlailTracker);
                float oneChainDistance = config.RightFlailLength / 100.0f / (rightFlail.Count - 1);
                Vector3 moveHandleUp = rightSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                this.rightHandle.transform.position = rightSaberPose.position + moveHandleUp;
                this.rightHandle.transform.rotation = rightSaberPose.rotation;
            }
        }

        /// <summary>
        /// Creates a flail chain sequence and connects them
        /// </summary>
        private List<GameObject> CreateFlail(string prefix, float length)
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
