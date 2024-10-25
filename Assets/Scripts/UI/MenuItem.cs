using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MenuItem : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private TextMeshProUGUI _label;
        private float _yoinkDuration = 0.2f;

        public void Awake()
        {
            _label = GetComponentInChildren<TextMeshProUGUI>();
            var border = transform.Find("Canvas/Border");
            _rectTransform = border.GetComponent<RectTransform>();
        }

        public IEnumerator Yoink(string newText)
        {
            var initialAnchor = new Vector2(_rectTransform.anchorMin.x, _rectTransform.anchorMin.y);
            var targetAnchor = new Vector2(_rectTransform.anchorMax.x, _rectTransform.anchorMin.y);
            var elapsed = 0f;
            var deltaTime = 0f;

            // Implement smootherstep.
            while (elapsed < _yoinkDuration)
            {
                deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _yoinkDuration;
                _rectTransform.anchorMin = Vector2.Lerp(
                    initialAnchor, targetAnchor,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            SetLabel(newText);
            
            elapsed = 0;
            while (elapsed < _yoinkDuration)
            {
                deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _yoinkDuration;
                _rectTransform.anchorMin = Vector2.Lerp(
                    targetAnchor, initialAnchor,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
            
            // Safeguard
            _rectTransform.anchorMin = initialAnchor;
        }

        public IEnumerator Hide()
        {
            var initialAnchor = new Vector2(_rectTransform.anchorMin.x, _rectTransform.anchorMin.y);
            var targetAnchor = new Vector2(_rectTransform.anchorMax.x, _rectTransform.anchorMin.y);
            var elapsed = 0f;

            // Implement smootherstep.
            while (elapsed < _yoinkDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _yoinkDuration;
                _rectTransform.anchorMin = Vector2.Lerp(
                    initialAnchor, targetAnchor,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
        }

        public void SetLabel(string text)
        {
            _label.SetText(text);
        }

        public void SetStartYAnchor(float yAnchor)
        {
            _rectTransform.anchorMin = new Vector2(0.75f, yAnchor);
            _rectTransform.anchorMax = new Vector2(0.95f, yAnchor + 0.1f);
        }
    }
}
