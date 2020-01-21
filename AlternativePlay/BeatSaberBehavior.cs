using UnityEngine;

namespace AlternativePlay
{
    public class BeatSaberBehavior : MonoBehaviour
    {
        private PlayerController playerController;

        /// <summary>
        /// To be invoked every time when starting the GameCore scene.
        /// </summary>
        public void BeginGameCoreScene()
        {
            // Currently nothing needs to be done on GameCore start
        }

        private void Awake()
        {
            this.playerController = FindObjectOfType<PlayerController>();
        }

        private void Update()
        {
            if (ConfigOptions.instance.PlayMode != PlayMode.BeatSaber || playerController == null)
            {
                // Do nothing if we aren't playing Beat Saber
                return;
            }

            if (ConfigOptions.instance.ReverseSaberDirection)
            {
                playerController.leftSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
                playerController.rightSaber.transform.Rotate(0.0f, 180.0f, 180.0f);
            }
        }
    }
}
