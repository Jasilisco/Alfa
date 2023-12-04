using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public Slider slider;

    public void loadLevel()
    {
        StartCoroutine(LoadAsynchrone());
    }

    public void reloadLevel(State level)
    {
        gameObject.SetActive(true);
        StartCoroutine(SimulateLoad(level));
    }

    IEnumerator LoadAsynchrone()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("LevelCreator");
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slider.value = progress;
            yield return null;
        }
    }

    IEnumerator SimulateLoad(State level)
    {
       float simulation = 0f;
       while (!level.isReady())
       {
            float progress = Mathf.Clamp01(simulation / .9f);
            slider.value = progress;
            simulation += 3f;
            yield return null;
       }
       gameObject.SetActive(false);
    }
}
