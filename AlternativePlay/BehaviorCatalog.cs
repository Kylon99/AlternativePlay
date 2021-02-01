using AlternativePlay.UI;
using UnityEngine;

namespace AlternativePlay
{
    public class BehaviorCatalog : PersistentSingleton<BehaviorCatalog>
    {
        public AlternativePlayUI AlternativePlayUI { get; set; }
        public InputManager InputManager { get; private set; }
        public ControllerManager ControllerManager { get; private set; }
        public ShowTrackersBehavior ShowTrackersBehavior { get; private set; }
        public BeatSaberBehavior BeatSaberBehavior { get; private set; }
        public DarthMaulBehavior DarthMaulBehavior { get; private set; }
        public BeatSpearBehavior BeatSpearBehavior { get; private set; }
        public GameModifiersBehavior GameModifiersBehavior { get; private set; }

        public void LoadStartBehaviors()
        {
            if (AlternativePlayUI == null) AlternativePlayUI = new GameObject(nameof(AlternativePlayUI)).AddComponent<AlternativePlayUI>();
        }

        public void LoadMenuBehaviors()
        {
        }

        public void LoadMenuSceneLoadedFreshBehaviors()
        {
            if (ShowTrackersBehavior == null) ShowTrackersBehavior = new GameObject(nameof(ShowTrackersBehavior)).AddComponent<ShowTrackersBehavior>();
        }

        public void LoadGameSceneLoadedBehaviors()
        {
            if (InputManager == null) InputManager = new GameObject(nameof(InputManager)).AddComponent<InputManager>();
            InputManager.BeginPolling();

            if (ControllerManager == null) ControllerManager = new GameObject(nameof(ControllerManager)).AddComponent<ControllerManager>();
            ControllerManager.BeginGameCoreScene();

            if (BeatSaberBehavior == null) BeatSaberBehavior = new GameObject(nameof(BeatSaberBehavior)).AddComponent<BeatSaberBehavior>();
            BeatSaberBehavior.BeginGameCoreScene();

            if (BeatSpearBehavior == null) BeatSpearBehavior = new GameObject(nameof(BeatSpearBehavior)).AddComponent<BeatSpearBehavior>();
            BeatSpearBehavior.BeginGameCoreScene();

            if (DarthMaulBehavior == null) DarthMaulBehavior = new GameObject(nameof(DarthMaulBehavior)).AddComponent<DarthMaulBehavior>();
            DarthMaulBehavior.BeginGameCoreScene();

            if (GameModifiersBehavior == null) GameModifiersBehavior = new GameObject(nameof(GameModifiersBehavior)).AddComponent<GameModifiersBehavior>();
            GameModifiersBehavior.BeginGameCoreScene();
        }
    }
}
