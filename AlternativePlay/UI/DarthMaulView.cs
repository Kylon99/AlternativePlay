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
    public class DarthMaulView : BSMLAutomaticViewController
    {
        [UIValue("ControllerChoice")]
        private string controllerChoice = Configuration.instance.ConfigurationData.DarthMaulControllerCount.ToString();
        [UIValue("ControllerChoiceList")]
        private List<object> controllerChoiceList = new List<object> { "One", "Two" };
        [UIAction("OnControllersChanged")]
        private void OnControllersChanged(string value)
        {
            Configuration.instance.ConfigurationData.DarthMaulControllerCount = (ControllerCountEnum)Enum.Parse(typeof(ControllerCountEnum), value);
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseLeftController")]
        private bool useLeftController = Configuration.instance.ConfigurationData.UseLeftController;
        [UIAction("OnUseLeftControllerChanged")]
        private void OnUseLeftControllerChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseLeftController = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("ReverseMaulDirection")]
        private bool reverseSaberDirection = Configuration.instance.ConfigurationData.ReverseMaulDirection;
        [UIAction("OnReverseMaulDirectionChanged")]
        private void OnReverseMaulDirectionChanged(bool value)
        {
            Configuration.instance.ConfigurationData.ReverseMaulDirection = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("UseTriggerToSeparate")]
        private bool useTriggerToSeparate = Configuration.instance.ConfigurationData.UseTriggerToSeparate;
        [UIAction("OnUseTriggerToSeparateChanged")]
        private void OnUseTriggerToSeparateChanged(bool value)
        {
            Configuration.instance.ConfigurationData.UseTriggerToSeparate = value;
            Configuration.instance.SaveConfiguration();
        }

        [UIValue("SeparationAmount")]
        private int separationAmount = Configuration.instance.ConfigurationData.MaulDistance;
        [UIAction("OnSeparationAmountChanged")]
        private void OnSeparationAmountChanged(int value)
        {
            Configuration.instance.ConfigurationData.MaulDistance = value;
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
        private string leftMaulTrackerSerial;
        [UIValue("LeftMaulTrackerSerial")]
        public string LeftMaulTrackerSerial { get => this.leftMaulTrackerSerial; set { this.leftMaulTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.LeftMaulTrackerSerial)); } }

        private string leftMaulTrackerHoverHint;
        [UIValue("LeftMaulTrackerHoverHint")]
        public string LeftMaulTrackerHoverHint { get => this.leftMaulTrackerHoverHint; set { this.leftMaulTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.LeftMaulTrackerHoverHint)); } }

        private string rightMaulTrackerSerial;
        [UIValue("RightMaulTrackerSerial")]
        public string RightMaulTrackerSerial { get => this.rightMaulTrackerSerial; set { this.rightMaulTrackerSerial = value; this.NotifyPropertyChanged(nameof(this.RightMaulTrackerSerial)); } }

        private string rightMaulTrackerHoverHint;
        [UIValue("RightMaulTrackerHoverHint")]
        public string RightMaulTrackerHoverHint { get => this.rightMaulTrackerHoverHint; set { this.rightMaulTrackerHoverHint = value; this.NotifyPropertyChanged(nameof(this.RightMaulTrackerHoverHint)); } }

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
                configData.LeftMaulTracker = this.selectedTracker == null ? null : this.selectedTracker.Serial;
                configData.LeftMaulTrackerFullName = this.selectedTracker == null ? null : this.selectedTracker.HoverHint;

                this.LeftMaulTrackerSerial = this.selectedTracker == null ? NoTrackerText : this.selectedTracker.Serial;
                this.LeftMaulTrackerHoverHint = this.selectedTracker == null ? NoTrackerHoverHint : this.selectedTracker.HoverHint;
            }
            else
            {
                configData.RightMaulTracker = this.selectedTracker == null ? null : this.selectedTracker.Serial;
                configData.RightMaulTrackerFullName = this.selectedTracker == null ? null : this.selectedTracker.HoverHint;

                this.RightMaulTrackerSerial = this.selectedTracker == null ? NoTrackerText : this.selectedTracker.Serial;
                this.RightMaulTrackerHoverHint = this.selectedTracker == null ? NoTrackerHoverHint : this.selectedTracker.HoverHint;
            }
            Configuration.instance.SaveConfiguration();
        }

        [UIAction("OnClearLeftTracker")]
        private void OnClearLeftTracker()
        {
            Configuration.instance.ConfigurationData.LeftMaulTracker = null;
            Configuration.instance.ConfigurationData.LeftMaulTrackerFullName = null;
            Configuration.instance.SaveConfiguration();
            this.LeftMaulTrackerSerial = NoTrackerText;
            this.LeftMaulTrackerHoverHint = NoTrackerHoverHint;
        }

        [UIAction("OnClearRightTracker")]
        private void OnClearRightTracker()
        {
            Configuration.instance.ConfigurationData.RightMaulTracker = null;
            Configuration.instance.ConfigurationData.RightMaulTrackerFullName = null;
            Configuration.instance.SaveConfiguration();
            this.RightMaulTrackerSerial = NoTrackerText;
            this.RightMaulTrackerHoverHint = NoTrackerHoverHint;
        }

        /// <summary>
        /// Initializes the bound variables for the fields on this view
        /// </summary>
        private void SetTrackerText()
        {
            var config = Configuration.instance.ConfigurationData;
            if (String.IsNullOrWhiteSpace(config.LeftMaulTracker))
            {
                this.LeftMaulTrackerSerial = NoTrackerText;
                this.LeftMaulTrackerHoverHint = NoTrackerHoverHint;
            }
            else
            {
                this.LeftMaulTrackerSerial = config.LeftMaulTracker;
                this.LeftMaulTrackerHoverHint = config.LeftMaulTrackerFullName;
            }

            if (String.IsNullOrWhiteSpace(config.RightMaulTracker))
            {
                this.RightMaulTrackerSerial = NoTrackerText;
                this.RightMaulTrackerHoverHint = NoTrackerHoverHint;
            }
            else
            {
                this.RightMaulTrackerSerial = config.RightMaulTracker;
                this.RightMaulTrackerHoverHint = config.RightMaulTrackerFullName;
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
                this.CurrentTrackerText = String.IsNullOrWhiteSpace(configData.LeftMaulTrackerFullName) ? NoTrackerHoverHint : configData.LeftMaulTrackerFullName;
            }
            else
            {
                this.CurrentTrackerText = String.IsNullOrWhiteSpace(configData.RightMaulTrackerFullName) ? NoTrackerHoverHint : configData.RightMaulTrackerFullName;
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

            string serialToFind = this.selectingLeft ? configData.LeftMaulTracker : configData.RightMaulTracker;
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
