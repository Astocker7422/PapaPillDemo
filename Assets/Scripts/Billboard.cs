using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls behavior of in world billboarded UI
//Attached to any billboarded UI
public class Billboard : MonoBehaviour
{
    void Update()
    {
        //Make object face the camera
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}
