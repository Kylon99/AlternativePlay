using AlternativePlay.Models;
using HarmonyLib;
using UnityEngine.XR;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(HapticFeedbackManager.PlayHapticFeedback))]
    [HarmonyPatch(typeof(HapticFeedbackManager))]
    [HarmonyPriority(Priority.VeryHigh)]
    public class NunchakuHapticPatch
    {
        public static Configuration Configuration { get; set; }

        public static NunchakuBehavior NunchakuBehavior { get; set; }

        private static void Prefix(ref XRNode node)
        {
            if (Configuration.Current.PlayMode != PlayMode.Nunchaku ||NunchakuBehavior == null || NunchakuBehavior.HeldState == NunchakuBehavior.Held.Both)
            {
                // Let the original function handle the haptic feedback
                return;
            }

            switch (NunchakuBehavior.HeldState)
            {
                case NunchakuBehavior.Held.Left:
                    node = XRNode.LeftHand;
                    break;
                case NunchakuBehavior.Held.Right:
                    node = XRNode.RightHand;
                    break;
            }
        }
    }
}
