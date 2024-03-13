using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneAnimationController : MonoBehaviour
{
    public Animator anim;
    public int animationNumber;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetInteger("AnimationNum", animationNumber);
    }
    private void OnEnable()
    {
        anim.SetInteger("AnimationNum", animationNumber);
    }
}
