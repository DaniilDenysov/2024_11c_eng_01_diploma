using Mirror;
using System.Collections;
using UnityEngine;

namespace Shooting
{
    [RequireComponent(typeof(LineRenderer))]
    public class RaycastProjectile : MonoBehaviour
    {
        [SerializeField, Range(0, 100f)] private float _fadeSpeed = 1f;
        private LineRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<LineRenderer>();
        }

        public void Fire(Vector3 position, Vector3 hit)
        {
            _renderer.SetPosition(0, position);
            _renderer.SetPosition(1, hit);
            StartCoroutine(FadeLineRenderer());
        }

        private IEnumerator FadeLineRenderer()
        {
            Color initialColor = _renderer.material.color;
            float initialWidth = _renderer.startWidth;
            float timeElapsed = 0f;

            while (timeElapsed < _fadeSpeed)
            {
                float alpha = Mathf.Lerp(1f, 0f, timeElapsed / _fadeSpeed);
                float width = Mathf.Lerp(initialWidth, 0f, timeElapsed / _fadeSpeed); 

                Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
                _renderer.material.color = newColor;

                _renderer.startWidth = width;
                _renderer.endWidth = width;

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _renderer.startWidth = 0f;
            _renderer.endWidth = 0f;
            _renderer.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0);

            Destroy(gameObject);
        }
    }
}
