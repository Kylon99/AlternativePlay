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

        public bool IsBusy { get; set; }

        public void ShowBeatSaber()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.beatSaberSettingsView, ViewController.AnimationType.In);
            this.IsBusy = false;
        }

        public void ShowDarthMaul()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.darthMaulSettingsView, ViewController.AnimationType.In);
            this.IsBusy = false;
        }

        public void ShowBeatSpear()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.beatSpearSettingsView, ViewController.AnimationType.In);
            this.IsBusy = false;
        }

        public void ShowTrackerSelect(TrackerConfigData trackerConfigData)
        {
            this.IsBusy = true;
            this.SetTitle("Select Tracker");

            this.trackerSelectView.SetSelectingTracker(trackerConfigData);
            this.trackerPoseView.SetSelectingTracker(trackerConfigData);

            this.ReplaceTopViewController(this.trackerSelectView);
            this.SetLeftScreenViewController(null, ViewController.AnimationType.In);
            this.SetRightScreenViewController(this.trackerPoseView, ViewController.AnimationType.In);

            this.IsBusy = false;

        }

        public void DismissTrackerSelect()
        {
            this.IsBusy = true;
            this.SetTitle(titleString);

            this.ReplaceTopViewController(this.alternativePlayView);
            var viewToDisplay = DecideLeftMainView();
            this.SetLeftScreenViewController(viewToDisplay, ViewController.AnimationType.Out);
            this.SetRightScreenViewController(this.gameModifiersView, ViewController.AnimationType.Out);
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
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            this.SetTitle(titleString);
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
