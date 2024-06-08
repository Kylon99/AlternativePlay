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
            this.Container.Bind<AssetLoaderBehavior>().AsSingle().NonLazy();
            this.Container.Bind<ShowTrackersBehavior>().AsSingle().NonLazy();

            // Manual Inject Harmony Patches
            BeatFlailNoteMoverPatch.Configuration = configuration;
            DarthMaulHapticPatch.Configuration = configuration;
            NunchakuHapticPatch.Configuration = configuration;
            OneColorSaberPatch.Configuration = configuration;
            TouchNotesPatch.Configuration = configuration;
        }
    }
}
