using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace AlternativePlay.UI
{
    public class GameModifiersViewController : BSMLResourceViewController
    {
        public override string ResourceName => Plugin.assemblyName + ".UI.Views.GameModifiersView.bsml";

        [UIValue("NoArrowsRandom")]
        private bool noArrowsRandom = ConfigOptions.instance.NoArrowsRandom;
        [UIAction("OnNoArrowsRandomChanged")]
        private void OnNoArrowsRandomChanged(bool value)
        {
            ConfigOptions.instance.NoArrowsRandom = value;
        }

        [UIValue("OneColor")]
        private bool oneColor = ConfigOptions.instance.OneColor;
        [UIAction("OnOneColorChanged")]
        private void OnOneColorChanged(bool value)
        {
            ConfigOptions.instance.OneColor = value;
        }

        [UIValue("RemoveOtherSaber")]
        private bool removeOtherSaber = ConfigOptions.instance.RemoveOtherSaber;
        [UIAction("OnRemoveOtherSaberChanged")]
        private void OnRemoveOtherSaberChanged(bool value)
        {
            ConfigOptions.instance.RemoveOtherSaber = value;
        }

        [UIValue("NoArrows")]
        private bool noArrows = ConfigOptions.instance.NoArrows;
        [UIAction("OnNoArrowsChanged")]
        private void OnNoArrowsChanged(bool value)
        {
            ConfigOptions.instance.NoArrows = value;
        }

        [UIValue("StabNotes")]
        private bool stabNotes = ConfigOptions.instance.StabNotes;
        [UIAction("OnStabNotesChanged")]
        private void OnStabNotesChanged(bool value)
        {
            ConfigOptions.instance.StabNotes = value;
        }
    }
}
