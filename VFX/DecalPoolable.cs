using System.Collections;
using System.Collections.Generic;
using DesignPatterns;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class DecalPoolable : Poolable<DecalPoolable>
{
    public DecalProjector DecalProjector;
}
