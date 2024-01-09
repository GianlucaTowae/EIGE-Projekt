using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LevelUpCanvas : MonoBehaviour
{
    [Serializable]
    private class Upgrade
    {
        public Sprite sprite;
        public UnityEvent action;
    }

    [SerializeField] private Upgrade[] upgrades;
    [SerializeField] private GameObject upgradePrefab;
    [SerializeField] private int amountOfOptions = 3;
    [SerializeField] private float padding;

    private Canvas _canvas;
    private GameObject[] _options;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();

        RectTransform stripeTransform = GetComponentsInChildren<RectTransform>()[1];
        Vector3 postion = stripeTransform.position;
        Rect chachedRect = stripeTransform.rect;
        float prefabWidth = upgradePrefab.GetComponent<RectTransform>().rect.width;
        postion.x -= chachedRect.width / 2;
        float distance = (chachedRect.width - 2 * padding - amountOfOptions * prefabWidth) / (amountOfOptions + 1);
        postion.x += padding;

        _options = new GameObject[amountOfOptions];
        for (int i = 0; i < _options.Length; i++)
        {
            _options[i] = Instantiate(upgradePrefab, transform);
            postion.x += distance + 0.5f * prefabWidth;
            _options[i].transform.position = postion;
            postion.x += 0.5f * prefabWidth;
        }
    }

    private void Update()
    {
        // For testing
        if (Input.GetKeyDown(KeyCode.H))
            Toggle();
    }

    private void Toggle()
    {
        if (_canvas.enabled)
            Hide();
        else
            Show();
    }

    private void Hide()
    {
        _canvas.enabled = false;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void Show()
    {
        for (int i = 0; i < _options.Length; i++)
        {
            int index = Random.Range(0, upgrades.Length);
            _options[i].GetComponent<Image>().sprite = upgrades[index].sprite;
            _options[i].GetComponent<Button>().onClick.RemoveAllListeners();
            _options[i].GetComponent<Button>().onClick.AddListener(delegate { upgrades[index].action?.Invoke(); });
            _options[i].GetComponent<Button>().onClick.AddListener(delegate { Hide(); });
        }
        _canvas.enabled = true;
    }

    // for testing
    public void Say(string text)
    {
        Debug.Log(text);
    }
}
