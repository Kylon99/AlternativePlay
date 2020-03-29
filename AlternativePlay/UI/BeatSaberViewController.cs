using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AlternativePlay.UI
{
    public class BeatSaberViewController : BSMLResourceViewController
    {
        public override string ResourceName => AlternativePlay.assemblyName + ".UI.Views.BeatSaberView.bsml";

        [UIValue("UseLeftSaber")]
        private bool useLeftSaber = ConfigOptions.instance.UseLeftSaber;
        [UIAction("OnUseLeftSaberChanged")]
        private void OnUseLeftSaberChanged(bool value)
        {
            ConfigOptions.instance.UseLeftSaber = value;
        }

        [UIValue("ReverseLeftSaber")]
        private bool reverseLeftSaber = ConfigOptions.instance.ReverseLeftSaber;
        [UIAction("OnReverseLeftSaberChanged")]
        private void OnReverseLeftSaberChanged(bool value)
        {
            ConfigOptions.instance.ReverseLeftSaber = value;
        }

        [UIValue("ReverseRightSaber")]
        private bool reverseRightSaber = ConfigOptions.instance.ReverseRightSaber;
        [UIAction("OnReverseRightSaberChanged")]
        private void OnReverseRightSaberChanged(bool value)
        {
            ConfigOptions.instance.ReverseRightSaber = value;
        }

    }
}
