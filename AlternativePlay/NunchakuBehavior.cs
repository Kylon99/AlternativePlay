using AlternativePlay.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlternativePlay
{
    public class NunchakuBehavior : MonoBehaviour
    {
        private enum Held
        {
            Both,
            Left,
            Right,
        }

        private const float NunchakuMass = 3.0f;
        private const float LinkMass = 1.0f;
        private const float AngularDrag = 2.0f;
        private const int LinkCount = 3;

        private List<GameObject> chain;
        private List<GameObject> links;

        private Held heldState;

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

            this.CreateNunchaku();
            this.links = Utilities.CreateLinkMeshes(this.chain.Count);
        }

        private void FixedUpdate()
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.Nunchaku)
            {
                // Do nothing if we aren't playing Nunchaku
                return;
            }

            // Apply gravity to the handles first
            float gravity = config.NunchakuGravity * -9.81f;
            foreach (var link in this.chain)
            {
                var rigidBody = link.GetComponent<Rigidbody>();
                rigidBody.AddForce(new Vector3(0, gravity, 0) * rigidBody.mass);
            }

            // Apply motion force from the controller
            var rightChain = this.chain.First();
            var leftChain = this.chain.Last();

            Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftNunchakuTracker);
            Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightNunchakuTracker);
            var leftSaberRigid = leftChain.GetComponent<Rigidbody>();
            var rightSaberRigid = rightChain.GetComponent<Rigidbody>();
            switch (this.heldState)
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
            var config = Configuration.instance.ConfigurationData;
            if (config.PlayMode != PlayMode.Nunchaku)
            {
                // Do nothing if we aren't playing Nunchaku
                return;
            }

            Utilities.MoveLinks(this.links, this.chain);

            var inputManager = BehaviorCatalog.instance.InputManager;

            bool bothTriggerClicked = inputManager.GetBothTriggerClicked();
            bool leftTriggerClicked = inputManager.GetLeftTriggerClicked();
            bool rightTriggerClicked = inputManager.GetRightTriggerClicked();

            if (bothTriggerClicked)
            {
                this.heldState = Held.Both;
            }
            else
            {
                if (leftTriggerClicked) this.heldState = Held.Left;
                if (rightTriggerClicked) this.heldState = Held.Right;
            }

            Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftNunchakuTracker);
            Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightNunchakuTracker);

            var rightChain = this.chain.First();
            var leftChain = this.chain.Last();

            var saberDevice = BehaviorCatalog.instance.SaberDeviceManager;
            switch (this.heldState)
            {
                case Held.Left:
                    // Move the left saber to the left controller position
                    leftSaberPose.rotation *= Quaternion.Euler(0.0f, 180.0f, 180.0f);
                    saberDevice.SetLeftSaberPose(leftSaberPose);

                    // Move the right saber to the end of the chain
                    Pose newRightSaberPose = new Pose(
                        rightChain.gameObject.transform.position / 10.0f,
                        rightChain.gameObject.transform.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f));
                    saberDevice.SetRightSaberPose(newRightSaberPose);
                    break;

                case Held.Right:
                    // Move the right saber to the right controller position
                    rightSaberPose.rotation *= Quaternion.Euler(0.0f, 180.0f, 180.0f);
                    saberDevice.SetRightSaberPose(rightSaberPose);

                    // Move the left saber to the end of the chain
                    Pose newLeftSaberPose = new Pose(
                        leftChain.gameObject.transform.position / 10.0f,
                        leftChain.gameObject.transform.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f));
                    saberDevice.SetLeftSaberPose(newLeftSaberPose);
                    break;

                case Held.Both:
                default:
                    // Move sabers to controller position
                    rightSaberPose.rotation *= Quaternion.Euler(0.0f, 180.0f, 180.0f);
                    saberDevice.SetRightSaberPose(rightSaberPose);

                    leftSaberPose.rotation *= Quaternion.Euler(0.0f, 180.0f, 180.0f);
                    saberDevice.SetLeftSaberPose(leftSaberPose);
                    break;
            }
        }

        private void CreateNunchaku()
        {
            var config = Configuration.instance.ConfigurationData;

            this.chain = new List<GameObject>();
            var rightHandle = Utilities.CreateLink("RightNunchaku", NunchakuMass, AngularDrag, true);
            this.chain.Add(rightHandle);

            for (int i = 0; i < LinkCount; i++)
            {
                var link = Utilities.CreateLink("NunchakuLink" + i.ToString(), LinkMass, AngularDrag);
                this.chain.Add(link);
            }

            var leftHandle = Utilities.CreateLink("LeftNunchaku", NunchakuMass, AngularDrag);
            this.chain.Add(leftHandle);

            Utilities.ConnectChain(this.chain, config.NunchakuLength / 100.0f);
        }
    }
}
