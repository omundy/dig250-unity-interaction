using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *  Controls player movement only
 */

public class PlayerMovement : MonoBehaviour
{
    // movement references and checks
    private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    public LayerMask groundLayer;
    [SerializeField] private bool grounded;

    // movement variables
    public float moveSpeed = 4.5f;
    public float jumpForce = 4f;

    [Header("Input")]

    // input variables (values set by input scripts)
    public Vector2 moveInput;
    public bool jumpInput;

    private void Awake()
    {
        // assign references
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // update grounded status
        grounded = Physics.Raycast(groundCheck.position, Vector3.down, 0.05f, groundLayer);

        // if input received
        if (jumpInput && grounded)
        {
            Debug.Log("PlayerMovement.Jump()");
            // add vertical force
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
        // reset jump input
        jumpInput = false;

        // get current velocity
        Vector3 newVolocity = rb.velocity;
        newVolocity.x = moveInput.x * moveSpeed;
        newVolocity.z = moveInput.y * moveSpeed;
        rb.velocity = newVolocity;
    }

}
