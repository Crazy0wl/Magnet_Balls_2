using UnityEngine;

namespace MB_Engine
{
    public class SettingsMenu : Menu
    {
        public GameObject AccelLock;
        public GameObject SoundLock;

        private void OnEnable()
        {
            AccelLock.SetActive(!GameData.AccelActive);
            SoundLock.SetActive(GameData.Muted);
        }

        public void OnClosePressed()
        {
            Hide();
        }

        public void OnSoundPressed()
        {
            GameData.Muted = !GameData.Muted;
            RefreshState();
        }

        public void OnAccelPressed()
        {
            GameData.AccelActive = !GameData.AccelActive;
            RefreshState();
        }

        private void RefreshState()
        {
            AccelLock.SetActive(!GameData.AccelActive);
            SoundLock.SetActive(GameData.Muted);
        }
    }
}