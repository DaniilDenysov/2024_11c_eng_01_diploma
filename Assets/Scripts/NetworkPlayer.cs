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
    [SerializeField, SyncVar(hook = nameof(OnTeamGuidChaged))] private string teamGuid; 
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


    [ClientRpc]
    public void SetTextColor(Color clr)
    {
        displayName.color = clr;
    }

    [Command(requiresAuthority = false)]
    public void CmdSetColor ()
    {
        SetTextColor(((CustomNetworkManager)NetworkManager.singleton).GetTeamColor(teamGuid));
    }

    private void OnTeamGuidChaged(string oldTeamGuid, string newTeamGuid)
    {
        Debug.Log($"Team guid changed from {oldTeamGuid} to {newTeamGuid}");
    }

    [Server]
    public void SetTeamGuid(string v)
    {
        teamGuid = v;
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

    [Command]
    public void CmdAddToTeam(string nickname)
    {
        if (!((CustomNetworkManager)NetworkManager.singleton).TryAddToTeam(nickname, out teamGuid))
        {
            Debug.LogError("Unable to join team!");
        }
    }

    public string GetName() => nickname;
    public string GetTeamGuid() => teamGuid;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (!isOwned) return;
        Debug.Log("Local player initialized!");
       
        LocalPlayerInstance = this;


        if (localPlayerInterfaces != null)
        {
            localPlayerInterfaces.SetActive(true);
        }
        else
        {
            Debug.LogError("Local player interfaces are not set in the inspector!");
        }

        if (displayName != null)
        {
            displayName.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Display name text is missing!");
        }

        if (SteamManager.Initialized)
        {
            var nickname = SteamFriends.GetPersonaName();
            if (!this.nickname.Equals(nickname))
            {
                CmdSetNickname(nickname);
                CmdAddToTeam(nickname);
            }
        }
        else
        {
            Debug.LogWarning("SteamManager is not initialized. Setting nickname to 'Steam error'.");
            CmdSetNickname("Steam error");
        }

        Debug.Log("Local player initialized successfully.");
    }


    public Camera GetPlayerCamera() => playerCamera;
}
