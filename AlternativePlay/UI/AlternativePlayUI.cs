using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using UnityEngine;
using Zenject;

namespace AlternativePlay.UI
{
    public class AlternativePlayUI : MonoBehaviour
    {
        [Inject]
        private AlternativePlayMainFlowCoordinator mainFlowCoordinator;

        private void Awake()
        {
            MenuButton menuButton = new MenuButton(
                "Alternative Play",
                "Change to Darth Maul or Beat Spear here!", this.ShowModFlowCoordinator, true);
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
