using AlternativePlay.Models;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace AlternativePlay
{
    public class GameModifiersBehavior : MonoBehaviour
    {
#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
#pragma warning restore CS0649

        private IDifficultyBeatmap currentBeatmap;
        private BeatmapCallbacksController beatmapCallbacksController;

        private PropertyInfo noteDataColorTypeProperty;
        private PropertyInfo sliderDataColorTypeProperty;
        private FieldInfo beatmapDataField;

        private void Start()
        {
            if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) { return; }

            // Get the reflection data for classes we require later
            this.noteDataColorTypeProperty = typeof(NoteData).GetProperty("colorType");
            this.sliderDataColorTypeProperty = typeof(SliderData).GetProperty("colorType");
            this.beatmapDataField = typeof(BeatmapCallbacksController).GetField("_beatmapData", BindingFlags.NonPublic | BindingFlags.Instance);

            // Get the map metadata
            GameplayCoreSceneSetupData data = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData;
            this.currentBeatmap = data.difficultyBeatmap;

            // Get the DIContainer from the singular GamesScenesManager to get its BeatmapCallbacksController
            var gameScenesManager = FindObjectOfType<GameScenesManager>();
            this.beatmapCallbacksController = gameScenesManager.currentScenesContainer.Resolve<BeatmapCallbacksController>();

            if (this.IsTransformNecessary() || this.configuration.Current.TouchNotes)
            {
                // Disable scoring due to transforms
                AlternativePlay.Logger.Info("Disabling score submission on Game Modifier mode transformation");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(AlternativePlay.assemblyName);

                this.StartCoroutine(this.TransformMap());
            }

        }

        /// <summary>
        /// Checks to see whether the transforms handled by this object are necessary, which are
        /// OneColor, NoArrowsRandom or NoArrows
        /// </summary>
        private bool IsTransformNecessary()
        {
            const string NoArrowsModeName = "NoArrows";
            const string OneSaberModeName = "OneSaber";

            // Return no transform if no option is selected
            if (!(this.configuration.Current.NoArrows || this.configuration.Current.OneColor || this.configuration.Current.RemoveOtherSaber || this.configuration.Current.NoArrowsRandom || this.configuration.Current.NoSliders)) { return false; } 

            bool IsOnlyOneColorSelected() { return this.configuration.Current.OneColor && !this.configuration.Current.NoArrows && !this.configuration.Current.NoArrowsRandom && !this.configuration.Current.TouchNotes; }
            bool AreOnlyNoArrowsOptionsSelected() { return (this.configuration.Current.NoArrows || this.configuration.Current.NoArrowsRandom) && !this.configuration.Current.OneColor; }

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

            var beatmapData = this.beatmapDataField.GetValue(this.beatmapCallbacksController) as BeatmapData;

            // Set up for One Color
            ColorType undesiredNoteType = ColorType.ColorA;
            if (this.configuration.Current.OneColor && !this.configuration.Current.NoArrowsRandom)
            {
                bool useLeft = this.configuration.Current.UseLeft && (this.configuration.Current.PlayMode == PlayMode.BeatSaber || this.configuration.Current.PlayMode == PlayMode.DarthMaul || this.configuration.Current.PlayMode == PlayMode.BeatSpear);
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
            foreach (NoteData note in beatmapData.GetBeatmapDataItems<NoteData>(0))
            {
                // Transform for NoArrows or TouchNotes here but do not if NoArrowsRandom was already applied
                if ((this.configuration.Current.NoArrows || this.configuration.Current.TouchNotes) && !this.configuration.Current.NoArrowsRandom)
                {
                    note.SetNoteToAnyCutDirection();
                }

                // Transform for One Color if this is the other note type
                if (this.configuration.Current.OneColor && note.colorType == undesiredNoteType)
                {
                    this.SetNoteColor(note, undesiredNoteType.Opposite());
                }

                // Transform for NoSliders by converting to a regular note
                if (this.configuration.Current.NoSliders && note.gameplayType == NoteData.GameplayType.BurstSliderHead)
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
            if (this.configuration.Current.NoSliders)
            {
                // Remove all Burst Sliders from list
                var burstSliders = beatmapData.GetBeatmapDataItems<SliderData>(0).Where(s => s.sliderType == SliderData.Type.Burst).ToList();
                burstSliders.ForEach(s => beatmapData.allBeatmapDataItems.Remove(s));
            }

            foreach (SliderData slider in beatmapData.GetBeatmapDataItems<SliderData>(0))
            {
                // Transform for One Color if this is the other note type
                if (this.configuration.Current.OneColor && slider.colorType == undesiredNoteType)
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
            this.noteDataColorTypeProperty.SetValue(noteData, color, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }

        /// <summary>
        /// Set the color type of the slider
        /// </summary>
        private void SetSliderColor(SliderData sliderData, ColorType color)
        {
            this.sliderDataColorTypeProperty.SetValue(sliderData, color, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }
    }
}
