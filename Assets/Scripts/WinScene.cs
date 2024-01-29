using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScene : MonoBehaviour
{
    [SerializeField] private TMP_Text highscoreLabel;

    private bool _waited = false;

    private void Start()
    {
        Sounds.Play(Sounds.Sound.WIN);
        StartCoroutine(WaitTime());
        int highscore = PlayerPrefs.GetInt("highscore");
        int score = PlayerPrefs.GetInt("score");
        highscoreLabel.text = "Highscore: " + highscore
                                            + "\nScore: " + score
                                            + (score == highscore ? "\n<i>New Highscore!</i>" : "");
    }

    void Update()
    {
        if (_waited && Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("StartScene");
    }

    private IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1f);
        _waited = true;
    }
}
