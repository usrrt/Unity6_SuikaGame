using UnityEngine;

public class FruitMergeHandler : MonoBehaviour
{
    private FruitSpawnerHandler _fruitSpawner;

    public bool IsDropped { get; private set; }
    public int fruitLv;

    private void Awake()
    {
        _fruitSpawner = GetComponentInParent<FruitSpawnerHandler>();
    }

    // 面倒矫 惑困苞老肺 钦己
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var hitFruit = collision.gameObject.GetComponent<FruitMergeHandler>();

        if (hitFruit == null)
            return;

        if (fruitLv == 9)
            return;

        hitFruit.IsDropped = true;

        if (hitFruit.fruitLv == fruitLv)
        {
            if (hitFruit.GetEntityId() < GetEntityId())
                return;

            ScoreManager.Instance.AddScore(fruitLv);
            MergeFruit(hitFruit);
        }
    }

    private void MergeFruit(FruitMergeHandler hitFruit)
    {
        // Debug.Log("merge");
        var spawnPos = (hitFruit.transform.position + transform.position) / 2;

        var mergeFruit = Instantiate(
            _fruitSpawner.fruitPrefabs[++fruitLv],
            spawnPos,
            Quaternion.identity,
            GameManager.Instance.transform
        );

        mergeFruit.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        var fruitHandler = mergeFruit.GetComponent<FruitMergeHandler>();
        fruitHandler._fruitSpawner = _fruitSpawner;
        fruitHandler.IsDropped = true; // 货肺 钦己等 苞老 贸府

        Destroy(hitFruit.gameObject);
        Destroy(gameObject);
    }
}
