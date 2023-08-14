using System.Reflection;
using UnityEngine;

namespace AlternativePlay
{
    public class AssetLoaderBehavior : MonoBehaviour
    {
        public GameObject TrackerPrefab { get; private set; }
        public GameObject SaberPrefab { get; private set; }
        public GameObject FlailHandleSegmentPrefab { get; private set; }
        public GameObject FlailTopCapPrefab { get; private set; }
        public GameObject FlailBottomCapPrefab { get; private set; }
        public GameObject LinkPrefab { get; private set; }

        private void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("AlternativePlay.Resources.alternativeplaymodels"));
            this.TrackerPrefab = assetBundle.LoadAsset<GameObject>("APTracker");
            this.SaberPrefab = assetBundle.LoadAsset<GameObject>("APSaber");
            this.FlailHandleSegmentPrefab = assetBundle.LoadAsset<GameObject>("APFlailHandleSegment");
            this.FlailTopCapPrefab = assetBundle.LoadAsset<GameObject>("APFlailTopCap");
            this.FlailBottomCapPrefab = assetBundle.LoadAsset<GameObject>("APFlailBottomCap");
            this.LinkPrefab = assetBundle.LoadAsset<GameObject>("APLink");
            assetBundle.Unload(false);
        }
    }
}
