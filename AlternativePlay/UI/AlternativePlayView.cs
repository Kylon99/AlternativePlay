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

        private string beatSaberColor = White;
        [UIValue("BeatSaberColor")]
        public string BeatSaberColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }
        [UIValue("BeatSaberDefaultColor")]
        public string BeatSaberDefaultColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }
        [UIValue("BeatSaberHighLightColor")]
        public string BeatSaberHighLightColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }

        private string beatSaberIcon;
        [UIValue("BeatSaberIcon")]
        public string BeatSaberIcon { get => this.beatSaberIcon; set { this.beatSaberIcon = value; this.NotifyPropertyChanged(nameof(this.BeatSaberIcon)); } }

        private string darthMaulColor = Grey;
        [UIValue("DarthMaulColor")]
        public string DarthMaulColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }
        [UIValue("DarthMaulDefaultColor")]
        public string DarthMaulDefaultColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }
        [UIValue("DarthMaulHightLightColor")]
        public string DarthMaulHightLightColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }

        private string darthMaulIcon;
        [UIValue("DarthMaulIcon")]
        public string DarthMaulIcon { get => this.darthMaulIcon; set { this.darthMaulIcon = value; this.NotifyPropertyChanged(nameof(this.DarthMaulIcon)); } }

        private string beatSpearColor = Grey;
        [UIValue("BeatSpearColor")]
        public string BeatSpearColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }
        [UIValue("BeatSpearDefaultColor")]
        public string BeatSpearDefaultColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }
        [UIValue("BeatSpearHighLightColor")]
        public string BeatSpearHighLightColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }

        private string beatSpearIcon;
        [UIValue("BeatSpearIcon")]
        public string BeatSpearIcon { get => this.beatSpearIcon; set { this.beatSpearIcon = value; this.NotifyPropertyChanged(nameof(this.BeatSpearIcon)); } }

        private string nunchakuColor = Grey;
        [UIValue("NunchakuColor")]
        public string NunchakuColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuColor)); } }
        [UIValue("NunchakuDefaultColor")]
        public string NunchakuDefaultColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuDefaultColor)); } }
        [UIValue("NunchakuHighLightColor")]
        public string NunchakuHighLightColor { get => this.nunchakuColor; set { this.nunchakuColor = value; this.NotifyPropertyChanged(nameof(this.NunchakuColor)); } }

        private string nunchakuIcon;
        [UIValue("NunchakuIcon")]
        public string NunchakuIcon { get => this.nunchakuIcon; set { this.nunchakuIcon = value; this.NotifyPropertyChanged(nameof(this.NunchakuIcon)); } }

        private string flailColor = Grey;
        [UIValue("FlailColor")]
        public string FlailColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }
        [UIValue("FlailDefaultColor")]
        public string FlailDefaultColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }
        [UIValue("FlailHighLightColor")]
        public string FlailHighLightColor { get => this.flailColor; set { this.flailColor = value; this.NotifyPropertyChanged(nameof(this.FlailColor)); } }

        private string flailIcon;
        [UIValue("FlailIcon")]
        public string FlailIcon { get => this.flailIcon; set { this.flailIcon = value; this.NotifyPropertyChanged(nameof(this.FlailIcon)); } }

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
            Configuration.instance.ConfigurationData.PlayMode = PlayMode.Flail;
            Configuration.instance.SaveConfiguration();

            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
            MainFlowCoordinator.ShowBeatFlail();
        }

        private void SetPlayModeColor(PlayMode playMode)
        {
            // Set everything to grey first
            this.BeatSaberColor = Grey;
            this.BeatSaberIcon = "AlternativePlay.Resources.BeatSaberGrey.png";
            this.DarthMaulColor = Grey;
            this.DarthMaulIcon = "AlternativePlay.Resources.DarthMaulGrey.png";
            this.BeatSpearColor = Grey;
            this.BeatSpearIcon = "AlternativePlay.Resources.BeatSpearGrey.png";
            this.NunchakuColor = Grey;
            this.NunchakuIcon = "AlternativePlay.Resources.NoArrows.png";
            this.FlailColor = Grey;
            this.FlailIcon = "AlternativePlay.Resources.NoArrows.png";

            // Set only the item we selected to white
            switch (playMode)
            {
                case PlayMode.DarthMaul:
                    this.DarthMaulColor = White;
                    this.DarthMaulIcon = "AlternativePlay.Resources.DarthMaul.png";
                    break;

                case PlayMode.BeatSpear:
                    this.BeatSpearColor = White;
                    this.BeatSpearIcon = "AlternativePlay.Resources.BeatSpear.png";
                    break;

                case PlayMode.Nunchaku:
                    this.NunchakuColor = White;
                    this.NunchakuIcon = "AlternativePlay.Resources.NoArrows.png";
                    break;

                case PlayMode.Flail:
                    this.FlailColor = White;
                    this.FlailIcon = "AlternativePlay.Resources.NoArrows.png";
                    break;

                case PlayMode.BeatSaber:
                default:
                    this.BeatSaberColor = White;
                    this.BeatSaberIcon = "AlternativePlay.Resources.BeatSaber.png";
                    break;
            }
        }
    }
}
