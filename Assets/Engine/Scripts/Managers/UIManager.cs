using System.Collections;
using UnityEngine;

namespace MB_Engine
{
    public class UIManager : Singleton<UIManager>
    {
        protected UIManager() { }
        [Header("Panels")]
        public MainMenu mainMenu;
        public InfoMenu infoMenu;
        public FortuneWheelMenu fortuneWheelMenu;
        public GameplayMenu gameplayMenu;
        public DailyBonus dailyBonusPanel;
        public PauseMenu pauseMenu;
        public GameOverMenu gameOverMenu;
        public LevelCompletedMenu levelComopletedMenu;
        public LevelListMenu levelList;
        public MessageText message;
        public SettingsMenu settings;
        private Camera uiCamera;
        

        public static MainMenu Main { get { return main.mainMenu; } }
        public static InfoMenu Info { get { return main.infoMenu; } }
        public static FortuneWheelMenu FortuneWheel { get { return main.fortuneWheelMenu; } }
        public static GameplayMenu Gameplay { get { return main.gameplayMenu; } }
        public static DailyBonus DailyBonus { get { return main.dailyBonusPanel; } }
        public static PauseMenu Pause { get { return main.pauseMenu; } }
        public static GameOverMenu GameOver { get { return main.gameOverMenu; } }
        public static LevelCompletedMenu LevelCompleted { get { return main.levelComopletedMenu; } }
        public static LevelListMenu LevelList { get { return main.levelList; } } 
        public static SettingsMenu Settings { get { return main.settings; } }
        public static Camera UICamera { get { return main.uiCamera; } }

        private void Start()
        {
            uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
        }

        public static void ShowMessage(string msg)
        {
            main.message.Show(msg);
        }

        public static void ClearMessage()
        {
            main.message.Clear();
        }

        public static Vector3 ViewportToWorldPoint(Vector3 pos)
        {
            //main.uiCamera.WorldToViewportPoint
            return main.uiCamera.ViewportToWorldPoint(pos);
        }
        public static Vector3 WorldToViewportPoint(Vector3 pos)
        {
            return main.uiCamera.WorldToViewportPoint(pos);
        }

        public static void Move(Transform transform, Vector3 fromPos, Vector3 toPos, float delay)
        {
            main.move(transform, fromPos, toPos, delay);
        }

        public static void ScaleTo(Transform transform, float fromScale, float toScale, float delay)
        {
            main.scaleTo(transform, fromScale, toScale, delay);
        }

        private void move(Transform transform, Vector3 fromPos, Vector3 toPos, float delay)
        {
            StartCoroutine(MoveRoutine(transform, fromPos, toPos, delay));
        }

        private void scaleTo(Transform transform, float from, float to, float delay)
        {
            StartCoroutine(ScaleToRoutine(transform, from, to, delay));
        }

        private IEnumerator MoveRoutine(Transform transform, Vector3 fromPos, Vector3 toPos, float delay)
        {
            yield return new WaitForSeconds(delay);
            for (float f = 0f; f <= 1f; f += Time.deltaTime * 8f)
            {
                yield return null;
                transform.localPosition = Vector3.LerpUnclamped(fromPos, toPos, f);
            }
            yield return null;
            //fromPos = transform.localPosition;
            //for (float f = 0f; f <= 1f; f += Time.deltaTime * 8f)
            //{
            //    yield return null;
            //    transform.localPosition = Vector3.LerpUnclamped(fromPos, toPos, f);
            //}
            transform.localPosition = Vector3.LerpUnclamped(fromPos, toPos, 1f);
        }

        private IEnumerator ScaleToRoutine(Transform transform, float fromScale, float toScale, float delay)
        {
            yield return new WaitForSeconds(delay);
            float bigScale = toScale * 1.5f;
            for (float f = 0f; f <= 1f; f += Time.deltaTime * 10f)
            {
                yield return null;
                transform.localScale = Vector3.one * Mathf.Lerp(0f, bigScale, f);
            }
            for (float f = 0f; f <= 1f; f += Time.deltaTime * 10f)
            {
                yield return null;
                transform.localScale = Vector3.one * Mathf.Lerp(bigScale, toScale, f);
            }
        }

        public static void MoveSpring(Transform transform, Vector3 fromPos, Vector3 toPos, float delay)
        {
            main.moveSpring(transform, fromPos, toPos, delay);
        }

        private void moveSpring(Transform transform, Vector3 fromPos, Vector3 toPos, float delay)
        {
            StartCoroutine(MoveSpringRoutine(transform, fromPos, toPos, delay));
        }

        private IEnumerator MoveSpringRoutine(Transform transform, Vector3 fromPos, Vector3 toPos, float delay)
        {
            transform.localPosition = fromPos;
            yield return new WaitForSeconds(delay);
            float amplitude = 1.2f;
            while (Mathf.Abs(amplitude) > 1f)
            {
                for (float f = 0f; f <= 1f; f += Time.deltaTime * 4f)
                {
                    yield return null;
                    transform.localPosition = Vector3.LerpUnclamped(fromPos, toPos, f * amplitude);
                }
                yield return null;
                amplitude *= -0.95f;
                fromPos = transform.localPosition;
            }
            transform.localPosition = toPos;
        }
    }
}