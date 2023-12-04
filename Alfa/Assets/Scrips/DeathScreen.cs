using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void display()
    {
        gameObject.SetActive(true);
    }

    public void restart()
    {
        SceneManager.LoadScene("UI");
    }
}
