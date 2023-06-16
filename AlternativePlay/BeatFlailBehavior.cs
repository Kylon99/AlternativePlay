using AlternativePlay.Models;
using System.Collections;
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
        private readonly Pose leftHiddenPose = new Pose(new Vector3(-1.0f, -1000.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));
        private readonly Pose rightHiddenPose = new Pose(new Vector3(1.0f, -1000.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));

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
            if (Configuration.Current.PlayMode != PlayMode.BeatFlail) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            if (Configuration.Current.LeftFlailMode != BeatFlailMode.None)
            {
                Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.LeftTracker);
            }

            if (Configuration.Current.RightFlailMode != BeatFlailMode.None)
            {
                Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.RightTracker);
            }

            this.StartCoroutine(this.DisableSaberMeshes());
        }

        private void Awake()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.Current.PlayMode != PlayMode.BeatFlail) { return; }

            // Create the GameObjects used for physics calculations
            this.leftPhysicsFlail = this.CreatePhysicsChain("Left", Configuration.Current.LeftFlailLength / 100.0f);
            this.rightPhysicsFlail = this.CreatePhysicsChain("Right", Configuration.Current.RightFlailLength / 100.0f);
            this.leftLinkMeshes = Utilities.CreateLinkMeshes(this.leftPhysicsFlail.Count, Configuration.Current.LeftFlailLength / 100.0f);
            this.leftHandleMesh = GameObject.Instantiate(BehaviorCatalog.instance.AssetLoaderBehavior.FlailHandlePrefab);
            this.rightLinkMeshes = Utilities.CreateLinkMeshes(this.rightPhysicsFlail.Count, Configuration.Current.RightFlailLength / 100.0f);
            this.rightHandleMesh = GameObject.Instantiate(BehaviorCatalog.instance.AssetLoaderBehavior.FlailHandlePrefab);
        }

        private void FixedUpdate()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.Current.PlayMode != PlayMode.BeatFlail) { return; }

            float gravity = Configuration.Current.Gravity * -9.81f;

            switch(Configuration.Current.LeftFlailMode)
            {
                default:
                case BeatFlailMode.Flail:
                    // Apply gravity to the left handle
                    foreach (var link in this.leftPhysicsFlail.Skip(1))
                    {
                        var rigidBody = link.GetComponent<Rigidbody>();
                        rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                    }

                    // Apply motion force from the left controller
                    var leftFirstLink = this.leftPhysicsFlail.First();
                    Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(Configuration.Current.LeftTracker);
                    leftFirstLink.transform.position = leftSaberPose.position * 10.0f;
                    leftFirstLink.transform.rotation = leftSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    break;

                case BeatFlailMode.Sword:
                case BeatFlailMode.None:
                    // Do nothing
                    break;
            }

            switch (Configuration.Current.RightFlailMode)
            {
                default:
                case BeatFlailMode.Flail:
                    // Apply gravity to the right handle
                    foreach (var link in this.rightPhysicsFlail.Skip(1))
                    {
                        var rigidBody = link.GetComponent<Rigidbody>();
                        rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
                    }

                    // Apply motion force from the right controller
                    var rightFirstLink = this.rightPhysicsFlail.First();
                    Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(Configuration.Current.RightTracker);
                    rightFirstLink.transform.position = rightSaberPose.position * 10.0f;
                    rightFirstLink.transform.rotation = rightSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    break;

                case BeatFlailMode.Sword:
                case BeatFlailMode.None:
                    // Do nothing
                    break;
            }
        }

        private void Update()
        {
            // Do nothing if we aren't playing Flail
            if (Configuration.Current.PlayMode != PlayMode.BeatFlail) { return; }

            switch (Configuration.Current.LeftFlailMode)
            {
                default:
                case BeatFlailMode.Flail:
                    // Move saber to the last link
                    var lastLeftLink = this.leftPhysicsFlail.Last();
                    Pose leftLastLinkPose = new Pose(lastLeftLink.transform.position / 10.0f, lastLeftLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                    BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaberPose(leftLastLinkPose);

                    // Move all links into place
                    Utilities.MoveLinkMeshes(this.leftLinkMeshes, this.leftPhysicsFlail, (float)Configuration.Current.LeftFlailLength / 100f);

                    // Move handle based on the original saber position
                    Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(Configuration.Current.LeftTracker);
                    float oneChainDistance = Configuration.Current.LeftFlailLength / 100.0f / (this.leftPhysicsFlail.Count - 1);
                    Vector3 moveHandleUp = leftSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                    this.leftHandleMesh.transform.position = leftSaberPose.position + moveHandleUp;
                    this.leftHandleMesh.transform.rotation = leftSaberPose.rotation;
                    break;

                case BeatFlailMode.Sword:
                    // Do nothing
                    break;

                case BeatFlailMode.None:
                    // Remove the sword
                    BehaviorCatalog.instance.SaberDeviceManager.SetLeftSaberPose(this.leftHiddenPose);
                    break;
            }

            switch (Configuration.Current.RightFlailMode)
            {
                default:
                case BeatFlailMode.Flail:
                    // Move saber to the last link
                    var lastRightLink = this.rightPhysicsFlail.Last();
                    Pose rightLastLinkPose = new Pose(lastRightLink.transform.position / 10.0f, lastRightLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                    BehaviorCatalog.instance.SaberDeviceManager.SetRightSaberPose(rightLastLinkPose);

                    // Move all links into place
                    Utilities.MoveLinkMeshes(this.rightLinkMeshes, this.rightPhysicsFlail, Configuration.Current.RightFlailLength / 100.0f);

                    // Move handle based on the original saber position
                    Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(Configuration.Current.RightTracker);
                    float oneChainDistance = Configuration.Current.RightFlailLength / 100.0f / (this.rightPhysicsFlail.Count - 1);
                    Vector3 moveHandleUp = rightSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                    this.rightHandleMesh.transform.position = rightSaberPose.position + moveHandleUp;
                    this.rightHandleMesh.transform.rotation = rightSaberPose.rotation;
                    break;

                case BeatFlailMode.Sword:
                    // Do nothing
                    break;

                case BeatFlailMode.None:
                    // Remove the sword
                    BehaviorCatalog.instance.SaberDeviceManager.SetRightSaberPose(this.rightHiddenPose);
                    break;
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

        /// <summary>
        /// Disables the rendering of the saber
        /// </summary>
        private IEnumerator DisableSaberMeshes()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (Configuration.Current.LeftFlailMode == BeatFlailMode.None)
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableLeftSaberMesh();
            }

            if (Configuration.Current.RightFlailMode == BeatFlailMode.None)
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableRightSaberMesh();
            }
        }
    }
}