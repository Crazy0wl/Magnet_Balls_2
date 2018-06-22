using System.Collections;
using UnityEngine;


namespace MB_Engine
{
    public class Anchor : BaseBonus
    {
        public override BallType BonusType { get { return BallType.Anchor; } }

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay));
        }

        private IEnumerator EmitRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            //SetVelocity(MainParticles, ball.Vel);
            //SetColor(ball.realColor);
            //PlaySound("ballDestroy", Random.Range(0.9f, 1.1f), 0f, 1f);
            //ForceBalls(Radius, Force);
            //ball.Destroy();
        }
    }
}
