﻿using AlternativePlay.Models;
using HarmonyLib;
using UnityEngine.XR;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(nameof(HapticFeedbackController.PlayHapticFeedback))]
    [HarmonyPatch(typeof(HapticFeedbackController))]
    [HarmonyPriority(Priority.VeryHigh)]
    internal class DarthMaulHapticPatch
    {
        private static void Prefix(HapticFeedbackController __instance, ref XRNode node)
        {
            if (Configuration.Current.PlayMode != PlayMode.DarthMaul ||
                BehaviorCatalog.instance.DarthMaulBehavior == null ||
                BehaviorCatalog.instance.DarthMaulBehavior.Split)
            {
                // Let the original function handle the haptic feedback
                return;
            }

            if (Configuration.Current.ControllerCount == ControllerCountEnum.One)
            {
                if (!Configuration.Current.UseLeft && node == XRNode.LeftHand)
                {
                    // Using right controller, move left hits to right hand
                    node = XRNode.RightHand;
                }

                if (Configuration.Current.UseLeft && node == XRNode.RightHand)
                {
                    // Using left controller, move right hits to left hand
                    node = XRNode.LeftHand;
                }

                return;
            }

            if (Configuration.Current.ControllerCount == ControllerCountEnum.Two && Configuration.Current.ReverseMaulDirection)
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
