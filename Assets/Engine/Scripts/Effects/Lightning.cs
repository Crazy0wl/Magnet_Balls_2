using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class Lightning : MonoBehaviour
    {
        #region fields
        public float Amplitude = 0.25f;
        public float FadeInSpeed = 0.1f;
        public float FadeOutSpeed = 0.1f;
        public float StartWidth = 1f;
        public float EndWidth = 1f;
        public ParticleSystem EndPS;
        private LineRenderer line;
        private FastPool pool;
        #endregion

        public void Init(FastPool pool)
        {
            this.pool = pool;
            if (!line)
            {
                line = GetComponent<LineRenderer>();
            }
        }

        public void Emit(Color color, Ball[] path)
        {
            if (!line)
            {
                line = GetComponent<LineRenderer>();
            }
            line.startColor = color;
            line.endColor = color;
            StartCoroutine(EmitRoutine(path));
        }

        public void Emit(Ball ballForDestroy, Ball[] path)
        {
            if (!line)
            {
                line = GetComponent<LineRenderer>();
            }
            line.startColor = ballForDestroy.RealColor;
            line.endColor = ballForDestroy.RealColor;
            StartCoroutine(EmitRoutine(ballForDestroy, path));
        }

        private void GenerateFromPath(Ball[] path)
        {
            Vector3[] points = new Vector3[path.Length];
            line.positionCount = points.Length;
            Vector3 pos3D = new Vector3(path[0].Pos.x, path[0].Pos.y, -0.5f);
            line.SetPosition(0, pos3D);
            for (int i = 1; i < path.Length - 1; i++)
            {
                pos3D = new Vector3(path[i].Pos.x, path[i].Pos.y, -0.5f) + Random.insideUnitSphere * Amplitude;
                line.SetPosition(i, pos3D);
            }
            pos3D = new Vector3(path[path.Length - 1].Pos.x, path[path.Length - 1].Pos.y, -0.5f);
            line.SetPosition(path.Length - 1, pos3D);
        }

        public IEnumerator EmitRoutine(Ball ballForDestroy, Ball[] path)
        {
            GenerateFromPath(path);
            yield return StartCoroutine(FadeInRoutine(ballForDestroy));
            yield return StartCoroutine(FadeOutRoutine());
        }

        public IEnumerator EmitRoutine(Ball[] path)
        {
            GenerateFromPath(path);
            yield return StartCoroutine(FadeInRoutine());
            yield return StartCoroutine(FadeOutRoutine());
        }

        private IEnumerator FadeInRoutine(Ball ballForDestroy)
        {
            line.startWidth = StartWidth;
            yield return new WaitForEndOfFrame();
            line.endWidth = EndWidth;
            EndPS.transform.position = ballForDestroy.transform.position;
            EndPS.gameObject.SetActive(true);
            ballForDestroy.Kill(Balls.main.BallScore, Vector3.zero, 0.1f);
        }

        private IEnumerator FadeInRoutine()
        {
            line.startWidth = StartWidth;
            yield return new WaitForEndOfFrame();
            line.endWidth = EndWidth;
        }

        private IEnumerator FadeOutRoutine()
        {
            for (float f = 1f; f >= 0f; f -= FadeOutSpeed * Time.deltaTime)
            {
                line.startWidth = f * StartWidth;
                yield return null;
                line.endWidth = f * EndWidth;
            }
            line.endWidth = line.startWidth = 0f;
            pool.FastDestroy(gameObject);
        }
    }
}