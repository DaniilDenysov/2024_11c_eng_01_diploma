using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UI;
using UnityEngine.Events;

namespace General
{
    public class NetworkPlayerContainer : LabelContainer<NetworkPlayer, NetworkPlayerContainer>
    {
        public override NetworkPlayerContainer GetInstance()
        {
            return this;
        }

        public NetworkPlayer GetOwner(MonoBehaviour behaviour)
        {
            foreach (var item in items)
            {
                if (item.gameObject == behaviour.gameObject)
                {
                    return item;
                }
            }

            return null;
        }
    }
}