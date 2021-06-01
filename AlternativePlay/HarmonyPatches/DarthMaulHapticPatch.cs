using AlternativePlay.Models;
using HarmonyLib;
using UnityEngine.XR;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(HapticFeedbackController.PlayHapticFeedback))]
    [HarmonyPatch(typeof(HapticFeedbackController))]
    internal class DarthMaulHapticPatch
    {
        private static void Prefix(HapticFeedbackController __instance, ref XRNode node)
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul ||
                BehaviorCatalog.instance.DarthMaulBehavior == null ||
                BehaviorCatalog.instance.DarthMaulBehavior.Split)
            {
                // Let the original function handle the haptic feedback
                return;
            }

            if (Configuration.instance.ConfigurationData.DarthMaulControllerCount == ControllerCountEnum.One)
            {
                if (!Configuration.instance.ConfigurationData.UseLeftController && node == XRNode.LeftHand)
                {
                    // Using right controller, move left hits to right hand
                    node = XRNode.RightHand;
                }

                if (Configuration.instance.ConfigurationData.UseLeftController && node == XRNode.RightHand)
                {
                    // Using left controller, move right hits to left hand
                    node = XRNode.LeftHand;
                }

                return;
            }

            if (Configuration.instance.ConfigurationData.DarthMaulControllerCount == ControllerCountEnum.Two && Configuration.instance.ConfigurationData.ReverseMaulDirection)
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
