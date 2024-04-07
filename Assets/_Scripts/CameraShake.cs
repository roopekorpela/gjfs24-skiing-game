using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake singelton;

    public AnimationCurve animationCurve;

    public bool start = false;
    public float duration = 1f;
    private void Awake()
    {
        singelton = this;
    }

    private void Update()
    {
        if (start)
        {
            StartCoroutine(ShakeCamera());
        }
    }

    public IEnumerator ShakeCamera()
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime;
            float strength = animationCurve.Evaluate(elapsedTime / duration);
            transform.position = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPos;
    }
}
