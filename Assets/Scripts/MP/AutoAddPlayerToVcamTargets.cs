using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//Got this from https://forum.unity.com/threads/add-spawned-object-to-cinemachine.688687/
public class AutoAddPlayerToVcamTargets : MonoBehaviour
{
    [TagField]
    public string Tag = string.Empty;
    public string Tag2 = string.Empty;

    // Start is called before the first frame update
    void Start()
    {
        var vcam = GetComponent<CinemachineVirtualCameraBase>();
        if (vcam != null && Tag.Length > 0)
        {
            var targets = GameObject.FindGameObjectsWithTag(Tag);
            if (targets.Length > 0)
                vcam.LookAt = vcam.Follow = targets[0].transform;
        }
    }
}
