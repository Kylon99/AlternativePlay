using AlternativePlay.Models;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using UnityEngine;
using Zenject;

namespace AlternativePlay.UI
{
    public class AlternativePlayUI : MonoBehaviour
    {
        private AlternativePlayMainFlowCoordinator mainFlowCoordinator;

        private void Start()
        {
            MenuButton menuButton = new MenuButton(
                "Alternative Play",
                "Darth Maul, Beat Spear, Flail, Nunchaku and use tracker as sabers here!", this.ShowModFlowCoordinator, true);
            MenuButtons.instance.RegisterButton(menuButton);
        }

        public void ShowModFlowCoordinator()
        {
            if (this.mainFlowCoordinator == null)
                this.mainFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<AlternativePlayMainFlowCoordinator>();

            if (this.mainFlowCoordinator.IsBusy) return;
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(this.mainFlowCoordinator);
        }
    }
}
