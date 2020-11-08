using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public static class Utilities
    {
        /// <summary>
        /// Checks to see if the tracker data used should cause score submissions to be disabled.
        /// If the position of the saber is anything other than (0, 0, 0) then score
        /// submission will be disabled.  Rotation can be any value.
        /// </summary>
        public static void CheckAndDisableForTrackerTransforms(TrackerConfigData trackerConfigData)
        {
            if (String.IsNullOrWhiteSpace(trackerConfigData?.Serial)) return;

            if (trackerConfigData.Position != Vector3.zero)
            {
                // Disable scoring due to transforms
                AlternativePlay.Logger.Info($"Position: {trackerConfigData.Position}");
                AlternativePlay.Logger.Info("Disabling score submission on tracker with non-default position");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(AlternativePlay.assemblyName);
            }
        }

        /// <summary>
        /// Transforms the <see cref="Transform"/> of a Unity game object based on the given <see cref="TrackerConfigData"/>
        /// given the current tracker's rotation and position.
        /// </summary>
        public static void TransformSaberFromTrackerData(Transform saberTransform, TrackerConfigData trackerConfigData,
            Quaternion trackerRotation, Vector3 trackerPosition)
        {
            // Calculate and apply rotation first
            Quaternion extraRotation = Quaternion.Euler(trackerConfigData.EulerAngles);
            Quaternion finalRotation = trackerRotation * extraRotation;
            saberTransform.localRotation = finalRotation;

            // Rotate position and add 
            Vector3 rotatedPosition = trackerRotation * trackerConfigData.Position;
            saberTransform.localPosition = trackerPosition + rotatedPosition;
        }
    }
}
