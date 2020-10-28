using AlternativePlay.Models;
using System;
using UnityEngine;

namespace AlternativePlay
{
    public static class Utilities
    {
        /// <summary>
        /// Checks to see if the tracker data used should cause score submissions to be disabled.
        /// If the position or scale of the saber is anything other than the defaults, score
        /// submission will be disabled.  Rotation can be any value.
        /// </summary>
        public static void CheckAndDisableForTrackerTransforms(TrackerConfigData trackerData)
        {
            if (String.IsNullOrWhiteSpace(trackerData?.Serial)) return;

            if (trackerData.Position != Vector3.zero || trackerData.Scale != 1.0f)
            {
                // Disable scoring due to transforms
                AlternativePlay.Logger.Info($"Position: {trackerData.Position} Scale: {trackerData.Scale}");
                AlternativePlay.Logger.Info("Disabling score submission on tracker with non-default position or scaling");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(AlternativePlay.assemblyName);
            }
        }

        /// <summary>
        /// Transforms the <see cref="Transform"/> of a Unity game object based on the given <see cref="TrackerConfigData"/>
        /// given the current tracker's rotation and position.
        /// </summary>
        public static void TransformSaberFromTrackerData(Transform saberTransform, TrackerConfigData trackerData, Quaternion trackerRotation, Vector3 trackerPosition)
        {
            // Calculate and apply rotation first
            Quaternion extraRotation = Quaternion.Euler(trackerData.EulerAngles);
            Quaternion finalRotation = trackerRotation * extraRotation;
            saberTransform.localRotation = finalRotation;

            // Rotate position and add 
            Vector3 rotatedPosition = finalRotation * trackerData.Position;
            saberTransform.localPosition = trackerPosition + rotatedPosition;
        }
    }
}
