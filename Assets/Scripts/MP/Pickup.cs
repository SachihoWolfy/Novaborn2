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
        // Should turn off code for people who aren't master client.
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (other.CompareTag("Player"))
        {
            PlayerController player = GameManager.instance.GetPlayer(other.gameObject);
            if (player != null)
            {
                if (type == PickupType.Health)
                {
                    Debug.Log("Saw Health Powerup");
                    // CRUD, this if statement doesn't pass if the player isn't the MasterClient.
                    // Bandaid it for now, look deeper in the future on why the heck this happens.
                    // The if statement can't seem to see what player.curHp really is.
                    if (player.curHp < player.maxHp || !player.photonView.IsMine)
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
                    Debug.Log("You have collected a shard");
                    player.photonView.RPC("CollectedShards", player.photonPlayer);

                    //IMPLEMENT LATER
                }


                // destroy the object
                photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void DestroyPickup()
    {
        Debug.Log("Attempting to destroy...");
        Destroy(this.gameObject);
    }
}
