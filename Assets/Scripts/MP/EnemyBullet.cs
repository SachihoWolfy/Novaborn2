using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private int damage;
    public Rigidbody rig;
    private Vector3 oldPosition;

    public void Initialize(int damage)
    {
        this.damage = damage;
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
            if (hit.collider.gameObject.tag == "Player")
            {
                PlayerController player = GameManager.instance.GetPlayer(hit.collider.gameObject);
                if (player != null)
                   player.photonView.RPC("TakeDamage", player.photonPlayer, -1, damage);
            }
            if (hit.collider.gameObject.tag != "Bullet" && hit.collider.gameObject.tag != "Debug" && hit.collider.gameObject.tag != "Enemy")
                Destroy(gameObject);
        }
    }
}
