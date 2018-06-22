using System.Collections;
using TMPro;
using UnityEngine;

namespace MB_Engine
{
    public class GameTitle : MonoBehaviour
    {
        public TextMeshPro TextMesh;

        public string Text
        {
            get { return TextMesh.text; }
            set { TextMesh.text = value; }
        }

        public float FontSize
        {
            get { return TextMesh.fontSize; }
            set { TextMesh.fontSize = value; }
        }

        public void Show(float duration)
        {
            StartCoroutine(ChangeAlphaRoutine(0f, 1f, duration));
        }

        public void Hide(float duration)
        {
            StartCoroutine(ChangeAlphaRoutine(1f, 0f, duration));
        }

        private IEnumerator ChangeAlphaRoutine(float fromAlpha, float toAlpha, float duration)
        {
            if (duration > 0f)
            {
                TextMesh.alpha = fromAlpha;
                for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
                {
                    TextMesh.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                    yield return null;
                }
            }
            TextMesh.alpha = toAlpha;
        }
    }
}