using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerLobbyConnectionController), "HandleMultiplayerSessionManagerConnected")]
    internal class MultiplayerPatch
    {
        public static MultiplayerLobbyConnectionController.LobbyConnectionType connectionType;
        private static void Postfix(MultiplayerLobbyConnectionController __instance)
        {
            connectionType = __instance.connectionType;
        }
    }

    [HarmonyPatch(typeof(MultiplayerLocalActivePlayerGameplayManager), "Start")]
    internal class MultiplayerLocalActivePlayerGameplayManagerPatch
    {
        internal static SaberManager multiplayerSaberManager=null;
        private static void Postfix(MultiplayerLocalActivePlayerGameplayManager __instance,SaberManager ____saberManager)
        {
            multiplayerSaberManager = ____saberManager;
#if DEBUG
            AlternativePlay.Logger.Notice($"SaberManager Set in MultiplayerLocalActivePlayerGameplayManager");
#endif
        }

        [HarmonyPatch(typeof(MultiplayerLocalActivePlayerGameplayManager), "OnDisable")]
        private static void Prefix()
        {
            multiplayerSaberManager = null;
        }
    }
}
