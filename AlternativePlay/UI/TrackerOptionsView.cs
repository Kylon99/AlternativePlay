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
        private string positionIncrement = Configuration.instance.ConfigurationData.PositionIncrement;
        [UIValue("PositionIncrementList")]
        private List<object> positionIncrementList = ConfigurationData.PositionIncrementList.Cast<object>().ToList();
        [UIAction("OnPositionIncrementChanged")]
        private void OnPositionIncrementChanged(string value)
        {
            Configuration.instance.ConfigurationData.PositionIncrement = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("RotationIncrementChoice")]
        private string rotationIncrement = Configuration.instance.ConfigurationData.RotationIncrement;
        [UIValue("RotationIncrementList")]
        private List<object> rotationIncrementList = ConfigurationData.RotationIncrementList.Cast<object>().ToList();
        [UIAction("OnRotationIncrementChanged")]
        private void OnRotationIncrementChanged(string value)
        {
            Configuration.instance.ConfigurationData.RotationIncrement = value;
            Configuration.instance.SaveConfiguration();
        }
    }
}
