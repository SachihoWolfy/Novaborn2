using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoginRegister : MonoBehaviour
{
    [HideInInspector]
    public string playFabId;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI displayText;

    public UnityEvent onLoggedIn;

    public static LoginRegister instance;
    void Awake() { instance = this; }

    // called when the 'Login' button is pressed
    public void OnLoginButton()
    {
        // request to login a user
        LoginWithPlayFabRequest loginRequest = new LoginWithPlayFabRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text
        };

        // send the request to the API
        PlayFabClientAPI.LoginWithPlayFab(loginRequest,
            // callback function for if register SUCCEEDED
            result =>
            {
                SetDisplayText("Logged in as: " + result.PlayFabId, Color.green);
                playFabId = result.PlayFabId;
                SceneManager.LoadScene("Menu");

                if (onLoggedIn != null)
                    onLoggedIn.Invoke();
            },
            // callback function for if register FAILED
            error => SetDisplayText(error.ErrorMessage, Color.red)
        );
    }

    // called when the 'Register' button is pressed
    public void OnRegisterButton()
    {
        // request to register a new user
        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest
        {
            Username = usernameInput.text,
            DisplayName = usernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };

        // send the request to the API
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest,
            // callback function for if register SUCCEEDED
            result => SetDisplayText("Registered a new account as: " + result.PlayFabId, Color.green),
            // callback function for if register FAILED
            error => SetDisplayText(error.ErrorMessage, Color.red)
        );
    }

    // sets the display text and color
    void SetDisplayText(string text, Color color)
    {
        displayText.text = text;
        displayText.color = color;
    }
}
