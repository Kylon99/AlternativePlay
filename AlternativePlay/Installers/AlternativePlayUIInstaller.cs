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
            var alternativePlayUI = this.Container.InstantiateComponentOnNewGameObject<AlternativePlayUI>();
            this.Container.Bind<AlternativePlayUI>().FromInstance(alternativePlayUI).AsSingle();
            var playModeSelectTab = this.Container.InstantiateComponentOnNewGameObject<PlayModeSelectTab>();
            this.Container.Bind<PlayModeSelectTab>().FromInstance(playModeSelectTab).AsSingle();

            // Manually Inject Harmony Patches
            GameplaySetupViewControllerPatch.PlayModeSelectTab = playModeSelectTab;
        }
    }
}
