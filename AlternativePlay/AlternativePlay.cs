using AlternativePlay.Models;
using BS_Utils.Utilities;
using HarmonyLib;
using IPA;
using System.Reflection;
using AlternativePlay.HarmonyPatches;

namespace AlternativePlay
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class AlternativePlay
    {
        public const string assemblyName = "AlternativePlay";

        public static IPA.Logging.Logger Logger { get; private set; }

        [Init]
        public AlternativePlay(IPA.Logging.Logger logger)
        {
            AlternativePlay.Logger = logger;
        }

        [OnStart]
        public void Start()
        {
            PersistentSingleton<Configuration>.TouchInstance();
            Configuration.instance.LoadConfiguration();

            var harmonyInstance = new Harmony("com.kylon99.beatsaber.alternativeplay");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            PersistentSingleton<BehaviorCatalog>.TouchInstance();
            BehaviorCatalog.instance.LoadStartBehaviors();

            BSEvents.gameSceneLoaded += this.OnGameSceneLoaded;
            BSEvents.menuSceneLoaded += this.OnMenuSceneLoaded;
            BSEvents.lateMenuSceneLoadedFresh += this.LateMenuSceneLoadedFresh;
        }

        private void OnMenuSceneLoaded()
        {
            BehaviorCatalog.instance.LoadMenuBehaviors();
        }

        private void LateMenuSceneLoadedFresh(ScenesTransitionSetupDataSO obj)
        {
            BehaviorCatalog.instance.LoadMenuSceneLoadedFreshBehaviors();
        }

        private void OnGameSceneLoaded()
        {
            if ((BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Multiplayer) ||
                (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer && MultiplayerPatch.connectionType != MultiplayerLobbyConnectionController.LobbyConnectionType.QuickPlay))
            {
                BehaviorCatalog.instance.LoadGameSceneLoadedBehaviors();
            }
        }
    }
}
