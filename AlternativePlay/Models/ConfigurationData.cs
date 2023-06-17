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
        public string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public int Selected { get; set; }
        public List<PlayModeSettings> PlayModeSettings { get; set; }
    }

    /// <summary>
    /// Represents one configuration in the list, which consists of options for one play mode.
    /// </summary>
    [Serializable]
    public class PlayModeSettings
    {
        public const string DefaultPositionIncrement = "1.0";
        public const string DefaultRotationIncrement = "10.0f";

        public const float PositionMax = 500.0f;
        public const float RotationMax = 360.0f;

        [NonSerialized]
        public static readonly List<string> PositionIncrementList = new List<string> { "0.1", "1", "5", "10", "100" };
        [NonSerialized]
        public static readonly List<string> RotationIncrementList = new List<string> { "0.1", "1", "5", "10", "30" };

        public PlayMode PlayMode { get; set; } = PlayMode.BeatSaber;

        // Common Options
        public bool UseLeft { get; set; }
        public ControllerCountEnum ControllerCount { get; set; } = ControllerCountEnum.One;
        public float Gravity { get; set; } = 3.5f;

        // Beat Saber Options
        public bool ReverseLeftSaber { get; set; }
        public bool ReverseRightSaber { get; set; }
        public bool RemoveOtherSaber { get; set; }

        // Darth Maul Options
        public bool ReverseMaulDirection { get; set; }
        public bool UseTriggerToSeparate { get; set; }
        public int MaulDistance { get; set; } = 15;

        // Spear Options
        public bool UseTriggerToSwitchHands { get; set; }
        public bool ReverseSpearDirection { get; set; }

        // Nunchaku Options
        public bool ReverseNunchaku { get; set; }
        public int NunchakuLength { get; set; } = 50; // in centimetres

        // Flail Options
        public BeatFlailMode LeftFlailMode { get; set; } = BeatFlailMode.Flail;
        public BeatFlailMode RightFlailMode { get; set; } = BeatFlailMode.Flail;
        public int LeftFlailLength { get; set; } = 80; // in centimetres
        public int RightFlailLength { get; set; } = 80; // in centimetres
        public int MoveNotesBack { get; set; } = 0; // in centimetres

        // Gameplay Changes Options
        public bool NoArrowsRandom { get; set; }
        public bool OneColor { get; set; }
        public bool NoSliders { get; set; }
        public bool NoArrows { get; set; }
        public bool TouchNotes { get; set; }

        // Tracker Select Options
        public TrackerConfigData LeftTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightTracker { get; set; } = new TrackerConfigData();
        public string PositionIncrement { get; set; } = DefaultPositionIncrement;
        public string RotationIncrement { get; set; } = DefaultRotationIncrement;

        // Convenince functions
        public static float GetIncrement(string increment)
        {
            bool success = float.TryParse(increment, out float result);
            if (!success) result = 0.1f;

            return result;
        }

        public static string PlayModeDescription(PlayMode playMode)
        {
            switch(playMode)
            {
                default:
                case PlayMode.BeatSaber:
                    return "Beat Saber";

                case PlayMode.DarthMaul:
                    return "Darth Maul";

                case PlayMode.BeatSpear:
                    return "Beat Spear";

                case PlayMode.BeatFlail:
                    return "Beat Flail";

                case PlayMode.Nunchaku:
                    return "Nunchaku";
            }
        }
    }
}