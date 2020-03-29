using AlternativePlay.UI;
using BS_Utils.Utilities;
using HarmonyLib;
using IPA;
using System.Reflection;
using UnityEngine;

namespace AlternativePlay
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class AlternativePlay
    {
        public const string assemblyName = "AlternativePlay";

        public static IPA.Logging.Logger Logger { get; private set; }

        private AlternativePlayUI alternativePlayUI;

        private InputManager inputManager;
        private BeatSaberBehavior beatSaberBehavior;
        private DarthMaulBehavior darthMaulBehavior;
        private BeatSpearBehavior beatSpearBehavior;
        private GameModifiersBehavior gameModifiersBehavior;


        [Init]
        public AlternativePlay(IPA.Logging.Logger logger)
        {
            AlternativePlay.Logger = logger;
        }

        [OnStart]
        public void Start()
        {
            PersistentSingleton<ConfigOptions>.TouchInstance();

            var harmonyInstance = new Harmony("com.kylon99.beatsaber.alternativeplay");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            if (alternativePlayUI == null) alternativePlayUI = new GameObject(nameof(AlternativePlayUI))
                    .AddComponent<AlternativePlayUI>();

            BSEvents.gameSceneLoaded += this.OnGameSceneLoaded;
            BSEvents.menuSceneLoaded += this.OnMenuSceneLoaded;
        }

        private void OnMenuSceneLoaded()
        {
            if (inputManager != null) inputManager.DisableInput();
        }

        private void OnGameSceneLoaded()
        {
            if (inputManager == null) inputManager = new GameObject(nameof(InputManager)).AddComponent<InputManager>();
            inputManager.BeginGameCoreScene();

            if (beatSaberBehavior == null) beatSaberBehavior = new GameObject(nameof(BeatSaberBehavior)).AddComponent<BeatSaberBehavior>();
            beatSaberBehavior.BeginGameCoreScene();

            if (beatSpearBehavior == null) beatSpearBehavior = new GameObject(nameof(BeatSpearBehavior)).AddComponent<BeatSpearBehavior>();
            beatSpearBehavior.BeginGameCoreScene(this.inputManager);

            if (darthMaulBehavior == null) darthMaulBehavior = new GameObject(nameof(DarthMaulBehavior)).AddComponent<DarthMaulBehavior>();
            darthMaulBehavior.BeginGameCoreScene(this.inputManager);

            if (gameModifiersBehavior == null) gameModifiersBehavior = new GameObject(nameof(GameModifiersBehavior)).AddComponent<GameModifiersBehavior>();
            gameModifiersBehavior.BeginGameCoreScene();

        }
    }
}
