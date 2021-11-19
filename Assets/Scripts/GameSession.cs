using UnityEngine;

public class GameSession : MonoBehaviour
{
    public delegate void ScoreHandler(int score);
    public static ScoreHandler onScoreChanged;
    public static ScoreHandler onScoreAdd;

    public delegate void GameEventHandler();
    public static GameEventHandler onGameOver;
    public static GameEventHandler onGameReset;

    public static bool isPlaying = false;

    private static int _score;

    public static int Score => _score;

    private void Start()
    {
        onGameOver += () => isPlaying = false;

        onGameReset += () => isPlaying = true;
        onGameReset += () => SetScore(0);

        onScoreAdd += AddScore;
    }

    private void SetScore(int score)
    {
        _score = score;
        onScoreChanged?.Invoke(_score);
    }
    private void AddScore(int score)
    {
        SetScore(_score + score);
    }
}
