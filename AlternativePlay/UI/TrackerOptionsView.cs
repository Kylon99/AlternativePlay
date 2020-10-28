using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Collections.Generic;
using System.Linq;

namespace AlternativePlay.UI
{
    [HotReload]
    public class TrackerOptionsView : BSMLAutomaticViewController
    {
        [UIValue("PositionIncrementChoice")]
        private string positionIncrement = Configuration.instance.ConfigurationData.PositionIncrement.ToString();
        [UIValue("PositionIncrementList")]
        private List<object> positionIncrementList = ConfigurationData.PositionIncrementList.Cast<object>().ToList();
        [UIAction("OnPositionIncrementChanged")]
        private void OnPositionIncrementChanged(string value)
        {
            float increment = float.Parse(value);
            Configuration.instance.ConfigurationData.PositionIncrement = increment;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("RotationIncrementChoice")]
        private string rotationIncrement = Configuration.instance.ConfigurationData.RotationIncrement.ToString();
        [UIValue("RotationIncrementList")]
        private List<object> rotationIncrementList = ConfigurationData.RotationIncrementList.Cast<object>().ToList();
        [UIAction("OnRotationIncrementChanged")]
        private void OnRotationIncrementChanged(string value)
        {
            float increment = float.Parse(value);
            Configuration.instance.ConfigurationData.RotationIncrement = increment;
            Configuration.instance.SaveConfiguration();
        }
    }
}
