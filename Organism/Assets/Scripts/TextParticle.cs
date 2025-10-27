using UnityEngine;
using TMPro;

public class TextParticle : MonoBehaviour
{
    public float lifespan;      
    public float speed = 1f;
    public float lightHigh = 0.7f;
    public float lightLow = 0.4f;

    Vector2 dir;
    TextMeshPro tmp;

    enum State { Float, Cluster, Scatter }
    State state;

    void Start()
    {
        lifespan = Random.Range(10f, 20f);
        dir = Random.insideUnitCircle.normalized;
        state = State.Float;

        tmp = GetComponent<TextMeshPro>();
        tmp.text = ((char)Random.Range(65, 91)).ToString();   
    }

    void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0f || IsOutOfScreen()) { Destroy(gameObject); return; }

        float L = EnvironmentManager.Light;
        state = (L < lightLow) ? State.Cluster : (L > lightHigh ? State.Scatter : State.Float);

        switch (state)
        {
            case State.Float: transform.position += (Vector3)(dir * speed * Time.deltaTime); break;
            case State.Cluster: transform.position = Vector2.Lerp(transform.position, Vector2.zero, 0.1f * Time.deltaTime); break;
            case State.Scatter: transform.position += (Vector3)(((Vector2)transform.position).normalized * speed * Time.deltaTime); break;
        }
    }

    bool IsOutOfScreen()
    {
        var vp = Camera.main.WorldToViewportPoint(transform.position);
        return vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1;
    }
}