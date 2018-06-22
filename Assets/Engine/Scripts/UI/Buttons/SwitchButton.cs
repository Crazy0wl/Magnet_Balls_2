using UnityEngine;

namespace MB_Engine
{
    [RequireComponent(typeof(UIButton))]
    public class SwitchButton : MonoBehaviour
    {
        public GameObject ActiveIcon;
        public GameObject InactiveIcon;
        protected bool Active;

        private void Awake()
        {
            RefreshIcon();
        }

        protected virtual void Pressed()
        {
            Active = !Active;
            RefreshIcon();
        }

        private void RefreshIcon()
        {
            ActiveIcon.SetActive(Active);
            InactiveIcon.SetActive(!Active);
        }

        private void OnClick()
        {
            Pressed();
        }
    }
}