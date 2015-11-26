using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EaseType
{
    Linear,
    EaseInQuad,
    EaseOutQuad,
    EaseInOutQuad,
    EaseInCubic,
    EaseOutCubic,
    EaseInOutCubic,
    EaseInQuart,
    EaseOutQuart,
    EaseInOutQuart,
    EaseInQuint,
    EaseOutQuint,
    EaseInOutQuint,
    EaseInSine,
    EaseOutSine,
    EaseInOutSine,
    EaseInExpo,
    EaseOutExpo,
    EaseInOutExpo,
    EaseInCirc,
    EaseOutCirc,
    EaseInOutCirc
}

public class Easing
{
    #region Public Members
    public delegate Vector3 ToVector3<T>(T v);
    public delegate float Function(float startValue, float changeInValue, float elapsedTime, float totalTime);
    #endregion

    private static IEnumerable<float> NewCounter(int start, int end, int step)
    {
        // Generates sequence of integers from start to end(inclusive) one step at a time.
        for (int i = start; i <= end; i += step)
        {
            yield return i;
        }
    }

    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float duration)
    {
        // Returns sequence generator from start to end over duration using the
        // given easing function. The sequence is generated as it is accessed
        // using the Time.deltaTime to calculate the portion of duration that has elapsed
        IEnumerable<float> timer = Easing.NewTimer(duration);
        return NewEase(ease, start, end, duration, timer);
    }

    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, int slices)
    {
        // Instead of easing based on time, generate n interpolated points(slices)
        // between the start and end positions.
        IEnumerable<float> counter = Easing.NewCounter(0, slices + 1, 1);
        return NewEase(ease, start, end, slices + 1, counter);
    }

    public static Function EaseFunction(EaseType type)
    {
        //Returns the static method that implements the given easing type for scalars.
        //Use this method to easily switch between easing interpolation types.
        //All easing methods clamp elapsedTime so that it is always less than duration.
        Function f = null;
        switch (type)
        {
            case EaseType.Linear: f = Easing.Linear; break;
            case EaseType.EaseInQuad: f = Easing.EaseInQuad; break;
            case EaseType.EaseOutQuad: f = Easing.EaseOutQuad; break;
            case EaseType.EaseInOutQuad: f = Easing.EaseInOutQuad; break;
            case EaseType.EaseInCubic: f = Easing.EaseInCubic; break;
            case EaseType.EaseOutCubic: f = Easing.EaseOutCubic; break;
            case EaseType.EaseInOutCubic: f = Easing.EaseInOutCubic; break;
            case EaseType.EaseInQuart: f = Easing.EaseInQuart; break;
            case EaseType.EaseOutQuart: f = Easing.EaseOutQuart; break;
            case EaseType.EaseInOutQuart: f = Easing.EaseInOutQuart; break;
            case EaseType.EaseInQuint: f = Easing.EaseInQuint; break;
            case EaseType.EaseOutQuint: f = Easing.EaseOutQuint; break;
            case EaseType.EaseInOutQuint: f = Easing.EaseInOutQuint; break;
            case EaseType.EaseInSine: f = Easing.EaseInSine; break;
            case EaseType.EaseOutSine: f = Easing.EaseOutSine; break;
            case EaseType.EaseInOutSine: f = Easing.EaseInOutSine; break;
            case EaseType.EaseInExpo: f = Easing.EaseInExpo; break;
            case EaseType.EaseOutExpo: f = Easing.EaseOutExpo; break;
            case EaseType.EaseInOutExpo: f = Easing.EaseInOutExpo; break;
            case EaseType.EaseInCirc: f = Easing.EaseInCirc; break;
            case EaseType.EaseOutCirc: f = Easing.EaseOutCirc; break;
            case EaseType.EaseInOutCirc: f = Easing.EaseInOutCirc; break;
        }
        return f;
    }

    #region Private Easing Functions

    private static Vector3 Identity(Vector3 v)
    { return v; }

    private static Vector3 TransformDotPosition(Transform t)
    { return t.position; }


    private static IEnumerable<float> NewTimer(float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            yield return elapsedTime;
            elapsedTime += Time.deltaTime;
            // make sure last value is never skipped
            if (elapsedTime >= duration)
            {
                yield return elapsedTime;
            }
        }
    }

    private static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver)
    {
        //Generic easing sequence generator used to implement the time and
        //slice variants.Normally you would not use this function directly.
        Vector3 distance = end - start;
        foreach (float i in driver)
        {
            yield return EaseVector(ease, start, distance, i, total);
        }
    }

    /**
     * Vector3 interpolation using given easing method. Easing is done independently
     * on all three vector axis.
     */
    private static Vector3 EaseVector(Function ease, Vector3 start, Vector3 distance, float elapsedTime, float duration)
    {
        start.x = ease(start.x, distance.x, elapsedTime, duration);
        start.y = ease(start.y, distance.y, elapsedTime, duration);
        start.z = ease(start.z, distance.z, elapsedTime, duration);
        return start;
    }

    private static float Linear(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * (elapsedTime / duration) + start;
    }

    private static float EaseInQuad(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime + start;
    }

    private static float EaseOutQuad(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return -distance * elapsedTime * (elapsedTime - 2) + start;
    }

    private static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime + start;
        elapsedTime--;
        return -distance / 2 * (elapsedTime * (elapsedTime - 2) - 1) + start;
    }

    private static float EaseInCubic(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime + start;
    }

    private static float EaseOutCubic(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * (elapsedTime * elapsedTime * elapsedTime + 1) + start;
    }

    private static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return distance / 2 * (elapsedTime * elapsedTime * elapsedTime + 2) + start;
    }

    private static float EaseInQuart(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
    }

    private static float EaseOutQuart(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1) + start;
    }

    private static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return -distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2) + start;
    }

    private static float EaseInQuint(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
    }

    private static float EaseOutQuint(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1) + start;
    }

    private static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2f);
        if (elapsedTime < 1) return distance / 2 * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
        elapsedTime -= 2;
        return distance / 2 * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2) + start;
    }

    private static float EaseInSine(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return -distance * Mathf.Cos(elapsedTime / duration * (Mathf.PI / 2)) + distance + start;
    }

    private static float EaseOutSine(float start, float distance, float elapsedTime, float duration)
    {
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * Mathf.Sin(elapsedTime / duration * (Mathf.PI / 2)) + start;
    }

    private static float EaseInOutSine(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return -distance / 2 * (Mathf.Cos(Mathf.PI * elapsedTime / duration) - 1) + start;
    }

    private static float EaseInExpo(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * Mathf.Pow(2, 10 * (elapsedTime / duration - 1)) + start;
    }

    private static float EaseOutExpo(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime to be <= duration
        if (elapsedTime > duration) { elapsedTime = duration; }
        return distance * (-Mathf.Pow(2, -10 * elapsedTime / duration) + 1) + start;
    }

    private static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return distance / 2 * Mathf.Pow(2, 10 * (elapsedTime - 1)) + start;
        elapsedTime--;
        return distance / 2 * (-Mathf.Pow(2, -10 * elapsedTime) + 2) + start;
    }

    private static float EaseInCirc(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        return -distance * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
    }

    private static float EaseOutCirc(float start, float distance, float elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 1.0f : elapsedTime / duration;
        elapsedTime--;
        return distance * Mathf.Sqrt(1 - elapsedTime * elapsedTime) + start;
    }

    private static float EaseInOutCirc(float start, float distance, float
                         elapsedTime, float duration)
    {
        // clamp elapsedTime so that it cannot be greater than duration
        elapsedTime = (elapsedTime > duration) ? 2.0f : elapsedTime / (duration / 2);
        if (elapsedTime < 1) return -distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) - 1) + start;
        elapsedTime -= 2;
        return distance / 2 * (Mathf.Sqrt(1 - elapsedTime * elapsedTime) + 1) + start;
    }

    #endregion
}

/**
 * Sequence generators are used as follows:
 *
 * IEnumerable<Vector3> sequence = Easing.NewEase(configuration);
 * foreach (Vector3 newPoint in sequence) {
 *   transform.position = newPoint;
 *   yield return WaitForSeconds(1.0f);
 * }
 *
 * Or:
 *
 * IEnumerator<Vector3> sequence = Ease.NewEase(configuration).GetEnumerator();
 * function Update() {
 *   if (sequence.MoveNext()) {
 *     transform.position = sequence.Current;
 *   }
 * }
 *
 * The low level functions work similarly to Unity's built in Lerp and it is
 * up to you to track and pass in elapsedTime and duration on every call. 
 *
 * float value = ease(start, distance, elapsedTime, duration);
 */
