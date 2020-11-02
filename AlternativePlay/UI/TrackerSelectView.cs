using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlternativePlay.UI
{
    [HotReload]
    public class TrackerSelectView : BSMLAutomaticViewController
    {
        // Internal tracker selection members
        private List<TrackerDisplayText> LoadedTrackers;
        private TrackerConfigData trackerConfigData;
        private TrackerConfigData originalTrackerData;
        private ModMainFlowCoordinator mainFlowCoordinator;

        public void SetMainFlowCoordinator(ModMainFlowCoordinator mainFlowCoordinator)
        {
            this.mainFlowCoordinator = mainFlowCoordinator;
        }

        public void SetSelectingTracker(TrackerConfigData trackerConfigData)
        {
            this.trackerConfigData = trackerConfigData;
            this.originalTrackerData = TrackerConfigData.Clone(trackerConfigData);
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);

            if (this.trackerConfigData == null)
            {
                // Calls to this method should never pass in null
                AlternativePlay.Logger.Error($"TrackerSelectView.DidActivate() Error null tracker was given at {Environment.StackTrace}");
                trackerConfigData = new TrackerConfigData();
            }

            this.InitializeTrackerList();
            BehaviorCatalog.instance.ShowTrackersBehavior.EnableShowTrackers();
        }

        protected override void DidDeactivate(DeactivationType deactivationType)
        {
            BehaviorCatalog.instance.ShowTrackersBehavior.DisableShowTrackers();
        }

        // Components
        [UIComponent("SelectTrackerList")]
        public CustomListTableData trackerList;

        private string currentTrackerText;
        [UIValue("CurrentTrackerText")]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(nameof(this.CurrentTrackerText)); } }

        // Events
        [UIAction("OnTrackerListCellSelected")]
        private void OnTrackerListCellSelected(TableView _, int row)
        {
            var tracker = this.LoadedTrackers[row];
            this.trackerConfigData.Serial = tracker.Serial;
            this.trackerConfigData.FullName = tracker.HoverHint;

            BehaviorCatalog.instance.ShowTrackersBehavior.SetSelectedSerial(this.trackerConfigData);
        }

        [UIAction("OnSelected")]
        private void OnSelected()
        {
            Configuration.instance.SaveConfiguration();
            this.mainFlowCoordinator.DismissTrackerSelect();
        }

        [UIAction("OnCancelled")]
        private void OnCancelled()
        {
            TrackerConfigData.Copy(this.originalTrackerData, this.trackerConfigData);
            Configuration.instance.SaveConfiguration();
            this.mainFlowCoordinator.DismissTrackerSelect();
        }

        /// <summary>
        /// Initializes the state and the bound variables for the Tracker Select list
        /// </summary>
        /// <param name="selectingLeft">Whether to initialize for the Left or the Right tracker</param>
        private void InitializeTrackerList()
        {
            this.trackerList.tableView.ClearSelection();
            this.trackerList.data.Clear();

            // Set the currently used tracker text
            this.CurrentTrackerText = String.IsNullOrWhiteSpace(this.trackerConfigData.FullName) ? TrackerConfigData.NoTrackerHoverHint : this.trackerConfigData.FullName;

            // Load the currently found trackers
            TrackedDeviceManager.instance.LoadTrackedDevices();
            TrackedDeviceManager.instance.TrackedDevices.ForEach(t =>
            {
                var customCellInfo = new CustomListTableData.CustomCellInfo(TrackerConfigData.FormatTrackerHoverHint(t));
                this.trackerList.data.Add(customCellInfo);
            });

            // Save the list of serials for later reference
            this.LoadedTrackers = TrackedDeviceManager.instance.TrackedDevices
                .Select(t => new TrackerDisplayText
                {
                    Serial = t.serialNumber,
                    HoverHint = TrackerConfigData.FormatTrackerHoverHint(t),
                }).ToList();

            // Reload all the data for display
            this.trackerList.tableView.ReloadData();

            // Find the cell to select
            int index = 0;
            if (!String.IsNullOrWhiteSpace(this.trackerConfigData.Serial))
            {
                index = this.LoadedTrackers.FindIndex(t => t.Serial == this.trackerConfigData.Serial);
            }

            if (index != -1 && this.trackerList.data.Count > 0)
            {
                this.trackerList.tableView.SelectCellWithIdx(index);
            }

            // Set the Tracker Renderer to show trackers
            BehaviorCatalog.instance.ShowTrackersBehavior.SetSelectedSerial(this.trackerConfigData);
        }
    }
}
