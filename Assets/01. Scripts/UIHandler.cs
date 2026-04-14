using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject popUpBackGround;
    public GameObject popUpMenu;

    public Button retryBtn;
    public Button exitBtn;

    [SerializeField]
    private Image _circleImg;
    private Material _vanishedMat;

    public float moveDuration;
    public Ease menuEase;

    private Vector3 _menuOriginPos;

    private void Awake()
    {
        _vanishedMat = _circleImg.material;
    }

    private void Start()
    {
        _menuOriginPos = popUpMenu.transform.localPosition;

        retryBtn.onClick.AddListener(OnClickRetryBtn);
        exitBtn.onClick.AddListener(OnClickExitBtn);

        GameManager.Instance.gameOverAction += UIMove;

        popUpBackGround.SetActive(false);
        popUpMenu.SetActive(false);

        OnVanishedCircle();
    }

    private void UIInit()
    {
        popUpMenu.transform.localPosition = _menuOriginPos;
        popUpMenu.SetActive(false);
        popUpBackGround.SetActive(false);
    }

    private void UIMove()
    {
        popUpBackGround.SetActive(true);
        popUpMenu.SetActive(true);

        popUpMenu
            .transform.DOLocalMove(Vector3.zero, moveDuration)
            .SetEase(menuEase)
            .SetUpdate(true) // timescale 영향 ㅇ안받게 해줌
            .OnComplete(() => Debug.Log("move complete"));
    }

    public void OnClickRetryBtn()
    {
        GameManager.Instance.ActivateRetryGame();
        UIInit();
    }

    public void OnClickExitBtn()
    {
        // 로비로 가기 전에 TimeScale초기화
        Time.timeScale = 1f;
        SceneManager.LoadScene("Lobby");
    }

    private void OnVanishedCircle()
    {
        Time.timeScale = 0;
        _vanishedMat.SetFloat("_Radius", -1.5f);

        DOTween
            .To(
                () => _vanishedMat.GetFloat("_Radius"),
                x => _vanishedMat.SetFloat("_Radius", x),
                1.3f,
                2f
            )
            .SetUpdate(true)
            .OnComplete(() =>
            {
                Time.timeScale = 1f;
                _circleImg.gameObject.SetActive(false);
            });
    }
}
