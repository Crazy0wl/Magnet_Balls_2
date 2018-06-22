using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MB_Engine
{
    public interface IBall
    {
        BallType BallType { get; set; }
        BallColor BallColor { get; set; }
        bool IsKinematic { get; set; }
        bool Destroying { get; set; }
        bool HasGravity { get; set; }
        Vector2 Pos { get; }
        Vector2 Vel { get; }
        Color RealColor {get; set;}
    }
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Ball : MonoBehaviour, IBall
    {
        #region fields
        public BallType BallType
        {
            get { return ballType; }
            set { SetBallType(value); }
        }
        public BallColor BallColor
        {
            get { return ballColor; }
            set { SetBallColor(value); }
        }
        public Rigidbody2D BallBody2D;
        public bool IsKinematic
        {
            get { return BallBody2D.isKinematic; }
            set
            {
                if (!value && isActiveAndEnabled && BallColor != BallColor.None)
                {
                    StartCoroutine(CheckChainRoutine(0.0f));
                }
                BallBody2D.isKinematic = value;
            }
        }
        public SpriteRenderer ballRenderer;
        public SpriteRenderer bonusRenderer;
        public Vector2 Pos { get { return BallBody2D.position; } }
        public Vector2 Vel { get { return BallBody2D.velocity; } }
        public bool WasJoin { get; set; }
        public Color RealColor
        {
            get { return ballRenderer.color; }
            set { ballRenderer.color = value; }
        }
        public bool Destroying { get; set; }
        public bool Colored { get; private set; }
        public bool HasGravity
        {
            get { return BallBody2D.gravityScale != 0f; }
            set { BallBody2D.gravityScale = value ? 1f : 0f; }
        }
        public bool Targeted;

        private BallColor ballColor;
        private CircleCollider2D ballCollider;
        private Dictionary<Ball, Joint2D> joints = new Dictionary<Ball, Joint2D>(6);
        private Ball baseBall;
        private Stack<Ball> path = new Stack<Ball>();
        private Queue<Ball> ballsInChain = new Queue<Ball>();
        private BallStyle style;
        private bool gunBall;
        private BallType ballType;
        private int waveNum;
        private bool reactAfterJoint;
        private bool killAfterJoint;
        #endregion


        #region public

        public void Init(bool gunBall, BallType ballType, BallStyle style, BallColor ballColor, bool checkJoin = false)
        {
            if (!ballCollider)
            {
                ballCollider = GetComponent<CircleCollider2D>();
            }
            if (!BallBody2D)
            {
                BallBody2D = GetComponent<Rigidbody2D>();
            }
            this.style = style;
            reactAfterJoint = false;
            killAfterJoint = false;
            this.ballColor = BallColor.None;
            ballRenderer.color = Color.white;
            Colored = true;
            Destroying = false;
            waveNum = 0;
            Targeted = false;
            ClearJoints();
            BallBody2D.isKinematic = this.gunBall = gunBall;
            BallType = ballType;
            BallColor = ballColor;
            WasJoin = false;
            ballCollider.enabled = true;
            if (checkJoin)
            {
                JoinNearBalls(ballCollider.radius);
            }
        }

        public void CorrectColor()
        {
            List<BallColor> colorsSet = new List<BallColor>(Balls.main.BallColors.Keys);
            colorsSet.Remove(BallColor.None);
            foreach (Ball b in joints.Keys)
            {
                if (colorsSet.Contains(b.BallColor))
                {
                    colorsSet.Remove(b.ballColor);
                }
            }
            if (colorsSet.Count > 0)
            {
                int index = Random.Range(0, colorsSet.Count);
                BallColor = colorsSet[index];
            }
            else
            {
                BallColor = BallColor.White;
            }
        }

        public void Shot(Vector2 force)
        {
            transform.parent = Balls.main.transform;
            IsKinematic = false;
            gunBall = false;
            BallBody2D.AddForce(force, ForceMode2D.Impulse);
            gameObject.layer = LayerMask.NameToLayer("balls");
        }

        private void ClearJoints()
        {
            foreach (Ball b in new List<Ball>(joints.Keys))
            {
                Unjoin(b);
            }
            joints.Clear();
            foreach (SpringJoint2D joint in GetComponents<SpringJoint2D>())
            {
                Destroy(joint);
            }
        }

        public void Kill(int addScore, Vector2 force, float delay)
        {
            if (!Destroying)
            {
                Destroying = true;
                if (gameObject.activeSelf)
                {
                    StartCoroutine(KillRoutine(addScore, force, delay));
                }
            }
        }

        public Ball GetBaseBall()
        {
            baseBall = null;
            ballsInChain.Clear();
            HasConnectionWithBase(this);
            return baseBall;
        }

        public void JoinNearBalls(float radius)
        {
            Collider2D[] hitColliders = new Collider2D[6];
            int collidersCount = Physics2D.OverlapCircleNonAlloc(Pos, radius, hitColliders);
            for (int i = 0; i < collidersCount; i++)
            {
                Ball b = hitColliders[i].GetComponent<Ball>();
                if (b && b != this && !b.gunBall)
                {
                    Join(b);
                }
            }
        }

        public void Destroy()
        {
            ClearJoints();
            Balls.DestroyBall(this);
        }

        public void CheckChain(float delay)
        {
            StartCoroutine(CheckChainRoutine(delay));
        }

        #endregion


        #region private

        public void SetBonusIcon(Sprite sprite)
        {
            bonusRenderer.gameObject.SetActive(true);
            bonusRenderer.sprite = sprite;
            bonusRenderer.color = Color.white;
            bonusRenderer.gameObject.ScaleFrom(Vector3.zero, 0.5f, 0f, EaseType.spring);
            Balls.EmitEffect("BallCollision", Pos, Vector2.zero);
        }

        public void SetBallSprite(Sprite sprite)
        {
            ballRenderer.sprite = sprite;
        }

        private void SetBallType(BallType ballType)
        {
            Destroying = false;
            ballRenderer.sprite = Balls.main.Simple;
            this.ballType = ballType;
            Sprite sprite = Balls.GetBallTypeSprite(ballType);
            bonusRenderer.gameObject.SetActive(false);

            BallBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            BallBody2D.gravityScale = 1f;
            switch (ballType)
            {
                case BallType.Crashed:
                    ballRenderer.sprite = sprite;
                    killAfterJoint = true;
                    break;
                case BallType.Simple:
                    break;
                case BallType.Chameleon:
                    SetBonusIcon(sprite);
                    StartCoroutine(ChangeColorCycleRoutine());
                    break;
                case BallType.Anchored:
                    SetBonusIcon(sprite);
                    BallBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                    break;
                case BallType.Freeze:
                    SetBonusIcon(sprite);
                    reactAfterJoint = true;
                    break;
                case BallType.Brush:
                    SetBonusIcon(sprite);
                    reactAfterJoint = true;
                    break;
                case BallType.Hammer:

                    break;
                case BallType.Bomb:
                    SetBonusIcon(sprite);
                    reactAfterJoint = true;
                    break;
                case BallType.Lightning:
                    SetBonusIcon(sprite);
                    reactAfterJoint = true;
                    break;
                case BallType.Aim:
                    SetBonusIcon(sprite);
                    reactAfterJoint = false;
                    break;
                case BallType.Frozen:
                    Colored = false;
                    ballRenderer.sprite = sprite;
                    bonusRenderer.sprite = null;
                    killAfterJoint = true;
                    break;
                case BallType.Anchor:
                    Colored = false;
                    ballRenderer.sprite = sprite;
                    bonusRenderer.sprite = null;
                    IsKinematic = true;
                    break;
                case BallType.Bubble:
                    Colored = false;
                    ballRenderer.sprite = sprite;
                    bonusRenderer.sprite = null;
                    BallBody2D.gravityScale = -1f;
                    killAfterJoint = true;
                    break;
                case BallType.Rust:
                    Colored = false;
                    ballRenderer.sprite = sprite;
                    bonusRenderer.sprite = null;
                    break;
                
            }
        }

        private void SetBallColor(BallColor ballColor)
        {
            RealColor = Balls.GetBallColor(ballColor);
            if (Colored)
            {
                if (style != BallStyle.Normal && BallType != BallType.Frozen && BallType != BallType.Rust)
                {
                    ballRenderer.sprite = Balls.GetColorFigureSprite(ballColor);
                }
                this.ballColor = ballColor;
            }
        }

        private void ProcessJoinWith(Ball ball, Joint2D joint)
        {
            if (!HasGravity)
            {
                HasGravity = true;
            }
            joints.Add(ball, joint);
            
            if (ball.reactAfterJoint)
            {
                ball.Kill(Balls.main.BallScore, Vel, 0.25f);
            }
        }

        private SpringJoint2D GetJoint(Ball ball)
        {
            SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.autoConfigureDistance = false;
            joint.frequency = Balls.main.SimpleBallJointStretch;
            joint.enableCollision = false;
            joint.connectedBody = ball.BallBody2D;
            joint.distance = 1f;
            return joint;
        }

        private bool Join(Ball ball)
        {
            if (!joints.ContainsKey(ball) && !ball.joints.ContainsKey(this))
            {
                if (ball.gunBall)
                {
                    Ball baseBall = GetBaseBall();
                    if (baseBall)
                    {
                        Vector2 pos = (Pos + ball.Pos) / 2f;
                        Balls.EmitEffect("GameOverCollision", pos, Vel);
                        UIManager.GameOver.Show();
                    }
                    else if (!IsKinematic && WasJoin)
                    {
                        BallType = BallType.Crashed;
                        Kill(Balls.main.BallScore, Vel * -1f, 0.01f);
                        return false;
                    }
                }
                else
                {
                    SpringJoint2D joint = GetJoint(ball);
                    ProcessJoinWith(ball, joint);
                    ball.ProcessJoinWith(this, joint);
                    if (!Destroying && BallColor == ball.BallColor)
                    {
                        Balls.EmitEffect("BallCollision", (Pos + ball.Pos) / 2f, Vel);
                        StartCoroutine(CheckChainRoutine(0.25f));
                    }
                    float volume = Mathf.Clamp(Vel.sqrMagnitude / 10f, 0.1f, 1f);
                    float pich = Mathf.Clamp(Vel.sqrMagnitude / 10f, 1f, 1.2f);
                    PlaySound("ballCollision", pich, volume);
                    ball.WasJoin = WasJoin = true;
                    return true;
                }
            }
            return false;
        }

        private void PlaySound(string soundName, float pitch, float volume)
        {
            SoundManager.Play(soundName, transform, pitch, volume);
        }

        private BallColor[] GetJoinBallColors()
        {
            List<BallColor> colors = new List<BallColor>();
            foreach (Ball b in joints.Keys)
            {
                if (!colors.Contains(b.BallColor) && b.BallColor != BallColor.None)
                {
                    colors.Add(b.BallColor);
                }
            }
            return colors.ToArray();
        }

        private bool Unjoin(Ball ball)
        {
            Joint2D joint = null;
            if (joints.TryGetValue(ball, out joint))
            {
                joints.Remove(ball);
                ball.joints.Remove(this);
                Destroy(joint);
                return true;
            }
            return false;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (killAfterJoint)
            {
                Kill(Balls.main.BallScore, Vel, 0f);
                return;
            }

            if (col.transform.tag == "wall")
            {
                if (Vel.sqrMagnitude > 10f)
                {
                    Balls.EmitEffect("BallCollision", col.contacts[0].point, Vel / 2f);
                    float volume = Mathf.Clamp(Vel.sqrMagnitude / 100f, 0.2f, 1f);
                    float pich = Random.Range(1.25f, 1.5f);
                    PlaySound("wallCollision", pich, volume);
                }
                WasJoin = true;
            }
            else if (col.transform.tag == tag && !gunBall)
            {
                Ball ball = col.gameObject.GetComponent<Ball>();
                if (ball)
                {
                    if (BallType == BallType.Hammer)// ball.BallType == BallType.Hammer)
                    {
                        if (ball.BallType == BallType.Simple)
                        {
                            ball.BallType = BallType.Crashed;
                        }
                        Vector2 force = Vel;
                        ball.BallBody2D.AddForceAtPosition(force, Pos, ForceMode2D.Impulse);
                        ball.Kill(Balls.main.BallScore, force, 0.01f);
                        return;
                    }
                    if (Join(ball))
                    {
                    }
                }
            }
        }

        #endregion


        #region routines

        private IEnumerator ChangeColorCycleRoutine()
        {
            while (BallType == BallType.Chameleon)
            {
                BallColor[] joinedColors = GetJoinBallColors();
                for (int i = 0; i < joinedColors.Length; i++)
                {
                    yield return StartCoroutine(ChangeColorRoutine(joinedColors[i]));
                    yield return new WaitForSeconds(0.1f);
                }
                yield return null;
            }
            yield return null;
        }

        private IEnumerator ChangeColorRoutine(BallColor destColor)
        {
            BallColor = destColor;
            yield return new WaitForEndOfFrame();
            yield return StartCoroutine(CheckChainRoutine(0.25f));
        }

        private IEnumerator KillRoutine(int scoreToAdd, Vector2 force, float delay)
        {
            Balls.EmitBonus(this, force, delay);
            yield return null;
            if (scoreToAdd != 0)
            {
                Balls.AddScore(scoreToAdd);
            }
        }

        private IEnumerator CheckChainRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (!Destroying && gameObject.activeSelf)
            {
                Queue<Ball> chain = GetBallsChain();
                if (chain.Count > 2)
                {
                    Balls.DestroyChain(chain, BallColor);
                }
            }
        }

        #endregion


        #region chain logic

        private void HasConnectionWithBase(Ball ball)
        {
            foreach (Ball b in ball.joints.Keys)
            {
                if (!ballsInChain.Contains(b))
                {
                    ballsInChain.Enqueue(b);
                    if (b.IsKinematic || b.BallType == BallType.Anchored)
                    {
                        baseBall = b;
                        break;
                    }
                    else { HasConnectionWithBase(b); }
                }
            }
        }

        private void InitShortestPathFrom(Ball ball)
        {
            foreach (Ball b in ball.joints.Keys)
            {
                if (b.waveNum == ball.waveNum - 1)
                {
                    if (!path.Contains(b) && !path.Contains(this))
                    {
                        path.Push(b);
                        InitShortestPathFrom(b);
                    }
                }
            }
        }

        private Queue<Ball> GetLinkedBalls()
        {
            ballsInChain.Clear();
            GetLinketBalls(this);
            return ballsInChain;
        }

        private void GetLinketBalls(Ball ball)
        {
            foreach (Ball b in ball.joints.Keys)
            {
                if (!ballsInChain.Contains(b))
                {
                    ballsInChain.Enqueue(b);
                    GetLinketBalls(b);
                }
            }
        }

        public Stack<Ball> GetPathTo(Ball ball)
        {
            CreateWave();
            path.Clear();
            path.Push(ball);
            InitShortestPathFrom(ball);
            ClearWave();
            return path;
        }

        private void CreateWave()
        {
            waveNum = 1;
            wavedBalls.Clear();
            wavedBalls.Push(this);
            List<Ball> allBalls = new List<Ball>(GetLinkedBalls());
            CreateWave(waveNum, allBalls);
        }

        private void ClearWave()
        {
            foreach (Ball b in wavedBalls)
            {
                b.waveNum = 0;
            }
        }

        private Stack<Ball> wavedBalls = new Stack<Ball>();

        private void CreateWave(int waveNum, List<Ball> allBalls)
        {
            bool hasBalls = false;
            foreach (Ball ball in allBalls.FindAll(b => b.waveNum == waveNum))
            {
                foreach (Ball joinedBall in ball.joints.Keys)
                {
                    if (joinedBall.waveNum == 0)
                    {
                        joinedBall.waveNum = waveNum + 1;
                        wavedBalls.Push(joinedBall);
                        hasBalls = true;
                    }
                }
            }
            if (hasBalls)
            {
                CreateWave(waveNum + 1, allBalls);
            }
        }

        private Queue<Ball> GetBallsChain()
        {
            ballsInChain.Clear();
            GetBallsChain(this);
            return ballsInChain;
        }

        private void GetBallsChain(Ball ball)
        {
            foreach (Ball b in ball.joints.Keys)
            {
                if ((b.BallColor == BallColor) && b.BallColor > 0 && (!b.IsKinematic))
                {
                    if (!ballsInChain.Contains(b))
                    {
                        ballsInChain.Enqueue(b);
                        GetBallsChain(b);
                    }
                }
            }
        }

        #endregion
    }
}