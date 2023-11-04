using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetEffect : MonoBehaviour
{
    [SerializeField] RicochetEffectData _effect;

    public RicochetEffectData Effect => _effect;
}
