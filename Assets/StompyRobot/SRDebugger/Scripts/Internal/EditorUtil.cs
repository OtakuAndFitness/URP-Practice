﻿#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SRDebugger.Internal.Editor
{
    public static class SRDebugEditorUtil
    {
        // Path to this file from the root path
        private const string TestPath = "SRDebugger/Scripts/Internal/EditorUtil.cs";
        private static GUIStyle _bgStyle;
        private static Texture2D _logoTexture;
        private static Texture2D _welcomeLogoTexture;
        private static Texture2D _bgTexture;
        private static GUIStyle _middleAlign;

        public static string GetRootPath()
        {
            // Find assets that match this file name
            var potentialAssets = AssetDatabase.FindAssets("EditorUtil");

            foreach (var potentialAsset in potentialAssets)
            {
                var path = AssetDatabase.GUIDToAssetPath(potentialAsset);

                if (path.Contains(TestPath))
                {
                    // Decend three levels in file tree
                    var rootPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(path)));
                    return rootPath;
                }
            }

            throw new Exception("Unable to find SRDebugger root path");
        }

        public static Texture2D GetLogo()
        {
            if (_logoTexture != null)
            {
                return _logoTexture;
            }

            return
                _logoTexture =
                    Resources.Load<Texture2D>("SRDebugger/Logo_" + (EditorGUIUtility.isProSkin ? "DarkBG" : "LightBG"));
        }

        public static Texture2D GetWelcomeLogo()
        {
            if (_welcomeLogoTexture != null)
            {
                return _welcomeLogoTexture;
            }

            return
                _welcomeLogoTexture =
                    Resources.Load<Texture2D>("SRDebugger/WelcomeLogo_" +
                                              (EditorGUIUtility.isProSkin ? "DarkBG" : "LightBG"));
        }

        public static Texture2D GetBackground()
        {
            if (_bgTexture != null)
            {
                return _bgTexture;
            }

            return
                _bgTexture =
                    Resources.Load<Texture2D>("SRDebugger/BG_" + (EditorGUIUtility.isProSkin ? "Dark" : "Light"));
            //return _bgTexture = Resources.Load<Texture2D>("SRDebugger/HeaderBG");
        }

        public static void DrawLogo(Texture2D logo)
        {
            if (logo == null)
            {
                Debug.LogError("Error loading SRDebugger logo");
                return;
            }

            var rect = EditorGUILayout.BeginVertical();

            GUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUI.DrawTexture(
                GUILayoutUtility.GetRect(logo.width, logo.width, logo.height, logo.height, GUILayout.ExpandHeight(false),
                    GUILayout.ExpandWidth(false)),
                logo);

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);

            EditorGUILayout.EndVertical();

            var size = EditorStyles.miniLabel.CalcSize(new GUIContent(SRDebug.Version));
            GUI.Label(new Rect(rect.xMax - size.x, rect.yMax - size.y, size.x, size.y), SRDebug.Version,
                EditorStyles.miniLabel);
        }

        public static bool DrawInspectorFoldout(bool isVisible, string content)
        {
            isVisible = EditorGUILayout.Foldout(isVisible, content, Styles.InspectorHeaderFoldoutStyle);

            EditorGUILayout.Separator();

            return isVisible;
        }

        public static void BeginDrawBackground()
        {
            if (_bgStyle == null)
            {
                _bgStyle = new GUIStyle();
                _bgStyle.margin = _bgStyle.padding = new RectOffset(0, 0, 0, 0);
            }

            var rect = EditorGUILayout.BeginVertical(_bgStyle);

            DrawTextureTiled(rect, GetBackground());
        }

        public static void EndDrawBackground()
        {
            EditorGUILayout.EndVertical();
        }

        public static void DrawTextureTiled(Rect rect, Texture2D tex)
        {
            GUI.BeginGroup(rect);

            var tilesX = Mathf.Max(1, Mathf.CeilToInt(rect.width/tex.width));
            var tilesY = Mathf.Max(1, Mathf.CeilToInt(rect.height/tex.height));

            for (var x = 0; x < tilesX; x++)
            {
                for (var y = 0; y < tilesY; y++)
                {
                    var pos = new Rect(x*tex.width, y*tex.height, tex.width, tex.height);
                    pos.x += rect.x;
                    pos.y += rect.y;

                    GUI.DrawTexture(pos, tex, ScaleMode.ScaleAndCrop);
                }
            }

            GUI.EndGroup();
        }

        public static bool ClickableLabel(string text, GUIStyle style)
        {
            var rect = EditorGUILayout.BeginVertical(Styles.NoPaddingNoMargin);

            GUILayout.Label(text, style);

            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
            {
                return true;
            }

            return false;
        }

        public static void DrawLayoutPreview(Rect rect)
        {
            if (_middleAlign == null)
            {
                _middleAlign = new GUIStyle(GUI.skin.box);
                _middleAlign.alignment = TextAnchor.MiddleCenter;
            }

            var iconPath = "SRDebugger/Icons/" + (EditorGUIUtility.isProSkin ? "Light" : "Dark");

            const float consoleHeight = 90;

            GUI.Box(rect, "");

            var consoleAlignment = Settings.Instance.ConsoleAlignment;

            var consoleRect = new Rect(rect.x,
                consoleAlignment == ConsoleAlignment.Top ? rect.y : rect.yMax - consoleHeight, rect.width,
                consoleHeight);

            GUI.Box(consoleRect, new GUIContent(Resources.Load<Texture2D>(iconPath + "/console-25"), "Console"),
                _middleAlign);

            var workRect = rect;

            if (consoleAlignment == ConsoleAlignment.Top)
            {
                workRect.yMin += consoleHeight;
            }
            else
            {
                workRect.yMax -= consoleHeight;
            }

            GUI.Box(GetAlignedRect(120, 50, Settings.Instance.ProfilerAlignment, workRect),
                new GUIContent(Resources.Load<Texture2D>(iconPath + "/profiler-25"), "Profiler"), _middleAlign);

            GUI.Box(GetAlignedRect(150, 36, Settings.Instance.OptionsAlignment, workRect),
                new GUIContent(Resources.Load<Texture2D>(iconPath + "/options-25"), "Pinned Options"), _middleAlign);

            if (Settings.Instance.EnableTrigger != Settings.TriggerEnableModes.Off)
            {
                GUI.Box(GetAlignedRect(25, 25, Settings.Instance.TriggerPosition, rect),
                    new GUIContent("", "Entry Trigger"),
                    _middleAlign);
            }
        }

        private static Rect GetAlignedRect(int width, int height, PinAlignment alignment, Rect workRect)
        {
            var rect = new Rect(0, 0, width, height);

            if (alignment == PinAlignment.BottomLeft || alignment == PinAlignment.BottomRight)
            {
                rect.position = new Vector2(0, workRect.height - rect.height);
            }

            if (alignment == PinAlignment.TopRight || alignment == PinAlignment.BottomRight)
            {
                rect.position += new Vector2(workRect.width - rect.width, 0);
            }

            rect.position += workRect.position;

            return rect;
        }

        public static void RenderGif(Rect pos, Texture2D map, int frameNo, int frameWidth, int frameHeight, int perLine,
            int paddingX = 0, int paddingY = 0)
        {
            var x = frameNo%perLine;
            var y = Mathf.FloorToInt((float) frameNo/perLine);

            var xCoord = x*(frameWidth + paddingX);
            var yCoord = (y + 1)*(frameHeight + paddingY);

            var texCoords = new Rect(
                xCoord/(float) map.width,
                (map.height - yCoord)/(float) map.height,
                (frameWidth)/(float) map.width,
                (frameHeight)/(float) map.height);

            GUI.DrawTextureWithTexCoords(pos, map, texCoords);

            //Debug.Log(texCoords);
            //Debug.Log("x: " + x + ", y: " + y);
        }

        public static void DrawFooterLayout(float width)
        {
            EditorGUILayout.BeginHorizontal();

            var margin = (EditorStyles.miniButton.padding.left)/2f;
            width = width - margin*2;

            if (GUILayout.Button("Web Site", GUILayout.Width(width/2f - margin)))
            {
                Application.OpenURL(SRDebugStrings.Current.SettingsWebSiteUrl);
            }

            if (GUILayout.Button("Asset Store Page", GUILayout.Width(width/2f - margin)))
            {
                Application.OpenURL(SRDebugStrings.Current.SettingsAssetStoreUrl);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Documentation", GUILayout.Width(width/2f - margin)))
            {
                Application.OpenURL(SRDebugStrings.Current.SettingsDocumentationUrl);
            }

            if (GUILayout.Button("Support", GUILayout.Width(width/2f - margin)))
            {
                Application.OpenURL(
                    SRDebugStrings.Current.SettingsSupportUrl);
            }

            EditorGUILayout.EndHorizontal();
        }

        public static class Styles
        {
            private static GUIStyle _inspectorHeaderStyle;
            private static GUIStyle _inspectorHeaderFoldoutStyle;
            private static GUIStyle _settingsHeaderBoxStyle;
            private static GUIStyle _headerLabel;
            private static GUIStyle _paragraphLabel;
            private static GUIStyle _radioButtonDescription;
            private static GUIStyle _radioButton;
            private static GUIStyle _leftToggleButton;
            private static GUIStyle _noPaddingNoMargin;
            private static GUIStyle _richTextLabel;

            public static string LinkColour
            {
                get
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        return "#7C8CB9";
                    }

                    return "#0032E6";
                }
            }

            public static GUIStyle InspectorHeaderStyle
            {
                get
                {
                    if (_inspectorHeaderStyle == null)
                    {
                        _inspectorHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
                        _inspectorHeaderStyle.fontSize = 12;
                    }

                    return _inspectorHeaderStyle;
                }
            }

            public static GUIStyle InspectorHeaderFoldoutStyle
            {
                get
                {
                    if (_inspectorHeaderFoldoutStyle == null)
                    {
                        _inspectorHeaderFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                        _inspectorHeaderFoldoutStyle.fontSize = 12;
                        _inspectorHeaderFoldoutStyle.fontStyle = FontStyle.Bold;
                    }

                    return _inspectorHeaderFoldoutStyle;
                }
            }

            public static GUIStyle SettingsHeaderBoxStyle
            {
                get
                {
                    if (_settingsHeaderBoxStyle == null)
                    {
                        _settingsHeaderBoxStyle = new GUIStyle("OL Title");
                        _settingsHeaderBoxStyle.padding = new RectOffset(0, 0, 0, 0);
                        _settingsHeaderBoxStyle.margin = new RectOffset(0, 0, 0, 0);
                        _settingsHeaderBoxStyle.clipping = TextClipping.Clip;
                        _settingsHeaderBoxStyle.overflow = new RectOffset(0, 0, 0, 0);
                        //_settingsHeaderBoxStyle.border = new RectOffset(1, 1, 1, 1);
                        _settingsHeaderBoxStyle.fixedHeight = 0.5f;
                    }

                    return _settingsHeaderBoxStyle;
                }
            }

            public static GUIStyle HeaderLabel
            {
                get
                {
                    if (_headerLabel == null)
                    {
                        _headerLabel = new GUIStyle(EditorStyles.largeLabel);
                        _headerLabel.fontSize = 18;
                        _headerLabel.fontStyle = FontStyle.Normal;
                        _headerLabel.margin = new RectOffset(5, 5, 5, 5);
                    }

                    return _headerLabel;
                }
            }

            public static GUIStyle ParagraphLabel
            {
                get
                {
                    if (_paragraphLabel == null)
                    {
                        _paragraphLabel = new GUIStyle(EditorStyles.label);
                        _paragraphLabel.margin = new RectOffset(5, 5, 5, 5);
                        _paragraphLabel.wordWrap = true;
                        _paragraphLabel.richText = true;
                    }

                    return _paragraphLabel;
                }
            }

            public static GUIStyle LeftToggleButton
            {
                get
                {
                    if (_leftToggleButton == null)
                    {
                        _leftToggleButton = new GUIStyle(EditorStyles.label);
                        _leftToggleButton.contentOffset = new Vector2(_leftToggleButton.contentOffset.x + 5,
                            _leftToggleButton.contentOffset.y);
                    }

                    return _leftToggleButton;
                }
            }

            public static GUIStyle RadioButton
            {
                get
                {
                    if (_radioButton == null)
                    {
                        _radioButton = new GUIStyle(EditorStyles.radioButton);
                        _radioButton.contentOffset = new Vector2(_radioButton.contentOffset.x + 5,
                            _radioButton.contentOffset.y);
                    }

                    return _radioButton;
                }
            }

            public static GUIStyle RadioButtonDescription
            {
                get
                {
                    if (_radioButtonDescription == null)
                    {
                        _radioButtonDescription = new GUIStyle(ParagraphLabel);
                        _radioButtonDescription.padding.left = (int) RadioButton.contentOffset.x +
                                                               RadioButton.padding.left;
                    }

                    return _radioButtonDescription;
                }
            }

            public static GUIStyle NoPaddingNoMargin
            {
                get
                {
                    if (_noPaddingNoMargin == null)
                    {
                        _noPaddingNoMargin = new GUIStyle();
                        _noPaddingNoMargin.margin = new RectOffset(0, 0, 0, 0);
                        _noPaddingNoMargin.padding = new RectOffset(0, 0, 0, 0);
                    }

                    return _noPaddingNoMargin;
                }
            }

            public static GUIStyle RichTextLabel
            {
                get
                {
                    if (_richTextLabel == null)
                    {
                        _richTextLabel = new GUIStyle(EditorStyles.label);
                        _richTextLabel.richText = true;
                        _richTextLabel.margin = new RectOffset(2, 2, 0, 0);
                    }

                    return _richTextLabel;
                }
            }
        }
    }
}

#endif
