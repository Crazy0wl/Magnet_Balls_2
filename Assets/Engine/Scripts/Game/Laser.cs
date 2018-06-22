using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public class Laser : MonoBehaviour
    {
        #region fields
        public float Distance = 10f;
        public float StartPosOffset = 0f;
        public int MaxCount = 5;
        public string CollideTag;
        public string ReflectedTag;
        public GameObject StartPoint;
        public GameObject EndPoint;
        public LineRenderer LaserRenderer;
        public Color LaserColor
        {
            get { return LaserRenderer.startColor; }
            set { LaserRenderer.startColor = LaserRenderer.endColor = value; }
        }
        private bool active;
        public bool Active
        {
            get { return active; }
            set
            {
                active = value;
                gameObject.SetActive(value);
                LaserRenderer.enabled = value;
                StartPoint.SetActive(value);
                EndPoint.SetActive(value);
            }
        }
        private Vector3 start;
        private Vector2 laserDir = Vector2.up;
        RaycastHit2D[] hits = new RaycastHit2D[1];
        #endregion

        private void Start()
        {
            start = GameManager.main.CamController.Cam.WorldToScreenPoint(StartPoint.transform.position);
        }

        private Vector3[] GetPoints(Vector2 dir)
        {
            List<Vector3> result = new List<Vector3>();
            Vector2 start = (Vector2)transform.position + dir.normalized * 0.6f;
            result.Add(new Vector3(start.x, start.y, -0.5f));
            for (int i = 0; i < MaxCount; i++)
            {
                Physics2D.RaycastNonAlloc(start + dir, dir, hits);
                if (hits.Length > 0)
                {
                    RaycastHit2D hit = hits[0];
                    if (hit.collider)
                    {
                        Vector3 startPos = new Vector3(start.x, start.y, -0.5f);
                        Vector3 endPos = new Vector3(hit.point.x, hit.point.y, -0.5f);
                        Debug.DrawLine(startPos, endPos, Color.red, 0.1f, false);
                        Vector3 pos = endPos;
                        result.Add(pos);
                        if (hit.collider.tag != ReflectedTag)
                        {
                            return result.ToArray();
                        }
                        dir = Vector3.Reflect((new Vector2(pos.x, pos.y) - start).normalized, hit.normal);
                        start = hit.point;
                    }
                }
            }
            return result.ToArray();
        }
        
        private void ProcessLaserInput()
        {
            laserDir = (Input.mousePosition - start).normalized;
            Vector3[] points = GetPoints(laserDir);
            LaserRenderer.positionCount = points.Length;
            LaserRenderer.SetPositions(points);
        }

        private void Update()
        {
            ProcessLaserInput();
        }
    }
}