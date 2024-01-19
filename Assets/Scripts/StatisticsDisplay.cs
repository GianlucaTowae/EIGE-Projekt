using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StatisticsDisplay : MonoBehaviour
{
    public enum Statistics
    {
        HEALTH,
        MAX_HEALTH,
        DAMAGE,
        SPEED,
    }

    private Dictionary<Statistics, int> _values;

    private TMP_Text _textField;

    private void Awake()
    {
        _textField = GetComponent<TMP_Text>();
        _values = new Dictionary<Statistics, int>
        {
            { Statistics.SPEED, 100 },
            { Statistics.DAMAGE, 100 },
            { Statistics.HEALTH, 5 },
            { Statistics.MAX_HEALTH, 5 },
        };
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        _textField.text = ToString();
    }

    public void SetStatistic(Statistics type, int value)
    {
        _values[type] = value;
        UpdateDisplayText();
        switch (type)
        {
            case Statistics.DAMAGE:
                StaticValues.damage = value;
                break;
            case Statistics.SPEED:
                StaticValues.speed = value;
                break;
            case Statistics.MAX_HEALTH:
                StaticValues.maxHealth = value;
                break;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(nameof(Statistics.SPEED).ToLower().FirstCharacterToUpper());
        sb.Append(": ");
        sb.Append(_values[Statistics.SPEED]);
        sb.Append("%");
        sb.AppendLine();

        sb.Append(nameof(Statistics.DAMAGE).ToLower().FirstCharacterToUpper());
        sb.Append(": ");
        sb.Append(_values[Statistics.DAMAGE]);
        sb.AppendLine();

        sb.Append(nameof(Statistics.HEALTH).ToLower().FirstCharacterToUpper());
        sb.Append(": ");
        for (int i = 0; i <  _values[Statistics.MAX_HEALTH]; i++)
        {
            if (i < _values[Statistics.HEALTH])
                sb.Append('\u2665');
            else
                sb.Append('\u2661');
        }
        sb.AppendLine();

        return sb.ToString();
    }
}