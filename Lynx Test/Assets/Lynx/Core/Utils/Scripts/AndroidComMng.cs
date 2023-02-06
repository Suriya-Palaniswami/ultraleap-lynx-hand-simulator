using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AndroidComMng : MonoBehaviour
{
    //PUBLIC
    [HideInInspector] public UnityEvent mAndroidSystemComPlugInInitSucceedEvent;
    [HideInInspector] public UnityEvent mAndroidSystemComPlugInInitFailedEvent;

    [HideInInspector] public CustomUnityIntEvent mAudioVolumeChangeEvent = null;
    [HideInInspector] public CustomUnityIntEvent mBatteryLevelChangeEvent = null;


    //PRIVATE
    private const string LynxAndroidSystemComPlugInName = "com.lynx.lynxandroidsystemcom.LynxAndroidSystemComMng";

    private AndroidJavaClass mAndroidSystemComPlugIn = null;
    private AndroidJavaObject mCurrentActivity = null;
    private AndroidJavaObject mApplicationContext = null;

    //Singleton
    private static AndroidComMng AndroidComMngInstance = null;
    public static AndroidComMng Instance()
    {
        if (!AndroidComMngInstance)
        {
            AndroidComMngInstance = FindObjectOfType(typeof(AndroidComMng)) as AndroidComMng;
            if (!AndroidComMngInstance)
            {
                Debug.LogError("There needs to be one active AndroidComMng script on a GameObject in your scene.");
            }
        }
        return AndroidComMngInstance;
    }



    private void Awake()
    {
        if (mAndroidSystemComPlugInInitSucceedEvent == null)
            mAndroidSystemComPlugInInitSucceedEvent = new UnityEvent();

        if (mAndroidSystemComPlugInInitFailedEvent == null)
            mAndroidSystemComPlugInInitFailedEvent = new UnityEvent();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Start()
    {
        AndroidInit();

    }
#endif   


    //Android Initialization
    private void AndroidInit()
    {
        // Important : Initialisation of the java plugin :
        InitAndroidJavaPlugin();

        // Pass the Unity object name to the plug in to send Message from java to Unity
        mAndroidSystemComPlugIn.CallStatic("setUnityGameObjectName", gameObject.name);

        if (mAudioVolumeChangeEvent == null)
            mAudioVolumeChangeEvent = new CustomUnityIntEvent();

        if (mBatteryLevelChangeEvent == null)
            mBatteryLevelChangeEvent = new CustomUnityIntEvent();

        mAndroidSystemComPlugIn.CallStatic("registerChangesReceivers", mCurrentActivity, mApplicationContext);
    }
    private void InitAndroidJavaPlugin()
    {
        try
        {
            //Set Android  plugin
            var plugin = new AndroidJavaClass(LynxAndroidSystemComPlugInName);
            if (plugin != null)
            {
                mAndroidSystemComPlugIn = plugin;
                string libJarVersion = mAndroidSystemComPlugIn.CallStatic<string>("getVersion");
                Debug.Log("LynxAndroidSystemComMng was correctly initialized and its version is " + libJarVersion);
                Debug.Log(" ");
            }
            else
            {
                Debug.LogError("mAndroidSystemComPlugIn is NULL");
                return;
            }

            //Set unityPlayer
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (unityPlayer == null)
            {
                Debug.LogError("unityPlayer is NULL");
                return;
            }

            //Set CurrentActivity
            mCurrentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (mCurrentActivity == null)
            {
                Debug.LogError("CurrentActivity is NULL");
                return;
            }

            //Set ApplicationContext
            mApplicationContext = mCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
            if (mApplicationContext == null)
            {
                Debug.LogError("ApplicationContext is NULL");
                return;
            }

            //If all Sets succeeded, invoke Success Event
            if (mAndroidSystemComPlugInInitSucceedEvent != null)
            {
                mAndroidSystemComPlugInInitSucceedEvent.Invoke();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e, this);
            Debug.Log("----------- Init AndroidSystemComPlugIn FAILED");

            // tell the user that the plug in init fails : 
            if (mAndroidSystemComPlugInInitFailedEvent != null)
            {
                mAndroidSystemComPlugInInitFailedEvent.Invoke();
            }
        }
    }

    //CALLBACKS sent from Java Plugin :
    public void AudioVolumeChange(string volume)
    {
        // Volume sent varies from 0 to 15. 
        //Debug.Log("------------------- VolumeChange(string volume) received in OSComMng = " + volume);
        int iVolume = 0;
        int.TryParse(volume, out iVolume);
        mAudioVolumeChangeEvent.Invoke(iVolume);
    }
    public void BatteryLevelChange(string batteryLevel)
    {
        // Battery level varies from 0 to 100. it's a percentage 
        //Debug.Log("------------------- BatteryLevelChange(string batteryLevel) received in OSComMng = " + batteryLevel);
        int iBatteryLevel = 0;
        int.TryParse(batteryLevel, out iBatteryLevel);
        mBatteryLevelChangeEvent.Invoke(iBatteryLevel);
    }
    

    //Current App Data Getters
    public string GetAppName()
    {
        //Debug.Log("----------- GetAppName()");
        string appName = Application.productName;
        return appName;
    }
    public SByte[] GetAppIconBytes()
    {
        //Debug.Log("----------- GetAppName()");
        string appPackageName = Application.identifier;

        int flag = new AndroidJavaClass("android.content.pm.PackageManager").GetStatic<int>("GET_META_DATA");
        AndroidJavaObject pm = mCurrentActivity.Call<AndroidJavaObject>("getPackageManager");

        SByte[] decodedBytes = mAndroidSystemComPlugIn.CallStatic<SByte[]>("GetIcon", pm, appPackageName);

        if (decodedBytes == null) // Cedric : it could happens
        {
            Debug.LogWarning("------------- No App icon for package name : " + appPackageName);
        }

        return decodedBytes;
    }
    public Texture2D GetAppIconTexture()
    {
        SByte[] decodedBytes = GetAppIconBytes();
        if(decodedBytes == null) return null;

        Texture2D texture2D = null;
        byte[] data;

        texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        data = Array.ConvertAll(decodedBytes, (a) => (byte)a);
        texture2D.LoadImage(data);
        return texture2D;
    }

    //Battery_Mng
    public int GetBatteryLevel()
    {
        if (mAndroidSystemComPlugIn == null)
        {
            return 0;
        }

        return mAndroidSystemComPlugIn.CallStatic<int>("getBatteryPercentage", mApplicationContext);
    }
    
    //Device_Audio_Mng
    public int GetAudioVolume()
    {
        return mAndroidSystemComPlugIn.CallStatic<int>("getAudioVolume", mApplicationContext);
    }
    public int GetMaxAudioVolume()
    {
        return mAndroidSystemComPlugIn.CallStatic<int>("getMaxAudioVolume", mApplicationContext);
    }
    public void SetAudioVolume(int volume) // volume is 0 to 15. (integer)
    {
        if (volume < 0 || volume > 15)
        {
            Debug.LogWarning("Try to set an audio volume not between 0 to 15, setAudioVolume will be not called");
            return;
        }

        mAndroidSystemComPlugIn.CallStatic("setAudioVolume", mApplicationContext, volume);
    }
    public void SetMicrophoneMute(bool mute)
    {
        mAndroidSystemComPlugIn.CallStatic("setMicrophoneMute", mApplicationContext, mute);
    }
    public bool isMicrophoneMute()
    {
        return mAndroidSystemComPlugIn.CallStatic<bool>("isMicrophoneMute", mApplicationContext);
    }

}
