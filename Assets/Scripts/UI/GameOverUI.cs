using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Highscore highscore;

    private void Start()
    {
        GameSession.onGameOver += InitGameOver;

        GameSession.onScoreChanged += (score) => scoreText.text = score.ToString();
        gameObject.SetActive(false);
    }

    private void InitGameOver()
    {
        gameObject.SetActive(true);
        highscore.AddHighscore();
    }
}
