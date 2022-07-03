using UnityEngine;
using UnityEngine.InputSystem;

// this is in progress
// see: https://medium.com/codex/why-you-should-use-unitys-new-input-system-268773863c4 


public class HeroController : MonoBehaviour
{
    // input settings
    private PlayerInput playerInput;
    // input actions
    private InputAction moveAction;
    private InputAction lookAction;

    // to control player
    private Rigidbody rb;
    public float moveSpeed = 8f;
    public float jumpForce = 200f;
    float movement;
    public bool grounded;
    [SerializeField] private Transform groundCheck;
    public LayerMask groundLayer;

    private void Awake()
    {
        // assign references
        rb = GetComponent<Rigidbody>();
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        // assign references and enable actions
        moveAction = playerInput.Player.Move;
        moveAction.Enable();
        lookAction = playerInput.Player.Look;
        lookAction.Enable();

        // assign method to "performed" event
        playerInput.Player.Jump.performed += OnJump;
        playerInput.Player.Jump.Enable();
    }

    private void OnDisable()
    {
        // disable everything to prevent memory leaks
        moveAction.Disable();
        lookAction.Disable();
        playerInput.Player.Jump.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (grounded)
        {
            Debug.Log("Jump!");
            // add vertical force on
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    private void FixedUpdate()
    {
        grounded = Physics.Raycast(
            groundCheck.position, Vector3.down,
            0.05f, groundLayer);

        Vector2 moveDir = moveAction.ReadValue<Vector2>();
        Vector3 vel = rb.velocity;
        vel.x = moveSpeed * moveDir.x;
        vel.z = moveSpeed * moveDir.y;
        rb.velocity = vel;

        //Vector2 lookDir = lookAction.ReadValue<Vector2>();
        //Debug.Log($"Looking, direction = {lookDir}");
    }

}