using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public AudioSource audioMain;
    public void SetVolume(float volume)
    {
        audioMain.volume = volume;
    }

    public void SetFullScreen(bool isFS)
    {
        Screen.fullScreen = isFS;
    }
}
