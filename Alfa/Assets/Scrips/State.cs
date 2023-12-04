using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface State
{
    public string getName();
    public void activate();
    public bool isRunning();
    public bool isEnd();
    public string getNext();
    public void deactivate();
    public bool isReady();
}
