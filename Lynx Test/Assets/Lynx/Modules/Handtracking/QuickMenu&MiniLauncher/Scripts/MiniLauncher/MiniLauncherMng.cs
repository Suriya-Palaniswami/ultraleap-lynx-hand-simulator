using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniLauncherMng : MonoBehaviour
{

    public bool active = false;


    private void Start()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
        active = (gameObject.activeSelf);
    }


    public void ActivateMiniLauncher()
    {
        if (gameObject.GetComponent<XRUIPositionner>() != null) gameObject.GetComponent<XRUIPositionner>().RecenterPanel();
        gameObject.SetActive(true);
        active = true;
        //GetComponentInChildren<MiniLauncherAppInfoDisplay>().DisplayAppInfo();
    }
    public void DeactivateMiniLauncher()
    {
        gameObject.SetActive(false);
        active = false;
    }

    public void Resume()
    {
        DeactivateMiniLauncher();
    }
    public void Quit()
    {
        CallLynxLauncherMng callLynxLauncherMng = gameObject.GetComponentInParent<CallLynxLauncherMng>();
        callLynxLauncherMng.BackToLauncher();
    }


}
