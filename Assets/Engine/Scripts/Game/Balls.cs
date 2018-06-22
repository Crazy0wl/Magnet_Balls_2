using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MB_Engine
{
    public enum BallStyle
    {
        Normal,
        ColorFigured
    }

    public class Balls : Singleton<Balls>
    {
        protected Balls() { }

        #region fields
        public static bool Active;
        public int BallScore = 10;
        private int score;
        public static int Score
        {
            get { return main.score; }
            set
            {
                if (value != main.score)
                {
                    int oldScore = main.score;
                    main.score = value;
                    ScoreChanged(oldScore, main.score);
                }
            }
        }
        public static event Action<int, int> ScoreAdded = delegate { };
        public static event Action<int, int> ScoreChanged = delegate { };
        public static event Action<int, int> BestScoreChanged = delegate { };
        public static event Action<int, int> BallCountChanged = delegate { };
        public static BallColor[] Colors { get { return main.colors; } }
        private BallColor[] colors;
        private List<Ball> balls = new List<Ball>();
        public List<Ball> AllBalls { get { return balls; } }
        public static BallsGun Gun { get { return main.gun; } }
        public BallsGun gun;
        public int ComboNum = 1;
        public FastPool BallsPool;
        [Range(0f, 20f)]
        public float SimpleBallJointStretch = 15f;
        [Range(0f, 100f)]
        public float BombExplosionForce;
        [Range(0f, 100f)]
        public float SimpleExplosionForce;
        [Range(10f, 50f)]
        public float MinCrashBallSpeed = 10f;
        [Header("Prefabs")]
        public GameObject BallPrefab;
        public GameObject LightningPrefab;
        public GameObject[] BonusPrefabs;
        public GameObject[] EffectsPrefabs;
        public GameObject Coin1Prefab;
        public GameObject Coin2Prefab;
        public GameObject Coin5Prefab;
        public GameObject Coin10Prefab;
        public GameObject Coin20Prefab;
        public GameObject Coin50Prefab;
        public GameObject Coin100Prefab;

        private Dictionary<BallType, FastPool> ballTypesPool = new Dictionary<BallType, FastPool>();
        private Dictionary<string, FastPool> effectsPool = new Dictionary<string, FastPool>();
        private Dictionary<BallColor, FastPool> coinsPool = new Dictionary<BallColor, FastPool>();
        private FastPool lightningPool;
        [Header("Ball colors")]
        #region
        public Color TransparentColor;
        public Color WhiteColor;
        public Color BlackColor;
        public Color GreenColor;
        public Color AquaColor;
        public Color RedColor;
        public Color BlueColor;
        public Color YellowColor;
        #endregion

        [Header("Color figures")]
        #region
        public Sprite White;
        public Sprite Black;
        public Sprite Green;
        public Sprite Aqua;
        public Sprite Red;
        public Sprite Blue;
        public Sprite Yellow;
        #endregion

        [Header("Ball types")]
        #region
        public Sprite Simple;
        public Sprite Chameleon;
        public Sprite Freeze;
        public Sprite Brush;
        public Sprite Bomb;
        public Sprite Lightning;
        public Sprite Aim;
        public Sprite Anchored;
        public Sprite Anchor;
        public Sprite Frozen;
        public Sprite Crashed;
        public Sprite Rust;
        public Sprite Bubble;
        #endregion

        private Dictionary<BallType, Sprite> BallTypeSprites = new Dictionary<BallType, Sprite>();
        private Dictionary<BallColor, Sprite> ColorFigureSprites = new Dictionary<BallColor, Sprite>();
        public Dictionary<BallColor, Color32> BallColors = new Dictionary<BallColor, Color32>();

        private bool inited;
        public Level CurrLevelProfile { get; private set; }
        public bool AccelEnabled;

        private bool oddLine;
        private Stack<Ball> staticBalls = new Stack<Ball>();
        private int[,] tempColorsArray;
        public float TopPosY;
        private float lastRowY;
        public static bool WasGameOver;
        public static bool EndlessMode;
        public GameObject TopWall;
        private List<Ball> anchorBalls = new List<Ball>();
        private List<Level> tempLevels = new List<Level>();

        #endregion

        #region static methods

        public static void CreateNewLine()
        {
            main.createNewLine();
        }

        public static Ball GetRandomBall(BallColor ballColor)
        {
            return main.getRandomBall(ballColor);
        }

        public static void Activate(float delay)
        {
            main.activate(delay);
        }

        public static void EmitBonus(Ball ball, Vector3 force, float delay)
        {
            main.emitBonus(ball, force, delay);
        }

        public static Effect EmitEffect(string name, Vector2 pos, Vector2 vel)
        {
            return main.emitEffect(name, pos, vel);
        }

        public static Coin EmitCoin(Ball ball)
        {
            return main.emitCoin(ball);
        }

        public static Effect EmitEffect(string name, Vector3 pos, Color color, float delay)
        {
            return main.emitEffect(name, pos, color, delay);
        }

        public static bool Restart()
        {
            return main.restart();
        }

        public static void AddScore(int score)
        {
            if (Active)
            {
                main.addScore(score);
            }
        }

        public static void DestroyBall(Ball ball)
        {
            main.destroyBall(ball);
        }

        public static Sprite GetColorFigureSprite(BallColor color)
        {
            return main.getColorFigureSprite(color);
        }

        public static Color GetBallColor(BallColor ballColor)
        {
            return main.getBallColor(ballColor);
        }

        public static Sprite GetBallTypeSprite(BallType ballType)
        {
            return main.getBallTypeSprite(ballType);
        }

        public static void DestroyChain(Queue<Ball> chainForDestroy, BallColor ballColor)
        {
            main.destroyChain(chainForDestroy, ballColor);
        }

        public static Ball CreateGunBall(Vector2 pos, BallColor ballColor)
        {
            return main.createGunBall(pos, ballColor);
        }

        public static BallColor[] GetColorSet()
        {
            return main.getColorSet();
        }

        public static void AddBall(Ball ball)
        {
            main.addBall(ball);
        }

        public static void DestroyBalls()
        {
            main.destroyBalls();
        }

        public static Lightning EmitLightning()
        {
            return main.emitLightning();
        }

        public static void Shake()
        {
            main.shake();
        }

        public static void LoadLevel(Level level)
        {
            main.loadPuzzle(level);
        }

        #endregion

        #region private methods

        private void shake()
        {
            gameObject.ShakePosition(Vector3.one * 0.5f, 0.25f, 0.01f);
        }

        private Ball getRandomBall(BallColor ballColor)
        {
            Ball result = AllBalls.Find(b => b.BallColor == ballColor && !b.IsKinematic);
            return result;
        }

        private void activate(float delay)
        {
            StartCoroutine(ActivateRoutine(delay));
        }

        private void init()
        {
            GameManager.main.Background.ResetPos();
            initDictionary();
            gun.Init();
        }

        private Sprite getColorFigureSprite(BallColor color)
        {
            if (ColorFigureSprites.ContainsKey(color))
            {
                return ColorFigureSprites[color];
            }
            return null;
        }

        private void initDictionary()
        {
            if (!inited)
            {
                coinsPool[BallColor.White] = FastPoolManager.CreatePool(Coin1Prefab);
                coinsPool[BallColor.Black] = FastPoolManager.CreatePool(Coin2Prefab);
                coinsPool[BallColor.Red] = FastPoolManager.CreatePool(Coin5Prefab);
                coinsPool[BallColor.Green] = FastPoolManager.CreatePool(Coin10Prefab);
                coinsPool[BallColor.Blue] = FastPoolManager.CreatePool(Coin20Prefab);
                coinsPool[BallColor.Yellow] = FastPoolManager.CreatePool(Coin50Prefab);
                coinsPool[BallColor.Aqua] = FastPoolManager.CreatePool(Coin100Prefab);

                foreach (GameObject o in BonusPrefabs)
                {
                    BaseBonus e = o.GetComponent<BaseBonus>();
                    ballTypesPool[e.BonusType] = FastPoolManager.CreatePool(o);
                }

                foreach (GameObject o in EffectsPrefabs)
                {
                    effectsPool[o.name] = FastPoolManager.CreatePool(o);
                }
                lightningPool = FastPoolManager.CreatePool(LightningPrefab);

                ColorFigureSprites = new Dictionary<BallColor, Sprite>();
                ColorFigureSprites[BallColor.Black] = Black;
                ColorFigureSprites[BallColor.White] = White;
                ColorFigureSprites[BallColor.Green] = Green;
                ColorFigureSprites[BallColor.Aqua] = Aqua;
                ColorFigureSprites[BallColor.Red] = Red;
                ColorFigureSprites[BallColor.Blue] = Blue;
                ColorFigureSprites[BallColor.Yellow] = Yellow;

                BallTypeSprites = new Dictionary<BallType, Sprite>();
                BallTypeSprites[BallType.Simple] = Simple;
                BallTypeSprites[BallType.Chameleon] = Chameleon;
                BallTypeSprites[BallType.Freeze] = Freeze;
                BallTypeSprites[BallType.Brush] = Brush;
                BallTypeSprites[BallType.Bomb] = Bomb;
                BallTypeSprites[BallType.Lightning] = Lightning;
                BallTypeSprites[BallType.Anchored] = Anchored;
                BallTypeSprites[BallType.Anchor] = Anchor;
                BallTypeSprites[BallType.Aim] = Aim;

                BallTypeSprites[BallType.Bubble] = Bubble;
                BallTypeSprites[BallType.Frozen] = Frozen;
                BallTypeSprites[BallType.Crashed] = Crashed;
                BallTypeSprites[BallType.Rust] = Rust;

                BallColors = new Dictionary<BallColor, Color32>();
                BallColors[BallColor.White] = WhiteColor;
                BallColors[BallColor.Black] = BlackColor;
                BallColors[BallColor.Green] = GreenColor;
                BallColors[BallColor.Aqua] = AquaColor;
                BallColors[BallColor.Red] = RedColor;
                BallColors[BallColor.Blue] = BlueColor;
                BallColors[BallColor.Yellow] = YellowColor;
                BallColors[BallColor.None] = Color.white;

                BallsPool = FastPoolManager.GetPool(BallPrefab);

                colors = new BallColor[8];
                colors[0] = BallColor.White;
                colors[1] = BallColor.Black;
                colors[2] = BallColor.Green;
                colors[3] = BallColor.Aqua;
                colors[4] = BallColor.Red;
                colors[5] = BallColor.Blue;
                colors[6] = BallColor.Yellow;
                inited = true;
            }
        }

        private BallColor getNoRepeatedColor(int column, int row)
        {
            int colorsCount = colors.Length;
            int maxX = Mathf.Max(0, column - 1);
            int maxY = Mathf.Max(0, row - 1);
            int minX = Mathf.Min(column + 1, Level.MAX_COLS - 1);
            int minY = Mathf.Min(row + 1, Level.MAX_ROWS - 1);
            HashSet<int> exclude = new HashSet<int>()
        {
            tempColorsArray[maxX, row],
            tempColorsArray[maxX, maxY],
            tempColorsArray[column, maxY],
            tempColorsArray[minX, row],
            tempColorsArray[column, minY],
            tempColorsArray[maxX, minY]
        };
            IEnumerable<int> range = Enumerable.Range(1, colorsCount).Where(i => !exclude.Contains(i));
            if (range.Count() == 0)
            {
                return BallColor.White;
            }
            tempColorsArray[column, row] = range.ElementAt(UnityEngine.Random.Range(0, colorsCount - exclude.Count));
            return (BallColor)tempColorsArray[column, row];
        }

        private BaseBonus InstantiateBonus(Ball ball, Vector3 force, float delay)
        {
            if (inited)
            {
                FastPool p = null;
                if (ballTypesPool.TryGetValue(ball.BallType, out p))
                {
                    BaseBonus result = p.FastInstantiate<BaseBonus>(ball.Pos, Quaternion.identity, transform);
                    result.Emit(ball, p, force, delay);
                    return result;
                }
            }
            return null;
        }

        private Effect InstantiateEffect(string name, Vector2 pos, Vector2 vel)
        {
            Effect result = GetEffect(name, pos);
            if (result)
            {
                result.Emit(vel);
            }
            return result;
        }

        private Effect InstantiateEffect(string name, Vector3 pos, Color color)
        {
            Effect result = GetEffect(name, pos);
            if (result)
            {
                result.Emit(color);
            }
            return result;
        }

        private Effect InstantiateEffect(string name, Vector3 pos, Color color, float delay)
        {
            Effect result = GetEffect(name, pos);
            if (result)
            {
                result.Emit(color, delay);
            }
            return result;
        }

        private Coin InstantiateCoinEffect(Ball ball)
        {
            Coin result = GetCoinEffect(ball.BallColor, ball.Pos);
            if (result)
            {
                Vector3 vel = new Vector3(0f, Mathf.Clamp(-ball.Vel.y * 3f, 5f, 25f), ball.Vel.magnitude);// cl Max(3f, -ball.Vel.y * 2f), ball.Vel.magnitude);
                result.Emit(vel);
            }
            return result;
        }

        private Effect GetEffect(string name, Vector2 pos)
        {
            if (inited)
            {
                FastPool p = null;
                if (effectsPool.TryGetValue(name, out p))
                {
                    Effect result = p.FastInstantiate<Effect>(pos, Quaternion.identity, transform);
                    result.Init(p);
                    return result;
                }
            }
            return null;
        }

        private Coin GetCoinEffect(BallColor col, Vector2 pos)
        {
            if (inited)
            {
                FastPool p = null;
                if (coinsPool.TryGetValue(col, out p))
                {
                    Coin result = p.FastInstantiate<Coin>(pos, Quaternion.identity, transform);
                    result.Init(p);
                    return result;
                }
            }
            return null;
        }

        private void emitBonus(Ball ball, Vector3 force, float delay)
        {
            InstantiateBonus(ball, force, delay);
        }

        private Lightning emitLightning()
        {
            Lightning result = lightningPool.FastInstantiate<Lightning>(transform);
            result.Init(lightningPool);
            return result;
        }

        private Effect emitEffect(string name, Vector2 pos, Vector2 vel)
        {
            return InstantiateEffect(name, pos, vel);
        }

        private Coin emitCoin(Ball ball)
        {
            return InstantiateCoinEffect(ball);
        }

        private Effect emitEffect(string name, Vector3 pos, Color color, float delay)
        {
            return InstantiateEffect(name, pos, color, delay);
        }

        private bool loadEndless()
        {
            Active = true;
            init();
            StopAllCoroutines();
            if (WasGameOver)
            {
                AdManager.ShowInterstitial();
                WasGameOver = false;
            }
            StartCoroutine(ResetBallsRoutine());
            UIManager.Gameplay.ResetScore();
            return true;
        }

        private bool loadPuzzle(Level levelProfile)
        {
            Active = true;
            init();
            StopAllCoroutines();
            if (WasGameOver)
            {
                AdManager.ShowInterstitial();
                WasGameOver = false;
            }
            StartCoroutine(ResetBallsRoutine(levelProfile));
            UIManager.Gameplay.ResetScore();
            return true;
        }

        private bool restart()
        {
            if (EndlessMode)
            {
                return loadEndless();
            }
            else
            {
                return loadPuzzle(CurrLevelProfile);
            }
        }

        private Sprite getBallTypeSprite(BallType ballType)
        {
            return main.BallTypeSprites[ballType];
        }

        private Color32 getBallColor(BallColor c)
        {
            if (c != BallColor.None && BallColors.ContainsKey(c))
            {
                return BallColors[c];
            }
            return Color.white;
        }

        private void addScore(int scoreToAdd, int combo = 1)
        {
            if (scoreToAdd != 0)
            {
                Score += scoreToAdd * combo;
                if (ScoreAdded != null)
                {
                    ScoreAdded(scoreToAdd, combo);
                }
            }
        }

        private void destroyBall(Ball ball)
        {
            if (ball.BallType == BallType.Anchor)
            {
                if (anchorBalls.Contains(ball))
                {
                    anchorBalls.Remove(ball);
                }
            }
            else if (balls.Contains(ball))
            {
                int oldValue = balls.Count;
                balls.Remove(ball);
                if (BallCountChanged != null)
                {
                    int newValue = balls.Count;
                    BallCountChanged(oldValue, newValue);
                }
                BallsPool.FastDestroy(ball.gameObject);
                if (!EndlessMode && balls.Count == 0)
                {
                    completeLevel();
                }
            }
        }

        private void completeLevel()
        {
            StartCoroutine(CompleteLevelRoutine());
        }

        private Ball CreateBall(Vector2 pos, BallType ballType, BallColor ballColor)
        {
            GameObject ballObject = BallsPool.FastInstantiate(pos, Quaternion.identity, transform);
            Ball ball = ballObject.GetComponent<Ball>();
            ball.Init(false, ballType, GameData.BallStyle, ballColor, true);
            return ball;
        }

        private Ball createGunBall(Vector2 pos, BallColor ballColor)
        {
            GameObject ballObject = BallsPool.FastInstantiate(pos, Quaternion.identity, gun.transform);
            ballObject.transform.localScale = Vector3.one;
            Ball ball = ballObject.GetComponent<Ball>();
            ball.Init(true, BallType.Simple, GameData.BallStyle, ballColor);
            return ball;
        }

        private void destroyChain(Queue<Ball> chainForDestroy, BallColor ballColor)
        {
            int chainLength = chainForDestroy.Count;
            while (chainForDestroy.Count > 0)
            {
                Ball b = chainForDestroy.Dequeue();
                b.Kill(0, Vector3.zero, chainForDestroy.Count * 0.05f);
            }
            addScore(chainLength * BallScore, ComboNum);
            if (chainLength > 3)
            {
                gun.SetGunBallType(ballColor);
            }
            ComboNum++;
        }

        private Vector2 GetBallPos(int col, int row)
        {
            float dY = Mathf.Sqrt(3f) / 2f;
            float offset = (row % 2 == 0) ? 0f : 0.5f;
            float x = col + offset - (CurrLevelProfile.Cols - 1) / 2f;
            float y = -row * dY - 1f + GameManager.main.CamController.transform.position.y;
            return new Vector2(x, y);
        }

        private void UpdateGravity()
        {
            Physics2D.gravity = new Vector2(Input.acceleration.x * 10f, Input.acceleration.y * 10f);
            Physics.gravity = new Vector3(Input.acceleration.x * 10f, Input.acceleration.y * 10f, Input.acceleration.z);
        }

        private BallColor[] getColorSet()
        {
            List<BallColor> colors = new List<BallColor>();
            foreach (Ball b in balls)
            {
                if (b.BallColor != BallColor.None && !colors.Contains(b.BallColor))
                {
                    colors.Add(b.BallColor);
                }
            }
            if (colors.Count == 0)
            {
                colors.Add(BallColor.White);
            }
            return colors.ToArray();
        }

        private void addBall(Ball ball)
        {
            balls.Add(ball);
        }

        private void destroyBalls()
        {
            while (staticBalls.Count > 0)
            {
                Destroy(staticBalls.Pop());
            }
            foreach (Ball b in anchorBalls)
            {
                Destroy(b.gameObject);
            }
            anchorBalls.Clear();
            foreach (Ball b in balls)
            {
                Destroy(b.gameObject);
            }
            balls = new List<Ball>();
            gun.DestroyBalls();
            ComboNum = 1;
            Score = 0;
        }

        private void createBalls(Level level)
        {
            TopWall.SetActive(true);
            foreach (Ball b in anchorBalls)
            {
                Destroy(b.gameObject);
            }
            anchorBalls.Clear();
            tempColorsArray = new int[Level.MAX_COLS, Level.MAX_ROWS];
            CurrLevelProfile = level;
            Time.timeScale = 0f;
            lastRowY = TopPosY;
            for (int row = 0; row < level.Rows; row++)
            {
                for (int col = 0; col < level.Cols; col++)
                {
                    BallType ballType = level.GetBall(col, row);
                    if (ballType != BallType.None)
                    {
                        BallColor ballColor = level.GetColor(col, row);
                        if (level.FixColors)
                        {
                            ballColor = (ballType == BallType.Simple || ballType == BallType.Anchored) ? getNoRepeatedColor(col, row) : BallColor.None;
                        }
                        Vector2 pos = GetBallPos(col, row);
                        pos.y += TopPosY;
                        Ball b = CreateBall(pos, ballType, ballColor);
                        if (ballType != BallType.Anchor)
                        {
                            balls.Add(b);
                        }
                        else
                        {
                            anchorBalls.Add(b);
                        }
                    }
                }
                oddLine = !oddLine;
            }
            Time.timeScale = 1f;
        }

        private void createBalls(int cols, int rows)
        {
            TopWall.SetActive(false);
            GameManager.main.Background.SetRandomColor();
            tempColorsArray = new int[Level.MAX_COLS, Level.MAX_ROWS];
            Time.timeScale = 0f;
            lastRowY = TopPosY;
            float dY = Mathf.Sqrt(3f) / 2f;
            for (int row = 0; row < rows; row++)
            {
                float xOffset = oddLine ? -0.5f : 0f;
                for (int col = 0; col < cols + (oddLine ? 1 : 0); col++)
                {
                    float x = col - (cols - 1) / 2f + xOffset;
                    float y = TopPosY - row * dY;
                    Vector2 pos = new Vector2(x, y);
                    BallType ballType = GetRandomBallType();
                    BallColor ballColor = BallColor.None;
                    if (ballType == BallType.Simple || ballType == BallType.Anchored)
                    {
                        ballColor = getNoRepeatedColor(col, row);
                    }
                         
                    Ball b = CreateBall(pos, ballType, ballColor);
                    if (row == 0)
                    {
                        b.IsKinematic = true;
                        staticBalls.Push(b);
                    }
                    balls.Add(b);
                }
                oddLine = !oddLine;
            }
            Time.timeScale = 1f;
        }

        private BallType GetRandomBallType()
        {
            if (UnityEngine.Random.Range(0f, 1f) > 0.9f)
            {
                int index = UnityEngine.Random.Range(0, 3);
                switch (index)
                {
                    case 0:
                        return BallType.Bubble;
                    case 1:
                        return BallType.Frozen;
                    case 2:
                        return BallType.Rust;
                }
            }
            return BallType.Simple;
        }

        private void createNewLine()
        {
            if (EndlessMode)
            {
                float xOffset = oddLine ? -0.5f : 0f;
                while (staticBalls.Count > 0)
                {
                    Ball b = staticBalls.Pop();
                    b.IsKinematic = false;
                }
                int cols = Level.MAX_COLS - 1;
                lastRowY += Mathf.Sqrt(3f) / 2f;
                List<BallColor> rowColors = new List<BallColor>();
                for (int i = 0; i < cols + (oddLine ? 1 : 0); i++)
                {
                    float x = i - (cols - 1) / 2f + xOffset;
                    Vector2 pos = new Vector2(x, lastRowY);
                    BallColor ballColor = BallColor.None;
                    rowColors.Add(ballColor);
                    BallType ballType = GetRandomBallType();
                    Ball b = CreateBall(pos, ballType, ballColor);
                    if (ballType != BallType.Anchored)
                    {
                        b.IsKinematic = true;
                        b.JoinNearBalls(0.6f);
                        if (b.Colored)
                        {
                            b.CorrectColor();
                        }
                        staticBalls.Push(b);
                    }
                    balls.Add(b);
                }
                oddLine = !oddLine;
            }
        }

        private void LateUpdate()
        {
            if (GameData.AccelActive)
            {
#if !UNITY_EDITOR
            UpdateGravity();
#endif
            }
        }

        #endregion

        #region routines

        private IEnumerator ActivateRoutine(float delay)
        {
            Time.timeScale = 1f;
            yield return new WaitForSeconds(delay);
            Active = true;
        }

        private IEnumerator CompleteLevelRoutine()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            UIManager.LevelCompleted.Show();
        }

        private IEnumerator ResetBallsRoutine()
        {
            Active = false;
            GameManager.main.CamController.Active = false;
            Time.timeScale = 0f;
            destroyBalls();
            Score = 0;
            Time.timeScale = 1f;
            GameManager.main.CamController.Active = true;
            createBalls(Level.MAX_COLS - 1, 7);
            gun.ResetGun();
            yield return new WaitForSeconds(0.5f);
            Active = true;
        }

        private IEnumerator ResetBallsRoutine(Level level)
        {
            init();
            Active = false;
            GameManager.main.CamController.Active = false;
            Time.timeScale = 0f;
            destroyBalls();
            yield return null;
            Score = 0;
            GameManager.main.CamController.Active = true;
            createBalls(level);
            gun.ResetGun(level);
            yield return null;
            Time.timeScale = 1f;
            Active = true;
        }
        
        #endregion
    }
}