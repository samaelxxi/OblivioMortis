using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EditorUserSettings : Services.SORegistrable, Services.IInitializable
{
    [field: SerializeField]
    public bool IsGodMode { get; private set; } = false;

    [field: SerializeField]
    public bool NotShittyConrols { get; private set; } = false;

    [field: SerializeField]
    public bool IsAbilityEnabledFromStart { get; private set; } = false;


    public void Initialize()
    {
        Globals.EditorUserSettings = this;
        if (IsGodMode)
        {
            Cheats.GodMode = true;
            Cheats.InfiniteAmmo = true;
        }
    }
}

