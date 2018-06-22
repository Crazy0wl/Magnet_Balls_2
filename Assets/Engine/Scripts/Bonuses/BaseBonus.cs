using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public class BaseBonus : MonoBehaviour
    {
        #region fields
        public string Name { get { return transform.name; } }
        public string CollisionMask;
        public ParticleSystem[] Particles;
        private ParticleSystem ps;
        public ParticleSystem MainParticles
        {
            get
            {
                if (ps)
                {
                    return ps;
                }
                ps = GetComponent<ParticleSystem>();
                return ps;
            }
        }
        public Vector2 Pos { get { return new Vector2(transform.position.x, transform.position.y); } }
        protected Ball ball;
        private FastPool pool;
        public float Radius;
        public float Power;
        public float LifeTime;
        public virtual BallType BonusType { get { return BallType.None; } }
        private Collider2D[] hitColliders = new Collider2D[16];
        private RaycastHit2D[] raycastHits = new RaycastHit2D[1];
        #endregion

        private void OnEnable()
        {
            Destroy(LifeTime);
        }

        public virtual void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            this.pool = pool;
            this.ball = ball;
            StartCoroutine(EnableParticlesRoutine(delay));
        }

        protected void Destroy(float delay)
        {
            StartCoroutine(DestroyRoutine(delay));
        }

        protected void SetVelocity(ParticleSystem ps, Vector3 vel)
        {
            ParticleSystem.VelocityOverLifetimeModule v = ps.velocityOverLifetime;
            ParticleSystem.MinMaxCurve velocityCurve = new ParticleSystem.MinMaxCurve();
            velocityCurve.constantMax = vel.x;
            v.x = velocityCurve;
            velocityCurve.constantMax = vel.y;
            v.y = velocityCurve;
            velocityCurve.constantMax = vel.z;
            v.z = velocityCurve;
        }

        public void ForceBalls()
        {
            StartCoroutine(ForceBallsRoutine());
        }

        private IEnumerator ForceBallsRoutine()
        {
            int collidersCount = Physics2D.OverlapCircleNonAlloc(ball.Pos, Radius, hitColliders, LayerMask.GetMask(CollisionMask));
            for (int i = 0; i < collidersCount; i++)
            {
                Collider2D hit = hitColliders[i];
                if (hit && hit.attachedRigidbody != null && !hit.attachedRigidbody.isKinematic)
                {
                    Vector2 direction = hit.attachedRigidbody.position - ball.Pos;
                    int count = Physics2D.LinecastNonAlloc(ball.Pos, hit.attachedRigidbody.position, raycastHits, LayerMask.GetMask(CollisionMask));
                    if (count > 0)
                    {
                        RaycastHit2D raycastHit = raycastHits[0];
                        if (raycastHit.rigidbody != null && raycastHit.rigidbody == hit.attachedRigidbody)
                        {
                            Vector2 force = direction.normalized * Power;
                            hit.attachedRigidbody.AddForceAtPosition(force * Power, ball.Pos, ForceMode2D.Force);
                            yield return null;
                        }
                    }
                }
            }
        }

        protected List<Ball> ForceBalls(float radius, float stretch)
        {
            List<Ball> result = new List<Ball>();
            for (int i = 0; i < Physics2D.OverlapCircleNonAlloc(Pos, radius, hitColliders, LayerMask.GetMask(CollisionMask)); i++)
            {
                Ball b = hitColliders[i].GetComponent<Ball>();
                if (b && b != this && !b.IsKinematic && !b.Destroying)
                {
                    result.Add(b);
                    Vector2 force = (b.Pos - Pos).normalized;
                    b.BallBody2D.AddForceAtPosition(force * stretch, this.ball.Pos, ForceMode2D.Impulse);
                }
            }
            return result;
        }

        protected void ForceBodies2D(float radius, float stretch)
        {
            for (int i = 0; i < Physics2D.OverlapCircleNonAlloc(Pos, radius, hitColliders, LayerMask.GetMask(CollisionMask)); i++)
            {
                if (hitColliders[i].attachedRigidbody)
                {
                    float distance = Vector2.Distance(hitColliders[i].attachedRigidbody.position, Pos);
                    Vector2 force = (hitColliders[i].attachedRigidbody.position - Pos).normalized * stretch / distance;
                    hitColliders[i].attachedRigidbody.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }

        protected void ForceBodies3D(float radius, float stretch)
        {
            Collider[] hitColliders = new Collider[64];
            for (int i = 0; i < Physics.OverlapSphereNonAlloc(Pos, radius, hitColliders, LayerMask.GetMask(CollisionMask)); i++)
            {
                if (hitColliders[i].attachedRigidbody)
                {
                    hitColliders[i].attachedRigidbody.AddExplosionForce(stretch, Pos, radius);
                }
            }
        }

        protected void SetColor(Color color)
        {
            foreach (ParticleSystem ps in Particles)
            {
                var module = ps.main;
                module.startColor = color;
            }
        }

        protected void PlaySound(string soundName, float pitch = 1f, float delay = 0, float volume = 1f)
        {
            if (!string.IsNullOrEmpty(soundName))
            {
                SoundManager.Play(soundName, ball.transform, pitch, volume, delay);
            }
        }

        #region routines

        private IEnumerator EnableParticlesRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (MainParticles)
            {
                MainParticles.Play();
            }
        }

        private IEnumerator DestroyRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (ball.Destroying)
            {
                ball.Destroy();
            }
            pool.FastDestroy(gameObject);
        }

        protected IEnumerator ForceBodies2DRoutine(float radius, float stretch)
        {
            for (int i = 0; i < Physics2D.OverlapCircleNonAlloc(Pos, radius, hitColliders, LayerMask.GetMask(CollisionMask)); i++)
            {
                Ball ball = hitColliders[i].GetComponent<Ball>();
                if (hitColliders[i].attachedRigidbody)
                {
                    float distance = Vector2.Distance(hitColliders[i].attachedRigidbody.position, Pos);
                    Vector2 force = (ball.Pos - Pos).normalized * stretch / distance;
                    hitColliders[i].attachedRigidbody.AddForce(force, ForceMode2D.Impulse);
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        #endregion
    }
}