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
        private bool leftTriggerDown;
        private bool rightTriggerCanClick;
        private bool rightTriggerDown;
        private bool isPolling;

        #region Button Polling Methods

        public bool GetLeftTriggerDown()
        {
            return leftTriggerDown;
        }

        public bool GetLeftTriggerClicked()
        {
            bool returnValue = false;
            if (leftTriggerCanClick && leftTriggerDown)
            {
                returnValue = true;
                leftTriggerCanClick = false;
            }
            return returnValue;
        }

        public bool GetRightTriggerDown()
        {
            return rightTriggerDown;
        }

        public bool GetRightTriggerClicked()
        {
            bool returnValue = false;
            if (rightTriggerCanClick && rightTriggerDown)
            {
                returnValue = true;
                rightTriggerCanClick = false;
            }

            return returnValue;
        }

        #endregion

        #region MonoBehavior Methods

        public void BeginPolling()
        {
            this.leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            this.rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            leftTriggerCanClick = true;
            rightTriggerCanClick = true;
            isPolling = true;
        }

        internal void EndPolling()
        {
            isPolling = false;
        }

        private void Update()
        {
            const float pulled = 0.75f;
            const float released = 0.2f;

            if (!isPolling) return;

            this.leftController.TryGetFeatureValue(CommonUsages.trigger, out float leftTriggerValue);
            this.rightController.TryGetFeatureValue(CommonUsages.trigger, out float rightTriggerValue);

            leftTriggerDown = leftTriggerValue > pulled;
            rightTriggerDown = rightTriggerValue > pulled;

            if (leftTriggerValue < released) { leftTriggerCanClick = true; }
            if (rightTriggerValue < released) { rightTriggerCanClick = true; }
        }

        #endregion
    }
}
