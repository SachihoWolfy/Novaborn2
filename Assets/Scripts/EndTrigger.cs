using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    bool hasBeenTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            MissionManager.instance.enterEnd();
        }
    }
}
