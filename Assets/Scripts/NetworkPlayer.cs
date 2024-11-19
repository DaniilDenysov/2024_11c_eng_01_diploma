using DTOs;
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject localPlayerInterfaces;
    public static NetworkPlayer LocalPlayerInstance;

    [SerializeField, SyncVar(hook = nameof(OnPlayerDataChanged))] private Player player;

    [SerializeField] private TMP_Text displayName;
    public Player GetPlayerData() => player;

    private void OnPlayerDataChanged(Player oldValue, Player newValue)
    {
        displayName.text = newValue.Nickname;
    }

    public override void OnStartAuthority()
    {
        if (isOwned)
        {
            LocalPlayerInstance = this;
        }
    }

    public override void OnStopAuthority()
    {
        if (isOwned)
        {
            LocalPlayerInstance = null;
        }
    }

    [Server]
    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    private void Start()
    {
        if (!isOwned)
        {
            Destroy(localPlayerInterfaces);
        }
    }
}
