using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Color = System.Drawing.Color;

public class SoundAttributions : MonoBehaviour
{
    private enum License
    {
        CEATIVE_COMMONS_0,
        ATTRIBUTION_3_0,
        ATTRIBUTION_4_0,
        ATTRIBUTION_NC_4_0
    }

    [Serializable]
    private class Attribution
    {
        public string soundName;
        public string link;
        public string author;
        public License license;
    }

    [SerializeField] private List<Attribution> soundAttributions;

    private TMP_Text _text;
    private Camera _camera;

    private void Awake ()
    {
        _text = GetComponent<TMP_Text>();
        _camera = GetComponentInParent<Canvas>().worldCamera;

        _text.text = soundAttributions.Aggregate("", (current, attr) => current + ToText(attr));
    }

    private string ToText(Attribution sa)
    {
        return "\"" + Link(sa.soundName, soundAttributions.IndexOf(sa).ToString()) + "\" by " + sa.author
                      + "\nLICENSED UNDER " + sa.license switch
            {
                License.CEATIVE_COMMONS_0 => Link("Creative Commons 0", soundAttributions.IndexOf(sa) + "license"),
                License.ATTRIBUTION_3_0 => Link("Attribution 3.0", soundAttributions.IndexOf(sa) + "license"),
                License.ATTRIBUTION_4_0 => Link("Attribution 4.0", soundAttributions.IndexOf(sa) + "license"),
                License.ATTRIBUTION_NC_4_0 => Link("Attribution NonCommercial 4.0", soundAttributions.IndexOf(sa) + "license")
            } + "\n";
    }

    private string Link(string name, string id)
    {
        return "<color=#0000EE><u><link=" + id + ">" + name + "</link></u></color>";
    }

    // https://gist.github.com/unnanego/4492be640f7a4c560c9294b1aea4cf86 - changed
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, _camera);

        // No link clicked
        if (linkIndex == -1)
            return;

        if (linkIndex % 2 == 0)
            Application.OpenURL(soundAttributions[linkIndex / 2].link);
        else
            Application.OpenURL(GetLink(soundAttributions[linkIndex / 2].license));
    }

    private string GetLink(License license)
    {
        return license switch
        {
            License.CEATIVE_COMMONS_0 => "https://creativecommons.org/publicdomain/zero/1.0/",
            License.ATTRIBUTION_3_0 => "https://creativecommons.org/licenses/by/3.0/",
            License.ATTRIBUTION_4_0 => "https://creativecommons.org/licenses/by/4.0/",
            License.ATTRIBUTION_NC_4_0 => "https://creativecommons.org/licenses/by-nc/4.0/",
            _ => throw new ArgumentOutOfRangeException(nameof(license), license, null)
        };
    }
}