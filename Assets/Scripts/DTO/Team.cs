using CustomTools;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team
{
     public string Guid;
    [Range(0, 100)] public int maxPlayerAmount;

    public void Setup()
    {
       if (string.IsNullOrWhiteSpace(Guid) || string.IsNullOrEmpty(Guid))  Guid = System.Guid.NewGuid().ToString();
       // color = GenerateRandomColorAvoidingWhite();
    }

    private Color GenerateRandomColorAvoidingWhite()
    {
        const float minValue = 0.2f;
        const float maxValue = 0.8f;

        float r = Random.Range(minValue, maxValue);
        float g = Random.Range(minValue, maxValue);
        float b = Random.Range(minValue, maxValue);

        return new Color(r, g, b);
    }
}
