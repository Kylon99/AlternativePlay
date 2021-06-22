using AlternativePlay.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlternativePlay
{
    public class NunchakuBehavior : MonoBehaviour
    {
        public enum Held
        {
            Both,
            Left,
            Right,
        }

        public Held HeldState { get; private set; }

        private const float NunchakuMass = 3.0f;
        private const float LinkMass = 1.0f;
        private const float AngularDrag = 2.0f;
        private const int LinkCount = 3;

        private List<GameObject> physicsChain;
        private List<GameObject> linkMeshes;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Nunchaku
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.Nunchaku) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            var config = Configuration.instance.ConfigurationData;
            Utilities.CheckAndDisableForTrackerTransforms(config.LeftNunchakuTracker);
            Utilities.CheckAndDisableForTrackerTransforms(config.RightNunchakuTracker);
        }

        private void Awake()
        {
            // Do nothing if we aren't playing Nunchaku
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.Nunchaku) { return; }

            // Create all Nunchaku GameObjects
            this.physicsChain = this.CreateNunchaku();
            this.linkMeshes = Utilities.CreateLinkMeshes(this.physicsChain.Count, Configuration.instance.ConfigurationData.NunchakuLength / 100.0f);
        }

        private void FixedUpdate()
        {
            // Do nothing if we aren't playing Nunchaku
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.Nunchaku) { return; }

            // Apply gravity to the handles first
            var config = Configuration.instance.ConfigurationData;
            float gravity = config.NunchakuGravity * -9.81f;
            foreach (var link in this.physicsChain)
            {
                var rigidBody = link.GetComponent<Rigidbody>();
                rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
            }

            // Apply motion force from the controller
            var rightChain = this.physicsChain.First();
            var leftChain = this.physicsChain.Last();

            Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftNunchakuTracker);
            Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightNunchakuTracker);
            var leftSaberRigid = leftChain.GetComponent<Rigidbody>();
            var rightSaberRigid = rightChain.GetComponent<Rigidbody>();
            switch (this.HeldState)
            {
                case Held.Left:
                    leftSaberRigid.isKinematic = true;
                    rightSaberRigid.isKinematic = false;

                    leftChain.gameObject.transform.position = leftSaberPose.position * 10.0f;
                    leftChain.gameObject.transform.rotation = leftSaberPose.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                    break;

                case Held.Right:
                    leftSaberRigid.isKinematic = false;
                    rightSaberRigid.isKinematic = true;

                    rightChain.gameObject.transform.position = rightSaberPose.position * 10.0f;
                    rightChain.gameObject.transform.rotation = rightSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    break;

                case Held.Both:
                default:
                    leftSaberRigid.isKinematic = true;
                    rightSaberRigid.isKinematic = true;

                    leftChain.gameObject.transform.position = leftSaberPose.position * 10.0f;
                    leftChain.gameObject.transform.rotation = leftSaberPose.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f);
                    rightChain.gameObject.transform.position = rightSaberPose.position * 10.0f;
                    rightChain.gameObject.transform.rotation = rightSaberPose.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    break;
            }
        }

        private void Update()
        {
            // Do nothing if we aren't playing Nunchaku
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.Nunchaku) { return; }

            var config = Configuration.instance.ConfigurationData;

            // Resolve Trigger button presses
            var inputManager = BehaviorCatalog.instance.InputManager;

            bool bothTriggerClicked = inputManager.GetBothTriggerClicked();
            bool leftTriggerClicked = inputManager.GetLeftTriggerClicked();
            bool rightTriggerClicked = inputManager.GetRightTriggerClicked();

            if (bothTriggerClicked)
            {
                this.HeldState = Held.Both;
            }
            else
            {
                if (leftTriggerClicked) this.HeldState = Held.Left;
                if (rightTriggerClicked) this.HeldState = Held.Right;
            }

            // Move the link meshes first
            Utilities.MoveLinkMeshes(this.linkMeshes, this.physicsChain, config.NunchakuLength / 100.0f);

            // Move the Sabers into place
            Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftNunchakuTracker);
            Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightNunchakuTracker);

            var rightChain = this.physicsChain.First();
            var leftChain = this.physicsChain.Last();

            Pose newLeftSaberPose;
            Pose newRightSaberPose;

            switch (this.HeldState)
            {
                case Held.Left:
                    // Move the left saber to the left controller position and right saber to the end of the chain
                    Pose rightChainPose = new Pose(
                        rightChain.gameObject.transform.position / 10.0f,
                        rightChain.gameObject.transform.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f));

                    newLeftSaberPose = config.ReverseNunchaku ? rightChainPose : leftSaberPose.Reverse();
                    newRightSaberPose = config.ReverseNunchaku ? leftSaberPose.Reverse() : rightChainPose;
                    break;

                case Held.Right:
                    // Move the right saber to the right controller position and the left saber to the end of the chain
                    Pose leftChainPose = new Pose(
                        leftChain.gameObject.transform.position / 10.0f,
                        leftChain.gameObject.transform.rotation * Quaternion.Euler(0.0f, -90.0f, 0.0f));

                    newRightSaberPose = config.ReverseNunchaku ? leftChainPose : rightSaberPose.Reverse();
                    newLeftSaberPose = config.ReverseNunchaku ? rightSaberPose.Reverse() : leftChainPose;
                    break;

                case Held.Both:
                default:
                    // Move sabers to controller position
                    newRightSaberPose = rightSaberPose.Reverse();
                    newLeftSaberPose = leftSaberPose.Reverse();
                    break;
            }

            // Add an adjustment to the left saber
            Vector3 oneChainDistance = new Vector3(0.0f, 0.0f, config.NunchakuLength / 100.0f / (this.physicsChain.Count - 1));
            if (config.ReverseNunchaku && this.HeldState != Held.Both)
            {
                newRightSaberPose.position += (newRightSaberPose.rotation * oneChainDistance);
            }
            else
            {
                newLeftSaberPose.position += (newLeftSaberPose.rotation * oneChainDistance);
            }

            var saberDevice = BehaviorCatalog.instance.SaberDeviceManager;
            saberDevice.SetLeftSaberPose(newLeftSaberPose);
            saberDevice.SetRightSaberPose(newRightSaberPose);
        }

        private void OnDestroy()
        {
            if (this.physicsChain != null) this.physicsChain.ForEach(o => GameObject.Destroy(o));
            if (this.linkMeshes != null) this.linkMeshes.ForEach(o => GameObject.Destroy(o));
        }

        private List<GameObject> CreateNunchaku()
        {
            var config = Configuration.instance.ConfigurationData;

            var chain = new List<GameObject>();
            var rightHandle = Utilities.CreateLink("RightNunchaku", NunchakuMass, AngularDrag, true);
            chain.Add(rightHandle);

            for (int i = 0; i < LinkCount; i++)
            {
                var link = Utilities.CreateLink("NunchakuLink" + i.ToString(), LinkMass, AngularDrag);
                chain.Add(link);
            }

            var leftHandle = Utilities.CreateLink("LeftNunchaku", NunchakuMass, AngularDrag);
            chain.Add(leftHandle);

            Utilities.ConnectChain(chain, config.NunchakuLength / 100.0f);
            return chain;
        }
    }
}