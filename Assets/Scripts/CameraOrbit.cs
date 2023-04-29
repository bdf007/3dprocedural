using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public float lookSensitivity;
    public float minXLook;
    public float maxXLook;
    public Transform camAnchor;
    public AudioSource backgroundSound;

    public bool invertXRotation;

    private float curXRot;

    private void Start()
    {
        // confine the cursor to the center of the game mode and disable the cursor (press ESC to make the cursor visible again)
        //Cursor.lockState = CursorLockMode.Locked;
    }
    // called at the end of each frame
    void LateUpdate()
    {
        // get the mouse X and Y imputs
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // rotate the player horizontally
        transform.eulerAngles += lookSensitivity * mouseX * Vector3.up;
        // enable inverted camera
        if (invertXRotation)
        {
            // modify curXRot to move camera up and down
            curXRot += lookSensitivity * mouseY;
        }
        // disable inverted camera
        else
        {
            curXRot -= lookSensitivity * mouseY;
        }
        // restrict the curXRot to be in between minXLook and maxXLook
        curXRot = Mathf.Clamp(curXRot, minXLook, maxXLook);

        // Store the angle of camAnchor in a temporary Vector3 variable named clampedAngle
        Vector3 clampedAngle = camAnchor.eulerAngles;
        // Apply the current X rotaation to clampedAngle on the x-axis
        clampedAngle.x = curXRot;
        // and apply the clampedAngle to the camAnchor
        camAnchor.eulerAngles = clampedAngle;

    }
}
