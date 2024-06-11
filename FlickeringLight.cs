using UnityEngine;
using System.Collections;

public class SimpleFlickeringLight : MonoBehaviour
{
    private Light lightSource;
    public float minIntensity = 0.5f;
    public float maxIntensity = 1.0f;
    public float flickerSpeed = 0.3f;
    

    void Start()
    {
        lightSource = GetComponent<Light>();         
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            lightSource.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}