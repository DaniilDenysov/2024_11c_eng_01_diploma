using DTOs;
using Managers;
using Mirror;
using Score;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject localPlayerInterfaces;
    [SerializeField] private Camera playerCamera;
    public static NetworkPlayer LocalPlayerInstance;

    [SyncVar(hook = nameof(OnNicknameChanged)), SerializeField] private string nickname; 

    [SerializeField] private TMP_Text displayName;


    [Command]
    public void CmdAddPlayer (string newNickname)
    {
        Scoreboard.Instance.AddPlayer(newNickname);
    }

    public override void OnStartAuthority()
    {
        if (isOwned)
        {
            if (SteamManager.Initialized)
            {
                nickname = SteamFriends.GetPersonaName();
            }
            else
            {
                Debug.LogError($"Steam not initialized!");
            }
            CmdAddPlayer(nickname);
            LocalPlayerInstance = this;
        }
    }

    public void OnNicknameChanged (string oldName, string newName)
    {
        Debug.Log($"Nickname changed from {oldName} to {newName}");
        if (!isOwned) displayName.text = newName;
        else displayName.gameObject.SetActive(false);
    }

    [Server]
    public void SetNickname (string newName)
    {
        nickname = newName;
    }

    public string GetName() => nickname;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayerInterfaces.SetActive(true);
    }

    public override void OnStopAuthority()
    {
        if (isOwned)
        {
            LocalPlayerInstance = null;
        }
    }

    public Camera GetPlayerCamera() => playerCamera;
}
