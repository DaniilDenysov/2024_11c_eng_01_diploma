using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject localPlayerInterfaces;


    private void Start()
    {
        if (!isOwned)
        {
            Destroy(localPlayerInterfaces);
        }
    }

}
