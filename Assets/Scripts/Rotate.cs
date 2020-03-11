using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls the behavior of a rotating object
//Attached to any object that rotates
public class Rotate : MonoBehaviour
{
    //The rotation values
    public float x;
    public float y;
    public float z;

    void Update ()
    {
        //Rotate the object based on the input
        transform.Rotate(new Vector3(x, y, z) * Time.deltaTime);
	}
}
