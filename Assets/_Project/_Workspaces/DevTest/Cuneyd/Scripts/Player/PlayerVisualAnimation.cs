using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualAnimation : MonoBehaviour
{
    private InputHandler _inputHandler;
    private PlayerController _playerController;
    private FrameInput _frameInput;
    
    // Rotation speed and target angle
    private float _targetAngle = 0f;
    [SerializeField] private float angle = 45f; // Assuming 45 degrees for left/right movement
    [SerializeField] private float movementAnimationCurve;

    // Scaling factors
    private Vector3 _normalScale;
    private Vector3 _movingScale;

    [SerializeField] private Vector2 moveScale;
    [SerializeField] private Vector2 initialJumpScale;
    [SerializeField] private Vector2 finalJumpScale;
    
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float scaleLerpSpeed = 5f; // Speed for scale interpolation

    // Start is called before the first frame update
    void Start()
    {
        _inputHandler = GetComponentInParent<InputHandler>();
        _playerController = GetComponentInParent<PlayerController>();

        // Initialize scales
        _normalScale = transform.localScale;
        _movingScale = new Vector3(_normalScale.x * moveScale.x, _normalScale.y * moveScale.y, _normalScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        _frameInput = _inputHandler.GetFrameInput();

        Vector3 targetScale;

        if ((_frameInput.Move.x != 0) && _playerController.grounded)
        {
            _targetAngle = -_frameInput.Move.x * angle;
            targetScale = _movingScale; // Target scale when moving
        }
        else if (_frameInput.JumpHeld)
        {
            _targetAngle = 0f;
            if (_frameInput.JumpDown)
            {
                transform.localScale = initialJumpScale;
            }

            targetScale = finalJumpScale;
        }
        else
        {
            _targetAngle = 0f;
            targetScale = _normalScale; // Target scale when not moving
        }
        
        // Smoothly rotate towards the target angle
        float currentAngle = Mathf.LerpAngle(transform.eulerAngles.z, _targetAngle, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        // Smoothly interpolate the scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);
        
    }
    
    
}