
///******************************************************************************
///* Lynx R-1 Samples - LynxScrollview.cs (2022)                                *
///* Author : GC                                                                *
///* *************************************                                      *
///* Lynx interfaces samples.                                                   *
///* Script to manage a 3D Scrollview with UltraLeap UI.                        *
///******************************************************************************

using UnityEngine;
using Leap.Unity.Interaction;

public class LynxScrollview : MonoBehaviour
{
    [SerializeField] private InteractionSlider scrollBar;
    [SerializeField] private GameObject layout;

    private Vector3 startPosition;

    #region UNITY API
    // Start is called before the first frame update
    void Start()
    {
        startPosition = layout.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Scroll();
    }
    #endregion

    private void Scroll()
    {
        layout.transform.localPosition = new Vector3(layout.transform.localPosition.x, startPosition.y - scrollBar.HorizontalSliderValue, layout.transform.localPosition.z);
    }
}
