using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class CrashedBall : BaseBonus
    {
        public MeshRenderer[] renderers;
        public Rigidbody[] bodies;

        public override BallType BonusType { get { return BallType.Crashed; } }

        public override void Emit(Ball ball, FastPool pool, Vector2 force, float delay)
        {
            base.Emit(ball, pool, force, delay);
            StartCoroutine(EmitRoutine(delay, force));
        }

        public IEnumerator EmitRoutine(float delay, Vector2 force)
        {
            int count = Random.Range(3, bodies.Length);
            for (int i = 0; i < count; i++)
            {
                Rigidbody body = bodies[i];
                body.transform.position = transform.position + Random.insideUnitSphere * 0.5f;
                body.transform.rotation = Random.rotation;
                body.velocity = Vector3.zero;
                body.maxAngularVelocity = 32f;
            }
            yield return new WaitForSeconds(delay);
            Vector2 vel = ball.Vel / 2f;
            ball.Destroy();
            Vector3 force3D = new Vector3(force.x, force.y, -force.magnitude / 2f);
            for (int i = 0; i < count; i++)
            {
                Rigidbody body = bodies[i];
                body.velocity = vel;
                renderers[i].material.color = ball.RealColor;
                body.gameObject.SetActive(true);
                body.AddRelativeTorque(0f, 0f, Random.Range(0f, 100f));
                body.AddForce(force3D * Random.Range(0.5f, 1f), ForceMode.Impulse);
            }
        }
    }
}