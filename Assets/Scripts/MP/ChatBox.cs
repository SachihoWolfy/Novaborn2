using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Photon.Pun;
public class ChatBox : MonoBehaviourPun
{
    public TextMeshProUGUI chatLogText;
    public TMP_InputField chatInput;
    // instance
    public static ChatBox instance;
    void Awake()
    {
        instance = this;
    }
    // called when the player wants to send a message
    public void OnChatInputSend()
    {
        if (chatInput.text.Length > 0)
        {
            photonView.RPC("Log", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, chatInput.text);
            chatInput.text = "";
        }
        EventSystem.current.SetSelectedGameObject(null);
    }
    [PunRPC]
    void Log(string playerName, string message)
    {
        chatLogText.text += string.Format("<br>{0}:</b> {1}", playerName, message);
        chatLogText.rectTransform.sizeDelta = new Vector2(chatLogText.rectTransform.sizeDelta.x, chatLogText.mesh.bounds.size.y + 20);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (EventSystem.current.currentSelectedGameObject == chatInput.gameObject)
                OnChatInputSend();
            else
                EventSystem.current.SetSelectedGameObject(chatInput.gameObject);
        }
    }

}
