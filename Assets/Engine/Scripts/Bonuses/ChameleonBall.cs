using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class ChameleonBall : BaseBonus
    {
        public override BallType BonusType { get { return BallType.Chameleon; } }

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay));
        }

        private IEnumerator EmitRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            transform.position = ball.Pos;
            ForceBalls();
            SetVelocity(MainParticles, ball.Vel);
            PlaySound("ballDestroy", Random.Range(0.9f, 1.1f), 0f, 1f);
            ball.Destroy();
        }
    }
}