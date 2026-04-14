using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Button startBtn;
    public GameObject titleObj;

    private RectTransform _titleRect;
    private RectTransform _startBtnRect;

    private Vector3 _originTitlePos;
    private Vector3 _originStartBtnPos;

    public float duration;
    public Ease ease;

    private void Awake()
    {
        _titleRect = titleObj.GetComponent<RectTransform>();
        _startBtnRect = startBtn.GetComponent<RectTransform>();
    }

    private void Start()
    {
        startBtn.onClick.RemoveAllListeners();
        startBtn.onClick.AddListener(OnClickStartBtn);

        _originTitlePos = _titleRect.position;
        _originStartBtnPos = _startBtnRect.position;
    }

    private void OnClickStartBtn()
    {
        Debug.Log("스타트");

        // ui 이동 완료 후에 씬 로드
        _titleRect.DOAnchorPosY(300f, duration).SetEase(ease);
        _startBtnRect
            .DOAnchorPosY(-200f, duration)
            .SetEase(ease)
            .OnComplete(() =>
            {
                Debug.Log("씬 전환");
                SceneManager.LoadScene("Game");
            });
    }
}
