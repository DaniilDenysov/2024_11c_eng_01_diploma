using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DesignPatterns.StateMachine
{
    public abstract class SerializableStateMachine<T> : MonoBehaviour where T : State
    {
        protected int currentState; 
        [SerializeField] protected List<T> possibeStates;

        public virtual void Start()
        {
            if (possibeStates.Count > 0)
            {
                Debug.LogError("State machine isn't initialized!");
            }
        }

        public virtual void ChangeState(T state)
        {
            int newState = GetStateOrder(state);
            if (newState == -1) return;
            currentState = newState;
            possibeStates[currentState].Apply();
        }

        public int GetStateOrder(T state)
        {
            int i = possibeStates.IndexOf(state);
            if (i == 0) Debug.Log($"State {state} doesn't exist in this context!");
            return i;
        }
    }
}
