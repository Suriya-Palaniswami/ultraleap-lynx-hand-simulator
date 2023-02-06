using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LynxText : MonoBehaviour
{
    [SerializeField] public List<Color> themes = new List<Color>();

    public void ChangeTheme(int index)
    {
        if (index <= themes.Count - 1) this.GetComponent<TextMeshProUGUI>().color = themes[index];
        else Debug.LogWarning("Index not found.");
    }

}
