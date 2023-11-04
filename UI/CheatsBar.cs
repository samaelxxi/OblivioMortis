using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public static class Cheats
{
    public static bool GodMode { get; set; } = false;
    public static bool InfiniteAmmo { get; set; } = false;
}


public class CheatsBar : MonoBehaviour
{
    public void ToggleGodMode()
    {
        Cheats.GodMode = !Cheats.GodMode;
    }

    public void ToggleInfiniteAmmo()
    {
        Cheats.InfiniteAmmo = !Cheats.InfiniteAmmo;
    }

    public void Teleport()
    {
        StartCoroutine(TeleportCoroutine());
    }

    IEnumerator TeleportCoroutine()
    {
        var camera = FindFirstObjectByType<CameraController>();
        var virtualCamera = camera.VirtualCamera;
        var c = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        float oldDist = c.m_CameraDistance;
        float oldFar = virtualCamera.m_Lens.FarClipPlane;
        virtualCamera.m_Lens.FarClipPlane = 900;
        c.m_CameraDistance = 700;

        var inputs = ServiceLocator.Get<PlayerInputs>();
        bool clicked = false;
        inputs.OnMeleeClicked += () => clicked = true;
        while (clicked == false)
        {
            yield return null;
        }
        var mouse = inputs.MouseLook;
        var ray = Camera.main.ScreenPointToRay(mouse);
        if (Physics.Raycast(ray, out var hit, 1000, LayerMask.GetMask("Ground", "Default", "Obstacle")))
        {
            Globals.Player.TeleportTo(hit.point + Vector3.up * 3);
        }
        c.m_CameraDistance = oldDist;
        virtualCamera.m_Lens.FarClipPlane = oldFar;
        inputs.OnMeleeClicked -= () => clicked = true;
    }
}
