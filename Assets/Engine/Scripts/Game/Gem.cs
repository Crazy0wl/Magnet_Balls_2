using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class Gem : MonoBehaviour
    {
        public Rigidbody Body;
        private FastPool pool;

        private void OnEnable()
        {
            Destroy(5f);
        }

        public void Init(FastPool pool)
        {
            this.pool = pool;
        }

        public void ApplyForce(Vector3 force)
        {
            transform.rotation = Random.rotation;
            Body.AddForce(force, ForceMode.Impulse);
            Body.AddTorque(force);
        }

        private void Destroy(float delay)
        {
            StartCoroutine(DestroyRoutine(delay));
        }

        private IEnumerator DestroyRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            pool.FastDestroy(gameObject);
        }
    }
}