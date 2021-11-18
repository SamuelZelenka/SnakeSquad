using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Image _screenGlow;
    [SerializeField] private GameObject _gameOverView;
    [SerializeField] private GameObject _scoreView;
    [SerializeField] private TextMeshProUGUI _scoreText;
    private void Start()
    {
        Snake.onMoveTick += ScreenGlowEffect;
        GameSession.onScoreChanged += UpdateScore;
        GameSession.onGameOver += GameOver;
    }

    private void ScreenGlowEffect(Vector2Int coordinate)
    {
        Color color = _screenGlow.color;
        _screenGlow.color = new Color(color.r, color.g, color.b, Random.value);
    }

    private void UpdateScore(int score)
    {
        _scoreText.text = score.ToString();
    }

    public void GameOver()
    {
        _gameOverView.SetActive(true);
        _scoreView.SetActive(false);
    }

    public void RestartGame()
    {
        GameSession.onGameReset?.Invoke();
        _gameOverView.SetActive(false);
        _scoreView.SetActive(true);
        GameSession.isPlaying = true;
        

    }
}
