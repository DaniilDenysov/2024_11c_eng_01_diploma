using Mirror;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HealthSystem
{
    /// <summary>
    /// Represents a custom slider with current and old value visualization.
    /// </summary>
    public class CustomSlider : NetworkBehaviour
    {
        [SerializeField] private Image currentValueImage;
        [SerializeField] private Image oldValueImage;
        [SerializeField] private Image backgroundImage;

        [SerializeField][Range(0, float.MaxValue)] private float maxValue = 100f;
        [SerializeField,Range(0, float.MaxValue)] private float currentValue = 100f;
        [SerializeField][Range(0, float.MaxValue)] private float speed = 5f;
        [SerializeField] [Range(0, 1f)] private float fadeInTimer = 0.5f;
        [SerializeField] [Range(0, 1f)] private float fadeOutTimer = 0.5f;


        [SerializeField] [Range(0, float.MaxValue)] private float holdTime = 4f;

        private float oldValue;
        private float lerpTime;

        [SerializeField] private bool eneableFading = true;
        private bool isFadeIn = false;
        private bool isFadeOut = false;
        private float lastShowed = 0f;

        /// <summary>
        /// Initializes the slider values.
        /// </summary>
        private void Awake()
        {
            oldValue = currentValue;
            lastShowed = -holdTime;
            if (eneableFading)
                Transparency(0f);
        }

        /// <summary>
        /// Gets the current value of the slider.
        /// </summary>
        /// <returns>The current value.</returns>
        public float GetCurrentValue() => currentValue;

        /// <summary>
        /// Gets the maximum value of the slider.
        /// </summary>
        /// <returns>The maximum value.</returns>
        public float GetMaxValue() => maxValue;

        private void OnValueChanged (float oldValue, float newValue)
        {

        }

        /// <summary>
        /// Sets the current value of the slider.
        /// </summary>
        /// <param name="value">The new value to set.</param>
        [ClientRpc]
        public void SetCurrentValue(float value)
        {
            SetCurrentValueWithoutNotification(value);

            if (eneableFading && !isFadeIn)
            {
                lastShowed = Time.time;
                FadeIn();
            }
            
        }

        [ClientRpc]
        public void SetCurrentValueWithoutNotification(float value)
        {
            currentValue = Mathf.Clamp(value, 0, maxValue);
            lerpTime = 0f;
        }


        /// <summary>
        /// Updates the old value to smoothly transition to the current value.
        /// </summary>
        private void Update()
        {
            if (currentValue != oldValue)
            {
                oldValue = Mathf.Lerp(oldValue, currentValue, lerpTime);
                lerpTime += speed * Time.deltaTime;
                UpdateVisuals();
            }

            if (lastShowed + holdTime <= Time.time)
            {
                if (eneableFading && isFadeIn)
                {
                    FadeOut();
                }
            }
        }

        /// <summary>
        /// Updates the visual representation of the slider.
        /// </summary>
        private void UpdateVisuals()
        {
            currentValueImage.fillAmount = currentValue / maxValue;
            oldValueImage.fillAmount = oldValue / maxValue;
        }

        private async void FadeIn()
        {
            isFadeIn = true;
            isFadeOut = false;
            float fadeTimer = 0f;
            float value = 0f;
            while (value < 1f)
            {
                fadeTimer += Time.deltaTime;
                value = Mathf.Lerp(0f, 1f, fadeTimer / fadeInTimer);
                Transparency(value);
                await Task.Yield();
            }

        }

        private async void FadeOut()
        {
            isFadeIn = false;
            isFadeOut = true;
            float fadeTimer = 0f;
            float value = 1f;
            while (value > 0f)
            {
                fadeTimer += Time.deltaTime;
                value = Mathf.Lerp(1f, 0f, fadeTimer / fadeOutTimer);
                Transparency(value);
                await Task.Yield();
            }
        }

       

        /// <summary>
        /// Adjusts the val transparency of both images.
        /// </summary>
        private void Transparency(float val) {
            var currentColor = currentValueImage.color;
            currentColor.a = val;
            currentValueImage.color = currentColor;
            var oldColor = oldValueImage.color;
            oldColor.a = val;
            oldValueImage.color = oldColor;

            var backgroundImageColor = backgroundImage.color;
            backgroundImageColor.a = val;
            backgroundImage.color = backgroundImageColor;
        }

    }
}

// async version, mb could be useful.
/*
  public class CustomSlider : MonoBehaviour
    {
        [SerializeField] private Image currentValueImage;
        [SerializeField] private Image oldValueImage;

        [SerializeField] [Range(0, float.MaxValue)] private float maxValue = 100f;
        [SerializeField] [Range(0, float.MaxValue)] private float speed = 5f;

        private float currentValue;
        private float oldValue;
        private Coroutine updateCoroutine;

        private void Awake()
        {
            currentValue = maxValue;
            oldValue = currentValue;
        }

        public float GetCurrentValue() => currentValue;
        public float GetMaxValue() => maxValue;

        public void SetCurrentValue(float value)
        {
            currentValue = Mathf.Clamp(value, 0, maxValue);
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
            }
            updateCoroutine = StartCoroutine(UpdateValueSmoothly());
        }

        private IEnumerator UpdateValueSmoothly()
        {
            float lerpTime = 0f;
            while (currentValue != oldValue)
            {
                oldValue = Mathf.Lerp(oldValue, currentValue, lerpTime);
                lerpTime += speed * Time.deltaTime;
                UpdateVisuals();
                yield return null;
            }
        }

        private void UpdateVisuals()
        {
            currentValueImage.fillAmount = currentValue / maxValue;
            oldValueImage.fillAmount = oldValue / maxValue;
        }
    }
 */