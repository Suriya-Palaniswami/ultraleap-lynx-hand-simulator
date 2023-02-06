using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LynxMaterial : MonoBehaviour
{
    [SerializeField] public List<Material> themes = new List<Material>();

    public void ChangeTheme(int index)
    {
        if (index <= themes.Count - 1) this.GetComponent<Renderer>().material = themes[index];
        else Debug.LogWarning("Index not found.");
    }
}
