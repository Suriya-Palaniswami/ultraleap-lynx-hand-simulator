using UnityEngine;
//#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
//#endif

public class LynxAndroidPermission : MonoBehaviour
{
    private static LynxAndroidPermission LynxAndroidPermissionInst = null; //Reference to the LynxAndroidPermissionInst, to make sure it's been included

    public static LynxAndroidPermission Instance()
    {
        if (!LynxAndroidPermissionInst)
        {
            LynxAndroidPermissionInst = FindObjectOfType(typeof(LynxAndroidPermission)) as LynxAndroidPermission;
            if (!LynxAndroidPermissionInst)
            {
                Debug.LogError("There needs to be one active LynxAndroidPermission script on a GameObject in your scene.");
            }
        }
        return LynxAndroidPermissionInst;
    }

    private void Start()
    {
        // cedric 02 Novembre 2020 : verify permission at start
        //VerifyLocationPermission();

        //Debug.Log("------------- before VerifyLocationPermission askedddddddddddddddd");

        VerifyWriteExternalStoragePermission();

    }

    public bool VerifyLocationPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            RequestLocationPermission();

            /*
            DialogBox3DMng dialogBox3DMng = DialogBox3DMng.Instance();

            string dlgTitle = LocalizationHelper.GetLocalizedString("AUT_LOCATION_PERMISSION_TITLE");
            string dlgMsg   = LocalizationHelper.GetLocalizedString("AUT_LOCATION_PERMISSION");

            dialogBox3DMng.SetTypeAndMessage(DialogBox3DMng.Type.YesNo, dlgTitle, dlgMsg);
            dialogBox3DMng.SetYesCallBack(this.gameObject, "RequestLocationPermission");
            dialogBox3DMng.SetNoCallBack(null, null);
            dialogBox3DMng.Show();
            */


            return false;
        }
        
        Debug.Log("Location permission already granted");
        return true;
#else
        return true;
#endif
    }


    public bool VerifyWriteExternalStoragePermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR

        Debug.Log("----------- in VerifyWriteExternalStoragePermission()");


        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            RequestWriteExternalStoragePermission();
            return false;
        }
        
        Debug.Log("External Storage Write permission already granted");
        return true;
#else
        return true;
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void RequestLocationPermission()
    {
        Permission.RequestUserPermission(Permission.FineLocation);

    }

    private void RequestWriteExternalStoragePermission()
    {
        Permission.RequestUserPermission(Permission.ExternalStorageWrite);
    }
#endif


    /*
    void OnApplicationFocus(bool value)
    {
        if (value == false)
        {
            Debug.Log("value = false dans focus");
        }
        else
        {
            Debug.Log("value = true dans focus");
        }

        
        if (Permission.HasUserAuthorizedPermission("Your permission string"))
        {
            Debug.Log("Permission Allowed");
        }
        else
        {
            Debug.Log("Permission Denied");
        }
        
    }
    */
}