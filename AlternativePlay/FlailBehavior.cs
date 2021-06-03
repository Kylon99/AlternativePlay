using AlternativePlay.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlternativePlay
{
    public class FlailBehavior : MonoBehaviour
    {
        private const float BallMass = 3.0f;
        private const float LinkMass = 1.0f;
        private const float HandleMass = 2.0f;
        private const float AngularDrag = 2.0f;
        private const int LinkCount = 3;

        private bool removeLeftFlail;
        private bool removeRightFlail;

        private List<GameObject> leftHandle;
        private List<GameObject> rightHandle;
        private List<GameObject> leftLinks;
        private List<GameObject> rightLinks;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.Flail) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            var config = Configuration.instance.ConfigurationData;

            this.removeLeftFlail = config.OneColor && !config.UseLeftFlail && config.RemoveOtherSaber;
            this.removeRightFlail = config.OneColor && config.UseLeftFlail && config.RemoveOtherSaber;

            if (!this.removeLeftFlail)
            {
                Utilities.CheckAndDisableForTrackerTransforms(config.LeftFlailTracker);
                this.leftHandle = this.CreateFlail("Left", config.LeftFlailLength / 100.0f);
                this.leftLinks = Utilities.CreateLinkMeshes(this.leftHandle.Count);
            }

            if (!this.removeRightFlail)
            {
                Utilities.CheckAndDisableForTrackerTransforms(config.RightFlailTracker);
                this.rightHandle = this.CreateFlail("Right", config.RightFlailLength / 100.0f);
                this.rightLinks = Utilities.CreateLinkMeshes(this.rightHandle.Count);
            }
        }

        private void FixedUpdate()
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.Flail)
            {
                // Do nothing if we aren't playing Flail
                return;
            }

            float gravity = config.FlailGravity * -9.81f;

            if (!this.removeLeftFlail)
            {
                // Apply gravity to the left handle
                foreach (var link in this.leftHandle.Skip(1))
                {
                    var rigidBody = link.GetComponent<Rigidbody>();
                    rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                }
                // Apply motion force from the left controller
                var leftFirstLink = this.leftHandle.First();
                Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftFlailTracker);
                leftFirstLink.transform.position = leftSaberPose.position * 10.0f;
                leftFirstLink.transform.rotation = leftSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }

            if (!this.removeRightFlail)
            {
                // Apply gravity to the right handle
                foreach (var link in this.rightHandle.Skip(1))
                {
                    var rigidBody = link.GetComponent<Rigidbody>();
                    rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                }

                // Apply motion force from the right controller
                var rightFirstLink = this.rightHandle.First();
                Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightFlailTracker);
                rightFirstLink.transform.position = rightSaberPose.position * 10.0f;
                rightFirstLink.transform.rotation = rightSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
        }

        private void Update()
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.Flail)
            {
                // Do nothing if we aren't playing Flail
                return;
            }

            if (!this.removeLeftFlail)
            {
                var lastLeftLink = this.leftHandle.Last();
                Pose leftSaberPose = new Pose(lastLeftLink.transform.position / 10.0f, lastLeftLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaberPose(leftSaberPose);
                Utilities.MoveLinks(this.leftLinks, this.leftHandle);
            }

            if (!this.removeRightFlail)
            {
                var lastRightLink = this.rightHandle.Last();
                Pose rightSaberPose = new Pose(lastRightLink.transform.position / 10.0f, lastRightLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                BehaviorCatalog.instance.SaberDeviceManager.SetRightSaberPose(rightSaberPose);
                Utilities.MoveLinks(this.rightLinks, this.rightHandle);
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
