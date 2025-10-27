using UnityEngine;

public class PaperShard : MonoBehaviour
{
    [Header("随机移动")]
    public float wanderSpeed = 0.6f;
    public float wanderChangeInterval = 1.2f;

    [Header("吃")]
    public float huntSpeed = 1.8f;
    public float detectRadius = 1.8f;

    [Header("变光团")]
    public Sprite eatenSprite;
    public GameObject lightOrbPrefab;
    public float fallSpeed = 2f;

    [Header("自然死亡")]
    public float lifespan = 20f;

    int eaten = 0;
    enum State { Wander, Hunt, After3 }
    State state = State.Wander;

    SpriteRenderer sr;
    Vector2 wanderVel; float wanderTimer;
    bool falling = false, decided = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        var rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<PolygonCollider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.isTrigger = false;
        PickNewWander();
    }

    void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0f || IsOutOfScreen()) { Destroy(gameObject); return; }

        switch (state)
        {
            case State.Wander:
                {
                    var t = FindNearestText();
                    if (t) { state = State.Hunt; break; }
                    Move(wanderVel);
                    TickWander();
                    break;
                }
            case State.Hunt:
                {
                    if (eaten >= 3) { state = State.After3; break; }
                    var t = FindNearestText();
                    if (!t) { state = State.Wander; break; }
                    Move(((Vector2)t.position - (Vector2)transform.position).normalized * huntSpeed);
                    break;
                }
            case State.After3:
                {
                    if (!decided)
                    {
                        decided = true;
                        if (Random.value < 0.9f) { if (eatenSprite) sr.sprite = eatenSprite; falling = true; }
                        else { Instantiate(lightOrbPrefab, transform.position, Quaternion.identity); Destroy(gameObject); return; }
                    }
                    if (falling) Move(Vector2.down * fallSpeed);
                    break;
                }
        }
    }

    void Move(Vector2 v) => transform.position += (Vector3)(v * Time.deltaTime);

    void PickNewWander()
    {
        wanderVel = Random.insideUnitCircle.normalized * wanderSpeed;
        wanderTimer = Random.Range(wanderChangeInterval * 0.6f, wanderChangeInterval * 1.4f);
    }
    void TickWander() { wanderTimer -= Time.deltaTime; if (wanderTimer <= 0f) PickNewWander(); }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state == State.After3) return;
        if (other.CompareTag("Text"))
        {
            Destroy(other.gameObject);
            eaten++;
            if (eaten >= 3) state = State.After3;
        }
    }

    Transform FindNearestText()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);
        Transform best = null; float bestD = float.PositiveInfinity;
        foreach (var h in hits)
        {
            if (!h || !h.CompareTag("Text")) continue;
            float d = (h.transform.position - transform.position).sqrMagnitude;
            if (d < bestD) { bestD = d; best = h.transform; }
        }
        return best;
    }

    bool IsOutOfScreen()
    {
        var vp = Camera.main.WorldToViewportPoint(transform.position);
        return vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}