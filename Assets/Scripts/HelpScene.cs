using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpScene : MonoBehaviour
{
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Image next;
    [SerializeField] private Image previous;
    private int _page;

    private void Start()
    {
        SetPage(0);
        next.GetComponent<Button>().onClick.AddListener(NextPage);
        previous.GetComponent<Button>().onClick.AddListener(PreviousPage);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("StartScene");

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            NextPage();
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            PreviousPage();
    }

    private void PreviousPage()
    {
        SetPage(_page - 1);
    }

    private void NextPage()
    {
        SetPage(_page + 1);
    }

    private void SetPage(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pages.Length)
            return;

        _page = pageIndex;
        foreach (GameObject page in pages)
        {
            page.SetActive(false);
        }
        pages[_page].SetActive(true);

        previous.enabled = _page != 0;
        next.enabled = _page != pages.Length - 1;
    }
}
