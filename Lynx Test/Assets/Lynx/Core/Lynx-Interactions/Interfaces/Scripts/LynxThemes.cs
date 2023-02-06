using System.Collections;
using System.Collections.Generic;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class LynxThemes : MonoBehaviour
{

    public int themeIndex = 0;

    //Singleton
    public static LynxThemes Instance { get; private set; } = null;
    protected LynxThemes() { }


    private void Awake()
    {
        Instance = this;
    }


    public void ChangeTheme(int index)
    {
        themeIndex = index;
        UpdateTheme();
    }

    public void UpdateTheme()
    {
        LynxButton[] lynxButtons = Resources.FindObjectsOfTypeAll<LynxButton>();
        foreach (LynxButton button in lynxButtons)
        {
            if (themeIndex < button.themes.Count) button.ChangeTheme(themeIndex);
            else Debug.LogWarning(button.name + " has not theme at index " + themeIndex);
        }

        LynxToggleButton[] lynxToggleButtons = Resources.FindObjectsOfTypeAll<LynxToggleButton>();
        foreach(LynxToggleButton button in lynxToggleButtons)
        {

            if (themeIndex < button.themes.Count) button.ChangeTheme(themeIndex);
            else Debug.LogWarning(button.name + " has not theme at index " + themeIndex);
        }

        LynxImage[] lynxImages = Resources.FindObjectsOfTypeAll<LynxImage>();
        foreach(LynxImage image in lynxImages)
        {
            if (themeIndex < image.themes.Count) image.ChangeTheme(themeIndex);
            else Debug.LogWarning(image.name + " has not theme at index " + themeIndex);
        }

        LynxSlider[] lynxSliders = Resources.FindObjectsOfTypeAll<LynxSlider>();
        foreach(LynxSlider slider in lynxSliders)
        {
            if (themeIndex < slider.themes.Count) slider.ChangeTheme(themeIndex);
            else Debug.LogWarning(slider.name + " has not theme at index " + themeIndex);
        }

        LynxDropdown[] lynxDropdowns = Resources.FindObjectsOfTypeAll<LynxDropdown>();
        foreach (LynxDropdown dropdown in lynxDropdowns)
        {
            if (themeIndex < dropdown.themes.Count) dropdown.ChangeTheme(themeIndex);
            else Debug.LogWarning(dropdown.name + " has not theme at index " + themeIndex);
        }

        LynxScrollbar[] lynxScrollbars = Resources.FindObjectsOfTypeAll<LynxScrollbar>();
        foreach (LynxScrollbar scrollbar in lynxScrollbars)
        {
            if (themeIndex < scrollbar.themes.Count) scrollbar.ChangeTheme(themeIndex);
            else Debug.LogWarning(scrollbar.name + " has not theme at index " + themeIndex);
        }

        LynxText[] lynxTexts = Resources.FindObjectsOfTypeAll<LynxText>();
        foreach (LynxText text in lynxTexts)
        {
            if (themeIndex < text.themes.Count) text.ChangeTheme(themeIndex);
            else Debug.LogWarning(text.name + " has not theme at index " + themeIndex);
        }

        LynxMaterial[] lynxMaterials = Resources.FindObjectsOfTypeAll<LynxMaterial>();
        foreach (LynxMaterial material in lynxMaterials)
        {
            if (themeIndex < material.themes.Count) material.ChangeTheme(themeIndex);
            else Debug.LogWarning(material.name + " has not theme at index " + themeIndex);
        }

        LynxButtonTimer[] lynxButtonTimers = Resources.FindObjectsOfTypeAll<LynxButtonTimer>();
        foreach (LynxButtonTimer button in lynxButtonTimers)
        {
            if (themeIndex < button.themes.Count) button.ChangeTheme(themeIndex);
            else Debug.LogWarning(button.name + " has not theme at index " + themeIndex);
        }

        LynxToggleSlider[] lynxToggleSliders = Resources.FindObjectsOfTypeAll<LynxToggleSlider>();
        foreach (LynxToggleSlider slider in lynxToggleSliders)
        {
            if (themeIndex < slider.themes.Count) slider.ChangeTheme(themeIndex);
            else Debug.LogWarning(slider.name + " has not theme at index " + themeIndex);
        }
    }
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(LynxThemes))]
class LynxThemesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(serializedObject.FindProperty("themeIndex"));
        if (GUILayout.Button("Test"))
        {
            LynxThemes.Instance.UpdateTheme();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
