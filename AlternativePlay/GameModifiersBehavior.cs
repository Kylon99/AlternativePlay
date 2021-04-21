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
        private ColorType undesiredNoteType;
        private SaberManager saberManager;

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
                AlternativePlay.Logger.Info("Disabling score submission on Game Modifier mode transformation");
                BS_Utils.Gameplay.ScoreSubmission.DisableSubmission(AlternativePlay.assemblyName);

                this.StartCoroutine(this.TransformMap());
            }

            this.StartCoroutine(this.DisableOtherSaberMesh());
        }

        private void Awake()
        {
            this.saberManager = FindObjectOfType<SaberManager>();
        }

        private void Update()
        {
            if (!(Configuration.instance.ConfigurationData.OneColor &&
                Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatSaber &&
                Configuration.instance.ConfigurationData.RemoveOtherSaber)) { return; }

            // Move the other saber away since there's a bug in the base game which makes it
            // able to cut bombs still
            Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSaber ? this.saberManager.rightSaber : this.saberManager.leftSaber;
            saberToHide.gameObject.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
            saberToHide.gameObject.transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
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

            // No transform if nothing is selected
            if (!config.NoArrows &&
                !config.OneColor &&
                !config.RemoveOtherSaber &&
                !config.NoArrowsRandom)
            {
                return false;
            }

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

            var config = Configuration.instance.ConfigurationData;

            // Set up for One Color
            if (config.OneColor && !config.NoArrowsRandom)
            {
                this.useLeft =
                    (config.PlayMode == PlayMode.BeatSaber && config.UseLeftSaber) ||
                    (config.PlayMode == PlayMode.DarthMaul && config.UseLeftController) ||
                    (config.PlayMode == PlayMode.BeatSpear && config.UseLeftSpear);

                this.undesiredNoteType = this.useLeft ? ColorType.ColorB : ColorType.ColorA;

                // Change the other saber to desired type
                SaberType desiredSaberType = this.useLeft ? SaberType.SaberA : SaberType.SaberB;
                var saberObject = new GameObject("SaberTypeObject").AddComponent<SaberTypeObject>();
                saberObject.SetField("_saberType", desiredSaberType);

                var player = Resources.FindObjectsOfTypeAll<SaberManager>().FirstOrDefault();
                Saber saberToSwap = this.useLeft ? player.rightSaber : player.leftSaber;
                saberToSwap.SetField("_saberType", saberObject);
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
                //BeatmapObjectCallbackController callbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
                // BeatmapData beatmapData = callbackController.GetField<BeatmapData>("_beatmapData");
                if (config.NoArrowsRandom)
                {
                    // Transform the map to No Arrows Random using the ingame algorithm first
                    AlternativePlay.Logger.Info($"Transforming NoArrowsRandom for song: {this.currentBeatmap.level.songName}");
                    var transformedBeatmap = BeatmapDataNoArrowsTransform.CreateTransformedData(beatmapData);
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
                    if ((config.NoArrows || config.TouchNotes) && !config.NoArrowsRandom)
                    {
                        note.SetNoteToAnyCutDirection();
                    }

                    // Transform for One Color if this is the other note type
                    if (config.OneColor && note.colorType == this.undesiredNoteType)
                    {
                        note.SwitchNoteColorType();
                    }
                });
                // Touch Notes speed detection is not handled here but in the HarmonyPatches

            }
            catch
            {
                AlternativePlay.Logger.Info($"Transforming Error: {this.currentBeatmap.level.songName}");
            }

            // Touch Notes speed detection is not handled here but in the HarmonyPatches
        }

        /// <summary>
        /// Disables the rendering of the other saber
        /// </summary>
        private IEnumerator DisableOtherSaberMesh()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (!(Configuration.instance.ConfigurationData.OneColor &&
                Configuration.instance.ConfigurationData.PlayMode == PlayMode.BeatSaber &&
                Configuration.instance.ConfigurationData.RemoveOtherSaber)) { yield break; }

            Saber saberToHide = Configuration.instance.ConfigurationData.UseLeftSpear ? this.saberManager.rightSaber : this.saberManager.leftSaber;
            var saberRenderers = saberToHide.gameObject.GetComponentsInChildren<Renderer>();
            foreach (var r in saberRenderers) { r.enabled = false; }
        }
    }
}
