using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBuilder
{
    public State getState(string key)
    {
        if (key == "LevelState")
            return new LevelState();
        else
            throw new Exception("No state asociated");
    }
}
