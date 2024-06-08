using HarmonyLib;
using UnityEngine;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerLobbyConnectionController), "HandleMultiplayerSessionManagerConnected")]
    public class MultiplayerPatch
    {
        public static MultiplayerLobbyConnectionController.LobbyConnectionType connectionType;
        private static void Postfix(MultiplayerLobbyConnectionController __instance)
        {
            connectionType = __instance.connectionType;
        }
    }

    [HarmonyPatch(typeof(MultiplayerLocalActivePlayerGameplayManager), "Start")]
    public class MultiplayerLocalActivePlayerGameplayManagerPatch
    {
        internal static SaberManager multiplayerSaberManager = null;
        private static void Postfix(MultiplayerLocalActivePlayerGameplayManager __instance, SaberManager ____saberManager)
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

    [HarmonyPatch(typeof(MultiplayerSyncStateManager<NodePoseSyncState, NodePoseSyncState.NodePose, PoseSerializable, NodePoseSyncStateNetSerializable, NodePoseSyncStateDeltaNetSerializable>), "LateUpdate")]
    public class MultiplayerSyncStateManagerPatch
    {
        private static PoseSerializable _leftSaber = new PoseSerializable();
        private static PoseSerializable _rightSaber = new PoseSerializable();
        private static void Prefix(LocalMultiplayerSyncState<NodePoseSyncState, NodePoseSyncState.NodePose, PoseSerializable> ____localState)
        {
            if (_leftSaber.position != Vector3.zero && _rightSaber.position != Vector3.zero)
            {
                ____localState[NodePoseSyncState.NodePose.LeftController] = _leftSaber;
                ____localState[NodePoseSyncState.NodePose.RightController] = _rightSaber;
            }
        }

        internal static void SetMultiplayerLeftSaberPose(Pose leftSaberPose)
        {
            _leftSaber.position = leftSaberPose.position;
            _leftSaber.rotation = leftSaberPose.rotation;
        }

        internal static void SetMultiplayerRightSaberPose(Pose rightSaberPose)
        {
            _rightSaber.position = rightSaberPose.position;
            _rightSaber.rotation = rightSaberPose.rotation;
        }
    }

}
