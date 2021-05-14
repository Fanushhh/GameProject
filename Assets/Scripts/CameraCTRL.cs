using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCTRL : MonoBehaviour
{
    float yaw = 0f, pitch = 0f;
    public Transform player;
    public Vector3 camerraOffSet;
    public float minPitch = -45f, maxPitch = 45f;


    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X"); 
        pitch -= Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); 
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f); 
        Vector3 worldSpaceOffset = transform.TransformDirection(camerraOffSet); 
        transform.position = player.position +  worldSpaceOffset; 
    }
}
