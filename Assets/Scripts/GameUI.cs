using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;


public class GameUI : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI winText;
    public Slider fpBar;
    public Image winBackground;
    private PlayerController player;
    // instance
    public static GameUI instance;
    void Awake()
    {
        instance = this;
    }
    public void Initialize(PlayerController localPlayer)
    {
        player = localPlayer;
        healthBar.maxValue = player.maxHp;
        healthBar.value = player.curHp;
        fpBar.maxValue = player.weapon.maxFpAmmo;
        fpBar.value = player.weapon.curFpAmmo;
        UpdatePlayerInfoText();
        UpdateAmmoText();
    }
    public void UpdateHealthBar()
    {
        healthBar.value = player.curHp;
    }
    public void UpdatePlayerInfoText()
    {
        playerInfoText.text = "<b>Alive:</b> " + GameManager.instance.alivePlayers + "\n<b> Kills:</b> " + player.kills;
    }
    public void UpdateAmmoText()
    {
        ammoText.text = player.weapon.curAmmo + " / " + player.weapon.maxAmmo;
    }
    public void UpdateFPAmmo()
    {
        //Implement a slider
        fpBar.value = player.weapon.curFpAmmo;
    }
    public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnerName + " WINS";
    }
}
