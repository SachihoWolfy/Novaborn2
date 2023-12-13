using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum PickupType
{
    Health,
    Ammo,
    Shield,
    Explosive
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
                if (player.curHp < player.maxHp)
                {
                    Debug.Log("Heal.");
                    player.photonView.RPC("Heal", player.photonPlayer, value);
                }
                else
                {
                    Debug.Log("Rejected. Player is FullHP.");
                    return;
                }
            }
            else if (type == PickupType.Ammo)
            {
                if (player.weapon.curFpAmmo < player.weapon.maxFpAmmo)
                {
                    Debug.Log("Ammo.");
                    player.photonView.RPC("GiveAmmo", player.photonPlayer, value);
                }
                else
                {
                    Debug.Log("Rejected. Player already has Full FirePower.");
                    return;
                }
            }
            else if (type == PickupType.Shield)
            {
                if (player.curShield < 1)
                {
                    Debug.Log("Shield.");
                    player.photonView.RPC("Shield", player.photonPlayer, value);
                }
                else
                {
                    Debug.Log("Rejected. Player already has shield.");
                    return;
                }
            }
            else if (type == PickupType.Explosive)
            {
                Debug.Log("Explosive.");
                //IMPLEMENT LATER
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
