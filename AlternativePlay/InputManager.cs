using UnityEngine;
using UnityEngine.XR;

namespace AlternativePlay
{
    /// <summary>
    /// This class manages the input system, most importantly helping to gate button presses
    /// on the controller, basically debouncing buttons.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        private InputDevice leftController;
        private InputDevice rightController;
        private bool leftTriggerCanClick;
        private bool rightTriggerCanClick;
        private bool bothTriggerCanClick;
        private bool isPolling;

        #region Button Polling Methods

        public bool LeftTriggerDown { get; private set; }
        public bool RightTriggerDown { get; private set; }
        public bool BothTriggerDown { get; private set; }

        public bool GetLeftTriggerClicked()
        {
            bool returnValue = false;
            if (this.leftTriggerCanClick && this.LeftTriggerDown)
            {
                returnValue = true;
                this.leftTriggerCanClick = false;
            }
            return returnValue;
        }

        public bool GetRightTriggerClicked()
        {
            bool returnValue = false;
            if (this.rightTriggerCanClick && this.RightTriggerDown)
            {
                returnValue = true;
                this.rightTriggerCanClick = false;
            }

            return returnValue;
        }

        public bool GetBothTriggerClicked()
        {
            bool returnValue = false;
            if (this.bothTriggerCanClick && this.BothTriggerDown)
            {
                returnValue = true;
                this.bothTriggerCanClick = false;
            }

            return returnValue;
        }

        #endregion

        #region MonoBehavior Methods

        public void BeginPolling()
        {
            this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            this.leftTriggerCanClick = true;
            this.rightTriggerCanClick = true;
            this.bothTriggerCanClick = true;
            this.isPolling = true;
        }

        internal void EndPolling()
        {
            this.isPolling = false;
        }

        private void Update()
        {
            const float pulled = 0.75f;
            const float released = 0.2f;

            if (!this.isPolling) return;

            this.leftController.TryGetFeatureValue(CommonUsages.trigger, out float leftTriggerValue);
            this.rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTriggerValue);

            this.LeftTriggerDown = leftTriggerValue > pulled;
            this.RightTriggerDown = rightTriggerValue > pulled;
            this.BothTriggerDown = this.LeftTriggerDown && this.RightTriggerDown;

            if (leftTriggerValue < released) { leftTriggerCanClick = true; }
            if (rightTriggerValue < released) { rightTriggerCanClick = true; }
            if (leftTriggerValue < released && rightTriggerValue < released) { this.bothTriggerCanClick = true; }
        }

        #endregion
    }
}
