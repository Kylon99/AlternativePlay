using AlternativePlay.Models;
using HarmonyLib;

namespace AlternativePlay.HarmonyPatches
{
    /// <summary>
    /// Patches the <see cref="NoteBasicCutInfoHelper.GetBasicCutInfo"/> method so that the
    /// check for saber type always succeeds.  This patch is <see cref="Priority.VeryHigh"/>
    /// since it is purposely set on and off by the user to play One Color or Spear.
    /// </summary>
    [HarmonyPatch(nameof(NoteBasicCutInfoHelper.GetBasicCutInfo))]
    [HarmonyPatch(typeof(NoteBasicCutInfoHelper))]
    [HarmonyPriority(Priority.VeryHigh)]
    public class OneColorSaberPatch
    {
        public static Configuration Configuration { get; set; }

        private static void Postfix(ref bool saberTypeOK)
        {
            if (Configuration.Current.PlayMode == PlayMode.BeatSpear ||
                // Allow multicutting if playing a one handed style
                (Configuration.Current.PlayMode == PlayMode.BeatFlail && (Configuration.Current.LeftFlailMode == BeatFlailMode.None || Configuration.Current.RightFlailMode == BeatFlailMode.None)) || 
                (Configuration.Current.OneColor && BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Multiplayer))
            {
                // Always allow saber hits from any type for OneColor or Spear
                saberTypeOK = true;
            }
        }
    }
}
