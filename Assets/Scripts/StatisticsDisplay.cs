using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StatisticsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject StatCanvas;
    public enum Statistics
    {
        HEALTH,
        DAMAGE,
        SPEED,
        SHIELD,
    }

    private Dictionary<Statistics, int> _values;

    private TMP_Text _textField;

    private void Awake()
    {
        _textField = StatCanvas.GetComponent<TMP_Text>();
        _values = new Dictionary<Statistics, int>
        {
            { Statistics.SPEED, 100 },
            { Statistics.DAMAGE, 100 },
            { Statistics.HEALTH, 5 },
            { Statistics.SHIELD, 0},
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
            case Statistics.HEALTH:
                StaticValues.health = value;
                break;
            case Statistics.DAMAGE:
                StaticValues.damage = value;
                break;
            case Statistics.SPEED:
                StaticValues.speed = value;
                break;
            case Statistics.SHIELD:
                StaticValues.shield = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
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
        for (int i = 0; i <  _values[Statistics.HEALTH]; i++)
        {
            sb.Append('\u2665');
        }
        sb.AppendLine();

        if(_values[Statistics.SHIELD] > 0){
            sb.Append(nameof(Statistics.SHIELD).ToLower().FirstCharacterToUpper());
            sb.Append(": ");
            sb.Append(new String('\u2665', _values[Statistics.SHIELD]));
            sb.AppendLine();
        }

        return sb.ToString();
    }
}