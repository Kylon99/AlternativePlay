using AlternativePlay.Models;
using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorManager), "ColorForSaberType")]
    static class ColorManagerColorForSaberType
    {
        static void Prefix(ref SaberType type)
        {
            var config = Configuration.instance.ConfigurationData;
            if (config.OneColor && type == SaberType.SaberA)
            {
                type = SaberType.SaberB;
            }
        }

        [HarmonyPatch(typeof(ColorManager), "EffectsColorForSaberType")]
        static class ColorManagerEffectsColorForSaberType
        {
            static void Prefix(ref SaberType type)
            {
                var config = Configuration.instance.ConfigurationData;
                if (config.OneColor && type == SaberType.SaberA)
                {
                    type = SaberType.SaberB;
                }
            }
        }
    }
}