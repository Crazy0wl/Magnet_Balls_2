using UnityEngine;

namespace MB_Engine
{
    public class RemoteSettingsManager : MonoBehaviour
    {
        public float ShowAdsPercent;

        private void Start()
        {
            ShowAdsPercent = RemoteSettings.GetFloat("ShowAdsPercent", 1f);
            RemoteSettings.Updated += RemoteSettings_Updated;
        }

        private void RemoteSettings_Updated()
        {
            ShowAdsPercent = RemoteSettings.GetFloat("ShowAdsPercent", 1f);
        }
    }
}
