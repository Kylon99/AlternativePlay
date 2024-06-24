using AlternativePlay.Installers;
using HarmonyLib;
using Zenject;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(GameplayCoreInstaller.InstallBindings))]
    [HarmonyPatch(typeof(GameplayCoreInstaller))]
    [HarmonyPriority(Priority.Normal)]
    internal class GameplayCoreInstallerPatch
    {
        private static void Postfix(GameplayCoreInstaller __instance)
        {
            var container = typeof(GameplayCoreInstaller).GetProperty("Container", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(__instance) as DiContainer;
            container.Install<GameSceneBehaviorsInstaller>();
        }
    }
}
