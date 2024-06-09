using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AlternativePlay.Models
{
    public class Configuration : PersistentSingleton<Configuration>
    {
        public static readonly string configurationFile = Path.Combine(Application.dataPath, @"..\UserData\AlternativePlayConfiguration.json");

        public ConfigurationData ConfigurationData { get; private set; }

        public static int SelectedIndex => instance.ConfigurationData.Selected;
        public static PlayModeSettings Current => instance.ConfigurationData.PlayModeSettings[instance.ConfigurationData.Selected];

        /// <summary>
        /// Sets the current <see cref="PlayModeSettings"/> to use to play to <paramref name="index"/>
        /// </summary>
        public static void SelectPlayModeSetting(int index)
        {
            Configuration.instance.ConfigurationData.Selected = 
                index < 0 || index >= Configuration.instance.ConfigurationData.PlayModeSettings.Count ? 0 : index;
            Configuration.instance.SaveConfiguration();
        }

        /// <summary>
        /// Returns the <see cref="PlayModeSettings"/> given by <paramref name="index"/>.
        /// If the index is out of range then null is returned.  <see cref="PlayModeSettings"/> 
        /// can be null if they have not been configured yet.
        /// </summary>
        public static PlayModeSettings GetPlayModeSetting(int index)
        {
            if (index < 0 || index >= Configuration.instance.ConfigurationData.PlayModeSettings.Count) { return null; }
            return Configuration.instance.ConfigurationData.PlayModeSettings[index];
        }

        /// <summary>
        /// Adds a <see cref="PlayModeSettings"/> to the list. If the settings are null then
        /// a new default initialized setting will be created.
        /// </summary>
        public static void AddPlayModeSetting(PlayModeSettings settings = null)
        {
            var playModeList = Configuration.instance.ConfigurationData.PlayModeSettings;
            playModeList.Add(settings ?? new PlayModeSettings());
            Configuration.instance.SaveConfiguration();
        }

        /// <summary>
        /// Deletes the <see cref="PlayModeSettings"/> at the given <paramref name="index"/>.
        /// </summary>
        public static void DeletePlayModeSetting(int index)
        {
            var playModeList = Configuration.instance.ConfigurationData.PlayModeSettings;
            if (index < 0 || index >= playModeList.Count) { return; }

            if (playModeList.Count == 1)
            {
                // Replace the last play mode setting with a new one.  There must always be one
                playModeList[0] = new PlayModeSettings();
            } else
            {
                playModeList.RemoveAt(index);
            }

            // If selected is now out of bounds reset to 0
            if (index >= playModeList.Count) { Configuration.SelectPlayModeSetting(0); }

            Configuration.instance.SaveConfiguration();
        }

        /// <summary>
        /// Loads the existing configuration or creates a new one
        /// </summary>
        public void LoadConfiguration()
        {
            if (!File.Exists(configurationFile))
            {
                // Create a new empty configuration if it doesn't exist
                var newConfiguration = this.NewConfiguration();
                string json = JsonConvert.SerializeObject(newConfiguration, Formatting.Indented);
                File.WriteAllText(configurationFile, json);

                this.ConfigurationData = newConfiguration;
                return;
            }

            string configurationText = File.ReadAllText(configurationFile);
            this.ConfigurationData = JsonConvert.DeserializeObject<ConfigurationData>(configurationText);

            if (this.ConfigurationData == null || this.ConfigurationData.PlayModeSettings == null || this.ConfigurationData.PlayModeSettings.Count == 0)
            {
                // If unserializable check for old version
                this.ConfigurationData = this.ConvertOldConfiguration(configurationText);

                // If there is still no configuration then use a new one
                if (this.ConfigurationData == null || this.ConfigurationData.PlayModeSettings == null || this.ConfigurationData.PlayModeSettings.Count == 0) 
                { 
                    this.ConfigurationData = this.NewConfiguration(); 
                }
            }

            // Sanitize and finish the loading
            this.SanitizeConfigurationData();
            this.SaveConfiguration();
        }

        /// <summary>
        /// Writes the current <see cref="Models.Configuration"/> to disk after removing null play settings
        /// </summary>
        public void SaveConfiguration()
        {
            string json = JsonConvert.SerializeObject(this.ConfigurationData, Formatting.Indented);
            File.WriteAllText(configurationFile, json);
        }

        /// <summary>
        /// Creates a new empty configuration with defaults
        /// </summary>
        private ConfigurationData NewConfiguration()
        {
            return new ConfigurationData
            {
                Selected = 0,
                PlayModeSettings = new List<PlayModeSettings>() { new PlayModeSettings() }
            };
        }

        /// <summary>
        /// Attempts to read the old 0.7.5 configuration data to convert it to the new one
        /// </summary>
        private ConfigurationData ConvertOldConfiguration(string configurationText)
        {
            // Attempt to read the old configuration
            ConfigurationData075 oldConfiguration = JsonConvert.DeserializeObject<ConfigurationData075>(configurationText);      
            if (oldConfiguration == null) { return null; }

            // Return a new configuration with the one converted play settings
            var result = this.NewConfiguration();
            result.PlayModeSettings[0] = oldConfiguration.ToConfigurationData();
            return result;
        }

        /// <summary>
        /// Prevents configuration values with nonstandard entries
        /// </summary>
        private void SanitizeConfigurationData()
        {
            // Create a new playmode settings if list is empty
            if (this.ConfigurationData.PlayModeSettings == null || this.ConfigurationData.PlayModeSettings.Count == 0)
            {
                this.ConfigurationData = this.NewConfiguration();
                return;
            }

            // Check to see if selected play mode is out of range
            if (this.ConfigurationData.Selected < 0 || this.ConfigurationData.Selected >= this.ConfigurationData.PlayModeSettings.Count) 
            { 
                this.ConfigurationData.Selected = 0;
            }

            // Clamp the position and rotation settings
            foreach (var playModeSettings in this.ConfigurationData.PlayModeSettings.Where(p => p != null))
            {
                if (!PlayModeSettings.PositionIncrementList.Contains(playModeSettings.PositionIncrement))
                {
                    playModeSettings.PositionIncrement = PlayModeSettings.DefaultPositionIncrement;
                }
                if (!PlayModeSettings.RotationIncrementList.Contains(playModeSettings.RotationIncrement))
                {
                    playModeSettings.RotationIncrement = PlayModeSettings.DefaultRotationIncrement;
                }

                int clamped = Math.Min(playModeSettings.MoveNotesBack, 150);
                clamped = Math.Max(clamped, 0);
                playModeSettings.MoveNotesBack = clamped;
            }
        }
    }
}
