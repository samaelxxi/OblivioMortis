using System.Collections;
using UnityEngine;

public static class MathUtils
{
    public static float TimeToVelocityOnCurve(AnimationCurve curve, float curTime, float deltaTime, float totalTime, float distance)
    {
        float curTimeNorm = curTime / totalTime;
        float nextTimeNorm = (curTime + Time.deltaTime) / totalTime;
        curTimeNorm = curve.Evaluate(curTimeNorm);
        nextTimeNorm = Mathf.Clamp01(curve.Evaluate(nextTimeNorm));
        float curPath = curTimeNorm * distance;
        float nextPath = nextTimeNorm * distance;
        float v = (nextPath - curPath) / deltaTime;

        return v;
    }
}