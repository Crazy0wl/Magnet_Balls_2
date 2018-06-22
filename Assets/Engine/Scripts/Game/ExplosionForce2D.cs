using UnityEngine;

namespace MB_Engine
{
    public class ExplosionForce2D : MonoBehaviour
    {
        private void Explode(Vector3 pos, float radius, float power)
        {
            Collider2D[] hitColliders = new Collider2D[32];
            int collidersCount = Physics2D.OverlapCircleNonAlloc(pos, radius, hitColliders);
            for (int i = 0; i < collidersCount; i++)
            {
                Collider2D hit = hitColliders[i];
                if (hit.attachedRigidbody != null)
                {
                    Vector3 direction = hit.transform.position - pos;
                    direction.z = 0;
                    RaycastHit2D raycastHit = Physics2D.Linecast(pos, hit.attachedRigidbody.position);
                    if (raycastHit.rigidbody != null && raycastHit.rigidbody == hit.attachedRigidbody)
                    {
                        hit.attachedRigidbody.AddForce(direction.normalized * power);
                    }
                }
            }
        }
    }
}