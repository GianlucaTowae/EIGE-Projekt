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
        public float probabilityFactor = 1f;
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

        _canvas.enabled = false;
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
        foreach (GameObject option in _options)
        {
            int index;
            do
            {
                index = Random.Range(0, upgrades.Length);
            } while (Random.value > upgrades[index].probabilityFactor);
            option.GetComponent<Image>().sprite = upgrades[index].sprite;
            option.GetComponent<Button>().onClick.RemoveAllListeners();
            option.GetComponent<Button>().onClick.AddListener(delegate { upgrades[index].action?.Invoke(); });
            option.GetComponent<Button>().onClick.AddListener(delegate { Hide(); });
        }

        _canvas.enabled = true;
    }

    // for testing
    public void Say(string text)
    {
        Debug.Log(text);
    }
}
