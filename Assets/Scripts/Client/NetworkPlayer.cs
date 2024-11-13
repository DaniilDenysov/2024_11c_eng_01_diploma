using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Camera camera;


    private void Start()
    {
        if (!isOwned)
        {
            Destroy(camera.gameObject);
        }
    }

}
