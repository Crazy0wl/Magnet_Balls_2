using UnityEngine;

namespace MB_Engine
{
    public class DailyBonusButton : MonoBehaviour
    {
        public int Count;
        public UILabel countLabel;
        private UISprite bonusIcon;

        public void Init(bool active)
        {
            countLabel.text = Count.ToString();
            Transform bonusIconTransform = transform.Find("Bonus");
            bonusIcon = bonusIconTransform.GetComponent<UISprite>();
            UIButton btn = GetComponent<UIButton>();
            btn.isEnabled = active;
            countLabel.alpha = active ? 1f : 0.5f;
            bonusIcon.alpha = active ? 1f : 0.5f;
        }

        public void Spend()
        {
            if (Count > 0)
            {
                Count--;
                countLabel.text = Count.ToString();
                GameData.Coins++;
            }
        }
    }
}