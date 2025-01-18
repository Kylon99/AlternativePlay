using Newtonsoft.Json;
using System;
using UnityEngine;
using Valve.VR;

namespace AlternativePlay.Models
{
    [Serializable]
    public class TrackerConfigData
    {
        [NonSerialized]
        public const string NoTrackerText = "Default";

        [NonSerialized]
        public const string NoTrackerHoverHint = "Not using any tracked devices";

        public string Serial { get; set; }
        public string Icon { get; set; }
        public string FullName { get; set; }
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 Position { get; set; }
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 EulerAngles { get; set; }

        /// <summary>
        /// Returns a deep copy of the given <see cref="TrackerConfigData"/>
        /// </summary>
        /// <param name="tracker">The tracker to copy</param>
        /// <returns>A deep copy of the given <see cref="TrackerConfigData"/></returns>
        public static TrackerConfigData Clone(TrackerConfigData tracker)
        {
            return new TrackerConfigData
            {
                Serial = tracker.Serial,
                Icon = tracker.Icon,
                FullName = tracker.FullName,
                Position = tracker.Position,
                EulerAngles = tracker.EulerAngles,
            };
        }

        /// <summary>
        /// Performs a deep copy from the source <see cref="TrackerConfigData"/> to the target 
        /// without destroying the target's reference
        /// </summary>
        /// <param name="source">The <see cref="TrackerConfigData"/> to copy from</param>
        /// <param name="target"><see cref="TrackerConfigData"/> to copy to</param>
        public static void Copy(TrackerConfigData source, TrackerConfigData target)
        {
            target.Serial = source.Serial;
            target.Icon = source.Icon;
            target.FullName = source.FullName;
            target.Position = source.Position;
            target.EulerAngles = source.EulerAngles;
        }
    }
}
