using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class FortuneWheel : Menu
    {
        public Rigidbody2D WheelBody;
        private bool Rotating { get { return Mathf.Abs(WheelBody.angularVelocity) > MinCheckSpeed; } }
        public float MaxForce;
        public float MinForce;
        public WheelBonus[] WheelBonuses;
        public float MinCheckSpeed;

        public override bool Show(float delay = 0f)
        {
            if (base.Show(delay))
            {
                for (int i = 0; i < WheelBonuses.Length; i++)
                {
                  //  WheelBonuses[i].Init();
                }
                return true;
            }
            return false;
        }

        public void SpinWheel()
        {
            if (!Rotating)
            {
                float force = -Random.Range(MinForce, MaxForce);
                WheelBody.AddTorque(force, ForceMode2D.Impulse);
                StartCoroutine(CheckRotationRoutine());

            }
        }

        private void SetBonus()
        {
            //BonusArrow.CurrBonus.gameObject.ScaleTo(Vector3.one * 1.5f, 0.5f, 0f, EaseType.spring);
            //BonusArrow.CurrBonus.GetBonus();
        }

        private IEnumerator CollectRoutine()
        {
            //while (BonusArrow.CurrBonus.Count > 0)
            //{
            //    BonusArrow.CurrBonus.Spend();
            //    yield return new WaitForSecondsRealtime(0.1f);
            //    GameData.AddBonus(BonusArrow.CurrBonus.Bonus);
            //}
            yield return new WaitForSecondsRealtime(0.2f);
            Hide();
        }

        private IEnumerator CheckRotationRoutine()
        {
            while (Rotating)
            {
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(CollectRoutine());
            yield return new WaitForSeconds(0.5f);
            Hide();
        }

        protected override void OnBackPressed()
        {
            if (!Rotating)
            {
                Hide();
            }
        }
    }
}