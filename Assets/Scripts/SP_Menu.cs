using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class SP_Menu : MonoBehaviour

{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject creditScreen;
    public GameObject levelScreen;
    [Header("Main Screen")]
    public Button singlePlayerButton;
    public Button multiplayerButton;
    public Button creditsButton;
    [Header("Credits")]
    public Button backButton;
    [Header("Levels")]
    public Button TEST_LEVEL;
    public Button backToMainButton;


    void Start()
    {
        PhotonNetwork.OfflineMode = true;
        if(NetworkManager.instance != null)
        {
            Destroy(NetworkManager.instance);
        }
        // enable the cursor since we hide it when we play the game
        Cursor.lockState = CursorLockMode.None;
    }

    // changes the currently visible screen
    void SetScreen(GameObject screen)
    {
        // disable all other screens
        mainScreen.SetActive(false);
        creditScreen.SetActive(false);
        levelScreen.SetActive(false);
        // activate the requested screen
        screen.SetActive(true);
    }

    // called when the "Create Room" button has been pressed.
    public void OnSPButton()
    {
        SetScreen(levelScreen);
    }
    public void OnMPButton()
    {
        SceneManager.LoadScene("LoginScene");
    }
    public void OnCreditButton()
    {
        SetScreen(creditScreen);
    }
    public void OnBackButton()
    {
        SetScreen(mainScreen);
    }
    public void OnTestLevel()
    {
        SceneManager.LoadScene("SP_TEST");
    }

}
