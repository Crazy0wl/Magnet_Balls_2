using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public enum PrizeType
    {
        Gems,
        Lives
    }
    [Serializable]
    public class Prize
    {
        public int Count;
        public PrizeType PrizeType;
    }

    public class SpinWheel : MonoBehaviour
    {
        public List<WheelBonus> Bonuses;
        public List<AnimationCurve> animationCurves;
        public bool Spining { get; private set; }
        private float anglePerItem;
        private int randomTime;
        private int itemNumber;
        public Transform SpinTransform;
        public event Action SpinStarted = delegate { };
        public event Action<WheelBonus> SpinFinished = delegate { };
        public Arrow WheelArrow;

        void Start()
        {
            anglePerItem = 360 / Bonuses.Count;
        }

        private void OnEnable()
        {
            SpinTransform.rotation = Quaternion.identity;
        }

        public void Spin()
        {
            randomTime = UnityEngine.Random.Range(1, 4);
            itemNumber = UnityEngine.Random.Range(0, Bonuses.Count);
            float maxAngle = 360 * randomTime + (itemNumber * anglePerItem);
            StartCoroutine(SpinWheelRoutine(5 * randomTime, maxAngle));
        }

        private IEnumerator SpinWheelRoutine(float time, float maxAngle)
        {
            Spining = true;
            if (SpinStarted != null)
            {
                SpinStarted();
            }

            float timer = 0.0f;
            float startAngle = SpinTransform.eulerAngles.z;	
            maxAngle = maxAngle - startAngle;
            int animationCurveNumber = UnityEngine.Random.Range(0, animationCurves.Count);
            Debug.Log("Animation Curve No. : " + animationCurveNumber);

            while (timer < time)
            {
                float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / time);
                SpinTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle + startAngle);
                timer += Time.deltaTime;
                yield return 0;
            }
            SpinTransform.eulerAngles = new Vector3(0.0f, 0.0f, maxAngle + startAngle);
            if (SpinFinished != null)
            {
                SpinFinished(WheelArrow.CurrBonus);
            }
            Spining = false;
        }
    }
}