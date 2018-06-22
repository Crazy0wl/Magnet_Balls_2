using UnityEngine;

namespace MB_Engine
{
    public class LevelStars : MonoBehaviour
    {
        public GameObject[] Stars;
        private int scoreStar1 = 100;
        private int scoreStar2 = 200;
        private int scoreStar3 = 300;
        private int starsCount;

        public void Show(int scoreStar1, int scoreStar2, int scoreStar3)
        {
            starsCount = 0;
            for (int i = 0; i < Stars.Length; i++)
            {
                Stars[i].SetActive(false);
            }
            this.scoreStar1 = scoreStar1;
            this.scoreStar2 = scoreStar2;
            this.scoreStar3 = scoreStar3;
        }

        public void UpdateState(int scoreValue)
        {
            if (GetStarsCount(scoreValue) > starsCount)
            {
                Stars[starsCount].SetActive(true);
                Stars[starsCount].ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
                starsCount++;
            }
        }

        public int GetStarsCount(int levelScore)
        {
            if (MathHelper.In(0, scoreStar1, levelScore))
            {
                return 1;
            }
            else if (MathHelper.In(scoreStar1, scoreStar2, levelScore))
            {
                return 2;
            }
            else if (levelScore > scoreStar3)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
    }
}