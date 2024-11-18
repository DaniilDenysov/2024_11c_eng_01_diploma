using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Messages
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private TMP_Text body, header;
        [SerializeField, Range(0, 100f)] private float disposeTime = 10f;
        [SerializeField] private UnityEvent<float> onMessageFade; 
        [SerializeField] private bool isDisposed = false;
        [SerializeField] private bool fadeOut = true;

        public Message Construct (string body)
        {
            this.body.text = body;
            return this;
        }

        public Message Construct(string body, string header = "")
        {
            if (!string.IsNullOrEmpty(header) && header != null) this.header.text = header;
            this.body.text = body;
            return this;
        }

        private async void Dispose()
        {

            while (disposeTime > 0 && !isDisposed)
            {
                disposeTime -= Time.deltaTime;
                onMessageFade?.Invoke(Mathf.Lerp(1,0,disposeTime));
                await Task.Yield();
            }
            isDisposed = true;
        }

        public void CloseWindowTemporary ()
        {
            gameObject.SetActive(false);
        }

        public void CloseWindow ()
        {
            isDisposed = true;
            Destroy(gameObject);
        }
    }
}
