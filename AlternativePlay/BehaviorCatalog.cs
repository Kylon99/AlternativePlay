using UnityEngine;

namespace AlternativePlay
{
    public class BehaviorCatalog
    {
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
        }

        public void LoadMenuBehaviors()
        {
            if (this.FlailBehavior != null) GameObject.Destroy(this.FlailBehavior);
            this.FlailBehavior = null;

            if (this.NunchakuBehavior != null) GameObject.Destroy(this.NunchakuBehavior);
            this.NunchakuBehavior = null;
        }

        public void LoadMenuSceneLoadedFreshBehaviors()
        {
            if (this.ShowTrackersBehavior == null) this.ShowTrackersBehavior = new GameObject(nameof(this.ShowTrackersBehavior)).AddComponent<ShowTrackersBehavior>();
            if (this.AssetLoaderBehavior == null) this.AssetLoaderBehavior = new GameObject(nameof(this.AssetLoaderBehavior)).AddComponent<AssetLoaderBehavior>();
        }

        public void LoadGameSceneLoadedBehaviors()
        {
            if (this.InputManager == null) this.InputManager = new GameObject(nameof(this.InputManager)).AddComponent<InputManager>();
            //this.InputManager.BeginPolling();

            if (this.SaberDeviceManager == null) this.SaberDeviceManager = new GameObject(nameof(this.SaberDeviceManager)).AddComponent<SaberDeviceManager>();
            //this.SaberDeviceManager.BeginGameCoreScene();

            if (this.BeatSaberBehavior == null) this.BeatSaberBehavior = new GameObject(nameof(this.BeatSaberBehavior)).AddComponent<BeatSaberBehavior>();
            //this.BeatSaberBehavior.BeginGameCoreScene();

            if (this.DarthMaulBehavior == null) this.DarthMaulBehavior = new GameObject(nameof(this.DarthMaulBehavior)).AddComponent<DarthMaulBehavior>();
            //this.DarthMaulBehavior.BeginGameCoreScene();

            if (this.BeatSpearBehavior == null) this.BeatSpearBehavior = new GameObject(nameof(this.BeatSpearBehavior)).AddComponent<BeatSpearBehavior>();
            //this.BeatSpearBehavior.BeginGameCoreScene();

            if (this.NunchakuBehavior == null) this.NunchakuBehavior = new GameObject(nameof(this.NunchakuBehavior)).AddComponent<NunchakuBehavior>();
            //this.NunchakuBehavior.BeginGameCoreScene();

            if (this.FlailBehavior == null) this.FlailBehavior = new GameObject(nameof(this.FlailBehavior)).AddComponent<BeatFlailBehavior>();
            //this.FlailBehavior.BeginGameCoreScene();

            if (this.GameModifiersBehavior == null) this.GameModifiersBehavior = new GameObject(nameof(this.GameModifiersBehavior)).AddComponent<GameModifiersBehavior>();
            //this.GameModifiersBehavior.BeginGameCoreScene();
        }
    }
}