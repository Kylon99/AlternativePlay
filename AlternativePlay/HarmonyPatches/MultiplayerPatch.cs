using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerLobbyConnectionController), "HandleMultiplayerSessionManagerConnected")]
    class MultiplayerPatch
    {
        public static MultiplayerLobbyConnectionController.LobbyConnectionType connectionType;
        static void Postfix(MultiplayerLobbyConnectionController __instance)
        {
            //AlternativePlay.Logger.Notice($"Patch MultiplayerLobbyConnenctionController {__instance.connectionState.ToString()}");
            //AlternativePlay.Logger.Notice($"Patch MultiplayerLobbyConnenctionController {__instance.connectionType.ToString()}");
            connectionType = __instance.connectionType;
        }
    }
}
