using System;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public delegate void ScoreHandler(int score);
    public static ScoreHandler onScoreChanged;
    public static ScoreHandler onScoreAdd;

    public delegate void GameEventHandler();
    public static GameEventHandler onGameOver;
    public static GameEventHandler onGameReset;

    public static bool isPlaying = true;

    private static int _score;

    private void Start()
    {
        onGameOver += () => print("ded");
        onGameOver += () => isPlaying = false;
        onGameReset += () => isPlaying = true;
        onScoreAdd += AddScore;
    }

    // Update is called once per frame
    private void AddScore(int score)
    {
        _score += score;
        onScoreChanged?.Invoke(_score);
    }
}
