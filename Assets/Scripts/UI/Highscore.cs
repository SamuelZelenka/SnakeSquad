using TMPro;
using System.Collections.Generic;
using UnityEngine;

public class Highscore : MonoBehaviour
{
    const string HIGHSCORE_FILENAME = "Highscore";

    [SerializeField] private Transform highScorePanel;
    [SerializeField] private HighscorePlayer scorePrefab;
    [SerializeField] private TMP_InputField nameInput;

    public void AddHighscore()
    {
        if (nameInput.text != "")
        {
            List<PlayerHighscoreData> players = GetHighScore();

            players.Add(new PlayerHighscoreData(nameInput.text, GameSession.Score));
            players.Sort();
            players.Reverse();

            HighscoreData highscoreData = new HighscoreData(players.ToArray());
            SaveSystem.SaveData(highscoreData, HIGHSCORE_FILENAME);
        }

        ShowHighScore();
    }
    private List<PlayerHighscoreData> GetHighScore()
    {
        List<PlayerHighscoreData> players = new List<PlayerHighscoreData>();
        HighscoreData data = SaveSystem.LoadData<HighscoreData>(HIGHSCORE_FILENAME);
        if (data == null)
        {
            return new List<PlayerHighscoreData>();
        }
        players.AddRange(data.players);
        return players;
    }
    public void ShowHighScore()
    {
        const int MAX_PLAYERS = 5;
        bool currentSessionDrawn = false;
        List<PlayerHighscoreData> players = GetHighScore();

        foreach (Transform child in highScorePanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count && i < MAX_PLAYERS; i++)
        {
            HighscorePlayer newPlayer = Instantiate(scorePrefab, highScorePanel);

            (string, int) playerValues = (players[i].name, players[i].score);

            bool isCurrentSession = playerValues.Item1 == nameInput.text && playerValues.Item2 == GameSession.Score;

            if (isCurrentSession && !currentSessionDrawn)
            {
                currentSessionDrawn = true;
                newPlayer.SetColor(Color.yellow);
            }

            newPlayer.SetValues(playerValues.Item1, playerValues.Item2);
        }
    }
}
