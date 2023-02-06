/**
 * @file LynxGazeInputEditor.cs
 *
 * @author Geoffrey Marhuenda
 *
 * @brief Add lynx input gaze into Unity Editor menu.
 */
using UnityEditor;
using UnityEngine;

namespace lynx
{
    public class LynxGazeInputEditor
    {
        private const string STR_INPUT_GAZE = "Assets/Lynx/Modules/Gaze-Pointer/Prefabs/LynxGazePointerInput.prefab";

        [MenuItem("Lynx/Inputs/Gaze pointer", false, 220)]
        private static void AddGazePointer()
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>(STR_INPUT_GAZE), null) as GameObject;
        }
    }
}