using System;
using System.Collections.Generic;
using System.Reflection;

namespace AlternativePlay.Models
{
    public enum PlayMode
    {
        BeatSaber = 0,
        DarthMaul = 1,
        BeatSpear = 2,
        Nunchaku = 3,
        BeatFlail = 4,
    }

    public enum ControllerCountEnum
    {
        One = 0,
        Two
    };

    public enum BeatFlailMode
    {
        Flail,
        Sword,
        None
    };

    [Serializable]
    public class ConfigurationData
    {
        public const string DefaultPositionIncrement = "1.0";
        public const string DefaultRotationIncrement = "10.0f";

        public const float PositionMax = 500.0f;
        public const float RotationMax = 360.0f;

        [NonSerialized]
        public static readonly List<string> PositionIncrementList = new List<string> { "0.1", "1", "5", "10", "100" };
        [NonSerialized]
        public static readonly List<string> RotationIncrementList = new List<string> { "0.1", "1", "5", "10", "30" };

        public string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public PlayMode PlayMode { get; set; } = PlayMode.BeatSaber;

        // Beat Saber Options
        public bool ReverseLeftSaber { get; set; }
        public bool ReverseRightSaber { get; set; }
        public bool UseLeftSaber { get; set; }
        public bool RemoveOtherSaber { get; set; }
        public TrackerConfigData LeftSaberTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightSaberTracker { get; set; } = new TrackerConfigData();

        // Darth Maul Options
        public ControllerCountEnum DarthMaulControllerCount { get; set; } = ControllerCountEnum.One;
        public bool UseLeftController { get; set; }
        public bool ReverseMaulDirection { get; set; }
        public bool UseTriggerToSeparate { get; set; }
        public int MaulDistance { get; set; } = 15;
        public TrackerConfigData LeftMaulTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightMaulTracker { get; set; } = new TrackerConfigData();

        // Spear Options
        public ControllerCountEnum SpearControllerCount { get; set; } = ControllerCountEnum.One;
        public bool UseLeftSpear { get; set; }
        public bool UseTriggerToSwitchHands { get; set; }
        public bool ReverseSpearDirection { get; set; }
        public TrackerConfigData LeftSpearTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightSpearTracker { get; set; } = new TrackerConfigData();

        // Nunchaku Options
        public bool ReverseNunchaku { get; set; }
        public int NunchakuLength { get; set; } = 50; // in centimetres
        public float NunchakuGravity { get; set; } = 3.5f;
        public TrackerConfigData LeftNunchakuTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightNunchakuTracker { get; set; } = new TrackerConfigData();

        // Flail Options
        public BeatFlailMode LeftFlailMode { get; set; } = BeatFlailMode.Flail;
        public BeatFlailMode RightFlailMode { get; set; } = BeatFlailMode.Flail;
        public int LeftFlailLength { get; set; } = 80; // in centimetres
        public int RightFlailLength { get; set; } = 80; // in centimetres
        public float FlailGravity { get; set; } = 3.5f;
        public int MoveNotesBack { get; set; } = 0; // in centimetres
        public TrackerConfigData LeftFlailTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightFlailTracker { get; set; } = new TrackerConfigData();

        // Gameplay Changes Options
        public bool NoArrowsRandom { get; set; }
        public bool OneColor { get; set; }
        public bool NoArrows { get; set; }
        public bool TouchNotes { get; set; }

        // Tracker Select Options
        public string PositionIncrement { get; set; } = DefaultPositionIncrement;
        public string RotationIncrement { get; set; } = DefaultRotationIncrement;

        // Convenince functions
        public static float GetIncrement(string increment)
        {
            bool success = float.TryParse(increment, out float result);
            if (!success) result = 0.1f;

            return result;
        }
    }
}