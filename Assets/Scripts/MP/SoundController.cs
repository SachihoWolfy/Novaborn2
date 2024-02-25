using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SoundController : MonoBehaviourPun
{
    //This Experimental Class focuses on having different ways to play audioClips,
    //mainly to also have sound be played to everyone as well.
    public static SoundController instance;
    public AudioSource audioRPC;
    void Awake()
    {
        instance = this;
    }
    [PunRPC]
    private void PlaySoundRPC(Vector3 audioTrasform, string clipName)
    {
        AudioClip clipHolder = AudioClipGetter.instance.GetClip(clipName);
        audioRPC.transform.position = audioTrasform;
        audioRPC.spread = 0;
        audioRPC.spatialBlend = 1;
        audioRPC.minDistance = 25;
        audioRPC.maxDistance = 100;
        audioRPC.clip = clipHolder;
        audioRPC.PlayOneShot(clipHolder);
        Debug.Log("Played AudioClip over RPC successfully: " + clipName);
    }
    public void PlaySound(AudioSource AS, AudioClip clip)
    {
        //AS.PlayOneShot(clip);
        photonView.RPC("PlaySoundRPC", RpcTarget.All, AS.transform.position, clip.name);
    }
}
