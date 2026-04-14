using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FruitSpawnerHandler : MonoBehaviour
{
    public Transform spawnPos;
    public List<GameObject> fruitPrefabs;

    private GameObject currentFruit;
    private Coroutine _runningCor;

    public float clampRange;
    public float moveSpeed;

    private int _nextFruitIdx;

    private void Awake()
    {
        for (int i = 0; i < fruitPrefabs.Count; i++)
        {
            var fruitMergeHandler = fruitPrefabs[i].GetComponent<FruitMergeHandler>();
            fruitMergeHandler.fruitLv = i;
        }
    }

    private void Start()
    {
        InitSpawnHandler();
    }

    private void Update()
    {
        FollowMouse();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_runningCor != null)
                return;

            _runningCor = StartCoroutine(DropCurrentFruitCor());
        }
    }

    private void InitSpawnHandler()
    {
        NextFruitPrepare();
        SpawnFruit(fruitPrefabs[_nextFruitIdx]);
    }

    private void SpawnFruit(GameObject fruit)
    {
        //NextFruitPrepare();

        currentFruit = Instantiate(fruit, spawnPos.position, Quaternion.identity, this.transform);
    }

    private void NextFruitPrepare()
    {
        _nextFruitIdx = Random.Range(0, fruitPrefabs.Count / 2);
        var nextFruit = fruitPrefabs[_nextFruitIdx];
        GameManager.Instance.UpdatePreviewImage(nextFruit);
    }

    private void FollowMouse()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        var mouseX = Camera.main.ScreenToWorldPoint(mouseScreenPos).x;
        mouseX = Mathf.Clamp(mouseX, -clampRange, clampRange);

        transform.position = Vector3.Lerp(
            transform.position,
            new Vector3(mouseX, transform.position.y, transform.position.z),
            Time.deltaTime * moveSpeed
        );
    }

    IEnumerator DropCurrentFruitCor()
    {
        currentFruit.transform.SetParent(GameManager.Instance.transform);
        currentFruit.GetComponent<Rigidbody2D>().gravityScale = 1.5f;
        currentFruit = null;
        NextFruitPrepare();
        yield return new WaitForSeconds(1f);
        SpawnFruit(fruitPrefabs[_nextFruitIdx]);
        _runningCor = null;
    }
}
