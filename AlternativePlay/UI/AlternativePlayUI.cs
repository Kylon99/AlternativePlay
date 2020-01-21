using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using BS_Utils.Utilities;
using UnityEngine;

namespace AlternativePlay.UI
{
    public class AlternativePlayUI : MonoBehaviour
    {
        private ModMainFlowCoordinator mainFlowCoordinator;

        private void Awake()
        {
            this.mainFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModMainFlowCoordinator>();
        }

        public void CreateUI()
        {
            MenuButton menuButton = new MenuButton(
                "Alternative Play",
                "Change to Darth Maul or Beat Spear here!", ShowModFlowCoordinator, true);
            MenuButtons.instance.RegisterButton(menuButton);
        }

        public void ShowModFlowCoordinator()
        {
            BeatSaberUI.MainFlowCoordinator.InvokeMethod("PresentFlowCoordinator", mainFlowCoordinator, null, false, false);
        }
    }
}
