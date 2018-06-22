using UnityEngine;

namespace MB_Engine
{
    public class FortuneWheelArrow : MonoBehaviour
    {
        public WheelBonus CurrBonus;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CurrBonus = collision.GetComponent<WheelBonus>();
        }
    }
}