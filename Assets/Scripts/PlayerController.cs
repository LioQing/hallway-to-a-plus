using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;
    public float lookSensitivity = 0.15f;
    public float jumpHeight = 0.4f;
    public float gravity = -9.81f;
    public bool shouldUpdate = true;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private bool _jumpInput;
    private bool _isRunning;
    private float _verticalRotation;
    private float _verticalVelocity;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (!shouldUpdate)
        {
            return;
        }
        
        if (_controller.isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = -2f;
        }

        if (_controller.isGrounded && _jumpInput)
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            _jumpInput = false;
        }

        _verticalVelocity += gravity * Time.deltaTime;

        var moveDelta = new Vector3(_moveInput.x, 0f, _moveInput.y);
        var currentSpeed = _isRunning ? runSpeed : walkSpeed;
        var move = transform.TransformDirection(moveDelta) * currentSpeed;
        
        move.y = _verticalVelocity;
        
        _controller.Move(move * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
    
    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (Cursor.visible || Cursor.lockState != CursorLockMode.Locked || PauseMenu.Paused || !shouldUpdate)
        {
            return;
        }
        
        var lookInput = ctx.ReadValue<Vector2>();
        var mouseDelta = lookInput * lookSensitivity;

        transform.Rotate(Vector3.up, mouseDelta.x);

        _verticalRotation -= mouseDelta.y;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -90f, 90f);
        
        Camera.main!.transform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        _jumpInput = ctx.ReadValueAsButton();
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        _isRunning = ctx.ReadValueAsButton();
    }
}