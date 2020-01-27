using AlternativePlay.UI;
using BS_Utils.Utilities;
using Harmony;
using IPA;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlternativePlay
{
    public class Plugin : IBeatSaberPlugin
    {
        public const string assemblyName = "AlternativePlay";
        private const string MenuScene = "MenuCore";
        private const string GameScene = "GameCore";

        private AlternativePlayUI alternativePlayUI;

        private BeatSaberBehavior beatSaberBehavior;
        private DarthMaulBehavior darthMaulBehavior;
        private BeatSpearBehavior beatSpearBehavior;
        private GameModifiersBehavior gameModifiersBehavior;

        private void OnMenuSceneLoadedFresh()
        {
            if (alternativePlayUI == null) alternativePlayUI = new GameObject(nameof(AlternativePlayUI))
                    .AddComponent<AlternativePlayUI>();
            this.alternativePlayUI.CreateUI();
        }

        private void OnGameSceneLoaded()
        {
            if (beatSaberBehavior == null) beatSaberBehavior = new GameObject(nameof(BeatSaberBehavior)).AddComponent<BeatSaberBehavior>();
            beatSaberBehavior.BeginGameCoreScene();

            if (beatSpearBehavior == null) beatSpearBehavior = new GameObject(nameof(BeatSpearBehavior)).AddComponent<BeatSpearBehavior>();
            beatSpearBehavior.BeginGameCoreScene();

            if (darthMaulBehavior == null) darthMaulBehavior = new GameObject(nameof(DarthMaulBehavior)).AddComponent<DarthMaulBehavior>();
            darthMaulBehavior.BeginGameCoreScene();

            if (gameModifiersBehavior == null) gameModifiersBehavior = new GameObject(nameof(GameModifiersBehavior)).AddComponent<GameModifiersBehavior>();
            gameModifiersBehavior.BeginGameCoreScene();

        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
        }

        public void OnApplicationQuit()
        {
        }

        public void OnApplicationStart()
        {
            PersistentSingleton<ConfigOptions>.TouchInstance();

            var harmonyInstance = HarmonyInstance.Create("com.kylon99.beatsaber.alternativeplay");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());

            BSEvents.menuSceneLoadedFresh += this.OnMenuSceneLoadedFresh;
            BSEvents.gameSceneLoaded += this.OnGameSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
        }

        public void OnSceneUnloaded(Scene scene)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

    }
}
