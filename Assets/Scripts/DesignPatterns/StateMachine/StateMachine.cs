using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.StateMachine
{
    public abstract class StateMachine<T> where T : State
    {
        protected int currentState;
        [SerializeField] protected List<T> possibleStates = new List<T>();

        public virtual void ChangeState(T state)
        {
            int newState = GetStateOrder(state);
            if (newState == -1) return;
            currentState = newState;
            possibleStates[currentState].Apply();
        }

        public virtual bool AddState(T state)
        {
            if (possibleStates.Contains(state)) return false;
            possibleStates.Add(state);
            return true;
        }

        public int GetStateOrder(T state)
        {
            int i = possibleStates.IndexOf(state);
            if (i == 0) Debug.Log($"State {state} doesn't exist in this context!");
            return i;
        }
    }
}
