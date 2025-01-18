using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Linq;
using Valve.VR;
using Zenject;

namespace AlternativePlay.UI
{
    [HotReload]
    public class TrackerSelectView : BSMLAutomaticViewController
    {
#pragma warning disable CS0649
        [Inject]
        private Configuration configuration;
        [Inject]
        private TrackedDeviceManager trackedDeviceManager;
        [Inject]
        private ShowTrackersBehavior showTrackersBehavior;
        [Inject]
        private AlternativePlayMainFlowCoordinator mainFlowCoordinator;
#pragma warning restore CS0649

        // Internal tracker selection members
        private TrackerConfigData trackerConfigData;
        private TrackerConfigData originalTrackerData;

        public void SetSelectingTracker(TrackerConfigData trackerConfigData)
        {
            this.trackerConfigData = trackerConfigData;
            this.originalTrackerData = TrackerConfigData.Clone(trackerConfigData);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (this.trackerConfigData == null)
            {
                // Calls to this method should never pass in null
                AlternativePlay.Logger.Error($"TrackerSelectView.DidActivate() Error null tracker was given at {Environment.StackTrace}");
                this.trackerConfigData = new TrackerConfigData();
            }

            this.InitializeTrackerList();
            this.showTrackersBehavior.EnableShowTrackers();
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            this.showTrackersBehavior.DisableShowTrackers();
        }

        // Components
        [UIComponent(nameof(SelectTrackerList))]
        public CustomCellListTableData SelectTrackerList;

        private string currentIcon;
        [UIValue(nameof(CurrentIcon))]
        public string CurrentIcon { get => this.currentIcon; set { this.currentIcon = value; this.NotifyPropertyChanged(nameof(this.CurrentIcon)); } }

        private string currentSerial;
        [UIValue(nameof(CurrentSerial))]
        public string CurrentSerial { get => this.currentSerial; set { this.currentSerial = value; this.NotifyPropertyChanged(nameof(this.CurrentSerial)); } }

        private string currentFullName;
        [UIValue(nameof(CurrentFullName))]
        public string CurrentFullName { get => this.currentFullName; set { this.currentFullName = value; this.NotifyPropertyChanged(nameof(this.CurrentFullName)); } }

        // Events
        [UIAction(nameof(OnTrackerListCellSelected))]
        private void OnTrackerListCellSelected(TableView _, TrackerSelectItem tracker)
        {
            this.trackerConfigData.Icon = tracker.Icon;
            this.trackerConfigData.Serial = tracker.Serial;
            this.trackerConfigData.FullName = tracker.FullName;

            this.CurrentIcon = tracker.Icon;
            this.CurrentSerial = tracker.Serial;
            this.CurrentFullName = tracker.FullName;

            this.showTrackersBehavior.SetSelectedSerial(this.trackerConfigData);
        }

        [UIAction(nameof(OnSelected))]
        private void OnSelected()
        {
            this.configuration.SaveConfiguration();
            this.mainFlowCoordinator.DismissTrackerSelect();
        }

        [UIAction(nameof(OnCancelled))]
        private void OnCancelled()
        {
            TrackerConfigData.Copy(this.originalTrackerData, this.trackerConfigData);
            this.configuration.SaveConfiguration();
            this.mainFlowCoordinator.DismissTrackerSelect();
        }

        /// <summary>
        /// Initializes the state and the bound variables for the Tracker Select list
        /// </summary>
        /// <param name="selectingLeft">Whether to initialize for the Left or the Right tracker</param>
        private void InitializeTrackerList()
        {
            // Set the currently used tracker text
            this.CurrentIcon = String.IsNullOrWhiteSpace(this.trackerConfigData.Icon) ? String.Empty : this.trackerConfigData.Icon;
            this.CurrentFullName = String.IsNullOrWhiteSpace(this.trackerConfigData.FullName) ? TrackerConfigData.NoTrackerHoverHint : this.trackerConfigData.FullName;

            // Load the currently found trackers
            this.trackedDeviceManager.LoadTrackedDeviceProperties();
            var list = this.trackedDeviceManager.TrackedDevices.Select(device => new TrackerSelectItem
            {
                Icon = this.MapDeviceTypeToIcon(device.DeviceClass),
                Serial = device.Serial,
                FullName = $"{device.Manufacturer} {device.Name}"
            }).ToList();

            this.SelectTrackerList.TableView.ClearSelection();
            this.SelectTrackerList.Data.Clear();
            this.SelectTrackerList.Data = list.Cast<object>().ToList();
            this.SelectTrackerList.TableView.ReloadData();

            // Find the cell to select
            int index = 0;
            if (!String.IsNullOrWhiteSpace(this.trackerConfigData.Serial))
            {
                index = this.trackedDeviceManager.TrackedDevices.FindIndex(t => t.Serial == this.trackerConfigData.Serial);
            }

            if (index != -1 && this.SelectTrackerList.Data.Count > 0)
            {
                this.SelectTrackerList.TableView.SelectCellWithIdx(index);
            }

            // Set the Tracker Renderer to show trackers
            this.showTrackersBehavior.SetSelectedSerial(this.trackerConfigData);
        }

        private string MapDeviceTypeToIcon(ETrackedDeviceClass deviceType)
        {
            switch (deviceType)
            {
                case ETrackedDeviceClass.HMD:
                    return IconNames.HMD;

                case ETrackedDeviceClass.Controller:
                    return IconNames.Controller;

                case ETrackedDeviceClass.GenericTracker:
                    return IconNames.Tracker;

                case ETrackedDeviceClass.TrackingReference:
                    return IconNames.TrackingReference;

                case ETrackedDeviceClass.Invalid:
                case ETrackedDeviceClass.DisplayRedirect:
                case ETrackedDeviceClass.Max:
                default:
                    return IconNames.Empty;
            }
        }
    }

    public class TrackerSelectItem
    {
        [UIValue(nameof(this.Icon))]
        public string Icon { get; set; }

        [UIValue(nameof(this.Serial))]
        public string Serial { get; set; }

        [UIValue(nameof(this.FullName))]
        public string FullName { get; set; }
    }
}
