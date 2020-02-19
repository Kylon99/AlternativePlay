using Harmony;
using UnityEngine.XR;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(HapticFeedbackController.Rumble))]
    [HarmonyPatch(typeof(HapticFeedbackController))]
    internal class DarthMaulHapticPatch
    {
        private static void Prefix(HapticFeedbackController __instance, ref XRNode node)
        {
            if (ConfigOptions.instance.PlayMode != PlayMode.DarthMaul ||
                ConfigOptions.instance.DarthMaulControllerCount != ControllerCountEnum.One ||
                DarthMaulBehavior.Split)
            {
                // Let the original function handle the haptic feedback
                return;
            }

            if (!ConfigOptions.instance.UseLeftController && node == XRNode.LeftHand)
            {
                node = XRNode.RightHand;
            }

            if (ConfigOptions.instance.UseLeftController && node == XRNode.RightHand)
            {
                node = XRNode.LeftHand;
            }
        }
    }
}
