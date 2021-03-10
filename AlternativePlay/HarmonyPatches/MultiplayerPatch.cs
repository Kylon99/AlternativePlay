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
}
