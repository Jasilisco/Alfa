using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
    }

    public void setMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void UpdateHp(int update)
    {
        slider.value = update;
    }
}
