using UnityEngine;

public class LightOrb : MonoBehaviour
{
    [Header("自然死亡")]
    public float lifespan = 30f;

    [Header("移动")]
    public float moveSpeed = 0.8f;
    public float wanderChangeInterval = 1.2f;
    Vector2 wanderVel; float wanderTimer;

    [Header("影响文字")]
    public float detectRadius = 2.0f;
    public float influence = 0.6f;
    public string textTag = "Text";

    [Header("图像")]
    public float highThreshold = 0.7f;
    public float lowThreshold = 0.4f;
    public Sprite idleSprite, attractSprite, repelSprite;

    enum State { Idle, Attract, Repel }
    State state = State.Idle;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        PickNewWander();
    }

    void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0f || IsOutOfScreen()) { Destroy(gameObject); return; }

        float L = EnvironmentManager.Light;
        state = (L > highThreshold) ? State.Attract : (L < lowThreshold ? State.Repel : State.Idle);
        sr.sprite = (state == State.Attract) ? (attractSprite ? attractSprite : sr.sprite)
                  : (state == State.Repel) ? (repelSprite ? repelSprite : sr.sprite)
                  : (idleSprite ? idleSprite : sr.sprite);

        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f) PickNewWander();
        transform.position += (Vector3)(wanderVel * Time.deltaTime);

        var hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);
        foreach (var h in hits)
        {
            if (!h || !h.CompareTag(textTag)) continue;

            Vector3 dir = (h.transform.position - transform.position).normalized;
            float s = influence * Time.deltaTime;
            if (state == State.Attract) h.transform.position -= dir * s;
            else if (state == State.Repel) h.transform.position += dir * s;
        }
    }

    void PickNewWander()
    {
        wanderVel = Random.insideUnitCircle.normalized * moveSpeed;
        wanderTimer = Random.Range(wanderChangeInterval * 0.6f, wanderChangeInterval * 1.4f);
    }

    bool IsOutOfScreen()
    {
        var vp = Camera.main.WorldToViewportPoint(transform.position);
        return vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}