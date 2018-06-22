using UnityEngine;

namespace MB_Engine
{
    public class GameBanners : MonoBehaviour
    {
        public GameObject[] Banners;

        void Start()
        {
            int randIndex = Random.Range(0, Banners.Length);
            SetActiveBanner(randIndex);
        }

        private void SetActiveBanner(int num)
        {
            for (int i = 0; i < Banners.Length; i++)
            {
                Banners[i].SetActive(false);
            }
            Banners[num].SetActive(true);
        }

        public void GoToApp(string appId)
        {
            string url = "market://details?id=" + appId;
            Application.OpenURL(url);
        }
    }
}