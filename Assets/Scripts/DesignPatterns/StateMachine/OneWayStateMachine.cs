using DesignPatterns.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayStateMachine<T> : StateMachine<T> where T : State
{
    public virtual void ChangeState()
    {
        if (possibleStates.Count == currentState++)
        {
            Debug.LogError("No state found!");
            return;
        }
        currentState++;
        possibleStates[currentState].Apply();
    }

    public override void ChangeState(T state)
    {
        Debug.LogError($"Impossible to change state directly in one-way state machine!");
    }
}
