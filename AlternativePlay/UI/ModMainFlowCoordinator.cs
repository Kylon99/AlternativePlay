using AlternativePlay.Models;
using BeatSaberMarkupLanguage;
using HMUI;

namespace AlternativePlay.UI
{
    public class ModMainFlowCoordinator : FlowCoordinator
    {
        private AlternativePlayView alternativePlayView;
        private GameModifiersView gameModifiersView;

        private BeatSaberView beatSaberSettingsView;
        private DarthMaulView darthMaulSettingsView;
        private BeatSpearView beatSpearSettingsView;

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
            alternativePlayView = BeatSaberUI.CreateViewController<AlternativePlayView>();
            alternativePlayView.MainFlowCoordinator = this;
            gameModifiersView = BeatSaberUI.CreateViewController<GameModifiersView>();

            beatSaberSettingsView = BeatSaberUI.CreateViewController<BeatSaberView>();
            darthMaulSettingsView = BeatSaberUI.CreateViewController<DarthMaulView>();
            beatSpearSettingsView = BeatSaberUI.CreateViewController<BeatSpearView>();
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            ViewController viewToDisplay;

            switch (Configuration.instance.ConfigurationData.PlayMode)
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
