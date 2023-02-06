/**
 * @file LynxHandtrackingEditor.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Add lynx handtracking feature into Unity Editor menu to help configuration and integration in the scene.
 */
using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

#if LYNX_XRI
using Leap.Unity.Preview.XRInteractionToolkit;
using Leap.Unity.Preview.HandRays;
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace lynx
{
    public class LynxHandtrackingEditor
    {
        private const string STR_LYNX_HANDS = "Lynx Hands.prefab";

        private const string STR_UL_INPUT_EVENT_SYSTEM = "UIInputEventSystem.prefab";
        private const string STR_XRI_DIRECT_INTERACTOR = "Direct Interactor.prefab";
        private const string STR_XRI_RAYCAST = "Ray Interactor.prefab";
        private const string STR_XRI_FAR_FIELD = "FarFieldDirection.prefab";

        /// <summary>
        /// Open Handtracking configuration window.
        /// </summary>
        [MenuItem("Lynx/Inputs/Handtracking/Add handtracking", false, 200)]
        public static void AddHandtracking()
        {
            EditorWindow window = EditorWindow.GetWindow(typeof(HandtrackingAddWindow));
            Rect r = window.position;
            r.position = new Vector2(150.0f, 150.0f);
            r.width = 400.0f;
            r.height = 250.0f;
            window.position = r;
        }

        /// <summary>
        /// Remove current handtracking from the scene.
        /// </summary>
        [MenuItem("Lynx/Inputs/Handtracking/Remove handtracking", false, 201)]
        public static void RemoveHandtracking()
        {
            // Remove handtracking
            List<LynxXRRig> leapRigs = LynxBuildSettings.FindObjectsOfTypeAll<LynxXRRig>();
            while (leapRigs.Count > 0)
            {
                string name = leapRigs[0].gameObject.name;
                GameObject.DestroyImmediate(leapRigs[0].gameObject);
                leapRigs.RemoveAt(0);
                Debug.Log($"{name} removed.");
            }
        }

        /// <summary>
        /// Add handtracking to the scene by adding the Lynx hands prefab and expcted components depending on which features should be integrate.
        /// </summary>
        /// <param name="isUnityEvent">Define if the hand tracking should use the Unity Event system (canvas in world space)</param>
        /// <param name="isDirectInteraction">Define if hand tracking will interact as OpenXR direct interactors (based on XR Interaction Toolkit).</param>
        /// <param name="isRaycast">Define if hand tracking will interact as OpenXR ray interactors (based on XR Interaction Toolkit).</param>
        public static void AddHandtracking(bool isUnityEvent, bool isDirectInteraction, bool isRaycast)
        {
            // Get handtracking prefab
            string str_leapRig = Directory.GetFiles(Application.dataPath, STR_LYNX_HANDS, SearchOption.AllDirectories)[0].Replace(Application.dataPath, "Assets/");
            GameObject leapRig = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(str_leapRig), null) as GameObject;
            leapRig.transform.parent = null;
            leapRig.transform.localPosition = Vector3.zero;

            LeapProvider provider = leapRig.GetComponentInChildren<LeapProvider>();

            InteractionHand[] interactionHands = leapRig.GetComponentsInChildren<InteractionHand>();
            for (int i = 0, count = interactionHands.Length; i < count; ++i)
                interactionHands[i].leapProvider = provider;

            ///// UNITY EVENTS
            if (isUnityEvent)
            {
                // UIInputEventSystem (for Unity events)
                string basePath = Application.dataPath.Replace("Assets", "");
                string[] str_uiInputEventSystemFiles = Directory.GetFiles(basePath, STR_UL_INPUT_EVENT_SYSTEM, SearchOption.AllDirectories);
                string str_uiInputEventSystem = string.Empty;

                if (str_uiInputEventSystemFiles[0].Contains("PackageCache"))
                {
                    str_uiInputEventSystem = "Packages/com.ultraleap.tracking.preview/UI Input/Runtime/Prefabs/UIInputEventSystem.prefab"; // [TODO] Dirty --> I should fix this
                }
                else
                    str_uiInputEventSystem = str_uiInputEventSystemFiles[0].Replace(Application.dataPath, "Assets/");



                GameObject uiInputEventSystem = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(str_uiInputEventSystem), null) as GameObject;
                uiInputEventSystem.transform.parent = leapRig.transform;
                Leap.Unity.InputModule.UIInputModule uiInputModule = uiInputEventSystem.GetComponent<Leap.Unity.InputModule.UIInputModule>();
                uiInputModule.MainCamera = Camera.main;
                uiInputModule.LeapDataProvider = provider;

                // Disable default Event system since it's not working with UIInputEventSystem
                List<EventSystem> eventSystems = LynxBuildSettings.FindObjectsOfTypeAll<EventSystem>();
                for (int i = 0, count = eventSystems.Count; i < count; ++i)
                {
                    if (eventSystems[i].gameObject.GetComponent<Leap.Unity.InputModule.UIInputModule>() == null)
                    {
                        if (eventSystems[i].enabled)
                        {
                            eventSystems[i].gameObject.SetActive(false);
                            Debug.Log($"Disabled EventSystem on {eventSystems[i].gameObject.name}");
                        }
                    }
                }
            }

#if LYNX_XRI
            // Add expected interactors based on Ultraleap changes.
            if (isDirectInteraction || isRaycast)
            {
                // Find XR Interaction Manager or create one
                List<XRInteractionManager> xrManagers = LynxBuildSettings.FindObjectsOfTypeAll<XRInteractionManager>();
                XRInteractionManager xrManager = null;
                if (xrManagers.Count > 0)
                    xrManager = xrManagers[0];
                else
                {
                    GameObject xrManagerObject = new GameObject("XR Interaction Manager");
                    xrManagerObject.transform.parent = leapRig.transform;
                    xrManager = xrManagerObject.AddComponent<XRInteractionManager>();
                }

                HandModelBase[] hands = leapRig.GetComponentsInChildren<HandModelBase>();

                // XR DIRECT INTERACTORS
                if (isDirectInteraction)
                {
                    string str_directInteractor = Directory.GetFiles(Application.dataPath, STR_XRI_DIRECT_INTERACTOR, SearchOption.AllDirectories)[0].Replace(Application.dataPath, "Assets/");
                    for (int i = 0, count = hands.Length; i < count; ++i)
                    {
                        GameObject interactor = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(str_directInteractor), null) as GameObject;
                        interactor.transform.parent = leapRig.transform;
                        interactor.GetComponent<XRDirectInteractor>().interactionManager = xrManager;
                        TrackedHandsController trackedHands = interactor.GetComponent<TrackedHandsController>();
                        trackedHands.chirality = hands[i].Handedness;
                        trackedHands.leapProvider = provider;
                        string nameSufix = hands[i].Handedness == Chirality.Left ? "Left" : "Right";
                        interactor.gameObject.name += $" {nameSufix}";
                    }
                }

                // RAYCAST INTERACTORS
                if (isRaycast)
                {

                    // Add raycast interactors
                    string str_raycastInteractor = Directory.GetFiles(Application.dataPath, STR_XRI_RAYCAST, SearchOption.AllDirectories)[0].Replace(Application.dataPath, "Assets/");
                    for (int i = 0, count = hands.Length; i < count; ++i)
                    {
                        GameObject interactor = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(str_raycastInteractor), null) as GameObject;
                        interactor.transform.parent = leapRig.transform;
                        interactor.GetComponent<XRRayInteractor>().interactionManager = xrManager;
                        TrackedHandsController trackedHands = interactor.GetComponent<TrackedHandsController>();
                        trackedHands.chirality = hands[i].Handedness;
                        trackedHands.leapProvider = provider;
                        string nameSufix = hands[i].Handedness == Chirality.Left ? "Left" : "Right";
                        interactor.gameObject.name += $" {nameSufix}";


                        // Add Far Field component
                        string basePath = Application.dataPath.Replace("Assets", "");
                        string[] str_farFieldFiles = Directory.GetFiles(basePath, STR_XRI_FAR_FIELD, SearchOption.AllDirectories);
                        string str_farField = string.Empty;

                        if (str_farFieldFiles[0].Contains("PackageCache"))
                        {
                            str_farField = "Packages/com.ultraleap.tracking.preview/XR Interaction Toolkit Integration/Runtime/FarFieldDirection.prefab"; // [TODO] Dirty --> I should fix this
                        }
                        else
                            str_farField = str_farFieldFiles[0].Replace(Application.dataPath, "Assets/");

                        GameObject farFieldGo = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(str_farField), null) as GameObject;
                        farFieldGo.transform.parent = leapRig.transform;
                        farFieldGo.transform.position = Vector3.zero;
                        farFieldGo.gameObject.name += $" {nameSufix}";

                        WristShoulderFarFieldHandRay wristShoulder = farFieldGo.GetComponent<WristShoulderFarFieldHandRay>();
                        wristShoulder.leapProvider = provider;
                        wristShoulder.chirality = hands[i].Handedness;
                    }
                }
            }
#endif

            Debug.Log("Handtracking added.");

        }


#if LYNX_XRI
        /// <summary>
        /// For optimization, auto map object missing XR Interaction Manager in its components.
        /// </summary>
        [MenuItem("Lynx/Inputs/Auto-mapping/XR interactables", false, 210)]
        public static void AutomapXRInteractionManager()
        {
            List<XRInteractionManager> xrManagers = LynxBuildSettings.FindObjectsOfTypeAll<XRInteractionManager>();
            XRInteractionManager xrManager = null;
            if (xrManagers.Count > 0)
            {
                xrManager = xrManagers[0];
                List<XRGrabInteractable> interactbales = LynxBuildSettings.FindObjectsOfTypeAll<XRGrabInteractable>();
                foreach (XRGrabInteractable i in interactbales)
                    i.interactionManager = xrManager;
            }
            else
                Debug.LogError("There is no XRInteractionManager in the scene. Please use XR > Interaction Manager");
        }
#endif

        /// <summary>
        /// Window asking the user what features to use with the handtracking.
        /// </summary>
        public class HandtrackingAddWindow : EditorWindow
        {
            bool cbUnityEvents = true;
            bool cbDirectInteractor = false;
            bool cbRaycasting = false;

            void OnGUI()
            {
                GUILayout.Space(20);
                GUILayout.Label("Add handtracking to the scene.\nPlease define which interaction types you want to use.", EditorStyles.label);

                GUILayout.Space(20);
                cbUnityEvents = EditorGUILayout.Toggle("Unity events", cbUnityEvents);
#if LYNX_XRI
                cbDirectInteractor = EditorGUILayout.Toggle("XR Direct interactors", cbDirectInteractor);
                cbRaycasting = EditorGUILayout.Toggle("XR raycast interactors", cbRaycasting);
#endif

                GUILayout.Space(20);
                if (GUILayout.Button("Add"))
                {
                    AddHandtracking(cbUnityEvents, cbDirectInteractor, cbRaycasting);
                    this.Close();
                }

                if (GUILayout.Button("Cancel"))
                {
                    Debug.Log("Cancelled");
                    this.Close();
                }
            }
        }

    }
}