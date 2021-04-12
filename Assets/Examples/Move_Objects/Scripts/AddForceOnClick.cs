using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceOnClick : MonoBehaviour {

    public float speed = 10.0f;
    Rigidbody rb;


    void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody> ();
    }

    void OnMouseDown ()
    {
        Debug.Log ("mouse clicked!");
        rb.AddForce (Vector3.up * speed);
    }


}
