using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScreenshotMng : MonoBehaviour
{

    //INSPECTOR
    [SerializeField] private GameObject timer;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private SoundsMng soundMng;
    
    //PUBLIC
    public ScreenshotAndVideoUtilities screenshotAndVideoUtilities;
    //public CaptureVideoMng captureVideoMng = null;

    //PRIVATE
    private bool screenshotProcessActive = false;
    private float timerScreenshot = 6.0f;
    private float timerBip = 0.0f;



    private void Update()
    {
        UpdateScreenshotProcess();
    }


    public void StartScreenshotProcess()
    {
        if (screenshotProcessActive) return;

        screenshotProcessActive = true;
        timer.SetActive(true);
    }

    private void UpdateScreenshotProcessOld()
    {
        if (screenshotProcessActive)
        {
            timerScreenshot -= Time.deltaTime;
            timerBip -= Time.deltaTime;

            int time = (int)timerScreenshot;

            if (timerScreenshot < 1)
            {
                screenshotProcessActive = false;
                timerScreenshot = 6.0f;
                timerBip = 0.0f;

                soundMng.playDiaph();

                timer.SetActive(false);
                screenshotAndVideoUtilities.TakeShot();

                //screenShotFinished.Invoke();

                return;
            }
            else
            {
                timerText.text = time.ToString() + "s";
            }

            if (timerBip <= 0)
            {
                soundMng.playBip();
                timerBip = 1.0f;
            }
        }
    }
    private void UpdateScreenshotProcess()
    {
        if (screenshotProcessActive == false) return;


        timerScreenshot -= Time.deltaTime;
        timerBip -= Time.deltaTime;

        int time = (int)timerScreenshot;

        if (timerScreenshot < 1)
        {
            screenshotProcessActive = false;
            timerScreenshot = 6.0f;
            timerBip = 0.0f;

            soundMng.playDiaph();

            timer.SetActive(false);
            screenshotAndVideoUtilities.TakeShot();

            //screenShotFinished.Invoke();

            return;
        }
        else
        {
            timerText.text = time.ToString() + "s";
        }

        if (timerBip <= 0)
        {
            soundMng.playBip();
            timerBip = 1.0f;
        }
    }




    //to be rewritten
    public void RecordVideo()
    {
        //if (captureVideoMng != null)
        //{
        //    captureVideoMng.RecordVideo();
        //}
        //else
        //{
        //    Debug.Log("Capture Video Manager = NULL, Impossible to record video");
        //}
    }
    //to be rewritten
    public void StopVideo()
    {
        //captureVideoMng.StopRecord();
    }

}
