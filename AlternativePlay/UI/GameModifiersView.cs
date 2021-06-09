using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AlternativePlay.UI
{
    [HotReload]
    public class GameModifiersView : BSMLAutomaticViewController
    {
        [UIValue("NoArrowsRandom")]
        private bool noArrowsRandom = Configuration.instance.ConfigurationData.NoArrowsRandom;
        [UIAction("OnNoArrowsRandomChanged")]
        private void OnNoArrowsRandomChanged(bool value)
        {
            Configuration.instance.ConfigurationData.NoArrowsRandom = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("OneColor")]
        private bool oneColor = Configuration.instance.ConfigurationData.OneColor;
        [UIAction("OnOneColorChanged")]
        private void OnOneColorChanged(bool value)
        {
            Configuration.instance.ConfigurationData.OneColor = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("NoArrows")]
        private bool noArrows = Configuration.instance.ConfigurationData.NoArrows;
        [UIAction("OnNoArrowsChanged")]
        private void OnNoArrowsChanged(bool value)
        {
            Configuration.instance.ConfigurationData.NoArrows = value;
            Configuration.instance.SaveConfiguration();
        }

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
