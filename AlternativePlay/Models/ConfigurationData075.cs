using System;
using System.Collections.Generic;

namespace AlternativePlay.Models
{
    public enum PlayMode075
    {
        BeatSaber = 0,
        DarthMaul = 1,
        BeatSpear = 2,
        Nunchaku = 3,
        BeatFlail = 4,
    }

    public enum ControllerCountEnum075
    {
        One = 0,
        Two
    };

    public enum BeatFlailMode075
    {
        Flail,
        Sword,
        None
    };

    [Serializable]
    public class ConfigurationData075
    {
        public PlayModeSettings ToConfigurationData()
        {
            var result = new PlayModeSettings();

            // Convert Playmode options first
            switch (this.PlayMode)
            {
                default:
                case PlayMode075.BeatSaber:
                    result.PlayMode = Models.PlayMode.BeatSaber;
                    result.UseLeft = this.UseLeftSaber;
                    result.ReverseLeftSaber = this.ReverseLeftSaber;
                    result.ReverseRightSaber = this.ReverseRightSaber;
                    result.RemoveOtherSaber = this.RemoveOtherSaber;

                    // Tracker Select options
                    result.LeftTracker = TrackerConfigData.Clone(this.LeftSaberTracker);
                    result.RightTracker = TrackerConfigData.Clone(this.RightSaberTracker);
                    break;

                case PlayMode075.DarthMaul:
                    result.PlayMode = Models.PlayMode.DarthMaul;
                    result.ControllerCount = this.DarthMaulControllerCount == ControllerCountEnum075.One ? ControllerCountEnum.One : ControllerCountEnum.Two;
                    result.UseLeft = this.UseLeftController;
                    result.ReverseMaulDirection = this.ReverseMaulDirection;
                    result.UseTriggerToSeparate = this.UseTriggerToSeparate;
                    result.MaulDistance = this.MaulDistance;

                    // Tracker Select options
                    result.LeftTracker = TrackerConfigData.Clone(this.LeftMaulTracker);
                    result.RightTracker = TrackerConfigData.Clone(this.RightMaulTracker);
                    break;

                case PlayMode075.BeatSpear:
                    result.PlayMode = Models.PlayMode.BeatSpear;
                    result.ControllerCount = this.SpearControllerCount == ControllerCountEnum075.One ? ControllerCountEnum.One : ControllerCountEnum.Two;
                    result.UseTriggerToSwitchHands = this.UseTriggerToSwitchHands;
                    result.ReverseSpearDirection = this.ReverseSpearDirection;

                    // Tracker Select options
                    result.LeftTracker = TrackerConfigData.Clone(this.LeftSpearTracker);
                    result.RightTracker = TrackerConfigData.Clone(this.RightSpearTracker);
                    break;

                case PlayMode075.Nunchaku:
                    result.PlayMode = Models.PlayMode.Nunchaku;
                    result.ReverseNunchaku = this.ReverseNunchaku;
                    result.NunchakuLength = this.NunchakuLength;
                    result.Gravity = this.NunchakuGravity;

                    // Tracker Select options
                    result.LeftTracker = TrackerConfigData.Clone(this.LeftNunchakuTracker);
                    result.RightTracker = TrackerConfigData.Clone(this.RightNunchakuTracker);
                    break;

                case PlayMode075.BeatFlail:
                    result.PlayMode = Models.PlayMode.BeatFlail;
                    result.LeftFlailMode = this.LeftFlailMode == BeatFlailMode075.Flail ? BeatFlailMode.Flail : this.LeftFlailMode == BeatFlailMode075.Sword ? BeatFlailMode.Sword : BeatFlailMode.None;
                    result.RightFlailMode = this.RightFlailMode == BeatFlailMode075.Flail ? BeatFlailMode.Flail : this.RightFlailMode == BeatFlailMode075.Sword ? BeatFlailMode.Sword : BeatFlailMode.None;
                    result.LeftFlailLength = this.LeftFlailLength;
                    result.RightFlailLength = this.RightFlailLength;
                    result.Gravity = this.FlailGravity;
                    result.MoveNotesBack = this.MoveNotesBack;

                    // Tracker Select options
                    result.LeftTracker = TrackerConfigData.Clone(this.LeftFlailTracker);
                    result.RightTracker = TrackerConfigData.Clone(this.RightFlailTracker);
                    break;
            }

            // Convert Game Modifier options
            result.NoArrowsRandom = this.NoArrowsRandom;
            result.OneColor = this.OneColor;
            result.NoSliders = this.NoSliders;
            result.NoArrows = this.NoArrows;
            result.TouchNotes = this.TouchNotes;

            // Common Tracker options
            result.PositionIncrement = this.PositionIncrement;
            result.RotationIncrement = this.RotationIncrement;

            return result;
        }

        public const string Version = "0.7.5";
        public const string DefaultPositionIncrement = "1.0";
        public const string DefaultRotationIncrement = "10.0f";

        public const float PositionMax = 500.0f;
        public const float RotationMax = 360.0f;

        [NonSerialized]
        public static readonly List<string> PositionIncrementList = new List<string> { "0.1", "1", "5", "10", "100" };
        [NonSerialized]
        public static readonly List<string> RotationIncrementList = new List<string> { "0.1", "1", "5", "10", "30" };

        public PlayMode075 PlayMode { get; set; } = PlayMode075.BeatSaber;

        // Beat Saber Options
        public bool ReverseLeftSaber { get; set; }
        public bool ReverseRightSaber { get; set; }
        public bool UseLeftSaber { get; set; }
        public bool RemoveOtherSaber { get; set; }
        public TrackerConfigData LeftSaberTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightSaberTracker { get; set; } = new TrackerConfigData();

        // Darth Maul Options
        public ControllerCountEnum075 DarthMaulControllerCount { get; set; } = ControllerCountEnum075.One;
        public bool UseLeftController { get; set; }
        public bool ReverseMaulDirection { get; set; }
        public bool UseTriggerToSeparate { get; set; }
        public int MaulDistance { get; set; } = 15;
        public TrackerConfigData LeftMaulTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightMaulTracker { get; set; } = new TrackerConfigData();

        // Spear Options
        public ControllerCountEnum075 SpearControllerCount { get; set; } = ControllerCountEnum075.One;
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
        public BeatFlailMode075 LeftFlailMode { get; set; } = BeatFlailMode075.Flail;
        public BeatFlailMode075 RightFlailMode { get; set; } = BeatFlailMode075.Flail;
        public int LeftFlailLength { get; set; } = 80; // in centimetres
        public int RightFlailLength { get; set; } = 80; // in centimetres
        public float FlailGravity { get; set; } = 3.5f;
        public int MoveNotesBack { get; set; } = 0; // in centimetres
        public TrackerConfigData LeftFlailTracker { get; set; } = new TrackerConfigData();
        public TrackerConfigData RightFlailTracker { get; set; } = new TrackerConfigData();

        // Gameplay Changes Options
        public bool NoArrowsRandom { get; set; }
        public bool OneColor { get; set; }
        public bool NoSliders { get; set; }
        public bool NoArrows { get; set; }
        public bool TouchNotes { get; set; }

        // Tracker Select Options
        public string PositionIncrement { get; set; } = DefaultPositionIncrement;
        public string RotationIncrement { get; set; } = DefaultRotationIncrement;

    }
}