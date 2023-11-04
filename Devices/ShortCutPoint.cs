using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCutPoint : InteractZone
{
    public event Action<Player> OnShortCutRequested;

    public override void OnInteraction()
    {
        OnShortCutRequested?.Invoke(_player);
    }
}
