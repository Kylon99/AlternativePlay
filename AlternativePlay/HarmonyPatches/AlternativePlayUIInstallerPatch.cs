using AlternativePlay.Installers;
using HarmonyLib;
using Zenject;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(MainSettingsMenuViewControllersInstaller.InstallBindings))]
    [HarmonyPatch(typeof(MainSettingsMenuViewControllersInstaller))]
    [HarmonyPriority(Priority.Normal)]
    public class AlternativePlayUIInstallerPatch
    {
        private static void Postfix(MainSettingsMenuViewControllersInstaller __instance)
        {
            var container = typeof(MainSettingsMenuViewControllersInstaller).GetProperty("Container", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(__instance) as DiContainer;
            container.Install<AlternativePlayUIInstaller>();
        }
    }
}
