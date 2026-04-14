using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private TextMeshProUGUI _bestText;

    private int _currentScore;
    private int _bestScore;

    private void Awake()
    {
        AssignInstance();
    }

    private void Start()
    {
        InitScoreManager();

        GameManager.Instance.gameOverAction += InitScoreManager;
    }

    private void InitScoreManager()
    {
        _currentScore = 0;
        _bestScore = PlayerPrefs.GetInt("BestScore");
        UpdateScoreText();
    }

    public void AddScore(int fruitLv)
    {
        int point = fruitLv + 10;
        _currentScore += point;

        _bestScore = Mathf.Max(_bestScore, _currentScore);
        PlayerPrefs.SetInt("BestScore", _bestScore);

        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        _scoreText.text = _currentScore.ToString();
        _bestText.text = _bestScore.ToString();
    }

    private void AssignInstance()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
}
