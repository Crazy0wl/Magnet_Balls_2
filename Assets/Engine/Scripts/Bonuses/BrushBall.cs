using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class BrushBall : BaseBonus
    {
        public override BallType BonusType { get { return BallType.Brush; } }
        private Collider2D[] brushHitColliders = new Collider2D[16];
        private RaycastHit2D[] brushRayCastHit = new RaycastHit2D[1];

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay));
        }

        private IEnumerator EmitRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetVelocity(MainParticles, ball.Vel);
            SetColor(ball.RealColor);
            PlaySound("squish", Random.Range(0.9f, 1.1f), 0f, 1f);
            Vector2 pos = transform.position = ball.Pos;
            ball.Destroy();
            int collidersCount = Physics2D.OverlapCircleNonAlloc(pos, Radius, brushHitColliders, LayerMask.GetMask(CollisionMask));
            for (int i = 0; i < collidersCount; i++)
            {
                Collider2D hit = brushHitColliders[i];
                if (hit.attachedRigidbody != null && !hit.attachedRigidbody.isKinematic)
                {
                    Vector2 direction = hit.attachedRigidbody.position - pos;
                    int count = Physics2D.LinecastNonAlloc(pos, hit.attachedRigidbody.position, brushRayCastHit, LayerMask.GetMask(CollisionMask));
                    if (count > 0)
                    {
                        RaycastHit2D raycastHit = brushRayCastHit[0];
                        if (raycastHit.rigidbody != null && raycastHit.rigidbody == hit.attachedRigidbody)
                        {
                            Ball b = hit.GetComponent<Ball>();
                            if (b && !b.Destroying)
                            {
                                Vector2 force = direction.normalized * Power;
                                b.BallBody2D.AddForceAtPosition(force, pos, ForceMode2D.Impulse);

                                b.BallColor = ball.BallColor;

                                if (b.Colored)
                                {
                                    b.CheckChain(0.25f);
                                }
                                yield return null;
                            }
                        }
                    }
                }
            }
            yield return null;
        }
    }
}