using HarmonyLib;
using UnityEngine.XR;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(HapticFeedbackController.HitNote))]
    [HarmonyPatch(typeof(HapticFeedbackController))]
    internal class DarthMaulHapticPatch
    {
        private static void Prefix(HapticFeedbackController __instance, ref XRNode node)
        {
            if (ConfigOptions.instance.PlayMode != PlayMode.DarthMaul ||
                DarthMaulBehavior.Split)
            {
                // Let the original function handle the haptic feedback
                return;
            }

            if (ConfigOptions.instance.DarthMaulControllerCount == ControllerCountEnum.One)
            {
                if (!ConfigOptions.instance.UseLeftController && node == XRNode.LeftHand)
                {
                    // Using right controller, move left hits to right hand
                    node = XRNode.RightHand;
                }

                if (ConfigOptions.instance.UseLeftController && node == XRNode.RightHand)
                {
                    // Using left controller, move right hits to left hand
                    node = XRNode.LeftHand;
                }

                return;
            }

            if (ConfigOptions.instance.DarthMaulControllerCount == ControllerCountEnum.Two && ConfigOptions.instance.ReverseMaulDirection)
            {
                // If reversing direction with two controller then always swap hands
                if (node == XRNode.LeftHand)
                {
                    node = XRNode.RightHand;
                }

                if (node == XRNode.RightHand)
                {
                    node = XRNode.LeftHand;
                }
            }
        }
    }
}
