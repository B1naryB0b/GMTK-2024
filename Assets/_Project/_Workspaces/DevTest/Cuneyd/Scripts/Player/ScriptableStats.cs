using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableStats : ScriptableObject
{
    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("INPUT")]
    [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
    public bool SnapInput = true;

    [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
    public float VerticalDeadZoneThreshold = 0.3f;

    [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
    public float HorizontalDeadZoneThreshold = 0.1f;

    [Header("MOVEMENT")]
    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float AirDeceleration = 30;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
    public float GrounderDistance = 0.05f;

    [Header("JUMP")]
    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("The maximum vertical movement speed")]
    public float MaxFallSpeed = 40;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
    public float FallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = .2f;

    [Header("DASH")]
    [Tooltip("How fast is the dash")]
    public float DashSpeed = 10.0f;

    [Tooltip("How Long is the Dash")]
    public float DashTime = 1f;
    
    [Tooltip("How long before you can dash again")]
    public float DashCooldown = 2f;

    [Tooltip("Controls the scaling of the dash in a given direction")]
    public Directions directionalScalingFactor; 
    
    [Serializable]
    public class Directions
    {
        [Range(0f, 1f)] public float W = 1f;       // Up
        [Range(0f, 1f)] public float WD = 1f;      // Up-Right
        [Range(0f, 1f)] public float D = 1f;       // Right
        [Range(0f, 1f)] public float SD = 1f;      // Down-Right
        [Range(0f, 1f)] public float S = 1f;       // Down
        [Range(0f, 1f)] public float SA = 1f;      // Down-Left
        [Range(0f, 1f)] public float A = 1f;       // Left
        [Range(0f, 1f)] public float WA = 1f;      // Up-Left
    }
    
    [Header("WALL JUMP")]
    [Tooltip("The immediate velocity applied when wall jumping")]
    public float WallJumpPower = 36f;

    [Tooltip("How much jump force is distributed along the horizontal axis")]
    [Range(0f, 1f), SerializeField]
    private float wallJumpX = 1f;
    
    [Tooltip("How much jump force is distributed along the vertical axis")]
    [Range(0f, 1f), SerializeField] 
    private float wallJumpY = 1f;

    public Vector2 WallJumpDistribution => new Vector2(wallJumpX, wallJumpY);
    
    [Tooltip("The amount of time we buffer a wall jump. This allows jump input before actually hitting the wall")]
    public float WallJumpBuffer = .2f;
    
    
}
