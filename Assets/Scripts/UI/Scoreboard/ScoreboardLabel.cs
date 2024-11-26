using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Score
{
    public class ScoreboardLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameDisplay, killCountDisplay, assistsCountDisplay, deathsCountDisplay; 
        
        public void Construct (string playerName, int killCount, int assistsCount, int deathsCount)
        {
            playerNameDisplay.text = playerName;
            killCountDisplay.text = $"{killCount}";
            assistsCountDisplay.text = $"{assistsCount}";
            deathsCountDisplay.text = $"{deathsCount}";
        }
    }
}
