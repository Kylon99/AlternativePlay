using AlternativePlay.Models;
using BS_Utils.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AlternativePlay
{
    public class GameModifiersBehavior : MonoBehaviour
    {
        private IDifficultyBeatmap currentBeatmap;
        private bool useLeft;
        private NoteType undesiredNoteType;

        private PlayerController playerController;


        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Get the map metadata
            GameplayCoreSceneSetupData data = BS_Utils.Plugin.LevelData?.GameplayCoreSceneSetupData;
            this.currentBeatmap = data.difficultyBeatmap;

            if (this.IsTransformNecessary() || Configuration.instance.ConfigurationData.TouchNotes)
            {
                // Disable scoring due to transforms
                AlternativePlay.Logger.Info("Disabling submission on Game Modifier mode transformation");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(AlternativePlay.assemblyName);

                SharedCoroutineStarter.instance.StartCoroutine(TransformMap());
            }
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
        }

        /// <summary>
        /// Checks to see whether the transforms handled by this object are necessary, which are
        /// OneColor, NoArrowsRandom or NoArrows
        /// </summary>
        private bool IsTransformNecessary()
        {
            const string NoArrowsModeName = "NoArrows";
            const string OneSaberModeName = "OneSaber";

            // No transform if nothing is selected
            if (!Configuration.instance.ConfigurationData.NoArrows &&
                !Configuration.instance.ConfigurationData.OneColor &&
                !Configuration.instance.ConfigurationData.RemoveOtherSaber &&
                !Configuration.instance.ConfigurationData.NoArrowsRandom)
            {
                return false;
            }

            bool IsOnlyOneColorSelected() { return Configuration.instance.ConfigurationData.OneColor && !Configuration.instance.ConfigurationData.NoArrows && !Configuration.instance.ConfigurationData.NoArrowsRandom && !Configuration.instance.ConfigurationData.TouchNotes; }
            bool AreOnlyNoArrowsOptionsSelected() { return (Configuration.instance.ConfigurationData.NoArrows || Configuration.instance.ConfigurationData.NoArrowsRandom) && !Configuration.instance.ConfigurationData.OneColor; }

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

            // Set up for One Color
            if (Configuration.instance.ConfigurationData.OneColor && !Configuration.instance.ConfigurationData.NoArrowsRandom)
            {
                this.useLeft =
                    (Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatSaber && Configuration.instance.ConfigurationData.UseLeftSaber) ||
                    (Configuration.instance.ConfigurationData.PlayMode == PlayMode.DarthMaul && Configuration.instance.ConfigurationData.UseLeftController) ||
                    (Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatSpear && Configuration.instance.ConfigurationData.UseLeftSpear);

                this.undesiredNoteType = this.useLeft ? NoteType.NoteB : NoteType.NoteA;

                // Change the other saber to desired type
                SaberType desiredSaberType = this.useLeft ? SaberType.SaberA : SaberType.SaberB;
                var saberObject = new GameObject("SaberTypeObject").AddComponent<SaberTypeObject>();
                saberObject.SetField("_saberType", desiredSaberType);

                var player = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
                Saber saberToSwap = this.useLeft ? player.rightSaber : player.leftSaber;
                saberToSwap.SetField("_saberType", saberObject);

                if (Configuration.instance.ConfigurationData.RemoveOtherSaber && Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatSaber)
                {
                    // Hide the off color saber
                    Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSaber ? this.playerController.rightSaber : this.playerController.leftSaber;
                    saberToHide.gameObject.SetActive(false);
                }
            }

            // Get the in memory beat map
            BeatmapObjectCallbackController callbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().First();
            BeatmapData beatmapData = callbackController.GetField<BeatmapData>("_beatmapData");

            if (Configuration.instance.ConfigurationData.NoArrowsRandom)
            {
                // Transform the map to No Arrows Random using the ingame algorithm first
                AlternativePlay.Logger.Info($"Transforming NoArrowsRandom for song: {this.currentBeatmap.level.songName}");
                var transformedBeatmap = BeatmapDataNoArrowsTransform.CreateTransformedData(beatmapData, true);
                callbackController.SetNewBeatmapData(transformedBeatmap);
            }

            // Transform every note
            var allNoteObjects = beatmapData.beatmapLinesData
                .SelectMany(line => line.beatmapObjectsData)
                .Where(objectData => objectData.beatmapObjectType == BeatmapObjectType.Note)
                .ToList();
            allNoteObjects.ForEach(beatmapObject =>
            {
                var note = beatmapObject as NoteData;

                // Transform for NoArrows or TouchNotes here but do not if NoArrowsRandom was already applied
                if ((Configuration.instance.ConfigurationData.NoArrows || Configuration.instance.ConfigurationData.TouchNotes) && !Configuration.instance.ConfigurationData.NoArrowsRandom)
                {
                    note.SetNoteToAnyCutDirection();
                }

                // Transform for One Color if this is the other note type
                if (Configuration.instance.ConfigurationData.OneColor && note.noteType == undesiredNoteType)
                {
                    note.SwitchNoteType();
                }
            });

            // Touch Notes speed detection is not handled here but in the HarmonyPatches
        }
    }
}
