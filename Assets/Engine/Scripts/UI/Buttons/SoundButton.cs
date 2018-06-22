using UnityEngine;

namespace MB_Engine
{
    public class SoundButton : MonoBehaviour
    {
        public GameObject ActiveIcon;
        public GameObject InactiveIcon;

        private void Awake()
        {
            RefreshState();
        }

        private void OnClick()
        {
            GameData.Muted = !GameData.Muted;
            RefreshState();
        }

        private void RefreshState()
        {
            ActiveIcon.SetActive(!GameData.Muted);
            InactiveIcon.SetActive(GameData.Muted);
            AudioListener.volume = GameData.Muted ? 0f : 1f;
        }
    }
}