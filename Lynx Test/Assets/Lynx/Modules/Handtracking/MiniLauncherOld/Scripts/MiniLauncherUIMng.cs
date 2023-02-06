using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MiniLauncherUIMng : MonoBehaviour
{   
    bool mScreenshotInProgress = false;
    float mTimerScreenshot = 6.0f;
    float mTimerBip = 0.0f;

    public CaptureVideoMng captureVideoMng = null;

    [SerializeField]
    private TextMeshPro mCounterScreenshot;

    [SerializeField]
    private SoundsMng mSoundMng;

    [SerializeField]
    private GameObject m_VideoRecordButton;

    [SerializeField]
    private GameObject m_StopVideoRecordButton;


    public ScreenshotAndVideoUtilities m_screenshotAndVideoUtilities;

    void Update()
    {
        if (mScreenshotInProgress)
        {
            mTimerScreenshot -= Time.deltaTime;
            mTimerBip -= Time.deltaTime;

            int time = (int)mTimerScreenshot;

            if (mTimerScreenshot < 1)
            {
                mScreenshotInProgress = false;
                mTimerScreenshot = 6.0f;
                mTimerBip = 0.0f;

                mSoundMng.playDiaph();

                mCounterScreenshot.gameObject.SetActive(false);
                m_screenshotAndVideoUtilities.TakeShot();

                //screenShotFinished.Invoke();

                return;
            }
            else
            {
                mCounterScreenshot.text = time.ToString() + "s";
            }

            if (mTimerBip <= 0)
            {
                mSoundMng.playBip();
                mTimerBip = 1.0f;
            }
        }
    }

    public void TakeScreenShotAsked()
    {
        if (mScreenshotInProgress) return;

        mScreenshotInProgress = true;
        mCounterScreenshot.gameObject.SetActive(true);
    }

    public void RecordVideo()
    {
        if (captureVideoMng!=null)
        {
            m_VideoRecordButton.SetActive(false);
            m_StopVideoRecordButton.SetActive(true);
            captureVideoMng.RecordVideo();
        }
        else
        {
            Debug.Log("Capture Video Manager = NULL, Impossible to record video");
        }
    }

    public void StopVideo()
    {
        m_StopVideoRecordButton.SetActive(false);
        m_VideoRecordButton.SetActive(true);
        captureVideoMng.StopRecord();
    }






}
