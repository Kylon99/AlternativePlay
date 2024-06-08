using AlternativePlay.Models;
using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(NoteBasicCutInfoHelper.GetBasicCutInfo))]
    [HarmonyPatch(typeof(NoteBasicCutInfoHelper))]
    public class TouchNotesPatch
    {
        public static Configuration Configuration { get; set; }

        private static void Prefix(ref float saberBladeSpeed)
        {
            if (Configuration.Current.TouchNotes && BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Multiplayer)
            {
                // Set the saber speed artificially to 3.0f, which is greater than the minimum 2.0f 
                // constant in the NoteBasicCutInfo class.
                saberBladeSpeed = 3.0f;
            }
        }
    }
}
