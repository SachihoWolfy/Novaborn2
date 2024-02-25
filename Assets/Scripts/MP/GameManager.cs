using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;
    private int playersInGame;
    // instance
    public static GameManager instance;
    public float postGameTime;
    [Header("Game Modifiers")]
    public bool pvp;
    public bool debug;
    public bool cutscene;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame()
    {
        playersInGame++;
        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length && !cutscene)
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        // initialize the player for all other players
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }


    public PlayerController GetPlayer(int playerId)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.id == playerId)
                return player;
        }
        return null;
    }
    public PlayerController GetPlayer(GameObject playerObject)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObject)
                return player;
        }
        return null;
    }


    public void CheckWinCondition()
    {
        if (alivePlayers == 1 && pvp && playersInGame > 1)
        {
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
        }
        if(MissionManager.instance != null)
        {
            if (MissionManager.instance.allMissionsComplete)
            {
                photonView.RPC("WinGamePVE", RpcTarget.All);
            }
            if (alivePlayers == 0)
            {
                photonView.RPC("LoseGame", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void WinGame(int winningPlayer)
    {
        // set the UI win text
        GameUI.instance.SetWinText(GetPlayer(winningPlayer).photonPlayer.NickName);
        Invoke("GoBackToMenu", postGameTime);
    }
    [PunRPC]
    void WinGamePVE()
    {
        GameUI.instance.SetWinTextPVE();
        Invoke("NextMission", postGameTime);
    }
    [PunRPC]
    void LoseGame()
    {
        GameUI.instance.SetMissionFailText();
        Invoke("GoBackToMenu", postGameTime);
    }
    void GoBackToMenu()
    {
        DestroyNetworkManager();
        NetworkManager.instance.ChangeScene("Menu");
    }
    void NextMission()
    {
        if (debug)
            GameUI.instance.DEBUG_DeactivateWinText();
        else
        {
            try { NetworkManager.instance.ChangeScene(NameOfSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1)); }
            catch (Exception ex)
            {
                Debug.LogException(ex, this);
                GoBackToMenu();
            }
        }
    }
    void DestroyNetworkManager()
    {   
        Destroy(NetworkManager.instance.gameObject);
    }
    //Got this function online, to save time. https://discussions.unity.com/t/getting-next-scene-name/188003
    public string NameOfSceneByBuildIndex(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }
}
