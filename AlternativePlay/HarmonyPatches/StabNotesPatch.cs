using Harmony;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(NoteBasicCutInfo.GetBasicCutInfo))]
    [HarmonyPatch(typeof(NoteBasicCutInfo))]
    internal class StabNotesPatch
    {
        private static void Prefix(ref float saberBladeSpeed)
        {
            if (ConfigOptions.instance.StabNotes)
            {
                // Set the saber speed artificially to 3.0f, which is greater than the minimum 2.0f 
                // constant in the NoteBasicCutInfo class.
                saberBladeSpeed = 3.0f;
            }
        }
    }
}
