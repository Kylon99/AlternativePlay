using AlternativePlay.Models;
using BeatSaberMarkupLanguage;
using HMUI;

namespace AlternativePlay.UI
{
    public class ModMainFlowCoordinator : FlowCoordinator
    {
        private const string titleString = "Alternative Play";
        private AlternativePlayView alternativePlayView;
        private GameModifiersView gameModifiersView;

        private BeatSaberView beatSaberSettingsView;
        private DarthMaulView darthMaulSettingsView;
        private BeatSpearView beatSpearSettingsView;

        private TrackerSelectView trackerSelectView;
        private TrackerPoseView trackerPoseView;
        private TrackerOptionsView trackerOptionsView;

        public bool IsBusy { get; set; }

        public void ShowBeatSaber()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.beatSaberSettingsView);
            this.IsBusy = false;
        }

        public void ShowDarthMaul()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.darthMaulSettingsView);
            this.IsBusy = false;
        }

        public void ShowBeatSpear()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.beatSpearSettingsView);
            this.IsBusy = false;
        }

        public void ShowTrackerSelect(TrackerConfigData trackerConfigData)
        {
            this.IsBusy = true;
            this.title = "Select Tracker";

            this.trackerSelectView.SetSelectingTracker(trackerConfigData);
            this.trackerPoseView.SetSelectingTracker(trackerConfigData);

            this.ReplaceTopViewController(this.trackerSelectView);
            this.SetLeftScreenViewController(this.trackerPoseView);
            this.SetRightScreenViewController(this.trackerOptionsView);

            this.IsBusy = false;

        }

        public void DismissTrackerSelect()
        {
            this.IsBusy = true;
            this.title = titleString;

            this.ReplaceTopViewController(this.alternativePlayView);
            var viewToDisplay = DecideLeftMainView();
            this.SetLeftScreenViewController(viewToDisplay);
            this.SetRightScreenViewController(this.gameModifiersView);
            this.IsBusy = false;
        }

        private void Awake()
        {
            this.alternativePlayView = BeatSaberUI.CreateViewController<AlternativePlayView>();
            this.alternativePlayView.MainFlowCoordinator = this;
            this.gameModifiersView = BeatSaberUI.CreateViewController<GameModifiersView>();

            this.beatSaberSettingsView = BeatSaberUI.CreateViewController<BeatSaberView>();
            this.beatSaberSettingsView.SetMainFlowCoordinator(this);
            this.darthMaulSettingsView = BeatSaberUI.CreateViewController<DarthMaulView>();
            this.darthMaulSettingsView.SetMainFlowCoordinator(this);
            this.beatSpearSettingsView = BeatSaberUI.CreateViewController<BeatSpearView>();
            this.beatSpearSettingsView.SetMainFlowCoordinator(this);

            this.trackerSelectView = BeatSaberUI.CreateViewController<TrackerSelectView>();
            this.trackerSelectView.SetMainFlowCoordinator(this);
            this.trackerPoseView = BeatSaberUI.CreateViewController<TrackerPoseView>();
            this.trackerOptionsView = BeatSaberUI.CreateViewController<TrackerOptionsView>();
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            this.title = titleString;
            this.showBackButton = true;

            var viewToDisplay = DecideLeftMainView();

            this.IsBusy = true;
            this.trackerSelectView.SetSelectingTracker(new TrackerConfigData());
            ProvideInitialViewControllers(this.alternativePlayView, viewToDisplay, this.gameModifiersView);
            this.IsBusy = false;
        }

        private ViewController DecideLeftMainView()
        {
            ViewController viewToDisplay;

            switch (Configuration.instance.ConfigurationData.PlayMode)
            {
                case PlayMode.DarthMaul:
                    viewToDisplay = this.darthMaulSettingsView;
                    break;

                case PlayMode.BeatSpear:
                    viewToDisplay = this.beatSpearSettingsView;
                    break;

                case PlayMode.BeatSaber:
                default:
                    viewToDisplay = this.beatSaberSettingsView;
                    break;
            }

            return viewToDisplay;
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            if (this.IsBusy) return;

            if (topViewController == this.trackerSelectView)
            {
                DismissTrackerSelect();
                return;
            }

            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
