using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class AnchoredBall : BaseBonus
    {
        public override BallType BonusType { get { return BallType.Anchored; } }

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay));
        }

        private IEnumerator EmitRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Vector3 vel = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 5f), Random.Range(-8f, 0f));
            SetVelocity(MainParticles, vel);
            SetColor(ball.RealColor);
            PlaySound("ballDestroy", Random.Range(0.9f, 1.1f), 0f, 1f);
            ForceBalls(Radius, Power);
            ball.Destroy();
        }
    }
}
