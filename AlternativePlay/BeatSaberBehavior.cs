using AlternativePlay.Models;
using System;
using System.Collections;
using UnityEngine;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
        private readonly Pose hiddenPose = new Pose(new Vector3(0.0f, -1000.0f, 0.0f), Quaternion.Euler(90.0f, 0.0f, 0.0f));

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Do nothing if we aren't playing Beat Saber
            if (Configuration.Current.PlayMode != PlayMode.BeatSaber) { return; }

            Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.LeftTracker);
            Utilities.CheckAndDisableForTrackerTransforms(Configuration.Current.RightTracker);

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Update()
        {
            if (Configuration.Current.PlayMode != PlayMode.BeatSaber) return;

            this.UpdateLeftSaber();
            this.UpdateRightSaber();
        }

        private void UpdateLeftSaber()
        {
            var saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;

            if (Configuration.Current.OneColor && Configuration.Current.RemoveOtherSaber && !Configuration.Current.UseLeft)
            {
                // Move the other saber away since there's a bug in the base game which makes it
                // able to cut bombs still
                saberDeviceManager.SetLeftSaberPose(this.hiddenPose);
                return;
            }

            // Move the left saber if we are reversing it or it was assigned a tracker
            if (Configuration.Current.ReverseLeftSaber || !String.IsNullOrWhiteSpace(Configuration.Current.LeftTracker.Serial))
            {
                Pose leftSaberPose = saberDeviceManager.GetLeftSaberPose(Configuration.Current.LeftTracker);
                saberDeviceManager.SetLeftSaberPose(Configuration.Current.ReverseLeftSaber ? leftSaberPose.Reverse() : leftSaberPose);
            }
        }

        private void UpdateRightSaber()
        {
            var saberDeviceManager = BehaviorCatalog.instance.SaberDeviceManager;

            if (Configuration.Current.OneColor && Configuration.Current.RemoveOtherSaber && Configuration.Current.UseLeft)
            {
                // Move the other saber away since there's a bug in the base game which makes it
                // able to cut bombs still
                saberDeviceManager.SetRightSaberPose(this.hiddenPose);
                return;
            }

            // Move the right saber if we are reversing it or it was assigned a tracker
            if (Configuration.Current.ReverseRightSaber || !String.IsNullOrWhiteSpace(Configuration.Current.RightTracker.Serial))
            {
                Pose rightSaberPose = saberDeviceManager.GetRightSaberPose(Configuration.Current.RightTracker);
                saberDeviceManager.SetRightSaberPose(Configuration.Current.ReverseRightSaber ? rightSaberPose.Reverse() : rightSaberPose);
            }
        }

        /// <summary>
        /// Disables the rendering of the other saber
        /// </summary>
        private IEnumerator DisableOtherSaberMesh()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (!(Configuration.Current.OneColor && Configuration.Current.RemoveOtherSaber)) { yield break; }

            if (Configuration.Current.UseLeft)
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableRightSaberMesh();
            }
            else
            {
                BehaviorCatalog.instance.SaberDeviceManager.DisableLeftSaberMesh();
            }
        }
    }
}
