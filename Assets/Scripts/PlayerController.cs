using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class PlayerController : MonoBehaviourPun
{
    [Header("Stats")]
    public float moveSpeed;
    public float jumpForce;
    public float sprintSpeed;
    public float shieldRate;
    public float iTime;
    private int shardcollected;

    [Header("Components")]
    public Rigidbody rig;
    public ParticleSystem ps;
    [Header("Arms and Headlight")]
    public GameObject arms;
    public GameObject head;
    public GameObject shieldBlock;
    public Material shieldDownMat;
    public Material shieldUpMat;
    public GameObject DeadBody;
    [Header("Look Sensitivity")]
    public float sensX;
    public float sensY;
    [Header("Clamping")]
    public float minY;
    public float maxY;
    [Header("Spectator")]
    private float rotX;
    private float rotY;

    [Header("Photon")]

    public int id;
    public Player photonPlayer;

    [Header("Health 'N Things")]

    private int curAttackerId;
    public int curHp;
    public int maxHp;
    public int curShield;
    public int maxShield;
    public int kills;
    public bool dead;
    private bool flashingDamage;
    private float lastShieldTime;
    private float lastDamageTime;
    public MeshRenderer mr;
    public PlayerWeapon weapon;
    [Header("Sounds")]
    public AudioSource AS;
    public AudioClip hurt;
    public AudioClip jump;
    public AudioClip heal;
    public AudioClip largeJump;
    public AudioClip shieldGet;
    public AudioClip shieldHurt;
    public AudioClip shieldBreak;
    [Header("Animation")]
    public Animator anim;
    [Header("NameTag")]
    [SerializeField] private TextMeshProUGUI nameText;

    // Start is called before the first frame update
    void Start()
    {
        lastShieldTime = Time.time;
        if (photonView.IsMine)
        {
            nameText.enabled = false;
            ps.Stop(false);
            return;
        }
        SetName();
    }

    public void SetName()
    {
        nameText.text = photonView.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine || dead)
            return;
        Move();
        if (Input.GetKeyDown(KeyCode.Space))
            TryJump();
        if (Input.GetMouseButtonDown(0))
            weapon.TryShoot();
        if (Input.GetMouseButton(0))
        {
            weapon.TryRapidShoot();
            weapon.setIsFiring(true);
        }
        else
        {
            weapon.setIsFiring(false);
            weapon.stopFiring();
            anim.SetBool("FP", false);
            if (weapon.AS2.isPlaying) { weapon.AS2.Stop(); anim.SetBool("FP", false); }
        }
        if (Input.GetMouseButtonDown(1))
            TryLargeJump();
        if (curShield > 1 && Time.time - lastShieldTime > shieldRate && Time.time - lastDamageTime > shieldRate)
        {
            //Debug.Log("Trying to regen shield.");
            lastShieldTime = Time.time;
            curShield = Mathf.Clamp(curShield + 20, 0, maxShield);
            GameUI.instance.UpdateShieldBar();
        }
        else
        {
            //Debug.Log("Shield regen unable to at the moment. " + (Time.time - lastShieldTime<shieldRate));
        }
    }
    void Move()
    {
        // get the input axis
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 dir;
        // calculate a direction relative to where we're facing
        if (Input.GetKey(KeyCode.LeftShift))
        {
            dir = (transform.forward * z + transform.right * x) * sprintSpeed;
        }
        else
        {
            dir = (transform.forward * z + transform.right * x) * moveSpeed;
        }
        dir.y = rig.velocity.y;
        // set that as our velocity
        rig.velocity = dir;
    }
    void TryJump()
    {
        // create a ray facing down
        Ray ray = new Ray(transform.position, Vector3.down);
        // shoot the raycast
        if (Physics.Raycast(ray, 1.5f))
        {
            rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            SoundController.instance.PlaySound(AS, jump);
        }
            
    }
    void TryLargeJump()
    {
        // create a ray facing down
        Ray ray = new Ray(transform.position, Vector3.down);
        // shoot the raycast
        if (Physics.Raycast(ray, 1.5f))
        {
            float ogMovespeed = moveSpeed;
            rig.AddForce(Vector3.up * jumpForce * 1.7f, ForceMode.Impulse);
            rig.AddForce(Vector3.forward * jumpForce, ForceMode.VelocityChange);
            SoundController.instance.PlaySound(AS, largeJump);
            while (!Physics.Raycast(ray, 1.5f)) { moveSpeed = ogMovespeed * 4f; }
            moveSpeed = ogMovespeed;
        }

    }

    private void LateUpdate()
    {
        rotateArms();
    }

    public void rotateArms()
    {
        if (photonView.IsMine)
        {
            // get the mouse movement inputs
            rotX += Input.GetAxis("Mouse X") * sensX;
            rotY += Input.GetAxis("Mouse Y") * sensY;

            // clamp the vertical rotation
            rotY = Mathf.Clamp(rotY, minY, maxY);
            // rotate the camera vertically
            arms.transform.localRotation = Quaternion.Euler(-rotY, 0, 0);
        }
    }

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;
        GameManager.instance.players[id - 1] = this;

        // is this not our local player?
        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
        else
        {
            GameUI.instance.Initialize(this);
            head.SetActive(false);
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        anim.SetBool("FP", false);
        if (dead)
        {
            return;
        }
        if(Time.time - lastDamageTime < iTime)
        {
            Debug.Log("invicibility frames");
            return;
        }
        lastDamageTime = Time.time;
        Debug.Log("Trying To Take Damage");
        if (curShield > 0)
        {
            SoundController.instance.PlaySound(AS, shieldHurt);
            curShield = Mathf.Clamp(curShield - damage, 0, curShield);
            if(curShield < 1)
            {
                SoundController.instance.PlaySound(AS, shieldBreak);
                photonView.RPC("DeactivateShield", RpcTarget.All);
            }
        }
        else
        {
            SoundController.instance.PlaySound(AS, hurt);
            curHp -= damage;
        }
        curAttackerId = attackerId;
        // flash the player red
        photonView.RPC("DamageFlash", RpcTarget.Others);
        anim.SetTrigger("Hurt");
        // update the health bar UI
        GameUI.instance.UpdateHealthBar();
        GameUI.instance.UpdateShieldBar();
        // die if no health left
        if (curHp <= 0)
            photonView.RPC("Die", RpcTarget.All);
    }
    [PunRPC]
    void DamageFlash()
    {
        if (flashingDamage)
            return;
        StartCoroutine(DamageFlashCoRoutine());
        IEnumerator DamageFlashCoRoutine()
        {
            flashingDamage = true;
            Color defaultColor = mr.material.color;
            if (curShield > 0)
            {
                mr.material.color = Color.blue;
            }
            else
            {
                mr.material.color = Color.red;
            }
            yield return new WaitForSeconds(0.1f);
            mr.material.color = defaultColor;
            flashingDamage = false;
        }
    }
    [PunRPC]
    void Die()
    {
        anim.SetBool("FP", false);
        curHp = 0;
        dead = true;
        GameManager.instance.alivePlayers--;
        // host will check win condition
        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.CheckWinCondition();
        // is this our local player?
        if (photonView.IsMine)
        {
            if (curAttackerId != 0 && curAttackerId != -1)
                GameManager.instance.GetPlayer(curAttackerId).photonView.RPC("AddKill", RpcTarget.All);
            // set the cam to spectator
            GetComponentInChildren<CameraController>().SetAsSpectator();
            // disable the physics and hide the player
            rig.isKinematic = true;
            photonView.RPC("SpawnDeadBody", RpcTarget.All);
            transform.position = new Vector3(0, -50, 0);
        }
    }
    [PunRPC]
    public void SpawnDeadBody()
    {
        GameObject deadBody = Instantiate(DeadBody, transform.position, Quaternion.identity);
        deadBody.transform.forward = this.transform.forward;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 dir;
        dir = (transform.forward * z + transform.right * x) * moveSpeed;
        deadBody.GetComponent<Rigidbody>().velocity = dir;
        deadBody.gameObject.transform.GetChild(1).rotation = arms.transform.rotation;
    }
    [PunRPC]
    public void AddKill()
    {
        kills++;
        GameUI.instance.UpdatePlayerInfoText();
    }
    [PunRPC]
    public void Heal(int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);
        AS.PlayOneShot(heal);
        // update the health bar UI
        GameUI.instance.UpdateHealthBar();
    }
    [PunRPC]
    public void Shield(int amountToShield)
    {
        curShield = Mathf.Clamp(curShield + amountToShield, 0, maxShield);
        lastShieldTime = Time.time;
        AS.PlayOneShot(shieldGet);
        GameUI.instance.UpdateShieldBar();
        photonView.RPC("ActivateShield", RpcTarget.All);
    }
    [PunRPC]
    public void ActivateShield()
    {
        shieldBlock.GetComponent<MeshRenderer>().material = shieldUpMat;
    }
    [PunRPC]
    public void DeactivateShield()
    {
        shieldBlock.GetComponent<MeshRenderer>().material = shieldDownMat;
    }
    [PunRPC]
    public void CollectedShards()
    {
        shardcollected++;
        LeaderBoard.instance.SetLeaderboardEntry(shardcollected);
        Debug.Log("This should work");
    }
    //Figure out how to launch the player with force. This may have something to do with the "Freeze Position" constraint on the rigidBody.
}
