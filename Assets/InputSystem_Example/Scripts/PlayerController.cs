using UnityEngine;
using UnityEngine.InputSystem;

/**
 *  Read input (new InputSystem) => send to PlayerMovement to control GameObject
 *  https://medium.com/codex/why-you-should-use-unitys-new-input-system-268773863c4 
 */

public class PlayerController : MonoBehaviour
{
    // input settings
    private InputControls inputControls;
    // script to control movement
    MovementController movementController;

    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool jumpInput;

    private void Awake()
    {
        // create instance of InputSystem settings
        inputControls = new InputControls();
        // assign reference
        movementController = GetComponent<MovementController>();
    }

    private void OnEnable()
    {
        // enable the entire action map
        inputControls.Player.Enable();

        // movement
        inputControls.Player.Move.performed += OnMoveInput;
        inputControls.Player.Move.canceled += OnMoveInput;

        // jump
        inputControls.Player.Jump.performed += OnJumpInput;
        inputControls.Player.Jump.canceled += OnJumpInput;
    }

    private void OnDisable()
    {
        // disable everything to prevent memory leaks

        inputControls.Player.Move.performed -= OnMoveInput;
        inputControls.Player.Move.canceled -= OnMoveInput;
        inputControls.Player.Jump.performed -= OnJumpInput;
        inputControls.Player.Jump.canceled -= OnJumpInput;

        inputControls.Player.Disable();
    }

    private void OnJumpInput(InputAction.CallbackContext context)
    {
        // access phases
        switch (context.phase)
        {
            case InputActionPhase.Started:
            case InputActionPhase.Performed:
                jumpInput = true;
                break;
            case InputActionPhase.Canceled:
                jumpInput = false;
                break;
        }

        // assign in movement script
        movementController.jumpInput = jumpInput;
    }

    void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            // read value
            moveInput = context.ReadValue<Vector2>();
        else
            moveInput = Vector2.zero;

        // assign in movement script
        movementController.moveInput = moveInput;
    }

}