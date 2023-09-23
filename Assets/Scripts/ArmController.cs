using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ArmController : MonoBehaviour
{
    [Header("Look Sensitivity")]
    public float sensX;
    public float sensY;
    [Header("Clamping")]
    public float minY;
    public float maxY;
    [Header("Spectator")]
    private float rotX;
    private float rotY;


    public void rotateArms(GameObject arms)
    {
        // get the mouse movement inputs
        rotX += Input.GetAxis("Mouse X") * sensX;
        rotY += Input.GetAxis("Mouse Y") * sensY;

        // clamp the vertical rotation
        rotY = Mathf.Clamp(rotY, minY, maxY);
        // rotate the camera vertically
        transform.localRotation = Quaternion.Euler(-rotY, 0, 0);
        // rotate the player horizontally
        transform.parent.rotation = Quaternion.Euler(transform.rotation.x, rotX, 0);
    }


}
