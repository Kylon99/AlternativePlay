using AlternativePlay.UI;
using UnityEngine;

namespace AlternativePlay
{
    public class BehaviorCatalog : PersistentSingleton<BehaviorCatalog>
    {
        public AlternativePlayUI AlternativePlayUI { get; set; }
        public InputManager InputManager { get; private set; }
        public SaberDeviceManager SaberDeviceManager { get; private set; }
        public ShowTrackersBehavior ShowTrackersBehavior { get; private set; }
        public BeatSaberBehavior BeatSaberBehavior { get; private set; }
        public DarthMaulBehavior DarthMaulBehavior { get; private set; }
        public BeatSpearBehavior BeatSpearBehavior { get; private set; }
        public NunchakuBehavior NunchakuBehavior { get; private set; }
        public BeatFlailBehavior FlailBehavior { get; private set; }
        public GameModifiersBehavior GameModifiersBehavior { get; private set; }
        public AssetLoaderBehavior AssetLoaderBehavior { get; private set; }

        public void LoadStartBehaviors()
        {
            if (AlternativePlayUI == null) AlternativePlayUI = new GameObject(nameof(AlternativePlayUI)).AddComponent<AlternativePlayUI>();
        }

        public void LoadMenuBehaviors()
        {
            if (FlailBehavior != null) GameObject.Destroy(FlailBehavior);
            FlailBehavior = null;

            if (NunchakuBehavior != null) GameObject.Destroy(NunchakuBehavior);
            NunchakuBehavior = null;
        }

        public void LoadMenuSceneLoadedFreshBehaviors()
        {
            if (ShowTrackersBehavior == null) ShowTrackersBehavior = new GameObject(nameof(ShowTrackersBehavior)).AddComponent<ShowTrackersBehavior>();
            if (AssetLoaderBehavior == null) AssetLoaderBehavior = new GameObject(nameof(AssetLoaderBehavior)).AddComponent<AssetLoaderBehavior>();
        }

        public void LoadGameSceneLoadedBehaviors()
        {
            if (InputManager == null) InputManager = new GameObject(nameof(InputManager)).AddComponent<InputManager>();
            InputManager.BeginPolling();

            if (SaberDeviceManager == null) SaberDeviceManager = new GameObject(nameof(SaberDeviceManager)).AddComponent<SaberDeviceManager>();
            SaberDeviceManager.BeginGameCoreScene();

            if (BeatSaberBehavior == null) BeatSaberBehavior = new GameObject(nameof(BeatSaberBehavior)).AddComponent<BeatSaberBehavior>();
            BeatSaberBehavior.BeginGameCoreScene();

            if (DarthMaulBehavior == null) DarthMaulBehavior = new GameObject(nameof(DarthMaulBehavior)).AddComponent<DarthMaulBehavior>();
            DarthMaulBehavior.BeginGameCoreScene();

            if (BeatSpearBehavior == null) BeatSpearBehavior = new GameObject(nameof(BeatSpearBehavior)).AddComponent<BeatSpearBehavior>();
            BeatSpearBehavior.BeginGameCoreScene();

            if (NunchakuBehavior == null) NunchakuBehavior = new GameObject(nameof(NunchakuBehavior)).AddComponent<NunchakuBehavior>();
            NunchakuBehavior.BeginGameCoreScene();

            if (FlailBehavior == null) FlailBehavior = new GameObject(nameof(FlailBehavior)).AddComponent<BeatFlailBehavior>();
            FlailBehavior.BeginGameCoreScene();

            if (GameModifiersBehavior == null) GameModifiersBehavior = new GameObject(nameof(GameModifiersBehavior)).AddComponent<GameModifiersBehavior>();
            GameModifiersBehavior.BeginGameCoreScene();
        }
    }
}