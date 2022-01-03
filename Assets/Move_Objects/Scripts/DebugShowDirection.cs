using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugShowDirection : MonoBehaviour {


    LineRenderer lineRenderer;
    Rigidbody rb;
    public float factor = 1f;

    void Start ()
    {
        rb = GetComponent<Rigidbody> ();
        lineRenderer = GetComponent<LineRenderer> ();
    }

    void Update ()
    {
        DrawLine ();

        //Debug.Log (rb.velocity);
    }

    void DrawLine ()
    {
        lineRenderer.SetPosition (0, transform.position);
        lineRenderer.SetPosition (1, rb.velocity * factor + transform.position);
    }

}
