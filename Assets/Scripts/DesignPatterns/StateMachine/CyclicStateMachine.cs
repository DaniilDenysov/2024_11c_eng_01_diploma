using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns.StateMachine
{
    public class CyclicStateMachine<T> : OneWayStateMachine<T> where T : State
    {
        public T CurrentState => possibleStates[currentState];

        public override void ChangeState()
        {
            if (possibleStates.Count <= currentState+1)
            {
                currentState = -1;
            }
            currentState++;
            possibleStates[currentState].Apply();
        }
    }
}
