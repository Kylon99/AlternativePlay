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

        private string beatSaberColor = White;
        [UIValue("BeatSaberColor")]
        public string BeatSaberColor { get => this.beatSaberColor; set { this.beatSaberColor = value; this.NotifyPropertyChanged(nameof(this.BeatSaberColor)); } }

        private string darthMaulColor = Grey;
        [UIValue("DarthMaulColor")]
        public string DarthMaulColor { get => this.darthMaulColor; set { this.darthMaulColor = value; this.NotifyPropertyChanged(nameof(this.DarthMaulColor)); } }

        private string beatSpearColor = Grey;
        [UIValue("BeatSpearColor")]
        public string BeatSpearColor { get => this.beatSpearColor; set { this.beatSpearColor = value; this.NotifyPropertyChanged(nameof(this.BeatSpearColor)); } }


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

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            SetPlayModeColor(Configuration.instance.ConfigurationData.PlayMode);
        }

        private void SetPlayModeColor(PlayMode playMode)
        {
            switch (playMode)
            {
                case PlayMode.DarthMaul:
                    this.BeatSaberColor = Grey;
                    this.DarthMaulColor = White;
                    this.BeatSpearColor = Grey;
                    break;

                case PlayMode.BeatSpear:
                    this.BeatSaberColor = Grey;
                    this.DarthMaulColor = Grey;
                    this.BeatSpearColor = White;
                    break;

                case PlayMode.BeatSaber:
                default:
                    this.BeatSaberColor = White;
                    this.DarthMaulColor = Grey;
                    this.BeatSpearColor = Grey;
                    break;
            }
        }
    }
}
