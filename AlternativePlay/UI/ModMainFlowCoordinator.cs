using BeatSaberMarkupLanguage;
using HMUI;

namespace AlternativePlay.UI
{
    public class ModMainFlowCoordinator : FlowCoordinator
    {
        private AlternativePlayViewController alternativePlayView;
        private GameModifiersViewController gameModifiersView;

        private BeatSaberViewController beatSaberSettingsView;
        private DarthMaulViewController darthMaulSettingsView;
        private BeatSpearViewController beatSpearSettingsView;

        public bool IsBusy { get; set; }

        public void ShowBeatSaber()
        {
            IsBusy = true;
            this.SetLeftScreenViewController(beatSaberSettingsView);
            IsBusy = false;
        }

        public void ShowDarthMaul()
        {
            IsBusy = true;
            this.SetLeftScreenViewController(darthMaulSettingsView);
            IsBusy = false;
        }

        public void ShowBeatSpear()
        {
            IsBusy = true;
            this.SetLeftScreenViewController(beatSpearSettingsView);
            IsBusy = false;
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
            IsBusy = true;
            this.ProvideInitialViewControllers(alternativePlayView, viewToDisplay, gameModifiersView);
            IsBusy = false;
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            if (IsBusy) return;
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
