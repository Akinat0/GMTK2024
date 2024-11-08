using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

public static class Coroutines
{
    public class CoroutinesRunner : MonoBehaviour
    {
    }
    
    static CoroutinesRunner coroutinesRunner;
    
    public static void StartCoroutine(IEnumerator coroutine)
    {
        if (coroutinesRunner == null)
            coroutinesRunner = new GameObject("coroutines_runner").AddComponent<CoroutinesRunner>();

        coroutinesRunner.StartCoroutine(coroutine);
    }

    public static void StopCoroutine(IEnumerator coroutine)
    {
        if(coroutinesRunner == null) //probably coroutine runner is already destroyed
            return;
        
        coroutinesRunner.StopCoroutine(coroutine);
    }
    
    public static IEnumerator Delay(float delay, Action finished = null)
    {
        yield return new WaitForSeconds(delay);
        finished?.Invoke();
    }

    public static IEnumerator DelayRealtime(float delay, Action finished = null)
    {
        yield return new WaitForSecondsRealtime(delay);
        finished?.Invoke();
    }

    public static IEnumerator LerpRoutine(Func<float> valueGetter, Action<float> valueSetter, float targetValue, float duration, Action finished = null)
    {
        float sourceValue = valueGetter();
        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;
            valueSetter(Mathf.Lerp(sourceValue, targetValue, time / duration));
        }
        
        valueSetter(targetValue);
        
        finished?.Invoke();
    }

    public static IEnumerator WaitUntil(Func<bool> predicate, Action finished)
    {
        yield return new WaitUntil(predicate);
        finished?.Invoke();
    }
    
    public static IEnumerator Repeat(float delay, [NotNull] Action repeatAction)
    {
        while (true)
        {
            repeatAction.Invoke();
            yield return new WaitForSeconds(delay);
        }
    }
    
    public static IEnumerator RepeatRealtime(float delay, [NotNull] Action repeatAction)
    {
        while (true)
        {
            repeatAction.Invoke();
            yield return new WaitForSecondsRealtime(delay);
        }
    }
    
    public static IEnumerator RepeatUntil(float delay, [NotNull] Action repeatAction, [NotNull] Func<bool> shouldRepeat, Action finished = null)
    {
        while (shouldRepeat.Invoke())
        {
            repeatAction.Invoke();
            yield return new WaitForSeconds(delay);
        }
        
        finished?.Invoke();
    }
    
    public static IEnumerator RepeatUntilRealtime(float delay, [NotNull] Action repeatAction, [NotNull] Func<bool> shouldRepeat, Action finished = null)
    {
        while (shouldRepeat.Invoke())
        {
            repeatAction.Invoke();
            yield return new WaitForSecondsRealtime(delay);
        }
        
        finished?.Invoke();
    }

    public static IEnumerator FramesDelay(int frames, Action finished = null)
    {
        for (int i = 0; i < frames; i++)
            yield return null;

        finished?.Invoke();
    }

    public static IEnumerator MoveLocal(float duration, Transform transform, Vector3 targetPosition, Quaternion targetRotation, Action finished = null)
    {
        Vector3 sourcePosition = transform.localPosition;
        Quaternion sourceRotation = transform.localRotation;
        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;

            float phase = time / duration;
            
            transform.localPosition = Vector3.Lerp(sourcePosition, targetPosition, phase);
            transform.localRotation = Quaternion.Lerp(sourceRotation, targetRotation, phase);
        }
        
        transform.localPosition =  targetPosition;
        transform.localRotation = targetRotation;
        
        finished?.Invoke();
    }
    
    public static IEnumerator Move(float duration, Transform transform, Vector3 targetPosition, Vector3 targetScale, Action finished = null)
    {
        Vector3 sourcePosition = transform.position;
        Vector3 sourceScale = transform.localScale;
        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;

            float phase = time / duration;
            
            transform.position = Vector3.Lerp(sourcePosition, targetPosition, phase);
            transform.localScale = Vector3.Lerp(sourceScale, targetScale, phase);
        }
        
        transform.position =  targetPosition;
        transform.localScale =  targetScale;
        
        finished?.Invoke();
    }
    
    public static IEnumerator MoveLocalRealtime(float duration, Transform transform, Vector3 targetPosition, Quaternion targetRotation, Action finished = null)
    {
        Vector3 sourcePosition = transform.localPosition;
        Quaternion sourceRotation = transform.localRotation;
        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.unscaledDeltaTime;

            float phase = time / duration;
            
            transform.localPosition = Vector3.Lerp(sourcePosition, targetPosition, phase);
            transform.localRotation = Quaternion.Lerp(sourceRotation, targetRotation, phase);
        }
        
        transform.localPosition =  targetPosition;
        transform.localRotation = targetRotation;
        
        finished?.Invoke();
    }
    
    public static IEnumerator ScaleLocal(float duration, Transform transform, Vector3 targetScale, Action finished = null)
    {
        Vector3 sourceScale = transform.localScale;
        
        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;

            float phase = time / duration;
            
            transform.localScale = Vector3.Lerp(sourceScale, targetScale, phase);
        }
        
        transform.localScale = targetScale;
        
        finished?.Invoke();
    }
    
    public static IEnumerator ScaleLocal(Transform transform, AnimationCurve scaleCurve, Action finished = null)
    {
        float time = 0;

        float duration = scaleCurve.keys[scaleCurve.length - 1].time;
        
        while (time < duration)
        {
            yield return null;
            
            time += Time.deltaTime;
            transform.localScale = Vector3.one * scaleCurve.Evaluate(time);
        }
        
        transform.localScale = Vector3.one * scaleCurve.Evaluate(duration);
        
        finished?.Invoke();
    }
    
    public static IEnumerator MoveToTransform(float duration, Transform transform, Transform target, Action onUpdate = null, Action finished = null)
    {
        Vector3 sourcePosition = transform.position;
        Quaternion sourceRotation = transform.rotation;
        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;

            float phase = time / duration;
            
            transform.position = Vector3.Lerp(sourcePosition, target.position, phase);
            transform.rotation = Quaternion.Lerp(sourceRotation, target.rotation, phase);
            
            onUpdate?.Invoke();
        }
        
        transform.position = target.position;
        transform.rotation = target.rotation;
        
        finished?.Invoke();
    }
    
    public static IEnumerator MoveToTransformRealtime(float duration, Transform transform, Transform target, Action finished = null)
    {
        Vector3 sourcePosition = transform.position;
        Quaternion sourceRotation = transform.rotation;
        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.unscaledDeltaTime;

            float phase = time / duration;
            
            transform.position = Vector3.Lerp(sourcePosition, target.position, phase);
            transform.rotation = Quaternion.Lerp(sourceRotation, target.rotation, phase);
        }
        
        transform.position = target.position;
        transform.rotation = target.rotation;
        
        finished?.Invoke();
    }

    public static IEnumerator DropObjectRoutine(Transform spawnedObject, Vector3 spawnLocation, float radius, float height, float duration)
    {
        float angle = UnityEngine.Random.Range(0, 360);

        Vector3 targetLocation = spawnLocation + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;

        if (Physics.Raycast(targetLocation + Vector3.up * 100, Vector3.down, out var result, 1000, LayerMask.GetMask("Terrain")))
            if (result.point.y > targetLocation.y)
                targetLocation = result.point + Vector3.up;

        float time = 0;

        while (time < duration)
        {
            yield return null;
            time += Time.deltaTime;

            float phase = time / duration;

            Vector3 heightOffset = new Vector3(0, height, 0) * Mathf.PingPong(phase, 0.5f);
            spawnedObject.position = Vector3.Lerp(spawnLocation, targetLocation, phase) + heightOffset;
        }

        spawnedObject.position = targetLocation;
    }
}
