using AlternativePlay.Models;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AlternativePlay
{
    public class GameModifiersBehavior : MonoBehaviour
    {
        private IDifficultyBeatmap currentBeatmap;
        private BeatmapCallbacksController beatmapCallbacksController;

        private PropertyInfo noteDataColorTypeProperty;
        private PropertyInfo sliderDataColorTypeProperty;
        private FieldInfo beatmapDataField;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) { return; }

            // Get the map metadata
            GameplayCoreSceneSetupData data = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData;
            this.currentBeatmap = data.difficultyBeatmap;

            // Get the DIContainer from the singular GamesScenesManager to get its BeatmapCallbacksController
            var gameScenesManager = FindObjectOfType<GameScenesManager>();
            this.beatmapCallbacksController = gameScenesManager.currentScenesContainer.Resolve<BeatmapCallbacksController>();

            if (this.IsTransformNecessary() || Configuration.instance.ConfigurationData.TouchNotes)
            {
                // Disable scoring due to transforms
                AlternativePlay.Logger.Info("Disabling score submission on Game Modifier mode transformation");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(AlternativePlay.assemblyName);

                this.StartCoroutine(this.TransformMap());
            }
        }

        private void Awake()
        {
            // Get the reflection data for classes we require later
            this.noteDataColorTypeProperty = typeof(NoteData).GetProperty("colorType");
            this.sliderDataColorTypeProperty = typeof(SliderData).GetProperty("colorType");
            this.beatmapDataField = typeof(BeatmapCallbacksController).GetField("_beatmapData", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Checks to see whether the transforms handled by this object are necessary, which are
        /// OneColor, NoArrowsRandom or NoArrows
        /// </summary>
        private bool IsTransformNecessary()
        {
            const string NoArrowsModeName = "NoArrows";
            const string OneSaberModeName = "OneSaber";

            var config = Configuration.instance.ConfigurationData;

            // Return no transform if no option is selected
            if (!(config.NoArrows || config.OneColor || config.RemoveOtherSaber || config.NoArrowsRandom || config.NoSliders)) { return false; } 

            bool IsOnlyOneColorSelected() { return config.OneColor && !config.NoArrows && !config.NoArrowsRandom && !config.TouchNotes; }
            bool AreOnlyNoArrowsOptionsSelected() { return (config.NoArrows || config.NoArrowsRandom) && !config.OneColor; }

            // Check for map modes that already reproduce our game mode
            string serializedName = this.currentBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;

            if (serializedName == OneSaberModeName && IsOnlyOneColorSelected())
            {
                AlternativePlay.Logger.Info($"No need to transform: {this.currentBeatmap.level.songName} for One Color as it is already a One Saber map");
                return false;
            }

            if (serializedName == NoArrowsModeName && AreOnlyNoArrowsOptionsSelected())
            {
                AlternativePlay.Logger.Info($"No need to transform: {this.currentBeatmap.level.songName} to No Arrows as it's already a No Arrows map");
                return false;
            }

            return true;
        }

        private IEnumerator TransformMap()
        {
            yield return new WaitForSecondsRealtime(0.01f);
            if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) { yield break; }

            var config = Configuration.instance.ConfigurationData;

            var beatmapData = this.beatmapDataField.GetValue(this.beatmapCallbacksController) as BeatmapData;

            // Set up for One Color
            bool useLeft = false;
            ColorType undesiredNoteType = ColorType.ColorA;
            if (config.OneColor && !config.NoArrowsRandom)
            {
                useLeft =
                    (config.PlayMode == PlayMode.BeatSaber && config.UseLeftSaber) ||
                    (config.PlayMode == PlayMode.DarthMaul && config.UseLeftController) ||
                    (config.PlayMode == PlayMode.BeatSpear && config.UseLeftSpear);

                undesiredNoteType = useLeft ? ColorType.ColorB : ColorType.ColorA;
            }

            this.TransformNotes(beatmapData, undesiredNoteType);
            this.TransformSliders(beatmapData, undesiredNoteType);

            // Touch Notes speed detection is not handled here but in the HarmonyPatches
        }

        /// <summary>
        /// Perform both the NoArrows and the OneColor transform for NoteData
        /// </summary>
        private void TransformNotes(BeatmapData beatmapData, ColorType undesiredNoteType)
        {
            var config = Configuration.instance.ConfigurationData;

            foreach (NoteData note in beatmapData.GetBeatmapDataItems<NoteData>(0))
            {
                // Transform for NoArrows or TouchNotes here but do not if NoArrowsRandom was already applied
                if ((config.NoArrows || config.TouchNotes) && !config.NoArrowsRandom)
                {
                    note.SetNoteToAnyCutDirection();
                }

                // Transform for One Color if this is the other note type
                if (config.OneColor && note.colorType == undesiredNoteType)
                {
                    this.SetNoteColor(note, undesiredNoteType.Opposite());
                }

                // Transform for NoSliders by converting to a regular note
                if (config.NoSliders && note.gameplayType == NoteData.GameplayType.BurstSliderHead)
                {
                    note.ChangeToGameNote();
                }
            };
        }

        /// <summary>
        /// Transform the SliderData based on NoSliders and OneColor
        /// </summary>
        private void TransformSliders(BeatmapData beatmapData, ColorType undesiredNoteType)
        {
            var config = Configuration.instance.ConfigurationData;

            if (config.NoSliders)
            {
                // Remove all Burst Sliders from list
                var burstSliders = beatmapData.GetBeatmapDataItems<SliderData>(0).Where(s => s.sliderType == SliderData.Type.Burst).ToList();
                burstSliders.ForEach(s => beatmapData.allBeatmapDataItems.Remove(s));
            }

            foreach (SliderData slider in beatmapData.GetBeatmapDataItems<SliderData>(0))
            {
                // Transform for One Color if this is the other note type
                if (config.OneColor && slider.colorType == undesiredNoteType)
                {
                    this.SetSliderColor(slider, undesiredNoteType.Opposite());
                }
            }
        }

        /// <summary>
        /// Set the color type of the note
        /// </summary>
        private void SetNoteColor(NoteData noteData, ColorType color)
        {
            noteDataColorTypeProperty.SetValue(noteData, color, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }

        /// <summary>
        /// Set the color type of the slider
        /// </summary>
        private void SetSliderColor(SliderData sliderData, ColorType color)
        {
            sliderDataColorTypeProperty.SetValue(sliderData, color, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }
    }
}
