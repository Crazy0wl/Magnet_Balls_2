using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class FreezeBall : BaseBonus
    {
        public override BallType BonusType { get { return BallType.Freeze; } }
        private Collider2D[] freezeHitColliders = new Collider2D[16];
        private RaycastHit2D[] freezeRayCastHit = new RaycastHit2D[1];

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay));
        }

        private IEnumerator EmitRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetVelocity(MainParticles, ball.Vel);
            PlaySound("freezing", Random.Range(0.9f, 1.1f), 0f, 1f);
            yield return new WaitForEndOfFrame();
            Vector2 pos = transform.position = ball.Pos;
            ball.Destroy();
            int collidersCount = Physics2D.OverlapCircleNonAlloc(pos, Radius, freezeHitColliders, LayerMask.GetMask(CollisionMask));
            for (int i = 0; i < collidersCount; i++)
            {
                Collider2D hit = freezeHitColliders[i];
                if (hit.attachedRigidbody != null && !hit.attachedRigidbody.isKinematic)
                {
                    Vector2 direction = hit.attachedRigidbody.position - pos;
                    int count = Physics2D.LinecastNonAlloc(pos, hit.attachedRigidbody.position, freezeRayCastHit, LayerMask.GetMask(CollisionMask));
                    if (count > 0)
                    {
                        RaycastHit2D raycastHit = freezeRayCastHit[0];
                        if (raycastHit.rigidbody != null && raycastHit.rigidbody == hit.attachedRigidbody)
                        {
                            Debug.DrawLine(pos, hit.attachedRigidbody.position, Color.red, 1f);
                            Ball b = hit.GetComponent<Ball>();
                            if (b && !b.Destroying)
                            {
                                Vector2 force = direction.normalized * Power;
                                b.BallBody2D.AddForceAtPosition(force, pos, ForceMode2D.Impulse);
                                if (b.BallType == BallType.Bubble)
                                {
                                    b.Kill(Balls.main.BallScore, force, 0f);
                                }
                                else
                                {
                                    b.BallType = BallType.Frozen;
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