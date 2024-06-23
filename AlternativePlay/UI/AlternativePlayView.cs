using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System.Linq;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class AlternativePlayView : BSMLAutomaticViewController
    {
#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
        [Inject]
        private AlternativePlayMainFlowCoordinator mainFlowCoordinator;
#pragma warning restore CS0649

        private int deleteIndex; // Caches the index to be deleted for after the Delete Modal is done


        /// <summary>
        /// Reloads the table with the latest configuration data
        /// </summary>
        /// <param name="index">Optional parameter for the row to scroll the table to.</param>
        public void RefreshConfigurations(int index = -1)
        {
            var list = this.configuration.ConfigurationData.PlayModeSettings
                .Select((settings, i) => new PlayModeSelectOption(this.configuration.ConfigurationData, i, this.ShowDeleteModal))
                .ToList();

            this.SelectModeList.tableView.ClearSelection();
            this.SelectModeList.data.Clear();
            this.SelectModeList.data = list.Cast<object>().ToList();
            this.SelectModeList.tableView.ReloadData();

            if (index != -1)
            {
                this.SelectModeList.tableView.ScrollToCellWithIdx(index, TableView.ScrollPositionType.End, false);
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (firstActivation)
            {
                this.RefreshConfigurations();
            }
        }

        /// <summary>
        /// Shows the Delete confirmation modal.  Deletion happens after the user selects OK.
        /// </summary>
        private void ShowDeleteModal(int index)
        {
            this.deleteIndex = index;
            this.DeleteModal.Show(true);
        }

        [UIComponent(nameof(SelectModeList))]
        public readonly CustomCellListTableData SelectModeList;

        [UIAction(nameof(this.OnModeClicked))]
        public void OnModeClicked(TableView _, PlayModeSelectOption selected)
        {
            var playModeSettings = this.configuration.GetPlayModeSetting(selected.Index);
            if (playModeSettings == null)
            {
                // Do nothing as this is an error
                return;
            }

            this.mainFlowCoordinator.ShowPlayModeSelect(playModeSettings, selected.Index);
        }

        [UIAction(nameof(this.OnAddNewConfiguration))]
        public void OnAddNewConfiguration()
        {
            // Add a new setting to the bottom of the list
            this.configuration.AddPlayModeSetting();

            int index = this.configuration.ConfigurationData.PlayModeSettings.Count - 1;
            this.RefreshConfigurations(index);
        }

        [UIComponent("DeleteModal")]
        public ModalView DeleteModal;

        [UIAction("OnOKClicked")]
        public void OnOKClicked()
        {
            // Delete the playmode setting at the saved index
            this.configuration.DeletePlayModeSetting(this.deleteIndex);
            int scrollToIndex = this.deleteIndex >= this.configuration.ConfigurationData.PlayModeSettings.Count
                ? this.configuration.ConfigurationData.PlayModeSettings.Count - 1
                : this.deleteIndex;
            this.RefreshConfigurations(scrollToIndex);

            this.DeleteModal.Hide(true);
        }

        [UIAction("OnCancelClicked")]
        public void OnCancelClicked()
        {
            this.DeleteModal.Hide(true);
        }
    }

}