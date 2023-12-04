using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class input
{
    private Dictionary<string, KeyCode> keyMappings = new Dictionary<string, KeyCode>();
    private PauseMenu pauseMenu;

    public input(ControlsConf controls, PauseMenu pause)
    {
        pauseMenu = pause;
        keyMappings = setControls(controls);
    }

    public KeyCode strToKeycode(string keyString)
    {
        try
        {
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyString);
            return keyCode;
        }
        catch (Exception)
        {
            throw new Exception("La cadena proporcionada no es un KeyCode válido: " + keyString);
        }
    }

    public void processInput(Player player)
    {
        if (Input.GetKeyDown(keyMappings["Pause"]))
        {
            if (pauseMenu.isPaused())
                pauseMenu.ResumeGame();
            else
                pauseMenu.PauseGame();
        }
        else if (Input.GetKey(keyMappings["Attack"]))
            player.attack();
        else
            player.Move(movementChecker());
    }


    public Vector2 movementChecker()
    {
        float axisX = 0f;
        float axisY = 0f;

        if (Input.GetKey(keyMappings["Up"]))
        {
            axisY = 1f;
        }
        else if (Input.GetKey(keyMappings["Down"]))
        {
            axisY = -1f;
        }

        if (Input.GetKey(keyMappings["Right"]))
        {
            axisX = 1f;
        }
        else if (Input.GetKey(keyMappings["Left"]))
        {
            axisX = -1f;
        }

        return new Vector2(axisX, axisY);
    }

    private Dictionary<string, KeyCode> setControls(ControlsConf controls)
    {
        var controlsDict = new Dictionary<string, KeyCode>();
        foreach (controlConf control in controls.controls)
        {
            if (validateControl(control.control))
                controlsDict.Add(control.control, strToKeycode(control.key));
        }
        return controlsDict;
    }

    private bool validateControl(string control)
    {
        switch (control)
        {
            case "Attack":
                return true;
            case "Pause":
                return true;
            case "Up":
                return true;
            case "Down":
                return true;
            case "Left":
                return true;
            case "Right":
                return true;
        }
        return false;
    }
}
