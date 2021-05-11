using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCTRL : MonoBehaviour
{
    float yaw = 0f, pitch = 0f;
    public Transform player;
    public Vector3 camerraOffSet;
    float distanceToTarget = 4f;
    public float minPitch = -45f, maxPitch = 45f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X"); // acumulam deplasamentul mouselui la unghiurile facute de sys coord local cu axele lumii;
        pitch -= Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // limitam rotatia sus jos astfel incat sa nu se dea peste cap;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f); // aplicam rotatia
        Vector3 worldSpaceOffset = transform.TransformDirection(camerraOffSet); // trecem deplasamentul camerei relativ la personaj din spatiul local in spatiul lume
        
        transform.position = player.position +  worldSpaceOffset; // calculam pozitia camerei;
    }
}
