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
    [SerializeField, SyncVar(hook = nameof(OnKillCountChanged))] private int kills; 
    [SerializeField, SyncVar(hook = nameof(OnAssistCountChanged))] private int assists; 
    [SerializeField, SyncVar(hook = nameof(OnDeathCountChanged))] private int deaths;
    

    [SerializeField] private TMP_Text displayName;


    private void OnKillCountChanged(int oldCount, int newCount)
    {
        Scoreboard.Instance.Refresh();
    }

    private void OnAssistCountChanged(int oldCount, int newCount)
    {
        Scoreboard.Instance.Refresh();
    }

    private void OnDeathCountChanged(int oldCount, int newCount)
    {
        Scoreboard.Instance.Refresh();
    }

    private void OnNicknameChanged(string oldName, string newName)
    {
        Debug.Log($"Nickname changed from {oldName} to {newName}");
        displayName.text = newName;
        Scoreboard.Instance.Refresh();
        if (isOwned)
        {
            displayName.gameObject.SetActive(false);
        }
    }

    [Server]
    public void AddKill()
    {
        kills++;
    }

    [Server]
    public void AddDeath()
    {
        deaths++;
    }

    [Command]
    public void CmdSetNickname(string nickname) => this.nickname = nickname;

    public string GetName() => nickname;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if (!isLocalPlayer) return;
        localPlayerInterfaces.SetActive(true);
        if (SteamManager.Initialized)
        {
            CmdSetNickname(SteamFriends.GetPersonaName());
        }
        else
        {
            CmdSetNickname("Steam error");
        }

        LocalPlayerInstance = this;
    }

    public int GetAssistCount()
    {
        return assists;
    }

    public int GetDeathCount()
    {
        return deaths;
    }

    internal int GetKillCount()
    {
        return kills;
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
