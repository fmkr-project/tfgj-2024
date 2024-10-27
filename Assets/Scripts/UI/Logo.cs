using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public class Logo : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private RectTransform _keineTransform;
        private Vector2 _initialPosition;
        private Vector2 _keineInitialPosition;
        
        private float _animationDuration = 0.3f;
        private float _animationOffset = 2000f;

        private void Awake()
        {
            _rectTransform = transform.Find("Canvas/Image").GetComponent<RectTransform>();
            _keineTransform = transform.Find("Canvas/Keine").GetComponent<RectTransform>();
            _initialPosition = _rectTransform.localPosition;
            _keineInitialPosition = _keineTransform.localPosition;
        }

        public IEnumerator Show()
        {
            // You know the drill.
            var elapsed = 0f;
            
            while (elapsed < _animationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _animationDuration;
                _rectTransform.localPosition = Vector2.Lerp(
                    _initialPosition + _animationOffset * Vector2.left,
                    _initialPosition,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                _keineTransform.localPosition = Vector2.Lerp(
                    _keineInitialPosition + _animationOffset * Vector2.left,
                    _keineInitialPosition,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _rectTransform.localPosition = _initialPosition;
            _keineTransform.localPosition = _keineInitialPosition;
        }
        
        public IEnumerator Hide()
        {
            var elapsed = 0f;
            
            while (elapsed < _animationDuration)
            {
                var deltaTime = Time.deltaTime;
                elapsed += deltaTime;
                var t = elapsed / _animationDuration;
                _rectTransform.localPosition = Vector2.Lerp(
                    _initialPosition,
                    _initialPosition + _animationOffset * Vector2.left,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                _keineTransform.localPosition = Vector2.Lerp(
                    _keineInitialPosition,
                    _keineInitialPosition + _animationOffset * Vector2.left,
                    (float) (6 * Math.Pow(t, 5) - 15 * Math.Pow(t, 4) + 10 * Math.Pow(t, 3)));
                yield return new WaitForSeconds(deltaTime);
            }

            _rectTransform.localPosition = _initialPosition + _animationOffset * Vector2.left;
            _keineTransform.localPosition = _keineInitialPosition + _animationOffset * Vector2.left;
        }
    }
}