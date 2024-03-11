using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad_Behavior : MonoBehaviour
{
    public Animator anim;
    private bool isUsed;
    public AudioSource AS;
    public AudioClip jumpPadSound;
    public float jumpForce;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isUsed)
        {
            anim.SetTrigger("Activate");
            AS.PlayOneShot(jumpPadSound);
            PlayerController player = other.GetComponent<PlayerController>();
            player.rig.velocity = new Vector3(player.rig.velocity.x, 0f, player.rig.velocity.z);
            player.rig.AddForce(this.transform.up * jumpForce, ForceMode.Impulse);
        }
    }
}
