using AlternativePlay.HarmonyPatches;
using Zenject;

namespace AlternativePlay.Installers
{
    public class GameSceneBehaviorsInstaller : Installer
    {
        public override void InstallBindings()
        {
            var inputManager = this.Container.InstantiateComponentOnNewGameObject<InputManager>();
            this.Container.Bind<InputManager>().FromInstance(inputManager).AsSingle();

            var saberDeviceManager = this.Container.InstantiateComponentOnNewGameObject<SaberDeviceManager>();
            this.Container.Bind<SaberDeviceManager>().FromInstance(saberDeviceManager).AsSingle();

            var beatSaberBehavior = this.Container.InstantiateComponentOnNewGameObject<BeatSaberBehavior>();
            this.Container.Bind<BeatSaberBehavior>().FromInstance(beatSaberBehavior).AsSingle();

            var darthMaulBehavior = this.Container.InstantiateComponentOnNewGameObject<DarthMaulBehavior>();
            this.Container.Bind<DarthMaulBehavior>().FromInstance(darthMaulBehavior).AsSingle();

            var beatSpearBehavior = this.Container.InstantiateComponentOnNewGameObject<BeatSpearBehavior>();
            this.Container.Bind<BeatSpearBehavior>().FromInstance(beatSpearBehavior).AsSingle();

            var nunchakuBehavior = this.Container.InstantiateComponentOnNewGameObject<NunchakuBehavior>();
            this.Container.Bind<NunchakuBehavior>().FromInstance(nunchakuBehavior).AsSingle();

            var flailBehavior = this.Container.InstantiateComponentOnNewGameObject<BeatFlailBehavior>();
            this.Container.Bind<BeatFlailBehavior>().FromInstance(flailBehavior).AsSingle();

            var gameModifiersBehavior = this.Container.InstantiateComponentOnNewGameObject<GameModifiersBehavior>();
            this.Container.Bind<GameModifiersBehavior>().FromInstance(gameModifiersBehavior).AsSingle();

            // Manual Inject Harmony Patches
            NunchakuHapticPatch.NunchakuBehavior = nunchakuBehavior;
            DarthMaulHapticPatch.DarthMaulBehavior = darthMaulBehavior;
        }
    }
}