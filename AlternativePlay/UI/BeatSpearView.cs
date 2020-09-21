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
    public class BeatSpearView : BSMLAutomaticViewController
    {
        [UIValue("ControllerChoice")]
        private string controllerChoice = Configuration.instance.ConfigurationData.SpearControllerCount.ToString();
        [UIValue("ControllerChoiceList")]
        private List<object> controllerChoiceList = new List<object> { "One", "Two" };
        [UIAction("OnControllersChanged")]
        private void OnControllersChanged(string value)
        {
            Configuration.instance.ConfigurationData.SpearControllerCount = (ControllerCountEnum)Enum.Parse(typeof(ControllerCountEnum), value);
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseLeftSpear")]
        private bool useLeftSpear = Configuration.instance.ConfigurationData.UseLeftSpear;
        [UIAction("OnUseLeftSpearChanged")]
        private void OnUseLeftSpearChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseLeftSpear = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseTriggerToSwitchHands")]
        private bool useTriggerToSwitchHands = Configuration.instance.ConfigurationData.UseTriggerToSwitchHands;
        [UIAction("OnUseTriggerToSwitchHands")]
        private void OnUseTriggerToSwitchHands(bool value)
        {
            Configuration.instance.ConfigurationData.UseTriggerToSwitchHands = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("ReverseSpearDirection")]
        private bool reverseSpearDirection = Configuration.instance.ConfigurationData.ReverseSpearDirection;
        [UIAction("OnReverseSpearDirectionChanged")]
        private void OnReverseSpearDirectionChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseSpearDirection = value;
            Configuration.instance.SaveConfiguration();
        }

        protected override void DidActivate(bool firstActivation, ActivationType activationType)
        {
            base.DidActivate(firstActivation, activationType);
            SetTrackerText();
        }

        #region SelectTracker Modal Members

        private const string NoTrackerText = "Default";
        private const string NoTrackerHoverHint = "Not using any tracked devices";

        // Internal tracker selection members
        private TrackerDisplayText selectedTracker;
        private List<TrackerDisplayText> LoadedTrackers;
        private bool selectingLeft = false;

        // Components
        [UIComponent("SelectTrackerList")]
        public CustomListTableData trackerList;

        // Text Displays for the Main View
        private string leftSpearTrackerSerial;
        [UIValue("LeftSpearTrackerSerial")]
        public string LeftSpearTrackerSerial { get => this.leftSpearTrackerSerial; set { this.leftSpearTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftSpearTrackerSerial)); } }

        private string leftSpearTrackerHoverHint;
        [UIValue("LeftSpearTrackerHoverHint")]
        public string LeftSpearTrackerHoverHint { get => this.leftSpearTrackerHoverHint; set { this.leftSpearTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftSpearTrackerHoverHint)); } }

        private string rightSpearTrackerSerial;
        [UIValue("RightSpearTrackerSerial")]
        public string RightSpearTrackerSerial { get => this.rightSpearTrackerSerial; set { this.rightSpearTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightSpearTrackerSerial)); } }

        private string rightSpearTrackerHoverHint;
        [UIValue("RightSpearTrackerHoverHint")]
        public string RightSpearTrackerHoverHint { get => this.rightSpearTrackerHoverHint; set { this.rightSpearTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightSpearTrackerHoverHint)); } }

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
                configData.LeftSpearTracker = this.selectedTracker == null ? null : this.selectedTracker.Serial;
                configData.LeftSpearTrackerFullName = this.selectedTracker == null ? null : this.selectedTracker.HoverHint;

                this.LeftSpearTrackerSerial = this.selectedTracker == null ? NoTrackerText : this.selectedTracker.Serial;
                this.LeftSpearTrackerHoverHint = this.selectedTracker == null ? NoTrackerHoverHint : this.selectedTracker.HoverHint;
            }
            else
            {
                configData.RightSpearTracker = this.selectedTracker == null ? null : this.selectedTracker.Serial;
                configData.RightSpearTrackerFullName = this.selectedTracker == null ? null : this.selectedTracker.HoverHint;

                this.RightSpearTrackerSerial = this.selectedTracker == null ? NoTrackerText : this.selectedTracker.Serial;
                this.RightSpearTrackerHoverHint = this.selectedTracker == null ? NoTrackerHoverHint : this.selectedTracker.HoverHint;
            }
            Configuration.instance.SaveConfiguration();
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftSpearTracker = null;
            Configuration.instance.ConfigurationData.LeftSpearTrackerFullName = null;
            Configuration.instance.SaveConfiguration();
            this.LeftSpearTrackerSerial = NoTrackerText;
            this.LeftSpearTrackerHoverHint = NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightSpearTracker = null;
            Configuration.instance.ConfigurationData.RightSpearTrackerFullName = null;
            Configuration.instance.SaveConfiguration();
            this.RightSpearTrackerSerial = NoTrackerText;
            this.RightSpearTrackerHoverHint = NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftSpearTracker))
            {
                this.LeftSpearTrackerSerial = NoTrackerText;
                this.LeftSpearTrackerHoverHint = NoTrackerHoverHint;
            }
            else
            {
                this.LeftSpearTrackerSerial = config.LeftSpearTracker;
                this.LeftSpearTrackerHoverHint = config.LeftSpearTrackerFullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightSpearTracker))
            {
                this.RightSpearTrackerSerial = NoTrackerText;
                this.RightSpearTrackerHoverHint = NoTrackerHoverHint;
            }
            else
            {
                this.RightSpearTrackerSerial = config.RightSpearTracker;
                this.RightSpearTrackerHoverHint = config.RightSpearTrackerFullName;
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
                this.CurrentTrackerText = String.IsNullOrWhiteSpace(configData.LeftSpearTrackerFullName) ? NoTrackerHoverHint : configData.LeftSpearTrackerFullName;
            }
            else
            {
                this.CurrentTrackerText = String.IsNullOrWhiteSpace(configData.RightSpearTrackerFullName) ? NoTrackerHoverHint : configData.RightSpearTrackerFullName;
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

            string serialToFind = this.selectingLeft ? configData.LeftSpearTracker : configData.RightSpearTracker;
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
