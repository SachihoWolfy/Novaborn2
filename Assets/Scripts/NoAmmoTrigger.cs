using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoAmmoTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    bool hasBeenTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            other.GetComponent<PlayerWeapon>().curAmmo = 0;
            other.GetComponent<PlayerWeapon>().maxAmmo = 5;
            other.GetComponent<PlayerWeapon>().ForceFlickerRifle();
            hasBeenTriggered = true;
        }
    }
}
