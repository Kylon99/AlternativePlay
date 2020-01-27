using BS_Utils.Utilities;

namespace AlternativePlay
{
    public enum PlayMode
    {
        BeatSaber = 0,
        DarthMaul = 1,
        BeatSpear = 2
    }

    public enum ControllerCountEnum
    {
        None = 0,
        One,
        Two
    };

    public class ConfigOptions : PersistentSingleton<ConfigOptions>
    {
        private Config config;

        #region Main Options
        private const string MainSection = "Main";
        private const string PlayModeOption = "PlayMode";

        private PlayMode playMode = PlayMode.BeatSaber;

        public PlayMode PlayMode { get => this.playMode; set { this.playMode = value; this.config.SetInt(MainSection, PlayModeOption, (int)value); } }
        #endregion

        #region Beat Saber
        private const string BeatSaberSection = "BeatSaber";
        private const string UseLeftSaberOption = "UseLeftSaber";
        private const string ReverseLeftSaberOption = "ReverseLeftSaber";
        private const string ReverseRightSaberOption = "ReverseRightSaber";
        private bool useLeftSaber;
        private bool reverseLeftSaber;
        private bool reverseRightSaber;

        public bool UseLeftSaber { get => this.useLeftSaber; set { this.useLeftSaber = value; this.config.SetBool(BeatSaberSection, UseLeftSaberOption, value); } }
        public bool ReverseLeftSaber { get => this.reverseLeftSaber; set { this.reverseLeftSaber = value; this.config.SetBool(BeatSaberSection, ReverseLeftSaberOption, value); } }
        public bool ReverseRightSaber { get => this.reverseRightSaber; set { this.reverseRightSaber = value; this.config.SetBool(BeatSaberSection, ReverseRightSaberOption, value); } }
        #endregion

        #region DarthMaul
        private const string DarthMaulSection = "DarthMaul";
        private const string DarthMaulControllerCountOption = "DarthMaulControllerCount";
        private const string UseLeftControllerOption = "UseLeftController";
        private const string ReverseMaulDirectionOption = "ReverseMaulDirection";
        private const string UseTriggerToSeparateOption = "UserTriggerToSeparate";
        private const string SeparationAmountOption = "SeparationAmount";

        private ControllerCountEnum darthMaulControllerCount = ControllerCountEnum.One;
        private bool useLeftController;
        private bool reverseMaulDirection;
        private bool useTriggerToSeparate;
        private int separationAmount;

        public ControllerCountEnum DarthMaulControllerCount { get => this.darthMaulControllerCount; set { this.darthMaulControllerCount = value; this.config.SetInt(DarthMaulSection, DarthMaulControllerCountOption, (int)value); } }
        public bool UseLeftController { get => this.useLeftController; set { this.useLeftController = value; this.config.SetBool(DarthMaulSection, UseLeftControllerOption, value); } }
        public bool ReverseMaulDirection { get => this.reverseMaulDirection; set { this.reverseMaulDirection = value; this.config.SetBool(DarthMaulSection, ReverseMaulDirectionOption, value); } }
        public bool UseTriggerToSeparate { get => this.useTriggerToSeparate; set { this.useTriggerToSeparate = value; this.config.SetBool(DarthMaulSection, UseTriggerToSeparateOption, value); } }
        public int SeparationAmount { get => this.separationAmount; set { this.separationAmount = value; this.config.SetInt(DarthMaulSection, SeparationAmountOption, value); } }
        #endregion

        #region BeatSpear
        private const string BeatSpearSection = "BeatSpear";
        private const string SpearControllerCountOption = "SpearControllerCount";
        private const string UseLeftSpearOption = "UseLeftSpear";
        private const string ReverseSpearDirectionOption = "ReverseSpearDirection";

        private ControllerCountEnum spearControllerCount = ControllerCountEnum.One;
        private bool useLeftSpear;
        private bool reverseSpearDirection;

        public ControllerCountEnum SpearControllerCount { get => this.spearControllerCount; set { this.spearControllerCount = value; this.config.SetInt(BeatSpearSection, SpearControllerCountOption, (int)value); } }
        public bool UseLeftSpear { get => this.useLeftSpear; set { this.useLeftSpear = value; this.config.SetBool(BeatSpearSection, UseLeftSpearOption, value); } }
        public bool ReverseSpearDirection { get => this.reverseSpearDirection; set { this.reverseSpearDirection = value; this.config.SetBool(BeatSpearSection, ReverseSpearDirectionOption, value); } }
        #endregion

        #region Gameplay Changes
        private const string GameplayChangesSection = "GameplayChanges";
        private const string NoArrowsRandomOption = "NoArrowsRandom";
        private const string OneColorOption = "OneColor";
        private const string RemoveOtherSaberOption = "RemoveOtherSaber";
        private const string NoArrowsOption = "NoArrows";
        private const string StabNotesOption = "StabNotes";

        private bool noArrowsRandom;
        private bool oneColor;
        private bool removeOtherSaber;
        private bool noArrows;
        private bool stabNotes;

        public bool NoArrowsRandom { get => this.noArrowsRandom; set { this.noArrowsRandom = value; this.config.SetBool(GameplayChangesSection, NoArrowsRandomOption, value); } }
        public bool OneColor { get => this.oneColor; set { this.oneColor = value; this.config.SetBool(GameplayChangesSection, OneColorOption, value); } }
        public bool RemoveOtherSaber { get => this.removeOtherSaber; set { this.removeOtherSaber = value; this.config.SetBool(GameplayChangesSection, RemoveOtherSaberOption, value); } }
        public bool NoArrows { get => this.noArrows; set { this.noArrows = value; this.config.SetBool(GameplayChangesSection, NoArrowsOption, value); } }
        public bool StabNotes { get => this.stabNotes; set { this.stabNotes = value; this.config.SetBool(GameplayChangesSection, StabNotesOption, value); } }

        #endregion

        public void SetDarthMaulGameModifiers()
        {
            this.NoArrowsRandom = false;
            this.OneColor = false;
            this.NoArrows = true;
            this.StabNotes = false;
        }

        public void SetBeatSpearGameModifiers()
        {
            this.NoArrowsRandom = false;
            this.OneColor = true;
            this.NoArrows = false;
            this.StabNotes = true;
        }

        /// <summary>
        /// Called before Start or Updates by Unity infrastructure
        /// </summary>
        private void Awake()
        {
            this.config = new Config(Plugin.assemblyName);

            // Common
            this.PlayMode = (PlayMode)this.config.GetInt(MainSection, PlayModeOption, (int)PlayMode.BeatSaber, true);

            // Beat Saber
            this.UseLeftSaber = this.config.GetBool(BeatSaberSection, UseLeftSaberOption, false, true);
            this.ReverseLeftSaber = this.config.GetBool(BeatSaberSection, ReverseLeftSaberOption, false, true);
            this.ReverseRightSaber = this.config.GetBool(BeatSaberSection, ReverseRightSaberOption, false, true);

            // Darth Maul
            this.DarthMaulControllerCount = (ControllerCountEnum)this.config.GetInt(DarthMaulSection, DarthMaulControllerCountOption, (int)ControllerCountEnum.One, true);
            this.UseLeftController = this.config.GetBool(DarthMaulSection, UseLeftControllerOption, false, true);
            this.ReverseMaulDirection = this.config.GetBool(DarthMaulSection, ReverseMaulDirectionOption, false, true);
            this.UseTriggerToSeparate = this.config.GetBool(DarthMaulSection, UseTriggerToSeparateOption, false, true);
            this.SeparationAmount = this.config.GetInt(DarthMaulSection, SeparationAmountOption, 15, true);

            // Spear
            this.SpearControllerCount = (ControllerCountEnum)this.config.GetInt(BeatSpearSection, SpearControllerCountOption, (int)ControllerCountEnum.One, true);
            this.UseLeftSpear = this.config.GetBool(BeatSpearSection, UseLeftSpearOption, false, true);
            this.ReverseSpearDirection = this.config.GetBool(BeatSpearSection, ReverseMaulDirectionOption, false, true);

            // Gameplay Changes
            this.NoArrowsRandom = this.config.GetBool(GameplayChangesSection, NoArrowsRandomOption, false, true);
            this.OneColor = this.config.GetBool(GameplayChangesSection, OneColorOption, false, true);
            this.RemoveOtherSaber = this.config.GetBool(GameplayChangesSection, RemoveOtherSaberOption, false, true);
            this.NoArrows = this.config.GetBool(GameplayChangesSection, NoArrowsOption, false, true);
            this.StabNotes = this.config.GetBool(GameplayChangesSection, StabNotesOption, false, true);
        }
    }
}
