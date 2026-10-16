using UnityEngine;

public static class Easing
{
    public static float OutElastic(float t)
    {
        if (t <= 0f || t >= 1f) return Mathf.Clamp01(t);
        return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI / 3f)) + 1f;
    }
}
