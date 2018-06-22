using System;
using UnityEngine;

namespace MB_Engine
{
    [Serializable]
    public class Level : ScriptableObject
    {
        public static int MAX_COLS = 10;
        public static int MAX_ROWS = 20;
        public int Num;
        public int Cols { get { return MAX_COLS; } }
        public int Rows { get { return MAX_ROWS; } }
        public int ShotSlots;
        public int FirstStarScore;
        public int SecondStarScore;
        public int ThirdStarScore;
        public bool Foldout;
        public BallType[] Balls = new BallType[MAX_COLS * MAX_ROWS];
        public BallColor[] Colors = new BallColor[MAX_COLS * MAX_ROWS];
        public bool FixColors;

        public void SetBall(int col, int row, BallType ballType)
        {
            Balls[col * MAX_ROWS + row] = ballType;
        }
        public BallType GetBall(int col, int row)
        {
            return Balls[col * MAX_ROWS + row];
        }

        public void SetColor(int col, int row, BallColor ballColor)
        {
            Colors[col * MAX_ROWS + row] = ballColor;
        }
        public BallColor GetColor(int col, int row)
        {
            return Colors[col * MAX_ROWS + row];
        }

        public void Clear()
        {
            for (int i = 0; i < Balls.Length; i++)
            {
                Balls[i] = BallType.None;
                Colors[i] = BallColor.None;
            }
        }

        public BallColor GetRandomColor()
        {
            return (BallColor)GetRandom((int)BallColor.None, (int)BallColor.Yellow);
        }

        public BallType GetRandomBall()
        {
            return GetRandom(0f, 1f) > 0.8f ? BallType.None : BallType.Simple;
        }

        private int GetRandom(int max, int min)
        {
            return UnityEngine.Random.Range(min, max);
        }

        private float GetRandom(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}