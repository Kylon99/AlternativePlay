using AlternativePlay.HarmonyPatches;
using AlternativePlay.Installers;
using AlternativePlay.Models;
using BS_Utils.Utilities;
using HarmonyLib;
using IPA;
using System.Reflection;
using Zenject;

namespace AlternativePlay
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class AlternativePlay
    {
        public const string assemblyName = "AlternativePlay";

        public static DiContainer ProjectContainer { get; private set; }
        public static IPA.Logging.Logger Logger { get; private set; }

        [Init]
        public AlternativePlay(IPA.Logging.Logger logger)
        {
            Logger = logger;
            ProjectContainer = ProjectContext.Instance.Container;
            ProjectContainer.Install<AlternativePlayInstaller>();
        }

        [OnStart]
        public void Start()
        {
            ProjectContainer.Resolve<Configuration>().LoadConfiguration();

            var harmonyInstance = new Harmony("com.kylon99.beatsaber.alternativeplay");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            BSEvents.gameSceneLoaded += this.OnGameSceneLoaded;
        }

        private void OnGameSceneLoaded()
        {
            if ((BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Multiplayer) ||
                (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer && MultiplayerPatch.connectionType != MultiplayerLobbyConnectionController.LobbyConnectionType.QuickPlay))
            {
                // ProjectContainer.TryResolve<BehaviorCatalog>().LoadGameSceneLoadedBehaviors();
            }
        }
    }
}
