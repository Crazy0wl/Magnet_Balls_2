using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class BubbleBall : BaseBonus
    {
        public override BallType BonusType { get { return BallType.Bubble; } }

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
            SetColor(ball.RealColor);
            PlaySound("bubblePop", Random.Range(0.9f, 1.1f), 0f, 1f);
            yield return new WaitForEndOfFrame();
            ball.Destroy();
        }
    }
}