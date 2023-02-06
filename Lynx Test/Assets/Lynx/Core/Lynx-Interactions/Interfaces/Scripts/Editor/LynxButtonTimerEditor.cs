
//   ==============================================================================
//   | Lynx HandTracking Sample : LynxInterfaces (2022)                           |
//   | Author : GC                                                                |
//   |======================================                                      |
//   | LynxButton Editor Script                                                   |
//   | Script to modify the inspector GUI of the LynxButton Script.               |
//   ==============================================================================

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(LynxButtonTimer))]
public class LynxButtonTimerEditor : ButtonEditor
{
    private static class Styles
    {
        public static GUIContent MoveDeltaText = EditorGUIUtility.TrTextContent("Move Delta", "Transition to apply on the current object");
        public static GUIContent MoveDuration = EditorGUIUtility.TrTextContent("Move Duration", "Duration for foward or backward press animation.");
        public static GUIContent IsUsingScale = EditorGUIUtility.TrTextContent("Is Using Scale", "If checked, the object will be affecte by its local scale for animation.");

        public static GUIContent TimerImageText = EditorGUIUtility.TrTextContent("Timer Image", "Duration image.");
        public static GUIContent DeltaTimeText = EditorGUIUtility.TrTextContent("Delta Time", "Duration to wait, until OnTimerPress event.");

        public static GUIContent OnPressText = EditorGUIUtility.TrTextContent("OnPress", "Event.");
        public static GUIContent OnUnpressText = EditorGUIUtility.TrTextContent("OnUnpress", "Event.");
        public static GUIContent OnTimerPressText = EditorGUIUtility.TrTextContent("OnTimerPress", "Event.");

        public static GUIContent ThemesText = EditorGUIUtility.TrTextContent("Themes", "To change the color theme of the buttons state.");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle bold = new GUIStyle();
        bold = EditorStyles.boldLabel;

        serializedObject.Update();

        EditorGUILayout.LabelField("Button Parameters", bold);
        EditorGUILayout.Space();
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Timer Button Parameters", bold);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_timerImage"), Styles.MoveDeltaText);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_deltaTime"), Styles.MoveDuration);
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("themes"), Styles.ThemesText);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Button Animation", bold);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_moveDelta"), Styles.MoveDeltaText);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_moveDuration"), Styles.MoveDuration);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isUsingScale"), Styles.IsUsingScale);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Lynx Events Button", bold);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnPress"), Styles.OnPressText);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnUnpress"), Styles.OnUnpressText);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("OnTimerPress"), Styles.OnTimerPressText);

        serializedObject.ApplyModifiedProperties();
    }
}
