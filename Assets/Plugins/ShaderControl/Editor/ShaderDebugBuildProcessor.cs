using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace ShaderControl {

    class ShaderDebugBuildProcessor : IPreprocessShaders, IPreprocessComputeShaders, IPostprocessBuildWithReport {

        ShadersBuildInfo shadersBuildInfo;

        const string dummyGOName = "ShaderControlQuickBuild";

        // In Unity 2022, OnProcessShader won't be called unless there's a change in the scene so we create a dummy change
        public static bool CreateDummyQuickBuildGO(bool justUpdate) {
#if UNITY_2022_1_OR_NEWER
            GameObject go = GameObject.Find(dummyGOName);
            if (go == null) {
                if (justUpdate) return false;
                go = new GameObject(dummyGOName);
                go.hideFlags = HideFlags.HideInHierarchy;
            }
            go.transform.position += Vector3.one;
            Scene scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
#endif
            return true;
        }

        public static ShadersBuildInfo CheckShadersBuildStore(ShadersBuildInfo shadersBuildInfo) {

            if (shadersBuildInfo == null) {
                string filename = GetStoredDataPath();
                shadersBuildInfo = AssetDatabase.LoadAssetAtPath<ShadersBuildInfo>(filename);
                if (shadersBuildInfo != null) {
                    return shadersBuildInfo;
                }
            }

            // Check if scriptable object exists
            string path = GetStoredDataPath();
            if (!File.Exists(path)) {
                string dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir);
                shadersBuildInfo = ScriptableObject.CreateInstance<ShadersBuildInfo>();
                AssetDatabase.CreateAsset(shadersBuildInfo, path);
                AssetDatabase.SaveAssets();
            }
            return shadersBuildInfo;
        }


        public void OnPostprocessBuild(BuildReport report) {
            SaveResults();
        }

        public int callbackOrder { get { return 0; } }

        static string GetStoredDataPath() {
            // Locate shader control path
            string[] paths = AssetDatabase.GetAllAssetPaths();
            for (int k = 0; k < paths.Length; k++) {
                if (paths[k].EndsWith("/ShaderControl/Editor", StringComparison.InvariantCultureIgnoreCase)) {
                    return paths[k] + "/Resources/BuiltShaders.asset";
                }
            }
            return null;
        }

        void SaveResults() {

            if (SCWindow.GetEditorPrefBool("QUICK_BUILD", false))
            {
                SCWindow.SetEditorPrefBool("QUICK_BUILD", false);
#if UNITY_2022_1_OR_NEWER
                SCWindow.SetEditorPrefBool("DELAY_SCENE_UPDATE", true);
#endif
            }

            if (shadersBuildInfo != null) {
                shadersBuildInfo.creationDateTicks = DateTime.Now.Ticks;
                EditorUtility.SetDirty(shadersBuildInfo);
                string filename = GetStoredDataPath();
                if (filename == null) {
                    Debug.LogError("Shader Control path not found.");
                } else {
                    AssetDatabase.SaveAssets();
                }
            }
            SCWindow.issueRefresh = 0;
        }

        public void OnProcessShader(
            Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerData) {

            try {
                bool skipCompilation = false;
                if (SCWindow.GetEditorPrefBool("QUICK_BUILD", false)) {
                    skipCompilation = true;
                }

                if (shadersBuildInfo == null) {
                    string filename = GetStoredDataPath();
                    shadersBuildInfo = AssetDatabase.LoadAssetAtPath<ShadersBuildInfo>(filename);
                    if (shadersBuildInfo == null) {
                        return;
                    }
                }

                ShaderBuildInfo sb = shadersBuildInfo.GetShader(shader.name);
                if (sb == null) {
                    sb = new ShaderBuildInfo();
                    sb.name = shader.name;
                    sb.simpleName = SCShader.GetSimpleName(sb.name);
                    sb.path = AssetDatabase.GetAssetPath(shader);
                    sb.isInternal = string.IsNullOrEmpty(sb.path) || !File.Exists(sb.path);
                    shadersBuildInfo.Add(sb);
                    EditorUtility.SetDirty(shadersBuildInfo);
                } else if (!sb.includeInBuild) {
                    skipCompilation = true;
                }

                int count = shaderCompilerData.Count;
                for (int i = 0; i < count; ++i) {
                    ShaderKeywordSet ks = shaderCompilerData[i].shaderKeywordSet;
                    ShaderKeyword[] shaderKeywords = ks.GetShaderKeywords();

                    // Check if variants are allowed
                    if (shaderKeywords.Length > 0 && sb.variants != null && sb.variants.Count > 0) {
                        bool includedVariant = false;
                        foreach (var variant in sb.variants) {
                            if (variant.Same(shader, shaderKeywords)) {
                                includedVariant = true;
                                break;
                            }
                        }
                        if (!includedVariant) {
                            shaderCompilerData.RemoveAt(i);
                            count--;
                            i--;
                            continue; // for
                        }
                    }

                    // Check if keywords are allowed
                    foreach (ShaderKeyword kw in shaderKeywords) {
#if UNITY_2021_2_OR_NEWER
                        string kname = kw.name;
#elif UNITY_2019_3_OR_NEWER
                    string kname = ShaderKeyword.GetKeywordName(shader, kw);
#elif UNITY_2018_4_OR_NEWER
                        string kname = kw.GetKeywordName();
#else
                        string kname = kw.GetName();
#endif
                        if (string.IsNullOrEmpty(kname)) {
                            continue;
                        }
                        if (!sb.KeywordsIsIncluded(kname)) {
                            shaderCompilerData.RemoveAt(i);
                            count--;
                            i--;
                            break;
                        } else {
                            EditorUtility.SetDirty(shadersBuildInfo);
                        }
                    }
                }

                if (skipCompilation) {
                    shaderCompilerData.Clear();
                    return;
                }
            } catch (Exception ex) {
                Debug.LogWarning("Shader Control detected an error during compilation of one shader: " + ex.ToString());
            }

        }

        // during quick builds, exclude compute shaders as well for faster compilation
        void IPreprocessComputeShaders.OnProcessComputeShader(ComputeShader shader, string kernelName, IList<ShaderCompilerData> shaderCompilerData)
        {
            if (SCWindow.GetEditorPrefBool("QUICK_BUILD", false))
            {
                shaderCompilerData.Clear();
            }

        }
    }
}