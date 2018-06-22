using UnityEngine;

namespace MB_Engine
{
    public class AccelLight : MonoBehaviour
    {
        private void LateUpdate()
        {
            float x = Mathf.Clamp(Input.acceleration.x, -30f, 30f);
            float y = Mathf.Clamp(Input.acceleration.y, -30f, 30f);
            float z = Input.acceleration.z;
            Quaternion targetRotation = new Quaternion(-x * 0.15f, -y * 0.15f, Input.acceleration.z * 1f, z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.2f);
        }
    }
}
