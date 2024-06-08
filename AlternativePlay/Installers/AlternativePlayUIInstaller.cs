using AlternativePlay.HarmonyPatches;
using AlternativePlay.UI;
using BeatSaberMarkupLanguage;
using Zenject;

namespace AlternativePlay.Installers
{
    public class AlternativePlayUIInstaller : Installer
    {
        public override void InstallBindings()
        {
            var mainFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<AlternativePlayMainFlowCoordinator>();
            this.Container.Bind<AlternativePlayMainFlowCoordinator>().FromInstance(mainFlowCoordinator).AsSingle();

            var alternativePlayUI = this.Container.InstantiateComponentOnNewGameObject<AlternativePlayUI>();
            this.Container.Bind<AlternativePlayUI>().FromInstance(alternativePlayUI).AsSingle();
            var playModeSelectTab = this.Container.InstantiateComponentOnNewGameObject<PlayModeSelectTab>();
            this.Container.Bind<PlayModeSelectTab>().FromInstance(playModeSelectTab).AsSingle();

            this.Container.Bind<AlternativePlayView>().FromInstance(BeatSaberUI.CreateViewController<AlternativePlayView>()).AsSingle();
            this.Container.Bind<PlayModeSelectView>().FromInstance(BeatSaberUI.CreateViewController<PlayModeSelectView>()).AsSingle();
            this.Container.Bind<GameModifiersView>().FromInstance(BeatSaberUI.CreateViewController<GameModifiersView>()).AsSingle();

            this.Container.Bind<BeatSaberView>().FromInstance(BeatSaberUI.CreateViewController<BeatSaberView>()).AsSingle();
            this.Container.Bind<DarthMaulView>().FromInstance(BeatSaberUI.CreateViewController<DarthMaulView>()).AsSingle();
            this.Container.Bind<BeatSpearView>().FromInstance(BeatSaberUI.CreateViewController<BeatSpearView>()).AsSingle();
            this.Container.Bind<NunchakuView>().FromInstance(BeatSaberUI.CreateViewController<NunchakuView>()).AsSingle();
            this.Container.Bind<BeatFlailView>().FromInstance(BeatSaberUI.CreateViewController<BeatFlailView>()).AsSingle();

            this.Container.Bind<TrackerSelectView>().FromInstance(BeatSaberUI.CreateViewController<TrackerSelectView>()).AsSingle();
            this.Container.Bind<TrackerPoseView>().FromInstance(BeatSaberUI.CreateViewController<TrackerPoseView>()).AsSingle();

            mainFlowCoordinator.ResolveViews(this.Container);

            // Manually Inject Haptic Patches
            GameplaySetupViewControllerPatch.PlayModeSelectTab = playModeSelectTab;
        }
    }
}
