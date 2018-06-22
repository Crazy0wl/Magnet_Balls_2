using System;
using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    [RequireComponent(typeof(UISprite))]
    public class Menu : MonoBehaviour
    {
        #region fields
        public float ShowDuration = 0.5f;
        public float HideDuration = 0.5f;
    
        public event Action BackPressed = delegate { };
        public event Action ShowCompleted = delegate { };
        public event Action ShowStarted = delegate { };
        public event Action HideStarted = delegate { };
        public event Action HideCompleted = delegate { };
        protected bool InProcess;
        public float Alpha { get; private set; }
        public static bool Processing;
        public bool Active { get { return gameObject.activeSelf; } }

        #endregion

        public virtual bool Show(float delay = 0f)
        {
            if (!InProcess)
            {
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    UISprite uiSprite = GetComponent<UISprite>();
                    Alpha = uiSprite.alpha = 0f;
                    StartCoroutine(ShowRoutine(ShowDuration, delay));
                    return true;
                }
            }
            return false;
        }

        public virtual bool Hide()
        {
            if (!InProcess)
            {
                if (gameObject.activeSelf)
                {
                    StartCoroutine(HideRoutine(HideDuration));
                    return true;
                }
            }
            return false;
        }

        private IEnumerator ChangeAlphaRoutine(float fromAlpha, float toAlpha, float duration)
        {
            UISprite uiSprite = GetComponent<UISprite>();
            bool hide = toAlpha == 0f;
            if (duration > 0f)
            {
                Alpha = uiSprite.alpha = fromAlpha;
                for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
                {
                    Alpha = uiSprite.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
                    yield return null;
                }
            }
            Alpha = uiSprite.alpha = toAlpha;
            InProcess = false;
            if (hide)
            {
                if (HideCompleted != null)
                {
                    HideCompleted();
                }
                gameObject.SetActive(false);
            }
            else
            {
                if (ShowCompleted != null)
                {
                    ShowCompleted();
                }
            }
        }

        private IEnumerator ShowRoutine(float duration, float delay)
        {
            InProcess = true;

            yield return new WaitForSeconds(delay);
            if (ShowStarted != null)
            {
                ShowStarted();
            }
            yield return StartCoroutine(ChangeAlphaRoutine(0f, 1f, duration));
        }

        private IEnumerator HideRoutine(float duration)
        {
            InProcess = true;
            if (HideStarted != null)
            {
                HideStarted();
            }
            yield return StartCoroutine(ChangeAlphaRoutine(1f, 0f, duration));
        }

        private void LateUpdate()
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (BackPressed != null)
                {
                    BackPressed();
                }
                OnBackPressed();
            }
        }

        protected virtual void OnBackPressed()
        {
        }
    }
}