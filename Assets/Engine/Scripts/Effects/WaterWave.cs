using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public class WaterWave : MonoBehaviour
    {
        #region fields
        struct WaterColumn
        {
            public float Height;
            public float Speed;
            public float PosX;
            public int IntPos;
            public int Num;
            
            public void Update(float dampening, float tension)
            {
                Speed += -Height * tension - Speed * dampening;
                Height += Speed;
            }
        }
        public EdgeCollider2D Collider;
        public Vector3[] points;
        public Vector2[] points2D;
        private WaterColumn[] columns;
        public float Tension = 0.025f;
        public float Dampening = 0.025f;
        public float Spread = 0.25f;
        public LineRenderer line;
        public Transform LeftTrigger;
        public Transform RightTrigger;
        private float[] lDeltas;
        private float[] rDeltas;
        private const int COLUMNS = 9;
        public string TagForDestroy;
        private Dictionary<int, WaterColumn> colDic;
        private Ball baseBall;
        public float DeltaY = 0.0f;
        #endregion

        private void Start()
        {
            colDic = new Dictionary<int, WaterColumn>();
            columns = new WaterColumn[line.positionCount];
            lDeltas = new float[line.positionCount];
            rDeltas = new float[line.positionCount];
            for (int i = 0; i < line.positionCount; i++)
            {
                float posX = Mathf.Lerp(LeftTrigger.position.x, RightTrigger.position.x, (float)i / (line.positionCount - 1));
                int intPos = (int)posX;
                columns[i] = new WaterColumn()
                {
                    Num = i,
                    PosX = posX,
                    IntPos = intPos
                };
                colDic[intPos] = columns[i];
            }
            points = new Vector3[line.positionCount];
            points2D = new Vector2[line.positionCount];
            line.startColor = Color.white;
            line.endColor = Color.red;
            SetLineColor(Color.white);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == TagForDestroy && !collision.attachedRigidbody.isKinematic)
            {
                Ball ballForDestroy = collision.GetComponent<Ball>();
                if (ballForDestroy && ballForDestroy.WasJoin)
                {
                    Balls.EmitEffect("BallCollision", ballForDestroy.Pos, -ballForDestroy.Vel);
                    baseBall = ballForDestroy.GetBaseBall();
                    if (baseBall)
                    {
                        Balls.EmitEffect("GameOverCollision", ballForDestroy.Pos, -ballForDestroy.Vel);
                        //Balls.WasGameOver = true;
                        UIManager.GameOver.Show();
                    }
                    else
                    {
                        float splashSpeed = ballForDestroy.Vel.y * 10f;
                        Splash(ballForDestroy.Pos.x, Mathf.Max(splashSpeed, 0.2f));
                        Vector3 force = new Vector3(0f, -ballForDestroy.Vel.y, -ballForDestroy.Vel.magnitude * 0.1f);

                        Balls.EmitCoin(ballForDestroy);
                     //   GameData.Coins++;
                        SoundManager.Play("gemAdded", Random.Range(0.9f, 1.1f), 1f);
                        Balls.EmitEffect("BallCollision", ballForDestroy.Pos, -ballForDestroy.Vel);
                        if (ballForDestroy.BallType == BallType.Lightning)
                        {
                            ballForDestroy.BallType = BallType.Simple;
                        }
                        ballForDestroy.Kill(Balls.main.BallScore, Vector3.zero, 0f);
                    }
                }
            }
        }

        public void SetLineColor(Color col)
        {
            line.startColor = line.endColor = col;
            Splash(-0.5f);
        }

        private void Splash(float xPos, float speed)
        {
            int index = (int)xPos;
            WaterColumn col = colDic[index];
            speed = Mathf.Clamp(speed, -5f, 5f);
            columns[col.Num].Speed = -speed;
        }

        private void Splash(float speed)
        {
            int index = (int) (columns.Length / 2f);
            speed = Mathf.Clamp(speed, -1f, 1f);
            columns[index].Speed = -speed;
        }

        public void LateUpdate()
        {
            UpdateWave();
        }
        
        public void UpdateWave()
        {
            for (int i = 0; i < columns.Length; i++)
            {
                // left
                if (i > 0)
                {
                    lDeltas[i] = Spread * (columns[i].Height - columns[i - 1].Height);
                    columns[i - 1].Speed += lDeltas[i];
                }
                // right
                if (i < columns.Length - 1)
                {
                    rDeltas[i] = Spread * (columns[i].Height - columns[i + 1].Height);
                    columns[i + 1].Speed += rDeltas[i];
                }
            }
            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    columns[i - 1].Height += lDeltas[i];
                }
                if (i < columns.Length - 1)
                {
                    columns[i + 1].Height += rDeltas[i];
                }
            }
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i].Update(Dampening, Tension);
                points2D[i] = new Vector2(columns[i].PosX, columns[i].Height + DeltaY);
                points[i] = points2D[i];
                points[i] += transform.position;
                line.SetPosition(i, points[i]);
            }
            Collider.points = points2D;
        }
    }
}