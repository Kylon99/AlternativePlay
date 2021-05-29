﻿using HarmonyLib;
using UnityEngine;

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

    [HarmonyPatch(typeof(MultiplayerSyncStateManager<NodePoseSyncState, NodePoseSyncState.NodePose, PoseSerializable, NodePoseSyncStateNetSerializable, NodePoseSyncStateDeltaNetSerializable>), "LateUpdate")]
    internal class MultiplayerSyncStateManagerPatch
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

        internal static void SetMultiplayerSaberPositionAndRotate(Saber leftSaber, Saber rightSaber)
        {
            _leftSaber.position = leftSaber.transform.position;
            _leftSaber.rotation = leftSaber.transform.rotation;
            _rightSaber.position = rightSaber.transform.position;
            _rightSaber.rotation = rightSaber.transform.rotation;
        }
    }

}
