/**
 * @file OpenXRConfiguration.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Add lynx menu in Unity Editor to manage OpenXR packages installation and configuration.
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using static UnityEditor.EditorApplication;

#if LYNX_OPENXR
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
using UnityEditor.XR.OpenXR.Features;
using UnityEditor.XR.Management.Metadata;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Interactions;
#endif

namespace lynx
{
    [InitializeOnLoad]
    public class OpenXRConfiguration
    {

        #region PUBLIC CONST VARIABLES
        public const string LYNX_OPENXR_SYMBOL_STR = "LYNX_OPENXR";
        public const string XR_OPENXR_PCKG_STR = "com.unity.xr.openxr@1.5.1";
        public const string XR_INTERACTION_PCKG_STR = "com.unity.xr.interaction.toolkit@2.1.1";
        public const string TMP_PCKG_STR = "com.unity.textmeshpro";
        #endregion

        #region PRIVATES VARIABLES
        private static AddRequest addRequest = null;
        private static RemoveRequest removeRequest = null;
        private static Queue<string> m_packages = new Queue<string>();
        private static bool m_bWaitingNext = true;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Packages name list to install/remove.
        /// </summary>
        public static string[] PackagesNames
        {
            get => m_packages.ToArray();

            set
            {
                m_packages.Clear();
                for (int i = 0, count = value.Length; i < count; ++i)
                    m_packages.Enqueue(value[i]);
            }
        }
        #endregion

        #region EDITOR MENUS
        [MenuItem("Lynx/Settings/Configure project settings", false, 100)]
        public static void ConfigureProject()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(ConfigurationWindow));
            Rect r = window.position;
            r.position = new Vector2(150.0f, 150.0f);
            r.width = 400.0f;
            r.height = 350.0f;
            window.position = r;
        }

        //[MenuItem("Lynx/Settings/Install packages", false, 101)]
        //public static void PackageInstaller()
        //{
        //    EditorWindow.GetWindow(typeof(PackagesInstallationWindow), true, "Project configuration", true);
        //}
        #endregion

        #region METHODS

        #region INSTALLATION HELPERS
        /// <summary>
        /// Install iteratively all packages from packages list names using request system.
        /// </summary>
        /// <param name="request">Request (AddRequest or RemoveRequest) used to check installation status.</param>
        /// <param name="nextPackageEvent">Method to use for next package to install (usually Client.Add() or Client.Remove()).</param>
        /// <param name="callbackFunction">Method to unsubscribe when the installation is over (avoid it to stay in editor update loop).</param>
        /// <param name="successPackageEvent">Event to fire when a package has been correctly installed/removed.</param>
        public static void Progress(Request request, Action<string> nextPackageEvent, CallbackFunction callbackFunction, Action successPackageEvent = null)
        {
            if (m_bWaitingNext)
            {
                if (m_packages.Count > 0) // Next element from the list
                {
                    m_bWaitingNext = false;
                    string packageName = m_packages.Dequeue();
                    nextPackageEvent.Invoke(packageName);
                }
                else // End
                {
                    EditorApplication.update -= callbackFunction;
                    m_bWaitingNext = true;

                    EditorApplication.UnlockReloadAssemblies();
                }
            }
            else if (request.IsCompleted)
            {
                if (request.Status == StatusCode.Success)
                {
                    successPackageEvent?.Invoke();
                    m_bWaitingNext = true;
                }
                else if (request.Status >= StatusCode.Failure)
                {
                    Debug.Log(request.Error.message);
                    m_bWaitingNext = true;
                }
            }
        }

        /// <summary>
        /// Start to install all packages in the packages list.
        /// </summary>
        public static void ProgressAdd()
        {
            Progress(
                addRequest,
                (packageName) => addRequest = Client.Add(packageName),
                ProgressAdd,
                () => Debug.Log("Installed: " + addRequest.Result.packageId)
            );
        }

        /// <summary>
        /// Start to remove all packages in the packages list.
        /// </summary>
        public static void ProgressRemove()
        {
            Progress(
                removeRequest,
                (packageName) => removeRequest = Client.Remove(packageName),
                ProgressRemove,
                () => Debug.Log("Removed: " + removeRequest.PackageIdOrName)
            );
        }
        #endregion

#if LYNX_OPENXR
        public static void ConfigureOpenXR()
        {
            BuildTargetGroup buildTarget = BuildTargetGroup.Android;

            XRManagerSettings managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
            managerSettings.name = "Lynx settings manager";
            OpenXRLoader openXRLoader = ScriptableObject.CreateInstance<OpenXRLoader>();
            managerSettings.TrySetLoaders(new List<XRLoader>() { openXRLoader });

            XRGeneralSettings xrSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
            xrSettings.Manager = managerSettings;
            xrSettings.name = "Lynx Settings";

            // Use default Khronos controller
            FeatureHelpers.RefreshFeatures(buildTarget);
            UnityEngine.XR.OpenXR.Features.OpenXRFeature feature = FeatureHelpers.GetFeatureWithIdForBuildTarget(buildTarget, KHRSimpleControllerProfile.featureId);
            if (feature)
            {
                feature.enabled = true;
                Debug.Log($"{feature.name} selected.");
            }

            // Feature set
            OpenXRFeatureSetManager.FeatureSet lynxFeatureSet = OpenXRFeatureSetManager.GetFeatureSetWithId(buildTarget, openxr.LynxFeatureSet.featureId);
            if (lynxFeatureSet != null)
            {
                lynxFeatureSet.isEnabled = true;
                lynxFeatureSet.requiredFeatureIds = new string[] { openxr.LynxR1Feature.featureId, openxr.LynxFeatureSet.ultraleapFeatureId };
                Debug.Log($"{lynxFeatureSet.name} enabled.");
            }
            OpenXRFeatureSetManager.SetFeaturesFromEnabledFeatureSets(buildTarget);

            XRGeneralSettingsPerBuildTarget xrGeneralSettings = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
            xrGeneralSettings.SetSettingsForBuildTarget(buildTarget, xrSettings);

            // Try to enable OpenXR
            XRGeneralSettings xr = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(buildTarget);
            if (xr == null)
            {
                XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
                if(EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings))
                    xr = buildTargetSettings.SettingsForBuildTarget(buildTarget);
            }

            if (xr != null)
            {
                XRManagerSettings xrPlugin = xr.AssignedSettings;
                XRPackageMetadataStore.AssignLoader(xrPlugin, typeof(OpenXRLoader).FullName, buildTarget);
            }
            else
                Debug.LogWarning("OpenXR not enabled.\nYou can enable it manually from XR Plugin Manager under Project settings.");
        }
#endif

        public static void ConfigureProjectSettings()
        {

            // Manage Android part
            LynxBuildSettings.SetupAndroidBuild();

            // For OpenXR, linear color is required
            PlayerSettings.colorSpace = ColorSpace.Linear;

#if LYNX_OPENXR
            // Manage OpenXR part
            ConfigureOpenXR();
#endif
        }

        #endregion

        #region CUSTOM WINDOWS
        /// <summary>
        /// Window warning the user about all the changes on his project.
        /// On validate, it will automatically configure the project settings for the user.
        /// </summary>
        public class ConfigurationWindow : EditorWindow
        {
            void OnGUI()
            {
                GUILayout.Space(20);
                GUILayout.Label("This will automatically configure your project:\n\n" +
                    "- Target Android platform\n" +
                    "\to Android 10 (API Level 29)\n" +
                    "\to ARM64\n" +
                    "\to Disable Multithreaded Rendering for better performances\n" +
                    "\to Use OpenGLES graphic API\n" +
                    "\to Set Landscape left\n", EditorStyles.label);


                GUILayout.Space(20);
#if LYNX_OPENXR
                GUILayout.Label("Configure OpenXR for Android:\n" +
                    "\to Select OpenXR in XR Plugin Management\n" +
                    "\to Use default Khronos controller (to avoid warning)\n" +
                    "\to Select Lynx-R1 provider\n" +
                    "\to Select Ultraleap provider\n", EditorStyles.label);
#else
                GUILayout.Label("Cannot configure OpenXR automatically.\nOpenXR is missing.\n", EditorStyles.label);
#endif

                if (GUILayout.Button("Validate"))
                {
                    Debug.Log("Configuring project...");
                    ConfigureProjectSettings();
                    this.Close();
                }


                if (GUILayout.Button("Cancel"))
                {
                    this.Close();
                }

            }
        }

        /// <summary>
        /// Let the user install missing packages useful for Lynx SDK.
        /// </summary>
        public class PackagesInstallationWindow : EditorWindow
        {
            bool cbOpenXR = true;
            bool cbInteractionToolkit = false;
            bool cbTMP = false;

            void OnGUI()
            {
                GUILayout.Space(20);
                GUILayout.Label("Lynx headset requires an OpenXR environment to run.", EditorStyles.label);

                GUILayout.Space(20);
                GUILayout.Label("Packages requirement", EditorStyles.boldLabel);
                cbOpenXR = EditorGUILayout.Toggle("OpenXR", cbOpenXR);

                GUILayout.Space(20);
                GUILayout.Label("Optional packages", EditorStyles.boldLabel);
                cbTMP = EditorGUILayout.Toggle("Text Mesh Pro", cbTMP);
                //if (cbOpenXR)
                cbInteractionToolkit = EditorGUILayout.Toggle("XR Interaction Toolkit", cbInteractionToolkit);
                //else
                //    cbInteractionToolkit = false;


                GUILayout.Space(20);
                if (GUILayout.Button("Install selected."))
                {
                    //{
                    //    Debug.LogWarning("[LYNX] Feature broken for now.");
                    //    this.Close();
                    //    return;
                    //}

                    Debug.Log("Installing...");

                    List<string> packagesToInstall = new List<string>();
                    if (cbOpenXR)
                    {
                        packagesToInstall.Add(XR_OPENXR_PCKG_STR);
                    }

                    if (cbInteractionToolkit)
                        packagesToInstall.Add(XR_INTERACTION_PCKG_STR);

                    if (cbTMP)
                        packagesToInstall.Add(TMP_PCKG_STR);

                    PackagesNames = packagesToInstall.ToArray();

                    EditorApplication.LockReloadAssemblies();
                    m_bWaitingNext = true;
                    EditorApplication.update += ProgressAdd;

                    this.Close();
                }

                if (GUILayout.Button("Cancel"))
                {
                    Debug.Log("Cancelled");
                    this.Close();
                }
            }
        }
        #endregion
    }
}