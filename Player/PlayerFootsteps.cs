using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// It's terrible but well, it works


public class PlayerFootsteps
{
    Transform _playerTransform;
    Transform _leftFoot;
    Transform _rightFoot;

    float _leftFootTimer = 0f;
    float _rightFootTimer = 0f;

    float _prevLeftLeg = 0f;
    float _prevRightLeg = 0f;

    float _prevDl = 0f;
    float _prevDr = 0f;


    PlayerInputs _playerInputs;

    public PlayerFootsteps(Transform playerTransform, Transform leftFoot, Transform rightFoot)
    {
        _playerTransform = playerTransform;
        _leftFoot = leftFoot;
        _rightFoot = rightFoot;
        _playerInputs = ServiceLocator.Get<PlayerInputs>();
    }

    public void Update(float timeDelta)
    {
        if (_playerInputs.Movement.magnitude < 0.1f)
            return;

        _leftFootTimer += timeDelta;
        _rightFootTimer += timeDelta;

        float leftLeg = _playerTransform.InverseTransformPoint(_leftFoot.position).y + 1;  // +1 to make ground ~= 0
        float rightLeg = _playerTransform.InverseTransformPoint(_rightFoot.position).y + 1;

        float dl = leftLeg - _prevLeftLeg;
        float dr = rightLeg - _prevRightLeg;

        if (dl > 0 && _prevDl < 0 && leftLeg < 0.15f)
            PlaySoundIfNeeded(true);

        else if (dr > 0 && _prevDr < 0 && rightLeg < 0.15f)
            PlaySoundIfNeeded(false);

        _prevDl = dl;
        _prevDr = dr;

        _prevLeftLeg = leftLeg;
        _prevRightLeg = rightLeg;
    }

    void PlaySoundIfNeeded(bool _isLeftLeg)
    {
        if (_isLeftLeg && _leftFootTimer > 0.5f && _rightFootTimer > 0.3f)
        {
            JSAM.AudioManager.PlaySound(OblivioSounds.PlayerFootstep);
            _leftFootTimer = 0f;
        }
        else if (!_isLeftLeg && _rightFootTimer > 0.5f && _leftFootTimer > 0.3f)
        {
            JSAM.AudioManager.PlaySound(OblivioSounds.PlayerFootstep);
            _rightFootTimer = 0f;
        }
    }
}
