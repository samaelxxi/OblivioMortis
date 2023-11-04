using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerInputType { KeyboardMouse, Gamepad }

[Attributes.AutoRegisteredService]
public class PlayerInputs : IRegistrable
{
    readonly PlayerControls _controls;

    public Vector3 Movement { get; private set; }

    public Vector2 MouseLook { get; private set; }
    public Vector2 GamepadLook { get; private set; }

    public bool Dash { get; private set; } = false;
    public bool Melee { get; private set; } = false;
    public bool Shoot { get; private set; } = false;
    public bool Ability { get; private set; } = false;
    public bool Ability2 { get; private set; } = false;
    public bool UseHealthItem { get; private set; } = false;

    public bool Interact { get; private set; } = false;
    public event Action OnInteractClicked;

    public event Action OnMeleeClicked;
    public event Action OnCheatsClicked;

    public event Action OnMenuClicked;


    public Vector3 WorldMouseAim = Vector3.zero;
    public bool MouseLookEnabled => _currentInputType == PlayerInputType.KeyboardMouse;

    PlayerInputType _currentInputType = PlayerInputType.KeyboardMouse;
    public event Action<PlayerInputType> OnInputTypeChanged;


    public void OnRegister() {}

    public PlayerInputs()
    {
        _controls = new PlayerControls();
        _controls.Enable();
        _controls.Gameplay.Movement.performed += SetMovement;
        _controls.Gameplay.Movement.canceled += ctx => Movement = Vector2.zero;
        _controls.Gameplay.MousePos.performed += MouseMove;
        _controls.Gameplay.GamepadTargeting.started += GamepadTargetStart;
        _controls.Gameplay.GamepadTargeting.performed += ctx => GamepadLook = ctx.ReadValue<Vector2>();
        _controls.Gameplay.GamepadTargeting.canceled += ctx => GamepadLook = Vector2.zero;
        _controls.Gameplay.Dash.started += ctx => Dash = true;
        _controls.Gameplay.MeleeAttack.started += OnMelee;
        _controls.Gameplay.Shoot.started += OnShoot;
        _controls.Gameplay.UseAbility.started += ctx => Ability = true;
        _controls.Gameplay.UseAbility2.started += ctx => Ability2 = true;
        _controls.Gameplay.UseHealthItem.started += ctx => UseHealthItem = true;
        _controls.Gameplay.Interact.started += OnInteract;
        _controls.Gameplay.EnableCheats.started += ctx => OnCheatsClicked?.Invoke();
        _controls.Gameplay.MenuClick.started += ctx => OnMenuClicked?.Invoke();
    }

    public void ResetOneFrameInputs()
    {
        Dash = false;
        Melee = false;
        Shoot = false;
        Ability = false;
        Ability2 = false;
        Interact = false;
        UseHealthItem = false;
    }

    void OnMelee(InputAction.CallbackContext ctx)
    {
        if (!IsMouseInScreen()) return;

        Melee = true;
        OnMeleeClicked?.Invoke();
    }

    void OnShoot(InputAction.CallbackContext ctx)
    {
        if (!IsMouseInScreen()) return;

        Shoot = true;
    }

    bool IsMouseInScreen()
    {
        var mousePos = Mouse.current.position.ReadValue();
        return mousePos.x > 0 && mousePos.x < Screen.width &&
                mousePos.y > 0 && mousePos.y < Screen.height;
    }

    void SetMovement(InputAction.CallbackContext ctx)
    {
        var inputs = ctx.ReadValue<Vector2>();
        Movement = Vector3.ClampMagnitude(new Vector3(inputs.x, 0f, inputs.y), 1f);
    }

    public bool ShouldLook()
    {
        return MouseLookEnabled || 
                (!MouseLookEnabled && GamepadLook != Vector2.zero);
    }

    void ChangeInputType(PlayerInputType type)
    {
        if (_currentInputType == type)
            return;
        _currentInputType = type;
        OnInputTypeChanged?.Invoke(type);
    }

    void GamepadTargetStart(InputAction.CallbackContext ctx)
    {
        ChangeInputType(PlayerInputType.Gamepad);
    }

    void MouseMove(InputAction.CallbackContext ctx)
    {
        if (!IsMouseInScreen()) return;

        var newVal = ctx.ReadValue<Vector2>();
        // MovedMouseThisFrame = true;
        ChangeInputType(PlayerInputType.KeyboardMouse);
        MouseLook = newVal;
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ServiceLocator.Get<UIView>().IsPaused)
            return;
        Interact = true;
        OnInteractClicked?.Invoke();
    }
}
