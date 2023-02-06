using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(LynxToggle))]
public class LynxToggleEditor : ToggleEditor
{
    private static class Styles
    {
        public static GUIContent MoveDeltaText = EditorGUIUtility.TrTextContent("Move Delta", "Transition to apply on the current object");
        public static GUIContent MoveDuration = EditorGUIUtility.TrTextContent("Move Duration", "Duration for foward or backward press animation.");
        public static GUIContent IsUsingScale = EditorGUIUtility.TrTextContent("Is Using Scale", "If checked, the object will be affecte by its local scale for animation.");

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

        serializedObject.ApplyModifiedProperties();
    }
}
