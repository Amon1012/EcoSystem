using UnityEngine;

public class SpawnerSimple : MonoBehaviour
{
    public GameObject prefab;
    public int maxCount = 50;
    public float spawnInterval = 0.1f;

    [Header("°´±³¾°·¶Î§")]
    public SpriteRenderer backgroundSprite;
    public float padding = 0.5f;

    float timer;

    void Update()
    {
        if (transform.childCount >= maxCount) return;

        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;

        Instantiate(prefab, RandomPointInArea(), Quaternion.identity, transform);
    }

    Vector3 RandomPointInArea()
    {
        if (backgroundSprite)
        {
            var b = backgroundSprite.bounds;
            return new Vector3(Random.Range(b.min.x + padding, b.max.x - padding), Random.Range(b.min.y + padding, b.max.y - padding), 0f);
        }

        var cam = Camera.main;
        float h = cam.orthographicSize, w = h * cam.aspect;
        return new Vector3(Random.Range(-w + padding, w - padding), Random.Range(-h + padding, h - padding), 0f);
    }
}