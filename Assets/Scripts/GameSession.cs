using UnityEngine;

public class GameSession : MonoBehaviour
{
    public delegate void ScoreHandler(int i);
    public static ScoreHandler OnInterract;

    private int _score;
    // Start is called before the first frame update
    void Start()
    {
        OnInterract += UpdateScore;
    }

    // Update is called once per frame
    void UpdateScore(int score)
    {
        _score += score;
    }
}
