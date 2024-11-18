using Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MessageLogManager : MonoBehaviour
    {
        public static MessageLogManager Instance;
        [SerializeField] private Transform messageContainer;
        [SerializeField] private Message messageWindowPrefab, messagePrefab;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public void DisplayMessage (string body)
        {
            GameObject instance = Instantiate(messageWindowPrefab.Construct(body).gameObject);
            instance.transform.SetParent(messageContainer);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        public void DisplayMessage(string body, string header)
        {
            GameObject instance = Instantiate(messageWindowPrefab.Construct(body).gameObject);
            instance.transform.SetParent(transform);
            instance.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        }
    }
}
