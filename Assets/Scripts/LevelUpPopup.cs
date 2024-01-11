using System;
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
        public float probabilityFactor = 1f;
    }

    [SerializeField] private Upgrade[] upgrades;
    [SerializeField] private GameObject upgradePrefab;
    [SerializeField] private int amountOfOptions = 3;
    [SerializeField] private float padding;

    private GameObject _stripe;
    private GameObject[] _options;
    private Button[] _buttons;

    private bool _active = false;

    private void Awake()
    {
        RectTransform stripeTransform = GetComponentsInChildren<RectTransform>()[1];
        Vector3 postion = stripeTransform.position;
        Rect chachedRect = stripeTransform.rect;
        float prefabWidth = upgradePrefab.GetComponent<RectTransform>().rect.width;
        postion.x -= chachedRect.width / 2;
        float distance = (chachedRect.width - 2 * padding - amountOfOptions * prefabWidth) / (amountOfOptions + 1);
        postion.x += padding;

        _options = new GameObject[amountOfOptions];
        _buttons = new Button[amountOfOptions];
        for (int i = 0; i < _options.Length; i++)
        {
            _options[i] = Instantiate(upgradePrefab, transform);
            postion.x += distance + 0.5f * prefabWidth;
            _options[i].transform.position = postion;
            postion.x += 0.5f * prefabWidth;

            _buttons[i] = _options[i].GetComponent<Button>();
        }

        _stripe = stripeTransform.gameObject;
        Hide();
    }

    private void Update()
    {
        // For testing
        if (Input.GetKeyDown(KeyCode.H))
            Toggle();

        if (!_active)
            return;

        for (int i = 0; i < _buttons.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                _buttons[i].onClick.Invoke();
        }
    }

    private void Toggle()
    {
        if (_active)
            Hide();
        else
            Show();
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
        for (int i = 0; i < _options.Length; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, upgrades.Length);
            } while (Random.value > upgrades[index].probabilityFactor);
            _options[i].GetComponent<Image>().sprite = upgrades[index].sprite;
            _buttons[i].onClick.RemoveAllListeners();
            _buttons[i].onClick.AddListener(delegate { upgrades[index].action?.Invoke(); });
            _buttons[i].onClick.AddListener(delegate { Hide(); });
        }

        foreach (GameObject obj in _options)
        {
            obj.SetActive(true);
        }
        _stripe.SetActive(true);
        _active = true;

        Time.timeScale = 0f;
    }

    // for testing
    public void Say(string text)
    {
        Debug.Log(text);
    }
}
