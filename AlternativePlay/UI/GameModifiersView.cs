using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AlternativePlay.UI
{
    [HotReload]
    public class GameModifiersView : BSMLAutomaticViewController
    {
        [UIValue("NoArrowsRandomIcon")]
        public string NoArrowsRandomIcon => IconNames.NoArrowsRandom;

        [UIValue("NoArrowsRandom")]
        private bool noArrowsRandom = Configuration.instance.ConfigurationData.NoArrowsRandom;
        [UIAction("OnNoArrowsRandomChanged")]
        private void OnNoArrowsRandomChanged(bool value)
        {
            Configuration.instance.ConfigurationData.NoArrowsRandom = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("OneColorIcon")]
        public string OneColorIcon => IconNames.OneColor;

        [UIValue("OneColor")]
        private bool oneColor = Configuration.instance.ConfigurationData.OneColor;
        [UIAction("OnOneColorChanged")]
        private void OnOneColorChanged(bool value)
        {
            Configuration.instance.ConfigurationData.OneColor = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("NoSlidersIcon")]
        public string NoSlidersIcon => IconNames.NoSliders;

        [UIValue("NoSliders")]
        private bool noSliders = Configuration.instance.ConfigurationData.NoSliders;
        [UIAction("OnNoSlidersChanged")]
        private void OnNoSlidersChanged(bool value)
        {
            Configuration.instance.ConfigurationData.NoSliders = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("NoArrowsIcon")]
        public string NoArrowsIcon => IconNames.NoArrows;

        [UIValue("NoArrows")]
        private bool noArrows = Configuration.instance.ConfigurationData.NoArrows;
        [UIAction("OnNoArrowsChanged")]
        private void OnNoArrowsChanged(bool value)
        {
            Configuration.instance.ConfigurationData.NoArrows = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("TouchNotesIcon")]
        public string TouchNotesIcon => IconNames.TouchNotes;

        [UIValue("TouchNotes")]
        private bool touchNotes = Configuration.instance.ConfigurationData.TouchNotes;
        [UIAction("OnTouchNotesChanged")]
        private void OnTouchNotesChanged(bool value)
        {
            Configuration.instance.ConfigurationData.TouchNotes = value;
            Configuration.instance.SaveConfiguration();
        }
    }
}
