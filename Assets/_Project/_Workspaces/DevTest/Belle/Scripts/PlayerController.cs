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

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(TrailRenderer))]
public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private ScriptableStats _stats;
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
    private Vector2 _mousePosition;
    private Vector2 _dashDirection;
    private bool CanUseDash => _dashUsable &&_time < _timeDashPressed + _stats.DashTime;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidBod = GetComponent<Rigidbody2D>();
        _capCol = GetComponent<CapsuleCollider2D>();
        _dashTrail = GetComponent<TrailRenderer>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
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
        if (!_dashing)
        {
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }


        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
            DashDown = Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        };

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
            HandleGravity();
        }

        ApplyMovement();
    }

    //COllisions
    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        //ground and roof
        bool groundHit = Physics2D.CapsuleCast(_capCol.bounds.center, _capCol.size, _capCol.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool roofHit = Physics2D.CapsuleCast(_capCol.bounds.center, _capCol.size, _capCol.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

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
            _dashUsable = true;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Falling
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGround = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    //Dashing
    private void HandleDash()
    {
        if (!_dashToConsume) return;

        if (CanUseDash)
        {
            _dashUsable = false;
            _dashing = true;
            _dashDirection = _mousePosition - _rigidBod.position;
            _frameVelocity = new Vector2(_dashDirection.x * _stats.DashSpeed, _dashDirection.y * _stats.DashSpeed);
            _dashTrail.emitting = true;
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
