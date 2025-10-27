using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static float Light { get; private set; } 

    [Range(0f, 2f)] public float speed = 0.2f;       //speed of light change

    void Update()
    {
        Light = 0.5f + 0.5f * Mathf.Sin(Time.time * speed); 
    }
}