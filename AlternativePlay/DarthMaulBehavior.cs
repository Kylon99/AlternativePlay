using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        private SaberManager saberManager;

        public bool Split { get; private set; }

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Darth Maul
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul || this.saberManager == null) { return; }

            TrackedDeviceManager.instance.LoadTrackedDevices();

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.LeftMaulTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.instance.ConfigurationData.RightMaulTracker);
        }

        private void Awake()
        {
            this.saberManager = FindObjectOfType<SaberManager>();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul)
            {
                // Do nothing if we aren't playing Darth Maul
                return;
            }

            if (Configuration.instance.ConfigurationData.UseTriggerToSeparate)
            {
                // Check to see if the trigger has been pressed
                bool leftTriggerPressed = BehaviorCatalog.instance.InputManager.GetLeftTriggerClicked();
                bool rightTriggerPressed = BehaviorCatalog.instance.InputManager.GetRightTriggerClicked();

                if (leftTriggerPressed || rightTriggerPressed)
                {
                    this.Split = !this.Split;
                }
            }

            this.TransformSabers();
        }

        /// <summary>
        /// Track the Maul sabers using internal Beat Saber locations or the assigned trackers
        /// </summary>
        private void TransformSabers()
        {

            if (this.Split)
            {
                this.TransformForSplitDarthMaul();
                return;
            }

            switch (Configuration.instance.ConfigurationData.DarthMaulControllerCount)
            {
                case ControllerCountEnum.One:
                    this.TransformOneControllerMaul();
                    break;

                case ControllerCountEnum.Two:
                    this.TransformTwoControllerMaul();
                    break;

                default:
                    // Do nothing
                    break;
            }
        }

        /// <summary>
        /// Tracks the sabers for when Darth Maul mode is split into two swords
        /// </summary>
        private void TransformForSplitDarthMaul()
        {
            var config = Configuration.instance.ConfigurationData;

            Pose leftSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftMaulTracker);
            this.saberManager.leftSaber.transform.position = leftSaberPose.position;
            this.saberManager.leftSaber.transform.rotation = leftSaberPose.rotation;

            Pose rightSaberPose = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightMaulTracker);
            this.saberManager.rightSaber.transform.position = rightSaberPose.position;
            this.saberManager.rightSaber.transform.rotation = rightSaberPose.rotation;
        }

        /// <summary>
        /// Moves the maul sabers based on a one controller scheme
        /// </summary>
        private void TransformOneControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;

            // Determine which is the held normally 'base' saber and which is the 'other' saber that must be moved
            Func<TrackerConfigData, Pose> getBaseController;
            TrackerConfigData baseTrackerConfig;
            Saber baseSaber;
            Saber otherSaber;

            if (config.UseLeftController)
            {
                getBaseController = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose;
                baseTrackerConfig = config.LeftMaulTracker;
                baseSaber = this.saberManager.leftSaber;
                otherSaber = this.saberManager.rightSaber;
            }
            else
            {
                getBaseController = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose;
                baseTrackerConfig = config.RightMaulTracker;
                baseSaber = this.saberManager.rightSaber;
                otherSaber = this.saberManager.leftSaber;
            }
            var rotateSaber = config.ReverseMaulDirection ? baseSaber : otherSaber;

            // Move the other saber to the base saber
            Pose basePose = getBaseController(baseTrackerConfig);
            baseSaber.transform.position = basePose.position;
            baseSaber.transform.rotation = basePose.rotation;
            otherSaber.transform.position = basePose.position;
            otherSaber.transform.rotation = basePose.rotation;

            // Rotate the 'opposite' saber 180 degrees around
            rotateSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            rotateSaber.transform.Translate(0.0f, 0.0f, sep * 2.0f, Space.Self);
        }

        /// <summary>
        /// Moves the maul sabers based on a two controller scheme
        /// </summary>
        private void TransformTwoControllerMaul()
        {
            var config = Configuration.instance.ConfigurationData;
            float sep = 1.0f * config.MaulDistance / 100.0f;

            // Determine Hand positions
            Pose leftHand = BehaviorCatalog.instance.SaberDeviceManager.GetLeftSaberPose(config.LeftMaulTracker);
            Pose rightHand = BehaviorCatalog.instance.SaberDeviceManager.GetRightSaberPose(config.RightMaulTracker);

            // Determine final saber positions and rotations
            Vector3 middlePos = (rightHand.position + leftHand.position) * 0.5f;
            Vector3 forward = (rightHand.position - leftHand.position).normalized;
            Vector3 rightHandUp = rightHand.rotation * Vector3.up;

            Saber forwardSaber = config.ReverseMaulDirection ? this.saberManager.leftSaber : this.saberManager.rightSaber;
            Saber backwardSaber = config.ReverseMaulDirection ? this.saberManager.rightSaber : this.saberManager.leftSaber;

            forwardSaber.transform.position = middlePos + (forward * sep);
            backwardSaber.transform.position = middlePos + (-forward * sep);
            forwardSaber.transform.rotation = Quaternion.LookRotation(forward, rightHandUp);
            backwardSaber.transform.rotation = Quaternion.LookRotation(-forward, -rightHandUp);
        }
    }

}
