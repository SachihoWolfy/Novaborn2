using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HeaderInfo : MonoBehaviourPun
{
    public TextMeshProUGUI nameText;
    public Slider healthBar;
    public Slider shieldBar;
    public Color32 EnemyColor;
    public TMP_ColorGradient EnemyGradient;
    private float maxValue;
    public void Initialize(string text, int maxVal)
    {
        nameText.text = text;
        if (this.gameObject.CompareTag("Enemy"))
        {
            nameText.color = EnemyColor;
            nameText.colorGradientPreset = EnemyGradient;
        }
        maxValue = maxVal;
        healthBar.maxValue = maxVal;
        healthBar.value = 100;
        shieldBar.maxValue = maxVal;
        shieldBar.value = 0;
    }
    [PunRPC]
    public void UpdateHealthBar(int curHP)
    {
        healthBar.value = curHP;
    }
    [PunRPC]
    public void UpdateShieldBar(int curSP)
    {
        shieldBar.value = curSP;
    }
}
