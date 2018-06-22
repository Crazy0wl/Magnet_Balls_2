using UnityEngine;

namespace MB_Engine
{
    [RequireComponent(typeof(UIButton))]
    public class LevelButton : MonoBehaviour
    {
        private Transform[] Stars;
        private Level level;
        private UIButton btn;

        private void OnEnable()
        {
            if (Stars == null)
            {
                Stars = new Transform[3];
                Stars[0] = transform.Find("Num/Stars/Star1/Star");
                Stars[1] = transform.Find("Num/Stars/Star2/Star");
                Stars[2] = transform.Find("Num/Stars/Star3/Star");
            }
            if (!btn)
            {
                btn = GetComponent<UIButton>();
            }

            int levelNum = 0;
            if (int.TryParse(gameObject.name, out levelNum))
            {
                GameObject LockObject = transform.Find("Lock").gameObject;
                GameObject NumObject = transform.Find("Num").gameObject;
                UIButton btn = GetComponent<UIButton>();
                if (Levels.main.levels.Count >= levelNum)
                {
                    level = Levels.main.levels[levelNum - 1];
                }
                NumObject.GetComponent<UILabel>().text = levelNum.ToString();
                if (levelNum > GameData.MaxLevel)
                {
                    LockObject.SetActive(true);
                    NumObject.SetActive(false);
                    btn.isEnabled = false;
                }
                else
                {
                    btn.isEnabled = true;
                    LockObject.SetActive(false);
                    NumObject.SetActive(true);
                }

                int starsCount = GameData.GetLevelStars(levelNum);
                for (int i = 0; i < 3; i++)
                {
                    Stars[i].gameObject.SetActive(i < starsCount);
                }
            }
        }

        private void OnClick()
        {
            UIManager.LevelList.Hide();
            UIManager.Gameplay.Show(level);
        }
    } 
}