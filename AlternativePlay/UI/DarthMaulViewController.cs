using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;

namespace AlternativePlay.UI
{
    public class DarthMaulViewController : BSMLResourceViewController
    {
        public override string ResourceName => Plugin.assemblyName + ".UI.Views.DarthMaulView.bsml";

        [UIValue("ControllerChoice")]
        private string controllerChoice = ConfigOptions.instance.DarthMaulControllerCount.ToString();
        [UIValue("ControllerChoiceList")]
        private List<object> controllerChoiceList = new List<object> { "One", "Two" };
        [UIAction("OnControllersChanged")]
        private void OnControllersChanged(string value)
        {
            ConfigOptions.instance.DarthMaulControllerCount = (ControllerCountEnum)Enum.Parse(typeof(ControllerCountEnum), value);
        }

        [UIValue("UseLeftController")]
        private bool useLeftController = ConfigOptions.instance.UseLeftController;
        [UIAction("OnUseLeftControllerChanged")]
        private void OnUseLeftControllerChanged(bool value)
        {
            ConfigOptions.instance.UseLeftController = value;
        }

        [UIValue("ReverseMaulDirection")]
        private bool reverseSaberDirection = ConfigOptions.instance.ReverseMaulDirection;
        [UIAction("OnReverseMaulDirectionChanged")]
        private void OnReverseMaulDirectionChanged(bool value)
        {
            ConfigOptions.instance.ReverseMaulDirection = value;
        }

        [UIValue("UseTriggerToSeparate")]
        private bool useTriggerToSeparate = ConfigOptions.instance.UseTriggerToSeparate;
        [UIAction("OnUseTriggerToSeparateChanged")]
        private void OnUseTriggerToSeparateChanged(bool value)
        {
            ConfigOptions.instance.UseTriggerToSeparate = value;
        }

        [UIValue("SeparationAmount")]
        private int separationAmount = ConfigOptions.instance.SeparationAmount;
        [UIAction("OnSeparationAmountChanged")]
        private void OnSeparationAmountChanged(int value)
        {
            ConfigOptions.instance.SeparationAmount = value;
        }
    }
}
