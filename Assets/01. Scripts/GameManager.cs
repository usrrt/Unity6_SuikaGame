using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Action gameOverAction;

    [SerializeField]
    Image _previewImg;

    private void Awake()
    {
        AssignInstance();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 떨어지는 과일 제외 -> 다른과일과 충돌한 과일, 합성된 과일은 IsDropped 활성화
        var hitFruit = collision.GetComponent<FruitMergeHandler>();

        if (hitFruit == null)
            Debug.Log("null");
        Debug.Log(hitFruit.IsDropped);

        if (hitFruit.IsDropped)
        {
            ActivateGameOver();
        }
    }

    public void UpdatePreviewImage(GameObject nextFruit)
    {
        if (Instance == null)
        {
            Debug.Log("GameManager is null");
            AssignInstance();
        }

        Sprite previewSprite = nextFruit.GetComponent<SpriteRenderer>().sprite;
        _previewImg.sprite = previewSprite;
    }

    public void ActivateGameOver()
    {
        // 게임 종료
        Debug.Log("GameOver");

        // 일시정지 후 재시작 UI
        Time.timeScale = 0f;
        gameOverAction.Invoke();
    }

    public void ActivateRetryGame()
    {
        Time.timeScale = 1f;

        // 과일 초기화
        ClearAllFruits();
    }

    //private void ActivateStartGame()
    //{
    //    Time.timeScale = 0;

    //    _transitionMat.SetFloat("_Radius", -2f);

    //    DOTween
    //        .To(
    //            () => _transitionMat.GetFloat("_Radius"),
    //            x => _transitionMat.SetFloat("_Radius", x),
    //            1.3f,
    //            2f
    //        )
    //        .SetUpdate(true)
    //        .OnComplete(() =>
    //        {
    //            Time.timeScale = 1f;
    //            _transitionImg.gameObject.SetActive(false);
    //        });
    //}

    private void ClearAllFruits()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void AssignInstance()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
