using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Speed FOV")]
    public Rigidbody playerRB;
    public float interpTime;
    public float multiplier;
    [Header("Look Sensitivity")]
    public float sensX;
    public float sensY;
    [Header("Clamping")]
    public float minY;
    public float maxY;
    [Header("Spectator")]
    public float spectatorMoveSpeed;
    private float rotX;
    private float rotY;
    private bool isSpectator;
    private float fovStandard = 60;

    // Start is called before the first frame update
    void Start()
    {
        // lock the cursor to the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }


    void LateUpdate()
    {
        // get the mouse movement inputs
        rotX += Input.GetAxis("Mouse X") * sensX;
        rotY += Input.GetAxis("Mouse Y") * sensY;

        // clamp the vertical rotation
        rotY = Mathf.Clamp(rotY, minY, maxY);
        // are we spectating?
        if (isSpectator)
        {
            // rotate the cam vertically
            transform.rotation = Quaternion.Euler(-rotY, rotX, 0);
            // movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = 0;
            if (Input.GetKey(KeyCode.E))
                y = 1;
            else if (Input.GetKey(KeyCode.Q))
                y = -1;
            Vector3 dir = transform.right * x + transform.up * y + transform.forward * z;
            transform.position += dir * spectatorMoveSpeed * Time.deltaTime;
        }
        else
        {
            // rotate the camera vertically
            transform.localRotation = Quaternion.Euler(-rotY, 0, 0);
            // rotate the player horizontally. This is funny
            transform.parent.parent.rotation = Quaternion.Euler(transform.rotation.x, rotX, 0);
        }
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, (playerRB.velocity.magnitude / 10) * 10 + fovStandard, playerRB.velocity.magnitude / 10);


    }
    public void SetAsSpectator()
    {
        isSpectator = true;
        transform.parent = null;
        Camera.main.fieldOfView = fovStandard;
    }


}
