using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.StateMachine
{
    public abstract class OneWaySerializableStateMachine<T> : SerializableStateMachine<T> where T : State
    {
        public void ChangeState ()
        {
            if (possibeStates.Count == currentState++)
            {
                Debug.LogError("No state found!");
                return;
            }
            currentState++;
            possibeStates[currentState].Apply();
        }

        public override void ChangeState(T state)
        {
            Debug.LogError($"Impossible to change state directly in one-way state machine!");
        }
    }
}
