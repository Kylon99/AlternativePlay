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
            GameplaySetup.Instance.AddTab("Alternative Play", "AlternativePlay.UI.PlayModeSelectTab.bsml", this, MenuType.All);
        }

        public void UpdatePlayModeSelectList()
        {
            var list = this.configuration.ConfigurationData.PlayModeSettings
                    .Select((settings, i) => new PlayModeSelectOption(this.configuration.ConfigurationData, i))
                    .ToList();

            this.SelectModeList.TableView.ClearSelection();
            this.SelectModeList.Data.Clear();
            this.SelectModeList.Data = list.Cast<object>().ToList();
            this.SelectModeList.TableView.ReloadData();
            this.SelectModeList.TableView.SelectCellWithIdx(this.configuration.ConfigurationData.Selected);
            this.SelectModeList.TableView.ScrollToCellWithIdx(this.configuration.SelectedIndex, TableView.ScrollPositionType.Center, false);
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