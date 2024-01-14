using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpScene : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("StartScene");
    }
}
