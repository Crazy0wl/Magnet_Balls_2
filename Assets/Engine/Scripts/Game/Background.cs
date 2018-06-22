using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class Background : MonoBehaviour
    {
        #region fields
        private float tileSize;
        private Vector3 startPosition;
        public SpriteRenderer Renderer;
        public Color Color
        {
            get { return Renderer.color; }
            set { Renderer.color = value; }
        }
        public Color[] BackColors;
        public Color DefaultColor;
        public Camera GameCamera;

        #endregion

        void Start()
        {
            startPosition = transform.position;
            tileSize = Renderer.size.y / 2f;
            Color = DefaultColor;
        }

        public void SetRandomColor()
        {
            Color destColor = BackColors[Random.Range(0, BackColors.Length)];
            StartCoroutine(ChangeColorRoutine(destColor));
        }

        public void ResetPos()
        {
            Vector3 pos = transform.position;
            pos.y = tileSize / 2f;
            transform.position = pos;
        }

        public void MoveUp()
        {
            transform.position = startPosition - Vector3.down * tileSize;
            startPosition = transform.position;
        }

        public void MoveDown()
        {
            transform.position = startPosition + Vector3.down * tileSize;
            startPosition = transform.position;
            
        }

        private void LateUpdate()
        {
            if (GameManager.main.CamController.transform.position.y - transform.position.y > tileSize / 2f)
            {
                MoveUp();
            }
        }

        private IEnumerator ChangeColorRoutine(Color toColor)
        {
            Color fromColor = Color;
            for (float f = 0f; f <= 1f; f += 0.01f)
            {
                Color = Color.Lerp(fromColor, toColor, f);
                yield return null;
            }
            GameCamera.backgroundColor = toColor * 0.25f;
            Color = toColor;
        }
    }
}