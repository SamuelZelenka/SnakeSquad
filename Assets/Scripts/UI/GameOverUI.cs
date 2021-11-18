using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Transform highScorePanel;
    [SerializeField] private GameObject scorePrefab;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        GameSession.onGameOver += () => gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameSession.onScoreChanged += (score) => scoreText.text = score.ToString();
    }
}
