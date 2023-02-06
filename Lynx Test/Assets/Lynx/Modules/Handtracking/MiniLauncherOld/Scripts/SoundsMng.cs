using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsMng : MonoBehaviour
{
    [SerializeField]
    private AudioSource mAudioSource;

    [SerializeField]
    private AudioClip[] mAudioClips;

    public void playClickButton()
    {
        Debug.Log("playClickButton");
        mAudioSource.PlayOneShot(mAudioClips[0]);
    }

    public void playBackButton()
    {
        mAudioSource.PlayOneShot(mAudioClips[1]);
    }

    public void playDiaph()
    {
        mAudioSource.PlayOneShot(mAudioClips[2]);
    }

    public void playBip()
    {
        mAudioSource.PlayOneShot(mAudioClips[3]);
    }
}
