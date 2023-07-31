using AlternativePlay.Models;
using System.Collections;
using System.Collections.Generic;
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

            if (this.IsTransformNecessary() || Configuration.Current.TouchNotes)
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

            // Return no transform if no option is selected
            if (!(Configuration.Current.NoArrows || Configuration.Current.OneColor || Configuration.Current.RemoveOtherSaber || Configuration.Current.NoArrowsRandom || Configuration.Current.NoSliders)) { return false; } 

            bool IsOnlyOneColorSelected() { return Configuration.Current.OneColor && !Configuration.Current.NoArrows && !Configuration.Current.NoArrowsRandom && !Configuration.Current.TouchNotes; }
            bool AreOnlyNoArrowsOptionsSelected() { return (Configuration.Current.NoArrows || Configuration.Current.NoArrowsRandom) && !Configuration.Current.OneColor; }

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
            if (Configuration.Current.OneColor && !Configuration.Current.NoArrowsRandom)
            {
                bool useLeft = Configuration.Current.UseLeft && (Configuration.Current.PlayMode == PlayMode.BeatSaber || Configuration.Current.PlayMode == PlayMode.DarthMaul || Configuration.Current.PlayMode == PlayMode.BeatSpear);
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
            foreach (NoteData note in CallGetBeatmapDataItems<NoteData>(beatmapData))
            {
                // Transform for NoArrows or TouchNotes here but do not if NoArrowsRandom was already applied
                if ((Configuration.Current.NoArrows || Configuration.Current.TouchNotes) && !Configuration.Current.NoArrowsRandom)
                {
                    note.SetNoteToAnyCutDirection();
                }

                // Transform for One Color if this is the other note type
                if (Configuration.Current.OneColor && note.colorType == undesiredNoteType)
                {
                    this.SetNoteColor(note, undesiredNoteType.Opposite());
                }

                // Transform for NoSliders by converting to a regular note
                if (Configuration.Current.NoSliders && note.gameplayType == NoteData.GameplayType.BurstSliderHead)
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
            if (Configuration.Current.NoSliders)
            {
                // Remove all Burst Sliders from list
                var burstSliders = CallGetBeatmapDataItems<SliderData>(beatmapData).Where(s => s.sliderType == SliderData.Type.Burst).ToList();
                burstSliders.ForEach(s => beatmapData.allBeatmapDataItems.Remove(s));
            }

            foreach (SliderData slider in CallGetBeatmapDataItems<SliderData>(beatmapData))
            {
                // Transform for One Color if this is the other note type
                if (Configuration.Current.OneColor && slider.colorType == undesiredNoteType)
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

        /// <summary>
        /// Attempts to call using reflection Main.BeatmapData.GetBeatmapDataItems.  In Beat Saber 1.27 this method
        /// was changed to accept one parameter which we pass in 0 anyways.  So in order to support everything from
        /// 1.21 to 1.29.1 we attempt to unify the two calls here.
        /// </summary>
        private IEnumerable<T> CallGetBeatmapDataItems<T>(BeatmapData beatmapData)
        {
            var method = typeof(BeatmapData).GetMethod(nameof(BeatmapData.GetBeatmapDataItems));
            var genericMethod = method.MakeGenericMethod(typeof(T));

            var parameterInfo = method.GetParameters();
            if (parameterInfo.Length == 0)
            {
                // Supports Beat Saber 1.21 - 1.26
                return (IEnumerable<T>)genericMethod.Invoke(beatmapData, null);
            } 
            else
            {
                // Supports Beat Saber 1.27 - 1.29.1
                return (IEnumerable<T>)genericMethod.Invoke(beatmapData, new object[] {0});
            }
        }
    }
}
