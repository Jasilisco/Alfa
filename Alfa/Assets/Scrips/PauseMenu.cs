using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void setVolume(float volume)
    {
        AudioController audioController = GameObject.FindWithTag("Director").GetComponent<AudioController>();
        audioController.setVolume(volume);
    }

    public void PauseGame()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public bool isPaused()
    {
        if (gameObject.activeSelf)
            return true;
        else return false;
    }
}
