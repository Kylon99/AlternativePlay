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
    public class BeatFlailNoteMoverPatch
    {
        public static Configuration Configuration { get; set; }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NoteMovement), nameof(NoteMovement.Init))]
        [HarmonyPriority(Priority.High)]
        private static void NoteMovementPrefix(ref NoteSpawnData noteSpawnData)
        {
            if (Configuration.Current.PlayMode == PlayMode.BeatFlail && Configuration.Current.MoveNotesBack > 0)
            {
                float realMoveNote = Configuration.Current.MoveNotesBack / 100.0f;
                noteSpawnData = new NoteSpawnData(noteSpawnData.moveStartOffset with
                {
                    z = noteSpawnData.moveStartOffset.z - realMoveNote
                }, noteSpawnData.moveEndOffset with
                {
                    z = noteSpawnData.moveEndOffset.z - realMoveNote
                }, noteSpawnData.jumpEndOffset with
                {
                    z = noteSpawnData.jumpEndOffset.z - realMoveNote
                }, noteSpawnData.gravityBase);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
        [HarmonyPriority(Priority.High)]
        private static void ObstacleControllerPrefix(ref ObstacleSpawnData obstacleSpawnData)
        {
            if (Configuration.Current.PlayMode == PlayMode.BeatFlail && Configuration.Current.MoveNotesBack > 0)
            {
                float realMoveNote = Configuration.Current.MoveNotesBack / 100.0f;
                obstacleSpawnData = new ObstacleSpawnData(obstacleSpawnData.moveOffset with
                {
                    z = obstacleSpawnData.moveOffset.z - realMoveNote
                }, obstacleSpawnData.obstacleWidth, obstacleSpawnData.obstacleHeight);
            }
        }
    }
}