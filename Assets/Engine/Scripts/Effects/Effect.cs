using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class Effect : MonoBehaviour
    {
        #region fields
        public string Name { get { return transform.name; } }
        public ParticleSystem[] Particles;
        private ParticleSystem ps;
        public ParticleSystem PS
        {
            get
            {
                if (!ps)
                {
                    ps = GetComponent<ParticleSystem>();
                }
                return ps;
            }
        }
        private FastPool pool;
        #endregion

        private void OnDisable()
        {
            if (pool != null)
            {
                pool.FastDestroy(gameObject);
            }
        }

        private void OnEnable()
        {
            Destroy(PS.main.duration * 2f);
        }

        private void Destroy(float delay)
        {
            StartCoroutine(DestroyRoutine(delay));
        }

        public void Init(FastPool pool)
        {
            this.pool = pool;
        }

        public void Emit(Vector3 velocity)
        {
            PS.Play(true);
            SetVelocity(velocity);
        }

        public void Emit(Color color)
        {
            PS.Play(true);
            setStartColor(color);
        }

        public void Emit(Color color, float delay)
        {
            PS.Play(true);
            ParticleSystem.MainModule psModule = PS.main;
            psModule.startDelay = delay;
            PS.Play(true);
            setStartColor(color);
        }

        #region private methods
        private void SetVelocity(Vector3 velocity)
        {
            SetVelocity(PS, velocity);
        }

        private void setStartColor(Color color)
        {
            foreach (ParticleSystem ps in Particles)
            {
                var module = ps.main;
                module.startColor = color;
            }
        }

        private void SetParameters(Color color, Vector3 velocity)
        {
            setStartColor(color);
            SetVelocity(velocity);
        }

        private void SetVelocity(ParticleSystem ps, Vector3 vel)
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

        private IEnumerator DestroyRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.FastDestroy(gameObject);
        }
        #endregion
    }
}