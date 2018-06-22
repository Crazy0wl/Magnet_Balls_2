using UnityEngine;

namespace MB_Engine
{
    public class Arrow : MonoBehaviour
    {
        public int GemsCount;
        public WheelBonus CurrBonus;

        private void OnCollisionEnter2D(Collision2D col)
        {
            SoundManager.Play("buttonClick");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            WheelBonus bonus = collision.GetComponent<WheelBonus>();
            if (bonus)
            {
                CurrBonus = bonus;
                GemsCount = bonus.Count;
            }
        }
    }
}