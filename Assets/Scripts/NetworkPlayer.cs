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

    [SerializeField, SyncVar(hook = nameof(OnNicknameChanged))] private string nickname; 

    [SerializeField] private TMP_Text displayName;


    [Command]
    public void CmdAddPlayer (string newNickname)
    {
        Scoreboard.Instance.AddPlayer(newNickname);
    }

  

    public override void OnStartClient()
    {
        base.OnStartClient();
       
    }

    private void OnNicknameChanged(string oldName, string newName)
    {
        Debug.Log($"Nickname changed from {oldName} to {newName}");
        displayName.text = newName;
        Scoreboard.Instance.AddPlayer(newName);
        if (isOwned)
        {
            displayName.gameObject.SetActive(false);
        }
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
        if (!isLocalPlayer) return;
        if (SteamManager.Initialized)
        {
            nickname = SteamFriends.GetPersonaName();
            CmdAddPlayer(nickname);
        }
        else
        {
            Debug.LogError($"Steam not initialized!");
        }

        LocalPlayerInstance = this;
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
