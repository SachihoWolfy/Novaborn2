using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;

public class CutsceneHandler : MonoBehaviour
{
    public GameObject[] screenList;
    public GameObject[] screenProps;
    public Button nextButton;
    public Button skipButton;
    public string nextScene;

    private int curScreen = 0;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnNextButton()
    {
        // Hopefully I won't be dumb enough to make the screenProps list longer than the screenList. It seems impossible that I would do that.
        if (screenList.Length > curScreen + 1)
            NextScreen();
        else
            EndCutscene();
    }
    public void OnSkipButton()
    {
        EndCutscene();
    }

    public void NextScreen()
    {
        screenList[curScreen].SetActive(false);
        screenProps[curScreen].SetActive(false);
        curScreen++;
        screenList[curScreen].SetActive(true);
        screenProps[curScreen].SetActive(true);
    }
    public void EndCutscene()
    {
        try
        {
            NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, nextScene);
        }
        catch(Exception e)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
