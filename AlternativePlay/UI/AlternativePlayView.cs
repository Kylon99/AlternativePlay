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

        private string beatSaberIcon;
        [UIValue("BeatSaberIcon")]
        public string BeatSaberIcon { get => this.beatSaberIcon; set { this.beatSaberIcon = value; this.NotifyPropertyChanged(nameof(this.BeatSaberIcon)); } }

        private string darthMaulColor = Grey;
        [UIValue("DarthMaulColor")]
        public string DarthMaulColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }

        private string darthMaulIcon;
        [UIValue("DarthMaulIcon")]
        public string DarthMaulIcon { get => this.darthMaulIcon; set { this.darthMaulIcon = value; this.NotifyPropertyChanged(nameof(this.DarthMaulIcon)); } }

        private string beatSpearColor = Grey;
        [UIValue("BeatSpearColor")]
        public string BeatSpearColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }

        private string beatSpearIcon;
        [UIValue("BeatSpearIcon")]
        public string BeatSpearIcon { get => this.beatSpearIcon; set { this.beatSpearIcon = value; this.NotifyPropertyChanged(nameof(this.BeatSpearIcon)); } }


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

        private void SetPlayModeColor(PlayMode playMode)
        {
            switch (playMode)
            {
                case PlayMode.DarthMaul:
                    this.BeatSaberColor = Grey;
                    this.BeatSaberIcon = "AlternativePlay.Resources.BeatSaberGrey.png";
                    this.DarthMaulColor = White;
                    this.DarthMaulIcon = "AlternativePlay.Resources.DarthMaul.png";
                    this.BeatSpearColor = Grey;
                    this.BeatSpearIcon = "AlternativePlay.Resources.BeatSpearGrey.png";
                    break;

                case PlayMode.BeatSpear:
                    this.BeatSaberColor = Grey;
                    this.BeatSaberIcon = "AlternativePlay.Resources.BeatSaberGrey.png";
                    this.DarthMaulColor = Grey;
                    this.DarthMaulIcon = "AlternativePlay.Resources.DarthMaulGrey.png";
                    this.BeatSpearColor = White;
                    this.BeatSpearIcon = "AlternativePlay.Resources.BeatSpear.png";
                    break;

                case PlayMode.BeatSaber:
                default:
                    this.BeatSaberColor = White;
                    this.BeatSaberIcon = "AlternativePlay.Resources.BeatSaber.png";
                    this.DarthMaulColor = Grey;
                    this.DarthMaulIcon = "AlternativePlay.Resources.DarthMaulGrey.png";
                    this.BeatSpearColor = Grey;
                    this.BeatSpearIcon = "AlternativePlay.Resources.BeatSpearGrey.png";
                    break;
            }
        }
    }
}
