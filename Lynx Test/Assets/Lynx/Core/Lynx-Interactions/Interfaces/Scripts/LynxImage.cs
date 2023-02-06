using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LynxImage : MonoBehaviour
{
    [SerializeField] public List<Color> themes = new List<Color>();

    public void ChangeTheme(int index)
    {
        if (index <= themes.Count - 1 && this.GetComponent<Image>() != null) 
            this.GetComponent<Image>().color = themes[index];
        else Debug.LogWarning("Index not found.");
    }
}
