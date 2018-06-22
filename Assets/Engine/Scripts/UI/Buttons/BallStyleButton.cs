using UnityEngine;

namespace MB_Engine
{
    public class BallStyleButton : MonoBehaviour
    {
        public GameObject Normal;
        public GameObject ColorFigured;

        private void Start()
        {
            GameData.BallStyleChanged += GameData_BallStyleChanged;
        }

        private void OnEnable()
        {
            Refresh(GameData.BallStyle);
        }

        private void GameData_BallStyleChanged(BallStyle style)
        {
            Refresh(style);
        }

        private void Refresh(BallStyle style)
        {
            switch (style)
            {
                case BallStyle.Normal:
                    Normal.SetActive(true);
                    ColorFigured.SetActive(false);
                    break;
                case BallStyle.ColorFigured:
                    Normal.SetActive(false);
                    ColorFigured.SetActive(true);
                    break;
            }
        }

        public void OnSwithPressed()
        {
            switch (GameData.BallStyle)
            {
                case BallStyle.Normal:
                    GameData.BallStyle = BallStyle.ColorFigured;
                    break;
                case BallStyle.ColorFigured:
                    GameData.BallStyle = BallStyle.Normal;
                    break;
            }
        }
    }
}