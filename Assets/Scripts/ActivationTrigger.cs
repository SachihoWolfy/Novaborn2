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
                if(obj != null)
                    if (obj.gameObject.GetComponent<EnemySpawner>() != null)
                         obj.gameObject.GetComponent<EnemySpawner>().isEnabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUsed)
        {
            isUsed = true;
            if (other.gameObject != null)
                foreach (GameObject obj in objectsToActivate)
                {
                    if (obj != null)
                        if (obj.gameObject.GetComponent<EnemySpawner>() != null)
                            obj.gameObject.GetComponent<EnemySpawner>().isEnabled = true;
                        else Debug.LogError("ACTIVATION ERROR: Object is NULL");
                }
            else Debug.LogError("ACTIVATION ERROR 2, Player is NULL");
        }
    }
}
