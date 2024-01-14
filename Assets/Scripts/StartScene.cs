using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField] private float blinkInterval = 0.8f;

    private bool _changeState = false;
    private TMP_Text _text;

    private void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
        StartCoroutine(Blink());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene("GameScene");
        if (_changeState)
        {
            _text.enabled = !_text.enabled;
            _changeState = false;
            StartCoroutine(Blink());
        }
    }

    private IEnumerator Blink()
    {
        yield return new WaitForSeconds(blinkInterval);
        _changeState = true;
    }

    public void LoadHelpScene()
    {
        SceneManager.LoadScene("HelpScene");
    }

}
