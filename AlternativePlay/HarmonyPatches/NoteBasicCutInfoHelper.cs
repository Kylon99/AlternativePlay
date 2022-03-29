using AlternativePlay.Models;
using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteBasicCutInfoHelper), "GetBasicCutInfo")]
    static class NoteBasicCutInfoHelperGetBasicCutInfo
    {
        static void Postfix(ColorType colorType, ref bool saberTypeOK)
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.OneColor)
            {
                if ((colorType != ColorType.None))
                {
                    saberTypeOK = true;
                }
            }
        }
    }
}