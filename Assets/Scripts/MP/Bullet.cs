using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private int attackerId;
    private bool isMine;
    public GameObject impactEffect;
    public Rigidbody rig;
    private Vector3 oldPosition;

    public void Initialize(int damage, int attackerId, bool isMine)
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;
        Destroy(gameObject, 5.0f);
    }

    private void Start()
    {
        oldPosition = transform.position;
    }
    private void Update()
    {
        // Shoot a ray between the old and new position to detect collisions
        RaycastHit hit;
        if (Physics.Raycast(oldPosition, transform.position - oldPosition, out hit, (transform.position - oldPosition).magnitude))
        {
            // Handle the collision
            //Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.gameObject.tag == "Player" && isMine && GameManager.instance.pvp)
            {
                PlayerController player = GameManager.instance.GetPlayer(hit.collider.gameObject);
                if (player.id != attackerId)
                {
                    player.photonView.RPC("TakeDamage", player.photonPlayer, attackerId, damage);
                }
            }
            if (hit.collider.gameObject.tag == "Enemy" && isMine)
            {
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.photonView.RPC("TakeDamage", Photon.Pun.RpcTarget.MasterClient, damage);
                }
            }
            if (hit.collider.gameObject.tag != "Bullet" && hit.collider.gameObject.tag != "Pickup" && hit.collider.gameObject.tag != "Debug")
            {
                if(hit.collider.gameObject.tag == "Player" && isMine)
                {
                    PlayerController player = GameManager.instance.GetPlayer(hit.collider.gameObject);
                    if(player != null)
                        if (player.photonView.IsMine)
                        {
                            return;
                        }
                }
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                GameObject impactSpawn = Instantiate(impactEffect, hit.point, transform.rotation);
                impactSpawn.transform.forward = this.transform.forward;
                Destroy(gameObject, 0.2f);
                this.enabled = false;
            }
        }
    }
}
