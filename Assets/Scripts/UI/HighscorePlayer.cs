using TMPro;
using UnityEngine;

public class HighscorePlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _highscoreText;

    public void SetValues(string name, int score)
    {
        _highscoreText.text = score.ToString() + "\t" + name;
    }
    public void SetColor(Color color)
    {
        _highscoreText.color = color;
    }
}
