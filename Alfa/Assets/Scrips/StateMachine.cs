using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private List<State> states;
    private State initialState;
    private State currentState;
    private State previousState;
    private bool end;

    public StateMachine(string initialState, List<State> states)
    {
        end = false;
        this.states = states;
        this.initialState = getState(initialState);
    }

    public State getPrevious()
    {
        return previousState;
    }

    public bool executionFinished()
    {
        return end;
    }
    public bool stateRunning()
    {
        return currentState.isRunning();
    }

    public State getState(string name)
    {
        return states.Find(state => state.getName() == name);
    }

    public void restartRun()
    {
        advance(initialState);
        end = false;
    }

    public State startExecution()
    {
        return advance(initialState);
    }

    public void endExecution()
    {
        end = true;
    }

    public State advance(State nextState)
    {
        if (nextState != null)
        {
            if (currentState != null)
            {
                previousState = currentState;
                currentState.deactivate();
            }
            currentState = nextState;
            currentState.activate();
            return currentState;
        }
        return null;
    }
}
