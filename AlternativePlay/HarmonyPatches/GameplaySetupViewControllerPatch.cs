using AlternativePlay.UI;
using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(GameplaySetupViewController), nameof(GameplaySetupViewController.RefreshContent))]
    public class GameplaySetupViewControllerPatch
    {
        public static PlayModeSelectTab PlayModeSelectTab { get; set; }

        public static void Postfix()
        {
            // Update the PlayModeSelectTab whenever the GameplaySetup tabs are refreshed
            PlayModeSelectTab.UpdatePlayModeSelectList();
        }
    }
}
