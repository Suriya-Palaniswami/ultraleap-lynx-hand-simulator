/**
 * @file LynxBuildSettings.cs
 * 
 * @author Geoffrey Marhuenda
 * 
 * @brief Automatically manage the Android settings to match with Lynx-R1 device.
 */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace lynx
{
    public class LynxBuildSettings
    {
        /// <summary>
        /// Configure project to target Lynx headset.
        /// </summary>
        public static void SetupAndroidBuild()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Android;

            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] { UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 });
            PlayerSettings.MTRendering = false;
            PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.colorSpace = ColorSpace.Linear;

            if (PlayerSettings.Android.disableDepthAndStencilBuffers)
                Debug.LogWarning("WARNING: Depth and Stencil is disabled. If screen stays black, you should enable it back.");
        }

        /// <summary>
        /// Retrieve all object even disabled from the scene.
        /// </summary>
        /// <typeparam name="T">Object type to retrieve.</typeparam>
        /// <returns>List of all T objects in the scene.</returns>
        public static List<T> FindObjectsOfTypeAll<T>()
        {
            List<T> results = new List<T>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                if (s.isLoaded)
                {
                    var allGameObjects = s.GetRootGameObjects();
                    for (int j = 0; j < allGameObjects.Length; j++)
                    {
                        var go = allGameObjects[j];
                        results.AddRange(go.GetComponentsInChildren<T>(true));
                    }
                }
            }
            return results;
        }
    }
}