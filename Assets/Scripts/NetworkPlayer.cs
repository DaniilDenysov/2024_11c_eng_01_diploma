using DTOs;
using Managers;
using Mirror;
using Score;
using Steamworks;
using System;
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
    [SerializeField, SyncVar(hook = nameof(OnKillsUpdated))] private int kills; 
    [SerializeField, SyncVar(hook = nameof(OnAssistsUpdated))] private int assists; 
    [SerializeField, SyncVar(hook = nameof(OnDeathsUpdated))] private int deaths; 
    

    [SerializeField] private TMP_Text displayName;


    private void OnDeathsUpdated(int oldValue, int newValue)
    {
        Scoreboard.Instance.SetIsDirty();
    }

    private void OnKillsUpdated (int oldValue, int newValue)
    {
        Scoreboard.Instance.SetIsDirty();
    }

    private void OnAssistsUpdated(int oldValue, int newValue)
    {
        Scoreboard.Instance.SetIsDirty();
    }

    [Server]
    public void SetKills(int kills)
    {
        this.kills = kills;
    }

    [Server]
    public void SetAssists(int assists)
    {
        this.assists = assists;
    }

    [Server]
    public void SetDeaths(int deaths)
    {
        this.deaths = deaths;
    }

    private void OnNicknameChanged(string oldName, string newName)
    {
        Debug.Log($"Nickname changed from {oldName} to {newName}");
        Scoreboard.Instance.SetIsDirty();
        displayName.text = newName;
        if (isOwned)
        {
            displayName.gameObject.SetActive(false);
        }
    }

    [Server]
    public void SetNickname(string v)
    {
        nickname = v;
    }

    public int GetAssists()
    {
        return assists;
    }

    public int GetDeaths()
    {
        return deaths;
    }

    public int GetKills()
    {
        return kills;
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
       // Scoreboard.Instance.RemovePlayerCmd(nickname);
    }


    [Command]
    public void CmdSetNickname(string nickname)
    {
        if (nickname.Equals(this.nickname)) return;
        this.nickname = nickname;
    }

    public string GetName() => nickname;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (!isLocalPlayer) return;
        localPlayerInterfaces.SetActive(true);
        if (SteamManager.Initialized)
        {
            var nickname = SteamFriends.GetPersonaName();
           if (!this.nickname.Equals(nickname)) CmdSetNickname(nickname);
        }
        else
        {
            CmdSetNickname("Steam error");
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
