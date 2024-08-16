using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    private enum FollowType
    {
        Disabled,
        Snap,
        Smooth
    }
    
    [Header("Follow Target")]
    [SerializeField] private FollowType followType;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    
    [Header("Smooth Follow Settings")]
    
    [Header("Position Settings")]
    [SerializeField] private bool lerpPosition = true;
    [SerializeField] private AnimationCurve lerpPositionCurve;
    [SerializeField] private float smoothFollowPositionSpeed = 1f;
    
    [Header("Rotation Settings")]
    [SerializeField] private bool lerpRotation = true;
    [SerializeField] private AnimationCurve lerpRotationCurve;
    [SerializeField] private float smoothFollowRotationSpeed = 1f;
    
    void Update()
    {
        switch (followType)
        {
            case FollowType.Snap:
                SnapFollow();
                break;
            case FollowType.Smooth:
                SmoothFollow();
                break;
            case FollowType.Disabled:
                return;
            default:
                Debug.LogError("Follow type not supported, please select a valid follow type");
                return;
        }
    }

    private void SnapFollow()
    {
        if (lerpPosition)
        {
            transform.position = target.position + offset;
        }

        if (lerpRotation)
        {
            transform.rotation = target.rotation;
        }
    }

    private void SmoothFollow()
    {
        if (lerpPosition)
        {
            float distance = Vector3.Distance(transform.position, target.position + offset);
            float positionCurveValue = lerpPositionCurve.Evaluate(distance);
            
            transform.position = Vector3.Lerp(transform.position, target.position + offset, positionCurveValue * Time.deltaTime * smoothFollowPositionSpeed);
        }

        if (lerpRotation)
        {
            float angle = Quaternion.Angle(transform.rotation, target.rotation);
            float rotationCurveValue = lerpRotationCurve.Evaluate(angle);
            
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, rotationCurveValue * Time.deltaTime * smoothFollowRotationSpeed);
        }
    }
}
