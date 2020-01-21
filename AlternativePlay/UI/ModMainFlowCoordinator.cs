using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;

namespace AlternativePlay.UI
{
    public class ModMainFlowCoordinator : FlowCoordinator
    {
        private AlternativePlayViewController alternativePlayView;
        private GameModifiersViewController gameModifiersView;

        private BeatSaberViewController beatSaberSettingsView;
        private DarthMaulViewController darthMaulSettingsView;
        private BeatSpearViewController beatSpearSettingsView;

        public void ShowBeatSaber()
        {
            this.SetLeftScreenViewController(beatSaberSettingsView);
        }

        public void ShowDarthMaul()
        {
            this.SetLeftScreenViewController(darthMaulSettingsView);
        }

        public void ShowBeatSpear()
        {
            this.SetLeftScreenViewController(beatSpearSettingsView);
        }

        private void Awake()
        {
            alternativePlayView = BeatSaberUI.CreateViewController<AlternativePlayViewController>();
            alternativePlayView.MainFlowCoordinator = this;
            gameModifiersView = BeatSaberUI.CreateViewController<GameModifiersViewController>();

            beatSaberSettingsView = BeatSaberUI.CreateViewController<BeatSaberViewController>();
            darthMaulSettingsView = BeatSaberUI.CreateViewController<DarthMaulViewController>();
            beatSpearSettingsView = BeatSaberUI.CreateViewController<BeatSpearViewController>();
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            ViewController viewToDisplay;

            switch (ConfigOptions.instance.PlayMode)
            {
                case PlayMode.DarthMaul:
                    viewToDisplay = darthMaulSettingsView;
                    break;

                case PlayMode.BeatSpear:
                    viewToDisplay = beatSpearSettingsView;
                    break;

                case PlayMode.BeatSaber:
                default:
                    viewToDisplay = beatSaberSettingsView;
                    break;
            }

            if (firstActivation)
            {
                title = "Alternative Play";
                showBackButton = true;
            }

            this.ProvideInitialViewControllers(alternativePlayView, viewToDisplay, gameModifiersView);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            MainFlowCoordinator mainFlow = BeatSaberUI.MainFlowCoordinator;
            mainFlow.InvokePrivateMethod("DismissFlowCoordinator", this, null, false);
        }
    }
}
