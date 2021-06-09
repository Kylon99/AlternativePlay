using AlternativePlay.Models;
using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    /// <summary>
    /// Patches the <see cref="NoteBasicCutInfoHelper.GetBasicCutInfo"/> method so that the
    /// check for saber type always succeeds.  This patch is <see cref="Priority.VeryHigh"/>
    /// since it is purposely set on and off by the user to play One Color.
    /// </summary>
    [HarmonyPatch(nameof(NoteBasicCutInfoHelper.GetBasicCutInfo))]
    [HarmonyPatch(typeof(NoteBasicCutInfoHelper))]
    [HarmonyPriority(Priority.VeryHigh)]
    internal class OneColorSaberPatch
    {
        private static void Postfix(ref bool saberTypeOK)
        {
            if (Configuration.instance.ConfigurationData.OneColor)
            {
                // Always allow saber hits from any type for OneColor
                saberTypeOK = true;
            }
        }
    }
}
