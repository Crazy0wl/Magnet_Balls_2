using System;
using System.Collections.Generic;
using UnityEngine;

namespace MB_Engine
{
    public enum BallType
    {
        None = 0,
        Simple = 1,
        Chameleon = 2,
        Freeze = 3,
        Brush = 4,
        Bomb = 5,
        Lightning = 6,
        Aim = 7,
        Hammer = 8,
        Rust = 10,
        Bubble = 11,
        Frozen = 12,
        Crashed = 13,
        Anchored = 14,
        Anchor = 15,
        Antimagnet = 16
    }

    public enum BallColor
    {
        None = 0,
        White = 1,
        Black = 2,
        Green = 3,  
        Aqua = 4,
        Red = 5,
        Blue = 6,
        Yellow = 7,
    }

    [Serializable]
    public class Levels : Singleton<Levels>
    {
        protected Levels() { }
        public Texture2D Anchored;
        public Texture2D Anchor;
        public Texture2D White;
        public Texture2D Black;
        public Texture2D Red;
        public Texture2D Green;
        public Texture2D Blue;
        public Texture2D Yellow;
        public Texture2D Aqua;
        public Texture2D Empty;
        public Texture2D Frozen;
        public Texture2D Rust;
        public Texture2D Bubble;
        public List<Level> levels;
    }
}