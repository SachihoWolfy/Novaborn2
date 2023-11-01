using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks

{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;
    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;
    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;
    public TMP_Dropdown mapList;
    public string selectedMap;
    [Header("Lobby Browser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;
    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();


    void Start()
    {
        // disable the menu buttons at the start
        if (createRoomButton != null)
        {
            createRoomButton.interactable = false;
        }
        findRoomButton.interactable = false;
        // enable the cursor since we hide it when we play the game
        Cursor.lockState = CursorLockMode.None;
        // are we in a game?
        if (PhotonNetwork.InRoom)
        {
            // go to the lobby
            SetScreen(lobbyScreen);
            UpdateLobbyUI();
            // make the room visible
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    // changes the currently visible screen
    void SetScreen(GameObject screen)
    {
        // disable all other screens
        mainScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        // activate the requested screen
        screen.SetActive(true);
        if (screen == lobbyBrowserScreen)
            UpdateLobbyBrowserUI();
    }

    public void OnPlayerNameValueChanged(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    public override void OnConnectedToMaster()
    {
        // enable the menu buttons once we connect to the server
        if (createRoomButton != null)
        {
            createRoomButton.interactable = true;
        }
        findRoomButton.interactable = true;
    }

    // called when the "Create Room" button has been pressed.
    public void OnCreateRoomButton()
    {
        SetScreen(createRoomScreen);
    }

    // called when the "Find Room" button has been pressed
    public void OnFindRoomButton()
    {
        SetScreen(lobbyBrowserScreen);
    }

    // called when the "Back" button gets pressed
    public void OnBackButton()
    {
        SetScreen(mainScreen);
    }
    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    [PunRPC]
    void UpdateLobbyUI()
    {
        // enable or disable the start game button depending on if we're the host
        startGameButton.interactable = PhotonNetwork.IsMasterClient;
        //mapList.interactable = PhotonNetwork.IsMasterClient;   
        // display all the players
        playerListText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
            playerListText.text += player.NickName + "\n";
        // set the room info text
        selectedMap = mapList.options[mapList.value].text;
        roomInfoText.text = "<b>Room Name</b>\n" + PhotonNetwork.CurrentRoom.Name + "\n<b>Current Map</b>\n" + selectedMap;
    }
    public void OnMapChange()
    {
        UpdateLobbyUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }
    public void OnStartGameButton()
    {
        // hide the room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        // tell everyone to load the game scene
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, selectedMap);
    }
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
    void UpdateLobbyBrowserUI()
    {
        // disable all room buttons
        foreach (GameObject button in roomButtons)
            button.SetActive(false);
        // display all current rooms in the master server
        for (int x = 0; x < roomList.Count; ++x)
        {
            // get or create the button object
            GameObject button = x >= roomButtons.Count ? CreateRoomButton() : roomButtons
           [x];
            button.SetActive(true);
            // set the room name and player count texts
            button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text =
           roomList[x].Name;
            button.transform.Find("PlayerCountText").GetComponent<TextMeshProUGUI>().text
            = roomList[x].PlayerCount + " / " + roomList[x].MaxPlayers;

            // set the button OnClick event
            Button buttonComp = button.GetComponent<Button>();
            string roomName = roomList[x].Name;
            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { OnJoinRoomButton(roomName); });
        }
    }

    GameObject CreateRoomButton()
    {
        GameObject buttonObj = Instantiate(roomButtonPrefab, roomListContainer.transform)
       ;
        roomButtons.Add(buttonObj);
        return buttonObj;
    }
    public void OnJoinRoomButton(string roomName)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }

    public void OnRefreshButton()
    {
        UpdateLobbyBrowserUI();
    }
    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        Debug.Log("OnRoomListUpdate called from Photon");
        roomList = allRooms;
    }



}
