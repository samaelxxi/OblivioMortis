using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using System;
using DG.Tweening;


public enum CharacterMotorState
{
    None, Moving, Dashing, Lunging, Knockback
}

public class PlayerCharacterInputs
{
    public Vector3 MoveInputVector;
    public Quaternion CameraRotation;
    public float RotationAngle;
    public bool DashDown;
    public bool MeleeDown;

    public float KnockbackForce;
    public Vector3 KnockbackDirection;
}

public class PlayerMotor : MonoBehaviour, ICharacterController
{
    public CharacterMotorState CurrentMotorState { get; private set; } = CharacterMotorState.None;
    public event Action<CharacterMotorState, CharacterMotorState> OnStateChange;

    PlayerMovementStats _stats;
    MeleeWeaponStats _meleeStats;  // TODO hmmm why motor has access to melee...
    KinematicCharacterMotor Motor;
    public Vector3 Velocity => Motor.Velocity;

    Vector3 _moveInputVector;
    float _rotationAngle;

    bool _meleeRequested = false;
    bool _meleeConsumed = false;
    bool _dashRequested = false;
    bool _dashConsumed = false;

    float _dashTime = 0f;
    float _timeSinceLastDashEnded = 0;

    float _meleeTime = 0f;

    bool _knockbackRequested = false;
    float _knockbackTime = 0f;
    float _knockbackForce = 0f;
    Vector3 _knockbackDirection = Vector3.zero;

    Vector3 _internalVelocityAdd = Vector3.zero;



    void Awake()
    {
        Motor = GetComponent<KinematicCharacterMotor>();
        TransitionToState(CharacterMotorState.Moving);
        Motor.CharacterController = this;
    }

    public bool IsDashReady()
    {
        return _timeSinceLastDashEnded >= _stats.DashCooldown;
    }

    public void TeleportTo(Vector3 position)
    {
        Motor.SetPosition(position);
    }

    public void SetRotation(Quaternion rotation)
    {
        Motor.SetRotation(rotation);
    }

    public void SetStats(PlayerStats stats)
    {
        _stats = stats.Movement;
        _meleeStats = stats.MeleeWeapon;  // need it for lunge
    }

    /// <summary>
    /// Handles movement state transitions and enter/exit callbacks
    /// </summary>
    public void TransitionToState(CharacterMotorState newState)
    {
        if (newState == CurrentMotorState)
        {
            Debug.LogError($"Trying to transition to same state {newState}!");
            return;
        }
        // Debug.Log($"Transitioning from {CurrentMotorState} to {newState}");
        CharacterMotorState tmpInitialState = CurrentMotorState;
        OnStateExit(tmpInitialState, newState);
        CurrentMotorState = newState;
        OnStateEnter(newState, tmpInitialState);
        OnStateChange?.Invoke(tmpInitialState, newState);
    }

    /// <summary>
    /// Event when entering a state
    /// </summary>
    public void OnStateEnter(CharacterMotorState state, CharacterMotorState fromState)
    {
        // Debug.Log($"OnStateEnter: {state}");
        switch (state)
        {
            case CharacterMotorState.Moving:
                {
                    break;
                }
            case CharacterMotorState.Dashing:
                {
                    _dashRequested = false;
                    _dashConsumed = true;
                    _dashTime = 0f;
                    break;
                }
            case CharacterMotorState.Lunging:
                {
                    _meleeRequested = false;
                    _meleeConsumed = true;
                    _meleeTime = 0f;
                    break;
                }
            case CharacterMotorState.Knockback:
                {
                    _knockbackTime = 0f;
                    break;
                }
        }
    }

    /// <summary>
    /// Event when exiting a state
    /// </summary>
    public void OnStateExit(CharacterMotorState state, CharacterMotorState toState)
    {
        // Debug.Log($"OnStateExit: {state}");
        switch (state)
        {
            case CharacterMotorState.Moving:
                {
                    break;
                }
            case CharacterMotorState.Dashing:
                {
                    _timeSinceLastDashEnded = 0f;
                    _dashConsumed = false;  // allow to consume again
                    break;
                }
            case CharacterMotorState.Lunging:
                {
                    _meleeConsumed = false;
                    break;
                }
            case CharacterMotorState.Knockback:
                {
                    _knockbackDirection = Vector3.zero;
                    _knockbackForce = 0f;
                    break;
                }
        }
    }

    /// <summary>
    /// This is called every frame by ExamplePlayer in order to tell the character what its inputs are
    /// </summary>
    public void SetInputs(PlayerCharacterInputs inputs)
    {
        // Clamp input
        Vector3 moveInputVector = inputs.MoveInputVector;

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

        if (inputs.KnockbackDirection != Vector3.zero)
        {
            _knockbackRequested = true;
            _knockbackForce = inputs.KnockbackForce;
            _knockbackDirection = inputs.KnockbackDirection;
        }

        switch (CurrentMotorState)
        {
            case CharacterMotorState.Moving:
                {
                    _moveInputVector = cameraPlanarRotation * moveInputVector;
                    _rotationAngle = inputs.RotationAngle;
                    if (inputs.DashDown)
                        _dashRequested = true;
                    else if (inputs.MeleeDown)
                        _meleeRequested = true;
                    break;
                }
            case CharacterMotorState.Dashing:
                {
                    break;
                }
            case CharacterMotorState.Lunging:
                {
                    if (inputs.DashDown)  // can dash if cancel attack allowed
                    {
                        _moveInputVector = cameraPlanarRotation * moveInputVector;
                        _rotationAngle = inputs.RotationAngle;
                        _dashRequested = true;
                    }
                    break;
                }
            case CharacterMotorState.Knockback:
                {
                    _moveInputVector = Vector3.zero;
                    break;
                }
        }
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called before the character begins its movement update
    /// </summary>
    public void BeforeCharacterUpdate(float deltaTime)
    {
        // if (_knockbackDirection != Vector3.zero && CurrentMotorState != CharacterMotorState.Knockback)
        if (_knockbackRequested && CurrentMotorState != CharacterMotorState.Knockback)
        {
            TransitionToState(CharacterMotorState.Knockback);
            _knockbackRequested = false;
        }
        switch (CurrentMotorState)
        {
            case CharacterMotorState.Moving:
                {
                    if (_dashRequested && !_dashConsumed)
                        TransitionToState(CharacterMotorState.Dashing);

                    else if (_meleeRequested && !_meleeConsumed)
                        TransitionToState(CharacterMotorState.Lunging);
                    break;
                }
            case CharacterMotorState.Lunging:
                {
                    if (_dashRequested && !_dashConsumed)
                        TransitionToState(CharacterMotorState.Dashing);
                    break;
                }
        }
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its rotation should be right now. 
    /// This is the ONLY place where you should set the character's rotation
    /// </summary>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (CurrentMotorState)
        {
            case CharacterMotorState.Moving:
            case CharacterMotorState.Dashing:  // it's a thing atm
                {
                    currentRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, _rotationAngle, 0), 
                                            deltaTime * _stats.RotationSpeed);
                    break;
                }
            // other states don't rotate
        }
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is where you tell your character what its velocity should be right now. 
    /// This is the ONLY place where you can set the character's velocity
    /// </summary>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        switch (CurrentMotorState)
        {
            case CharacterMotorState.Moving:
                {
                    // Ground movement
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        float currentVelocityMagnitude = currentVelocity.magnitude;
                        Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                        // Reorient velocity on slope
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        Vector3 targetMovementVelocity = reorientedInput * _stats.MaxMoveSpeed;

                        // Smooth movement Velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-_stats.Acceleration * deltaTime));
                    }
                    // Air movement
                    else
                    {
                        // Add move input
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            Vector3 addedVelocity = _stats.AirAccelerationSpeed * deltaTime * _moveInputVector;

                            Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                            // Limit air velocity from inputs
                            if (currentVelocityOnInputsPlane.magnitude < _stats.MaxAirMoveSpeed)
                            {
                                // clamp addedVel to make total vel not exceed max vel on inputs plane
                                Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, _stats.MaxAirMoveSpeed);
                                addedVelocity = newTotal - currentVelocityOnInputsPlane;
                            }
                            else
                            {
                                // Make sure added vel doesn't go in the direction of the already-exceeding velocity
                                if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                {
                                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                }
                            }

                            // Prevent air-climbing sloped walls
                            if (Motor.GroundingStatus.FoundAnyGround)
                            {
                                if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                                {
                                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                    addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                }
                            }

                            // Apply added velocity
                            currentVelocity += addedVelocity;
                        }
                        AddGravity(ref currentVelocity, deltaTime);
                    }

                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }
                    break;
                }
            case CharacterMotorState.Dashing:
                {
                    if (_dashTime > _stats.DashPrepareTime && _dashTime < _stats.DashTime + _stats.DashPrepareTime)
                    {
                        float startTime = _dashTime - _stats.DashPrepareTime;
                        float speed = MathUtils.TimeToVelocityOnCurve(_stats.DashCurve, startTime, deltaTime, 
                            _stats.DashTime, _stats.DashDistance);
                        currentVelocity = _moveInputVector * speed;
                    }
                    else
                    {
                        currentVelocity = Vector3.zero;
                    }
                    break;
                }
            case CharacterMotorState.Lunging:
                {
                    if (_meleeTime < _meleeStats.LungeTime)
                    {
                        float speed = MathUtils.TimeToVelocityOnCurve(_meleeStats.LungeCurve, _meleeTime, deltaTime,
                            _meleeStats.LungeTime, _meleeStats.LungeDistance);
                        currentVelocity = transform.forward * speed;
                    }
                    else
                    {
                        currentVelocity = Vector3.zero;
                    }

                    AddGravity(ref currentVelocity, deltaTime);  // it won't work so easy... need to accumulate gravity(at least)
                    break;
                }
            case CharacterMotorState.Knockback:
                {
                    if (_knockbackTime < Globals.KnockbackTime)
                    {
                        float speed = MathUtils.TimeToVelocityOnCurve(Globals.KnockbackCurve, _knockbackTime, deltaTime,
                            Globals.KnockbackTime, _knockbackForce);
                        currentVelocity = _knockbackDirection * speed;
                    }
                    else
                    {
                        currentVelocity = Vector3.zero;
                    }
                    break;
                }
        }
    }

    void AddGravity(ref Vector3 currentVelocity, float deltaTime)
    {
        // TODO gravity should be applied in all states and should be cumulative
        currentVelocity += _stats.Gravity * deltaTime;
        currentVelocity *= (1f / (1f + (_stats.Drag * deltaTime)));
    }

    /// <summary>
    /// (Called by KinematicCharacterMotor during its update cycle)
    /// This is called after the character has finished its movement update
    /// </summary>
    public void AfterCharacterUpdate(float deltaTime)
    {
        switch (CurrentMotorState)
        {
            case CharacterMotorState.Moving:
                {
                    {

                    }
                    break;
                }
            case CharacterMotorState.Dashing:
                {
                    _dashTime += deltaTime;
                    if (_dashTime > _stats.TotalDashTime)
                        TransitionToState(CharacterMotorState.Moving);
                    break;
                }
            case CharacterMotorState.Lunging:
                {
                    _meleeTime += deltaTime;
                    if (_meleeTime > _meleeStats.AttackTotalTime)
                        TransitionToState(CharacterMotorState.Moving);
                    break;
                }
            case CharacterMotorState.Knockback:
                {
                    _knockbackTime += deltaTime;
                    if (_knockbackTime > Globals.KnockbackTime)
                        TransitionToState(CharacterMotorState.Moving);
                    break;
                }
        }
        if (_dashRequested)  // skip if couldn't perform during this frame
            _dashRequested = false;
        if (_meleeRequested)
            _meleeRequested = false;
        _timeSinceLastDashEnded += deltaTime;
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // Handle landing and leaving ground
        if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLeaveStableGround();
        }
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (_stats.IgnoredColliders.Count == 0)
        {
            return true;
        }

        if (_stats.IgnoredColliders.Contains(coll))
        {
            return false;
        }

        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void AddVelocity(Vector3 velocity)
    {
        switch (CurrentMotorState)
        {
            case CharacterMotorState.Moving:
                {
                    _internalVelocityAdd += velocity;
                    break;
                }
        }
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    protected void OnLanded()
    {
    }

    protected void OnLeaveStableGround()
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }
}
