using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Health,
    Ammo
}

public class Pickup : MonoBehaviourPun
{
    public PickupType type;
    public int value;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Pickup triggered.");

        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);

            if (type == PickupType.Health)
            {
                Debug.Log("Heal.");
                player.photonView.RPC("Heal", player.photonPlayer, value);
            }
            else if (type == PickupType.Ammo)
            {
                Debug.Log("Ammo.");
                player.photonView.RPC("GiveAmmo", player.photonPlayer, value);
            }
                

            // destroy the object
            photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void DestroyPickup()
    {
        Debug.Log("Attempting to destroy...");
        Destroy(this.gameObject);
    }
}
