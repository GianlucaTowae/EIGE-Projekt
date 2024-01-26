using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScene : MonoBehaviour
{
    private bool _waited = false;

    private void Start()
    {
        Sounds.Play(Sounds.Sound.WIN);
        StartCoroutine(WaitTime());
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
