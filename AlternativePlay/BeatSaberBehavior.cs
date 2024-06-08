using AlternativePlay.Models;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
        [Inject]
        private Configuration configuration;
        [Inject]
        private SaberDeviceManager saberDeviceManager;

        private readonly Pose hiddenPose = new Pose(new Vector3(0.0f, -1000.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));

        private void Start()
        {
            // Do nothing if we aren't playing Beat Saber
            if (this.configuration.Current.PlayMode != PlayMode.BeatSaber) { return; }

            Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.LeftTracker);
            Utilities.CheckAndDisableForTrackerTransforms(this.configuration.Current.RightTracker);

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Update()
        {
            if (this.configuration.Current.PlayMode != PlayMode.BeatSaber) return;

            this.UpdateLeftSaber();
            this.UpdateRightSaber();
        }

        private void UpdateLeftSaber()
        {
            if (this.configuration.Current.OneColor && this.configuration.Current.RemoveOtherSaber && !this.configuration.Current.UseLeft)
            {
                // Move the other saber away since there's a bug in the base game which makes it
                // able to cut bombs still
                this.saberDeviceManager.SetLeftSaberPose(this.hiddenPose);
                return;
            }

            // Move the left saber if we are reversing it or it was assigned a tracker
            if (this.configuration.Current.ReverseLeftSaber || !String.IsNullOrWhiteSpace(this.configuration.Current.LeftTracker.Serial))
            {
                Pose leftSaberPose = this.saberDeviceManager.GetLeftSaberPose(this.configuration.Current.LeftTracker);
                this.saberDeviceManager.SetLeftSaberPose(this.configuration.Current.ReverseLeftSaber ? leftSaberPose.Reverse() : leftSaberPose);
            }
        }

        private void UpdateRightSaber()
        {
            if (this.configuration.Current.OneColor && this.configuration.Current.RemoveOtherSaber && this.configuration.Current.UseLeft)
            {
                // Move the other saber away since there's a bug in the base game which makes it
                // able to cut bombs still
                this.saberDeviceManager.SetRightSaberPose(this.hiddenPose);
                return;
            }

            // Move the right saber if we are reversing it or it was assigned a tracker
            if (this.configuration.Current.ReverseRightSaber || !String.IsNullOrWhiteSpace(this.configuration.Current.RightTracker.Serial))
            {
                Pose rightSaberPose = this.saberDeviceManager.GetRightSaberPose(this.configuration.Current.RightTracker);
                this.saberDeviceManager.SetRightSaberPose(this.configuration.Current.ReverseRightSaber ? rightSaberPose.Reverse() : rightSaberPose);
            }
        }

        /// <summary>
        /// Disables the rendering of the other saber
        /// </summary>
        private IEnumerator DisableOtherSaberMesh()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (!(this.configuration.Current.OneColor && this.configuration.Current.RemoveOtherSaber)) { yield break; }

            if (this.configuration.Current.UseLeft)
            {
                this.saberDeviceManager.DisableRightSaberMesh();
            }
            else
            {
                this.saberDeviceManager.DisableLeftSaberMesh();
            }
        }
    }
}
