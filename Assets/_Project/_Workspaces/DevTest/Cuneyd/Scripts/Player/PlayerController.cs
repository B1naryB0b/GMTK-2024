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
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(TrailRenderer))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    private enum PlayerMoveMode
    {
        Velocity,
        Force
    }

    [SerializeField] private PlayerMoveMode moveMode;
    
    [SerializeField] private ScriptableStats _stats;

    public DropletManager _dropletManager;
    private InputHandler _inputHandler;
    private Rigidbody2D _rigidBod;
    private CircleCollider2D _circleCol;
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
    public bool grounded => _grounded;
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
    public bool dashing => _dashing;
    private bool _dashUsable;
    private bool _dashToConsume;
    private float _timeDashPressed;
    private float _dashCooldownTime;
    private Vector2 _dashDirection;
    public Vector2 dashDirection => _dashDirection;
    private bool CanUseDash => _dashUsable &&_time < _timeDashPressed + _stats.DashTime && _largeEnoughToDash;
    private bool _largeEnoughToDash;

    private bool _bufferedWallJumpUsable;
    private bool HasBufferedWallJump => _bufferedWallJumpUsable && _time < _timeJumpWasPressed + _stats.WallJumpBuffer;
    private bool CanWallJump => _wallHit && !_grounded && HasBufferedWallJump;
    private bool _wallHit;
    public bool wallHit => _wallHit;
    private Vector2 _wallNormal;
    public Vector2 wallNormal => _wallNormal;

    private float _velocityScaling;
    
    // Start is called before the first frame update
    void Awake()
    {
        _dropletManager = GetComponent<DropletManager>();
        _inputHandler = GetComponent<InputHandler>();
        _rigidBod = GetComponent<Rigidbody2D>();
        _circleCol = GetComponent<CircleCollider2D>();
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

        AdjustVelocityForMass();
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
        return Physics2D.CircleCast(_circleCol.bounds.center, _circleCol.radius * transform.localScale.y, direction, _stats.GrounderDistance, ~_stats.PlayerLayer);
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
            _dashDirection = _frameInput.Move.normalized;
            Vector2 dashScaling = GetDashDirectionalScaling(_frameInput.Move);

            if (_dashDirection != Vector2.zero)
            {
                _dashUsable = false;
                _dashing = true;
                _frameVelocity = new Vector2(_dashDirection.x * dashScaling.x * _stats.DashSpeed, _dashDirection.y * dashScaling.y * _stats.DashSpeed);
                _dashTrail.emitting = true;
                _dashCooldownTime = _time + _stats.DashCooldown;
                _dropletManager.SubtractMass(-dashDirection);
            }
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

    //PLEASE UNDER NO CIRCUMSTANCES TOUCH THIS FUNCTION ----------------------------------------------------------------------------------------------------------
    //I know this looks awful, but just trust me, it works
    private Vector2 GetDashDirectionalScaling(Vector2 moveDir)
    {
        // Round the moveDir to get consistent directional values
        moveDir.x = moveDir.x > 0 ? 1 : (moveDir.x < 0 ? -1 : 0);
        moveDir.y = moveDir.y > 0 ? 1 : (moveDir.y < 0 ? -1 : 0);

        Vector2 scaling = Vector2.zero;

        // Check for each direction
        if (moveDir.x == 1 && moveDir.y == 1)
        {
            // Up-Right (W + D)
            scaling = new Vector2(1, 1) * _stats.directionalScalingFactor.WD;
        }
        else if (moveDir.x == -1 && moveDir.y == 1)
        {
            // Up-Left (W + A)
            scaling = new Vector2(1, 1) * _stats.directionalScalingFactor.WA;
        }
        else if (moveDir.x == 1 && moveDir.y == -1)
        {
            // Down-Right (S + D)
            scaling = new Vector2(1, 1) * _stats.directionalScalingFactor.SD;
        }
        else if (moveDir.x == -1 && moveDir.y == -1)
        {
            // Down-Left (S + A)
            scaling = new Vector2(1, 1) * _stats.directionalScalingFactor.SA;
        }
        else if (moveDir.x == 1)
        {
            // Right (D)
            scaling = new Vector2(1, 0) * _stats.directionalScalingFactor.D;
        }
        else if (moveDir.x == -1)
        {
            // Left (A)
            scaling = new Vector2(1, 0) * _stats.directionalScalingFactor.A;
        }
        else if (moveDir.y == 1)
        {
            // Up (W)
            scaling = new Vector2(0, 1) * _stats.directionalScalingFactor.W;
        }
        else if (moveDir.y == -1)
        {
            // Down (S)
            scaling = new Vector2(0, 1) * _stats.directionalScalingFactor.S;
        }

        return scaling;
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

    private void AdjustVelocityForMass()
    {
        float mass = _dropletManager.GetMass();
        _largeEnoughToDash = mass > 0f;
        
        _velocityScaling = -Mathf.Pow((mass - 1f) / 1.75f, 3f) + ((1 - mass) / 100f) + 1f;
    }

    private void ApplyMovement()
    {
        switch (moveMode)
        {
            case PlayerMoveMode.Force:
                _rigidBod.AddForce(_frameVelocity, ForceMode2D.Force);
                break;
            case PlayerMoveMode.Velocity:
                _rigidBod.velocity = _frameVelocity * _velocityScaling;
                break;
            default:
                Debug.LogError("Select a player movement mode");
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}
