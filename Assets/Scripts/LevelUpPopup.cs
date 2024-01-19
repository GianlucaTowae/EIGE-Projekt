using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelUpPopup : MonoBehaviour
{
    [Serializable]
    private class Upgrade
    {
        public Sprite sprite;
        public UnityEvent action;
    }

    [SerializeField] private Upgrade[] upgrades;
    [SerializeField] private GameObject upgradePrefab;
    [SerializeField] private float padding;

    private GameObject _stripe;
    private GameObject[] _options;
    private Button[] _buttons;

    private bool _active = false;

    private void Awake()
    {
        int amountOfOptions = upgrades.Length;

        float screenWidth = GetComponent<RectTransform>().rect.width;
        float screenHeight = GetComponent<RectTransform>().rect.height;
        float prefabWidth = upgradePrefab.GetComponent<RectTransform>().rect.width;
        Vector3 postion = new Vector3(0f, screenHeight / 2, 0f);
        float distance = (screenWidth - 2 * padding - amountOfOptions * prefabWidth) / (amountOfOptions + 1);
        postion.x += padding;

        _options = new GameObject[amountOfOptions];
        _buttons = new Button[amountOfOptions];
        for (int i = 0; i < amountOfOptions; i++)
        {
            // Instantiate
            _options[i] = Instantiate(upgradePrefab, transform);

            // Position
            postion.x += distance + 0.5f * prefabWidth;
            _options[i].transform.position = postion;
            postion.x += 0.5f * prefabWidth;

            // Set image
            _options[i].GetComponent<Image>().sprite = upgrades[i].sprite;

            // Set action
            _buttons[i] = _options[i].GetComponent<Button>();
            // Takes last state of i otherwise, which causes an IndexOutOfRangeException
            int index = i;
            _buttons[i].onClick.AddListener(delegate { upgrades[index].action?.Invoke(); });
            _buttons[i].onClick.AddListener(Hide);
        }

        _stripe = transform.GetChild(0).gameObject;
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            Show();

        if (!_active)
            return;

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                _buttons[i].onClick.Invoke();
        }
    }

    private void Hide()
    {
        foreach (GameObject obj in _options)
        {
            obj.SetActive(false);
        }
        _stripe.SetActive(false);
        _active = false;

        Time.timeScale = 1f;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Show()
    {
        foreach (GameObject obj in _options)
        {
            obj.SetActive(true);
        }
        _stripe.SetActive(true);
        _active = true;

        Time.timeScale = 0f;
    }

    // for testing
    private void Say(string text)
    {
        Debug.Log(text);
    }
}
