using Managers;
using TMPro;
using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DTOs;
//using Lobby;
using UI;
using Steamworks;
using System.Collections;

public class PlayerLabel : NetworkBehaviour
{
    public static PlayerLabel LocalPlayer;
    [SerializeField] private Sprite iconPlaceHolder;
    [SyncVar(hook = nameof(OnAvatarChanged))] private Texture2D playerAvatar;


    [SerializeField] private TMP_Text displayName, readyDisplay;
    [SerializeField] private Image characterSelected;
    [SyncVar(hook = nameof(OnPlayerStateChanged))] public Player Player = new Player();
  //  [SerializeField] private List<CharacterData> characters = new List<CharacterData>();
    [SerializeField] private AudioClip startGameSound;
   // [SerializeField] private AudioEventChannel eventChannel;
    public static event Action<bool> OnPartyOwnerChanged;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            Debug.Log("Assigned");
            LocalPlayer = this;
            CmdSetPlayerName(SteamFriends.GetPersonaName());
            var playerSteamID = SteamUser.GetSteamID();
          /*  var playerSteamID = SteamUser.GetSteamID();
            int avatarInt = SteamFriends.GetLargeFriendAvatar(playerSteamID); 

            if (avatarInt != -1)
            {
                StartCoroutine(LoadSteamAvatar(avatarInt));
            }*/
        }
    }

    private void OnAvatarChanged(Texture2D oldValue, Texture2D newValue)
    {
        if (newValue == null) return;
        Rect spriteRect = new Rect(0, 0, newValue.width, newValue.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        iconPlaceHolder = Sprite.Create(newValue, spriteRect, pivot);
    }

    private IEnumerator LoadSteamAvatar(int avatarInt)
    {
        uint imageWidth, imageHeight;
        bool success = SteamUtils.GetImageSize(avatarInt, out imageWidth, out imageHeight);

        if (!success || imageWidth == 0 || imageHeight == 0)
        {
            Debug.LogError("Failed to get avatar image size.");
            yield break;
        }

        byte[] imageData = new byte[imageWidth * imageHeight * 4];
        success = SteamUtils.GetImageRGBA(avatarInt, imageData, (int)(imageWidth * imageHeight * 4));

        if (!success)
        {
            Debug.LogError("Failed to get avatar image data.");
            yield break;
        }


        var texture = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(imageData);
        texture.Apply();

        playerAvatar = texture;
    }

    private void Start()
    {
        PlayerLabelsContainer.Instance.Add(this);
    }

   /* public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CmdSetPlayerName(SteamFriends.GetPersonaName());
    }*/

    public override void OnStopClient()
    {
        base.OnStopClient();
     //   CharacterSelectionLabel.OnDeselected?.Invoke(Player.CharacterGUID);
    }

    #region commands
    [Command]
    public void CmdStartGame ()
    {
        if (!Player.IsPartyOwner) return;
        CustomNetworkManager networkManager = ((CustomNetworkManager)NetworkManager.singleton);
        if (!networkManager.CanStartGame()) return;

        OnGameStarted();
        networkManager.StartGame();
    }

    [ClientRpc]
    public void OnGameStarted ()
    {
       // eventChannel.RaiseEvent(startGameSound);
    }
 
    [Command]
    public void CmdReady()
    {
        var player = new Player(Player);
        player.IsReady = !Player.IsReady;
        Player = player;
    }
    #endregion
    #region setters
    [ClientRpc]
    public void CmdSetPlayerIcon(string playerName)
    {
        
    }

    [Command]
    public void CmdSetPlayerName(string playerName)
    {
        var player = new Player(Player);
        player.Nickname = playerName;
        Player = player;
    }

    [Server]
    public void SetPartyOwner (bool isPartyOwner)
    {
        var player = new Player(Player);
        player.IsPartyOwner = isPartyOwner;
        Player = player;
    }

    [Server]
    public void SetConnectionId(int connectionId)
    {
        var player = new Player(Player);
        player.ConnectionId = connectionId;
        Player = player;
    }

    [Command]
    public void SetCharacterGUID(string characterGUID)
    {
        var player = new Player(Player);
        player.CharacterGUID = characterGUID;
        Player = player;
    }
    #endregion
    #region getters
    public bool GetPartyOwner() => Player.IsPartyOwner;
    public string GetPlayerName() => Player.Nickname;
    public bool GetReady() => Player.IsReady;
    #endregion
    #region callbacks
    public void OnPlayerStateChanged (Player oldState, Player newState)
    {
        string suffix = "";
        if (isLocalPlayer) suffix = " (You)";
        displayName.text = newState.Nickname + suffix;
        readyDisplay.text = newState.IsReady == true ? "Ready" : "Not ready";
        readyDisplay.color = newState.IsReady == true ? Color.green : Color.red;
       // CharacterData characterData = characters.Where((c) => c.CharacterGUID == newState.CharacterGUID).FirstOrDefault();
        if (!isOwned) return;
        OnPartyOwnerChanged?.Invoke(newState.IsPartyOwner);
    }
    #endregion
    #region rpcs

    [ClientRpc]
    public void AddToContainer ()
    {
        PlayerLabelsContainer.Instance.Add(this);
    }
    #endregion
}
