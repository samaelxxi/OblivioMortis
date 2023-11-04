using System.Collections;
using System;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine.VFX;
using System.Linq;


[SelectionBase]
public class Player : MonoBehaviour
{
    [SerializeField] PlayerStats _stats;
    [SerializeField] MeleeWeapon _meleeWeapon;
    [SerializeField] RangedWeapon _rangedWeapon;
    [SerializeField] Animator _animator;
    [SerializeField] CameraController _camera;

    [SerializeField] SkinnedMeshRenderer _meshRenderer;
    [SerializeField] Transform _leftLeg;
    [SerializeField] Transform _rightLeg;


    [SerializeField] VisualEffect _upgradeVFX;
    [SerializeField] VisualEffect _dashVFX;
    [SerializeField] VisualEffect _respawnActivationVFX;
    [SerializeField] ParticleSystem _healingVFX;


    [SerializeField, Foldout("Events")]
    GameEvent<PlayerUpgradeType> _upgradeCollected;


    public PlayerStats Stats => _stats;

    public Camera Camera => _camera.MainCamera;
    public int Health => _data.Health;
    public int MaxHealth => _data.MaxHealth;
    public int SoulFuel => _data.SoulFuel;
    public int MaxSoulFuel => _data.MaxSoulFuel;
    public int HealthItems => _data.HealthItems;
    public int MaxHealthItems => _data.MaxHealthItems;
    public int Ammo => _rangedWeapon.Ammo;
    public int MaxAmmo => _rangedWeapon.MaxAmmo;
    public bool IsAlive => _data.Health > 0;

    // not looking cool...
    public event Action<int> OnHealthChanged { add => _data.OnHealthChanged += value; remove => _data.OnHealthChanged -= value; }
    public event Action<int> OnMaxHealthChanged { add => _data.OnMaxHealthChanged += value; remove => _data.OnMaxHealthChanged -= value; }
    public event Action<int> OnSoulFuelChanged { add => _data.OnSoulFuelChanged += value; remove => _data.OnSoulFuelChanged -= value; }
    public event Action<int> OnSoulPartsChanged { add => _data.OnSoulPartsChanged += value; remove => _data.OnSoulPartsChanged -= value; }
    public event Action<int> OnMaxSoulFuelChanged { add => _data.OnMaxSoulFuelChanged += value; remove => _data.OnMaxSoulFuelChanged -= value; }
    public event Action<int> OnHealthItemsChanged { add => _data.OnHealthItemsChanged += value; remove => _data.OnHealthItemsChanged -= value; }
    public event Action<int> OnAmmoChanged { add => _rangedWeapon.OnAmmoChanged += value; remove => _rangedWeapon.OnAmmoChanged -= value; }
    public event Action<int> OnMaxAmmoChanged { add => _rangedWeapon.OnMaxAmmoChanged += value; remove => _rangedWeapon.OnMaxAmmoChanged -= value; }
    public event Action<BulletEffectType> OnBulletEffectChanged { add => _rangedWeapon.OnBulletEffectChanged += value; remove => _rangedWeapon.OnBulletEffectChanged -= value; }
    public event Action<PlayerInputType> OnInputTypeChanged { add => _inputs.OnInputTypeChanged += value; remove => _inputs.OnInputTypeChanged -= value; }
    public event Action OnDeath { add => _data.OnDeath += value; remove => _data.OnDeath -= value; }

    PlayerData _data;
    PlayerMotor _motor;
    PlayerInputs _inputs;
    readonly PlayerCharacterInputs _moveInputs = new();
    PlayerFootsteps _footsteps;


    bool _isInvulnerable = false;
    bool _isDisabled = false;
    bool _isUsingAbility = false;
    bool _isMeleeHitStaggerActive = false;
    float _abilityTime;
    float _timeSinceLastShot = 0;
    Tweener _abilityTweener;
    Coroutine _glowCoroutine;


    void Awake()
    {
        _inputs = ServiceLocator.Get<PlayerInputs>();
        _motor = GetComponent<PlayerMotor>();
        _motor.SetStats(_stats);
        _motor.OnStateChange += OnMotorStateChange;
        _data = new PlayerData();
        _data.SetupPlayer(_stats);
        _data.OnDeath += Die;
        _meleeWeapon.SetStats(_stats.MeleeWeapon);
        _meleeWeapon.OnHit += OnMeleeWeaponHit;
        _rangedWeapon.SetStats(_stats.RangedWeapon);
        _rangedWeapon.OnShoot += OnBulletShot;
        _upgradeCollected.RegisterListener(GetUpgrade);
        _footsteps = new PlayerFootsteps(transform, _leftLeg, _rightLeg);
        ServiceLocator.Get<PlayerSpawnService>().RegisterPlayer(this);
        ServiceLocator.Get<SceneTransitionManager>().OnSceneTransitionStarted += () => SetDisabled(true);
        ServiceLocator.Get<SceneTransitionManager>().OnSceneTransitionEnded += () => this.InSeconds(0.5f, () => SetDisabled(false));
        Globals.Player = this;  // ehh why even...
        GlobalEvents.OnRespawnActivated.Add(OnRespawnActivated);
        GlobalEvents.OnEnemyDamaged.Add(OnEnemyDamaged);

        #if UNITY_EDITOR
        _animator.ChangeStateDuration("MeleePrep", _stats.MeleeWeapon.AttackPreparationTime);
        _animator.ChangeStateDuration("Melee", _stats.MeleeWeapon.AttackDuration);
        _animator.ChangeStateDuration("MeleeRest", _stats.MeleeWeapon.AttackRestTime);
        #endif
        // cheat
        if (Globals.IsAbilityEnabledFromStart)
            _data.GetUpgrade(PlayerUpgradeType.Ability);
    }

    void Update()
    {
        if (_isDisabled)
        {
            ResetInputs();
            return;
        }

        ProcessMelee();
        ProcessMovement();
        ProcessShooting();
        ProcessAbility();
        ProcessHealing();
        ResetInputs();

        SetAnimatorMovementDirection();
        _footsteps.Update(Time.deltaTime);

        _timeSinceLastShot -= Time.deltaTime;
        if (_timeSinceLastShot < 0)
            _animator.SetFloat(AnimationHashes.IsBattleMode, 0, 0.2f, Time.deltaTime);
    }


    #region Inputs
    void ProcessMelee()
    {
        if (!CanMakeMeleeAttack())
            return;
        _moveInputs.MeleeDown = true;
    }

    void ProcessMovement()
    {
        if (CanChangeMovementDirection())
            SetMovementDirection();

        if (CanChangeLookDirection())
            SetLookDirection();

        if (CanDash())
        {
            if (_meleeWeapon.State != MeleeWeaponState.Idle)
                _meleeWeapon.CancelAttack();
            _moveInputs.DashDown = true;
        }
        _motor.SetInputs(_moveInputs);
    }

    void SetMovementDirection()
    {
        _moveInputs.MoveInputVector = _inputs.Movement;
        _moveInputs.CameraRotation = _camera.Rotation;
    }

    void SetLookDirection()
    {
        if (!_inputs.ShouldLook())
            return;

        Vector2 lookDir = GetCameraSpaceScreenLookDir();
        float angle = -Vector2.SignedAngle(Vector2.up, lookDir);
        if (!_inputs.MouseLookEnabled)  // if gamepad should rotate according to camera
            angle += _camera.Pitch;
        _moveInputs.RotationAngle = angle;
    }

    void ProcessShooting()
    {
        if (!CanShoot())
            return;
        _rangedWeapon.Use();
        _animator.SetTrigger(AnimationHashes.Shoot);
    }

    void ProcessAbility()
    {
        _abilityTime += Time.deltaTime;
        if (ShouldCancelAbility())
        {
            StopAbility();
            return;
        }

        if (!CanUseAbility())
            return; 

        StartAbility();
    }

    void StartAbility()
    {
        _abilityTime = 0;

        if (_abilityTweener != null && _abilityTweener.IsActive() && _abilityTweener.IsPlaying())
            _abilityTweener.Kill();
        _abilityTweener = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 
            _stats.AbilitySlowDownTimeScale, 0.2f);

        _rangedWeapon.ActivateAbility(_inputs.Ability ? 0 : 1);
        _isUsingAbility = true;
        _camera.AddZoom(_stats.CameraZoomOutOnRicochet);

        if (!Cheats.GodMode)
            _data.ChangeSoulFuel(-1);
    }

    void StopAbility()
    {
        if (_abilityTweener.IsActive() && _abilityTweener.IsPlaying())
            _abilityTweener.Kill();
        _abilityTweener = DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1, 0.3f);

        _rangedWeapon.DeactivateAbility();
        _isUsingAbility = false;
        _camera.ZoomToDefault();
    }

    void ProcessHealing()
    {
        if (CanUseHealthItem())
        {
            _data.UseHealthItem();
            _healingVFX.Play();
        }
    }

    void ResetInputs()
    {
        _inputs.ResetOneFrameInputs();
        _moveInputs.DashDown = false;
        _moveInputs.MeleeDown = false;
        _moveInputs.KnockbackForce = 0;
        _moveInputs.KnockbackDirection = Vector3.zero;
    }

    void SetAnimatorMovementDirection()
    {
        if (IsMoving() || IsDashing())  // move and dash blend trees depend on these
        {
            var dirWS = Quaternion.Euler(0, _camera.Pitch, 0) * _moveInputs.MoveInputVector;
            var forward = Vector3.Dot(dirWS, transform.forward);  // project onto unit vectors to get needed component
            var right = Vector3.Dot(dirWS, transform.right);
            _animator.SetFloat(AnimationHashes.Forward, forward, _stats.AnimationDamp, Time.deltaTime);
            _animator.SetFloat(AnimationHashes.Right, right, _stats.AnimationDamp, Time.deltaTime);
        }
        else
        {
            _animator.SetFloat(AnimationHashes.Forward, 0);
            _animator.SetFloat(AnimationHashes.Right, 0);
        }
    }

    #endregion


    public void TakeDamage(int amount, bool forced = false)
    {
        _camera.NoiseShake(NoiseShakeType.DamageTaken);

        if (Cheats.GodMode) return;

        if (!forced && _isInvulnerable)
            return;
        _camera.NoiseShake(NoiseShakeType.DamageTaken);
        JSAM.AudioManager.PlaySound(OblivioSounds.PlayerDamaged);
        // ServiceLocator.Get<VFXManager>().PlayParticle(ParticleType.Blood, transform.position, Quaternion.LookRotation(Vector3.up));
        StartCoroutine(Invulnerability());
        _data.ChangeHealth(-amount);
    }

    IEnumerator Invulnerability()
    {
        _isInvulnerable = true;

        DOGlow(Color.white, _stats.InvulnerabilityTime, 0.2f);
        yield return new WaitForSeconds(_stats.InvulnerabilityTime);
        
        _isInvulnerable = false;
    }

    public void TakeDamageWithKnockback(int amount, Vector3 direction, float knockback)
    {
        if (Cheats.GodMode) return;

        TakeDamage(amount);
        ServiceLocator.Get<VFXManager>().PlayParticle(ParticleType.Blood, transform.position, Quaternion.LookRotation(direction));
        if (_data.Health <= 0 || _isInvulnerable)
            return;
        _moveInputs.KnockbackForce = knockback;  // process in motor later
        _moveInputs.KnockbackDirection = direction.SetY(0);
    }


    void Die()
    {
        SetDisabled(true);
        _animator.ResetTrigger(AnimationHashes.Respawn);
        _animator.SetTrigger(AnimationHashes.Die);
    }

    public void Respawn()
    {
        _data.SetupPlayer(_stats);
        _rangedWeapon.SetStats(_stats.RangedWeapon);
        _animator.SetTrigger(AnimationHashes.Respawn);
    }

    void OnEnemyDamaged(Enemy enemy, int damage)
    {
        if (Vector3.Distance(transform.position, enemy.transform.position) <= _stats.GrabSoulRadius)
            _data.DamageToSoul(damage);
    }

    void OnRespawnActivated()
    {
        _data.OnRespawnActivated();
        _respawnActivationVFX.Play();
        DOGlow(Color.yellow, 5, 2);
    }

    void OnBulletShot()
    {
        JSAM.AudioManager.PlaySound(OblivioSounds.PlayerShot);
        _camera.ImpulseShake(ImpulseShakeType.Shoot, -GetCameraSpaceScreenLookDir());
        _timeSinceLastShot = 3;
        _animator.SetFloat(AnimationHashes.IsBattleMode, 1);
    }

    void OnMeleeWeaponHit(Collider other)
    {
        if (other.TryGetComponent(out MeleeBulletRefill bulletRefill))
            _rangedWeapon.RefillAmmo(bulletRefill.Effect);
        if (other.TryGetComponent<Enemy>(out var enemy))
            enemy.GetMeleeHit(_stats.MeleeWeapon, (enemy.transform.position - transform.position).normalized);
        if (other.TryGetComponent<Lever>(out var lever))
            lever.OnInteraction();

        StartCoroutine(MeleeHitTimeStagger(_stats.HitStaggerTime));
    }

    IEnumerator MeleeHitTimeStagger(float seconds)
    {
        if (_isMeleeHitStaggerActive)
            yield break;
        _isMeleeHitStaggerActive = true;
        float timeScale = Time.timeScale;
        Time.timeScale = 0.001f;
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = timeScale;
        _isMeleeHitStaggerActive = false;
    }

    void OnMeleeAttackStart()
    {
        if (!Globals.NotShittyConrols && _moveInputs.MoveInputVector.sqrMagnitude > 0.1f)
        {
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(_moveInputs.CameraRotation * Vector3.forward, Vector3.up).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
                cameraPlanarDirection = Vector3.ProjectOnPlane(_moveInputs.CameraRotation * Vector3.up, Vector3.up).normalized;
            var rot = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);
            Vector3 lookDir = rot * _moveInputs.MoveInputVector;
            _motor.SetRotation(Quaternion.LookRotation(lookDir));
            float angle = -Vector2.SignedAngle(Vector2.up, lookDir);
            if (!_inputs.MouseLookEnabled)  // if gamepad should rotate according to camera
                angle += _camera.Pitch;
            _moveInputs.RotationAngle = angle;
        }


        _camera.TempZoomIn(_stats.CameraZoomInOnMeleeDistance, _stats.MeleeWeapon.AttackTotalTime);
        _camera.ImpulseShake(ImpulseShakeType.MeleeAttack, GetCameraSpaceScreenLookDir());
        _animator.SetTrigger(AnimationHashes.MeleeAttack);
        JSAM.AudioManager.PlaySound(OblivioSounds.PlayerMelee);
        _meleeWeapon.StartAttack();
    }

    void OnDashStart()
    {
        _animator.SetTrigger(AnimationHashes.Dash);
        JSAM.AudioManager.PlaySound(OblivioSounds.PlayerDash);
        DOGlow(GameColors.GoldColor, 0.4f, 0.3f, 0.15f, 2, true);
        _dashVFX.Play();
    }

    void OnMotorStateChange(CharacterMotorState oldState, CharacterMotorState newState)
    {
        if (newState == CharacterMotorState.Dashing)
        {
            OnDashStart();
        }
        else if (newState == CharacterMotorState.Lunging)
        {
            OnMeleeAttackStart();
        }
    }



    public Vector3 GetVelocity()
    {
        return _motor.Velocity;
    }

    public void TeleportTo(Vector3 position)
    {
        _motor.TeleportTo(position);
    }

    void GetUpgrade(PlayerUpgradeType type)
    {
        switch (type)
        {
            case PlayerUpgradeType.Souls:
            case PlayerUpgradeType.Health:
            case PlayerUpgradeType.Ability:
                _data.GetUpgrade(type);
                break;
            case PlayerUpgradeType.Ammo:
                _rangedWeapon.GetUpgrade(type);
                break;
        }
        Debug.Log($"Got upgrade {type}");

        JSAM.AudioManager.PlaySound(OblivioSounds.PlayerUpgrade);
        StartCoroutine(UpgradeVFX());
    }

    IEnumerator UpgradeVFX()
    {
        _upgradeVFX.Play();
        DOGlow(Color.yellow, 3, 0.2f);
        yield return new WaitForSecondsRealtime(3);
        _upgradeVFX.Stop();
    }

    public void SetRotation(Quaternion rotation)
    {
        _motor.SetRotation(rotation);
    }

    void DOGlow(Color color, float duration, float changeTime, float endTime = -1, float power = 1, bool forced = false)
    {
        bool isGlowing = _meshRenderer.materials[0].IsKeywordEnabled("_FRESNELENABLED");
        if (isGlowing && !forced)
            return;
        else if (isGlowing)
            StopCoroutine(_glowCoroutine);

        _glowCoroutine = StartCoroutine(DOGlowCoroutine(color, duration, changeTime, endTime, power));
    }

    IEnumerator DOGlowCoroutine(Color color, float duration, float changeTime, 
                                float endTime = -1, float power = 1)
    {
        float time = 0;
        bool emissionOn = true;
        Array.ForEach(_meshRenderer.materials, mat => mat.SetColor("_FresnelColor", color));
        Array.ForEach(_meshRenderer.materials, mat => mat.EnableKeyword("_FRESNELENABLED"));
        while (time < duration)
        {
            if (emissionOn)
                Array.ForEach(_meshRenderer.materials, mat => mat.DOFloat(power, "_FresnelPower", changeTime));
            else
                Array.ForEach(_meshRenderer.materials, mat => mat.DOFloat(0, "_FresnelPower", changeTime));
            yield return new WaitForSeconds(changeTime);
            emissionOn = !emissionOn;
            time += changeTime;
        }
        Array.ForEach(_meshRenderer.materials, mat => mat.DOFloat(0, "_FresnelPower", changeTime));
        yield return new WaitForSeconds(endTime < 0 ? changeTime : endTime);
        Array.ForEach(_meshRenderer.materials, mat => mat.DisableKeyword("_FRESNELENABLED"));
    }

    public void SetDisabled(bool isDisabled)
    {
        _isDisabled = isDisabled;
        if (_isDisabled)
        {
            ResetInputs();
            _animator.SetFloat(AnimationHashes.Forward, 0);
            _animator.SetFloat(AnimationHashes.Right, 0);
            _moveInputs.MoveInputVector = Vector2.zero;
            _motor.SetInputs(_moveInputs);
        }
    }

    Vector2 GetCameraSpaceScreenLookDir()
    {
        Vector2 lookDir;
        if (_inputs.MouseLookEnabled)
        {
            var ray = _camera.MainCamera.ScreenPointToRay(_inputs.MouseLook);
            var shootPlane = new Plane(transform.up, _rangedWeapon.ShootPoint);
            shootPlane.Raycast(ray, out var distance);
            var pointOnPlane = ray.GetPoint(distance);
            Vector3 preLookDir = pointOnPlane - transform.position;
            lookDir = new Vector2(preLookDir.x, preLookDir.z);
        }
        else
            lookDir = _inputs.GamepadLook;
        return lookDir;
    }




    #region Predicates
    bool CanChangeMovementDirection() => _motor.CurrentMotorState == CharacterMotorState.Moving 
                                        || IsMeleeAttacking();

    bool IsMoving() => _motor.CurrentMotorState == CharacterMotorState.Moving;

    bool IsDashing() => _motor.CurrentMotorState == CharacterMotorState.Dashing;

    bool IsMeleeAttacking() => _motor.CurrentMotorState == CharacterMotorState.Lunging;

    bool CanChangeLookDirection() => !IsMeleeAttacking();

    bool CanDash() => _inputs.Movement.sqrMagnitude > 0.01f && _inputs.Dash &&
                _motor.IsDashReady() && !_isUsingAbility &&
                (IsMoving() || (IsMeleeAttacking() && _meleeWeapon.CanCancelAttack()));

    bool CanShoot() =>  _inputs.Shoot && _rangedWeapon.ReadyToShoot() && !IsMeleeAttacking();

    bool CanUseAbility() =>_data.HasAbility && (_inputs.Ability || _inputs.Ability2)
                        && IsMoving() && !_isUsingAbility;
                // && _data.SoulFuel > 0;  // to play with

    bool ShouldCancelAbility() =>_isUsingAbility && ((_inputs.Ability || _inputs.Ability2) || 
                                    _abilityTime > _stats.AbilityDurationScaled);

    bool CanUseHealthItem() => _inputs.UseHealthItem && _data.HealthItems > 0 && _data.Health < _data.MaxHealth;

    bool CanMakeMeleeAttack() =>_inputs.Melee && _meleeWeapon.CanAttack() && IsMoving() && !_isUsingAbility;

    #endregion
}
