using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class BombBall : BaseBonus
    {
        public override BallType BonusType { get { return BallType.Bomb; } }
        private Collider2D[] bombHitColliders = new Collider2D[32];
        private RaycastHit2D[] bombRayCastHits = new RaycastHit2D[1];

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay));
        }

        private IEnumerator EmitRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetVelocity(MainParticles, ball.Vel);
            PlaySound("bomb", Random.Range(0.9f, 1.1f), 0f, 1f);
            Vector2 pos = transform.position = ball.Pos;
            bool wasExplode = false;
            ball.Destroy();
            float minRadius = Radius;
            float maxRadius = Radius * 2f;
            int collidersCount = Physics2D.OverlapCircleNonAlloc(pos, maxRadius, bombHitColliders, LayerMask.GetMask(CollisionMask));
            for (int i = 0; i < collidersCount; i++)
            {
                Collider2D hit = bombHitColliders[i];
                if (hit.attachedRigidbody != null && !hit.attachedRigidbody.isKinematic)
                {
                    Vector2 direction = hit.attachedRigidbody.position - pos;
                    int count = Physics2D.LinecastNonAlloc(pos, hit.attachedRigidbody.position, bombRayCastHits, LayerMask.GetMask(CollisionMask));
                    if (count > 0)
                    {
                        RaycastHit2D raycastHit = bombRayCastHits[0];
                        if (raycastHit.rigidbody != null && raycastHit.rigidbody == hit.attachedRigidbody)
                        {
                            Debug.DrawLine(pos, hit.attachedRigidbody.position, Color.red, 1f);
                            Ball b = hit.GetComponent<Ball>();
                            if (b && !b.Destroying)
                            {
                                float distance = Vector2.Distance(pos, b.Pos);
                                Vector2 force = direction.normalized * Power;
                                b.BallBody2D.AddForceAtPosition(force, pos, ForceMode2D.Impulse);
                                yield return null;
                                //if (b.BallType != BallType.Frozen && b.BallType != BallType.Bubble && b.BallType != BallType.Bomb && distance < minRadius)
                                if (distance < minRadius)
                                {
                                    if (b.BallType != BallType.Frozen && b.BallType != BallType.Bubble)
                                    {
                                        b.BallType = BallType.Crashed;
                                    }
                                    b.Kill(Balls.main.BallScore, force / distance, distance * 0.05f * Time.timeScale);
                                    Balls.EmitEffect("BallCollision", raycastHit.point, force / 10f);
                                    wasExplode = true;
                                }
                            }
                        }
                    }
                }
            }
            if (wasExplode)
            {
                SoundManager.Play("ballsCrack", pos, Random.Range(1.4f, 1.6f), 1f, 0.05f);
            }
            Balls.Shake();
        }
    }
}