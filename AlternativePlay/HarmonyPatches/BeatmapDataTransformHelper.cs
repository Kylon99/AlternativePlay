using AlternativePlay.Models;
using HarmonyLib;
using System.Linq;
using System.Reflection;

namespace AlternativePlay.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataTransformHelper), "CreateTransformedBeatmapData")]
    internal static class BeatmapDataTransformHelperCreateTransformedBeatmapData
    {
        private static PropertyInfo s_colorTypeProperty;
        public static PropertyInfo ColorTypeProperty => s_colorTypeProperty ?? (s_colorTypeProperty = typeof(NoteData).GetProperty("colorType"));

        private static PropertyInfo slider_colorTypeProperty;
        public static PropertyInfo SliderColorTypeProperty => slider_colorTypeProperty ?? (slider_colorTypeProperty = typeof(SliderData).GetProperty("colorType"));

        private static void SetNoteColorType(NoteData noteData, ColorType colorType)
        {
            BeatmapDataTransformHelperCreateTransformedBeatmapData.ColorTypeProperty.GetSetMethod(true).Invoke(noteData, new object[]
            {
                colorType
            });
        }


        private static void SetSliderColorType(SliderData sliderData, ColorType colorType)
        {
            BeatmapDataTransformHelperCreateTransformedBeatmapData.SliderColorTypeProperty.GetSetMethod(true).Invoke(sliderData, new object[]
            {
                colorType
            });
        }

        private static void SwitchNoteColorType(NoteData noteData)
        {
            ColorType colorType = (ColorType)BeatmapDataTransformHelperCreateTransformedBeatmapData.ColorTypeProperty.GetValue(noteData);
            /*if (colorType == ColorType.ColorA)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetNoteColorType(noteData, ColorType.ColorB);
                return;
            }*/
            if (colorType == ColorType.ColorB)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetNoteColorType(noteData, ColorType.ColorA);
            }
        }


        private static void SwitchSliderColorType(SliderData sliderData)
        {
            ColorType colorType = (ColorType)BeatmapDataTransformHelperCreateTransformedBeatmapData.SliderColorTypeProperty.GetValue(sliderData);
            /*if (colorType == ColorType.ColorA)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetSliderColorType(sliderData, ColorType.ColorB);
                return;
            }*/
            if (colorType == ColorType.ColorB)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetSliderColorType(sliderData, ColorType.ColorA);
            }
        }

        private static void Prefix(ref IReadonlyBeatmapData beatmapData, IPreviewBeatmapLevel beatmapLevel, GameplayModifiers gameplayModifiers, bool leftHanded, EnvironmentEffectsFilterPreset environmentEffectsFilterPreset, EnvironmentIntensityReductionOptions environmentIntensityReductionOptions, MainSettingsModelSO mainSettingsModel)
        {
            var config = Configuration.instance.ConfigurationData;

            if (config.NoArrows || config.OneColor)
            {
                BeatmapData copy = beatmapData.GetCopy();
                var beatmapObjectDataItems = copy.allBeatmapDataItems.Where(x => x is BeatmapObjectData).Select(x => x as BeatmapObjectData).ToArray();

                foreach (BeatmapObjectData beatmapObjectData in beatmapObjectDataItems)
                {
                    var noteData = beatmapObjectData as NoteData;
                    var sliderData = beatmapObjectData as SliderData;
                    if (noteData != null && noteData.cutDirection != NoteCutDirection.None)
                    {
                        if (config.NoArrows || config.TouchNotes)
                        {
                            noteData.SetNoteToAnyCutDirection();
                        }

                        if (config.OneColor)
                        {
                            
                            BeatmapDataTransformHelperCreateTransformedBeatmapData.SwitchNoteColorType(noteData);
                        }
                    }

                    if (sliderData != null && sliderData.headCutDirection != NoteCutDirection.None)
                    {
                        if (config.OneColor)
                        {
                            BeatmapDataTransformHelperCreateTransformedBeatmapData.SwitchSliderColorType(sliderData);
                        }
                    }
                }
                beatmapData = copy;
            }

        }


    }
}