using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * 5f);
        rb.AddForce(transform.forward * -5f);
        rb.AddTorque(transform.right * -200f, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
