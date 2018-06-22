using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public class BallsGun : MonoBehaviour
    {
        #region fields
        public WaterWave LineWave;
        public BallType BallForShotType
        {
            get
            {
                if (ballForShot)
                {
                    return ballForShot.BallType;
                }
                return BallType.Simple;
            }
        }
        [Range(10f, 50f)]
        public float ShotStretch;
        [Range(1, 10)]
        public int ShotCount = 7;
        [Header("Color bonuses")]
        public BallType GreenBonus;
        public BallType AquaBonus;
        public BallType RedBonus;
        public BallType BlueBonus;
        public BallType YellowBonus;
        public BallType BlackBonus;
        private Dictionary<BallColor, BallType> bonusColorDic;
        private bool touched;
        private bool charging;
        private float startTouchPosX;
        private float endTouchPosX;
        private bool swaping;
        private bool inited;
        private List<Ball> balls;
        private Ball ballForShot;
        public LineRenderer line;
        public Laser LaserAim;
        public Transform LeftTrigger;
        public Transform RightTrigger;
        public float MoveSpeed = 0.05f;
        private float maxPosY;
        private bool moving;
        private bool pressed;
        private Vector3 toPos;
        public bool LaserEnabled;
        //private int ballsCount;
        //public int BallsCount
        //{
        //    get { return ballsCount; }
        //    private set
        //    {
        //        ballsCount = value;
        //        if (BallsCountChanged != null)
        //        {
        //            BallsCountChanged(value);
        //        }
        //    }
        //}
        public event Action<int> BallsCountChanged = delegate { };
        #endregion

        public void Init()
        {
            moving = false;
            Vector3 camPos = GameManager.main.CamController.transform.position;
            camPos.y = 0f;
            GameManager.main.CamController.transform.position = camPos;
            if (!inited)
            {
                balls = new List<Ball>();
                bonusColorDic = new Dictionary<BallColor, BallType>();
                bonusColorDic[BallColor.Green] = GreenBonus;
                bonusColorDic[BallColor.Aqua] = AquaBonus;
                bonusColorDic[BallColor.Red] = RedBonus;
                bonusColorDic[BallColor.Blue] = BlueBonus;
                bonusColorDic[BallColor.Yellow] = YellowBonus;
                bonusColorDic[BallColor.Black] = BlackBonus;
                inited = true;
            }
            AlignPos();
            line.startWidth = line.endWidth = 1f;
            maxPosY = transform.position.y;
        }

        private Vector3 GetShotPos()
        {
            return new Vector3(0f, transform.position.y + 1.1f, transform.position.z);
        }

        private void AlignPos()
        {
            Camera cam = GameManager.main.CamController.Cam;
            Vector3 stageDimensions = cam.ScreenToWorldPoint(new Vector3(0f, 0f, cam.transform.position.z + 1f));
            transform.position = new Vector3(0f, -stageDimensions.y, -0.5f);
        }

        public void DestroyBalls()
        {
            foreach (Ball b in balls)
            {
                Destroy(b.gameObject);
            }
            balls.Clear();
            if (ballForShot != null)
            {
                Destroy(ballForShot.gameObject);
                ballForShot = null;
            }
            balls = new List<Ball>();
        }

        public void SetGunBallType(BallColor ballColor)
        {
            if (bonusColorDic.ContainsKey(ballColor))
            {
                SetGunBallType(bonusColorDic[ballColor]);
            }
        }

        public void SetGunBallType(BallType ballType)
        {
            if (ballForShot && ballType != BallType.None)
            {
                if (ballForShot.BallType == ballType)
                {
                    ballForShot.BallType = BallType.Simple;
                }
                else
                {
                    ballForShot.BallType = ballType;
                    LaserEnabled = ballType == BallType.Aim;
                    LaserAim.gameObject.SetActive(LaserEnabled);
                }
            }
        }
        
        private void ProcessLaserInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                pressed = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (pressed)
                {
                    pressed = false;
                    Shot(Input.mousePosition, false);
                }
            }
        }

        private void HideTutorial()
        {
            if (UIManager.Info.isActiveAndEnabled)
            {
                UIManager.Info.Hide();
            }
        }

        private void ProcessSimpleInput()
        {
            if (Input.GetMouseButton(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Shot(Input.mousePosition, true);
                }
            }
        }
        
        private void LateUpdate()
        {
            if (Balls.Active && inited)
            {
                if (Input.mousePosition.y > GameManager.main.CamController.Cam.WorldToScreenPoint(GetShotPos()).y + 50f)
                {
                    if (LaserEnabled)
                    {
                        ProcessLaserInput();
                    }
                    else
                    {
                        ProcessSimpleInput();
                    }
                }
                else if (!charging)
                {
                    if (!touched && Input.GetMouseButtonDown(0))
                    {
                        touched = true;
                        startTouchPosX = Input.mousePosition.x;
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        touched = false;
                        endTouchPosX = Input.mousePosition.x;
                        if (!moving && Mathf.Abs(endTouchPosX - startTouchPosX) > 20f)
                        {
                            if (endTouchPosX > startTouchPosX)
                            {
                                StartCoroutine(SwapBallRoutine(-1));
                            }
                            else
                            {
                                StartCoroutine(SwapBallRoutine(1));
                            }
                        }
                        endTouchPosX = startTouchPosX;
                    }
                }
            }
        }

        private void RestoreGunPos(int direction, float k = 1f)
        {
            Vector3 gunPos = transform.position;
            gunPos.x = 0;
            gunPos.z = -0.5f;
            transform.position = gunPos;
            foreach (Ball b in balls)
            {
                Vector3 pos = b.transform.position;
                pos.x += direction * k;
                b.transform.position = RoundXVector(pos, 1);
            }
        }

        private Vector3 RoundXVector(Vector3 v, int digigs, int comp = 1)
        {
            return new Vector3((float)System.Math.Round(v.x * comp, digigs), v.y, v.z);
        }

        private Vector3 GetLerpPosSinY(Vector3 startPos, Vector3 endPos, float t)
        {
            Vector3 result = Vector3.Lerp(startPos, endPos, t);
            result.y += Mathf.Sin(Mathf.PI * t) / 2f;
            return result;
        }

        private bool CanShot()
        {
            return ballForShot && !swaping && !charging && inited;
        }

        private void Shot(Vector3 touchPos, bool hasGravity)
        {
            if (CanShot())
            {
                //if (!Balls.EndlessMode)
                //{
                //    if (BallsCount > 0 && Balls.main.BallsLimit)
                //    {
                //        BallsCount--;
                //    }
                //    else
                //    {
                //        UIManager.GameOver.Show();
                //        return;
                //        // TODO: show no more balls
                //    }
                //}

                HideTutorial();
                Camera cam = GameManager.main.CamController.Cam;
                Vector3 heading = touchPos - cam.WorldToScreenPoint(GetShotPos());
                ballForShot.HasGravity = hasGravity;
                ballForShot.Shot(heading.normalized * ShotStretch);
                ballForShot.transform.parent = Balls.main.transform;
                Balls.AddBall(ballForShot);
                Balls.main.ComboNum = 1;
                Charge();
                PlaySound("shot", UnityEngine.Random.Range(0.9f, 1.1f), 1f);
            }
        }

        private void PlaySound(string soundName, float pitch, float volume)
        {
            SoundManager.Play(soundName, transform, pitch, volume);
        }

        private void Charge()
        {
            if (balls.Count > 0)
            {
                StartCoroutine(ChargeRoutine());
            }
            else
            {
               // if (Balls.EndlessMode || !Balls.main.BallsLimit)
                {
                    StartCoroutine(MoveUpRoutine(MoveSpeed));
                }
                
                StartCoroutine(GenerateGunBallsRoutine());
            }
        }

        #region routines

        private IEnumerator InitLineRoutine()
        {
            line.startWidth = line.endWidth = 1f;
            yield return null;
        }
       
        private IEnumerator MoveUpRoutine(float speed)
        {
            moving = true;
            Vector3 fromPos = gameObject.transform.position;
            toPos = gameObject.transform.position;
            float dY = Mathf.Sqrt(3f) / 2f;
            toPos.y += dY;
            for (float f = 0f; f <= 1f; f += speed * Time.deltaTime)
            {
                transform.position = Vector3.Lerp(fromPos, toPos, f);
                yield return null;
            }
            transform.position = toPos;
            yield return null;
            if (transform.position.y > maxPosY)
            {
                Balls.CreateNewLine();
                maxPosY = transform.position.y;
            }
            moving = false;
        }

        private IEnumerator ChargeRoutine()
        {
            if (!charging && !swaping)
            {
                charging = true;
                ballForShot = balls[balls.Count - 1];
                balls.Remove(ballForShot);
                Vector3 oldBallForShotPos = new Vector3(ballForShot.Pos.x, ballForShot.Pos.y, transform.position.z);
                Vector3 oldGunPos = transform.position;
                Vector3 targetGunPos = oldGunPos;
                targetGunPos.x += 0.5f;
                Vector3 shotPos = GetShotPos();
                for (float f = 0; f <= 1f; f += Time.smoothDeltaTime * 10f)
                {
                    if (ballForShot)
                    {
                        ballForShot.transform.position = Vector3.Lerp(oldBallForShotPos, shotPos, f);
                        transform.position = Vector3.Lerp(oldGunPos, targetGunPos, f);
                        yield return 0;
                    }
                }
                if (ballForShot)
                {
                    transform.position = targetGunPos;
                    RestoreGunPos(1, 0.5f);
                    ballForShot.transform.position = shotPos;

                    yield return 0;
                }
                SetLineColor(ballForShot);
                charging = false;
            }
        }

        private void SetLineColor(Ball ball)
        {
            if (ball)
            {
                LaserAim.LaserColor = ball.RealColor;
                LineWave.SetLineColor(ball.RealColor);
                LaserEnabled = ball.BallType == BallType.Aim;
                LaserAim.gameObject.SetActive(LaserEnabled);
            }
            else
            {
                LineWave.SetLineColor(Color.white);
                LaserAim.gameObject.SetActive(false);
            }
        }

        private IEnumerator SwapBallRoutine(int direction)
        {
            if (!swaping && balls.Count > 0)
            {
                swaping = true;
                Vector3 oldGunPos = transform.position;
                oldGunPos.x = 0f;
                Vector3 targetGunPos = oldGunPos;
                targetGunPos.x += direction;
                Ball ballForReturn = ballForShot;
                Vector3 targetBallForShotPos = GetShotPos();
                ballForReturn.transform.parent = transform;
                if (direction < 0)
                {
                    balls.Add(ballForReturn);
                    ballForShot = balls[0];
                }
                else
                {
                    balls.Insert(0, ballForReturn);
                    ballForShot = balls[balls.Count - 1];
                }
                ballForShot.transform.parent = Balls.main.transform;
                balls.Remove(ballForShot);
                Vector3 oldBallForShotPos = RoundXVector(ballForShot.transform.position, 3, -1);
                Vector3 oldPos = ballForShot.transform.position;
                for (float t = 0; t <= 1f; t += Time.smoothDeltaTime * 5f)
                {
                    ballForReturn.transform.position = GetLerpPosSinY(targetBallForShotPos, oldBallForShotPos, t);
                    ballForShot.transform.position = GetLerpPosSinY(oldPos, targetBallForShotPos, t);
                    transform.position = Vector3.Lerp(oldGunPos, targetGunPos, Mathf.Sin((Mathf.PI / 2f) * t));
                    yield return new WaitForEndOfFrame();
                }
                PlaySound("shot", UnityEngine.Random.Range(0.9f, 1.1f), 1f);
                ballForReturn.transform.position = oldBallForShotPos;
                ballForShot.transform.position = targetBallForShotPos;
                transform.position = targetGunPos;
                RestoreGunPos(direction);
                SetLineColor(ballForShot);
                swaping = false;
            }
        }

        public void ResetGun()
        {
            StartCoroutine(GenerateGunBallsRoutine());
        }
        
        public void ResetGun(Level levelProfile)
        {
            //BallsCount = 7 * levelProfile.ShotSlots;
            StartCoroutine(GenerateGunBallsRoutine());
        }

        private IEnumerator GenerateGunBallsRoutine()
        {
            List<BallColor> colors = new List<BallColor>(Balls.EndlessMode ? Balls.Colors : Balls.GetColorSet());
            if (colors.Contains(BallColor.None))
            {
                colors.Remove(BallColor.None);
            }
            charging = true;
            Vector3 gunPos = transform.position;
            gunPos.x = 0;
            gunPos.z = -0.5f;
            transform.position = gunPos;

            for (int i = 0; i < ShotCount; i++)
            {
                BallColor color = colors[UnityEngine.Random.Range(0, colors.Count)];
                Vector2 pos = new Vector2(-(ShotCount + 1) / 2f + (i + 1), 0f);
                Ball newBall = Balls.CreateGunBall(pos, color);
                balls.Add(newBall);
                yield return new WaitForEndOfFrame();
            }
            charging = false;
            Charge();
        }

        #endregion
    }
}