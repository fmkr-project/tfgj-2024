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
        private float _yoinkDuration = 0.11f;

        private Vector2 _initialAnchor;
        private Vector2 _targetAnchor;

        public void Awake()
        {
            _label = GetComponentInChildren<TextMeshProUGUI>();
            var border = transform.Find("Canvas/Border");
            _rectTransform = border.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _initialAnchor = new Vector2(_rectTransform.anchorMin.x, _rectTransform.anchorMin.y);
            _targetAnchor = new Vector2(_rectTransform.anchorMax.x, _rectTransform.anchorMin.y);
        }

        public IEnumerator Yoink(string newText)
        {
            var elapsed = 0f;
            var deltaTime = 0f;

            // Implement smootherstep.
            while (elapsed < _yoinkDuration)
            {
                deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _yoinkDuration;
                _rectTransform.anchorMin = Vector2.Lerp(
                    _initialAnchor, _targetAnchor,
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
                    _targetAnchor, _initialAnchor,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
            
            // Safeguard
            _rectTransform.anchorMin = _initialAnchor;
        }

        public IEnumerator Hide()
        {
            var elapsed = 0f;

            // Implement smootherstep.
            while (elapsed < _yoinkDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _yoinkDuration;
                _rectTransform.anchorMin = Vector2.Lerp(
                    _initialAnchor, _targetAnchor,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
            
            // Safeguard
            _rectTransform.anchorMin = _targetAnchor;
        }

        public IEnumerator Show()
        {
            // Synchronize animation
            var elapsed = 0f;
            while (elapsed < _yoinkDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                yield return new WaitForSeconds(deltaTime);
            }
            
            elapsed = 0f;
            while (elapsed < _yoinkDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _yoinkDuration;
                _rectTransform.anchorMin = Vector2.Lerp(
                    _targetAnchor, _initialAnchor,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }
            
            // Safeguard
            _rectTransform.anchorMin = _initialAnchor;
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
