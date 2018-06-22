using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MB_Engine
{
    [Serializable]
    [CustomEditor(typeof(Levels))]
    public class LevelEditor : Editor
    {
        #region fields
        private Levels targetLevels;
        private GUISkin normalSkin;
        private GUISkin offsetSkin;
        private GUIStyle removeBtnStyle;
        private GUIStyle addBtnStyle;
        private bool foldoutTextures = false;
        private bool foldoutLevels = false;
        private BallType ballType = BallType.None;
        private BallColor ballColor = BallColor.None;
        #endregion

        private void SetupSkins()
        {
            if (normalSkin == null)
            {
                normalSkin = CreateInstance(typeof(GUISkin)) as GUISkin;
            }
            normalSkin.button.border = new RectOffset(0, 0, 0, 0);
            normalSkin.button.padding = new RectOffset(0, 0, 0, 0);
            normalSkin.button.margin = new RectOffset(0, 0, 0, 0);
            normalSkin.button.fixedWidth = 32;
            normalSkin.button.fixedHeight = 32;
            normalSkin.button.stretchHeight = false;
            normalSkin.button.stretchWidth = false;

            if (offsetSkin == null)
            {
                offsetSkin = CreateInstance(typeof(GUISkin)) as GUISkin;
            }
            offsetSkin.button.border = new RectOffset(0, 0, 0, 0);
            offsetSkin.button.padding = new RectOffset(0, 0, 0, 0);
            offsetSkin.button.margin = new RectOffset(0, 0, 0, 0);
            offsetSkin.button.fixedWidth = 32;
            offsetSkin.button.fixedHeight = 32;
            offsetSkin.button.stretchHeight = false;
            offsetSkin.button.stretchWidth = false;
            offsetSkin.button.margin.left = (int)(offsetSkin.button.fixedWidth * 0.5f);

            if (removeBtnStyle == null)
            {
                removeBtnStyle = new GUIStyle(GUI.skin.button);
                removeBtnStyle.normal.textColor = Color.red;
                removeBtnStyle.fontSize = 24;
                removeBtnStyle.fontStyle = FontStyle.Bold;
                removeBtnStyle.fixedHeight = 32;
                removeBtnStyle.fixedWidth = 32;
            }

            if (addBtnStyle == null)
            {
                addBtnStyle = new GUIStyle(GUI.skin.button);
                addBtnStyle.normal.textColor = Color.blue;
                addBtnStyle.fontSize = 24;
                addBtnStyle.fontStyle = FontStyle.Bold;
                addBtnStyle.fixedWidth = 32;
                addBtnStyle.fixedHeight = 32;
            }
        }

        private Texture2D GetBallTypeTexture(BallType ballType)
        {
            Texture2D result = null;
            switch (ballType)
            {
                case BallType.Simple:
                    result = targetLevels.White != null ? targetLevels.White : null;
                    break;
                case BallType.Frozen:
                    result = targetLevels.Frozen != null ? targetLevels.Frozen : null;
                    break;
                case BallType.Rust:
                    result = targetLevels.Rust != null ? targetLevels.Rust : null;
                    break;
                case BallType.Bubble:
                    result = targetLevels.Bubble != null ? targetLevels.Bubble : null;
                    break;
                case BallType.Anchored:
                    result = targetLevels.Anchored != null ? targetLevels.Anchored : null;
                    break;
                case BallType.Anchor:
                    result = targetLevels.Anchor != null ? targetLevels.Anchor : null;
                    break;
                case BallType.None:
                    result = targetLevels.Empty != null ? targetLevels.Empty : null;
                    break;
            }
            return result;
        }

        private Texture2D GetBallColorTexture(BallColor ballColor)
        {
            Texture2D result = null;
            switch (ballColor)
            {
                case BallColor.White:
                    result = targetLevels.White != null ? targetLevels.White : null;
                    break;
                case BallColor.Black:
                    result = targetLevels.Black != null ? targetLevels.Black : null;
                    break;
                case BallColor.Green:
                    result = targetLevels.Green != null ? targetLevels.Green : null;
                    break;
                case BallColor.Aqua:
                    result = targetLevels.Aqua != null ? targetLevels.Aqua : null;
                    break;
                case BallColor.Red:
                    result = targetLevels.Red != null ? targetLevels.Red : null;
                    break;
                case BallColor.Blue:
                    result = targetLevels.Blue != null ? targetLevels.Blue : null;
                    break;
                case BallColor.Yellow:
                    result = targetLevels.Yellow != null ? targetLevels.Yellow : null;
                    break;
                case BallColor.None:
                    result = targetLevels.Empty != null ? targetLevels.Empty : null;
                    break;
            }
            return result;
        }

        public override void OnInspectorGUI()
        {
            targetLevels = (Levels)target;
            List<Level> levels = targetLevels.levels;
            if (levels == null)
            {
                levels = targetLevels.levels = new List<Level>();
            }
            SetupSkins();

            #region Texture setup
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Separator();
            foldoutTextures = EditorGUILayout.Foldout(foldoutTextures, "Textures");
            if (foldoutTextures)
            {
                targetLevels.White = EditorGUILayout.ObjectField("White", targetLevels.White, typeof(Texture2D), false) as Texture2D;
                targetLevels.Black = EditorGUILayout.ObjectField("Black", targetLevels.Black, typeof(Texture2D), false) as Texture2D;
                targetLevels.Red = EditorGUILayout.ObjectField("Red", targetLevels.Red, typeof(Texture2D), false) as Texture2D;
                targetLevels.Green = EditorGUILayout.ObjectField("Green", targetLevels.Green, typeof(Texture2D), false) as Texture2D;
                targetLevels.Blue = EditorGUILayout.ObjectField("Blue", targetLevels.Blue, typeof(Texture2D), false) as Texture2D;
                targetLevels.Yellow = EditorGUILayout.ObjectField("Yellow", targetLevels.Yellow, typeof(Texture2D), false) as Texture2D;
                targetLevels.Aqua = EditorGUILayout.ObjectField("Aqua", targetLevels.Aqua, typeof(Texture2D), false) as Texture2D;

                targetLevels.Empty = EditorGUILayout.ObjectField("Empty", targetLevels.Empty, typeof(Texture2D), false) as Texture2D;
                targetLevels.Frozen = EditorGUILayout.ObjectField("Frozen", targetLevels.Frozen, typeof(Texture2D), false) as Texture2D;
                targetLevels.Rust = EditorGUILayout.ObjectField("Rust", targetLevels.Rust, typeof(Texture2D), false) as Texture2D;
                targetLevels.Bubble = EditorGUILayout.ObjectField("Bubble", targetLevels.Bubble, typeof(Texture2D), false) as Texture2D;
                targetLevels.Anchored = EditorGUILayout.ObjectField("Anchored", targetLevels.Anchored, typeof(Texture2D), false) as Texture2D;
                targetLevels.Anchor = EditorGUILayout.ObjectField("Anchor", targetLevels.Anchor, typeof(Texture2D), false) as Texture2D;
            }
            #endregion

            EditorGUILayout.Separator();
            foldoutLevels = EditorGUILayout.Foldout(foldoutLevels, "Levels");
            if (foldoutLevels)
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    int levelNum = i + 1;
                    Level level = levels[i];
                    if (level)
                    {
                        EditorGUI.indentLevel++;

                        level.Foldout = EditorGUILayout.Foldout(level.Foldout, levelNum.ToString());
                        level.Num = levelNum;
                        if (level.Foldout)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (GUILayout.Button("x", removeBtnStyle))
                            {
                                levels.Remove(level);
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                            if (GUILayout.Button("#", removeBtnStyle))
                            {
                                level.Clear();
                                EditorGUILayout.EndHorizontal();
                                break;
                            }
                            EditorGUILayout.EndHorizontal();
                            if (EditorGUILayout.BeginFadeGroup(1f))
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Score Stars", GUILayout.ExpandWidth(true));
                                level.FirstStarScore = Mathf.Max(EditorGUILayout.IntField(level.FirstStarScore, GUILayout.ExpandWidth(true)), 1);
                                level.SecondStarScore = Mathf.Max(EditorGUILayout.IntField(level.SecondStarScore, GUILayout.ExpandWidth(true)), level.FirstStarScore + 1);
                                level.ThirdStarScore = Mathf.Max(EditorGUILayout.IntField(level.ThirdStarScore, GUILayout.ExpandWidth(true)), level.SecondStarScore + 1);
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndFadeGroup();
                            

                            EditorGUILayout.BeginHorizontal();
                            ballType = (BallType)EditorGUILayout.EnumPopup("Ball", ballType);
                            EditorGUILayout.EndHorizontal();

                            if (ballType != BallType.None)
                            {
                                if (ballType == BallType.Simple)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    ballColor = (BallColor)EditorGUILayout.EnumPopup("Color", ballColor);
                                    EditorGUILayout.EndHorizontal();
                                }
                            }

                            EditorGUILayout.BeginHorizontal();
                            level.ShotSlots = Mathf.Max(EditorGUILayout.IntField("Shot slots", level.ShotSlots, GUILayout.ExpandWidth(true)), 1);
                            level.FixColors = EditorGUILayout.BeginToggleGroup("Fix colors", level.FixColors);
                            EditorGUILayout.EndToggleGroup();
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Separator();
                            for (int row = 0; row < level.Rows; row++)
                            {
                                bool odd = row % 2 == 0;
                                EditorGUILayout.BeginHorizontal();
                                for (int col = 0; col < level.Cols; col++)
                                {
                                    if (col == level.Cols - 1 && !odd)
                                    {
                                        continue;
                                    }
                                    BallType ballType = level.GetBall(col, row);
                                    BallColor ballColor = level.GetColor(col, row);
                                    Texture2D backgroundTexture = null;
                                    Texture2D bonusTexture = null;
                                    if (ballType == BallType.Simple)
                                    {
                                        backgroundTexture = GetBallColorTexture(ballColor);
                                    }
                                    else
                                    {
                                        backgroundTexture = GetBallTypeTexture(ballType);

                                    }

                                    GUISkin style = !odd && col == 0 ? offsetSkin : normalSkin;
                                    style.button.normal.background = backgroundTexture;

                                    if (GUILayout.Button(bonusTexture, style.button))
                                    {
                                        ballType = this.ballType;
                                        ballColor = this.ballColor;
                                    }
                                    level.SetBall(col, row, ballType);
                                    level.SetColor(col, row, ballColor);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUI.indentLevel--;
                        EditorUtility.SetDirty(level);
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("+", addBtnStyle))
                {
                    levels.Add(CreateInstance("Level") as Level);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorUtility.SetDirty(targetLevels);
        }
    }
}