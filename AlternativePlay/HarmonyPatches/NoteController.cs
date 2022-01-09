using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlternativePlay.Models;
using BS_Utils.Utilities;
using HarmonyLib;
using UnityEngine;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteController), "Init")]
    static class NoteControllerInit
    {
        private static bool useLeft;
        private static ColorType undesiredNoteType;
        static void Prefix(ref NoteData noteData, ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos, Transform ____noteTransform)
        {
            

            var config = Configuration.instance.ConfigurationData;

            if (config.OneColor && !config.NoArrowsRandom)
            {
                useLeft =
                    (config.PlayMode == PlayMode.BeatSaber && config.UseLeftSaber) ||
                    (config.PlayMode == PlayMode.DarthMaul && config.UseLeftController) ||
                    (config.PlayMode == PlayMode.BeatSpear && config.UseLeftSpear);

                undesiredNoteType = useLeft ? ColorType.ColorB : ColorType.ColorA;
            }

            if ((config.NoArrows || config.TouchNotes) && !config.NoArrowsRandom)
            {
                AlternativePlay.Logger.Info($"Modifiers: NoArrows");
                noteData.SetNoteToAnyCutDirection();
            }

            

            if (config.OneColor && noteData.colorType == undesiredNoteType)
            {
                ColorType type = noteData.colorType.Opposite();
                noteData.SetPrivateField("<colorType>k__BackingField", type);
            }
        }

        static void Postfix(Transform ____noteTransform)
        {
            //____noteTransform.localScale = Vector3.one * 1;
        }
    }
}