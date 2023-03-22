using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AlternativePlay.UI
{
    [HotReload]
    public class AlternativePlayView : BSMLAutomaticViewController
    {
        public ModMainFlowCoordinator MainFlowCoordinator { get; set; }

        private const string Grey = "#4F4F4F";
        private const string White = "#FFFFFF";

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }

        [UIValue("BeatSaberIcon")]
        public string BeatSaberIcon => IconNames.BeatSaber;

        private string beatSaberColor = White;
        [UIValue("BeatSaberColor")]
        public string BeatSaberColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }
        [UIValue("BeatSaberDefaultColor")]
        public string BeatSaberDefaultColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }
        [UIValue("BeatSaberHighLightColor")]
        public string BeatSaberHighLightColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }


        [UIValue("DarthMaulIcon")]
        public string DarthMaulIcon => IconNames.DarthMaul;

        private string darthMaulColor = Grey;
        [UIValue("DarthMaulColor")]
        public string DarthMaulColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }
        [UIValue("DarthMaulDefaultColor")]
        public string DarthMaulDefaultColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }
        [UIValue("DarthMaulHightLightColor")]
        public string DarthMaulHightLightColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }


        [UIValue("BeatSpearIcon")]
        public string BeatSpearIcon => IconNames.BeatSpear;

        private string beatSpearColor = Grey;
        [UIValue("BeatSpearColor")]
        public string BeatSpearColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }
        [UIValue("BeatSpearDefaultColor")]
        public string BeatSpearDefaultColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }
        [UIValue("BeatSpearHighLightColor")]
        public string BeatSpearHighLightColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }


        [UIValue("NunchakuIcon")]
        public string NunchakuIcon => IconNames.Nunchaku;

        private string nunchakuColor = Grey;
        [UIValue("NunchakuColor")]
        public string NunchakuColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuColor)); } }
        [UIValue("NunchakuDefaultColor")]
        public string NunchakuDefaultColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuDefaultColor)); } }
        [UIValue("NunchakuHighLightColor")]
        public string NunchakuHighLightColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuColor)); } }


        [UIValue("FlailIcon")]
        public string FlailIcon => IconNames.BeatFlail;

        private string flailColor = Grey;
        [UIValue("FlailColor")]
        public string FlailColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }
        [UIValue("FlailDefaultColor")]
        public string FlailDefaultColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }
        [UIValue("FlailHighLightColor")]
        public string FlailHighLightColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }


        [UIAction("BeatSaberClick")]
        private void OnBeatSaberClick()
        {
            Configuration.instance.ConfigurationData.PlayMode = PlayMode.BeatSaber;
            Configuration.instance.SaveConfiguration();

            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
            MainFlowCoordinator.ShowBeatSaber();
        }

        [UIAction("DarthMaulClick")]
        private void OnDarthMaulClick()
        {
            Configuration.instance.ConfigurationData.PlayMode = PlayMode.DarthMaul;
            Configuration.instance.SaveConfiguration();

            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
            MainFlowCoordinator.ShowDarthMaul();
        }

        [UIAction("BeatSpearClick")]
        private void OnBeatSpearClick()
        {
            Configuration.instance.ConfigurationData.PlayMode = PlayMode.BeatSpear;
            Configuration.instance.SaveConfiguration();

            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
            MainFlowCoordinator.ShowBeatSpear();
        }

        [UIAction("NunchakuClick")]
        private void OnNunchakuClick()
        {
            Configuration.instance.ConfigurationData.PlayMode = PlayMode.Nunchaku;
            Configuration.instance.SaveConfiguration();

            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
            MainFlowCoordinator.ShowNunchaku();
        }

        [UIAction("FlailClick")]
        private void OnFlailClick()
        {
            Configuration.instance.ConfigurationData.PlayMode = PlayMode.BeatFlail;
            Configuration.instance.SaveConfiguration();

            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
            MainFlowCoordinator.ShowBeatFlail();
        }

        private void SetPlayModeColor(PlayMode playMode)
        {
            this.BeatSaberColor = playMode == PlayMode.BeatSaber ? White : Grey;
            this.DarthMaulColor = playMode == PlayMode.DarthMaul ? White : Grey;
            this.BeatSpearColor = playMode == PlayMode.BeatSpear ? White : Grey;
            this.NunchakuColor = playMode == PlayMode.Nunchaku ? White : Grey;
            this.FlailColor = playMode == PlayMode.BeatFlail ? White : Grey;
        }
    }
}
