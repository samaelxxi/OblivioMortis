using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCut : MonoBehaviour, ISaveable
{
    [SerializeField] ShortCutPoint _pointA;
    [SerializeField] ShortCutPoint _pointB;

    [SerializeField] bool _isActivated = false;


    void Start()
    {
        _pointA.OnShortCutRequested += TeleportToB;
        _pointB.OnShortCutRequested += TeleportToA;

        if (!_isActivated)
            Deactivate();
        else
            Activate();
    }

    public void Activate()
    {
        _isActivated = true;
        _pointA.SetActive(true);
        _pointB.SetActive(true);
    }

    public void Deactivate()
    {
        _isActivated = false;
        _pointA.SetActive(false);
        _pointB.SetActive(false);
    }

    void TeleportToB(Player player)
    {
        if (!_isActivated)
            return;

        player.TeleportTo(_pointB.transform.position);
        JSAM.AudioManager.PlaySound(OblivioSounds.ShortcutActivated);
    }

    void TeleportToA(Player player)
    {
        if (!_isActivated)
            return;

        player.TeleportTo(_pointA.transform.position);
        JSAM.AudioManager.PlaySound(OblivioSounds.ShortcutActivated);
    }

    public string Save()
    {
        return _isActivated.ToString();
    }

    public void Load(string data)
    {
        _isActivated = bool.Parse(data);
        if (_isActivated)
            Activate();
        else
            Deactivate();
    }
}
