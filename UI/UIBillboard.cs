using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class UIBillboard : MonoBehaviour
{
    Camera mainCamera;

    Player _player;


    public void SetPlayer(Player player)
    {
        _player = player;
        mainCamera = player.Camera;
        Enable(false);
    }

    public void Enable(bool enabled = true)
    {
        gameObject.SetActive(enabled);
    }

    void LateUpdate()
    {
        if (_player == null)
            return;
        var pos = _player.transform.position + Vector3.up * 3;
        transform.SetPositionAndRotation(pos, mainCamera.transform.rotation);
    }
}
