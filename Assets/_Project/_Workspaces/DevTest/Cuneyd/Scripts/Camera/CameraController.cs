using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    //exposed variables
    [Header("Camera Modes")]
    [SerializeField] private bool snapToPlayer;
    [SerializeField] private bool trackYAxis;

    [Header("Camera Parameters")]
    [SerializeField] private float smoothTime;
    [SerializeField] private float minimumYTrackingGap;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float lookAheadAmount;

    //private variables
    private Vector3 _targetPosition;
    private Vector3 _currentVelocity = Vector3.zero;
    private float _lookAheadDir;
    private float _faceDir;

    private void Awake()
    {
        if (playerTransform != null)
        {
            _targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z);
        }
        else
        {
            Debug.Log("No controller");
        }  
    }

    void LateUpdate()
    {
        _targetPosition.x = playerTransform.position.x;
        if (trackYAxis || playerTransform.position.y < _targetPosition.y)
        {
            _targetPosition.y = playerTransform.position.y;
        }
        //Vector3 adjustedPosition = _targetPosition + cameraOffset;
        if (snapToPlayer)
        {
            transform.position = _targetPosition + new Vector3(cameraOffset.x * _faceDir, cameraOffset.y, 0.0f);
        }
        else
        {
            Vector3 adjustedPosition = _targetPosition + new Vector3(cameraOffset.x * _faceDir + _lookAheadDir * lookAheadAmount, cameraOffset.y, 0.0f);
            transform.position = Vector3.SmoothDamp(transform.position, adjustedPosition, ref _currentVelocity, smoothTime);
        }
    }

    public void UpdatePlayerY(float yCoord)
    {
        if (yCoord - _targetPosition.y > minimumYTrackingGap)
        {
            _targetPosition.y = yCoord;
        }
    }

    public void SetLookAhead(float lookAheadMultiplier)
    {
        _lookAheadDir = lookAheadMultiplier;
        if (lookAheadMultiplier != 0.0f)
        {
            _faceDir = lookAheadMultiplier;
        }
    }
}
