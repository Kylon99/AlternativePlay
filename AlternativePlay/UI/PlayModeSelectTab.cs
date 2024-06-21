using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.GameplaySetup;
using HMUI;
using System.Linq;
using UnityEngine;
using Zenject;

namespace AlternativePlay.UI
{
    /// <summary>
    /// Holds the UI properties for the Gameplay Setup MOD menu for AlternativePlay
    /// </summary>
    public class PlayModeSelectTab : MonoBehaviour
    {
#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
#pragma warning restore CS0649

        private void Start()
        {
            GameplaySetup.instance.AddTab("Alternative Play", "AlternativePlay.UI.PlayModeSelectTab.bsml", this, MenuType.All);
        }

        public void UpdatePlayModeSelectList()
        {
            var list = this.configuration.ConfigurationData.PlayModeSettings
                    .Select((settings, i) => new PlayModeSelectOption(this.configuration.ConfigurationData, i))
                    .ToList();

            this.SelectModeList.tableView.ClearSelection();
            this.SelectModeList.data.Clear();
            this.SelectModeList.data = list.Cast<object>().ToList();
            this.SelectModeList.tableView.ReloadData();
            this.SelectModeList.tableView.SelectCellWithIdx(this.configuration.ConfigurationData.Selected);
            this.SelectModeList.tableView.ScrollToCellWithIdx(this.configuration.SelectedIndex, TableView.ScrollPositionType.Center, false);
        }

        [UIAction(nameof(OnModeClicked))]
        public void OnModeClicked(TableView _, PlayModeSelectOption selected)
        {
            this.configuration.SelectPlayModeSetting(selected.Index);
            this.UpdatePlayModeSelectList();
        }

        [UIComponent(nameof(SelectModeList))]
        public readonly CustomCellListTableData SelectModeList;
    }
}