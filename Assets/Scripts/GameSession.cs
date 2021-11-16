using UnityEngine;

public class GameSession : MonoBehaviour
{
    public delegate void GameEventHandler();
    public static GameEventHandler OnGameOver;
    public static GameEventHandler OnGameReset;


    private int _score;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void AddScore(int score)
    {
        _score += score;
    }

}
