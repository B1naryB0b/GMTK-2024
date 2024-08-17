using System;
using UnityEngine;

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public Vector2 Move;
}
public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public Vector2 FrameInput { get; }
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private ScriptableStats _stats;
    private InputHandler _inputHandler;
    private Rigidbody2D _rigidBod;
    private CapsuleCollider2D _capCol;
    private TrailRenderer _dashTrail;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    private float _time;

    //implament interface
    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    //colision Variables
    private float _frameLeftGround = float.MinValue;
    private bool _grounded;
    //jumping variables
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGround + _stats.CoyoteTime;
    //Dash Variables
    private bool _dashing;
    private bool _dashUsable;
    private bool _dashToConsume;
    private float _timeDashPressed;
    private float _dashCooldownTime;
    private Vector2 _dashDirection;
    private bool CanUseDash => _dashUsable &&_time < _timeDashPressed + _stats.DashTime;


    private bool _bufferedWallJumpUsable;
    private bool HasBufferedWallJump => _bufferedWallJumpUsable && _time < _timeJumpWasPressed + _stats.WallJumpBuffer;
    private bool CanWallJump => _wallHit && !_grounded && HasBufferedWallJump;
    private bool _wallHit;
    private Vector2 _wallNormal;
    
    
    
    // Start is called before the first frame update
    void Awake()
    {
        _inputHandler = GetComponent<InputHandler>();
        _rigidBod = GetComponent<Rigidbody2D>();
        _capCol = GetComponent<CapsuleCollider2D>();
        _dashTrail = GetComponent<TrailRenderer>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Start()
    {
        _dashTrail.emitting = false;

        _rigidBod.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rigidBod.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        GatherInputs();
        Debug.Log("update");
    }

    private void GatherInputs()
    {
        _frameInput = _inputHandler.GetFrameInput();

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }

        if (_frameInput.DashDown)
        {
            _dashToConsume = true;
            _timeDashPressed = _time;
        }

    }

    private void FixedUpdate()
    {
        CheckCollisions();

        HandleDash();
        if (!_dashing)
        {
            HandleJump();
            HandleDirection();
            HandleWallJump();
            HandleGravity();
        }

        ApplyMovement();
    }
    

    //COllisions
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        //ground and roof
        bool groundHit = CastInDirection(Vector2.down);
        bool roofHit = CastInDirection(Vector2.up);

        RaycastHit2D wallHitLeft = CastInDirection(Vector2.left);
        RaycastHit2D wallHitRight = CastInDirection(Vector2.right);
        _wallHit = wallHitLeft.collider != null || wallHitRight.collider != null;

        //hit a roof
        if (roofHit)
        {
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
        }

        //landed
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
        }
        // Falling
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGround = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        if (_wallHit)
        {
            _frameVelocity.x = 0f;
            _wallNormal = wallHitLeft.collider != null ? wallHitLeft.normal : wallHitRight.normal;
            _bufferedWallJumpUsable = true;

        }
        
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    private RaycastHit2D CastInDirection(Vector2 direction)
    {
        return Physics2D.CapsuleCast(_capCol.bounds.center, _capCol.size, _capCol.direction, 0, direction, _stats.GrounderDistance, ~_stats.PlayerLayer);
    }
    
    //Dashing
    private void HandleDash()
    {
        if (_time >= _dashCooldownTime)
        {
            _dashUsable = true;
        }
        
        if (!_dashToConsume) return;

        if (CanUseDash)
        {
            _dashUsable = false;
            _dashing = true;
            _dashDirection = _frameInput.Move.normalized;
            _frameVelocity = new Vector2(_dashDirection.x * _stats.DashSpeed, _dashDirection.y * _stats.DashSpeed);
            _dashTrail.emitting = true;
            _dashCooldownTime = _time + _stats.DashCooldown;
        }

        if(_dashing)
        {

            if (_time >= _timeDashPressed + _stats.DashTime)
            {
                _dashing = false;
                _timeDashPressed = 0;
                _dashDirection = new Vector2(0, 0);
                _dashTrail.emitting = false;
            }
        }
    }

    //Jumping
    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rigidBod.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    //RunningDirection
    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
    }
    
    //WallJump
    private void HandleWallJump()
    {
        if (CanWallJump)
        {
            ExecuteWallJump();
        }
    }

    private void ExecuteWallJump()
    {
        Vector2 jumpNormalizer = _stats.WallJumpDistribution.normalized;
        _frameVelocity = new Vector2(_wallNormal.x * jumpNormalizer.x * _stats.WallJumpPower, jumpNormalizer.y * _stats.WallJumpPower);
        _wallHit = false;
        _bufferedWallJumpUsable = false;
        Jumped?.Invoke();
    }

    

    //Gravity
    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    private void ApplyMovement()
    {
        _rigidBod.velocity = _frameVelocity;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}
