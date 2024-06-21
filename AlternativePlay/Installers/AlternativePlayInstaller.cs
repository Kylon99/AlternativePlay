using AlternativePlay.HarmonyPatches;
using AlternativePlay.Models;
using Zenject;

namespace AlternativePlay.Installers
{
    public class AlternativePlayInstaller : Installer
    {
        public override void InstallBindings()
        {
            var configuration = new Configuration();
            this.Container.Bind<Configuration>().FromInstance(configuration);
            this.Container.Bind<TrackedDeviceManager>().AsSingle().NonLazy();
            var assetLoaderBehavior = this.Container.InstantiateComponentOnNewGameObject<AssetLoaderBehavior>();
            this.Container.Bind<AssetLoaderBehavior>().FromInstance(assetLoaderBehavior).AsSingle();
            var showTrackersBehavior = this.Container.InstantiateComponentOnNewGameObject<ShowTrackersBehavior>();
            this.Container.Bind<ShowTrackersBehavior>().FromInstance(showTrackersBehavior).AsSingle();

            // Manually Inject Harmony Patches
            BeatFlailNoteMoverPatch.Configuration = configuration;
            DarthMaulHapticPatch.Configuration = configuration;
            NunchakuHapticPatch.Configuration = configuration;
            OneColorSaberPatch.Configuration = configuration;
            TouchNotesPatch.Configuration = configuration;
        }
    }
}
