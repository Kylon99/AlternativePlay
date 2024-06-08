using AlternativePlay.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

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

        [Inject]
        private Configuration configuration;
        [Inject]
        private TrackedDeviceManager trackedDeviceManager;
        [Inject]
        private SaberDeviceManager saberDeviceManager;
        [Inject]
        private AssetLoaderBehavior assetLoaderBehavior;

        private List<GameObject> leftPhysicsFlail;
        private List<GameObject> rightPhysicsFlail;

        private List<GameObject> leftLinkMeshes;
        private List<GameObject> rightLinkMeshes;
        private GameObject leftHandleMesh;
        private GameObject rightHandleMesh;

        private void Start()
        {
            // Do nothing if we aren't playing Flail
            if (this.configuration.Current.PlayMode != PlayMode.BeatFlail) { return; }

            // Create the GameObjects for the flails
            if (this.configuration.Current.LeftFlailMode == BeatFlailMode.Flail) 
            {
                this.leftPhysicsFlail = this.CreatePhysicsChain("Left", this.configuration.Current.LeftFlailLength / 100.0f);
                this.leftLinkMeshes = Utilities.CreateLinkMeshes(this.leftPhysicsFlail.Count, this.configuration.Current.LeftFlailLength / 100.0f);
                this.leftHandleMesh = this.CreateFlailHandle("LeftHandle", this.configuration.Current.LeftHandleLength / 100.0f);
            }

            if (this.configuration.Current.RightFlailMode == BeatFlailMode.Flail)
            {
                this.rightPhysicsFlail = this.CreatePhysicsChain("Right", this.configuration.Current.RightFlailLength / 100.0f);
                this.rightLinkMeshes = Utilities.CreateLinkMeshes(this.rightPhysicsFlail.Count, this.configuration.Current.RightFlailLength / 100.0f);
                this.rightHandleMesh = this.CreateFlailHandle("RightHandle", this.configuration.Current.RightHandleLength / 100.0f);
            }

            this.trackedDeviceManager.LoadTrackedDevices();

            if (this.configuration.Current.LeftFlailMode != BeatFlailMode.None)
            {
                Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.LeftTracker);
            }

            if (this.configuration.Current.RightFlailMode != BeatFlailMode.None)
            {
                Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.RightTracker);
            }

            this.StartCoroutine(this.DisableSaberMeshes());
        }

        private void FixedUpdate()
        {
            // Do nothing if we aren't playing Flail
            if (this.configuration.Current.PlayMode != PlayMode.BeatFlail) { return; }

            float gravity = this.configuration.Current.Gravity * -9.81f;

            switch (this.configuration.Current.LeftFlailMode)
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
                    Pose leftSaberPose = this.saberDeviceManager.GetLeftSaberPose(this.configuration.Current.LeftTracker);
                    leftFirstLink.transform.position = leftSaberPose.position * 10.0f;
                    leftFirstLink.transform.rotation = leftSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    break;

                case BeatFlailMode.Sword:
                case BeatFlailMode.None:
                    // Do nothing
                    break;
            }

            switch (this.configuration.Current.RightFlailMode)
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
                    Pose rightSaberPose = this.saberDeviceManager.GetRightSaberPose(this.configuration.Current.RightTracker);
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
            if (this.configuration.Current.PlayMode != PlayMode.BeatFlail) { return; }

            switch (this.configuration.Current.LeftFlailMode)
            {
                default:
                case BeatFlailMode.Flail:
                    // Move saber to the last link
                    var lastLeftLink = this.leftPhysicsFlail.Last();
                    Pose leftLastLinkPose = new Pose(lastLeftLink.transform.position / 10.0f, lastLeftLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                    this.saberDeviceManager.SetLeftSaberPose(leftLastLinkPose);

                    // Move all links into place
                    Utilities.MoveLinkMeshes(this.leftLinkMeshes, this.leftPhysicsFlail, (float)this.configuration.Current.LeftFlailLength / 100f);

                    // Move handle based on the original saber position
                    Pose leftSaberPose = this.saberDeviceManager.GetLeftSaberPose(this.configuration.Current.LeftTracker);
                    float oneChainDistance = this.configuration.Current.LeftFlailLength / 100.0f / (this.leftPhysicsFlail.Count - 1);
                    Vector3 moveHandleUp = leftSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                    this.leftHandleMesh.transform.position = leftSaberPose.position + moveHandleUp;
                    this.leftHandleMesh.transform.rotation = leftSaberPose.rotation;

                   break;

                case BeatFlailMode.Sword:
                    // Do nothing
                    break;

                case BeatFlailMode.None:
                    // Remove the sword
                    this.saberDeviceManager.SetLeftSaberPose(this.leftHiddenPose);
                    break;
            }

            switch (this.configuration.Current.RightFlailMode)
            {
                default:
                case BeatFlailMode.Flail:
                    // Move saber to the last link
                    var lastRightLink = this.rightPhysicsFlail.Last();
                    Pose rightLastLinkPose = new Pose(lastRightLink.transform.position / 10.0f, lastRightLink.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 180.0f));
                    this.saberDeviceManager.SetRightSaberPose(rightLastLinkPose);

                    // Move all links into place
                    Utilities.MoveLinkMeshes(this.rightLinkMeshes, this.rightPhysicsFlail, this.configuration.Current.RightFlailLength / 100.0f);

                    // Move handle based on the original saber position
                    Pose rightSaberPose = this.saberDeviceManager.GetRightSaberPose(this.configuration.Current.RightTracker);
                    float oneChainDistance = this.configuration.Current.RightFlailLength / 100.0f / (this.rightPhysicsFlail.Count - 1);
                    Vector3 moveHandleUp = rightSaberPose.rotation * new Vector3(0.0f, 0.0f, oneChainDistance); // Move handle forward one chain length
                    this.rightHandleMesh.transform.position = rightSaberPose.position + moveHandleUp;
                    this.rightHandleMesh.transform.rotation = rightSaberPose.rotation;
                    break;

                case BeatFlailMode.Sword:
                    // Do nothing
                    break;

                case BeatFlailMode.None:
                    // Remove the sword
                    this.saberDeviceManager.SetRightSaberPose(this.rightHiddenPose);
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

            if (this.configuration.Current.LeftFlailMode == BeatFlailMode.None)
            {
                this.saberDeviceManager.DisableLeftSaberMesh();
            }

            if (this.configuration.Current.RightFlailMode == BeatFlailMode.None)
            {
                this.saberDeviceManager.DisableRightSaberMesh();
            }
        }

        /// <summary>
        /// Create a segmented variable length flail handle
        /// </summary>
        /// <param name="name">The name for the parent handle GameObject</param>
        /// <param name="flailTotalLength">The length of the handle in meters</param>
        /// <returns>The parent GameObject for the flail handle</returns>
        private GameObject CreateFlailHandle(string name, float flailTotalLength)
        {
            const float flailSegmentLength = 0.10f; // 10 cm or 0.1m
            const float topCapLength = 0.063f; // 6.3cm
            const float bottomCapLength = 0.01f; // 1 cm

            int segmentCount = (int)(flailTotalLength / flailSegmentLength);

            var assetLoader = this.assetLoaderBehavior;

            // Instantiate One-Off Game Objects
            var handle = new GameObject(name);
            var topCap = GameObject.Instantiate(assetLoader.FlailTopCapPrefab, Vector3.zero, Quaternion.identity, handle.transform);
            var bottomCapPosition = new Vector3(0.0f, 0.0f, (flailSegmentLength * segmentCount * -1.0f) - topCapLength - bottomCapLength);
            var bottomCap = GameObject.Instantiate(assetLoader.FlailBottomCapPrefab, bottomCapPosition, Quaternion.identity, handle.transform);

            // Instantiate Handle Segments
            var segmentList = Enumerable.Range(0, segmentCount).Select(i =>
            {
                float zPosition = (flailSegmentLength * i * -1.0f) - topCapLength;
                var positionRelative = new Vector3(0.0f, 0.0f, zPosition);
                var gameObject = GameObject.Instantiate(assetLoader.FlailHandleSegmentPrefab, positionRelative, Quaternion.identity, handle.transform);

                return new { GameObject = gameObject, Index = i };

            }).ToList();
            segmentList.ForEach(segment => this.remapSegmentUV(segment.GameObject, segment.Index));

            return handle;
        }

        /// <summary>
        /// Remaps the UV coordinates in the U direction based on the MOD 4 of the index.
        /// </summary>
        private void remapSegmentUV(GameObject segment, int index)
        {
            const int segmentsPerTexture = 4;

            Mesh mesh = segment.GetComponent<MeshFilter>().mesh;
            var uvList = new List<Vector2>();
            mesh.GetUVs(0, uvList);

            var modifiedUV = uvList.Select(uv =>
            {
                float correctedX = (uv.x < 0.01f && uv.x > -0.01f) ? 0.0f : 0.25f;
                float multipleX = 0.25f * (index % segmentsPerTexture);
                return new Vector2(correctedX + multipleX, uv.y);
            }).ToList();

            mesh.SetUVs(0, modifiedUV);
        }
    }
}