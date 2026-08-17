using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : Singleton<GameInput>
{
    public event EventHandler OnJumpPerformed;
    public Vector3 MovementDirection => GetMovementDirection();
    public bool IsSprinting { get; private set; }

    private InputActions inputActions;

    void OnEnable()
    {
        inputActions = new InputActions();
        inputActions.Player.Enable();

        inputActions.Player.Jump.performed += Jump_Performed;
        inputActions.Player.Sprint.performed += Sprint_Performed;
        inputActions.Player.Sprint.canceled += Sprint_Canceled;
    }

    private void Jump_Performed(InputAction.CallbackContext _context)
    {
        OnJumpPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Sprint_Performed(InputAction.CallbackContext _context)
    {
        IsSprinting = true;
    }

    private void Sprint_Canceled(InputAction.CallbackContext _context)
    {
        IsSprinting = false;
    }

    private Vector3 GetMovementDirection()
    {
        Vector2 inputDirection = inputActions.Player.Move.ReadValue<Vector2>();
        return new(inputDirection.x, 0, inputDirection.y);
    }

    void OnDisable()
    {
        inputActions.Player.Disable();

        inputActions.Player.Jump.performed -= Jump_Performed;
        inputActions.Player.Sprint.performed -= Sprint_Performed;
        inputActions.Player.Sprint.canceled -= Sprint_Canceled;
    }
}
