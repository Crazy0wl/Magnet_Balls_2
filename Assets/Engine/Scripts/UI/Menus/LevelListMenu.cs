using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class LevelListMenu : Menu
    {
        #region fields
        public UICenterOnChild centerOnChild;
        public UISprite BackgroundSprite;
        public GameObject CenteredObject;
        public GameObject[] LevelPanelMarks;
        public GameObject[] LevelPanels;
        private int currPanelNum;
        public int CurrPanelNum
        {
            get
            {
                int result = PlayerPrefs.GetInt("LevelPanelNum", 0);
                return result;
            }
            set
            {
                PlayerPrefs.SetInt("LevelPanelNum", value);
            }
        }
        private Color destColor;
        public Color[] BackgroundColors;
        public Background GameplayBackground;
        #endregion

        public override bool Show(float delay)
        {
            base.Show();
            
            centerOnChild.onCenter = delegate (GameObject go) { CenterFinished(go); };
            StartCoroutine(GoToCurrentPanel());
            return true;
        }

        public override bool Hide()
        {
            CurrPanelNum = currPanelNum;
            return base.Hide();
        }

        private IEnumerator GoToCurrentPanel()
        {
            yield return new WaitForEndOfFrame();
            centerOnChild.CenterOn(LevelPanels[CurrPanelNum].transform);
        }

        public void PreviewPressed()
        {
            if (currPanelNum > 0)
            {
                centerOnChild.CenterOn(LevelPanels[currPanelNum - 1].transform);
            }
        }

        public void NextPressed()
        {
            if (currPanelNum < LevelPanels.Length - 1)
            {
                centerOnChild.CenterOn(LevelPanels[currPanelNum + 1].transform);
            }
        }

        private void CenterFinished(GameObject centeredObject)
        {
            CenteredObject = centeredObject;
            int panelNum = 0;
            if (int.TryParse(centeredObject.name, out panelNum))
            {
                for (int i = 0; i < LevelPanelMarks.Length; i++)
                {
                    NGUITools.SetActive(LevelPanelMarks[i], false);
                }
                currPanelNum = panelNum - 1;
                NGUITools.SetActive(LevelPanelMarks[panelNum - 1], true);
                GameplayBackground.Color = destColor = BackgroundColors[panelNum - 1];
            }
        }

        private void LateUpdate()
        {
            BackgroundSprite.color = Color.Lerp(BackgroundSprite.color, destColor, 0.05f);
        }

        public void Close()
        {
            Hide();
            UIManager.Main.Show();
        }

        protected override void OnBackPressed()
        {
            UIManager.Main.Show();
        }
    }
}