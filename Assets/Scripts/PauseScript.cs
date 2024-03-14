using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseCanvas;
    public Slider sensSlider;
    public TextMeshProUGUI sensInfoText;
    public float sensitivity = 3;
    public GameObject otherCanvas;

    public static PauseScript instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        sensSlider.maxValue = 5f;
        sensSlider.minValue = 1f;
        sensSlider.value = 3f;
        pauseCanvas.SetActive(false);
    }

    void TogglePause()
    {
        if (pauseCanvas.gameObject.activeSelf)
        {
            if(!SceneManager.GetActiveScene().name.Contains("Menu") && !SceneManager.GetActiveScene().name.Contains("Cutscene"))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            pauseCanvas.SetActive(false);
            otherCanvas.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            pauseCanvas.SetActive(true);
            otherCanvas.SetActive(false);
        }
    }

    void CheckIfCanvasStillCanvas()
    {
        if (otherCanvas == null)
        {
            otherCanvas = GameObject.FindGameObjectWithTag("HudTag");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckIfCanvasStillCanvas();
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void OnQuitButton()
    {
        if (SceneManager.GetActiveScene().name.Contains("Menu"))
        {
            Debug.Log("Quitting...");
            Application.Quit();
        }
        else
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Menu");
        }
    }
    public void OnScrollBarChange()
    {
        sensitivity = sensSlider.value;
        sensInfoText.SetText("Sensitivity: " + sensitivity);
    }
}
