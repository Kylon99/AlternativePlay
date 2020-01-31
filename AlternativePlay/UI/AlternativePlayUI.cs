using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using UnityEngine;

namespace AlternativePlay.UI
{
    public class AlternativePlayUI : MonoBehaviour
    {
        private ModMainFlowCoordinator mainFlowCoordinator;

        private void Awake()
        {
            MenuButton menuButton = new MenuButton(
                "Alternative Play",
                "Change to Darth Maul or Beat Spear here!", ShowModFlowCoordinator, true);
            MenuButtons.instance.RegisterButton(menuButton);
        }

        public void ShowModFlowCoordinator()
        {
            if (this.mainFlowCoordinator == null)
                this.mainFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModMainFlowCoordinator>();

            if (mainFlowCoordinator.IsBusy) return;

            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(mainFlowCoordinator);
        }
    }
}
