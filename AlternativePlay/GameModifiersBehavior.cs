using AlternativePlay.Models;
using BS_Utils.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AlternativePlay
{
    public class GameModifiersBehavior : MonoBehaviour
    {
        private IDifficultyBeatmap currentBeatmap;
        private bool useLeft;
        private ColorType undesiredNoteType;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) { return; }

            // Get the map metadata
            GameplayCoreSceneSetupData data = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData;
            this.currentBeatmap = data.difficultyBeatmap;

            if (this.IsTransformNecessary() || Configuration.instance.ConfigurationData.TouchNotes)
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

            var config = Configuration.instance.ConfigurationData;
            if (!config.NoArrows && !config.OneColor && !config.RemoveOtherSaber && !config.NoArrowsRandom) { return false; } // No transform if nothing is selected

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
            yield return new WaitForSecondsRealtime(0.1f);
            if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) { yield break; }

            var config = Configuration.instance.ConfigurationData;

            // Set up for One Color
            if (config.OneColor && !config.NoArrowsRandom)
            {
                this.useLeft =
                    (config.PlayMode == PlayMode.BeatSaber && config.UseLeftSaber) ||
                    (config.PlayMode == PlayMode.DarthMaul && config.UseLeftController) ||
                    (config.PlayMode == PlayMode.BeatSpear && config.UseLeftSpear);

                this.undesiredNoteType = this.useLeft ? ColorType.ColorB : ColorType.ColorA;
            }

            try
            {
                BeatmapObjectCallbackController callbackController = null;
                BeatmapData beatmapData = null;
                BeatmapObjectCallbackController[] callbackControllers = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>();
                foreach (BeatmapObjectCallbackController cbc in callbackControllers)
                {
                    if (cbc.GetField<BeatmapData>("_beatmapData") != null)
                    {
                        callbackController = cbc;
                        beatmapData = callbackController.GetField<BeatmapData>("_beatmapData");
                    }
                }
                if (config.NoArrowsRandom)
                {
                    // Transform the map to No Arrows Random using the ingame algorithm first
                    AlternativePlay.Logger.Info($"Transforming NoArrowsRandom for song: {this.currentBeatmap.level.songName}");
                    var transformedBeatmap = BeatmapDataNoArrowsTransform.CreateTransformedData(beatmapData);
                    callbackController.SetNewBeatmapData(transformedBeatmap);
                }

                // Transform every note
                this.TransformNotes(beatmapData);

                // Touch Notes speed detection is not handled here but in the HarmonyPatches
            }
            catch (Exception e)
            {
                AlternativePlay.Logger.Error($"Transforming Error: {this.currentBeatmap.level.songName}");
                AlternativePlay.Logger.Error($"Error Message: {e.Message}");
                AlternativePlay.Logger.Error($"Stack Trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Perform both the NoArrows and the OneColor transform here based on the 
        /// configuration data.
        /// </summary>
        private void TransformNotes(BeatmapData beatmapData)
        {
            var config = Configuration.instance.ConfigurationData;

            var allNoteObjects = beatmapData.beatmapLinesData
                .SelectMany(line => line.beatmapObjectsData)
                .Where(objectData => objectData.beatmapObjectType == BeatmapObjectType.Note)
                .ToList();

            allNoteObjects.ForEach(beatmapObject =>
            {
                var note = beatmapObject as NoteData;

                // Transform for NoArrows or TouchNotes here but do not if NoArrowsRandom was already applied
                if ((config.NoArrows || config.TouchNotes) && !config.NoArrowsRandom)
                {
                    note.SetNoteToAnyCutDirection();
                }

                // Transform for One Color if this is the other note type
                if (config.OneColor && note.colorType == this.undesiredNoteType)
                {
                    this.FlipNoteType(note);
                }
            });
        }

        /// <summary>
        /// Set the color type of the note to the opposite
        /// </summary>
        private void FlipNoteType(NoteData noteData)
        {
            ColorType type = noteData.colorType.Opposite();
            noteData.SetPrivateField("<colorType>k__BackingField", type);
        }
    }
}
