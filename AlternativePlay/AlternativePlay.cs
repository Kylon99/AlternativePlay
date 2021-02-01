using AlternativePlay.Models;
using BS_Utils.Utilities;
using HarmonyLib;
using IPA;
using IPA.Loader;
using System.Reflection;

namespace AlternativePlay
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class AlternativePlay
    {
        public const string assemblyName = "AlternativePlay";

        public static IPA.Logging.Logger Logger { get; private set; }

        private string privateMultiModName = "BeatTogether";

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
            if ((BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Multiplayer) 
                || (PluginManager.GetPlugin(privateMultiModName).Name == privateMultiModName && BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer))
            {
                BehaviorCatalog.instance.LoadGameSceneLoadedBehaviors();
            }
        }
    }
}
