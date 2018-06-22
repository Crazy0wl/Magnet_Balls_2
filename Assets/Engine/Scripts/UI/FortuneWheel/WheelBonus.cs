using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class WheelBonus : MonoBehaviour
    {
        #region fields
        //public int MinCount;
        //public int MaxCount;
        public int Count;
        private int currCount;
        //public int Mult = 5;
        private UILabel countLabel;
        public GameObject PinObject;
        #endregion

        private void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            //Count = Random.Range(MinCount, MaxCount) * Mult;
            //if (Count < 0)
            //{
            //    Count = 0;
            //}
            currCount = Count;
            countLabel = GetComponentInChildren<UILabel>();
            countLabel.text = string.Format("{0}", Count);
        }

        private void LateUpdate()
        {
            PinObject.transform.rotation = Quaternion.identity;
        }

        public IEnumerator GetPrizeRoutine()
        {
            while (currCount-- > 0)
            {
                GameData.Coins++;
                countLabel.text = string.Format("{0}", currCount);
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
    }
}
