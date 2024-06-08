using AlternativePlay.Models;
using BeatSaberMarkupLanguage;
using HMUI;
using Zenject;

namespace AlternativePlay.UI
{
    /// <summary>
    /// The flow consists of three steps:
    /// 1. <see cref="AlternativePlayView"/> - The main view allowing you to select from a list of configurations
    /// 2. <see cref="PlayModeSelectView"/> - For configuring the settings of a play mode (<see cref="BeatSaberView"/>, <see cref="DarthMaulView"/>, etc) and the <see cref="GameModifiersView"/> on the right
    /// 3. <see cref="TrackerSelectView"/> - For setting trackers and options with a <see cref="TrackerPoseView"/> on the right
    /// </summary>
    public class AlternativePlayMainFlowCoordinator : FlowCoordinator
    {
        private const string titleString = "Alternative Play";
        private AlternativePlayView alternativePlayView;
        private PlayModeSelectView playModeSelectView;
        private GameModifiersView gameModifiersView;

        private BeatSaberView beatSaberSettingsView;
        private DarthMaulView darthMaulSettingsView;
        private BeatSpearView beatSpearSettingsView;
        private NunchakuView nunchakuView;
        private BeatFlailView beatFlailView;

        private TrackerSelectView trackerSelectView;
        private TrackerPoseView trackerPoseView;

        public bool IsBusy { get; set; }

        public void ResolveViews(DiContainer container)
        {
            this.alternativePlayView = container.Resolve<AlternativePlayView>();
            this.playModeSelectView = container.Resolve<PlayModeSelectView>();
            this.gameModifiersView = container.Resolve<GameModifiersView>();

            this.beatSaberSettingsView = container.Resolve<BeatSaberView>();
            this.darthMaulSettingsView = container.Resolve<DarthMaulView>();
            this.beatSpearSettingsView = container.Resolve<BeatSpearView>();
            this.nunchakuView = container.Resolve<NunchakuView>();
            this.beatFlailView = container.Resolve<BeatFlailView>();

            this.trackerSelectView = container.Resolve<TrackerSelectView>();
            this.trackerPoseView = container.Resolve<TrackerPoseView>();
        }

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

        public void ShowNunchaku()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.nunchakuView, ViewController.AnimationType.In);
            this.IsBusy = false;
        }

        public void ShowBeatFlail()
        {
            this.IsBusy = true;
            this.SetLeftScreenViewController(this.beatFlailView, ViewController.AnimationType.In);
            this.IsBusy = false;
        }

        public void ShowPlayModeSelect(PlayModeSettings settings, int index)
        {
            this.IsBusy = true;

            // Give the settings to all the views first
            this.playModeSelectView.SetPlayModeSettings(settings, index);
            this.beatSaberSettingsView.SetPlayModeSettings(settings);
            this.darthMaulSettingsView.SetPlayModeSettings(settings);
            this.beatSpearSettingsView.SetPlayModeSettings(settings);
            this.beatFlailView.SetPlayModeSettings(settings);
            this.nunchakuView.SetPlayModeSettings(settings);
            this.trackerPoseView.SetPlayModeSettings(settings);
            this.gameModifiersView.SetPlayModeSettings(settings);

            // Display the views
            this.SetTitle("Edit Play Modes");
            this.ReplaceTopViewController(this.playModeSelectView, null, ViewController.AnimationType.In, ViewController.AnimationDirection.Vertical);
            this.SetLeftScreenViewController(this.DecideLeftMainView(this.playModeSelectView.Settings), ViewController.AnimationType.In);
            this.SetRightScreenViewController(this.gameModifiersView, ViewController.AnimationType.In);

            this.IsBusy = false;
        }

        public void DismissPlayModeSelect()
        {
            this.IsBusy = true;

            this.alternativePlayView.RefreshConfigurations(this.playModeSelectView.index);

            this.ReplaceTopViewController(this.alternativePlayView);
            this.SetLeftScreenViewController(null, ViewController.AnimationType.Out);
            this.SetRightScreenViewController(null, ViewController.AnimationType.Out);

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

            this.ReplaceTopViewController(this.playModeSelectView);
            var viewToDisplay = this.DecideLeftMainView(this.playModeSelectView.Settings);
            this.SetLeftScreenViewController(viewToDisplay, ViewController.AnimationType.Out);
            this.SetRightScreenViewController(this.gameModifiersView, ViewController.AnimationType.Out);
            this.IsBusy = false;
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            this.SetTitle(titleString);
            this.showBackButton = true;

            this.IsBusy = true;
            this.trackerSelectView.SetSelectingTracker(new TrackerConfigData());
            this.ProvideInitialViewControllers(this.alternativePlayView);
            this.IsBusy = false;
        }

        private ViewController DecideLeftMainView(PlayModeSettings settings)
        {
            switch (settings.PlayMode)
            {
                case PlayMode.DarthMaul:
                    return this.darthMaulSettingsView;

                case PlayMode.BeatSpear:
                    return this.beatSpearSettingsView;

                case PlayMode.Nunchaku:
                    return this.nunchakuView;

                case PlayMode.BeatFlail:
                    return this.beatFlailView;

                case PlayMode.BeatSaber:
                default:
                    return this.beatSaberSettingsView;
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            if (this.IsBusy) return;

            if (topViewController == this.playModeSelectView)
            {
                this.DismissPlayModeSelect();
                return;
            }

            if (topViewController == this.trackerSelectView)
            {
                this.DismissTrackerSelect();
                return;
            }

            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
