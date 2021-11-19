using System;

[Serializable]
public abstract class SaveData { }

public class HighscoreData : SaveData
{
    public PlayerHighscoreData[] players;
    public HighscoreData(PlayerHighscoreData[] players)
    {
        this.players = players;
    }
}

[Serializable]
public class PlayerHighscoreData : IComparable<PlayerHighscoreData>
{
    public string name;
    public int score;

    public PlayerHighscoreData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

    public int CompareTo(PlayerHighscoreData playerData)
    {
        if (playerData == null)
        {
            return 1;
        }
        else
        {
            return this.score.CompareTo(playerData.score);
        }
    }
}

