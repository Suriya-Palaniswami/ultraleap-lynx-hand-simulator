using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CaptureVideoMng : MonoBehaviour
{
    // Public Properties
    public int      m_maxFrames = 5000;     // maximum number of frames you want to record in one video
    public int      m_frameRate = 25; // number of frames to capture per second

    // private 
    Texture2D mTextureVideoReceive = null;

    // important : size of one eye screen for lynx. 
    int mWidth  = 1536; 
    int mHeight = 1404;

    int mResizedWidth; 
    int mResizedHeight;

    int mSizeInBytes = 0;
      
    RenderTexture mRenderTexture; 
    Texture2D     mResizedTexture;


    // Lynx Android plug in part : 
    private const string     mLynxPluginInName = "com.lynx.lynxandroidsystemcom.LynxAndroidSystemComMng";
    private AndroidJavaClass mAndroidSystemComPlugIn = null;
    private AndroidJavaObject mCurrentActivity = null;
    private AndroidJavaObject mApplicationContext = null;

    // Serialize frame part 
    private bool mThreadIsProcessing;
    private bool mTerminateThreadWhenDone;
    private bool mProcessStarted = false;
    
    private string mTempFramePath;
   
    private List<byte[]> mFrameQueue;
    private int mSavingFrameNumber;

    // Timing Data
    private float mCaptureFrameTime;
    private float mLastFrameTime;
    private int   frameNumber;
    

    // The Encoder Thread
    private Thread mEncoderThread;

    // just to test perf :
    private System.Diagnostics.Stopwatch m_sw = new System.Diagnostics.Stopwatch();

    private Material PlaneMaterialForVideo;

    private void InitAndroidJavaPlugin()
    {
        try
        {
            Debug.Log("begin InitAndroidJavaPlugin");
           
            var plugin = new AndroidJavaClass(mLynxPluginInName);

            if (plugin != null)
            {
                mAndroidSystemComPlugIn = plugin;
            }
            else
            {
                Debug.LogError("mAndroidSystemComPlugIn is NULL");
                return;
            }

            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            if (unityPlayer == null)
            {
                Debug.LogError("unityPlayer is NULL");
                return;
            }


            mCurrentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (mCurrentActivity == null)
            {
                Debug.LogError("CurrentActivity is NULL");
                return;
            }


            mApplicationContext = mCurrentActivity.Call<AndroidJavaObject>("getApplicationContext");

            if (mApplicationContext == null)
            {
                Debug.LogError("ApplicationContext is NULL");
                return;
            }

            // test a fonction of the plugin :
            // code frame encoding :           
            int BatteryLevel = mAndroidSystemComPlugIn.CallStatic<int>("getBatteryPercentage", mApplicationContext);
            Debug.Log("BatteryLevel to test plug in validity :" + BatteryLevel);


            mAndroidSystemComPlugIn.CallStatic("createVideoEncoder", mApplicationContext);

            Debug.Log("----------- Init AndroidSystemComPlugIn SUCCEED");

        }
        catch (System.Exception e)
        {
            Debug.LogError(e, this);
            Debug.Log("----------- Init AndroidSystemComPlugIn FAILED");
        }
    }


    void Awake()
    {
        mSizeInBytes = mWidth*mHeight*4; // RGBA
        
        mTextureVideoReceive = new Texture2D(mWidth, mHeight, TextureFormat.RGBA32, false);

        mResizedWidth  = mWidth / 2;  // 768  
        mResizedHeight = mHeight/ 2;  // 702

        mRenderTexture  = new  RenderTexture(mResizedWidth, mResizedHeight, 24);
        mResizedTexture = new Texture2D(mResizedWidth, mResizedHeight);

        mFrameQueue = new List<byte[]>();
    }


    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        InitAndroidJavaPlugin();
#endif      
    }
  
    void Update()
    {
        if (!mProcessStarted) return;

        if (frameNumber <= m_maxFrames)
        {           
            // Inform encoder of the validity of the frame
            SetFrameStateToEncoder();

            // Calculate number of video frames to produce from this game frame
            // Generate 'padding' frames if desired framerate is higher than actual framerate
            float thisFrameTime = Time.time;
            int framesToCapture = ((int)(thisFrameTime / mCaptureFrameTime)) - ((int)(mLastFrameTime / mCaptureFrameTime));

            // Capture the frame :
            if (framesToCapture > 0)
            {               
                IntPtr pixels = IntPtr.Zero;
                lynx.LynxSXRAPI.GetCameraAnd3DImage(ref pixels);

                mTextureVideoReceive.LoadRawTextureData(pixels, mSizeInBytes);               
                mTextureVideoReceive.Apply(); // necessaire

                // Resize now texture 
                RenderTexture.active = mRenderTexture;
                Graphics.Blit(mTextureVideoReceive, mRenderTexture);
                VerticallyFlipRenderTexture(mRenderTexture);

                mResizedTexture.ReadPixels(new Rect(0, 0, mResizedWidth, mResizedHeight), 0, 0);
                RenderTexture.active = null;
               
            }



            // Add the required number of copies to the queue
            for (int i = 0; i < framesToCapture && frameNumber <= m_maxFrames; ++i)
            {
                
                byte[] frameRawData = mResizedTexture.GetRawTextureData(); // RGBA32.// rapide en fait 1 ms voir moins
                                                                          //                m_sw.Stop();
                mFrameQueue.Add(frameRawData);

                frameNumber++;             
            }

            mLastFrameTime = thisFrameTime;
        }
        else //keep making screenshots until it reaches the max frame amount
        {          
            Debug.Log("--------------------- End of capture because frameNumber > mMaxFrames");
            StopRecord();
        }
    }

    public void RecordVideo()
    {
        // compute video file path : it's in DCIM/Lynx/ScreenAndVideoShots
        string GalleryPath = ScreenshotAndVideoUtilities.GetGalleryPath();     
        string strVideoPath = ScreenshotAndVideoUtilities.ComputeVideoShotPath();
        Debug.Log("strVideoPath : " + strVideoPath);
    
        // Prepare the data directory
        mTempFramePath = Application.persistentDataPath + "/TempFrame";

        Debug.Log("Capturing to: " + mTempFramePath + "/");

        if (!System.IO.Directory.Exists(mTempFramePath))
        {
            System.IO.Directory.CreateDirectory(mTempFramePath);
        }

        
        frameNumber = 0;

        mSavingFrameNumber = 0;

        mCaptureFrameTime = 1.0f / (float)m_frameRate;
        mLastFrameTime = Time.time;

        mResizedWidth = mWidth / 2;    // 768  
        mResizedHeight = mHeight / 2;  // 702

        // Important : Set that we autorise to recup frame image to sxrApi
        lynx.LynxSXRAPI.EnableCopyCameraAnd3DImage(true);
        
        // Important : prepare and launch the encoder in java plug in
        mAndroidSystemComPlugIn.CallStatic("prepareRecording", strVideoPath, mResizedWidth, mResizedHeight, 2000000, m_frameRate); // 2000000 = bit rate.

        // Kill the encoder thread if running from a previous execution
        if (mEncoderThread != null && (mThreadIsProcessing || mEncoderThread.IsAlive))
        {
            mThreadIsProcessing = false;
            mEncoderThread.Join();
        }

        // Start a new encoder thread
        mThreadIsProcessing = true;
        mEncoderThread = new Thread(EncodeAndSave);
        mEncoderThread.Start();
       
        // Go !!!
        mProcessStarted = true;
    }

    public void StopRecord()
    {
        mTerminateThreadWhenDone = true;
        Debug.Log("--------------------- Manual end of capture");
        mProcessStarted = false;
        lynx.LynxSXRAPI.EnableCopyCameraAnd3DImage(false);

        isNewFrameAvailable   = false;
        indexOfAvailableFrame = -1;

        mAndroidSystemComPlugIn.CallStatic("StopEncoding");
    }

    bool isNewFrameAvailable = false;
    int indexOfAvailableFrame = -1;

    void SetFrameStateToEncoder()
    {
        if (isNewFrameAvailable)
        {           
            mAndroidSystemComPlugIn.CallStatic("setFrameAvailable", indexOfAvailableFrame);
            isNewFrameAvailable = false;
        }
    }


    void SetNewFrameAvailable(int iFrameNumber)
    {
        isNewFrameAvailable = true;
        indexOfAvailableFrame = iFrameNumber;      
    }


    private void EncodeAndSave()
    {
        while (mThreadIsProcessing)
        {
            if (mFrameQueue.Count > 0)
            {   
                // attention
                string path = mTempFramePath + "/frame" + mSavingFrameNumber + ".raw";

                // a remettre
                System.IO.File.WriteAllBytes(path, mFrameQueue[0]);

                //mFrameQueue.Remove(myBytes);
                mFrameQueue.RemoveAt(0);

                GC.Collect();

                SetNewFrameAvailable(mSavingFrameNumber);

                // Done
                mSavingFrameNumber++;
            }
            else
            {

                if (mTerminateThreadWhenDone)
                {
                    break;
                }

                Thread.Sleep(1);

            }
        }

        mTerminateThreadWhenDone = false;
        mThreadIsProcessing = false;    
        mFrameQueue.Clear();

        Debug.Log("FRAMES SAVER THREAD FINISHED");
    }


    private static void VerticallyFlipRenderTexture(RenderTexture target)
    {
        var temp = RenderTexture.GetTemporary(target.descriptor);
        Graphics.Blit(target, temp, new Vector2(1, -1), new Vector2(0, 1));
        Graphics.Blit(temp, target);
        RenderTexture.ReleaseTemporary(temp);
    }


    void DisplayNativeImageBufferVideo()
    {
        // Load data into the texture and upload it to the GPU.
        //mTextureVideoReceive.LoadRawTextureData(mManagedArray);
        mTextureVideoReceive.Apply();

        // Assign texture to renderer's material.
        PlaneMaterialForVideo.mainTexture = mTextureVideoReceive;
    }






























}
