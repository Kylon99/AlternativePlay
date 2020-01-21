using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;

namespace AlternativePlay.UI
{
    public class BeatSpearViewController : BSMLResourceViewController
    {
        public override string ResourceName => Plugin.assemblyName + ".UI.Views.BeatSpearView.bsml";

        [UIValue("ControllerChoice")]
        private string controllerChoice = ConfigOptions.instance.SpearControllerCount.ToString();
        [UIValue("ControllerChoiceList")]
        private List<object> controllerChoiceList = new List<object> { "One", "Two" };
        [UIAction("OnControllersChanged")]
        private void OnControllersChanged(string value)
        {
            ConfigOptions.instance.SpearControllerCount = (ControllerCountEnum)Enum.Parse(typeof(ControllerCountEnum), value);
        }

        [UIValue("UseLeftController")]
        private bool useLeftController = ConfigOptions.instance.UseLeftSpear;
        [UIAction("OnUseLeftControllerChanged")]
        private void OnUseLeftControllerChanged(bool value)
        {
            ConfigOptions.instance.UseLeftSpear = value;
        }

        [UIValue("ReverseSpearDirection")]
        private bool reverseSaberDirection = ConfigOptions.instance.ReverseSpearDirection;
        [UIAction("OnReverseSpearDirectionChanged")]
        private void OnReverseSpearDirectionChanged(bool value)
        {
            ConfigOptions.instance.ReverseSpearDirection = value;
        }
    }
}
