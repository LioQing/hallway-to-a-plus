using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSensitivity = 0.2f;
    public float jumpHeight = 1f;
    public float gravity = -9.81f;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private bool _jumpInput;
    private float _verticalRotation;
    private float _verticalVelocity;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
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
        var move = transform.TransformDirection(moveDelta) * moveSpeed;
        
        move.y = _verticalVelocity;
        
        _controller.Move(move * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
    }
    
    public void OnLook(InputAction.CallbackContext ctx)
    {
        if (Cursor.lockState != CursorLockMode.Locked)
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
}