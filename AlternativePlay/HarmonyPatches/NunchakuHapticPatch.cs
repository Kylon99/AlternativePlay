using AlternativePlay.Models;
using HarmonyLib;
using UnityEngine.XR;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(HapticFeedbackController.PlayHapticFeedback))]
    [HarmonyPatch(typeof(HapticFeedbackController))]
    [HarmonyPriority(Priority.VeryHigh)]
    internal class NunchakuHapticPatch
    {
        private static void Prefix(ref XRNode node)
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.Nunchaku ||
                BehaviorCatalog.instance.NunchakuBehavior == null ||
                BehaviorCatalog.instance.NunchakuBehavior.HeldState == NunchakuBehavior.Held.Both)
            {
                // Let the original function handle the haptic feedback
                return;
            }

            switch (BehaviorCatalog.instance.NunchakuBehavior.HeldState)
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
