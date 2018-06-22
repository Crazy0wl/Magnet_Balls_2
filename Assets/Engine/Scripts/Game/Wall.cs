using UnityEngine;

namespace MB_Engine
{
    public class Wall : MonoBehaviour
    {
        public string CollisionTag;
        public string SoundName = "wallCollision";
        public float MinVel = 10f;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag == CollisionTag)
            {
                float vel = collision.rigidbody.velocity.sqrMagnitude;
                if (vel > MinVel)
                {
                    float volume = Mathf.Clamp(vel / 100f, 0f, 1f);
                    float pitch = Random.Range(1.5f, 2f);
                    SoundManager.Play(SoundName, pitch, volume);
                    Balls.EmitEffect("BallCollision", collision.contacts[0].point, collision.rigidbody.velocity / 2f);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //if (collision.transform.tag == CollisionTag)
            //{
            //    float vel = collision.rigidbody.velocity.sqrMagnitude;
            //    if (vel > MinVel)
            //    {
            //        float volume = Mathf.Clamp(vel / 100f, 0f, 1f);
            //        float pitch = Random.Range(1.25f, 1.5f);
            //        SoundManager.Play(SoundName);
            //        //SoundManager.Play("wallCollision")
            //    }
            //    //if (collision.rigidbody.velocity.sqrMagnitude > )
            //    Balls.EmitEffect("BallCollision", collision.contacts[0].point, collision.rigidbody.velocity / 2f);
            //    //float volume = Mathf.Clamp()
            //}
        }
    }
}