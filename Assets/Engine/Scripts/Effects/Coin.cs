using UnityEngine;

namespace MB_Engine
{
    public class Coin : Effect
    {
        public int Cost;
        public new void Emit(Vector3 vel)
        {
            GameData.Coins += Cost;
            base.Emit(vel);
        }
    }
}
