using UnityEngine;

namespace MB_Engine
{
    public class Gems : MonoBehaviour
    {
        public UILabel GemsCountText;
       // public GameObject GemIcon;

        private void Start()
        {
            GameData.CoinsChanged += GameData_CoinsChanged;
            GemsCountText.text = GameData.Coins.ToString();
        }

        private void GameData_CoinsChanged(int arg1, int arg2)
        {
            GemsCountText.text = arg2.ToString();
          //  GemIcon.ShakeScale(Vector3.one * 0.2f, 0.5f, 0f);
        }
    }
}
