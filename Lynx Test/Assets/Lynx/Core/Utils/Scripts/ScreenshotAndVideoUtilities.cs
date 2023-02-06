using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine.Video;

/// <summary>
/// Lynx Utilities to create screenshots.
/// </summary>
public class ScreenshotAndVideoUtilities : MonoBehaviour
{
    //INSPECTOR
    [Header("The camera object that takes screenshots")]
    public GameObject cameraGameObjectForScreenShot;

    //PRIVATE
    private static string m_screenshotsFolderPath;
    //private RawImage rawImage = null; // Loaded image

#if !UNITY_EDITOR && UNITY_ANDROID
    // Path the lynx shared folder where screenshots and video are stored. 
    private static string mLYNX_GALLERY_PATH = "/../../../../DCIM/Lynx/ScreenAndVideoShots";
#endif



    public void TakeShot()
    {
        //screenshot not ready for android 12
        //GetGalleryPath();
        //TakeScreenShot(1024, 1024);
    }
    public static string GetGalleryPath()
    {
        string galleryFullPath;

#if !UNITY_EDITOR && UNITY_ANDROID
        galleryFullPath = Application.persistentDataPath + mLYNX_GALLERY_PATH; 
        //Debug.Log("galleryFullPath on Android : " + galleryFullPath);// typically /storage/emulated/0/Android/data/com.a.LynxAppStartTest/files/
#else
        //string applicationPath = Application.dataPath; // typically on Windows the Assets folder
        galleryFullPath = Application.dataPath + "/ScreenAndVideoShots";
#endif

        if (!System.IO.Directory.Exists(galleryFullPath))
        {
            Debug.Log("***** ScreenshotAndVideoUtilities::GetGalleryPath : Create Lynx folder because it doesn't exist");
            System.IO.Directory.CreateDirectory(galleryFullPath);
        }

        m_screenshotsFolderPath = galleryFullPath;
        return galleryFullPath;
    }
    /// <summary>
    /// Create a screenshot with given dimensions and save it with the day time.
    /// </summary>
    /// <param name="width">Width for the screenshot.</param>
    /// <param name="height">Height for the screenshot.</param>
    /// <returns>Saved file name.</returns>
    public void TakeScreenShot(int resWidth, int resHeight)
    {
        Debug.Log("****** ScreenshotAndVideoUtilities::TakeScreenShot begins");

        RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {      
            cameraGameObjectForScreenShot.SetActive(true);

            if (cameraGameObjectForScreenShot != null)
            {
                //Debug.Log("goCamera find = " + cameraGameObjectForScreenShot.name);
                Camera cameraForScreenshot = cameraGameObjectForScreenShot.GetComponent<Camera>();

                if (cameraForScreenshot != null)
                {
                    mainCamera = cameraForScreenshot;
                }
                else
                {
                    Debug.LogWarning("No camera object found on Eye Left object");
                    return;
                }
            }
        }

        mainCamera.targetTexture = renderTexture;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        mainCamera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        byte[] bytes = screenShot.EncodeToPNG();

        string filename = ComputeScreenShotPath();

        System.IO.File.WriteAllBytes(filename, bytes);

        // cedric : change 05 septembre 2022 for Open XR version
        // don't desactivate the camera. it's no more the mono camera like on SVR, it's now on Open XR version the main running camera. 
        //cameraGameObjectForScreenShot.SetActive(false);

        Debug.Log("******** ScreenshotAndVideoUtilities::New screenshot taken with path : " + filename);
    }

    /// <summary>
    /// Create a screenshot file name based "timestamp".
    /// </summary>
    /// <returns>Saved file name.</returns>
    public static string ComputeScreenShotPath()
    {     
        string filename = string.Format("{0}/screen_{1}.png",
                                    m_screenshotsFolderPath,
                                    System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                                   
        //Debug.Log("-------------- ScreenShot filename " + filename);
        
        return filename;
    }
    /// <summary>
    /// Create a video file name based on "timestamp".
    /// </summary>
    /// <returns>Saved file name.</returns>
    public static string ComputeVideoShotPath()
    {
        string filename = string.Format("{0}/video_{1}.mp4",
                                    m_screenshotsFolderPath,
                                    System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

        //Debug.Log("-------------- VideoShot filename " + filename);

        return filename;
    }


    // Marche :
    public void MyGetThumbnailVideo()
    {
        /*
        string folder = Application.dataPath;
#if !UNITY_EDITOR && UNITY_ANDROID
        folder = Application.persistentDataPath;
#endif
        folder = folder + "/Screenshots";

        string videoPath = folder + "/theVideo.mp4";

        Texture2D text = NativeGallery.GetVideoThumbnail(videoPath);

        if (text != null)
            Debug.Log("texture = " + text);
        else
            Debug.Log("texture == null");

        rawImage.texture = text;
        */
    }
}
