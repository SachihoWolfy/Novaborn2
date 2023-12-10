using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public int curAmmo;
    public int maxAmmo;
    public int curFpAmmo;
    public int maxFpAmmo;
    public float bulletSpeed;
    public float shootRate;
    public float fpRate;
    public float fpTime;
    public bool fpActive;
    private float lastShootTime;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    private PlayerController player;
    public bool isFiring;
    public Animator anim;

    [Header("Sounds")]
    public AudioSource AS;
    public AudioSource AS2;
    public AudioClip Shoot;
    public AudioClip fpShoot;
    public AudioClip Fail;
    public AudioClip PowerGet;
    public AudioClip fpShootMP;


    void Awake()
    {
        // get required components
        player = GetComponent<PlayerController>();
    }
    public void TryShoot()
    {
        anim.SetBool("FP", false);
        if (curFpAmmo > 0) { fpActive = true; } else { fpActive = false; }
        // can we shoot?
        if ((curAmmo <= 0 || Time.time - lastShootTime < shootRate) && !fpActive)
        {
            Debug.Log("No Shooties");
            AS.PlayOneShot(Fail);
            return;
        }

        if (fpActive && Time.time - lastShootTime < fpRate && curFpAmmo > 0)
        {
            //Decrepit FP_Shoot function. FP Shooting is now done by TryRapidShoot()
        }
        else
        {
            curAmmo--;
            lastShootTime = Time.time;
            // update the ammo UI
            GameUI.instance.UpdateAmmoText();
            // spawn the bullet
            Debug.Log("Tried Shooting");
            player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, Camera.main.transform.forward);
            SoundController.instance.PlaySound(AS, Shoot);
            anim.SetTrigger("Shoot");
        }
    }
    public void TryRapidShoot()
    {
        if (curFpAmmo > 0) { fpActive = true; } else { fpActive = false; anim.SetBool("FP", false); }
        if (fpActive && !(Time.time - lastShootTime < fpRate) && curFpAmmo > 0)
        {
            curFpAmmo--;
            lastShootTime = Time.time;
            // update the ammo UI
            GameUI.instance.UpdateFPAmmo();
            // spawn the bullet
            Debug.Log("Tried Shooting");
            player.photonView.RPC("SpawnBullet", RpcTarget.All, bulletSpawnPos.transform.position, Camera.main.transform.forward);
            if (!AS2.isPlaying && isFiring)
            {
                AS2.Play(0);
                SoundController.instance.PlaySound(AS, fpShootMP);
                isFiring = true;
                anim.SetBool("FP", true);
            }
        }
    }
    public void stopFiring()
    {
        if ((!isFiring && AS.isPlaying) || !fpActive)
        {
            AS2.Stop();
        }
    }
    public void setIsFiring(bool x)
    {
        isFiring = x;
    }
    [PunRPC]
    void SpawnBullet(Vector3 pos, Vector3 dir)
    {
        // spawn and orientate it
        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;
        // get bullet script
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        // initialize it and set the velocity
        bulletScript.Initialize(damage, player.id, player.photonView.IsMine);
        bulletScript.rig.velocity = dir * bulletSpeed;
    }
    [PunRPC]
    public void GiveAmmo(int ammoToGive)
    {
        curAmmo = Mathf.Clamp(curAmmo + ammoToGive, 0, maxAmmo);
        curFpAmmo = Mathf.Clamp(curFpAmmo + maxFpAmmo, 0, maxFpAmmo);
        // update the ammo text
        GameUI.instance.UpdateAmmoText();
        GameUI.instance.UpdateFPAmmo();
        AS.PlayOneShot(PowerGet);
    }

    //Implement giveExplosive
    //Implement TryExplosiveShot

    //Implement weaponModes somewhere
}
