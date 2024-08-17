using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private InputActions _inputActions;
    private FrameInput _frameInput;

    private void Awake()
    {
        _inputActions = new InputActions();
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Update()
    {
        _frameInput = new FrameInput
        {
            JumpDown = _inputActions.Player.Jump.WasPressedThisFrame(),
            JumpHeld = _inputActions.Player.Jump.IsPressed(),
            DashDown = _inputActions.Player.Dash.WasPressedThisFrame(),
            Move = _inputActions.Player.Move.ReadValue<Vector2>()
        };
    }

    public FrameInput GetFrameInput()
    {
        return _frameInput;
    }
}