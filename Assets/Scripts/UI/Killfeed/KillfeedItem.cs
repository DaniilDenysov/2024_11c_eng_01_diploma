using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kilfeed
{
    public class KillfeedItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text display;
        [SerializeField, Range(0,100f)] private float lifetime = 3f;

        public void Construct (string killer, string phrase, string victim)
        {
            display.text = $"{killer} {phrase} {victim}";
            Destroy(gameObject, lifetime);
        }
    }
}
