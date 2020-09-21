using AlternativePlay.Models;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR;

namespace AlternativePlay.UI
{
    [HotReload]
    public class BeatSaberView : BSMLAutomaticViewController
    {
        [UIValue("UseLeftSaber")]
        private bool useLeftSaber = Configuration.instance.ConfigurationData.UseLeftSaber;
        [UIAction("OnUseLeftSaberChanged")]
        private void OnUseLeftSaberChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseLeftSaber = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("ReverseLeftSaber")]
        private bool reverseLeftSaber = Configuration.instance.ConfigurationData.ReverseLeftSaber;
        [UIAction("OnReverseLeftSaberChanged")]
        private void OnReverseLeftSaberChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseLeftSaber = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("ReverseRightSaber")]
        private bool reverseRightSaber = Configuration.instance.ConfigurationData.ReverseRightSaber;
        [UIAction("OnReverseRightSaberChanged")]
        private void OnReverseRightSaberChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseRightSaber = value;
            Configuration.instance.SaveConfiguration();
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            SetTrackerText();
        }

        #region SelectTracker Modal Members

        private const string NoTrackerText = "None";
        private const string NoTrackerHoverHint = "Not using any tracked devices";

        // Internal tracker selection members
        private TrackerDisplayText selectedTracker;
        private List<TrackerDisplayText> LoadedTrackers;
        private bool selectingLeft = false;

        // Components
        [UIComponent("SelectTrackerList")]
        public CustomListTableData trackerList;

        // Text Displays for the Main View
        private string leftSaberTrackerSerial;
        [UIValue("LeftSaberTrackerSerial")]
        public string LeftSaberTrackerSerial { get => this.leftSaberTrackerSerial; set { this.leftSaberTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftSaberTrackerSerial)); } }

        private string leftSaberTrackerHoverHint;
        [UIValue("LeftSaberTrackerHoverHint")]
        public string LeftSaberTrackerHoverHint { get => this.leftSaberTrackerHoverHint; set { this.leftSaberTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftSaberTrackerHoverHint)); } }

        private string rightSaberTrackerSerial;
        [UIValue("RightSaberTrackerSerial")]
        public string RightSaberTrackerSerial { get => this.rightSaberTrackerSerial; set { this.rightSaberTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightSaberTrackerSerial)); } }

        private string rightSaberTrackerHoverHint;
        [UIValue("RightSaberTrackerHoverHint")]
        public string RightSaberTrackerHoverHint { get => this.rightSaberTrackerHoverHint; set { this.rightSaberTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightSaberTrackerHoverHint)); } }

        // Text Display for the Current Tracker in the Tracker Select Modal
        private string currentTrackerText;
        [UIValue("CurrentTrackerText")]
        public string CurrentTrackerText { get => this.currentTrackerText; set { this.currentTrackerText = value; this.NotifyPropertyChanged(nameof(this.CurrentTrackerText)); } }

        // Events

        [UIAction("OnShowSelectLeftTracker")]
        private void OnShowSelectLeftTracker()
        {
            InitializeTrackerModal(selectingLeft: true);
        }

        [UIAction("OnShowSelectRightTracker")]
        private void OnShowSelectRightTracker()
        {
            InitializeTrackerModal(selectingLeft: false);
        }

        [UIAction("OnTrackerListCellSelected")]
        public void OnTrackerListCellSelected(TableView _, int row)
        {
            // Save the selected row for later when the select button may be pressed
            if (row == 0)
            {
                // The "None" entry was selected
                this.selectedTracker = null;
                return;
            }

            var tracker = this.LoadedTrackers[row - 1];
            this.selectedTracker = new TrackerDisplayText
            {
                Serial = tracker.Serial,
                HoverHint = tracker.HoverHint
            };
        }

        [UIAction("OnTrackerSelected")]
        public void OnTrackerSelected()
        {
            var configData = Configuration.instance.ConfigurationData;

            if (selectingLeft)
            {
                configData.LeftSaberTracker = this.selectedTracker == null ? null : this.selectedTracker.Serial;
                configData.LeftSaberTrackerFullName = this.selectedTracker == null ? null : this.selectedTracker.HoverHint;

                this.LeftSaberTrackerSerial = this.selectedTracker == null ? NoTrackerText : this.selectedTracker.Serial;
                this.LeftSaberTrackerHoverHint = this.selectedTracker == null ? NoTrackerHoverHint : this.selectedTracker.HoverHint;
            }
            else
            {
                configData.RightSaberTracker = this.selectedTracker == null ? null : this.selectedTracker.Serial;
                configData.RightSaberTrackerFullName = this.selectedTracker == null ? null : this.selectedTracker.HoverHint;

                this.RightSaberTrackerSerial = this.selectedTracker == null ? NoTrackerText : this.selectedTracker.Serial;
                this.RightSaberTrackerHoverHint = this.selectedTracker == null ? NoTrackerHoverHint : this.selectedTracker.HoverHint;
            }
            Configuration.instance.SaveConfiguration();
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftSaberTracker = null;
            Configuration.instance.ConfigurationData.LeftSaberTrackerFullName = null;
            Configuration.instance.SaveConfiguration();
            this.LeftSaberTrackerSerial = NoTrackerText;
            this.LeftSaberTrackerHoverHint = NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightSaberTracker = null;
            Configuration.instance.ConfigurationData.RightSaberTrackerFullName = null;
            Configuration.instance.SaveConfiguration();
            this.RightSaberTrackerSerial = NoTrackerText;
            this.RightSaberTrackerHoverHint = NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftSaberTracker))
            {
                this.LeftSaberTrackerSerial = NoTrackerText;
                this.LeftSaberTrackerHoverHint = NoTrackerHoverHint;
            }
            else
            {
                this.LeftSaberTrackerSerial = config.LeftSaberTracker;
                this.LeftSaberTrackerHoverHint = config.LeftSaberTrackerFullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightSaberTracker))
            {
                this.RightSaberTrackerSerial = NoTrackerText;
                this.RightSaberTrackerHoverHint = NoTrackerHoverHint;
            }
            else
            {
                this.RightSaberTrackerSerial = config.RightSaberTracker;
                this.RightSaberTrackerHoverHint = config.RightSaberTrackerFullName;
            }
        }


        /// <summary>
        /// Initializes the state and the bound variables for the Tracker Select modal dialog
        /// </summary>
        /// <param name="selectingLeft">Whether to initialize for the Left or the Right tracker</param>
        private void InitializeTrackerModal(bool selectingLeft)
        {
            this.selectingLeft = selectingLeft;
            var configData = Configuration.instance.ConfigurationData;

            this.trackerList.tableView.ClearSelection();
            this.trackerList.data.Clear();

            // Set the currently used tracker text
            if (this.selectingLeft)
            {
                this.CurrentTrackerText = String.IsNullOrWhiteSpace(configData.LeftSaberTrackerFullName) ? NoTrackerHoverHint : configData.LeftSaberTrackerFullName;
            }
            else
            {
                this.CurrentTrackerText = String.IsNullOrWhiteSpace(configData.RightSaberTrackerFullName) ? NoTrackerHoverHint : configData.RightSaberTrackerFullName;
            }

            // Add the "No Tracker" cell first
            var noneTrackerCell = new CustomListTableData.CustomCellInfo(NoTrackerText);
            this.trackerList.data.Add(noneTrackerCell);

            // Load the currently found trackers
            TrackedDeviceManager.instance.LoadTrackedDevices();
            TrackedDeviceManager.instance.TrackedDevices.ForEach(t =>
            {
                var customCellInfo = new CustomListTableData.CustomCellInfo(FormatTrackerHoverHint(t));
                this.trackerList.data.Add(customCellInfo);
            });

            // Save the list of serials for later reference
            this.LoadedTrackers = TrackedDeviceManager.instance.TrackedDevices
                .Select(t => new TrackerDisplayText
                {
                    Serial = t.serialNumber,
                    HoverHint = FormatTrackerHoverHint(t),
                }).ToList();

            // Reload all the data for display
            this.trackerList.tableView.ReloadData();

            // Find the cell to select
            int index = 0;
            this.selectedTracker = null;

            string serialToFind = this.selectingLeft ? configData.LeftSaberTracker : configData.RightSaberTracker;
            if (!String.IsNullOrWhiteSpace(serialToFind))
            {
                index = this.LoadedTrackers.FindIndex(t => t.Serial == serialToFind) + 1;
                this.selectedTracker = this.LoadedTrackers.Find(t => t.Serial == serialToFind);
            }
            this.trackerList.tableView.SelectCellWithIdx(index);
        }

        /// <summary>
        /// Standardizes the formatting of the tracker information
        /// </summary>
        /// <param name="tracker">The</param>
        /// <returns></returns>
        private string FormatTrackerHoverHint(InputDevice tracker)
        {
            return $"{tracker.serialNumber} - {tracker.manufacturer} {tracker.name}";
        }

        #endregion

    }
}
