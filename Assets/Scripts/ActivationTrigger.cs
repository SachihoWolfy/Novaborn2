using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationTrigger : MonoBehaviour
{
    public GameObject[] objectsToActivate;
    public bool isUsed;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject obj in objectsToActivate)
        {
            if(obj.activeSelf)
                obj.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUsed)
        {
            isUsed = true;
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(true);
            }
        }
    }
}
