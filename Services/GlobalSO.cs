using UnityEngine;


// [CreateAssetMenu(fileName = "GlobalSO", menuName = "ScriptableObjects/GlobalSO", order = 1)]
public class GlobalSO : Services.SORegistrable
{
    [field: SerializeField]
    public AnimationCurve KnockbackCurve { get; private set; } = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [field: SerializeField]
    public float KnockbackTime { get; private set; } = 0.2f;


    [field: SerializeField]
    public DialogSettings DialogSettings { get; private set; }

    [field: SerializeField]
    public Material DamageFlashMaterial { get; private set; }

}
