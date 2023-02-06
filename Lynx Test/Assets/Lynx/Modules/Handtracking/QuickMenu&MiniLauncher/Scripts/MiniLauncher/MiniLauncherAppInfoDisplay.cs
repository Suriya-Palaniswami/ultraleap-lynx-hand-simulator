using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniLauncherAppInfoDisplay : MonoBehaviour
{
    //INSPECTOR
    [SerializeField] private TextMeshProUGUI appName;
    [SerializeField] private RawImage appIconImage;
    [SerializeField] private Image appIconImageBackground;

    //PRIVATE
    private string packageName;
    private string applicationName;
    private Texture2D iconTexture;


#if UNITY_ANDROID && !UNITY_EDITOR
    private void Start()
    {
            DisplayAppInfo();          
    }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnEnable()
    {
        DisplayAppInfo();
    }
#endif

    public void DisplayAppInfo()
    {
        //Debug.Log("----------- GetAppName()");

        applicationName = AndroidComMng.Instance().GetAppName();
        iconTexture = AndroidComMng.Instance().GetAppIconTexture();

        if (applicationName != null) appName.text = applicationName;
        else appName.text = "AppName";

        if (iconTexture != null)
        {
            appIconImage.texture = iconTexture;
            appIconImage.color = Color.white;
            appIconImageBackground.color = Color.black;
        }

    }

}
