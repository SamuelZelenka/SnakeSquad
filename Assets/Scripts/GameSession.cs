using System;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public delegate void ScoreHandler(int score);

    public static ScoreHandler OnScoreChanged;
    public static ScoreHandler OnScoreAdd;
    
    
    public delegate void GameEventHandler();
    public static GameEventHandler OnGameOver;
    public static GameEventHandler OnGameReset;

    public static bool isPlaying = true;

    private int _score;

    void Start()
    {
        OnGameOver += () => print("ded");
    }

    // Update is called once per frame
    void AddScore(int score)
    {
        _score += score;
        OnScoreAdd?.Invoke(score);
    }
}
