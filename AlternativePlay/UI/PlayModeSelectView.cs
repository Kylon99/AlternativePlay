using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class PlayModeSelectView : BSMLAutomaticViewController
    {
#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
        [Inject]
        private AlternativePlayMainFlowCoordinator mainFlowCoordinator;
#pragma warning restore CS0649

        public int index { get; private set; }
        public PlayModeSettings Settings { get; private set; }

        public void SetPlayModeSettings(PlayModeSettings Settings, int index)
        {
            this.Settings = Settings;
            this.index = index;
        }

        private const string Grey = "#4F4F4F";
        private const string White = "#FFFFFF";

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            this.SetPlayModeColor(this.Settings.PlayMode);
        }

        [UIValue(nameof(BeatSaberIcon))]
        public string BeatSaberIcon => IconNames.BeatSaber;

        private string beatSaberColor = White;
        [UIValue(nameof(BeatSaberColor))]
        public string BeatSaberColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }
        [UIValue(nameof(BeatSaberDefaultColor))]
        public string BeatSaberDefaultColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }
        [UIValue(nameof(BeatSaberHighLightColor))]
        public string BeatSaberHighLightColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }


        [UIValue(nameof(DarthMaulIcon))]
        public string DarthMaulIcon => IconNames.DarthMaul;

        private string darthMaulColor = Grey;
        [UIValue(nameof(DarthMaulColor))]
        public string DarthMaulColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }
        [UIValue(nameof(DarthMaulDefaultColor))]
        public string DarthMaulDefaultColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }
        [UIValue(nameof(DarthMaulHightLightColor))]
        public string DarthMaulHightLightColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }


        [UIValue(nameof(BeatSpearIcon))]
        public string BeatSpearIcon => IconNames.BeatSpear;

        private string beatSpearColor = Grey;
        [UIValue(nameof(BeatSpearColor))]
        public string BeatSpearColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }
        [UIValue(nameof(BeatSpearDefaultColor))]
        public string BeatSpearDefaultColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }
        [UIValue(nameof(BeatSpearHighLightColor))]
        public string BeatSpearHighLightColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }


        [UIValue(nameof(NunchakuIcon))]
        public string NunchakuIcon => IconNames.Nunchaku;

        private string nunchakuColor = Grey;
        [UIValue(nameof(NunchakuColor))]
        public string NunchakuColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuColor)); } }
        [UIValue(nameof(NunchakuDefaultColor))]
        public string NunchakuDefaultColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuDefaultColor)); } }
        [UIValue(nameof(NunchakuHighLightColor))]
        public string NunchakuHighLightColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuColor)); } }


        [UIValue(nameof(FlailIcon))]
        public string FlailIcon => IconNames.BeatFlail;

        private string flailColor = Grey;
        [UIValue(nameof(FlailColor))]
        public string FlailColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }
        [UIValue(nameof(FlailDefaultColor))]
        public string FlailDefaultColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }
        [UIValue(nameof(FlailHighLightColor))]
        public string FlailHighLightColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }


        [UIAction(nameof(OnBeatSaberClick))]
        private void OnBeatSaberClick()
        {
            this.Settings.PlayMode = PlayMode.BeatSaber;
            this.configuration.SaveConfiguration();

            this.SetPlayModeColor(this.Settings.PlayMode);
            this.mainFlowCoordinator.ShowBeatSaber();
        }

        [UIAction(nameof(OnDarthMaulClick))]
        private void OnDarthMaulClick()
        {
            this.Settings.PlayMode = PlayMode.DarthMaul;
            this.configuration.SaveConfiguration();

            this.SetPlayModeColor(this.Settings.PlayMode);
            this.mainFlowCoordinator.ShowDarthMaul();
        }

        [UIAction(nameof(OnBeatSpearClick))]
        private void OnBeatSpearClick()
        {
            this.Settings.PlayMode = PlayMode.BeatSpear;
            this.configuration.SaveConfiguration();

            this.SetPlayModeColor(this.Settings.PlayMode);
            this.mainFlowCoordinator.ShowBeatSpear();
        }

        [UIAction(nameof(OnNunchakuClick))]
        private void OnNunchakuClick()
        {
            this.Settings.PlayMode = PlayMode.Nunchaku;
            this.configuration.SaveConfiguration();

            this.SetPlayModeColor(this.Settings.PlayMode);
            this.mainFlowCoordinator.ShowNunchaku();
        }

        [UIAction(nameof(OnFlailClick))]
        private void OnFlailClick()
        {
            this.Settings.PlayMode = PlayMode.BeatFlail;
            this.configuration.SaveConfiguration();

            this.SetPlayModeColor(this.Settings.PlayMode);
            this.mainFlowCoordinator.ShowBeatFlail();
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
