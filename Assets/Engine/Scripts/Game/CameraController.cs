using UnityEngine;

namespace MB_Engine
{
    public class CameraController : MonoBehaviour
    {
        #region fields
        public GameObject Target;       
        private Vector3 offset;
        private bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                if (value)
                {
                    Init();
                }
            }
        }
        public Camera Cam;
        public float Speed = 0.1f;
        private bool inited = false;
        #endregion

        private void Init()
        {
            if (!inited)
            {
                offset = transform.position - Target.transform.position;
                inited = true;
            }
        }
        private void LateUpdate()
        {
            if (Active)
            {
                Vector3 targetPos = transform.position;
                targetPos.y = (Target.transform.position + offset).y;
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.1f);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            SoundManager.Play("shot", Random.Range(1.25f, 1.5f), 1f);
        }
    }
}