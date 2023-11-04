using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AI
{
    public enum CurveType
    {
        Linear,
        Polynomial,
        Logistic,
        Logit,
        Normal,
        Sine
    }

    public static class ResponseCurves
    {
        public static float ComputeValue(float x, CurveType curveType, float slope, float exponent, 
                                        float minVal, float maxVal, float xShift = 0, float yShift = 0)
        {
            x = Normalize(x, minVal, maxVal);

            return curveType switch
            {
                CurveType.Linear =>
                        Sanitize((slope * (x - xShift)) + yShift),
                CurveType.Polynomial =>
                    Sanitize((slope * MathF.Pow(x - xShift, exponent)) + yShift),
                CurveType.Logistic =>
                    Sanitize((slope / (1 + MathF.Exp(-10.0f * exponent * (x - 0.5f - xShift)))) + yShift),
                CurveType.Logit =>
                    Sanitize(slope * MathF.Log((x - xShift) / (1.0f - (x - xShift))) / 5.0f + 0.5f + yShift),
                CurveType.Normal =>
                    Sanitize(slope * MathF.Exp(-30.0f * exponent * (x - xShift - 0.5f) * (x - xShift - 0.5f)) + yShift),
                CurveType.Sine =>
                    Sanitize(0.5f * slope * MathF.Sin(2.0f * MathF.PI * (x - xShift)) + 0.5f + yShift),
                _ => 0.0f,
            };
        }

        static float Normalize(float x, float min, float max)
        {
            if (x < min)
                x = min;
            else if (x > max)
                x = max;

            return (x - min) / (max - min);
        }

        static float Sanitize(float y)
        {
            if (float.IsInfinity(y))
                return 0.0f;

            if (float.IsNaN(y))
                return 0.0f;

            if (y < 0.0)
                return 0.0f;

            if (y > 1.0)
                return 1.0f;

            return y;
        }
    }
}