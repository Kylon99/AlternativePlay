using AlternativePlay.Models;
using HarmonyLib;
using UnityEngine;

namespace AlternativePlay.HarmonyPatches
{
    /// <summary>
    /// Patches the <see cref="NoteMovement"/> and <see cref="ObstacleController"/>
    /// classes so that notes are moved back (+Z) a certain distance to account 
    /// for the length of the Flail chain.
    /// </summary>
    [HarmonyPatch]
    internal class BeatFlailNoteMoverPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NoteMovement), nameof(NoteMovement.Init))]
        [HarmonyPriority(Priority.High)]
        private static void NoteMovementPrefix(ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos)
        {
            if (Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatFlail &&
                Configuration.instance.ConfigurationData.MoveNotesBack > 0)
            {
                float realMoveNote = Configuration.instance.ConfigurationData.MoveNotesBack / 100.0f;
                moveStartPos.z -= realMoveNote;
                moveEndPos.z -= realMoveNote;
                jumpEndPos.z -= realMoveNote;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
        [HarmonyPriority(Priority.High)]
        private static void ObstacleControllerPrefix(ref Vector3 startPos, ref Vector3 midPos, ref Vector3 endPos)
        {
            if (Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatFlail &&
                Configuration.instance.ConfigurationData.MoveNotesBack > 0)
            {
                float realMoveNote = Configuration.instance.ConfigurationData.MoveNotesBack / 100.0f;
                startPos.z -= realMoveNote;
                midPos.z -= realMoveNote;
                endPos.z -= realMoveNote;
            }
        }
    }
}
