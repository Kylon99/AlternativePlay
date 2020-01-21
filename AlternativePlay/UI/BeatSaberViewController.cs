using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AlternativePlay.UI
{
    public class BeatSaberViewController : BSMLResourceViewController
    {
        public override string ResourceName => Plugin.assemblyName + ".UI.Views.BeatSaberView.bsml";

        [UIValue("UseLeftSaber")]
        private bool useLeftSaber = ConfigOptions.instance.UseLeftSaber;
        [UIAction("OnUseLeftSaberChanged")]
        private void OnUseLeftSaberChanged(bool value)
        {
            ConfigOptions.instance.UseLeftSaber = value;
        }

        [UIValue("ReverseSaberDirection")]
        private bool reverseSaberDirection = ConfigOptions.instance.ReverseSaberDirection;
        [UIAction("OnReverseSaberDirectionChanged")]
        private void OnReverseSaberDirectionChanged(bool value)
        {
            ConfigOptions.instance.ReverseSaberDirection = value;
        }
    }
}
