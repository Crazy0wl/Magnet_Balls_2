using UnityEngine;

namespace MB_Engine
{
    public class BonusButton : MonoBehaviour
    {
        public BallType Bonus;
        public int Cost;
        private UILabel costLabel;
        public float ShowDelay;
        public int PosY;

        private void OnEnable()
        {
            costLabel = GetComponentInChildren<UILabel>();
            costLabel.text = Cost.ToString();
            UIButton btn = GetComponent<UIButton>();
            btn.isEnabled = Cost <= GameData.Coins;
            gameObject.transform.localScale = Vector3.zero;

            Vector3 fromPos = transform.localPosition;
            fromPos.y = 100f;
            Vector3 toPos = transform.localPosition;
            toPos.y = PosY;
            UIManager.Move(transform, fromPos, toPos, ShowDelay);
            UIManager.ScaleTo(transform, 0f, 1f, ShowDelay * 1.25f);
        }
    }
}