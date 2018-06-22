using UnityEngine;

namespace MB_Engine
{
    public class Portal : MonoBehaviour
    {
        public GameObject Out;
        public string Tag;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //if (collision.tag == Tag)
            //{
            //    Ball ballForDestroy = collision.GetComponent<Ball>();
            //    if (ballForDestroy)
            //    {
            //        ballForDestroy.ClearJoints();
            //        Vector3 pos = ballForDestroy.Pos;
            //        Vector3 outPos = Out.transform.position;
            //        outPos.z = -1f;
            //        ballForDestroy.transform.position = outPos;
            //    }
            //}
        }
    }
}