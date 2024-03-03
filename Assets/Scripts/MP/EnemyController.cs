using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEditor;

public class EnemyController : MonoBehaviourPun
{
    //extremely basic navmesh AI. Follows player.
    //To-do: Get Player position somehow
    //To-do: Aim at player

    UnityEngine.AI.NavMeshAgent agent;
    [Header("Info")]
    public string enemyName;
    public float moveSpeed;
    public int curHp;
    public int maxHp;
    public float chaseRange;
    public float attackRange;
    public float bulletSpeed;
    private PlayerController targetPlayer;
    public float playerDetectRate = 0.2f;
    private float lastPlayerDetectTime;
    public string objectToSpawnOnDeath;
    [Header("Attack")]
    public int damage;
    public float attackRate;
    public float shootRange;
    private float lastAttackTime;
    [Header("Components")]
    public HeaderInfo healthBar;
    public MeshRenderer sr;
    public Material hurtMat;
    public Material normalMat;
    public Rigidbody rig;
    public GameObject moveables;
    public GameObject gun;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    public Animator anim;
    public ParticleSystem laser;
    public string deadEnemyName;
    private Transform gunDefaultRotation;
    [Header("Sounds")]
    public AudioClip hurt;
    public AudioClip death;
    public AudioClip fire;
    public AudioSource AS;

    //privates
    private bool panicking;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        healthBar.Initialize(enemyName, maxHp);
        gunDefaultRotation = gun.transform;
    }


    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            agent.enabled = false;
            return;
        }
        if (agent.enabled && !agent.isOnNavMesh && !agent.isOnOffMeshLink && !panicking)
        {
            Panic();
        }
        if (targetPlayer != null)
        {
            // calculate the distance
            float dist = Vector3.Distance(transform.position, targetPlayer.transform.position);
            // if we're chasing, look at them!
            if (dist <= chaseRange)
            {
                AimAtPlayer(moveables);
            }
            if (dist < shootRange)
            {
                AimAtPlayer(gun);
                if (laser.isStopped)
                {
                    laser.Play();
                }
            } 
            else if (laser.isPlaying)
            {
                laser.Stop();
            }
            // if we're able to attack, do so
            if (dist < shootRange && Time.time - lastAttackTime >= attackRate)
            {

                // And FIRE!
                Attack();
            }
            // otherwise, do we move after the player?
            else if (dist > attackRange)
            {
                if(agent.isStopped)
                agent.isStopped = false;

                Vector3 dir = targetPlayer.transform.position - transform.position;
                //rig.velocity = dir.normalized * moveSpeed;
            }
            else
            {
                // Stop Everything We're too Close!!!!
                agent.isStopped = true;
                //rig.velocity = Vector3.zero;
                // We also don't look at player.
            }
            agent.destination = targetPlayer.transform.position;
        }
        DetectPlayer();

    }

    // Panic by deactivating and reactivating navmeshAgent until we are on a surface that Navmesh works on.
    IEnumerator Panic()
    {
        yield return new WaitForSeconds(1f);
        agent.enabled = false;
        panicking = true;
        yield return new WaitForSeconds(1f);
        agent.enabled = true;
        panicking = false;
    }

    // Aims at target player
    public void AimAtPlayer(GameObject hands)
    {
        hands.transform.LookAt(targetPlayer.transform.position);
    }
    // attacks the targeted player
    void Attack()
    {
        lastAttackTime = Time.time;
        //targetPlayer.photonView.RPC("TakeDamage", targetPlayer.photonPlayer, damage);
        SoundController.instance.PlaySound(AS, fire);
        photonView.RPC("SpawnEnemyBullet", RpcTarget.All, bulletSpawnPos.transform.position, gun.transform.forward);
        anim.SetTrigger("Shoot");
    }
    [PunRPC]
    void SpawnEnemyBullet(Vector3 pos, Vector3 dir)
    {
        // spawn and orientate it
        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;
        // get bullet script
        EnemyBullet bulletScript = bulletObj.GetComponent<EnemyBullet>();
        // initialize it and set the velocity
        bulletScript.Initialize(damage);
        bulletScript.rig.velocity = dir * bulletSpeed;
    }
    // updates the targeted player
    void DetectPlayer()
    {
        if (Time.time - lastPlayerDetectTime > playerDetectRate)
        {
            lastPlayerDetectTime = Time.time;
            // loop through all the players
            foreach (PlayerController player in GameManager.instance.players)
            {
                // calculate distance between us and the player
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (player == targetPlayer)
                {
                    if (dist > chaseRange)
                        targetPlayer = null;
                }
                else if (dist < chaseRange)
                {
                    //Target first player that enters range.
                    if (targetPlayer == null)
                    {
                        targetPlayer = player;
                        anim.SetTrigger("Detect");
                    }
                    //Change target
                    else if(Vector3.Distance(player.transform.position, this.transform.position) < Vector3.Distance(targetPlayer.transform.position, this.transform.position))
                    {
                        targetPlayer = player;
                        anim.SetTrigger("Detect");
                    }
                    if(targetPlayer != null)
                    {
                        if (targetPlayer.dead)
                        {
                            targetPlayer = null;
                        }
                    }
                }
            }
        }
    }
    [PunRPC]
    public void TakeDamage(int damage)
    {
        anim.SetTrigger("Hurt");
        Debug.Log("Enemy Damaged.");
        curHp -= damage;
        // update the health bar
        healthBar.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
        if (curHp <= 0)
        {
            MissionManager.instance.KillEnemy();
            Die();
        }
        else
        {
            photonView.RPC("FlashDamage", RpcTarget.All);
            SoundController.instance.PlaySound(AS, hurt);
        }
    }
    [PunRPC]
    void FlashDamage()
    {
        StartCoroutine(DamageFlash());
        IEnumerator DamageFlash()
        {
            sr.material = hurtMat;
            yield return new WaitForSeconds(0.05f);
            sr.material = normalMat;
        }
    }
    void Die()
    {
        int rand = Random.Range(1, 11);
        if (rand == 7 || rand == 6 || rand == 5)
        {
            objectToSpawnOnDeath = "ShardBox";
        }
        else if (rand == 10)
        {
            objectToSpawnOnDeath = "AmmoBox";
        }
        else if (rand == 9)
        {
            objectToSpawnOnDeath = "HealthPack";
        }
        else if (rand == 8)
        {
            objectToSpawnOnDeath = "ShieldPickup";
        }
        else
        {
            objectToSpawnOnDeath = string.Empty;
        }
        if (objectToSpawnOnDeath != string.Empty)
            PhotonNetwork.Instantiate(objectToSpawnOnDeath, transform.position, transform.rotation);
        if (deadEnemyName != string.Empty)
        {
            PhotonNetwork.Instantiate(deadEnemyName, transform.position, transform.rotation);
        }
        // destroy the object across the network
        SoundController.instance.PlaySound(AS, death);
        PhotonNetwork.Destroy(gameObject);
    }
}
