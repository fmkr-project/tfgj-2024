using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Fader : MonoBehaviour
    {
        private Image _bg;
        private Color _defaultColor = Color.black;

        private void Awake()
        {
            _bg = GetComponent<Image>();
            // Faders start black and activated
            _bg.color = _defaultColor;
        }

        public void SetDefaultColor(Color c)
        {
            _defaultColor = c;
        }

        public IEnumerator FadeOut(float duration)
        {
            var elapsed = 0f;
            var deltaTime = 0f;
            while (elapsed < duration)
            {
                deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var targetColor = _defaultColor;
                targetColor.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                _bg.color = targetColor;
                yield return new WaitForSeconds(deltaTime);
            }
        }

        public IEnumerator FadeIn(float duration)
        {
            var elapsed = 0f;
            var deltaTime = 0f;
            while (elapsed < duration)
            {
                deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var targetColor = _defaultColor;
                targetColor.a = Mathf.Lerp(1f, 0f, elapsed / duration);
                _bg.color = targetColor;
                yield return new WaitForSeconds(deltaTime);
            }
        }
    }
}
