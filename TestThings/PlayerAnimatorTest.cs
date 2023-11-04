using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorTest : MonoBehaviour
{
    [SerializeField] PlayerStats _stats;

    Animator _animator;

    [SerializeField] MeshRenderer _floor;

    float _dashLength;

    void Start()
    {
        _animator = FindAnyObjectByType<Animator>();

        foreach (var clip in _animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.Contains("Dash"))
            {
                _dashLength = clip.length;
                break;
            }
        }
    }

    Vector2 MoveInput;

    Vector2 _floorOffset;


    float _dashingFor = 0f;
    bool _isDashing = false;

    void Update()
    {
        _dashingFor += Time.deltaTime;
        if (_dashingFor >= _dashLength)
            _isDashing = false;


        MoveInput.x = Input.GetAxisRaw("Horizontal");
        MoveInput.y = Input.GetAxisRaw("Vertical");
        MoveInput.Normalize();
        bool _isDash = Input.GetKeyDown(KeyCode.Space);
        bool _isAttack = Input.GetKeyDown(KeyCode.Mouse0);
        bool _isShoot = Input.GetKeyDown(KeyCode.Mouse1);

        _animator.SetFloat("Right", MoveInput.x, _stats.AnimationDamp, Time.deltaTime);
        _animator.SetFloat("Forward", MoveInput.y, _stats.AnimationDamp, Time.deltaTime);

        if (_isDash)
        {
            _animator.SetTrigger("Dash");
            _dashingFor = 0;
            _isDashing = true;
        }
        if (_isAttack)
        {
            _animator.SetTrigger("MeleeAttack");
        }
        if (_isShoot)
        {
            _animator.SetTrigger("Shoot");
        }

        if (_isDashing)
            MoveInput *= 2;
        _floorOffset.x += MoveInput.x * Time.deltaTime;
        _floorOffset.y += MoveInput.y * Time.deltaTime;

        var mat = _floor.material;
        mat.SetTextureOffset("_BaseMap", _floorOffset);
        _floor.material = mat;
        // _floor.material.SetTextureOffset("_MainTex", _floorOffset);

    }
}
