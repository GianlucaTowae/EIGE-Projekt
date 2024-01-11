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
        DAMAGE,
        SPEED
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
            { Statistics.HEALTH, 5 }
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
        sb.Append(new String('\u2665', _values[Statistics.HEALTH]));
        sb.AppendLine();

        return sb.ToString();
    }
}