using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScene : MonoBehaviour
{
    [SerializeField] private TMP_Text highscore;

    private bool _waited = false;

    private void Start()
    {
        Sounds.Play(Sounds.Sound.WIN);
        StartCoroutine(WaitTime());
        highscore.text = "Highscore: " + PlayerPrefs.GetInt("highscore")
                        + "\nScore: " + PlayerPrefs.GetInt("score");
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
