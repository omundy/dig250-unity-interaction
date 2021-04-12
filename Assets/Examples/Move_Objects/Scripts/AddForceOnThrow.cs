using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceOnThrow : MonoBehaviour {

    Rigidbody rb;
    public float speed = 10.0f;
    Vector3 mOffset;
    float mZCoord;
    Vector3 lastPos;
    public Vector3 velocity;

    void Start ()
    {
        rb = gameObject.GetComponent<Rigidbody> ();
    }

    void OnMouseDown ()
    {
        // update mouse Z
        mZCoord = Camera.main.WorldToScreenPoint (gameObject.transform.position).z;
        // store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseWorldPos ();
    }

    void OnMouseDrag ()
    {
        Debug.Log ("OnMouseDrag()");
        // set position based on mouse pos in world
        transform.position = GetMouseWorldPos () + mOffset;
    }

    void OnMouseUp ()
    {
        // "let go" by adding force based on current "velocity"
        rb.AddForce (velocity * speed, ForceMode.Force);
    }

    private Vector3 GetMouseWorldPos ()
    {
        // pixel coordinates (x, y)
        Vector3 mousepoint = Input.mousePosition;
        // z coordinates of gameobject on screen
        mousepoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint (mousepoint);
    }

    void Update ()
    {
        // update velocity
        velocity = (rb.position - lastPos) * 50;
        lastPos = rb.position;
        Debug.Log (velocity);
    }
}
