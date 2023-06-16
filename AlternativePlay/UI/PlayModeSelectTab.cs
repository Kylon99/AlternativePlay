using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using System.Linq;
using UnityEngine;

namespace AlternativePlay.UI
{
    /// <summary>
    /// Holds the UI properties for the Gameplay Setup MOD menu for AlternativePlay
    /// </summary>
    public class PlayModeSelectTab : MonoBehaviour
    {
        public void UpdatePlayModeSelectList()
        {
            var list = Configuration.instance.ConfigurationData.PlayModeSettings
                    .Select((settings, i) => new PlayModeSelectOption(settings, i))
                    .ToList();

            this.SelectModeList.tableView.ClearSelection();
            this.SelectModeList.data.Clear();
            this.SelectModeList.data = list.Cast<object>().ToList();
            this.SelectModeList.tableView.ReloadData();
            this.SelectModeList.tableView.ScrollToCellWithIdx(Configuration.SelectedIndex, TableView.ScrollPositionType.Center, false);
        }

        private void Awake()
        {
            GameplaySetup.instance.AddTab("Alternative Play", "AlternativePlay.UI.PlayModeSelectTab.bsml", this, MenuType.All);
        }

        [UIAction(nameof(OnModeClicked))]
        public void OnModeClicked(TableView _, PlayModeSelectOption selected)
        {
            Configuration.SelectPlayModeSetting(selected.Index);
            this.UpdatePlayModeSelectList();
        }

        [UIComponent(nameof(SelectModeList))]
        public readonly CustomCellListTableData SelectModeList;
    }
}