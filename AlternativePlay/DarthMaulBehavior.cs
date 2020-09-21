using AlternativePlay.Models;
using UnityEngine;

namespace AlternativePlay
{
    public class DarthMaulBehavior : MonoBehaviour
    {
        private PlayerController playerController;
        private InputManager inputManager;

        public static bool Split { get; set; }

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
        }

        private void Update()
        {
            if (Configuration.instance.ConfigurationData.PlayMode != PlayMode.DarthMaul || playerController == null)
            {
                // Do nothing if we aren't playing Darth Maul
                return;
            }

            if (Configuration.instance.ConfigurationData.UseTriggerToSeparate)
            {
                // Check to see if the trigger has been pressed
                bool leftTriggerPressed = inputManager.GetLeftTriggerClicked();
                bool rightTriggerPressed = inputManager.GetRightTriggerClicked();

                if (leftTriggerPressed || rightTriggerPressed)
                {
                    Split = !Split;
                }

                if (Split) return;  // When you split Darth Maul it's just regular two sabers so do nothing
            }

            float sep = 1.0f * Configuration.instance.ConfigurationData.MaulDistance / 100.0f;
            switch (Configuration.instance.ConfigurationData.DarthMaulControllerCount)
            {
                case ControllerCountEnum.One:
                    var baseSaber = Configuration.instance.ConfigurationData.UseLeftController ? playerController.leftSaber : playerController.rightSaber;
                    var otherSaber = Configuration.instance.ConfigurationData.UseLeftController ? playerController.rightSaber : playerController.leftSaber;
                    var rotateSaber = Configuration.instance.ConfigurationData.ReverseMaulDirection ? baseSaber : otherSaber;

                    otherSaber.transform.localPosition = baseSaber.transform.localPosition;
                    otherSaber.transform.localRotation = baseSaber.transform.localRotation;

                    rotateSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                    rotateSaber.transform.Translate(0.0f, 0.0f, sep * 2.0f, Space.Self);
                    break;

                case ControllerCountEnum.Two:
                    Vector3 leftHandPos = playerController.leftSaber.transform.position;
                    Vector3 rightHandPos = playerController.rightSaber.transform.position;
                    Vector3 middlePos = (rightHandPos + leftHandPos) * 0.5f;
                    Vector3 forward = (rightHandPos - leftHandPos).normalized;

                    var forwardSaber = Configuration.instance.ConfigurationData.ReverseMaulDirection ? playerController.leftSaber : playerController.rightSaber;
                    var backwardSaber = Configuration.instance.ConfigurationData.ReverseMaulDirection ? playerController.rightSaber : playerController.leftSaber;

                    forwardSaber.transform.position = middlePos + forward * sep;
                    backwardSaber.transform.position = middlePos + -forward * sep;
                    forwardSaber.transform.rotation = Quaternion.LookRotation(forward, backwardSaber.transform.up);
                    backwardSaber.transform.rotation = Quaternion.LookRotation(-forward, -backwardSaber.transform.up);
                    break;

                case ControllerCountEnum.None:
                default:
                    // Do nothing
                    break;
            }

        }
    }
}
