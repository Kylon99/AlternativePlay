using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace AlternativePlay.Models
{
    public class Configuration : PersistentSingleton<Configuration>
    {
        public static readonly string configurationFile = Path.Combine(Application.dataPath, @"..\UserData\AlternativePlayConfiguration.json");

        public ConfigurationData ConfigurationData { get; private set; }

        /// <summary>
        /// Loads the existing configuration or creates a new one
        /// </summary>
        public void LoadConfiguration()
        {
            if (!File.Exists(configurationFile))
            {
                // Create a new empty configuration if it doesn't exist
                var newConfiguration = new ConfigurationData();
                string json = JsonConvert.SerializeObject(newConfiguration, Formatting.Indented);
                File.WriteAllText(configurationFile, json);

                this.ConfigurationData = newConfiguration;
                return;
            }

            string configuration = File.ReadAllText(configurationFile);
            this.ConfigurationData = JsonConvert.DeserializeObject<ConfigurationData>(configuration);

            // Sanitize Configuration Data
            if (!ConfigurationData.PositionIncrementList.Contains(this.ConfigurationData.PositionIncrement))
            {
                this.ConfigurationData.PositionIncrement = ConfigurationData.DefaultPositionIncrement;
            }
            if (!ConfigurationData.RotationIncrementList.Contains(this.ConfigurationData.RotationIncrement))
            {
                this.ConfigurationData.RotationIncrement = ConfigurationData.DefaultRotationIncrement;
            }

            int clamped = Math.Min(Configuration.instance.ConfigurationData.MoveNotesBack, 150);
            clamped = Math.Max(clamped, 0);
            Configuration.instance.ConfigurationData.MoveNotesBack = clamped;
        }

        /// <summary>
        /// Writes the current <see cref="ConfigurationData"/> to disk
        /// </summary>
        public void SaveConfiguration()
        {
            string json = JsonConvert.SerializeObject(this.ConfigurationData, Formatting.Indented);
            File.WriteAllText(configurationFile, json);
        }
    }
}
