using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AlternativePlay.UI
{
    public class AlternativePlayViewController : BSMLResourceViewController
    {
        public override string ResourceName => Plugin.assemblyName + ".UI.Views.AlternativePlayView.bsml";
        public ModMainFlowCoordinator MainFlowCoordinator { get; set; }

        [UIAction("BeatSaberClick")]
        private void OnBeatSaberClick()
        {
            ConfigOptions.instance.PlayMode = PlayMode.BeatSaber;
            MainFlowCoordinator.ShowBeatSaber();
        }

        [UIAction("DarthMaulClick")]
        private void OnDarthMaulClick()
        {
            ConfigOptions.instance.PlayMode = PlayMode.DarthMaul;
            MainFlowCoordinator.ShowDarthMaul();
        }

        [UIAction("BeatSpearClick")]
        private void OnBeatSpearClick()
        {
            ConfigOptions.instance.PlayMode = PlayMode.BeatSpear;
            MainFlowCoordinator.ShowBeatSpear();
        }

    }
}
